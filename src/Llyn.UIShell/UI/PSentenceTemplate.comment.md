# PSentenceTemplate.xaml

## `Theme.Sentence.Row`

The Example row as the card shows it, on one line.
That is the two frame fields, then the sentence field.
The id the row will be stored under stands at its right.
It is also the Source button and the buttons that open and drop rows.
Each field's hint reads the unreadable mark when the row holds something it cannot read back.
So an empty-looking field says which kind of empty it is.

The two frame fields sit in the first two columns.
Each is placed by the column the row was told to take.
So a language writing the role first draws the role first, and nothing about the row itself changes.
Both are typed into and both offer what the workspace has already saved for the language.
Neither offers anything on a workspace that has saved none, because nothing ships a marker or a role.

## `Theme.Reference.Row`, `Theme.Reference.Item`

One Source in the list a row cites from, and the row it sits in.
Both are offered to the Situation rows as well.
That is why they live in this dictionary rather than beside a single template.

## `Theme.Sentence.Citation`

What the Source button reads.
It reads the cited Source's name.
It reads the invitation to assign one when the row cites none.
It reads the unreadable mark when the citation cannot be read back.

## `Theme.Sentence.Notice`

The line shown in the Source list while the workspace holds no Sources at all.

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
