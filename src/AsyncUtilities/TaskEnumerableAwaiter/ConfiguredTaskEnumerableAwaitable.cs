using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AsyncUtilities
{
    /// <summary>Provides an awaitable type that enables configured awaits on a collection of <see cref="Task"/> instances.</summary>
    public readonly struct ConfiguredTaskEnumerableAwaitable
    {
        private readonly IEnumerable<Task> _tasks;
        private readonly bool _continueOnCapturedContext;

        /// <summary>Initializes the awaitable.</summary>
        /// <param name="tasks">The tasks to be awaited.</param>
        /// <param name="continueOnCapturedContext">
        /// true to attempt to marshal the continuation back to the original synchronization context captured; otherwise, false.
        /// </param>
        internal ConfiguredTaskEnumerableAwaitable(IEnumerable<Task> tasks, bool continueOnCapturedContext)
        {
            _tasks = tasks ?? throw new ArgumentNullException(nameof(tasks));
            _continueOnCapturedContext = continueOnCapturedContext;
        }

        /// <summary>Returns an awaiter for this <see cref="ConfiguredTaskEnumerableAwaitable"/> instance.</summary>
        public TaskEnumerableAwaiter GetAwaiter() =>
            new TaskEnumerableAwaiter(_tasks, _continueOnCapturedContext);
    }

    /// <summary>Provides an awaitable type that enables configured awaits on a collection of <see cref="Task{TResult}"/> instances.</summary>
    /// <typeparam name="TResult">The type of the produced result.</typeparam>
    public readonly struct ConfiguredTaskEnumerableAwaitable<TResult>
    {
        private readonly IEnumerable<Task<TResult>> _tasks;
        private readonly bool _continueOnCapturedContext;

        /// <summary>Initializes the awaitable.</summary>
        /// <param name="tasks">The tasks to be awaited.</param>
        /// <param name="continueOnCapturedContext">
        /// true to attempt to marshal the continuation back to the original synchronization context captured; otherwise, false.
        /// </param>
        internal ConfiguredTaskEnumerableAwaitable(IEnumerable<Task<TResult>> tasks, bool continueOnCapturedContext)
        {
            _tasks = tasks ?? throw new ArgumentNullException(nameof(tasks));
            _continueOnCapturedContext = continueOnCapturedContext;
        }

        /// <summary>Returns an awaiter for this <see cref="ConfiguredTaskEnumerableAwaitable{TResult}"/> instance.</summary>
        public TaskEnumerableAwaiter GetAwaiter() =>
            new TaskEnumerableAwaiter(_tasks, _continueOnCapturedContext);
    }
}