# AsyncUtilities
[![NuGet](https://img.shields.io/nuget/dt/AsyncUtilities.svg)](https://www.nuget.org/packages/AsyncUtilities)
[![NuGet](https://img.shields.io/nuget/v/AsyncUtilities.svg)](https://www.nuget.org/packages/AsyncUtilities)
[![license](https://img.shields.io/github/license/i3arnon/AsyncUtilities.svg)](LICENSE)

A collection of somewhat useful utilities and extension methods for async programming:

### Utilities:

- Non-generic [`ValueTask`](#valuetask)
- [`StripedAsyncLock<TKey>`](#stripedasynclock)
- `Striped<TKey, TLock>`
- `CancelableTaskCompletionSource`
- `TaskEnumerableAwaiter`

### Extension Methods:

- `Task.ContinueWithSynchronously`
- `Task.ToCancellationTokenSource`
- `IEnumerable<Task>.GetAwaiter`
- `TaskCompletionSource.TryCompleteFromCompletedTask`

## Usage

### <a id="valueTask">ValueTask</a>
The non-generic `ValueTask` can be useful in async methods that are invoked very frequently, are likely to complete synchronously but unlike `ValueTask<TResult>` don't return a result. Usually these method will await another async method that does return a `ValueTask<TResult>`, for example:

```C#
async ValueTask DrawAsync(string name)
{
    var pen = await GetItemAsync<Pen>("pen");
    var apple = await GetItemAsync<Apple>("apple");
    
    var applePen = pen.JamIn(apple);
    applePen.Draw();
}

async ValueTask<T> GetItemAsync<T>(string name)
{
    var item = GetFromCache<T>(name);
    if (item != null)
    {
        return item;
    }
    
    return await GetFromDbAsync<T>(name);
}
```

### <a id="stripedasynclock">StripedAsyncLock</a>
`StripedAsyncLock<TKey>` can be used to reduce contention on an `AsyncLock` by dividing it into more granular stripes. This allows different operations to lock separate stripes concurrently instead of locking the entire lock altogether:

```C#
StripedAsyncLock<string> _lock = new StripedAsyncLock<string>(stripes: 100);
Cache _cache = new Cache();

async Task<T> GetOrCreateItem<T>(string name) where T : new()
{
    var item = _cache.Get<T>(name);
    if (item != null)
    {
        return item;
    }
    
    using (await _lock.LockAsync(name))
    {
        var item = _cache.Get<T>(name);
        if (item != null)
        {
            return item;
        }
        
        item = new T();
        _cache.Set(item;
        return item;
    }
}
```

## Install

```powershell
Install-Package AsyncUtilities
```