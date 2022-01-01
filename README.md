# AsyncUtilities
[![NuGet](https://img.shields.io/nuget/dt/AsyncUtilities.svg)](https://www.nuget.org/packages/AsyncUtilities)
[![NuGet](https://img.shields.io/nuget/v/AsyncUtilities.svg)](https://www.nuget.org/packages/AsyncUtilities)
[![license](https://img.shields.io/github/license/i3arnon/AsyncUtilities.svg)](LICENSE)

A collection of somewhat useful utilities and extension methods for async programming:


[Utilities:](#utilities)

1. [AsyncLock](#async-lock)
1. [Striped Lock](#striped-lock)
1. [TaskEnumerableAwaiter](#task-enumerable-awaiter)
1. [CancelableTaskCompletionSource](#cancelable-task-completion-source)

[Extension Methods:](#extension-methods)

1. [ContinueWithSynchronously](#continue-with-synchronously)
1. [TryCompleteFromCompletedTask](#complete-from-completed-task)
1. [ToCancellationTokenSource](#to-cancellation-token-source)

---

## <a name="async-lock"/> `AsyncLock`

When awaiting async operations, there's no thread-affinity (by default). It's common to start the operation in a certain thread, and resume in another. For that reason, all synchronization constructs that are thread-affine (i.e. match the lock with a specific thread) can't be used in an async context. One of these is the simple `lock` statement (which actually uses `Monitor.Enter`, `Monitor.Exit`). For such a case I included `AsyncLock` which is a simple wrapper over a `SemaphoreSlim` of 1 (for now, at least):

```csharp
AsyncLock _lock = new AsyncLock();

async Task ReplaceAsync<T>(string id, T newItem)
{
    using (var dbContext = new DBContext())
    {
        using (await _lock.LockAsync())
        {
            await dbContext.DeleteAsync(id);
            await dbContext.InsertAsync(newItem);
        }
    }
}
```

---

## <a name="striped-lock"/> Striped Lock

Lock striping is a technique used to reduce contention on a lock by splitting it up to multiple lock instances (*stripes*) with higher granularity where each key is associated with a certain lock/strip (this, for example, is how locks are used inside [`ConcurrentDictionary`](http://referencesource.microsoft.com/#mscorlib/system/Collections/Concurrent/ConcurrentDictionary.cs,f1cd2689b9df7f4a,references) to enable higher concurrency). Using a striped lock is similar in practice to using a `Dictionary<TKey, TLock>` however that forces the number of locks to match the number of keys, while using a striped lock allows to set the concurrency level independently: A higher degree means more granularity but higher memory consumption and vice versa.

### `Striped`

The `Striped` generic class allows using any kind of lock (with any kind of key). The only necessary things are:

- The number of stripes
- The lock must have a parameterless constructor, or a delegate for creating it is supplied.
- The key must implement `GetHashCode` and `Equals` correctly, or an `IEqualityComparer<TKey>` be supplied.

```csharp
ThreadSafeCache<string> _cache = new ThreadSafeCache<string>();
Striped<string, object> _lock = Striped.Create<string, object>(Environment.ProcessorCount);

string GetContent(string filePath)
{
    var content = _cache.Get(filePath);
    if (content != null)
    {
        return content;
    }

    lock (_lock[filePath])
    {
        // Double-checked lock
        content = _cache.Get(filePath);
        if (content != null)
        {
            return content;
        }

        content = File.ReadAllText(filePath);
        _cache.Set(content);
        return content;
    }
}
```

### `StripedAsyncLock`

Since this library is mainly for asynchronous programming, and you can't use a simple synchronous lock for async methods, there's also a concrete encapsulation for an async lock. It returns an `IDisposable` so it can be used in a `using` scope:

```csharp
ThreadSafeCache<string> _cache = new ThreadSafeCache<string>();
StripedAsyncLock<string> _lock = new StripedAsyncLock<string>(stripes: 100);

async Task<string> GetContentAsync(string filePath)
{
    var content = _cache.Get(filePath);
    if (content != null)
    {
        return content;
    }

    using (await _lock.LockAsync(filePath))
    {
        // Double-checked lock
        content = _cache.Get(filePath);
        if (content != null)
        {
            return content;
        }

        using (var reader = File.OpenText(filePath))
        {
            content = await reader.ReadToEndAsync();
        }

        _cache.Set(content);
        return content;
    }
}
```

---

## <a name="task-enumerable-awaiter"/> `TaskEnumerableAwaiter`

`TaskEnumerableAwaiter` is an awaiter for a collection of tasks. It makes the C# compiler support awaiting a collection of tasks directly instead of calling `Task.WhenAll` first:

```csharp
HttpClient _httpClient = new HttpClient();

static async Task DownloadAllAsync()
{
    string[] urls = new[]
    {
        "http://www.google.com",
        "http://www.github.com",
        "http://www.twitter.com"
    };

    string[] strings = await urls.Select(url => _httpClient.GetStringAsync(url));
    foreach (var content in strings)
    {
        Console.WriteLine(content);
    }
}
```

It supports both `IEnumerable<Task>` & `IEnumerable<Task<TResult>>` and using `ConfigureAwait(false)` to avoid context capturing. I've written about it more extensively [here](http://blog.i3arnon.com/2018/01/02/task-enumerable-awaiter/).

---

## <a name="cancelable-task-completion-source"/> `CancelableTaskCompletionSource`

When you're implementing asynchronous operations yourself you're usually dealing with `TaskCompletionSource` which allows returning an uncompleted task and completing it in the future with a result, exception or cancellation. `CancelableTaskCompletionSource` joins together a `CancellationToken` and a `TaskCompletionSource` by cancelling the `CancelableTaskCompletionSource.Task` when the `CancellationToken` is cancelled. This can be useful when wrapping pre async/await custom asynchronous implementations, for example:

```csharp
Task<string> OperationAsync(CancellationToken cancellationToken)
{
    var taskCompletionSource = new CancelableTaskCompletionSource<string>(cancellationToken);

    StartAsynchronousOperation(result => taskCompletionSource.SetResult(result));

    return taskCompletionSource.Task;
}

void StartAsynchronousOperation(Action<string> callback)
{
    // ...
}
```

---

# <a name="extension-methods"/> Extension Methods:

## <a name="continue-with-synchronously"/> `TaskExtensions.ContinueWithSynchronously`

When implementing low-level async constructs, it's common to add a small continuation using `Task.ContinueWith` instead of using an async method (which adds the state machine overhead). To do that efficiently and safely you need the `TaskContinuationOptions.ExecuteSynchronously` and make sure it runs on the `ThreadPool`:

```csharp
Task.Delay(1000).ContinueWith(
    _ => Console.WriteLine("Done"),
    CancellationToken.None,
    TaskContinuationOptions.ExecuteSynchronously,
    TaskScheduler.Default);
```

`TaskExtensions.ContinueWithSynchronously` encapsulates that for you (with all the possible overloads):

```csharp
Task.Delay(1000).ContinueWithSynchronously(_ => Console.WriteLine("Done"));
```

---

## <a name="complete-from-completed-task"/> `TaskCompletionSourceExtensions.TryCompleteFromCompletedTask`

When working with `TaskCompletionSource` it's common to copy the result (or exception/cancellation) of another task, usually returned from an async method. `TryCompleteFromCompletedTask` checks the task's state and complete the `TaskCompletionSource` accordingly. This can be used for example when there's a single worker executing the actual operations one at a time and completing `TaskCompletionSource` instances that consumers are awaiting:

```csharp
BlockingCollection<string> _urls = new BlockingCollection<string>();
Queue<TaskCompletionSource<string>> _waiters = new Queue<TaskCompletionSource<string>>();
HttpClient _httpClient = new HttpClient();

async Task DownloadAllAsync()
{
    while (true)
    {
        await DownloadAsync(_urls.Take());
    }
}

async Task DownloadAsync(string url)
{
    Task<string> downloadTask = _httpClient.GetStringAsync(url);
    await downloadTask;

    TaskCompletionSource<string> taskCompletionSource = _waiters.Dequeue();
    taskCompletionSource.TryCompleteFromCompletedTask(downloadTask);
}
```

---

## <a name="to-cancellation-token-source"/> `TaskExtensions.ToCancellationTokenSource`

It can sometimes be useful to treat an existing task as a signaling mechanism for cancellation, especially when that task doesn't represent a specific operation but an ongoing state. `ToCancellationTokenSource` creates a `CancellationTokenSource` that gets cancelled when the task completes.

For example, TPL Dataflow blocks have a `Completion` property which is a task to enable the block's consumer to await its completion. If we want to show a loading animation to represent the block's operation, we need to cancel it when the block completes and we know that happened when the `Completion` task completes:

```csharp
HttpClient _httpClient = new HttpClient();

async Task<IEnumerable<string>> DownloadAllAsync(IEnumerable<string> urls)
{
    var results = new ConcurrentBag<string>();
    var block = new ActionBlock<string>(async url =>
    {
        var result = await _httpClient.GetStringAsync(url);
        results.Add(result);
    });

    var cancellationToken = block.Completion.ToCancellationTokenSource().Token;
    var loadingAnimation = new LoadingAnimation(cancellationToken);
    loadingAnimation.Show();

    foreach (var url in urls)
    {
        block.Post(url);
    }

    await block.Completion;
    return results;
}
```

---
