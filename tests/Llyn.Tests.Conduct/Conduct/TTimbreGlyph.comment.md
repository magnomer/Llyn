# TTimbreGlyph.cs
Hash: `e35119dfe12666e8`

## `public sealed class TTimbreGlyph`

Covers the editor's glyph block and its transcription text gate end to end on a real workspace.

A Korean draft splits its rows by the Hanja scheme, and the blank row stays among the transcription rows.
A language without a glyph section, or an empty desk, answers the hidden block.
The transcription text gate writes the typed text into the held glyph row, and a filling desk writes nothing.

## `private static CEditor TTimbreGlyphPrepare(LEngine engine, long entry)`

Opens the stored entry `entry` in an editor over the library vista.

## `private static long TTimbreGlyphSave(LEngine engine, string language)`

Stores a headword in `language` with a romanization, a Hanja row and a blank Yale row.
