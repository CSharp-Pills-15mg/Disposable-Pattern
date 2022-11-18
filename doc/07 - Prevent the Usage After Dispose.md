# 07 - Prevent the Usage After Dispose

After the object is disposed, it should not be used anymore, but can we provide a mechanism to ensure this? A common practice is to throw an exception if any method of the object is used after the object itself is disposed.

In this session, we will implement this behavior, but, first, let's remember how our class looks like from the previous session.

## From the previous session

### The safe class

Now we can call it "the safe class" ;)

It implements the `IDisposable` interface and provides a finalizer as a fail safe mechanism. Both the public `Dispose()` and the finalizer are calling the internal `DisposeInternal()` method, but only the public `Dispose()` is also suppressing the execution of the finalizer for performance reasons.

```csharp
internal class MyBusiness : IDisposable
{
    private IntPtr pointer;
    
    public MyBusiness()
    {
        pointer = Marshal.AllocHGlobal(1024);
    }
    
    public void DoSomeWork()
    {
        // ...
    }
    
    public void Dispose()
    {
        DisposeInternal();
        GC.SuppressFinalize(this);
    }
    
    private void DisposeInternal()
    {
        Marshal.FreeHGlobal(pointer);
    }
    
    ~MyBusiness()
    {
        DisposeInternal();
    }
}
```

### The usage of the class

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

## Add and use the `IsDisposed` flag

### Create the `IsDisposed` flag.

The object needs a way to remember if it was already disposed.

This can be done by creating a flag called `IsDisposed`, that is, initially, set on `false`. When the object is disposed the flag will be set on `true`.

```csharp
internal class MyBusiness : IDisposable
{
    private bool isDisposed;
    private IntPtr pointer;
    
    ...
    
    public void Dispose()
    {
        DisposeInternal();
        GC.SuppressFinalize(this);
    }
    
    private void DisposeInternal()
    {
        Marshal.FreeHGlobal(pointer);
        
        isDisposed = true;
    }
    
    ~MyBusiness()
    {
        DisposeInternal();
    }
}
```

### Protect each public method

Now, each public method can check this flag and throw an `ObjectDisposedException` if necessary.

```csharp
internal class MyBusiness : IDisposable
{
    private bool isDisposed;
    
    ...
    
    public void DoSomeWork()
    {
        if (isDisposed)
            throw new ObjectDisposedException("The current instance was disposed.");
        
        // ...
    }
}
```

### Multiple disposing

If the object is already disposed, in contrast with the other public methods, a common behavior for the `Dispose()` method is to just return, without executing anything and without throwing an exception.

This will allow multiple executions of `Dispose()` without throwing an exception.

```csharp
internal class MyBusiness : IDisposable
{
    private bool isDisposed;
    private IntPtr pointer;
    
    ...
    
    private void DisposeInternal()
    {
        if (isDisposed)
            return;
        
        Marshal.FreeHGlobal(pointer);
        
        isDisposed = true;
    }
    
    ...
}
```

## Final implementation

In the end, the class will look like this:

```csharp
internal class MyBusiness : IDisposable
{
    private bool isDisposed;
    private IntPtr pointer;
    
    public MyBusiness()
    {
        pointer = Marshal.AllocHGlobal(1024);
    }
    
    public void DoSomeWork()
    {
        if (isDisposed)
            throw new ObjectDisposedException("The current instance was disposed.");
        
        // ...
    }
    
    public void Dispose()
    {
        DisposeInternal();
        GC.SuppressFinalize(this);
    }
    
    private void DisposeInternal()
    {
        if (isDisposed)
            return;
        
        Marshal.FreeHGlobal(pointer);
        
        isDisposed = true;
    }
    
    ~MyBusiness()
    {
        DisposeInternal();
    }
}
```

## Conclusion

We added some good protection mechanism that will prevent the usage of the object after it was disposed.

But we still need to discuss about:

- How to dispose the disposable members, if any?

- And also about derived classes and how to allow them to dispose their own managed and unmanaged memory when it exists.