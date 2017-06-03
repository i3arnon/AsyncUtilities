using System;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncUtilities
{
    public class AsyncLock
    {
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

        public ValueTask<Releaser> LockAsync() => 
            LockAsync(CancellationToken.None);

        public async ValueTask<Releaser> LockAsync(CancellationToken cancellationToken)
        {
            await _semaphore.WaitAsync(cancellationToken);
            return new Releaser(this);
        }

        public struct Releaser : IDisposable
        {
            private readonly AsyncLock _asyncLock;

            private bool _isDisposed;

            internal Releaser(AsyncLock asyncLock)
            {
                _asyncLock = asyncLock;
                _isDisposed = false;
            }

            public void Dispose()
            {
                if (_isDisposed)
                {
                    return;
                }

                _asyncLock?._semaphore.Release();
                _isDisposed = true;
            }
        }
    }
}
