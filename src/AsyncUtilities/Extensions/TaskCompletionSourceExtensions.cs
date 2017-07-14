using System;
using System.Threading.Tasks;

namespace AsyncUtilities
{
    /// <summary>Provides a set of static methods for working with 
    /// <see cref="TaskCompletionSource{TResult}"/> instances.</summary>
    public static class TaskCompletionSourceExtensions
    {
        /// <summary>
        /// Tries to complete a <see cref="TaskCompletionSource{TResult}"/> with the status 
        /// and result of the provided <see cref="completedTask"/>.
        /// </summary>
        /// <typeparam name="TResult">
        /// The type of the result value associated with this <see cref="TaskCompletionSource{TResult}" />.
        /// </typeparam>
        /// <param name="taskCompletionSource">
        /// The <see cref="TaskCompletionSource{TResult}"/> instance to complete with the <paramref name="completedTask"/>.
        /// </param>
        /// <param name="completedTask">
        /// The completed <see cref="Task{TResult}"/> to use when completing the <paramref name="taskCompletionSource"/>.
        /// </param>
        /// <returns>
        /// <see langword="true" /> if the <paramref name="taskCompletionSource"/> was completed successfully; 
        /// otherwise, <see langword="false" />.
        /// </returns>
        public static bool TryCompleteFromCompletedTask<TResult>(
            this TaskCompletionSource<TResult> taskCompletionSource,
            Task<TResult> completedTask)
        {
            if (taskCompletionSource == null) throw new ArgumentNullException(nameof(taskCompletionSource));
            if (completedTask == null) throw new ArgumentNullException(nameof(completedTask));

            switch (completedTask.Status)
            {
                case TaskStatus.Faulted:
                    return taskCompletionSource.TrySetException(completedTask.Exception.InnerExceptions);
                case TaskStatus.Canceled:
                    return taskCompletionSource.TrySetCanceled();
                case TaskStatus.RanToCompletion:
                    return taskCompletionSource.TrySetResult(completedTask.Result);
                default:
                    throw new ArgumentException("Argument must be a completed task", nameof(completedTask));
            }
        }
    }
}