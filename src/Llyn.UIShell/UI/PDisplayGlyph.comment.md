# PDisplayGlyph.cs

## `public partial class PDisplay`

The glyph row of the reading view: the Han characters of a shown entry, each a link to its entry.
The characters come from the entry's glyph row when it holds text, else from the headword itself.
So a traditional headword needs no lookup, and a Japanese headword shows its kanji with the kana dropped.

## `private void PDisplayGlyphShow(LEntryDraft draft)`

Rebuilds the chips from the draft, one per distinct Han character in order.
The pack's glyph section is asked once per render, and the row hides while the language declares none.
The glyph typography is put on the list, so the chips are drawn in the serif face the pack names.

## `private bool PDisplayGlyphCheck(string scheme)`

Whether a transcription scheme is the glyph scheme of the shown language.
The transcription rows ask it to leave the glyph row out.

## `private void PDisplayGlyphHandle(object sender, ExecutedRoutedEventArgs e)`

A chip was pressed: the host opens the character's entry in the glyph language, making it first when none exists.

## `private void PDisplayGlyphClear()`

Drops the chips and forgets the section, for the next entry to start clean.
