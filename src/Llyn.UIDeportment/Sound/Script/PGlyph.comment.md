# PGlyph.cs

## `public partial class PEditor`

The glyph row of the input panel: the traditional form of a Han-script headword, typed or looked up.
The form is stored as a transcription row in the pack's glyph scheme, so the row is a transcription item.
It is rendered apart from the transcription rows, beneath them, with no scheme dropdown and no plus or minus.
Its text edits flow through the transcription change handler, so no request of its own exists.

## `internal async void PGlyphNotationHandle(object sender, ExecutedRoutedEventArgs e)`

Opens the lookup menu under the row, searching in the glyph scheme.
The engine finds that scheme's sources in the pack's glyph section.

## `private void PGlyphApply(FrameworkElement container, object item, string? _)`

Fills one row of `Theme.Glyph.Row`: the scheme name, the measure twin, the field and the lookup button.
The lookup button shows only when the list tag says the pack declares sources.

## `private static void PGlyphMeasureApply(TextBlock measure, string text)`

The twin takes the text, or the glyph placeholder while the field is blank.

## `private static void PGlyphFieldHandle(object sender, TextChangedEventArgs e)`

Writes the typed text back to the row, which sends it on as the binding did.

## `private bool PGlyphSchemeCheck(string scheme)`

Whether a transcription scheme is the glyph scheme of the language shown.
The transcription rows ask it to leave the glyph row out.

## `private static bool PGlyphSchemeCheck(LGlyph? glyph, string scheme)`

Whether a transcription scheme is the scheme of `glyph`, which may be absent.
The prepare pass reads the section off the draft's language before the form shows it.

## `private void PGlyphShow(LEntryDraft draft)`

Renders the glyph row from the draft's row in the glyph scheme, keyed by its transcription id.
The pack's glyph section is asked once per render, and the row is shown only while the language declares one.
The list's tag says whether the section declares sources, so the template can hide the lookup button.
The glyph typography goes into the list's resources, so the field takes it and the scheme label does not.

## `private void PGlyphAttach()`

Hands the glyph list its rows and fill, and binds the lookup command on the sound panel.
