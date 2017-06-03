using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncUtilities
{
    /// <summary>
    /// <see cref="StripedAsyncLock{TKey}"/> divides an <see cref="AsyncLock"/> into granular
    /// stripes allowing different operations to lock separate stripes concurrently instead
    /// of locking the entire <see cref="AsyncLock"/> altogether
    /// </summary>
    /// <typeparam name="TKey">
    /// The type of the keys the stripes correspond to.
    /// </typeparam>
    public class StripedAsyncLock<TKey>
    {
        private readonly Striped<TKey, AsyncLock> _striped;

        /// <summary>
        /// Initializes a new <see cref="StripedAsyncLock{TKey}"/> instance with the
        /// specified number of stripes.
        /// </summary>
        /// <param name="stripes">
        /// The amount of stripes to divide the <see cref="AsyncLock"/> into.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="stripes"/> is less than 1.
        /// </exception>
        public StripedAsyncLock(int stripes)
            : this(stripes, null)
        {
        }

        /// <summary>
        /// Initializes a new <see cref="StripedAsyncLock{TKey}"/> instance with the
        /// specified number of stripes that uses the specified <see cref="IEqualityComparer{TKey}"/>.
        /// </summary>
        /// <param name="stripes">
        /// The amount of stripes to divide the <see cref="AsyncLock"/> into.
        /// </param>
        /// <param name="comparer">
        /// The <see cref="IEqualityComparer{TKey}"/> implementation to use when generating
        /// a hash code in order to find a stripe.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="stripes"/> is less than 1.
        /// </exception>
        public StripedAsyncLock(int stripes, IEqualityComparer<TKey> comparer)
        {
            if (stripes <= 0) throw new ArgumentOutOfRangeException(nameof(stripes));

            _striped = Striped.Create(stripes, () => new AsyncLock(), comparer);
        }

        /// <summary>
        /// Asynchronously locks the <see cref="StripedAsyncLock{TKey}"/>.
        /// </summary>
        /// <param name="key">
        /// The key corresponding to the striped <see cref="AsyncLock"/>.
        /// </param>
        /// <returns>
        /// A <see cref="ValueTask"/> task that will complete when the <see cref="StripedAsyncLock{TKey}"/>
        /// has been taken with a <see cref="Releaser"/> result. Disposing of the <see cref="Releaser"/>
        /// will release the <see cref="StripedAsyncLock{TKey}"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="key"/> is null.
        /// </exception>
        public ValueTask<Releaser> LockAsync(TKey key) => 
            LockAsync(key, CancellationToken.None);

        /// <summary>
        /// Asynchronously locks the <see cref="StripedAsyncLock{TKey}"/>, while observing a
        /// <see cref="CancellationToken"/>.
        /// </summary>
        /// <param name="key">
        /// The key corresponding to the striped <see cref="AsyncLock"/>.
        /// </param>
        /// <param name="cancellationToken">
        /// The <see cref="CancellationToken"/> token to observe.
        /// </param>
        /// <returns>
        /// A <see cref="ValueTask"/> task that will complete when the <see cref="StripedAsyncLock{TKey}"/>
        /// has been taken with a <see cref="Releaser"/> result. Disposing of the <see cref="Releaser"/>
        /// will release the <see cref="StripedAsyncLock{TKey}"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="key"/> is null.
        /// </exception>
        public async ValueTask<Releaser> LockAsync(TKey key, CancellationToken cancellationToken)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));

            return new Releaser(await _striped.GetLock(key).LockAsync(cancellationToken));
        }

        /// <summary>
        /// <see cref="Releaser"/> enables holding an <see cref="StripedAsyncLock{TKey}"/> with a using scope.
        /// </summary>
        public struct Releaser : IDisposable
        {
            private readonly AsyncLock.Releaser _releaser;

            internal Releaser(AsyncLock.Releaser releaser) => 
                _releaser = releaser;

            /// <summary>
            /// Releases the held <see cref="StripedAsyncLock{TKey}"/>.
            /// </summary>
            public void Dispose() => 
                _releaser.Dispose();
        }
    }
}