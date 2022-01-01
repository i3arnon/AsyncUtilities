using System.Threading;
using System.Threading.Tasks;
using static System.Threading.Tasks.TaskCreationOptions;

namespace AsyncUtilities
{
    /// <summary>
    /// Represents a <see cref="TaskCompletionSource{TResult}"/> associated with a <see cref="CancellationToken"/>.  
    /// Canceling the <see cref="CancellationToken"/> will cancel the <see cref="CancelableTaskCompletionSource{TResult}"/>.
    /// </summary>
    /// <typeparam name="TResult">
    /// The type of the result value associated with this <see cref="CancelableTaskCompletionSource{TResult}"/>.
    /// </typeparam>
    public class CancelableTaskCompletionSource<TResult> : TaskCompletionSource<TResult>
    {
        private readonly CancellationTokenRegistration _registration;

        /// <summary>
        /// Gets the <see cref="CancellationToken"/> associated with this <see cref="CancelableTaskCompletionSource{TResult}"/>.
        /// </summary>
        public CancellationToken CancellationToken { get; }

        /// <summary>
        /// Creates a <see cref="CancelableTaskCompletionSource{TResult}"/>.
        /// </summary>
        /// <param name="cancellationToken">
        /// The <see cref="CancellationToken"/> to associate with the <see cref="CancelableTaskCompletionSource{TResult}"/>.
        /// </param>
        public CancelableTaskCompletionSource(CancellationToken cancellationToken)
            : this(cancellationToken, state: null, None)
        {
        }

        /// <summary>
        /// Creates a <see cref="CancelableTaskCompletionSource{TResult}"/>.
        /// </summary>
        /// <param name="cancellationToken">
        /// The <see cref="CancellationToken"/> to associate with the <see cref="CancelableTaskCompletionSource{TResult}"/>.
        /// </param>
        /// <param name="creationOptions">
        /// The options to use when creating the underlying <see cref="Task{TResult}"/>.
        /// </param>
        public CancelableTaskCompletionSource(CancellationToken cancellationToken, TaskCreationOptions creationOptions)
            : this(cancellationToken, state: null, creationOptions)
        {
        }


        /// <summary>
        /// Creates a <see cref="CancelableTaskCompletionSource{TResult}"/>.
        /// </summary>
        /// <param name="cancellationToken">
        /// The <see cref="CancellationToken"/> to associate with the <see cref="CancelableTaskCompletionSource{TResult}"/>.
        /// </param>
        /// <param name="state">
        /// The state to use as the underlying <see cref="Task{TResult}"/>'s AsyncState.
        /// </param>
        public CancelableTaskCompletionSource(CancellationToken cancellationToken, object? state)
            : this(cancellationToken, state, None)
        {
        }

        /// <summary>
        /// Creates a <see cref="CancelableTaskCompletionSource{TResult}"/>.
        /// </summary>
        /// <param name="cancellationToken">
        /// The <see cref="CancellationToken"/> to associate with the <see cref="CancelableTaskCompletionSource{TResult}"/>.
        /// </param>
        /// <param name="state">
        /// The state to use as the underlying <see cref="Task{TResult}"/>'s AsyncState.
        /// </param>
        /// <param name="creationOptions">
        /// The options to use when creating the underlying <see cref="Task{TResult}"/>.
        /// </param>
        public CancelableTaskCompletionSource(
            CancellationToken cancellationToken,
            object? state,
            TaskCreationOptions creationOptions)
            : base(state, creationOptions)
        {
            CancellationToken = cancellationToken;

            if (!cancellationToken.CanBeCanceled)
            {
                return;
            }

            _registration = cancellationToken.Register(
                state => ((CancelableTaskCompletionSource<TResult>)state!).TrySetCanceled(),
                this);

            Task.ContinueWithSynchronously(
                (_, state) => ((CancelableTaskCompletionSource<TResult>)state!)._registration.Dispose(),
                this);
        }
    }
}