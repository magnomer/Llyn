# QGlyph.cs
Hash: `2def43d2f0684beb`

## `internal sealed class QGlyph`

The editor's driver for the glyph row of the input panel.
The row is the traditional form of a Han-script headword, typed or looked up.
The form is stored as a transcription row in the pack's glyph scheme, so the row is a transcription item.
It is rendered apart from the transcription rows, beneath them, with no scheme dropdown and no plus or minus.
Its typed text goes to the one transcription text gate, `CTranscription.CTranscriptionSet`.

## `internal QGlyph(FrameworkElement surface, QNotation notation)`

Hands the glyph list its rows and fill, and binds the lookup command on the sound panel.
The lookup menu is the editor's one `QNotation`.

## `internal void QGlyphIntroduce(CErrand errand, CEntry entry, CTimbre timbre, CTranscription transcription)`

Holds the errand, timbre and transcription facets the row reaches.
It repaints the row after each entry draft change.
The row's typography is written by `QEditorFont`.

## `private void QGlyphNotationRefine(object sender, ExecutedRoutedEventArgs e)`

Opens the lookup menu under the glyph row.
It is subscribed before `QGlyphNotationObserve`, so the old search closes first.

## `private void QGlyphNotationObserve(object sender, ExecutedRoutedEventArgs e)`

Asks the errand for a search on the row in its own scheme, which is the glyph scheme.
The engine finds that scheme's sources in the pack's glyph section.

## `private void QGlyphItemRefine(FrameworkElement container, object item, string? _)`

Fills one row of `Theme.Glyph.Row`, which holds the scheme name, the measure twin, the field and the lookup button.
The field is filled while its typing is not heard, so a repaint sends nothing.
The lookup button shows only when the list tag says the pack declares sources.

## `private static void QGlyphMeasureRefine(TextBlock measure, string text)`

The twin takes the text, or the glyph placeholder while the field is blank.

## `private void QGlyphFieldObserve(object sender, TextChangedEventArgs e)`

Hands the row's id and the raw typed text to the transcription text gate.
The row keeps the draft's text until the draft returns with the typed one.

## `private void QGlyphRefine(CEntryDraft _)`

Paints the glyph block from `CTimbre.CTimbreGlyphRead` on every draft repaint, keyed by the transcription id.
The row shows only while the language declares a glyph section.
The list's tag says whether the section declares sources, so the template can hide the lookup button.
The rows are built without the transcription change handler, since the field hears its own typing.
The rows take the draft's text before the full refill, so each field gets the text it holds.
A refill from the previous draft's text would blank a typed field and move its caret to the start.
