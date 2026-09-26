# PCorpusExcerpt.xaml

## `ResourceDictionary`

The reading side of the Corpus panel: the anthology meta and the excerpt page the rows open.
The panel keeps its layout and merges these shapes from here, as it merges the editor popups.
Nothing here answers an event, so the dictionary is loose and needs no class of its own.
The anthology and quotation rows take `Theme.Catalog.Row` directly, and the panel marks the chosen one.
`Theme.Excerpt.Flag` hides an empty flag through its `Empty` row in the look sheet.
`Theme.Display.Value` is the face the read Gloss takes, and the theme reaches it by a dynamic reference.

## `<Style x:Key="Theme.Excerpt.Chip" TargetType="Border">`

The language chip, shaped as a speech chip and coloured in the accent, with the flag before the name.
The same shape dresses the language toggle on the editing side.
So the language reads the same whether it is read or chosen.

## `<Style x:Key="Theme.Excerpt.Tally" TargetType="Border">`

The usage count, a chip of the same shape in the raised surface colour.
So it reads as a figure and not a language.
It reads as a sentence, not a bare number.
A bare number beside a sentence says nothing about what it counts.
