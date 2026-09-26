# TEngineVault.cs

## `public sealed class TEngineVault`

Covers the engine reading an entry through its port rather than through the database.
It is the first engine test that runs against no SQLite row at all.
That is what the port exists for.
The engine starts over `TRigFake`, so no workspace folder and no database exist.
A fake vault holding one entry answers the read.
An id the fake does not hold comes back as `null`, with the fake still asked exactly once.
