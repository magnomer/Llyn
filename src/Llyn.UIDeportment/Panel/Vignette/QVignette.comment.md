# QVignette.cs
Hash: `67b7f75f6b008fe0`

## `internal sealed class QVignette`

The reading side of the Repertoire panel, the page one chosen Situation is shown on.
Its title stands at the head, the kind and the reference count under it, then the description and the media.
The Corpus panel has a reading side of its own in `QExcerpt`.
It subscribes what it paints itself, so the owner `QRepertoire` only builds, introduces and lights it.

## `internal QVignette(UserControl scope)`

Takes the Repertoire page, and finds every `PVignette` part in it by contract ID.

## `internal void QVignetteIntroduce(CRepertoire repertoire, CAtelier atelier)`

`QRepertoireIntroduce` calls it once the repertoire Conduct exists.
It takes the atelier only for the description's Markdown, which resolves its links through it.
It subscribes the repertoire's Situation notice, and the atlas's rows event and clear.
It attaches the shared media fills to the two read-only media lists.

## `internal void QVignetteVisibleRefine()`

Shows the page while the diptych shows its parent, and the body or the unselected notice as the repertoire says.
The owner's mode refine calls it with the rest of the panel's mode.

## `private void QVignetteTallyRefine()`

Writes the chosen Situation's tally on the reading side, at each atlas row notice and each Situation paint.
The Conduct words the sentence through the engine, so a reference added elsewhere shows at the next row notice.

## `private void QVignetteRefine(CSituation situation)`

Paints the read page of a Situation the repertoire announces, ready from Conduct.
Each media row becomes the driver row the editor builds, so no shape crosses into the surface.
The tally chip is repainted with it, counting the Situation the atlas has chosen.

## `private void QVignetteClearRefine()`

Empties the media lists when the atlas panel clears, so no video plays on after its Situation is gone.

## `private void QVignetteTitleRefine(CStateWording title)`

Draws the title in the headword's place, looking up the key Conduct chose when one stands.
A never-written title arrives muted, and paints in the muted colour, because the head of the page cannot stand empty.

## `private void QVignetteKindRefine(CStateWording kind)`

Draws the kind in its chip, or hides the chip while no kind was ever written.

## `private void QVignetteDescriptionRefine(CSituation situation)`

Draws the description's ready Markdown blocks into its card, or hides the section while none was ever written.
A description worded by a key shows the key's text as one plain paragraph instead.
The entry note is drawn the same way, and the same painter keeps the two alike.

## `private void QVignetteMediaRefine(IReadOnlyList<QImageItem>? pictures, IReadOnlyList<QVideoItem>? videos)`

Hands the read rows to the two media lists, which draw them through the card's own line templates.
Each list hides itself through the look sheet while it holds nothing, so the page carries no empty media block.
Null empties both, which also stops any video still playing.
