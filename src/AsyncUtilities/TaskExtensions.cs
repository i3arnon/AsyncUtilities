using System;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncUtilities
{
    public static class TaskExtensions
    {
        public static Task ContinueWithSynchronously(this Task task, Action<Task> continuationAction)
        {
            if (task == null) throw new ArgumentNullException(nameof(task));
            if (continuationAction == null) throw new ArgumentNullException(nameof(continuationAction));

            return task.ContinueWith(
                continuationAction,
                CancellationToken.None,
                TaskContinuationOptions.ExecuteSynchronously,
                TaskScheduler.Default);
        }

        public static Task ContinueWithSynchronously(
            this Task task,
            Action<Task, object> continuationAction,
            object state)
        {
            if (task == null) throw new ArgumentNullException(nameof(task));
            if (continuationAction == null) throw new ArgumentNullException(nameof(continuationAction));

            return task.ContinueWith(
                continuationAction,
                state,
                CancellationToken.None,
                TaskContinuationOptions.ExecuteSynchronously,
                TaskScheduler.Default);
        }

        public static Task<TResult> ContinueWithSynchronously<TResult>(
            this Task task,
            Func<Task, TResult> continuationFunction)
        {
            if (task == null) throw new ArgumentNullException(nameof(task));
            if (continuationFunction == null) throw new ArgumentNullException(nameof(continuationFunction));

            return task.ContinueWith(
                continuationFunction,
                CancellationToken.None,
                TaskContinuationOptions.ExecuteSynchronously,
                TaskScheduler.Default);
        }

        public static Task<TResult> ContinueWithSynchronously<TResult>(
            this Task task,
            Func<Task, object, TResult> continuationFunction,
            object state)
        {
            if (task == null) throw new ArgumentNullException(nameof(task));
            if (continuationFunction == null) throw new ArgumentNullException(nameof(continuationFunction));

            return task.ContinueWith(
                continuationFunction,
                state,
                CancellationToken.None,
                TaskContinuationOptions.ExecuteSynchronously,
                TaskScheduler.Default);
        }

        public static Task ContinueWithSynchronously<TResult>(
            this Task<TResult> task,
            Action<Task<TResult>> continuationAction)
        {
            if (task == null) throw new ArgumentNullException(nameof(task));
            if (continuationAction == null) throw new ArgumentNullException(nameof(continuationAction));

            return task.ContinueWith(
                continuationAction,
                CancellationToken.None,
                TaskContinuationOptions.ExecuteSynchronously,
                TaskScheduler.Default);
        }

        public static Task ContinueWithSynchronously<TResult>(
            this Task<TResult> task,
            Action<Task<TResult>, object> continuationAction,
            object state)
        {
            if (task == null) throw new ArgumentNullException(nameof(task));
            if (continuationAction == null) throw new ArgumentNullException(nameof(continuationAction));

            return task.ContinueWith(
                continuationAction,
                state,
                CancellationToken.None,
                TaskContinuationOptions.ExecuteSynchronously,
                TaskScheduler.Default);
        }

        public static Task<TNewResult> ContinueWithSynchronously<TResult, TNewResult>(
            this Task<TResult> task,
            Func<Task<TResult>, TNewResult> continuationFunction)
        {
            if (task == null) throw new ArgumentNullException(nameof(task));
            if (continuationFunction == null) throw new ArgumentNullException(nameof(continuationFunction));

            return task.ContinueWith(
                continuationFunction,
                CancellationToken.None,
                TaskContinuationOptions.ExecuteSynchronously,
                TaskScheduler.Default);
        }

        public static Task<TNewResult> ContinueWithSynchronously<TResult, TNewResult>(
            this Task<TResult> task,
            Func<Task<TResult>, object, TNewResult> continuationFunction,
            object state)
        {
            if (task == null) throw new ArgumentNullException(nameof(task));
            if (continuationFunction == null) throw new ArgumentNullException(nameof(continuationFunction));

            return task.ContinueWith(
                continuationFunction,
                state,
                CancellationToken.None,
                TaskContinuationOptions.ExecuteSynchronously,
                TaskScheduler.Default);
        }
    }
}