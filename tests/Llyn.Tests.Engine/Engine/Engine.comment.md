# Engine

These tests cover the engine's clock, observers, rig of fakes, stamp times, vault reads and flag cache.
They run over real temporary workspaces or over a rig of fakes.
The fakes stand where no disk or web should be touched.
Entry storage, loading and markup live in the sibling Entry folder.
Each file lists the promise a panel or a surface relies on.

## `TEngineClock.cs`

It covers the clock port the engine stamps a moment with.
A frozen rig clock stamps a held entry draft and a held author draft with the frozen moment.
So every timestamp the engine writes is the rig's to set.
No engine test depends on the wall clock.

## `TEngineObserver.cs`

It covers the engine's subscription as a delegate rather than a contract.
A lambda attached hears one bulletin when a setting is saved.
A method group detached with the delegate it was attached with falls silent, since equality is target plus method.
A surface that closes must stop being called, or it would be told about state it no longer shows.
The same method group attached twice is one subscriber, so a re-attached surface keeps one voice.
A tenure keeps its state until an announcement moves the engine revision.
A second reading with nothing written between answers the very same state, so a refresh skips the disk.
An applied request or an undo announces the draft, so the next reading computes a fresh state.
A store by another tenure moves the stored record, so the watching tenure computes again.
A cancelled tenure answers the fixed ended state, never the state it kept while alive.

## `TEngineRig.cs`

It covers the engine standing on a rig of fakes, with no file on disk.
A start reads the settings and the rescue through the fake ports and names the rig's root.
An entry created through the engine lands in the fake entry vault and reads back from it.
A rig apply moves the engine onto the second rig, so the next read hits the second fake.
The settings the second rig never held are inherited, as a fresh folder inherits them.
A second rig whose default settings cannot be written still opens with the inherited settings.
The write's vault fault is recorded once and goes no further.

## `TEngineStamp.cs`

It covers the reading view's stamp times, which the engine words for Conduct.
A stored entry answers both times already worded.
An id no entry has answers not stored with empty times, so the view hides its stamp row.
A stamp that does not parse, or none at all, shows as nothing.
The Korean fact sets the machine culture to Korean and reads the stamp for the interface language.
  The added time is worded in the English short form, so the machine culture never words it.

## `TEngineVault.cs`

It covers the engine reading an entry through its port rather than through the database.
The engine starts over `TRigFake`, so no workspace folder and no SQLite row exist.
A fake vault holding one entry answers the read.
An id the fake does not hold comes back as `null`, with the fake still asked exactly once.

## `TEnsign.cs`

It covers the flag cache below the shell.
The cases cover which keys are unasked, which paths are kept, and what a stale age discards.
A broken file is forwarded to the usher once, and a locked one is kept.
A deleted path is asked again, a clear during the store commits nothing, and uneven paths throw.
The engine keys a named variety's flag through the same format, and a blank variety to nothing.
Each cache case builds its own cache over a fake usher, so none touches a disk or another's store.
The unflagged variety case and the blocked flag folder case start an engine on a temporary workspace.
The engine's fill still answers the languages over a flagged pack.
This holds though a file blocks the workspace flag folder.
That disk failure is answered by the flag loader in Infrastructure, so the outlet only forwards.
The engine runs one fill at a time, so a second overlapping fill finds the flag already kept.
The fetch waits on a completion the case sets, so the overlap never depends on timing.

## `TLanguageFake`

It declares one variety with a flag code, and hands every fetch the one pending task the case completes.
It counts the fetches and scans no language, so the engine's start imports nothing.
Its stored flag find answers none.

## `TUsherFake`

It answers presence from a set the test fills, and records every path the cache asks it to delete.
It judges an input or output failure a lock.

## Stamp

Commit: `9377e657b395`
- `TEngineClock.cs` `8989e402d29be091`
- `TEngineObserver.cs` `e4fab726a273bd31`
- `TEngineRig.cs` `8a63b1df2c9274ab`
- `TEngineStamp.cs` `17909bda313a9240`
- `TEngineVault.cs` `d9d6fd5e5587e548`
- `TEnsign.cs` `a60a8e4d1e0348e2`
