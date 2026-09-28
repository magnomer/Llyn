# QLecternSound.cs

## `public sealed class QLecternSound`

The reading view's sound driver, drawing what [CDisplaySound](../../Llyn.Conduct/Display/CDisplaySound.comment.md) answers.
It draws the transcriptions, the glyph row and the reflex rows.
It fills the fanqie, script and paradigm controls through the seams handed over.
Each block arrives ready with its font, so no Refine asks Conduct twice.
[QLectern](QLectern.comment.md) subscribes its Refines to the area's open, close and notice events.
The primary pronunciation and the accents live on [QLecternAccent](QLecternAccent.comment.md).
The play button and the volume live on [QLecternPlayback](QLecternPlayback.comment.md).

## `public void QLecternGlyphIntroduce(ItemsControl transcriptions, UIElement section, ColumnDefinition lead, TextBlock label, ItemsControl glyph)`

Holds the glyph section, and binds the transcription and glyph lists to their rows.
The chip list is attached to `QGlyphItem.QGlyphItemRefine`, which fills each chip.
The transcription list is attached to `QTranscriptionItem.QTranscriptionItemRefine` the same way.

## `public void QLecternReflexIntroduce(ItemsControl reflex, UIElement loading, ToggleButton fold)`

Binds the reflex list to its rows and holds the loading line and the fold toggle.
The list is attached to `QReflexItem.QReflexItemRefine`, which fills each row.
The fold repaint is subscribed to the area's fold change, so every lectern follows one toggle.

## `public void QLecternFanqieIntroduce(DependencyObject fanqie, TextBlock reading, Action<IReadOnlyList<CFanqieGroup>, bool> fanqieSeam)`

Holds the fanqie box for its font, the reading line and the seam the box owns.
`fanqieSeam` draws the groups and the pending state, so no veneer type is named here.

## `internal void QLecternRouteIntroduce(PWindow host)`

Holds the window, whose openers the glyph, category and stem gates open through.
The card half holds the window the same way for its chips.

## `public void QLecternScriptIntroduce(DependencyObject script, Action<IReadOnlyList<CScriptGroup>, bool> scriptSeam)`

Holds the script box for its font and the seam that draws its groups.

## `public void QLecternParadigmIntroduce(DependencyObject paradigm, Action<IReadOnlyList<CParadigmSlot>, bool, bool> paradigmSeam)`

Holds the paradigm box for its font and the seam that draws its slots.

## `public void QLecternGlyphRefine()`

Rebuilds the chips from the ready glyph row, and hides the section while it has no cell.
The glyph font goes into the list's resources, so the chips take it and the label does not.
The heading is looked up from the key Conduct chose, with the scheme's name as the fallback.

## `public void QLecternTranscriptionRefine()`

Rebuilds the transcription rows from the area's answer.

## `public void QLecternReflexRefine()`

Draws the reflex block of an entry just opened.

## `public void QLecternRenewalRefine()`

Draws the reflex block again after a reflex fill, through the area's resonate, which reloads the rows.

## `public void QLecternFoldRefine()`

Hides or shows the folded rows from the shared fold, and sets the toggle to match.
It runs after the rows are rebuilt and whenever the fold gate changes the fold.
Writing the toggle back to the same value lets its event settle at once.

## `public void QLecternFanqieRefine()`

Draws the fanqie box, the reading and the reflex anchors from one ready block.
It runs after the reflex rows, so the anchors land on the rows just drawn.

## `public void QLecternScriptRefine()`

Draws the script box with its font and whether a fetch runs.

## `public void QLecternParadigmRefine()`

Draws the paradigm box with the font of the paradigm's own language.

## `public void QLecternSilenceRefine()`

Empties every row and collapses the glyph section when the entry closes.
The reflex rows, the fold, the loading line and the reading empty, and the seams draw nothing pending.

## `public void QLecternFoldObserve()`

Hears the fold toggle and hands its state to the fold gate.

## `public void QLecternDiweiObserve(string kind, string key)`

Hears a rime-cell click in the fanqie box and hands it to the gate with the window's category opener.

## `public void QLecternStemObserve(string? key)`

Hears a phonetic-series click in the fanqie box and hands it to the gate with the window's stem opener.

## `public void QLecternGlyphObserve(object parameter)`

Hears a chip's command and hands the chip's character and language to the glyph gate.
The window's entry opener goes with them, so the gate opens what it resolved.
An inert chip or anything else is ignored, since only a linked chip opens an entry.

## `private void QLecternReflexRefine(CLecternReflex reflex)`

Rebuilds the rows, writes their ready anchors and shows the loading line while a fill runs.
