# TPosture.cs
Hash: `57c7fc16ea6b3bf0`

## `public sealed class TPosture`

Covers the posture the session keeps beside the settings file: each tab's listing, open tab, split and volume.
Every panel's ordering and filter reaches the posture through its vista, which announces and never saves.
Each push lands in that tab's layout record, and an ordering and a filter share one record.
A restarted vista opens on what was stored.
So the round trip is checked through the vista rather than the file.
A typed query or a chosen row announces too, but moves no stored field, so nothing is written.
An equal volume saved again writes nothing.
A volume that is not a number is ignored and writes nothing.
A workspace with only the legacy settings file has its posture migrated and written beside it.
A volume set without a save reads the new level and writes nothing.
A posture store whose save throws `LVaultFault` has each fault recorded in the audit.
Two failing saves raise `LPostureSaveFailed` once, with the first fault.
The posture across a workspace open lives in `TPostureWorkspace`.
The loader round-trips an ordering by its stored name, and reads nothing, junk and unusable keys as defaults.
The loader clamps a stored volume to the range zero to one.
