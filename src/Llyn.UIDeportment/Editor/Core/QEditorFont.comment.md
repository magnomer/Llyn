# QEditorFont.cs

## `internal sealed class QEditorFont`

The editor's typefaces: every font the language's timbre names for the editor is written here.
The headword and its measuring twin, the example and gloss typography, and the glyph row each take their own role.
Sound keeps only sound, so `QEditorSound` and `QGlyph` read no font.

## `internal QEditorFont(FrameworkElement surface)`

Only holds the editor scope, since every write waits for a draft.

## `internal void QEditorFontIntroduce(CEditor editor)`

Holds the Conduct editor and repaints each typeface after every draft change.

## `private void QEditorHeadwordRefine(CEntryDraft _)`

The headword and its measuring twin take the headword typeface of the language, on one baseline.

## `private void QEditorGlossRefine(CEntryDraft _)`

The gloss typography goes into the editor's resources, beside the example typography of `QEditorExampleRefine`.
Each reads its own font from the timbre, so each Refine asks Conduct once.

## `private void QEditorGlyphRefine(CEntryDraft _)`

The glyph typography goes into the glyph list's resources, so the field takes it and the scheme label does not.
