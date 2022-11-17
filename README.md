# Disposable Pattern

## Pill Category

Patterns, Frameworks (.NET Framework, .NET)

## Description

.NET provides a mechanism (Garbage Collector ) that keeps track of the memory allocated by .NET objects and automatically releases that memory when the objects are not referenced anymore.

But it cannot detect when memory is manually allocated, for example by some native code. That memory must be manually deallocated.

.NET offers support for these situation by providing the `IDisposable` interface, which, together with the finalizer method, can be used to reliably release the unmanaged resources (like manually allocated memory) when they are not needed anymore.

## Donations

> If you like my work and want to support me, you can buy me a coffee:
>
> [![ko-fi](https://www.ko-fi.com/img/githubbutton_sm.svg)](https://ko-fi.com/Y8Y62EZ8H)

