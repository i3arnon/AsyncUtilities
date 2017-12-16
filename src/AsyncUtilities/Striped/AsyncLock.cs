using System;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncUtilities
{
    /// <summary>
    /// An asynchronous locking mechanism.
    /// </summary>
    public class AsyncLock
    {
        private readonly SemaphoreSlim _semaphore;

        /// <summary>
        /// Creates a new <see cref="AsyncLock"/> instance.
        /// </summary>
        public AsyncLock() => 
            _semaphore = new SemaphoreSlim(initialCount: 1, maxCount: 1);

        /// <summary>
        /// Asynchronously locks the <see cref="AsyncLock"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="ValueTask"/> task that will complete when the <see cref="AsyncLock"/> 
        /// has been taken with a <see cref="Releaser"/> result.  Disposing of the <see cref="Releaser"/> 
        /// will release the <see cref="AsyncLock"/>.
        /// </returns>
        public ValueTask<Releaser> LockAsync() => 
            LockAsync(CancellationToken.None);

        /// <summary>
        /// Asynchronously locks the <see cref="AsyncLock"/>, while observing a
        /// <see cref="CancellationToken"/>.
        /// </summary>
        /// <param name="cancellationToken">
        /// The <see cref="CancellationToken"/> token to observe.
        /// </param>
        /// <returns>
        /// A <see cref="ValueTask"/> task that will complete when the <see cref="AsyncLock"/> 
        /// has been taken with a <see cref="Releaser"/> result.  Disposing of the <see cref="Releaser"/> 
        /// will release the <see cref="AsyncLock"/>.
        /// </returns>
        public async ValueTask<Releaser> LockAsync(CancellationToken cancellationToken)
        {
            await _semaphore.WaitAsync(cancellationToken);
            return new Releaser(this);
        }

        /// <summary>
        /// <see cref="Releaser"/> enables holding an <see cref="AsyncLock"/> with a using scope.
        /// </summary>
        public struct Releaser : IDisposable
        {
            private readonly AsyncLock _asyncLock;

            private bool _isDisposed;

            internal Releaser(AsyncLock asyncLock)
            {
                _asyncLock = asyncLock;
                _isDisposed = false;
            }

            /// <summary>
            /// Releases the held <see cref="AsyncLock"/>.
            /// </summary>
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
