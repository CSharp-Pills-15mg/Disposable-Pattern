# 04 - `IDisposable` Interface and the `using` Statement

## From previous session

In the previous session we discussed about the `try-finally` block and how it can be used to ensure the call of the `Close()` method.

### The unsafe class

This is the our unsafe class:

```csharp
internal class MyBusiness
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
    
    public void Close()
    {
        // Usually, the memory is deallocated by calling a function from the initial native dll,
        // that knows how to deallocate that particular memory.
        // We emulate that by using the "Marshal" class to deallocate the 1 KB of memory.
        Marshal.FreeHGlobal(pointer);
    }
}
```

### Safely using the unsafe class

And this is how it is safely used:

```
internal static void Main()
{
    while (true)
    {
        MyBusiness myBusiness = new();
        try
        {
            myBusiness.DoSomeWork();
            
            DoSomethingElse();
        }
        finally
        {
            myBusiness.Close();
        }
    }
}

internal static DoSometingElse()
{
    // Sometimes it may throw.    
    if (DateTime.Now.Ticks % 2 == 0)
        throw new Exception("Something went terribly wrong.");
}
```

It turns out that .NET provides a standard way for this type of behavior:

- the `IDisposable` interface
- the `using` block

## `IDisposable` interface

The `IDisposable` interface is providing a single method called `Dispose()` that should be used for releasing the used resources (for example, the manually allocated memory). Looks like it is exactly what we did in the `Close()` method.

### Implement `IDisposable`

Let's implement the interface and rename the `Close()` method to `Dispose()`.

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
}
```

### Update the usage

Now, let's update the usage. Instead of manually calling the `Close()` method, now renamed to `Dispose()`, we may use the `using` block that is aware of the `IDisposable` interface and it will automatically call the `Dispose()` method:

```csharp
internal static void Main()
{
    while (true)
    {
        using (MyBusiness myBusiness = new())
        {
            myBusiness.DoSomeWork();
            
            DoSomethingElse();
        }
    }
}

internal static DoSometingElse()
{
    // Sometimes it may throw.    
    if (DateTime.Now.Ticks % 2 == 0)
        throw new Exception("Something went terribly wrong.");
}
```

Actually, from C# 8, we can use the `using` declaration that is a more compact way of doing the same thing:

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

## Conclusions

We provided a more compact way of calling the `Dispose()` method and releasing the memory, but we still do not prevent unsafe usage. If the consumer forgets to use the `using` statement, we are back to square 1: memory leak :(
