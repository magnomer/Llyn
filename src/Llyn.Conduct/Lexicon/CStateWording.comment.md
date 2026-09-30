# CStateWording.cs

## `public sealed record CStateWording(string CStateWordingText, string? CStateWordingKey, bool CStateWordingMuted)`

A written value as a list row or a read page shows it, ready.
It carries its text, or the key the driver looks up instead.
The driver shows the text while the key is null, and never judges the value's state itself.

**Parameters**

- `CStateWordingText`: the value's text, empty when nothing legible is written.
- `CStateWordingKey`: the key worded in place of the text, null while the text itself shows.
- `CStateWordingMuted`: whether the value was never written, so a page mutes or hides it.

## `internal static CStateWording LStateWordingRead(CStateValue value, string? unset)`

An unknown value reads the unknown mark, and a value with no text reads the key its owner names.
An owner that words nothing for an unwritten value names no key, and its driver hides the value instead.
The state verdicts arrive from the engine on the value, so only the choice of key happens here.
