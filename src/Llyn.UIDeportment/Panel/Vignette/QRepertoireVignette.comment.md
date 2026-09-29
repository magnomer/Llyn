# QRepertoireVignette.cs

## `internal sealed partial class QRepertoire`

The reading side of the Repertoire panel, the page one chosen Situation is shown on.
Its title stands at the head, the kind and the reference count under it, then the description and the media.
The Corpus panel draws its excerpt the same way in `PCorpusExcerpt.cs`.

## `private static string? QVignetteTextRead(CStateValue value)`

What one stored field reads as, or null for one never written.
A written value reads itself, and an unknown one reads the localized unknown mark.
The value itself says whether it is unknown, as a card field does in the entry display.

## `private void QVignetteShow(CSituationDraft situation)`

Paints the read page of a Situation the engine announces.
The tally chips are repainted with it, counting the Situation the atlas has chosen.

## `private void QVignetteClear()`

Empties the media lists when the atlas panel clears, so no video plays on after its Situation is gone.

## `private void QVignetteTitleShow(CStateValue value)`

Draws the title in the headword's place.
A never-written title reads the untitled text in the muted colour, because the head of the page cannot stand empty.

## `private void QVignetteKindShow(CStateValue value)`

Draws the kind in its chip, or hides the chip while no kind was ever written.
An entry with no speech draws no speech chip, and the kind follows that.

## `private void QVignetteDescriptionShow(CStateValue value)`

Renders the description from Markdown into its card, or hides the section while none was ever written.
The entry note is rendered the same way, and the same renderer keeps the two alike.

## `private void QVignetteMediaShow(IReadOnlyList<PImage>? pictures, IReadOnlyList<PVideo>? videos)`

Hands the read rows to the two media lists, which draw them through the card's own line templates.
Each list hides itself through the look sheet while it holds nothing, so the page carries no empty media block.
Null empties both, which also stops any video still playing.

## `private List<PImage> QVignetteImageRead(IReadOnlyList<CImageDraft> rows)`

The picture rows the page reads, built as the editor builds its own.
A row whose location was never written is left out, as the line template collapsed it before.
The surface is handed driver rows, so no shape crosses into it.

## `private List<PVideo> QVignetteVideoRead(IReadOnlyList<CVideoDraft> rows)`

The video rows the page reads, a never-written location left out the same way.
