# 09 - Derived Classes

When our class is inherited, that inheritor may also need to manually release some memory and/or dispose some member. It cannot create a new `Dispose()` method, so we must provide a way to hook up to our disposable mechanism.

But first, let's remember how the current implementation of the class look like.

## From the previous session

### The class

```csharp
internal class MyBusiness : IDisposable
{
    private bool isDisposed;
    private IntPtr pointer;
    private FileStream fileStream;
    
    public MyBusiness()
    {
        pointer = Marshal.AllocHGlobal(1024);
        fileStream = File.OpenWrite("c:\\temp\\file.txt");
    }
    
    public void DoSomeWork()
    {
        if (isDisposed)
            throw new ObjectDisposedException();
        
        // ...
    }
    
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

## Solution

### Inherit the original class

Let's imagine another class that inherits our initial one. 

```csharp
internal class OtherBusiness : MyBusiness
{
}
```

### Create and use some managed and unmanaged resources

Let's manually allocate some unmanaged memory and have a pointer field to reference that memory.

```csharp
internal class OtherBusiness : MyBusiness
{
    private IntPtr otherPointer;
    
    public MyBusiness()
    {
        otherPointer = Marshal.AllocHGlobal(1024);
    }
}
```

Also, we should have a disposable member. This time let it be a `Font` instance.

```csharp
internal class OtherBusiness : MyBusiness
{
    private IntPtr pointer;
    private Font font;
    
    public MyBusiness()
    {
        IntPtr pointer = Marshal.AllocHGlobal(1024);
        font = new Font();
    }
}
```

And a public method where all these resources are used.

```csharp
internal class OtherBusiness : MyBusiness
{
    private IntPtr pointer;
    private Font font;
    
    public MyBusiness()
    {
        IntPtr pointer = Marshal.AllocHGlobal(1024);
        font = new Font();
    }
    
    public void DoSomeOtherWork()
    {
        // Use the pointer and the font here.
        // ...
    }
}
```

How should we dispose the resources?

### Base Class - Make the private `Dispose()` as protected virtual

In order for the inheritor to be able to dispose its own resources, the base class must allow it to overwrite the previously private `Dispose()` method. Let's make it virtual:

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
    
    protected virtual void Dispose(bool isDisposing)
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

In this way, the base class still controls the way the finalization mechanism is implemented, but, it allows the inheritor to overwrite the actual disposing method.

### Derived Class - Hook up to the disposable mechanism

Now, the inheritor looks like this:

```csharp
internal class OtherBusiness : MyBusiness
{
    private IntPtr pointer;
    private Font font;
    
    public MyBusiness()
    {
        IntPtr pointer = Marshal.AllocHGlobal(1024);
        font = new Font();
    }
    
    public void DoSomeOtherWork()
    {
        // Use the pointer and the font here.
        // ...
    }
    
    protected virtual void Dispose(bool disposing)
    {
        Marshal.FreeHGlobal(pointer);
        
        if (isDisposing)
        {
            font.Dispose();
        }
        
        base.Dispose(isDisposing);
    }
}
```

> **Notes**
>
> - Don't forget to call the `base.Dispose()` at the end.
> - There is no need to provide a finalizer method. It is provided by the base class.

### Derived Class - Prevent Usage After Dispose

Same as the base class, the derived class may implement its own mechanism (the `isDisposed` boolean flag) to prevent the usage after dispose:

```csharp
internal class OtherBusiness : MyBusiness
{
    private bool isDisposed;
    private IntPtr pointer;
    private Font font;
    
    public MyBusiness()
    {
        IntPtr pointer = Marshal.AllocHGlobal(1024);
        font = new Font();
    }
    
    public void DoSomeOtherWork()
    {
        if (isDisposed)
            throw new ObjectDisposedException();
        
        // Use the pointer and the font here.
        // ...
    }
    
    protected virtual void Dispose(bool disposing)
    {
        if (isDisposed)
            return;
        
        Marshal.FreeHGlobal(pointer);
        
        if (isDisposing)
        {
            font.Dispose();
        }
        
        isDisposed = true;
        
        base.Dispose(isDisposing);        
    }
}
```

## Conclusion

We finished. This is the full implementation of the disposable pattern in C#.

In these session we showed the full implementation of the pattern, but in real life we encounter particular situations when we may need just a partial implementation.

See the next session for more details.
