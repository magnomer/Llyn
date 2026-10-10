# QLecternSound.cs
Hash: `bc1e971efd7bdf17`

## `public sealed class QLecternSound`

The reading view's phonology section, drawing the fanqie, script and paradigm boxes [CDisplaySound](../../Llyn.Conduct/Display/CDisplaySound.comment.md) answers.
The box openings come from the display's [CFold](../../Llyn.Conduct/Sound/CFold.comment.md), as in the editor.
[QLectern](QLectern.comment.md) builds it once in its constructor over the view's page.
It pulls its own boxes by contract ID and hears their notices.
The lectern subscribes its redraws to the display's open, close and notices.
Each block arrives ready with its font, so no Refine asks Conduct twice.
It reads each block itself and turns it into the box's own rows, as `QCadence` does in the editor.
So no box takes a Conduct list, and each box stays a plain view of rows and flags.
The glyph row lives on [QLecternGlyph](QLecternGlyph.comment.md), the transcriptions on [QLecternTranscription](QLecternTranscription.comment.md).
The reflex rows live on [QLecternReflex](QLecternReflex.comment.md).
The primary pronunciation and the accents live on [QLecternAccent](QLecternAccent.comment.md).
The play button and the volume live on [QLecternPlayback](QLecternPlayback.comment.md).

## `public QLecternSound(FrameworkElement surface, CDisplaySound area, CFold fold, CLedger ledger, CEnvoy envoy)`

Holds the fanqie, script and paradigm boxes and the reading line, all pulled from `surface`.
`area` is the display's sound area, which answers the blocks.
`fold` is the display's box fold area over the shown entry, which owns both box openings.
Each box is held as its own type, so it draws through its own member.
The fanqie's category and stem notices go to this section's observers, so a click reaches the shown entry's language.
Its representative notice goes straight to the sound area's gate, since the notice already carries raw values.
The script box's failure notice shows through `ledger` and `envoy` under `Display.ScriptFailed`.
It hooks both box switches' clicks itself, as `QCadence` does in the editor.
So no box raises a callback that only forwards a click.

## `public void QLecternBoxRefine()`

Paints the stored opening of the rime-book and script boxes for the shown entry.
Both boxes sit in folded layout, so the head shows the switch and the body follows the stored state.
The editor writes the same stored state, so both modes show one opening.
The lectern subscribes it to open, close and the display's fold notice.
So a fold written in the editor repaints here, and a closed display paints both closed.

## `public void QLecternFanqieRefine()`

Draws the fanqie box and the reading from one ready block.
The reflex anchors a fanqie change moves are rewritten by the reflex section, which the lectern subscribes first.

## `public void QLecternScriptRefine()`

Draws the script box with its font and whether a fetch runs.

## `public void QLecternParadigmRefine()`

Draws the paradigm box with the font of the paradigm's own language.
It maps the slot rows and the inflection view into Deportment items, then hands the box those.
The sheet is null for a pack without a layout.
The lectern subscribes the paradigm, script and fanqie Refines to open and close, in that order.
A closed display answers empty blocks, so the same Refines empty the boxes and the reading.
Each also answers its own notice, marshalled onto the page through `QObserver`.

## `private void QLecternSpellingObserve(object sender, RoutedEventArgs e)`

Hears a click on the rime-book switch and hands its new state to the fold area's gate.
The verdict goes to `QLook.QLookCheckedRefine`, which restores the switch on a refusal.
The body repaints later from the store, through the display's fold notice.

## `private void QLecternWritingObserve(object sender, RoutedEventArgs e)`

Hears a click on the script switch with the same gate, verdict and repaint path as the rime-book switch.

## `private void QLecternDiweiObserve(bool initial, string key)`

Hears a rime-cell click in the fanqie box and hands it to the gate.
The gate raises the choice with the shown entry's language when an entry is shown.

## `private void QLecternStemObserve(string? key)`

Hears a phonetic-series click in the fanqie box and hands it to the gate.
The gate raises the choice with the shown entry's language when an entry is shown.
