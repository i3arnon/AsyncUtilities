using System;
using System.Runtime.CompilerServices;

#if NETSTANDARD2_0

namespace AsyncUtilities
{
    /// <summary>Provides an awaiter for a <see cref="ValueTask"/>.</summary>
    public readonly struct ValueTaskAwaiter : ICriticalNotifyCompletion
    {
        private readonly ValueTask _value;

        /// <summary>Gets whether the <see cref="ValueTask"/> has completed.</summary>
        public bool IsCompleted =>
            _value.IsCompleted;

        /// <summary>Initializes the awaiter.</summary>
        /// <param name="value">The value to be awaited.</param>
        internal ValueTaskAwaiter(ValueTask value) =>
            _value = value;

        /// <summary>Gets the result of the ValueTask.</summary>
        public void GetResult() =>
            _value._task?.GetAwaiter().GetResult();

        /// <summary>Schedules the continuation action for this ValueTask.</summary>
        public void OnCompleted(Action continuation) =>
            _value.
                AsTask().
                ConfigureAwait(continueOnCapturedContext: true).
                GetAwaiter().
                OnCompleted(continuation);

        /// <summary>Schedules the continuation action for this ValueTask.</summary>
        public void UnsafeOnCompleted(Action continuation) =>
            _value.
                AsTask().
                ConfigureAwait(continueOnCapturedContext: true).
                GetAwaiter().
                UnsafeOnCompleted(continuation);
    }
}

#endif