# LEnginePortrait.cs

## `public sealed partial class LEngine`

Assembles the likeness of one entry from what the workspace holds.
The panel reads the same records through the same engine, so both see one entry the same way.

## `public LPortrait LEnginePortraitRead(long entryId, LPortraitLabel label)`

Everything the display gathers is gathered here, in the display's own order.
The portrait carries the transcription and the names of the parts of speech, because a reader reads names.
The reflex lines follow the transcription lines in the same list, each labelled by its language and kind.
The value ids the entry is filed under stay in the draft.
A writer that needs them can find them there.
A missing entry is an error, because a caller asked to portray one that no longer stands.
Translations, incoming links and the favourite mark are read defensively.
None of them is the entry itself, and none is worth refusing an export over.

## `private LSentenceOrder LEngineFrameRead(string language)`

An unknown or unknown language falls back to the default order, exactly as the panel does.
