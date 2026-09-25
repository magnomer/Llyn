# LLecternSound.cs

## `public sealed class LLecternSound`

The reading view's sound deportment, standing between the veneer and [LDisplaySound](../../Llyn.Conduct/Display/LDisplaySound.comment.md).
It draws the transcriptions and the glyph row.
It draws the reflex rows and fills the fanqie, script and paradigm controls through the seams handed over.
Fonts come from `LFontFace`.
The primary pronunciation and the accents live on [LLecternAccent](LLecternAccent.comment.md).
The play button and the volume live on [LLecternPlayback](LLecternPlayback.comment.md), which shares the display's sound.

## `public void LLecternGlyphAttach(LWindow window, ItemsControl transcriptions, UIElement section, ColumnDefinition lead, TextBlock label, ItemsControl glyph, Action<string, string> glyphSeam)`

Holds the window and the glyph section, and binds the transcription and glyph lists to their rows.
`glyphSeam` opens a character's entry in a language, and the window owns it.

## `public void LLecternReflexAttach(ItemsControl reflex, UIElement loading, ToggleButton fold)`

Binds the reflex list to its rows and holds the loading line and the fold toggle.

## `public void LLecternFanqieAttach(DependencyObject fanqie, TextBlock reading, Action<IReadOnlyList<LFanqieGroup>, bool> fanqieSeam, Action<string, string, string> diweiSeam, Action<string, string?> stemSeam)`

Holds the fanqie box for its font, the reading line and the seams the box and the window own.
`fanqieSeam` draws the groups and the pending state, so no veneer type is named here.
`diweiSeam` and `stemSeam` open a category or a stem in the shown entry's language.

## `public void LLecternScriptAttach(DependencyObject script, Action<IReadOnlyList<LScriptGroup>, bool> scriptSeam)`

Holds the script box for its font and the seam that draws its groups.

## `public void LLecternParadigmAttach(DependencyObject paradigm, Action<IReadOnlyList<LParadigmSlot>, bool, bool> paradigmSeam)`

Holds the paradigm box for its font and the seam that draws its slots.

## `public void LLecternReflexUpdate()`

Re-reads the shown entry after a reflex fill and redraws the rows, their anchors and the loading line.
Nothing shown means nothing to redraw.

## `public void LLecternFanqieUpdate()`

Redraws the fanqie box and the reading after a fanqie fetch, while an entry is shown.

## `public void LLecternScriptUpdate()`

Redraws the script box after a script fetch, while an entry is shown.

## `public void LLecternParadigmUpdate()`

Redraws the paradigm box after an inflection fetch, while an entry is shown.

## `public void LLecternFoldHandle(bool opened)`

Sets the shared fold state to `opened`, as the toggle reads, then hides or shows the folded rows.
Applying the rows writes the toggle back to the same value, so its event settles at once.

## `public void LLecternDiweiShow(string kind, string key)`

Opens the category a fanqie click names, in the shown entry's language.

## `public void LLecternStemShow(string? key)`

Opens the stem a fanqie click names, in the shown entry's language.

## `public void LLecternFanqieSet(long fanqieId, int rank)`

Asks for a fanqie row to stand as the shown entry's representative.

## `public void LLecternSoundShow()`

Draws the glyph row, then the transcriptions.
It then starts the reflex, script and fanqie fetches and draws every phonology section.

## `public void LLecternSoundClear()`

Empties every row and collapses the glyph section.
The reflex rows, the fold, the loading line and the reading empty, and the seams draw nothing.

## `public void LLecternGlyphHandle(object parameter)`

Opens the entry of the chip the command carries, and ignores an inert chip or anything else.

## `private void LLecternGlyphShow()`

Rebuilds the chips from the cells, and hides the row while the language declares no section.
The glyph typography goes into the list's resources, so the chips take it and the label does not.

## `private void LLecternTranscriptionShow()`

Rebuilds the transcription rows from the display's answer.

## `private void LLecternReflexShow()`

Rebuilds the reflex rows from the written reflexes, folding the languages the pack folds away.
The fold is applied last, from the shared state.

## `private void LLecternAnchorShow(IReadOnlyList<LFanqieGroup> groups)`

Writes each reflex row's anchor label from the rows of `groups`, under the shown headword.
The caller hands in blocks it already divided, so the anchor never divides again.

## `private void LLecternPendingShow()`

Shows the loading line while a reflex fill runs for the shown entry.

## `private void LLecternFanqieShow(IReadOnlyList<LFanqieGroup> groups)`

Rewrites the anchors, sets the glyph font and draws `groups` and the reading.
The caller divides once and hands the blocks in, so the anchors and the rime share one division.

## `private void LLecternScriptShow()`

Sets the glyph font and draws the script groups with whether a fetch runs.

## `private void LLecternParadigmShow()`

Reads the slots, starts the inflection fetch when morphology is on and draws the box.
The font follows the first slot's language, and no slots clear it.
