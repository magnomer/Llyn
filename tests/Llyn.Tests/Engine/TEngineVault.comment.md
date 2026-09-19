# TEngineVault.cs

## `public sealed class TEngineVault`

Covers the engine reading an entry through its port rather than through the database.
It is the first engine test that runs against no SQLite row at all.
That is what the port exists for.
A fake vault holding one entry answers the read, and the workspace database stays empty.
An id the fake does not hold comes back as `null`, with the fake still asked exactly once.

## `private sealed class TVaultFake : LEntryVault`

An in-memory entry vault backed by a dictionary.
It counts every read so a test can prove the engine went through it.
The operations the test never drives throw, so a stray call is a failure rather than a silent pass.
The finders answer empty, since the engine sweeps them at start and must find nothing.

## `internal LEntry TVaultFakeAdd(LEntry entry)`

Stores `entry` under the next id, as an archive would, and returns it with that id.
