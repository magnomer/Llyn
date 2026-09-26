# PSentenceTemplate.xaml

## `Theme.Sentence.Row`

The Example row as the card shows it, on one line.
That is the two frame fields, then the sentence field.
It is also the citation field at the sentence's right end, and the buttons that open and drop rows.
Each field's hint reads the unknown mark when the row holds something it cannot read back.
So an empty-looking field says which kind of empty it is.
The Deportment class of the same name loads this markup, and the editor's fill writes every part.
What is typed leaves through the editor's text handler, so the row holds no copy of it.

The two frame fields sit in the first two columns.
Each is placed by the column the row was told to take.
So a language writing the role first draws the role first, and nothing about the row itself changes.
Both are typed into and both offer what the workspace has already saved for the language.
Neither offers anything on a workspace that has saved none, because nothing ships a marker or a role.

## `PSentenceReach`

The four Mention commands and the Gloss removal are bound on the row by the fill, five bindings in all.
So both the field's menu and the chip line reach the same handlers.
The menu names the field as its target and a chip names itself as parameter.
The handlers tell the two apart by what arrived.

## `PSentenceText`

The sentence field carries the shared linking menu.
Its selection is what the menu's items read when they decide whether they apply.
It is named after the row value it writes, so the text handler knows which value changed.

## `PSentenceMentionLine`

The chip line under the sentence, one chip per Mention, collapsed while there is none.
It stands in the sentence column so it reads as part of the sentence and not of the row's controls.

## `PSentenceGlossLine`

The Gloss rows under the sentence.
The fill hands it the row's Glosses and attaches the Gloss row fill.

## `Theme.Sentence.Citation`

The field the cited Source is typed into, showing its `Author (Year)` byline.
The byline is looked up when the field is filled, from the row's anchor and the shared Source list.
A typed line is the field's alone until the editor commits or discards it.
It is bare like the sentence field and muted, so it reads as the sentence's tail.
The fill binds its font, size and baseline drop to the sentence field and its frame.
So its box stands level with the sentence box whatever font the language pack gave the sentence.
It fades out while empty and the row is neither hovered nor focused.
So an uncited row shows no blank box.
The hint reads the invitation to assign a Source.

## `Theme.Sentence.Opening`

The switch that opens a frame stands in the place the frame will be written, right after the dot.
Standing in the gutter, it asked to be read as a handle on the row.
It is drawn as the plus the card's other openings are drawn as.
It gives way as soon as the frame carries a marker or a role.
A written frame is closed by clearing it.
An opened frame holding nothing keeps it and reads as a minus while open.
The fill writes the sign and hides the switch once the frame is written.

## `PSentenceFrame`

The frame is written where it is read: `(+marker role)` in front of the sentence, in the sentence's own face.
Fields of its own, sitting above or beside the line, would say the frame is a property of the row.
It is the opening of the sentence, and the reading view has always drawn it as one.
The frame stands at the top of the row rather than filling it.
A row carrying no frame shows none, so its sentence begins where the reading view begins it.

## `Theme.Sentence.Hint`

The hint of an empty frame field, hidden until the fill finds the field empty.

## `<Canvas>`

The two frame fields are laid out where they cannot widen the line they open.
A canvas asks for no room, so the ghost copies beside it alone say how wide a field is.
The fill binds each field to the size those copies settled on.
Left to measure itself, a written field asks for a little more room than its text needs.

## `PSentenceBody`

The sentence column, dropped by the fill so its baseline meets the frame's.

## `PSentenceControl`

Everything a writer needs and a reader does not is laid out whether shown or not.
It is faded rather than hidden, and the editor shows it while the list is hovered or focused.
Hiding it would give the row one height under the pointer and another away from it.
A card that moves while it is being pointed at cannot be read as the card it will become.
