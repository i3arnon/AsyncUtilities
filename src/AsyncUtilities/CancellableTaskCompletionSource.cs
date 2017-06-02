using System.Threading;
using System.Threading.Tasks;

namespace AsyncUtilities
{
    public class CancellableTaskCompletionSource<TResult> : TaskCompletionSource<TResult>
    {
        private CancellationTokenRegistration _registration;

        public CancellationToken CancellationToken { get; }

        public CancellableTaskCompletionSource(CancellationToken cancellationToken)
            : this(cancellationToken, null, TaskCreationOptions.None)
        {
        }

        public CancellableTaskCompletionSource(CancellationToken cancellationToken, TaskCreationOptions creationOptions)
            : this(cancellationToken, null, creationOptions)
        {
        }

        public CancellableTaskCompletionSource(CancellationToken cancellationToken, object state)
            : this(cancellationToken, state, TaskCreationOptions.None)
        {
        }

        public CancellableTaskCompletionSource(
            CancellationToken cancellationToken,
            object state,
            TaskCreationOptions creationOptions)
            : base(state, creationOptions)
        {
            CancellationToken = cancellationToken;

            _registration = cancellationToken.Register(
                @this => ((CancellableTaskCompletionSource<TResult>)@this).TrySetCanceled(),
                this);

            Task.ContinueWith(
                (_, @this) => ((CancellableTaskCompletionSource<TResult>)@this)._registration.Dispose(),
                this,
                CancellationToken.None,
                TaskContinuationOptions.ExecuteSynchronously,
                TaskScheduler.Default);
        }
    }
}