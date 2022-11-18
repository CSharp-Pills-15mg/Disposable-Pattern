# 03 - How to Safely Use the Unsafe Class?

In the previous session, we were discussing two possible issues with the current implementation:

- The consumer of our class may forget to call `Close()`;
- An exception may be thrown, that prevents the execution of the `Close()` method.

In this session we will discuss what the consumer of our class may do to prevent these possible issues.

> **Note**
>
> There are things that we can do in the initial class itself to prevent an unsafe usage, but that will be discussed in a future session.

## Issue 1 - Forget to call `Close()`

We are humans and, from time to time, we do mistakes. After all, doing mistakes is normal, it is the way humans learn from the moment we are born. The important think is not to do the same mistake twice.

But, what can we do to prevent or, at least, reduce such mistakes? Well, one thing that comes in my mind would be to help those around us to learn better ways of writing code, isn't it?

Ok, ok, I understand we have limited control regarding the others' actions. Then, is there something that we can do, which is under our control? Actually, it is. We have full control of the class that we initially wrote and there are things that we can do there to prevent this human error.

We'll discuss this in another session. In this session, let's focus on what the consumer can do:

- For now, the only thing the consumer can do for preventing this issue (Issue 1) is to remember to call the `Close()` method (:P)

## Issue 2 - Exception prevents the `Close()`

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

### Possible solution: try-finally

Putting the `Close()` method into the `finally` block, will ensure it will always be called:

```csharp
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

Problem solved, isn't it?

Or is it?

### Not really a solution

This solution depends completely on the way the consumer will use our class. This is not a desirable situation.

A better solution would be to find a way to prevent unsafe usage of our class from the start, instead of hoping that the consumer will know how to use it safely.

## Conclusions

The discussed solutions depend completely on the knowledge, skills and memory of the consumer. If the consumer doesn't know how to safely use our class or forgets to use the `try-finally` block, we are back to square 1: memory leak :(

In the next sessions we will discuss what can be done in the class itself in order to prevent unsafe usage.
