# PSentenceTemplate.xaml

## `Theme.Sentence.Row`

The Example row as the card shows it, on one line.
That is the two frame fields, then the sentence field.
The id the row will be stored under stands at its right.
It is also the citation field at the sentence's right end, and the buttons that open and drop rows.
Each field's hint reads the unknown mark when the row holds something it cannot read back.
So an empty-looking field says which kind of empty it is.

The two frame fields sit in the first two columns.
Each is placed by the column the row was told to take.
So a language writing the role first draws the role first, and nothing about the row itself changes.
Both are typed into and both offer what the workspace has already saved for the language.
Neither offers anything on a workspace that has saved none, because nothing ships a marker or a role.

## `<Grid.CommandBindings>`

The four Mention commands are bound on the row.
So both the field's menu and the chip line reach the same handlers.
The menu names the field as its target and a chip names itself as parameter.
The handlers tell the two apart by what arrived.

## `<TextBox x:Name="PSentenceLine" ContextMenu="{DynamicResource Theme.Mention.Menu}">`

The sentence field carries the shared linking menu.
Its selection is what the menu's items read when they decide whether they apply.

## `<ItemsControl x:Name="PSentenceMentionLine">`

The chip line under the sentence, one chip per Mention, collapsed while there is none.
It stands in the sentence column so it reads as part of the sentence and not of the row's controls.

## `Theme.Sentence.Citation`

The field the cited Source is typed into, showing its `Author (Year)` byline.
It is bare like the sentence field, right-aligned and muted, so it reads as the sentence's tail.
Its font, size and baseline drop are bound to the sentence field and its frame, not to the resources.
So its box stands level with the sentence box whatever font the language pack gave the sentence.
It fades out while empty and the row is neither hovered nor focused.
So an uncited row shows no blank box.
The hint reads the invitation to assign a Source.

## `Theme.Sentence.Opening`

The switch that opens a frame stands in the place the frame will be written, right after the dot.
Standing in the gutter, it asked to be read as a handle on the row.
It did not read as the opening of a sentence.
It is drawn as the plus the card's other openings are drawn as.
What it means is where it is.
It gives way as soon as the frame carries a marker or a role.
A written frame is closed by clearing it.
An opened frame holding nothing keeps it and reads as a minus while open.
Closing the frame is all it can do then.

## `Theme.Sentence.Frame`

The frame is written where it is read: `(+marker role)` in front of the sentence, in the sentence's own face.
Fields of its own, sitting above or beside the line, would say the frame is a property of the row.
It is not.
It is the opening of the sentence, and the reading view has always drawn it as one.

The frame stands at the top of the row rather than filling it.
A row is as tall as the sentence it carries.
A field filling it would be framed far below the line it opens.

A row carrying no frame shows none, so its sentence begins where the reading view begins it.
The card offers one instead, from the strip kept clear beside the row.

## `<Canvas>`

The two frame fields are laid out where they cannot widen the line they open.
A canvas asks for no room, so the invisible copies beside it alone say how wide a field is.
Each field is then given the width those copies settled on.
Left to measure itself, a written field asks for a little more room than its text needs.
The sentence would then sit further along in the writing view than in the reading view.

## `<StackPanel Grid.Column="3">`

What a row cites stands in a column of its own, clear of the field it is read beside.
The id a row is kept under is not drawn at all.
It is the store's word for the row and not the writer's.
Laid over the written line, it covered the end of a sentence long enough to reach it.
A writer cannot revise what a mark is sitting on.

## `Theme.Sentence.Control`

Everything a writer needs and a reader does not is laid out whether shown or not.
It is faded rather than hidden.
Hiding it would give the row one height under the pointer and another away from it.
A card that moves while it is being pointed at cannot be read as the card it will become.
