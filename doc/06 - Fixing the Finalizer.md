# 06 - Fixing the Finalizer

## From the last session

In the previous session we identified two more issues:

- **Issue 3 - The finalizer is unnecessary executed**
  - After the `Dispose()` is manually executed, the finalizer brings no additional value, but it will still be executed by the Garbage Collector, delaying the destruction of the object.


- **Issue 4 - The finalizer will throw an exception**
  - This second execution will, most probably, throw an exception, because the unmanaged memory is already released. The `pointer` points to an unallocated memory. One of the rules for implementing a finalizer method is that it should never throw an exception.

In this session will provide a solution for these new issues. But first, let's remember how the current implementation looks.

### The unsafe class

It implements the `IDisposable` interface and, in addition provides a finalizer that calls the `Dispose()` method:

```csharp
internal class MyBusiness : IDisposable
{
    private IntPtr pointer;
    
    public MyBusiness()
    {
        // Imagine we call some function from a native dll.
        // That function will allocate some memory and return the pointer.
        // We emulate that by using the "Marshal" class to allocating 1 KB of memory.
        pointer = Marshal.AllocHGlobal(1024);
    }
    
    public void DoSomeWork()
    {
        // Do some actions with the pointer.
        // Probably by calling other functions from the native dll.
        // ...
    }
    
    public void Dispose()
    {
        // Usually, the memory is deallocated by calling a function from the initial native dll,
        // that knows how to deallocate that particular memory.
        // We emulate that by using the "Marshal" class to deallocate the 1 KB of memory.
        Marshal.FreeHGlobal(pointer);
    }
    
    ~MyBusiness()
    {
        Dispose();
    }
}
```

### The usage of the unsafe class

```csharp
internal static void Main()
{
    while (true)
    {
        using MyBusiness myBusiness = new();
        myBusiness.DoSomeWork();
        
        DoSomethingElse();
    }
}

internal static DoSometingElse()
{
    // Sometimes it may throw.    
    if (DateTime.Now.Ticks % 2 == 0)
        throw new Exception("Something went terribly wrong.");
}
```

## Solution - Suppress the Finalization

If the consumer already manually executed the `Dispose()` method, the execution of the finalizer method provides no benefit. It turns out that .NET provides a way to suppress its execution by calling the static method `GC.SuppressFinalize(...)` and providing as parameter the object for which the finalization should be suppressed.

This method must be executed when the `Dispose()` is manually called and not when the `Dispose()` is called from the finalizer. To be able to do the difference, let's, first, extract the content of the `Dispose()` in a private method that will be executed from both the public `Dispose` and the finalizer.

Then, we can call `GC.SuppressFinalize(this)` only from the public `Dispose()` method.

The final code looks like this:

```csharp
internal class MyBusiness : IDisposable
{
    private IntPtr pointer;
    
    ...
    
    public void Dispose()
    {
        DisposeInternal();
        GC.SuppressFinalize(this);
    }
    
    private void DisposeInternal()
    {
        // Usually, the memory is deallocated by calling a function from the initial native dll,
        // that knows how to deallocate that particular memory.
        // We emulate that by using the "Marshal" class to deallocate the 1 KB of memory.
        Marshal.FreeHGlobal(pointer);
    }
    
    ~MyBusiness()
    {
        DisposeInternal();
    }
}
```

> **Note**
>
> In the final state of the implementation, by convention, the `DisposeInternal()` method will be, actually, renamed to `Dispose(bool)`.
>
> This will be done later, when the boolean parameter will also be added.

The current implementation is, actually, fixing both issues (Issue 3 and Issue 4):

- The finalizer will not be called unnecessary.
- The finalizer will not throw because it will not be called a second time, after the memory is released from the first call.

## Conclusions

Now the class is safe to be used. Isn't it? We took care of releasing the unmanaged memory even if the consumer forgets to manually do it.

Is this all?

Actually... There are things that can be improved. We did not discussed yet about:

- How to prevent the usage of the object after it was disposed?
- What about the potential derived classes? How should they release their unmanaged memory if needed?
- And, also, how to handle a disposable instance of another type if it is part of our disposable class? Is there any change in the implementation to handle this case?

Let's take them one by one.
