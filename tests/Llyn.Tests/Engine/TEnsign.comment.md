# TEnsign.cs

## `public sealed class TEnsign`

The flag cache below the shell: which keys are unasked, which paths are kept, and what a stale age discards.
A broken file is forwarded to the usher once.
Each case builds its own cache over a fake usher, so no case touches a disk or another case's store.

## `private sealed class TUsherFake : LUsher`

Answers presence from a set the test fills, and records every path the cache asks it to delete.
