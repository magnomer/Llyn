# Imprint

This folder holds the edit area of the Source panel and the rows its author credits use.
It is a control of its own because the markup and the writing side outgrew one file.
Its C# shell is bare, and the driver `QImprint` wires the named parts.
`QAuthor`, which `QImprint` builds over `PAuthor`, wires the credit rows and the byline.

## `PImprint`

The edit area is the selected Source as the panel writes it.
It is laid out as the reading side lays the Source out.
Every slot the reading side draws is drawn here in the same place.
A bare field stands where the text stood.
The title is written in the head of the page and the kind is chosen inside its chip.
Each other field is written under its heading.
No field carries a control that records it unknown, as no other edit area offers one.
The kind is chosen from a list that carries its own unknown entry.
The area carries no buttons of its own.
Save is the rail's `PReferenceStore`, delete is `PReferenceBin`, and there is no discard.
Leaving the area asks about the draft as the repertoire panel does for a Situation.
The mode toggle sits in the panel's action row, not here.

### `PImprintKind`

The kind chip is shaped as the reading side draws it, and it opens the list of kinds beneath it.
The list is built in code from the same order the old dropdown offered, one radio row per kind.
The control ties the chip to its list and sets the expand icon on `PImprintKindIcon`.

### `PAuthor`

It holds the ordered credit rows, each a field naming the author it credits, with its handles at the end.
Each row adds under itself, as an entry's sentence lines do.
The blank row that appears takes the typed name.
So the credit rows sit where the reading side draws the names, and the sections below keep their places.
Nothing is offered until a draft is held, because the credits live on the draft.

### `PAuthorCredit`

The rows are drawn from a template the merged dictionary holds.
The control takes the clicks, keys, typing and focus of every row on the list.
The markup names no handler, and every row part is named for the control to fill.
Each attached event routes up from the field or button that raised it.

### `PByline`

This dropdown lists the stored authors the typed name matches, placed under the field being typed into.
It uses the editor's popup styles, so the two dropdowns read as one kind of thing.
The control sets the frame's least width to the field's width each time it opens.

### `PImprintTally`

The citation count stays visible while editing, because one correction reaches every place citing the Source.

## `PBylineTemplate`

`Theme.Byline.Row` offers one author: the name alone.
The row is a plain surface rather than a button, so the list beneath it keeps its own selection.
It answers the pointer going down rather than coming up.
A popup takes the window's activation the moment it is pressed, which ends the field's focus.
Answering on the press credits the Author before that happens.
So the field losing focus finds its row already replaced, and puts nothing back.
The pieces of the name are runs of one line rather than three blocks.
A run carries no spacing of its own, so the name reads as one word broken only by weight.
The row names no handler and binds nothing.
`QAuthor` fills the three runs and subscribes the press on `PBylineRow`.

## `PImprintCredit`

A credit row is one field naming the author it credits, with the add, drop and order controls beside it.
The handles are the entry's sentence handles, so the two lists are worked the same way.
The field is the whole of the row's editing, so no menu stands apart from the list.

### `PAuthorEarlier`

A dictionary carries no code, so the four row buttons name no handlers.
The edit area maps the name of whichever button raised the click to its action.

### `Imprint.Credit.Field`

The field is bare and sits where the reading side draws the name, at the same size.
So the edit side reads as the view side does, with only the caret to tell them apart.

### `Imprint.Credit.Control`

The handles stay hidden, and the edit area shows them while the pointer or caret is in the list.
So the row at rest is the name alone, as the reading side shows it.

### `Imprint.Credit.Handle`

A handle that cannot act is faded rather than hidden, so the row keeps its shape.
The fade is a disabled state row of the look sheet, not a trigger here.
The marks take the handle's own foreground, so the pointer over one lights it as the entry's handles light.

### `PAuthorName`

The field shows the credited name, and typing over it changes nothing until it is entered.
The edit area fills the field from its row, and refills it only when the row's name changes.

## Stamp

Commit: `c2f335c331fe`
- `PBylineTemplate.xaml` `e4597d61b0883a5b`
- `PImprint.xaml` `17befeac0f96d4d4`
- `PImprint.xaml.cs` `e34dbf56e78d6612`
- `PImprintCredit.xaml` `b2e9c56fbf1a24ec`
