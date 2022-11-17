# 01 - Memory Leaks

What is a memory leak and how to create a one?

## Managed vs Unmanaged Memory

.NET provides a mechanism that keeps track of the allocated memory and releases it when it is no longer needed. You probably heard about Garbage Collector. This is called managed memory.

Other languages, as it is the case of C/C++ does not offer such mechanisms and the developer must manually release the allocated memory. This is the unmanaged memory.

## Memory leak in C/C++

As we stated above, in C/C++ there is a common practice to manually allocate chunks of memory for different purposes. If we forget to release that memory before the end of the current execution block (for example, the end of the function), congratulations, we just created a memory leak. The memory will not be accessible again from the application because the variable pointing to that memory address (the pointer) has just been destroyed and, furthermore, it will never be available again for other allocations.

> **Notes**
>
> - When the execution block ends, all the local variables declared in that block are automatically destroyed. But, if those variables are pointers, the memory pointed by them remains untouched.
>

## Memory leak in C#

### a) Call unmanaged code

Managed code does not, so easily, create memory leaks.

One way is to execute functions from an unmanaged library (for example, a COM dll) that manually allocates some memory and return a pointer. This pointer must be used, later, to manually release that memory. This is done, most probably, by calling another function from the same library that will do the deallocation.

If we forget to call the second function from the library, the allocated memory will remain unused.

### b) Manually allocate memory

.NET provides a static function that can be used to allocate memory, just like in C/C++:

- `Marshal.AllocHGlobal()`

If we use it continuously for a sufficient number of times, all the available memory will be filled up and the application will crush:

```csharp
while (true)
{
    IntPtr handle = Marshal.AllocHGlobal(1024);
}
```

### c) Continuously create instances

I encountered situations when the application was constantly creating new instances of a specific class and was keeping references to all of them, even when the old ones were not needed anymore. Over the time, the unneeded instances were piling up and the application was using the entire memory resulting in an `OutOfMEmoryException`.

This is not a memory leak in the strict sense of the word. The memory was still referenced by variables and accessible by the application, that's why the Garbage Collector did not released it, but the effect is the same as the memory leaks described in the previous paragraphs.

> **Note**
>
> This third case (c) will will not be the topic for the current presentation.
>
> We will focus in the next sessions on the first (a) and second (b) cases.