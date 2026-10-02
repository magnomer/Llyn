# QRepertoireVignette.cs

## `internal sealed partial class QRepertoire`

The reading side of the Repertoire panel, the page one chosen Situation is shown on.
Its title stands at the head, the kind and the reference count under it, then the description and the media.
The Corpus panel draws its excerpt the same way in `PCorpusExcerpt.cs`.

## `private void QVignetteRefine(CSituation situation)`

Paints the read page of a Situation the repertoire announces, ready from Conduct.
Each media row becomes the driver row the editor builds, so no shape crosses into the surface.
The tally chips are repainted with it, counting the Situation the atlas has chosen.

## `private void QVignetteClearRefine()`

Empties the media lists when the atlas panel clears, so no video plays on after its Situation is gone.

## `private void QVignetteTitleRefine(CStateWording title)`

Draws the title in the headword's place, looking up the key Conduct chose when one stands.
A never-written title arrives muted, and paints in the muted colour, because the head of the page cannot stand empty.

## `private void QVignetteKindRefine(CStateWording kind)`

Draws the kind in its chip, or hides the chip while no kind was ever written.
An entry with no speech draws no speech chip, and the kind follows that.

## `private void QVignetteDescriptionRefine(CSituation situation)`

Draws the description's ready Markdown blocks into its card, or hides the section while none was ever written.
A description worded by a key shows the key's text as one plain paragraph instead.
The entry note is drawn the same way, and the same painter keeps the two alike.

## `private void QVignetteMediaRefine(IReadOnlyList<QImageItem>? pictures, IReadOnlyList<QVideoItem>? videos)`

Hands the read rows to the two media lists, which draw them through the card's own line templates.
Each list hides itself through the look sheet while it holds nothing, so the page carries no empty media block.
Null empties both, which also stops any video still playing.
