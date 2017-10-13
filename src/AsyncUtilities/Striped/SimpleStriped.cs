using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace AsyncUtilities
{
    internal class SimpleStriped<TKey, TLock> : Striped<TKey, TLock>
        where TLock : class
    {
        private readonly TLock[] _locks;

        private ReadOnlyCollection<TLock> _locksReadOnlyWrapper;

        public override IEnumerable<TLock> Locks =>
            _locksReadOnlyWrapper ?? (_locksReadOnlyWrapper = new ReadOnlyCollection<TLock>(_locks));

        public SimpleStriped(
            int stripes,
            Func<TLock> creatorFunction,
            IEqualityComparer<TKey> comparer)
            : base(stripes, creatorFunction, comparer)
        {
            _locks = new TLock[_stripeMask + 1];
            for (var index = 0; index < _locks.Length; index++)
            {
                _locks[index] = _creatorFunction();
            }
        }

        protected override TLock GetLock(int stripe) =>
            _locks[stripe];
    }
}