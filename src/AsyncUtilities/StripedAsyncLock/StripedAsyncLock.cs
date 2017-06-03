using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncUtilities
{
    public class StripedAsyncLock<TKey>
    {
        private readonly Striped<TKey, AsyncLock> _striped;

        public StripedAsyncLock(int stripes)
            : this(stripes, null)
        {
        }

        public StripedAsyncLock(int stripes, IEqualityComparer<TKey> comparer)
        {
            if (stripes <= 0) throw new ArgumentOutOfRangeException(nameof(stripes));

            _striped = Striped.Create(stripes, () => new AsyncLock(), comparer);
        }

        public ValueTask<Releaser> LockAsync(TKey key) => 
            LockAsync(key, CancellationToken.None);

        public async ValueTask<Releaser> LockAsync(TKey key, CancellationToken cancellationToken)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));

            return new Releaser(await _striped.GetLock(key).LockAsync(cancellationToken));
        }

        public struct Releaser : IDisposable
        {
            private readonly AsyncLock.Releaser _releaser;

            internal Releaser(AsyncLock.Releaser releaser) => 
                _releaser = releaser;

            public void Dispose() => 
                _releaser.Dispose();
        }
    }
}