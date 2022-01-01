using System;
using System.Threading;
using System.Threading.Tasks;
using static System.Threading.Tasks.TaskContinuationOptions;

namespace AsyncUtilities
{
    public static partial class TaskExtensions
    {
        /// <summary>
        /// Creates a continuation that executes synchronously on the ThreadPool when the target <see cref="Task"/> completes.
        /// </summary>
        /// <param name="task">
        /// The antecedent <see cref="Task"/>.
        /// </param>
        /// <param name="continuationAction">
        /// An action to run when the <see cref="Task"/> completes. When run, the delegate will be
        /// passed the completed task as an argument.
        /// </param>
        /// <returns>A new continuation <see cref="Task"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="task"/> argument is null.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="continuationAction"/> argument is null.
        /// </exception>
        public static Task ContinueWithSynchronously(this Task task, Action<Task> continuationAction)
        {
            if (task is null) throw new ArgumentNullException(nameof(task));
            if (continuationAction is null) throw new ArgumentNullException(nameof(continuationAction));

            return task.ContinueWith(
                continuationAction,
                CancellationToken.None,
                ExecuteSynchronously,
                TaskScheduler.Default);
        }

        /// <summary>
        /// Creates a continuation that executes synchronously on the ThreadPool when the target <see cref="Task"/> completes.
        /// </summary>
        /// <param name="task">
        /// The antecedent <see cref="Task"/>.
        /// </param>
        /// <param name="continuationAction">
        /// An action to run when the <see cref="Task"/> completes. When run, the delegate will be
        /// passed the completed task as an argument.
        /// </param>
        /// <param name="state">
        /// An object representing data to be used by the continuation action.
        /// </param>
        /// <returns>A new continuation <see cref="Task"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="task"/> argument is null.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="continuationAction"/> argument is null.
        /// </exception>
        public static Task ContinueWithSynchronously(
            this Task task,
            Action<Task, object?> continuationAction,
            object? state)
        {
            if (task is null) throw new ArgumentNullException(nameof(task));
            if (continuationAction is null) throw new ArgumentNullException(nameof(continuationAction));

            return task.ContinueWith(
                continuationAction,
                state,
                CancellationToken.None,
                ExecuteSynchronously,
                TaskScheduler.Default);
        }

        /// <summary>
        /// Creates a continuation that executes synchronously on the ThreadPool when the target <see cref="Task"/> completes.
        /// </summary>
        /// <param name="task">
        /// The antecedent <see cref="Task"/>.
        /// </param>
        /// <param name="continuationFunction">
        /// A function to run when the <see cref="Task"/> completes. When run, the delegate will be
        /// passed the completed task as an argument.
        /// </param>
        /// <returns>A new continuation <see cref="Task{TResult}"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="task"/> argument is null.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="continuationFunction"/> argument is null.
        /// </exception>
        public static Task<TResult> ContinueWithSynchronously<TResult>(
            this Task task,
            Func<Task, TResult> continuationFunction)
        {
            if (task is null) throw new ArgumentNullException(nameof(task));
            if (continuationFunction is null) throw new ArgumentNullException(nameof(continuationFunction));

            return task.ContinueWith(
                continuationFunction,
                CancellationToken.None,
                ExecuteSynchronously,
                TaskScheduler.Default);
        }

        /// <summary>
        /// Creates a continuation that executes synchronously on the ThreadPool when the target <see cref="Task"/> completes.
        /// </summary>
        /// <param name="task">
        /// The antecedent <see cref="Task"/>.
        /// </param>
        /// <param name="continuationFunction">
        /// A function to run when the <see cref="Task"/> completes. When run, the delegate will be
        /// passed the completed task as an argument.
        /// </param>
        /// <param name="state">
        /// An object representing data to be used by the continuation function.
        /// </param>
        /// <returns>A new continuation <see cref="Task{TResult}"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="task"/> argument is null.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="continuationFunction"/> argument is null.
        /// </exception>
        public static Task<TResult> ContinueWithSynchronously<TResult>(
            this Task task,
            Func<Task, object?, TResult> continuationFunction,
            object? state)
        {
            if (task is null) throw new ArgumentNullException(nameof(task));
            if (continuationFunction is null) throw new ArgumentNullException(nameof(continuationFunction));

            return task.ContinueWith(
                continuationFunction,
                state,
                CancellationToken.None,
                ExecuteSynchronously,
                TaskScheduler.Default);
        }

        /// <summary>
        /// Creates a continuation that executes synchronously on the ThreadPool when the target <see cref="Task{TResult}"/> completes.
        /// </summary>
        /// <param name="task">
        /// The antecedent <see cref="Task{TResult}"/>.
        /// </param>
        /// <param name="continuationAction">
        /// An action to run when the <see cref="Task{TResult}"/> completes. When run, the delegate will be
        /// passed the completed task as an argument.
        /// </param>
        /// <returns>A new continuation <see cref="Task"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="task"/> argument is null.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="continuationAction"/> argument is null.
        /// </exception>
        public static Task ContinueWithSynchronously<TResult>(
            this Task<TResult> task,
            Action<Task<TResult>> continuationAction)
        {
            if (task is null) throw new ArgumentNullException(nameof(task));
            if (continuationAction is null) throw new ArgumentNullException(nameof(continuationAction));

            return task.ContinueWith(
                continuationAction,
                CancellationToken.None,
                ExecuteSynchronously,
                TaskScheduler.Default);
        }

        /// <summary>
        /// Creates a continuation that executes synchronously on the ThreadPool when the target <see cref="Task{TResult}"/> completes.
        /// </summary>
        /// <param name="task">
        /// The antecedent <see cref="Task{TResult}"/>.
        /// </param>
        /// <param name="continuationAction">
        /// An action to run when the <see cref="Task{TResult}"/> completes. When run, the delegate will be
        /// passed the completed task as an argument.
        /// </param>
        /// <param name="state">
        /// An object representing data to be used by the continuation action.
        /// </param>
        /// <returns>A new continuation <see cref="Task"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="task"/> argument is null.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="continuationAction"/> argument is null.
        /// </exception>
        public static Task ContinueWithSynchronously<TResult>(
            this Task<TResult> task,
            Action<Task<TResult>, object?> continuationAction,
            object? state)
        {
            if (task is null) throw new ArgumentNullException(nameof(task));
            if (continuationAction is null) throw new ArgumentNullException(nameof(continuationAction));

            return task.ContinueWith(
                continuationAction,
                state,
                CancellationToken.None,
                ExecuteSynchronously,
                TaskScheduler.Default);
        }

        /// <summary>
        /// Creates a continuation that executes synchronously on the ThreadPool when the target <see cref="Task{TResult}"/> completes.
        /// </summary>
        /// <param name="task">
        /// The antecedent <see cref="Task{TResult}"/>.
        /// </param>
        /// <param name="continuationFunction">
        /// A function to run when the <see cref="Task{TResult}"/> completes. When run, the delegate will be
        /// passed the completed task as an argument.
        /// </param>
        /// <returns>A new continuation <see cref="Task{TNewResult}"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="task"/> argument is null.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="continuationFunction"/> argument is null.
        /// </exception>
        public static Task<TNewResult> ContinueWithSynchronously<TResult, TNewResult>(
            this Task<TResult> task,
            Func<Task<TResult>, TNewResult> continuationFunction)
        {
            if (task is null) throw new ArgumentNullException(nameof(task));
            if (continuationFunction is null) throw new ArgumentNullException(nameof(continuationFunction));

            return task.ContinueWith(
                continuationFunction,
                CancellationToken.None,
                ExecuteSynchronously,
                TaskScheduler.Default);
        }

        /// <summary>
        /// Creates a continuation that executes synchronously on the ThreadPool when the target <see cref="Task{TResult}"/> completes.
        /// </summary>
        /// <param name="task">
        /// The antecedent <see cref="Task{TResult}"/>.
        /// </param>
        /// <param name="continuationFunction">
        /// A function to run when the <see cref="Task{TResult}"/> completes. When run, the delegate will be
        /// passed the completed task as an argument.
        /// </param>
        /// <param name="state">
        /// An object representing data to be used by the continuation function.
        /// </param>
        /// <returns>A new continuation <see cref="Task{TNewResult}"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="task"/> argument is null.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="continuationFunction"/> argument is null.
        /// </exception>
        public static Task<TNewResult> ContinueWithSynchronously<TResult, TNewResult>(
            this Task<TResult> task,
            Func<Task<TResult>, object?, TNewResult> continuationFunction,
            object? state)
        {
            if (task is null) throw new ArgumentNullException(nameof(task));
            if (continuationFunction is null) throw new ArgumentNullException(nameof(continuationFunction));

            return task.ContinueWith(
                continuationFunction,
                state,
                CancellationToken.None,
                ExecuteSynchronously,
                TaskScheduler.Default);
        }
    }
}