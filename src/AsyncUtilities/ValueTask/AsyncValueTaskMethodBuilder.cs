using System;
using System.Runtime.CompilerServices;
using System.Security;

#if NETSTANDARD2_0

namespace AsyncUtilities
{
    /// <summary>Represents a builder for asynchronous methods that returns a <see cref="ValueTask"/>.</summary>
    public struct AsyncValueTaskMethodBuilder
    {
        private AsyncTaskMethodBuilder _methodBuilder;
        private bool _haveResult;
        private bool _useBuilder;

        /// <summary>Creates an instance of the <see cref="AsyncValueTaskMethodBuilder"/> struct.</summary>
        /// <returns>The initialized instance.</returns>
        public static AsyncValueTaskMethodBuilder Create() =>
            new AsyncValueTaskMethodBuilder
            {
                _methodBuilder = AsyncTaskMethodBuilder.Create()
            };

        /// <summary>Begins running the builder with the associated state machine.</summary>
        /// <typeparam name="TStateMachine">The type of the state machine.</typeparam>
        /// <param name="stateMachine">The state machine instance, passed by reference.</param>
        public void Start<TStateMachine>(ref TStateMachine stateMachine)
            where TStateMachine : IAsyncStateMachine =>
            _methodBuilder.Start(ref stateMachine);

        /// <summary>Associates the builder with the specified state machine.</summary>
        /// <param name="stateMachine">The state machine instance to associate with the builder.</param>
        public void SetStateMachine(IAsyncStateMachine stateMachine) =>
            _methodBuilder.SetStateMachine(stateMachine);

        /// <summary>Marks the task as successfully completed.</summary>
        public void SetResult()
        {
            if (_useBuilder)
            {
                _methodBuilder.SetResult();
            }
            else
            {
                _haveResult = true;
            }
        }

        /// <summary>Marks the task as failed and binds the specified exception to the task.</summary>
        /// <param name="exception">The exception to bind to the task.</param>
        public void SetException(Exception exception) =>
            _methodBuilder.SetException(exception);

        /// <summary>Gets the task for this builder.</summary>
        public ValueTask Task
        {
            get
            {
                if (_haveResult)
                {
                    return new ValueTask();
                }

                _useBuilder = true;
                return new ValueTask(_methodBuilder.Task);
            }
        }

        /// <summary>Schedules the state machine to proceed to the next action when the specified awaiter completes.</summary>
        /// <typeparam name="TAwaiter">The type of the awaiter.</typeparam>
        /// <typeparam name="TStateMachine">The type of the state machine.</typeparam>
        /// <param name="awaiter">the awaiter</param>
        /// <param name="stateMachine">The state machine.</param>
        public void AwaitOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine)
            where TAwaiter : INotifyCompletion
            where TStateMachine : IAsyncStateMachine
        {
            _useBuilder = true;
            _methodBuilder.AwaitOnCompleted(ref awaiter, ref stateMachine);
        }

        /// <summary>Schedules the state machine to proceed to the next action when the specified awaiter completes.</summary>
        /// <typeparam name="TAwaiter">The type of the awaiter.</typeparam>
        /// <typeparam name="TStateMachine">The type of the state machine.</typeparam>
        /// <param name="awaiter">the awaiter</param>
        /// <param name="stateMachine">The state machine.</param>
        [SecuritySafeCritical]
        public void AwaitUnsafeOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine)
            where TAwaiter : ICriticalNotifyCompletion
            where TStateMachine : IAsyncStateMachine
        {
            _useBuilder = true;
            _methodBuilder.AwaitUnsafeOnCompleted(ref awaiter, ref stateMachine);
        }
    }
}

#endif