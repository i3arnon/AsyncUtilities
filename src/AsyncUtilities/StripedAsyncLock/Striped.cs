using System;
using System.Collections.Generic;

namespace AsyncUtilities
{
    public static class Striped
    {
        public static Striped<TKey, TLock> Create<TKey, TLock>(int stripes)
            where TLock : class, new() =>
            Create<TKey, TLock>(stripes, () => new TLock());

        public static Striped<TKey, TLock> Create<TKey, TLock>(
            int stripes,
            Func<TLock> creatorFunction)
            where TLock : class =>
            Create<TKey, TLock>(stripes, creatorFunction, null);

        public static Striped<TKey, TLock> Create<TKey, TLock>(
            int stripes,
            Func<TLock> creatorFunction,
            IEqualityComparer<TKey> comparer)
            where TLock : class
        {
            if (stripes <= 0) throw new ArgumentOutOfRangeException(nameof(stripes));
            if (creatorFunction == null) throw new ArgumentNullException(nameof(creatorFunction));

            return new SimpleStriped<TKey, TLock>(stripes, creatorFunction, comparer);
        }
    }

    public abstract class Striped<TKey, TLock> where TLock : class
    {
        private readonly IEqualityComparer<TKey> _comparer;

        protected readonly Func<TLock> _creatorFunction;
        protected readonly int _stripeMask;

        protected Striped(
            int stripes,
            Func<TLock> creatorFunction,
            IEqualityComparer<TKey> comparer)
        {
            _creatorFunction = creatorFunction;
            _comparer = comparer ?? EqualityComparer<TKey>.Default;
            _stripeMask = GetStripeMask(stripes);
        }

        public TLock GetLock(TKey key)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));

            return GetLock(GetStripe(key));
        }

        public IEnumerable<TLock> GetLocks(params TKey[] keys)
        {
            if (keys == null) throw new ArgumentNullException(nameof(keys));

            if (keys.Length == 0)
            {
                return new TLock[0];
            }

            var stripes = new int[keys.Length];
            for (var index = 0; index < keys.Length; index++)
            {
                stripes[index] = GetStripe(keys[index]);
            }

            Array.Sort(stripes);

            var locks = new TLock[stripes.Length];
            var lastStripe = stripes[0];
            locks[0] = GetLock(lastStripe);
            for (var index = 1; index < stripes.Length; index++)
            {
                var currentStripe = stripes[index];
                if (currentStripe == lastStripe)
                {
                    locks[index] = locks[index - 1];
                }
                else
                {
                    locks[index] = GetLock(currentStripe);
                    lastStripe = currentStripe;
                }
            }

            return locks;
        }

        protected abstract TLock GetLock(int stripe);

        private int GetStripe(TKey key)
        {
            var hashCode = _comparer.GetHashCode(key) & 0x7FFF_FFFF;
            return SmearHashCode(hashCode) & _stripeMask;
        }

        private int SmearHashCode(int hashCode)
        {
            hashCode ^= (hashCode >> 20) ^ (hashCode >> 12);
            return hashCode ^ (hashCode >> 7) ^ (hashCode >> 4);
        }

        private int GetStripeMask(int stripes)
        {
            stripes |= stripes >> 1;
            stripes |= stripes >> 2;
            stripes |= stripes >> 4;
            stripes |= stripes >> 8;
            stripes |= stripes >> 16;
            return stripes;
        }
    }
}