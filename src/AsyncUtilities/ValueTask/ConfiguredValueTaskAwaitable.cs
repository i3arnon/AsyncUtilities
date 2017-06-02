using System;
using System.Runtime.CompilerServices;

namespace AsyncUtilities
{
    /// <summary>Provides an awaitable type that enables configured awaits on a <see cref="ValueTask"/>.</summary>
    public struct ConfiguredValueTaskAwaitable
    {
        private readonly ValueTask _value;
        private readonly bool _continueOnCapturedContext;

        /// <summary>Initializes the awaitable.</summary>
        /// <param name="value">The wrapped <see cref="ValueTask"/>.</param>
        /// <param name="continueOnCapturedContext">
        /// true to attempt to marshal the continuation back to the original synchronization context captured; otherwise, false.
        /// </param>
        internal ConfiguredValueTaskAwaitable(ValueTask value, bool continueOnCapturedContext)
        {
            _value = value;
            _continueOnCapturedContext = continueOnCapturedContext;
        }

        /// <summary>Returns an awaiter for this <see cref="ConfiguredValueTaskAwaitable"/> instance.</summary>
        public ConfiguredValueTaskAwaiter GetAwaiter() =>
            new ConfiguredValueTaskAwaiter(_value, _continueOnCapturedContext);

        /// <summary>Provides an awaiter for a <see cref="ConfiguredValueTaskAwaitable"/>.</summary>
        public struct ConfiguredValueTaskAwaiter : ICriticalNotifyCompletion
        {
            private readonly ValueTask _value;
            private readonly bool _continueOnCapturedContext;

            /// <summary>Initializes the awaiter.</summary>
            /// <param name="value">The value to be awaited.</param>
            /// <param name="continueOnCapturedContext">The value to pass to ConfigureAwait.</param>
            internal ConfiguredValueTaskAwaiter(ValueTask value, bool continueOnCapturedContext)
            {
                _value = value;
                _continueOnCapturedContext = continueOnCapturedContext;
            }

            /// <summary>Gets whether the <see cref="ConfiguredValueTaskAwaitable"/> has completed.</summary>
            public bool IsCompleted =>
                _value.IsCompleted;

            /// <summary>Gets the result of the ValueTask.</summary>
            public void GetResult() => 
                _value._task?.GetAwaiter().GetResult();

            /// <summary>Schedules the continuation action for the <see cref="ConfiguredValueTaskAwaitable"/>.</summary>
            public void OnCompleted(Action continuation) => 
                _value.
                AsTask().
                ConfigureAwait(_continueOnCapturedContext).
                GetAwaiter().
                OnCompleted(continuation);

            /// <summary>Schedules the continuation action for the <see cref="ConfiguredValueTaskAwaitable"/>.</summary>
            public void UnsafeOnCompleted(Action continuation) => 
                _value.
                AsTask().
                ConfigureAwait(_continueOnCapturedContext).
                GetAwaiter().
                UnsafeOnCompleted(continuation);
        }
    }
}