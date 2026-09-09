# LEnginePortrait.cs

## `public sealed partial class LEngine`

Assembles the likeness of one entry from what the workspace holds.
The panel reads the same records through the same engine, so both see one entry the same way.

## `public LPortrait LEnginePortraitRead(string entryId, LPortraitLabel label)`

Everything the display gathers is gathered here, in the display's own order.
A missing entry is an error, because a caller asked to portray one that no longer stands.
Translations, incoming links and the favourite mark are read defensively.
None of them is the entry itself, and none is worth refusing an export over.

## `private LSentenceOrder LEngineFrameRead(string language)`

An unknown or unreadable language falls back to the default order, exactly as the panel does.
