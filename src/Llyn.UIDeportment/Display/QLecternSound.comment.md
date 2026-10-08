# QLecternSound.cs
Hash: `503903e110ee8e89`

## `public sealed class QLecternSound`

The reading view's phonology section, drawing the fanqie, script and paradigm boxes [CDisplaySound](../../Llyn.Conduct/Display/CDisplaySound.comment.md) answers.
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

## `public QLecternSound(FrameworkElement surface, CDisplaySound area, CLedger ledger, CEnvoy envoy)`

Holds the fanqie, script and paradigm boxes and the reading line, all pulled from `surface`.
`area` is the display's sound area, the only part it reads.
Each box is held as its own type, so it draws through its own member.
The fanqie's category and stem notices go to this section's observers, so a click reaches the shown entry's language.
Its representative notice goes straight to the sound area's gate, since the notice already carries raw values.
The script box's failure notice shows through `ledger` and `envoy` under `Display.ScriptFailed`.

## `public void QLecternFanqieRefine()`

Draws the fanqie box and the reading from one ready block.
The reflex anchors a fanqie change moves are rewritten by the reflex section, which the lectern subscribes first.

## `public void QLecternScriptRefine()`

Draws the script box with its font and whether a fetch runs.

## `public void QLecternParadigmRefine()`

Draws the paradigm box with the font of the paradigm's own language.
The lectern subscribes the paradigm, script and fanqie Refines to open and close, in that order.
A closed display answers empty blocks, so the same Refines empty the boxes and the reading.
Each also answers its own notice, marshalled onto the page through `QObserver`.

## `private void QLecternDiweiObserve(bool initial, string key)`

Hears a rime-cell click in the fanqie box and hands it to the gate.
The gate raises the choice with the shown entry's language when an entry is shown.

## `private void QLecternStemObserve(string? key)`

Hears a phonetic-series click in the fanqie box and hands it to the gate.
The gate raises the choice with the shown entry's language when an entry is shown.
