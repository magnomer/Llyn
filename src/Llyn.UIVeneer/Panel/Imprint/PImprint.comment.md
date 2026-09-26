# PImprint.xaml

## `<UserControl>`

The edit area of the Source panel, the selected Source as the panel writes it.
It is laid out as the reading side lays the Source out.
It is a control of its own because the markup and the writing side outgrew one file.
Every slot the reading side draws is drawn here in the same place.
A bare field stands where the text stood.
The title is written in the head of the page and the kind is chosen inside its chip.
Each other field is written under its heading.
No field carries a control that records it unknown, as no other edit area offers one.
The kind is chosen from a list that carries its own unknown entry.
The area carries no buttons of its own.
Save is the rail's `PReferenceStore`, delete is `PReferenceBin`, and there is no discard.
That is how the repertoire panel arranges a Situation.
Leaving the area asks about the draft as it does there.
The mode toggle is not in here, because it sits in the panel's action row.
The markup carries no class, and the edit area control loads it and wires its named parts.

## `<ToggleButton x:Name="PImprintKind" ...>`

The kind chip, shaped as the reading side draws it, opening the list of kinds beneath it.
The list is built in code from the same order the old dropdown offered, one radio row per kind.
The control ties the chip to its list and sets the expand icon on `PImprintKindIcon`.

## `<Grid x:Name="PAuthor" Margin="2,0">`

The ordered credit rows, each a field naming the author it credits, with its handles at the end.
Each row adds under itself, as an entry's sentence lines do.
The blank row that appears takes the typed name.
So the credit rows sit where the reading side draws the names, and the sections below keep their places.
Nothing is offered until a draft is held, because the credits live on the draft.

## `<ItemsControl x:Name="PAuthorCredit" ...>`

The credit rows, drawn from a template the merged dictionary holds.
The control takes the clicks, keys, typing and focus of every row on the list.
The markup names no handler, and every row part is named for the control to fill.
Each attached event routes up from the field or button that raised it.

## `<Popup x:Name="PByline" ...>`

The dropdown of stored authors the typed name matches, placed under the field being typed into.
It is drawn with the editor's popup styles, so the two dropdowns read as one kind of thing.
The control sets the frame's least width to the field's width each time it opens.

## `<TextBlock x:Name="PImprintTally" ...>`

The citation count stays visible while editing, because one correction reaches every place citing the Source.
