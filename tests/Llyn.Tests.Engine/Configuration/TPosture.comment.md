# TPosture.cs
Hash: `a645d8f2fa5e2f88`

## `public sealed class TPosture`

Covers the posture the session keeps beside the settings file: each tab's listing, open tab, split and volume.
Every panel's ordering and filter reaches the posture through its vista, which announces and never saves.
Each push lands in that tab's layout record, and an ordering and a filter share one record.
A restarted vista opens on what was stored.
So the round trip is checked through the vista rather than the file.
A typed query or a chosen row announces too, but moves no stored field, so nothing is written.
An equal volume saved again writes nothing.
A volume that is not a number is ignored and writes nothing.
A workspace with only the legacy settings file is read once and its posture written beside it.
A posture file standing beside the legacy one wins.
A workspace moved onto keeps its own posture, and one without any inherits the posture held.
One moved onto with only the legacy settings file yields its mode and volume.
A volume set without a save reads the new level and writes nothing.
A posture file that will not read leaves the posture held and the file as it was.
A posture store whose save throws `LVaultFault` has each fault recorded in the audit.
Two failing saves raise `LPostureSaveFailed` once, with the first fault.
A failing save after a workspace opens raises it again, since each open clears the failed flag.
The loader round-trips an ordering by its stored name, and reads nothing, junk and unusable keys as defaults.
