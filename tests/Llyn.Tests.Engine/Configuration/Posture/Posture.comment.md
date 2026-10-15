# Posture

This folder holds the tests of the window posture beside the settings file.
They cover its keep-file adapter and which file wins per workspace.

## `TPosture.cs`

Covers the posture the session keeps beside the settings file.
It holds each tab's listing, open tab, split and volume.
Every panel's ordering and filter reaches the posture through its vista, which announces and never saves.
Each push lands in that tab's layout record, and an ordering and a filter share one record.
A restarted vista opens on what was stored.
So the round trip is checked through the vista and not the file.
A typed query or a chosen row announces too, but moves no stored field, so nothing is written.
An equal volume saved again writes nothing.
A volume that is not a number is ignored and writes nothing.
A volume set without a save reads the new level and writes nothing.
A workspace with only the legacy settings file has its posture migrated and written beside it.
A posture store whose save throws `LVaultFault` has each fault recorded in the audit.
Two failing saves raise `LPostureSaveFailed` once, with the first fault.
The loader round-trips an ordering by its stored name.
The loader reads nothing, junk and unusable keys as defaults.
The loader clamps a stored volume to the range zero to one.
The posture across a workspace open lives in `TPostureWorkspace`.

## `TPostureFile.cs`

Covers the posture adapter over a keep file.
No file reads as nothing, and a saved state reads back equal under `posture.json`.
The legacy `settings` key reads the same shape.
A file the system will not open surfaces as `LVaultFault`, the one exception the engine catches.

## `TPostureWorkspace.cs`

Covers which posture file wins when a workspace starts or another one opens.
A posture file standing beside the legacy one wins.
A workspace moved onto keeps its own posture, and one without any inherits the posture held.
One moved onto with only the legacy settings file yields its mode and volume.
A posture file that will not read leaves the posture held and the file as it was.
A failing save after a workspace opens raises `LPostureSaveFailed` again, since each open clears the failed flag.
A failing read at start is recorded once and raised once as `LPostureReadFailed` with the first fault.

## Stamp

Commit: `2f9a06b8eb95`
- `TPosture.cs` `57c7fc16ea6b3bf0`
- `TPostureFile.cs` `b37706d39b7c30e7`
- `TPostureWorkspace.cs` `6904557928566577`
