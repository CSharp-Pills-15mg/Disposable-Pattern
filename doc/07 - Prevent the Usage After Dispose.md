# 07 - Prevent the Usage After Dispose

After the object is disposed, it should not be used anymore, but can we provide a mechanism to ensure this? A common practice is to throw an exception if any method of the object is used after the object itself is disposed.

In this session, we will implement this behavior, but first let's remember how our class looks like from the previous session

## From the previous session

### The safe class

Now we can call it "the safe class" ;)

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

Each public method will check this flag and throw an `ObjectDisposedException` if necessary.

```csharp
internal class MyBusiness : IDisposable
{
    private bool isDisposed;
    
    ...
    
    public void DoSomeWork()
    {
        if (isDisposed)
            throw new ObjectDisposedException();
        
        // ...
    }
}
```

### Multiple disposing

If the object is already disposed, a common behavior for the `Dispose()` method is to just return without executing anything and not throwing an exception.

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
            throw new ObjectDisposedException();
        
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

We added some good protection that will prevent the usage of the object after it was disposed.

But we still need to discuss about disposable members and how to dispose them?

And also about derived classes and how to allow them to dispose their own managed and unmanaged memory when it exists.