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
