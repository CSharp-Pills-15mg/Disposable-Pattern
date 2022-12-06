# Disposable Pattern in C#

## Table of Contents

**01 - Memory Leaks**

- What are they?
- How to create one in C#?

**02 - Create the Unsafe Class**

- Let's create a class containing unmanaged resources (manually allocated memory).
- How to ensure the memory is released when it is no longer needed?

**03 - How to Safely Use the Unsafe Class?**

- How the consumer of our class should use it?
- But is it safe to rely only on the consumer's skills and knowledge?
  - Can we do something in the class's implementation to prevent unsafe usage?

**04 - `IDisposable` Interface and the `using` Statement**

- .NET offers a standard way to deal with releasing unmanaged resources.

**05 - Adding a Finalizer**

- The finalizer is a fail safe that can clean the unmanaged resources if they were not manually cleaned.

**06 - Fixing the Finalizer**

- Improving the performance by suppressing the execution of the finalizer when the resources were already manually cleaned. `Dispose()` was manually executed.

**07 - Prevent the Usage After Dispose**

- After the object was disposed, it is not safe to be used anymore. Let's prevent accidental usages.

**08 - Disposable Resources**

- Our class may have, beside unmanaged resources, also members that already implement the `IDisposable` interface. How do we handle them?

**09 - Derived Classes**

- We implemented the disposable pattern, but what happens if somebody inherits our class and it also needs to release unmanaged/managed resources? We should offer a mechanism for them to hook up into the already existing mechanism.

**10 - Full Implementation**

- And here is the full implementation of the base and derived classes.

**10.a - No Unmanaged Code**

- Often, in the real life, we do not use unmanaged resources directly. If this is the case, the code can be simplified.

**10.b - No Inheritors**

- The code of our class can be simplified also when we know for sure there will be no inheritors. Mark the class as `sealed` and remove the support for them.

**10.c - No Unmanaged Code and No Inheritors**

- If no inheritors will be created and, in addition, no unmanaged resources exist, the implementation gets considerably simpler.
