# TPosture.cs

## `public sealed class TPosture`

Covers the posture the window keeps beside the settings file: geometry, tab layout, linked panels, open tab, split and volume.
Every panel's ordering and filter reaches the posture through its vista, which announces and never saves.
Each push lands in that tab's layout record, and a dragged width and a chosen ordering share one record.
A restarted vista opens on what was stored.
So the round trip is checked through the vista rather than the file.
A typed query or a chosen row announces too, but moves no stored field, so nothing is written.
An equal geometry saved twice writes the file once.
An equal volume saved again writes nothing.
A volume that is not a number is ignored and writes nothing.
A workspace with only the legacy settings file is read once and its posture written beside it.
A posture file standing beside the legacy one wins.
A workspace moved onto keeps its own posture, and one without any inherits the posture held.
One moved onto with only the legacy settings file yields its window, mode and volume.
The settings file is not rewritten before the posture reads it.
A posture file that will not read leaves the posture held and the file as it was.
The loader round-trips an ordering by its stored name, and reads nothing, junk and unusable keys as defaults.
