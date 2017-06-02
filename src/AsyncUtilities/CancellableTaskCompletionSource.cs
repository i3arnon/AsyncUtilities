using System.Threading;
using System.Threading.Tasks;

namespace AsyncUtilities
{
    /// <summary>
    /// Represents a <see cref="TaskCompletionSource{TResult}"/> associated with a <see cref="CancellationToken"/>.
    /// Canceling the <see cref="CancellationToken"/> will cancel the <see cref="CancellableTaskCompletionSource{TResult}"/>.
    /// </summary>
    /// <typeparam name="TResult">
    /// The type of the result value associated with this <see cref="CancellableTaskCompletionSource{TResult}"/>.
    /// </typeparam>
    public class CancellableTaskCompletionSource<TResult> : TaskCompletionSource<TResult>
    {
        private readonly CancellationTokenRegistration _registration;

        /// <summary>
        /// Gets the <see cref="CancellationToken"/> associated with this <see cref="CancellableTaskCompletionSource{TResult}"/>.
        /// </summary>
        public CancellationToken CancellationToken { get; }

        /// <summary>
        /// Creates a <see cref="CancellableTaskCompletionSource{TResult}"/>.
        /// </summary>
        /// <param name="cancellationToken">
        /// The <see cref="CancellationToken"/> to associate with the <see cref="CancellableTaskCompletionSource{TResult}"/>.
        /// </param>
        public CancellableTaskCompletionSource(CancellationToken cancellationToken)
            : this(cancellationToken, null, TaskCreationOptions.None)
        {
        }

        /// <summary>
        /// Creates a <see cref="CancellableTaskCompletionSource{TResult}"/>.
        /// </summary>
        /// <param name="cancellationToken">
        /// The <see cref="CancellationToken"/> to associate with the <see cref="CancellableTaskCompletionSource{TResult}"/>.
        /// </param>
        /// <param name="creationOptions">
        /// The options to use when creating the underlying <see cref="Task{TResult}"/>.
        /// </param>
        public CancellableTaskCompletionSource(CancellationToken cancellationToken, TaskCreationOptions creationOptions)
            : this(cancellationToken, null, creationOptions)
        {
        }


        /// <summary>
        /// Creates a <see cref="CancellableTaskCompletionSource{TResult}"/>.
        /// </summary>
        /// <param name="cancellationToken">
        /// The <see cref="CancellationToken"/> to associate with the <see cref="CancellableTaskCompletionSource{TResult}"/>.
        /// </param>
        /// <param name="state">
        /// The state to use as the underlying <see cref="Task{TResult}"/>'s AsyncState.
        /// </param>
        public CancellableTaskCompletionSource(CancellationToken cancellationToken, object state)
            : this(cancellationToken, state, TaskCreationOptions.None)
        {
        }

        /// <summary>
        /// Creates a <see cref="CancellableTaskCompletionSource{TResult}"/>.
        /// </summary>
        /// <param name="cancellationToken">
        /// The <see cref="CancellationToken"/> to associate with the <see cref="CancellableTaskCompletionSource{TResult}"/>.
        /// </param>
        /// <param name="state">
        /// The state to use as the underlying <see cref="Task{TResult}"/>'s AsyncState.
        /// </param>
        /// <param name="creationOptions">
        /// The options to use when creating the underlying <see cref="Task{TResult}"/>.
        /// </param>
        public CancellableTaskCompletionSource(
            CancellationToken cancellationToken,
            object state,
            TaskCreationOptions creationOptions)
            : base(state, creationOptions)
        {
            CancellationToken = cancellationToken;

            if (!cancellationToken.CanBeCanceled)
            {
                return;
            }

            _registration = cancellationToken.Register(
                @this => ((CancellableTaskCompletionSource<TResult>)@this).TrySetCanceled(),
                this);

            Task.ContinueWithSynchronously(
                (_, @this) => ((CancellableTaskCompletionSource<TResult>)@this)._registration.Dispose(),
                this);
        }
    }
}