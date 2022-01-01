using System;
using System.Threading.Tasks;
using static System.Threading.Tasks.TaskStatus;

namespace AsyncUtilities
{
    /// <summary>Provides a set of static methods for working with 
    /// <see cref="TaskCompletionSource{TResult}"/> instances.</summary>
    public static class TaskCompletionSourceExtensions
    {
        /// <summary>
        /// Tries to complete a <see cref="TaskCompletionSource{TResult}"/> with the status 
        /// and result of the provided <paramref name="completedTask"/>.
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
            if (taskCompletionSource is null) throw new ArgumentNullException(nameof(taskCompletionSource));
            if (completedTask is null) throw new ArgumentNullException(nameof(completedTask));

            return completedTask.Status switch
            {
                Faulted => taskCompletionSource.TrySetException(completedTask.Exception!.InnerExceptions),
                Canceled => taskCompletionSource.TrySetCanceled(),
                RanToCompletion => taskCompletionSource.TrySetResult(completedTask.Result),
                _ => throw new ArgumentException("Argument must be a completed task", nameof(completedTask))
            };
        }
    }
}