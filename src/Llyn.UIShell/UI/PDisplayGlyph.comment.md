# PDisplayGlyph.cs

## `public partial class PDisplay`

The glyph row of the reading view, holding the same text as the editor field.
Each Han character is a link to its entry.
The characters come from the entry's glyph row when it holds text, else from the headword itself.
So a traditional headword needs no lookup, and a Japanese headword shows its kanji with the kana dropped.

## `private void PDisplayGlyphShow(LEntryDraft draft)`

Rebuilds the chips from the draft, one per rune in order.
So the row reads exactly as the editor field does.
Only a Han rune carries the glyph language, and the others stay inert.
The pack's glyph section is asked once per render, and the row hides while the language declares none.
The glyph typography goes into the list's resources, so the chips take it and the label does not.

## `private bool PDisplayGlyphCheck(string scheme)`

Whether a transcription scheme is the glyph scheme of the shown language.
The transcription rows ask it to leave the glyph row out.

## `private void PDisplayGlyphHandle(object sender, ExecutedRoutedEventArgs e)`

A chip was pressed: the host opens the character's entry in the chip's language, making it first when none exists.
An inert chip carries no language and is ignored.

## `private void PDisplayGlyphClear()`

Drops the chips and forgets the section, for the next entry to start clean.
