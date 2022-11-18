# 02 - Create the Unsafe Class

## The unsafe class

Let's create a class that uses a pointer:

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
}
```

## Instantiate it continuously

Now, let's instantiate and use that class multiple times.

```csharp
internal static void Main()
{
    while (true)
    {
        MyBusiness myBusiness = new();
        myBusiness.DoSomeWork();
    }
}
```

Do you see the problem? Each time we instantiate the class, a new 1 KB of memory is allocated and it is never released. The memory used by the `MyBusiness` instance itself, which is managed memory, will be released, eventually, by the Garbage Collector, but the manually allocated memory will not be released because the Garbage Collector has no knowledge of it.

How can we fix the problem?

## The `Close()` method

One way to fix the problem is to provide a method, let's call it `Close()`, that releases the manually allocated memory.

```csharp
internal class MyBusiness
{
    private IntPtr pointer;
    
    ...
    
    public void Close()
    {
        // Usually, the memory is deallocated by calling a function from the initial native dll,
        // that knows how to deallocate that particular memory.
        // We emulate that by using the "Marshal" class to deallocate the 1 KB of memory.
        Marshal.FreeHGlobal(pointer);
    }
}
```

Now, let's call it:

```csharp
internal static void Main()
{
    while (true)
    {
        MyBusiness myBusiness = new();
        myBusiness.DoSomeWork();
        myBusiness.Close();
    }
}
```

## Possible issues

### Issue 1 - Forget to call `Close()`

The developer using our class may forget to call `Close()`.

- We are humans, after all.
- Can we provide a more robust implementation, that will prevent this type of human error?

### Issue 2 - Exception prevents the `Close()`

Between the instantiation and the call to the `Close()` method, an exception may be thrown. That will prevent the `Close()` method to be executed:

```csharp
internal static void Main()
{
    while (true)
    {
        MyBusiness myBusiness = new();
        myBusiness.DoSomeWork();
        
        DoSomethingElse();
        
        myBusiness.Close();
    }
}

internal static DoSometingElse()
{
    // Sometimes it may throw.    
    if (DateTime.Now.Ticks % 2 == 0)
        throw new Exception("Something went terribly wrong.");
}
```

How can we provide a more robust implementation, that will prevent this type of situation?

## Conclusions

Using the class that we created may result, in some cases, in memory leaks. Can there be something done about this?

- Should the consumer do something about the way the class is used?

- Can we, as owners and developers of the class, do something to prevent wrong usages?