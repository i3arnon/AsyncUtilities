using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace AsyncUtilities
{
    /// <summary>
    /// <see cref="Striped"/> divides a locking mechanism into granular 
    /// stripes allowing different operations to hold separate stripes concurrently instead
    /// of holding the entire locking mechanism altogether.
    /// </summary>
    public static class Striped
    {
        /// <summary>
        /// Create a new <see cref="Striped{TKey,TLock}"/> instance with the specified 
        /// amount of stripes.
        /// </summary>
        /// <typeparam name="TKey">
        /// The type of the keys the stripes correspond to.
        /// </typeparam>
        /// <typeparam name="TLock">
        /// The type of the locking mechanism to stripe.
        /// </typeparam>
        /// <param name="stripes">
        /// The amount of stripes to divide the <typeparamref name="TLock"/> into.
        /// </param>
        /// <returns>
        /// A new <see cref="Striped{TKey,TLock}"/> instance with the specified arguments.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="stripes"/> is less than 1.
        /// </exception>
        public static Striped<TKey, TLock> Create<TKey, TLock>(int stripes)
            where TLock : class, new() =>
            Create<TKey, TLock>(stripes, () => new TLock());

        /// <summary>
        /// Create a new <see cref="Striped{TKey,TLock}"/> instance with the specified 
        /// amount of stripes, that creates new locking mechanism instances using the 
        /// creatorFunction.
        /// </summary>
        /// <typeparam name="TKey">
        /// The type of the keys the stripes correspond to.
        /// </typeparam>
        /// <typeparam name="TLock">
        /// The type of the locking mechanism to stripe.
        /// </typeparam>
        /// <param name="stripes">
        /// The amount of stripes to divide the <typeparamref name="TLock"/> into.
        /// </param>
        /// <param name="creatorFunction">
        /// The function to create new <typeparamref name="TLock"/> instances.
        /// </param>
        /// <returns>
        /// A new <see cref="Striped{TKey,TLock}"/> instance with the specified arguments.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="stripes"/> is less than 1.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="creatorFunction"/> is null.
        /// </exception>
        public static Striped<TKey, TLock> Create<TKey, TLock>(
            int stripes,
            Func<TLock> creatorFunction)
            where TLock : class =>
            Create<TKey, TLock>(stripes, creatorFunction, comparer: null);

        /// <summary>
        /// Create a new <see cref="Striped{TKey,TLock}"/> instance with the specified 
        /// amount of stripes, that creates new locking mechanism instances using the 
        /// creatorFunction and uses the specified <see cref="IEqualityComparer{TKey}"/>.
        /// </summary>
        /// <typeparam name="TKey">
        /// The type of the keys the stripes correspond to.
        /// </typeparam>
        /// <typeparam name="TLock">
        /// The type of the locking mechanism to stripe.
        /// </typeparam>
        /// <param name="stripes">
        /// The amount of stripes to divide the <typeparamref name="TLock"/> into.
        /// </param>
        /// <param name="creatorFunction">
        /// The function to create new <typeparamref name="TLock"/> instances.
        /// </param>
        /// <param name="comparer">
        /// The <see cref="IEqualityComparer{TKey}"/> implementation to use when generating
        /// a hash code in order to find a stripe.
        /// </param>
        /// <returns>
        /// A new <see cref="Striped{TKey,TLock}"/> instance with the specified arguments.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="stripes"/> is less than 1.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="creatorFunction"/> is null.
        /// </exception>
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

    /// <summary>
    /// <see cref="Striped{TKey,TLock}"/> divides a <typeparamref name="TLock"/> into granular 
    /// stripes allowing different operations to hold separate stripes concurrently instead 
    /// of holding the entire <typeparamref name="TLock"/> altogether.
    /// </summary>
    /// <typeparam name="TKey">
    /// The type of the keys the stripes correspond to.
    /// </typeparam>
    /// <typeparam name="TLock">
    /// The type of the locking mechanism to stripe.
    /// </typeparam>
    public abstract class Striped<TKey, TLock> where TLock : class
    {
        private static readonly TLock[] _emptyArray = new TLock[0];

        private readonly IEqualityComparer<TKey> _comparer;

        /// <summary>
        /// The function to create new <typeparamref name="TLock"/> instances.
        /// </summary>
        protected readonly Func<TLock> _creatorFunction;
        /// <summary>
        /// The mask used to generate a stripe's index.
        /// </summary>
        protected readonly int _stripeMask;

        /// <summary>
        /// Gets the striped <typeparamref name="TLock"/> instances in the 
        /// <see cref="Striped{TKey,TLock}"/>.
        /// </summary>
        /// <returns>
        /// An <see cref="IEnumerable{TLock}"/> containing the striped <typeparamref name="TLock"/> 
        /// instances in the <see cref="Striped{TKey,TLock}"/>/>
        /// </returns>
        public abstract IEnumerable<TLock> Locks { get; }

        /// <summary>
        /// Gets the striped <typeparamref name="TLock"/> the key corresponds to.
        /// </summary>
        /// <param name="key">
        /// The key corresponding to the striped <typeparamref name="TLock"/>.
        /// </param>
        /// <returns>
        /// The striped <typeparamref name="TLock"/> the key corresponds to.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="key"/> is null.
        /// </exception>
        public TLock this[TKey key]
        {
            get
            {
                if (key == null) throw new ArgumentNullException(nameof(key));

                return GetLock(key);
            }
        }

        /// <summary>
        /// Create a new <see cref="Striped{TKey,TLock}"/> instance with the specified 
        /// amount of stripes, that creates new locking mechanism instances using the 
        /// creatorFunction and uses the specified <see cref="IEqualityComparer{TKey}"/>.
        /// </summary>
        /// <param name="stripes">
        /// The amount of stripes to divide the <typeparamref name="TLock"/> into.
        /// </param>
        /// <param name="creatorFunction">
        /// The function to create new <typeparamref name="TLock"/> instances.
        /// </param>
        /// <param name="comparer">
        /// The <see cref="IEqualityComparer{TKey}"/> implementation to use when generating
        /// a hash code in order to find a stripe.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="stripes"/> is less than 1.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="creatorFunction"/> is null.
        /// </exception>
        protected Striped(
            int stripes,
            Func<TLock> creatorFunction,
            IEqualityComparer<TKey> comparer)
        {
            if (stripes <= 0) throw new ArgumentOutOfRangeException(nameof(stripes));

            _creatorFunction = creatorFunction ?? throw new ArgumentNullException(nameof(creatorFunction));
            _comparer = comparer ?? EqualityComparer<TKey>.Default;
            _stripeMask = GetStripeMask(stripes);
        }

        /// <summary>
        /// Gets the striped <typeparamref name="TLock"/> the key corresponds to.
        /// </summary>
        /// <param name="key">
        /// The key corresponding to the striped <typeparamref name="TLock"/>.
        /// </param>
        /// <returns>
        /// The striped <typeparamref name="TLock"/> the key corresponds to.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="key"/> is null.
        /// </exception>
        public TLock GetLock(TKey key)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));

            return GetLock(GetStripe(key));
        }

        /// <summary>
        /// Gets the striped <typeparamref name="TLock"/>s the keys correspond to in a sorted order.
        /// </summary>
        /// <param name="keys">
        /// The keys corresponding to the striped <typeparamref name="TLock"/>s.
        /// </param>
        /// <returns>
        /// The striped <typeparamref name="TLock"/>s the keys corresponds to in a sorted order.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="keys"/> is null.
        /// </exception>
        public IEnumerable<TLock> GetLocks(params TKey[] keys)
        {
            if (keys == null) throw new ArgumentNullException(nameof(keys));

            if (keys.Length == 0)
            {
                return _emptyArray;
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

        /// <summary>
        /// Gets the striped <typeparamref name="TLock"/> the key corresponds to.
        /// </summary>
        /// <param name="stripe">
        /// The index corresponding to the striped <typeparamref name="TLock"/>.
        /// </param>
        /// <returns>
        /// The striped <typeparamref name="TLock"/> the key corresponds to.
        /// </returns>
        protected abstract TLock GetLock(int stripe);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int GetStripe(TKey key)
        {
            var hashCode = _comparer.GetHashCode(key) & int.MaxValue;
            return SmearHashCode(hashCode) & _stripeMask;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int SmearHashCode(int hashCode)
        {
            hashCode ^= (hashCode >> 20) ^ (hashCode >> 12);
            return hashCode ^ (hashCode >> 7) ^ (hashCode >> 4);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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