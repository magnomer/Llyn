# Reflex

This folder holds the reflex block's looks in reading and writing mode.

## `PThemeReflex.xaml`

This sheet is the look of a reflex row, shared by the editor and the reading view.
It merges the pronunciation and accent styles, so a row sits in the reading stack like a pronunciation row.
A row has no border and no box.
The text columns are shared across every row, so the rows read as one table under the headword.
So the anchored reading of every row starts at one x, whatever the notes before it measure.
A language whose readings have no kind leaves that cell blank, so its readings still line up.
The language prints once on the first row of its run, and hovering it shows the region.
A folded row hides at no height while the fold toggle under it is closed.
It is still measured, so the shared columns hold the same widths folded or open.
The editor row and the view row lay out alike, so a reading stands at one place in both modes.
The editor fields, hover controls and row template live in `PThemeReflexEditor.xaml`.

### `Theme.Reflex.Surface`

The surface is a little lower than a pronunciation row so a language's readings read as one block.
Hidden under the fold it is unseen and flat, yet still measured for the shared columns.

### `Theme.Reflex.Label` and `Theme.Reflex.Tag`

Beneath the lead, a `QLook` row hides the label rather than collapsing it.
The same holds for a blank kind tag.
So the shared column keeps its width.

### `Theme.Reflex.Text`

It carries no side margin, so it stands where the editor field stands.
The row fill draws the reading in common use in the accent colour.
The weight is left alone because the faces the scripts fall back to carry no heavier one.

### `Theme.Reflex.Edge`

This is the slash around the reading of a phonemic language.
Both modes draw it in the reading colour, as part of the reading.

### `Theme.Reflex.Fold`

The toggle stands centred under the table.
So both panels hold the table in a stack no wider than its rows.
It reads `More readings` with a chevron, which turns over and reads `Fewer readings` while open.

### `Theme.Reflex.Anchor`

Both modes draw the anchor, so the placement stands at one place in the editor and the reading view.

### `Theme.Reflex.Loading`

This line stands where the rows will be drawn while a fill runs.
Both modes hide it when the fill answers.

### `Theme.Reflex.Display`

The reading stands between the same slashes the editor row draws, in the same stack with the same inset.
`QReflexItem.QReflexItemRefine` fills every named part.

## `PThemeReflexEditor.xaml`

This sheet is the editor side of a reflex row.
It merges the shared row styles, so a field stands where the reading view prints its text.
A cell grows its box only while edited, and the row fill names its box style on the cell.
So no measure style is declared here.

### `Theme.Reflex.Anchoring`

This is the anchor text made clickable, opening the dropdown through the command.
The row fill collapses it where anchoring is barred.
`QLook` rows show the muted prompt while it is empty.

### `Theme.Reflex.Field` and `Theme.Reflex.Lead`

The reading cell shows the muted placeholder while blank, so an empty cell still measures.
The row fill names the accent lead field on the cell for the reading in common use.

### `Theme.Reflex.Name` and `Theme.Reflex.Aside`

The name field takes the label size and colour, so it reads as the label does.
The aside field takes the size and colour of the minor text it covers.
They overhang the hidden text under them by the two pixels a field pads its text with.
So the column is as wide as the view column.

### `Theme.Reflex.Rebuild`

This is the shared regenerate button, placed at the top right corner of the reflex block.
The editor sets the cue from the engine fill state, so the mark turns as long as the fetch runs.

### `Theme.Reflex.Control`

The hover star, plus and minus are stacked as the accent controls are.
The handles are built into the slot on first hover or focus.

### `Theme.Reflex.Row`

Every field is bare and stands where the reading view prints its text.
The side cells are the same grids in both templates, so an empty cell measures alike in both modes.
The language cell right margin of 24 is the margin `Theme.Pronunciation.Label` gives the view label.
The region is no field, only the hover the view has on the language.
The reading field of a phonemic language stands between two slashes it does not hold.
`QReflexItem.QReflexItemRefine` fills every part and picks the reading field style.

## Stamp

Commit: `c2f335c331fe`
- `PThemeReflex.xaml` `aecc563c8fe66a2c`
- `PThemeReflexEditor.xaml` `364ef6fac882732e`
