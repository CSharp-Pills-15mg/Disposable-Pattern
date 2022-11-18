# 05 - Adding a Finalizer

## From the last session

In the last session we implemented the `IDisposable` interface, but the consumer of our class must still remember to call the `Dispose()` method either directly or indirectly with the `using ` statement.

### The unsafe class

It implements the `IDisposable` interface.

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

## Garbage Collector

.NET provides the garbage collection mechanism that releases the memory (managed memory) used by the .NET instances. The Garbage Collector keeps a list with all the instances created by the application and, from time to time, it inspects this list. When it finds an instance that is not referenced anymore by any variable or field from a class, that instance is destroyed and the memory used by it, released.

Before destroying an instance, the Garbage Collector is offering it a chance for one last wish. That last wish is called the finalizer method:

```csharp
internal class MyBusiness
{
    ~MyBusiness()
    {
        // This is a finalizer.
    }
}
```

This finalizer comes with both benefits and drawback:

- The benefits are obvious: the object can do somehting before it is destroyed. For example it may release some manually allocated memory (pointers).
- The drawbacks are less obvious. When an object provides a finalizer method, the Garbage Collector executes the finalizer, but it does not destroy the object just yet. The object will remain in the memory until the next run of the Garbage Collector. When Garbage Collector runs again, it will again discover that the object is not referenced anymore, but, this time, because the finalizer was already executed, it will destroy the object and will release its memory (the managed memory).
  - So, the destruction of an object that provides a finalizer method takes two runs of the Garbage Collector. The destruction of an object that does not provide a finalizer method can be destroyed from the first run.
  - Good practice: Create a finalizer method only if the object really needs it.

## Use a finalizer

Let's use a finalizer to call `Dispose()` and release the unmanaged memory:

```csharp
internal class MyBusiness : IDisposable
{
    private IntPtr pointer;
    
    ...
    
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

By creating a finalizer method we ensure that the `Dispose()` will be executed even if the consumer of the class will forget to call it explicitly.

### Important Note

If we rely on Garbage Collector we do not control when will the unmanaged memory be released. We know for sure that it will be released, but we do not control the exact moment.

When we consume a class that implements the `IDisposable` interface, it is, usually, a good practice to call the `Dispose()` method either explicitly or implicitly with an `using` statement, as we already did in the previous examples.

## More Issues

By providing a finalizer, we decently solved both issues discussed in the previous sessions (Issue 1 and Issue 2). Are we done?

Actually... no. Not yet.

After the consumer is calling the `Dispose()` method (either explicitly or implicitly with an `using` statement) and the unmanaged memory is released, the Garbage Collector will be executed at a later time, call the finalizer which, in turn will call, again, the `Dispose()` method.

We have two issues here:

### Issue 3 - The finalizer is unnecessary executed

- After the `Dispose()` is manually executed, the finalizer brings no additional benefits, but it will still be executed by the Garbage Collector, delaying the destruction of the object.

### Issue 4 - The finalizer will throw an exception

- This second execution will, most probably, throw an exception, because the unmanaged memory is already released. The `pointer` points to an unallocated memory. One of the rules for implementing a finalizer method is that it should never throw an exception.

## Conclusions

It appears that we made the implementation worse. Now, if the consumer is calling the `Dispose()` method it does more harm than good.

It appears we still need to continue our quest for a safe-to-be-used class.