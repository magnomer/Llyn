# PRepertoireVignette.cs

## `public partial class PRepertoire`

The reading side of the Repertoire panel, the page one chosen Situation is shown on.
Its title stands at the head, the kind and the reference count under it, then the description and the media.
The Corpus panel draws its excerpt the same way in `PCorpusExcerpt.cs`.

## `private string? PVignetteTextRead(LStateValue value)`

What one stored field reads as, or null for one never written.
A written value reads itself, and an unknown one reads the localized unknown mark.
The value itself says whether it is unknown, as a card field does in the entry display.

## `private void PVignetteShow(LSituation situation)`

Paints the read page of a Situation the engine announces.
The tally counts the Situation the atlas has chosen.

## `private void PVignetteClear()`

Empties the media lists when the atlas panel clears, so no video plays on after its Situation is gone.

## `private void PVignetteTitleShow(LStateValue value)`

Draws the title in the headword's place.
A never-written title reads the untitled text in the muted colour, because the head of the page cannot stand empty.

## `private void PVignetteKindShow(LStateValue value)`

Draws the kind in its chip, or hides the chip while no kind was ever written.
An entry with no speech draws no speech chip, and the kind follows that.

## `private void PVignetteDescriptionShow(LStateValue value)`

Renders the description from Markdown into its card, or hides the section while none was ever written.
The entry note is rendered the same way, and the same renderer keeps the two alike.

## `private void PVignetteMediaShow(LSituation? situation)`

Hands the Situation's pictures and videos to the two media lists, which draw them through the card's own line templates.
Each list hides itself through the look sheet while it holds nothing, so the page carries no empty media block.
A null Situation empties both, which also stops any video still playing.

## `private string PRepertoireTallyRead(long? id)`

How many places reference one Situation, read from the count the catalog fill already holds, worded as a sentence.
None, one and many are three texts, because a number alone beside a title says nothing about what it counts.
It is drawn on both sides of the panel, so the reader and the writer see the same figure.
