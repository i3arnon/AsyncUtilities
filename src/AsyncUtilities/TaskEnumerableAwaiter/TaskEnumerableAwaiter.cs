using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace AsyncUtilities
{
    /// <summary>Provides an awaiter for a collection of <see cref="Task"/> instances.</summary>
    public readonly struct TaskEnumerableAwaiter : ICriticalNotifyCompletion
    {
        private readonly ConfiguredTaskAwaitable.ConfiguredTaskAwaiter _awaiter;

        /// <summary>Gets whether the all the tasks completed.</summary>
        public bool IsCompleted => _awaiter.IsCompleted;

        /// <summary>Initializes the awaiter.</summary>
        /// <param name="tasks">The tasks to be awaited.</param>
        /// <param name="continueOnCapturedContext">
        /// true to attempt to marshal the continuation back to the original synchronization context captured; otherwise, false.
        /// </param>
        internal TaskEnumerableAwaiter(IEnumerable<Task> tasks, bool continueOnCapturedContext = true)
        {
            if (tasks is null) throw new ArgumentNullException(nameof(tasks));

            _awaiter =
                Task.WhenAll(tasks).
                    ConfigureAwait(continueOnCapturedContext).
                    GetAwaiter();
        }

        /// <summary>Schedules the continuation action for these tasks.</summary>
        public void OnCompleted(Action continuation) =>
            _awaiter.OnCompleted(continuation);

        /// <summary>Schedules the continuation action for these tasks.</summary>
        public void UnsafeOnCompleted(Action continuation) =>
            _awaiter.UnsafeOnCompleted(continuation);

        /// <summary>Ends the await on the completed tasks.</summary>
        public void GetResult() =>
            _awaiter.GetResult();
    }

    /// <summary>Provides an awaiter for a collection of <see cref="Task{TResult}"/> instances.</summary>
    /// <typeparam name="TResult">The type of the produced result.</typeparam>
    public readonly struct TaskEnumerableAwaiter<TResult> : ICriticalNotifyCompletion
    {
        private readonly ConfiguredTaskAwaitable<TResult[]>.ConfiguredTaskAwaiter _awaiter;

        /// <summary>Gets whether the all the tasks completed.</summary>
        public bool IsCompleted => _awaiter.IsCompleted;

        /// <summary>Initializes the awaiter.</summary>
        /// <param name="tasks">The tasks to be awaited.</param>
        /// <param name="continueOnCapturedContext">
        /// true to attempt to marshal the continuation back to the original synchronization context captured; otherwise, false.
        /// </param>
        internal TaskEnumerableAwaiter(IEnumerable<Task<TResult>> tasks, bool continueOnCapturedContext = true)
        {
            if (tasks is null) throw new ArgumentNullException(nameof(tasks));

            _awaiter =
                Task.WhenAll(tasks).
                    ConfigureAwait(continueOnCapturedContext).
                    GetAwaiter();
        }

        /// <summary>Schedules the continuation action for these tasks.</summary>
        public void OnCompleted(Action continuation) =>
            _awaiter.OnCompleted(continuation);

        /// <summary>Schedules the continuation action for these tasks.</summary>
        public void UnsafeOnCompleted(Action continuation) =>
            _awaiter.UnsafeOnCompleted(continuation);

        /// <summary>Ends the await on the completed tasks.</summary>
        public TResult[] GetResult() => _awaiter.GetResult();
    }
}