# CStateWording.cs

## `public sealed record CStateWording(string CStateWordingText, string? CStateWordingKey)`

A written value as a list row shows it, ready: its text, or the key the driver looks up instead.
The driver shows the text while the key is null, and never judges the value's state itself.

**Parameters**

- `CStateWordingText`: the value's text, empty when nothing legible is written.
- `CStateWordingKey`: the key worded in place of the text, null while the text itself shows.

## `internal static CStateWording LStateWordingRead(CStateValue value, string unset)`

An unknown value reads the unknown mark, and a value with no text reads the key its owner names.
The state verdicts arrive from the engine on the value, so only the choice of key happens here.
