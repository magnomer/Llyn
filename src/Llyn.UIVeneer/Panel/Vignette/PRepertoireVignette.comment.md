# PRepertoireVignette.cs

## `public partial class PRepertoire`

The reading side of the Repertoire panel, the page one chosen Situation is shown on.
Its title stands at the head, the kind and the reference count under it, then the description and the media.
The Corpus panel draws its excerpt the same way in `PCorpusExcerpt.cs`.

## `private string? PVignetteTextRead(LStateValue value)`

What one stored field reads as, or null for one never written.
A written value reads itself, and an unknown one reads the mark.
Whether it is unknown is the converter's to say, as for a card field in the entry display.

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

Hands the Situation to the two media lists, which draw its pictures and videos through the card's own converters.
Each list hides itself while its side of the Situation is empty, so the page carries no empty media block.
A null Situation empties both, which also stops any video still playing.

## `private string PRepertoireTallyRead(long? id)`

How many places reference one Situation, read from the count the catalog fill already holds, worded as a sentence.
None, one and many are three texts, because a number alone beside a title says nothing about what it counts.
It is drawn on both sides of the panel, so the reader and the writer see the same figure.

# PRepertoireVignette.xaml

## `ResourceDictionary`

The reading side of the Repertoire panel: the atlas rows, the vignette page and the occurrence rows under it.
The panel merges it, as the Corpus panel merges its excerpt shapes.
Nothing here answers an event, so the dictionary is loose.
The occurrence row stands here because it is read, never edited.

## `<Style x:Key="Theme.Vignette.Chip" TargetType="Border">`

The kind chip, shaped as a speech chip and coloured as the cards colour a Situation.
The same style dresses the chip on both sides.
The kind reads the same whether it is read or written.

## `<Style x:Key="Theme.Vignette.Tally" TargetType="Border">`

The reference count, a chip of the same shape in the raised surface colour.
That colour makes it read as a figure and not a kind.
It reads as a sentence, not a bare number.
A bare number beside a title says nothing about what it counts.
