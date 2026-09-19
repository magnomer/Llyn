# TPosture.cs

## `public sealed class TPosture`

Covers the posture the window keeps beside the settings file: geometry, tab layout, linked panels, open tab, split and volume.
Every panel's ordering and filter reaches the posture through its vista, which announces and never saves.
Each push lands in that tab's layout record, and a dragged width and a chosen ordering share one record.
A restarted vista opens on what was stored.
So the round trip is checked through the vista rather than the file.
A typed query or a chosen row announces too, but moves no stored field, so nothing is written.
An equal geometry saved twice writes the file once.
A workspace with only the legacy settings file is read once and its posture written beside it.
A posture file standing beside the legacy one wins.
A workspace moved onto keeps its own posture, and one without any inherits the posture held.
The loader round-trips an ordering by its stored name, and reads nothing, junk and unusable keys as defaults.
