# LTranscriptionSheet.cs
Hash: `968b1aed56c83ea4`

## `public sealed record LTranscriptionSheet(`

The transcription block of the held draft, answered ready by the engine.
The glyph rows are left out, since the glyph block shows them.

**Parameters**

- `LTranscriptionSheetShown` — True when the draft's language declares any scheme.
- `LTranscriptionSheetScheme` — The first scheme no row holds, which a new row takes, or null when none is free.
- `LTranscriptionSheetRows` — The rows outside the glyph scheme, in the draft's order, blank rows included.

## `public bool LTranscriptionSheetFree`

Whether a new row can still be added, since some scheme is free.
