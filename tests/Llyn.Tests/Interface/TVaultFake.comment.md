# TVaultFake.cs

## `internal sealed class TVaultFake : LEntryVault`

An in-memory entry vault backed by a dictionary.
It counts every read so a test can prove the engine went through it.
The operations the test never drives throw, so a stray call is a failure rather than a silent pass.
The finders answer empty, since the engine sweeps them at start and must find nothing.
`TRigFake` seats it as the entry port of a rig built from fakes.

## `internal LEntry TVaultFakeAdd(LEntry entry)`

Stores `entry` under the next id, as an archive would, and returns it with that id.
