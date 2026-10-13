# Vignette

These loose dictionaries hold the shapes of the Repertoire panel's Situation.
The reading side and the editing side are split.
So the reading side never depends on the editing side being loaded.
Nothing here answers an event, so the dictionaries carry no code.
The rows use `Theme.Catalog.Row` directly, and the panel's row fills mark the chosen one.

## `PRepertoireVignette`

The reading side holds the atlas rows, the vignette page and the occurrence rows under it.
The panel merges it, as the Corpus panel merges its excerpt shapes.

## `Theme.Atlas.Kind`

The kind at the end of an atlas row, dressed as catalog meta.
The look sheet collapses it while the row has no kind, so the title takes the width.

## `Theme.Vignette.Heading`

A heading at the height, margin and size of the entry display's section headings.
So a Situation page heads its parts as an entry page does.

## `Theme.Vignette.Picture`

The picture and video lists of the reading side, indented to the description's edge.
The look sheet collapses either list while it holds nothing, so no empty media block is drawn.

## `Theme.Vignette.Chip`

The kind chip, shaped as a speech chip and coloured as the cards colour a Situation.
The same style dresses the chip on both sides, so the kind reads the same read or written.

## `Theme.Vignette.Tally`

The reference count, a chip of the same shape in the raised surface colour.
That colour makes it read as a figure and not a kind.
It reads as a sentence, because a bare number beside a title says nothing about what it counts.

## Scenario dictionary

The editing side holds the bare fields a Situation is written into.
The measure style is based on the vignette's kind, so the dictionary merges the vignette shapes itself.
A loose dictionary resolves a static reference only in itself, in what it merges, and in the application resources.
The panel merges the vignette shapes too.

## `Theme.Scenario.Measure`

The unseen twin that measures the kind.
Its chip closes on the written kind, or on the placeholder while it is empty.
The panel writes that text into `PScenarioMeasure` whenever the kind or its placeholder changes.
Without it the chip in the editor stood at a fixed width the reading side never showed.

## `Theme.Scenario.Hint`

The unseen twin that measures the empty title, copied from the entry editor's `Editor.Field.Hint`.
It reads the field's own placeholder rather than one fixed word, because the placeholder changes with the state.
The panel copies the placeholder in and shows the twin only while the title is empty.

## `Theme.Scenario.Ghost`

The unseen twin that measures the written title.
The panel copies the title into it on every change.
The head row closes on the text rather than on a fixed box.

## `Theme.Scenario.Kind`

The bare field the kind is written into, inside the same chip the reading side draws.
Bare, so the written kind sits exactly where the read kind sits.

## `Theme.Scenario.Description`

The bare field the description is written into, where the reading side renders it.
Its size matches the rendered paragraph, so a line of description sits at one height in both modes.

## `Theme.Scenario.Media`

The two row lists, indented to the description's edge and spaced as a card spaces its media.

## Stamp

Commit: `c2f335c331fe`
- `PRepertoireScenario.xaml` `6ed95f4d4dd74d0b`
- `PRepertoireVignette.xaml` `06aff15316124fc1`
