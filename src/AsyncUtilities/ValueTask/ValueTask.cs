using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace AsyncUtilities
{
    /// <summary>
    /// Provides a value type that wraps a <see cref="Task"/>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Methods may return an instance of this value type when it's likely that the result of their
    /// operations will be available synchronously and when the method is expected to be invoked so
    /// frequently that the cost of allocating a new <see cref="Task"/> for each call will
    /// be prohibitive.
    /// </para>
    /// <para>
    /// For example, for uses other than consuming the result of an asynchronous operation via await, 
    /// <see cref="ValueTask"/> can lead to a more convoluted programming model, which can in turn actually 
    /// lead to more allocations.  For example, consider a method that could return either a <see cref="Task"/> 
    /// with a cached task as a common result or a <see cref="ValueTask"/>.  If the consumer of the result 
    /// wants to use it as a <see cref="Task"/>, such as to use with in methods like Task.WhenAll and Task.WhenAny, 
    /// the <see cref="ValueTask"/> would first need to be converted into a <see cref="Task"/> using 
    /// <see cref="AsTask"/>, which leads to an allocation that would have been avoided if a cached 
    /// <see cref="Task"/> had been used in the first place.
    /// </para>
    /// <para>
    /// As such, the default choice for any asynchronous method should be to return a <see cref="Task"/> or 
    /// <see cref="Task{TResult}"/>. Only if performance analysis proves it worthwhile should a <see cref="ValueTask"/> 
    /// be used instead of <see cref="Task"/>.
    /// </para>
    /// </remarks>
    [AsyncMethodBuilder(typeof(AsyncValueTaskMethodBuilder))]
    public readonly struct ValueTask : IEquatable<ValueTask>
    {
        private static readonly Task _completedTask = Task.FromResult(result: false);

        internal readonly Task _task;

        /// <summary>
        /// Initialize the <see cref="ValueTask"/> with a <see cref="Task"/> that represents the operation.
        /// </summary>
        /// <param name="task">The task.</param>
        public ValueTask(Task task) => 
            _task = task ?? throw new ArgumentNullException(nameof(task));

        /// <summary>Returns the hash code for this instance.</summary>
        public override int GetHashCode() =>
            _task?.GetHashCode() ?? 0;

        /// <summary>Returns a value indicating whether this value is equal to a specified <see cref="object"/>.</summary>
        public override bool Equals(object obj) =>
            obj is ValueTask valueTask && Equals(valueTask);

        /// <summary>Returns a value indicating whether this value is equal to a specified <see cref="ValueTask"/> value.</summary>
        public bool Equals(ValueTask other) => 
            _task == other._task;

        /// <summary>Returns a value indicating whether two <see cref="ValueTask"/> values are equal.</summary>
        public static bool operator ==(ValueTask left, ValueTask right) =>
            left.Equals(right);

        /// <summary>Returns a value indicating whether two <see cref="ValueTask"/> values are not equal.</summary>
        public static bool operator !=(ValueTask left, ValueTask right) => 
            !left.Equals(right);

        /// <summary>
        /// Gets a <see cref="Task"/> object to represent this ValueTask.  It will
        /// either return the task object if one exists, or it'll manufacture a new
        /// task object to represent the result.
        /// </summary>
        public Task AsTask() => 
            _task ?? _completedTask;

        /// <summary>Gets whether the <see cref="ValueTask"/> represents a completed operation.</summary>
        public bool IsCompleted =>
            _task == null || _task.IsCompleted;

        /// <summary>Gets whether the <see cref="ValueTask"/> represents a successfully completed operation.</summary>
        public bool IsCompletedSuccessfully =>
            _task == null || _task.Status == TaskStatus.RanToCompletion;

        /// <summary>Gets whether the <see cref="ValueTask"/> represents a failed operation.</summary>
        public bool IsFaulted =>
            _task != null && _task.IsFaulted;

        /// <summary>Gets whether the <see cref="ValueTask"/> represents a canceled operation.</summary>
        public bool IsCanceled =>
            _task != null && _task.IsCanceled;

        /// <summary>Gets an awaiter for this value.</summary>
        public ValueTaskAwaiter GetAwaiter() => 
            new ValueTaskAwaiter(this);

        /// <summary>Configures an awaiter for this value.</summary>
        /// <param name="continueOnCapturedContext">
        /// true to attempt to marshal the continuation back to the captured context; otherwise, false.
        /// </param>
        public ConfiguredValueTaskAwaitable ConfigureAwait(bool continueOnCapturedContext) => 
            new ConfiguredValueTaskAwaitable(this, continueOnCapturedContext);
    }
}