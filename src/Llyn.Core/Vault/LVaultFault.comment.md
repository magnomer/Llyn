# LVaultFault.cs

## `public sealed class LVaultFault : Exception`

The one exception a store adapter raises when the store itself fails, a file locked or a folder denied.
The engine catches it where it would carry on without the store, records it and keeps running.
A pure ring never names the file system, so it cannot catch the file system's own exceptions.
The adapter wraps them here and the cause travels as the inner exception.

## `public LVaultFault(Exception cause)`

Wraps the adapter's exception, keeping its message as this one's.
