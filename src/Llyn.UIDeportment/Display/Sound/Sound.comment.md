# Sound

This folder holds the reading view's sound section drivers.
They cover pronunciation and accents, glyph row, playback, reflex rows, transcriptions and the fanqie, script and paradigm boxes.

## `QLecternAccent`

Conduct supplies the pronunciation block, keeping the choice of readings outside this section.
The surface and its label column go to one `QLecternLead`.

### `QLecternAccentRefine`

The lectern subscribes it to open and close, keeping pronunciation rows aligned with the shown entry.
The surface and label column collapse when the block holds nothing spoken.

### `QLecternEnsignRefine`

It is subscribed to open only, after the block's redraw.
Flags arrive after the rows are drawn, so each row then repaints its own flag.
A null load result leaves the current rows untouched.

### `QLecternPrimaryRefine`

A resolved flag replaces the primary label, avoiding both representations at once.

## `QLecternGlyph`

The chip command is bound here, so the sound strip holds no adapter.
The glyph font goes into the list's resources, so the chips take it and the label does not.
An inert chip is ignored, since only a linked chip reaches the open gate.

## `QLecternPlayback`

Both playback paths use the page's volume slider, keeping accent playback and the main button at the same volume.
The accent rows these buttons play are drawn by `QLecternAccent`.
A command parameter that is not an accent row is ignored.
An accent row plays by its row id, so presentation holds no audio payload.

## `QLecternReflex`

This section delegates row and anchor presentation to `QReflexList`.
Only the hinge click is heard, so a repaint that sets the hinge writes nothing back.
The lectern subscribes its redraws ahead of the fold refresh.

### `QLecternRenewalRefine`

A reflex bulletin uses the area's resonate path before drawing, rather than the ordinary read path.

### `QLecternFoldRefine`

It runs after the rows are redrawn and on the fold bulletin.

### `QLecternAnchorRefine`

A fanqie bulletin updates anchors without rebuilding the reflex rows.

### `QLecternHingeRefine`

The hinge returns to its previous state when the gate refused the write.
A successful verdict leaves the clicked state alone, awaiting the subscribed fold refresh.

## `QLecternSound`

Box openings come from the supplied `CFold`, keeping stored state separate from the sound blocks.
Each block arrives with its font, avoiding an additional font read from Conduct.
The section turns each block into the box's own presentation rows.
Glyphs, transcriptions, reflexes, accents and playback live on their own sections.
The fanqie representative notice goes straight to the sound gate, since it already carries raw values.
The script box's failure shows through the ledger and envoy under `Display.ScriptFailed`.
Both box switch clicks are hooked here, keeping those gate calls outside the boxes.

### `QLecternBoxRefine`

Both box openings are refreshed together from the supplied fold state.

### `QLecternFanqieRefine`

The lectern subscribes the reflex anchor refresh before this fanqie redraw.

### `QLecternParadigmRefine`

The font and sheet come from the same ready paradigm block, keeping layout choices outside this section.
The paradigm, script and fanqie Refines are subscribed in that order.
Open and close share those callbacks, so the supplied blocks determine what remains visible.

### `QLecternSpellingObserve`

The verdict goes to `QLook.QLookCheckedRefine`, which restores the switch on a refusal.
The subscribed fold notice refreshes the boxes from stored state.
The script switch follows the same path.

## `QLecternTranscription`

Open and close share one row replacement path, keeping the transcription list aligned with the sound area's answer.

## Stamp

Commit: `c6e60141a172`
- `QLecternAccent.cs` `c85ebff88f1d4c47`
- `QLecternGlyph.cs` `6b0375222c9ae963`
- `QLecternPlayback.cs` `6ec02ff5268f6011`
- `QLecternReflex.cs` `51a11764b210178b`
- `QLecternSound.cs` `bc1e971efd7bdf17`
- `QLecternTranscription.cs` `1270a1871234006d`
