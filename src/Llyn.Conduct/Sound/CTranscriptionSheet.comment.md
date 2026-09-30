# CTranscriptionSheet.cs

## `public sealed record CTranscriptionSheet(`

The editor's transcription block for the held draft, ready to paint.
The engine answers the rows with their schemes, so no driver matches schemes.

**Parameters**

- `CTranscriptionSheetShown`: whether the block shows, while the pack declares any scheme.
- `CTranscriptionSheetFree`: whether a row can be added, since some scheme is still free.
- `CTranscriptionSheetRows`: the rows outside the glyph scheme, blank ones kept.
