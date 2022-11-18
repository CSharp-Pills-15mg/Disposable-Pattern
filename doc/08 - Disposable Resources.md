# 08 - Disposable Resources

Until now we discussed about classes that contain unmanaged memory and how to ensure that the memory is released.

But what if out class contains members that implement the `IDisposable` interface? Should we do something special about them? Should we call their `Dispose()` method? If yes, when?

In this session we will discuss about disposable members, but first, let's remember how our class looks like.

## From the previous session

### The class

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

## Adding a `FileStream` field

Let's assume that our class need to open a file to write some data into it. It will have a field of type `FileStream` that will be initialized in the constructor and used in another methods like this:

```csharp
internal class MyBusiness : IDisposable
{
    ...
        
    private FileStream fileStream;
    
    public MyBusiness()
    {
        IntPtr pointer = Marshal.AllocHGlobal(1024);
        
        // Create the file stream in the constructor.
        fileStream = File.OpenWrite("c:\\temp\\file.txt");
    }
    
    public void DoSomeWork()
    {
        if (isDisposed)
            throw new ObjectDisposedException();
        
        // Use the fileStream to write data in the file.
        // ...
    }
    
    ...
}
```

## Disposing the `fileStream`

The `FileStream` implements the `IDisposable` interface and provides the `Dispose()` method, but, because it is initialized in one method (the constructor) and used in another one (the `DoSomething()` method), we cannot use a `using` statement.

The `fileStream` must be available for the lifetime of the parent object and be destroyed when the parent object is destroyed. So, the disposable mechanism implemented until now is perfect for this job. Let's call the `fileStream.Dispose()` method when the parent object is disposed next to the release of the unmanaged memory:

```csharp
internal class MyBusiness : IDisposable
{
    private bool isDisposed;
    private IntPtr pointer;
    private FileStream fileStream;
    
    ...
    
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
        fileStream.Dispose();
        
        isDisposed = true;
    }
    
    ~MyBusiness()
    {
        DisposeInternal();
    }
}
```

The `DisposeInternal()` method is executed in two different contexts:

- When manually executed by calling the `Dispose()` method (either explicitly or implicitly from the `using` statement), everything goes as planned;
- But, when it is executed by the finalizer, we may have a problem. 

## Issue 5 - Disposing other objects during finalization

When our object is disposed during the finalization phase, we know the object is not referenced by the main part of the application and the Garbage Collector is trying to destroy it. The `fileStream` field is in the same situation, the Garbage Collector will try to destroy it, but we do not know if the `fileStream` was already destroyed or not. So, we should let Garbage Collector to do its job and not try to dispose the `fileStream` ourselves:

### Solution for Issue 5

To fix this problem, in the `DisposeInternal()` method we need a way to dispose the disposable fields only if the `Dispose()` was called directly and not from the finalizer.

We may add a boolean parameter for that:

```csharp
internal class MyBusiness : IDisposable
{
    private bool isDisposed;
    private IntPtr pointer;
    private FileStream fileStream;
    
    ...
    
    public void Dispose()
    {
        DisposeInternal(true);
        GC.SuppressFinalize(this);
    }
    
    private void DisposeInternal(bool isDisposing)
    {
        if (isDisposed)
            return;
        
        Marshal.FreeHGlobal(pointer);
        
        if (isDisposing)
        {
            fileStream.Dispose();
        }
        
        isDisposed = true;
    }
    
    ~MyBusiness()
    {
        DisposeInternal(false);
    }
}
```

## Rename the private `Dispose(bool)`

By convention, the private method that we called until now `DisposeInternal(bool)`  is usually called `Dispose(bool)`. We could not name it from the beginning `Dispose()` because it would have had the same signature with the public `Dispose()` method. But now, when we added the boolean parameter, we are free to name it as we wish.

Let's rename it:

```csharp
internal class MyBusiness : IDisposable
{
    private bool isDisposed;
    private IntPtr pointer;
    private FileStream fileStream;
    
    ...
    
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
    
    private void Dispose(bool isDisposing)
    {
        if (isDisposed)
            return;
        
        Marshal.FreeHGlobal(pointer);
        
        if (isDisposing)
        {
            fileStream.Dispose();
        }
        
        isDisposed = true;
    }
    
    ~MyBusiness()
    {
        Dispose(false);
    }
}
```

## Conclusions

I think we are good for now. We took care of the disposable members and called their `Dispose()` methods, too. And this is done only when manually disposing the object, not when it is disposed during the finalization.

The last topic to discuss is "Derived Classes":

- How can we allow other derived classes to dispose their own unmanaged memory during the dispose process?



