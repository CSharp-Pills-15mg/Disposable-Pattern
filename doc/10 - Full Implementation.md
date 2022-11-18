# 10 - Full Implementation

The Disposable Pattern is all about releasing the unmanaged resources (for example unmanaged memory):

- the one allocated directly by the current instance or;
- the one allocated by other managed instances that our instance create.

The final, full implementation of the Disposable Pattern is displayed below.

### The main class

When the class is responsible of unmanaged resources (for example unmanaged memory), either directly, by using pointers, or indirectly by having other disposable members, it must provide a mechanism to easily and relaibly release those resources. The preferred mechanism is to implement the `IDisposable` interface.

The full implementation provides the following features:

- A `Dispose()` method that releases the unmanaged memory used by the class and, also, further calls the `Dispose()` method of other members that implement the `IDisposable` interface.
- A finalizer method, provided as a failsafe way, for releasing the unmanaged memory, to handle the situation when the consumer forgets to manually call `Dispose()`.

- Prevents the finalizer to be uselessly executed if the consumer has already called the `Dispose()` method manually.
- Prevents the usage of the object after it was disposed by throwing the `ObjectDisposedException`.
- Provids a mechanism for the derived classes to hook up to the disposable process and release their own unmanaged memory, by making the internal `Dispose(bool)` method as `protected virtual`.

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

### The usage of the main class

This is the way how the class is instantiated and used.

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

## The derived class

This is the implementation of a derived class that also needs to release unmanaged memory and dispose other disposable members.

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

### The usage of the derived class

The derived class is used in the same way as the base one.

```csharp
internal static void Main()
{
    while (true)
    {
        using OtherBusiness otherBusiness = new();
        otherBusiness.DoSomeWork();
        otherBusiness.DoSomOtherWork();
        
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

### Partial Implementation

The above example is the full implementation of the Disposable Pattern in C#, but, in real life, we encounter particular situations when we may need just a partial implementation.

For example, in most of the real-life cases, we will not manually allocate unmanaged memory. In most of these situations our classes will contain other managed objects, which already implement the `IDisposable` pattern. This will simplify the needed implementation.

Another simplification can be achieved if the base class is sealed and cannot be inherited. The class is not required to provide support for a derived class to hook up to the disposing mechanism.

In the next sessions will discuss these situations.
