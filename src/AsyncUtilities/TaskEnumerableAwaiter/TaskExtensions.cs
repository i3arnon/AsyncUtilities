using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AsyncUtilities
{
    public static partial class TaskExtensions
    {
        /// <summary>Gets an awaiter for a collection of <see cref="Task"/> instances.</summary>
        /// <param name="tasks">The tasks to create an awaiter for.</param>
        public static TaskEnumerableAwaiter GetAwaiter(this IEnumerable<Task> tasks)
        {
            if (tasks is null) throw new ArgumentNullException(nameof(tasks));

            return new TaskEnumerableAwaiter(tasks);
        }

        /// <summary>Configures an awaiter for a collection of <see cref="Task"/> instances.</summary>
        /// <param name="tasks">The tasks to create an awaiter for.</param>
        /// <param name="continueOnCapturedContext">
        /// true to attempt to marshal the continuation back to the captured context; otherwise, false.
        /// </param>
        public static ConfiguredTaskEnumerableAwaitable ConfigureAwait(
            this IEnumerable<Task> tasks,
            bool continueOnCapturedContext)
        {
            if (tasks is null) throw new ArgumentNullException(nameof(tasks));

            return new ConfiguredTaskEnumerableAwaitable(tasks, continueOnCapturedContext);
        }

        /// <summary>Gets an awaiter for a collection of <see cref="Task{TResult}"/> instances.</summary>
        /// <param name="tasks">The tasks to create an awaiter for.</param>
        public static TaskEnumerableAwaiter<TResult> GetAwaiter<TResult>(this IEnumerable<Task<TResult>> tasks)
        {
            if (tasks is null) throw new ArgumentNullException(nameof(tasks));

            return new TaskEnumerableAwaiter<TResult>(tasks);
        }

        /// <summary>Configures an awaiter for a collection of <see cref="Task{TResult}"/> instances.</summary>
        /// <param name="tasks">The tasks to create an awaiter for.</param>
        /// <param name="continueOnCapturedContext">
        /// true to attempt to marshal the continuation back to the captured context; otherwise, false.
        /// </param>
        public static ConfiguredTaskEnumerableAwaitable<TResult> ConfigureAwait<TResult>(
            this IEnumerable<Task<TResult>> tasks,
            bool continueOnCapturedContext)
        {
            if (tasks is null) throw new ArgumentNullException(nameof(tasks));

            return new ConfiguredTaskEnumerableAwaitable<TResult>(tasks, continueOnCapturedContext);
        }
    }
}