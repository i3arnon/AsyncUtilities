using System;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncUtilities
{
    /// <summary>Provides a set of static methods for working with specific kinds of <see cref="Task"/> instances.</summary>
    public static partial class TaskExtensions
    {
        /// <summary>
        /// Creates a <see cref="CancellationTokenSource"/> that will signal 
        /// cancellation when the provided <paramref name="task"/> will complete.
        /// </summary>
        /// <param name="task">The task that will trigger cancellation when it completes.</param>
        /// <returns>
        /// A new <see cref="CancellationTokenSource"/> instance that will signal cancellation 
        /// to the <see cref="CancellationToken"/>s when the provided <paramref name="task"/> will be completed.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="task"/> argument is null.
        /// </exception>
        public static CancellationTokenSource ToCancellationTokenSource(this Task task)
        {
            if (task == null) throw new ArgumentNullException(nameof(task));
            
            var cancellationTokenSource = new CancellationTokenSource();
            task.ContinueWithSynchronously(
                (_, @this) => ((CancellationTokenSource)@this).Cancel(),
                cancellationTokenSource);
            return cancellationTokenSource;
        }
    }
}