# PSentenceTemplate.xaml

## `Theme.Sentence.Row`

The Example row as the card shows it, on one line.
That is the two frame fields, then the sentence field with the id the row will be stored under at its right.
It is also the Source button and the buttons that open and drop rows.
Each field's hint reads the unreadable mark when the row holds something it cannot read back.
So an empty-looking field says which kind of empty it is.

The two frame fields sit in the first two columns and each is placed by the column the row was told to take.
So a language writing the role first draws the role first, and nothing about the row itself changes.
Both are typed into and both offer what the workspace has already saved for the language.
Neither offers anything on a workspace that has saved none, because nothing ships a marker or a role.

## `Theme.Reference.Row`, `Theme.Reference.Item`

One Source in the list a row cites from, and the row it sits in.
Both are offered to the Situation rows as well.
That is why they live in this dictionary rather than beside a single template.

## `Theme.Sentence.Identity`

The box the row's id stands in, hidden until the row has an id.

## `Theme.Sentence.Citation`

What the Source button reads.
It reads the cited Source's name.
It reads the invitation to assign one when the row cites none.
It reads the unreadable mark when the citation cannot be read back.

## `Theme.Sentence.Notice`

The line shown in the Source list while the workspace holds no Sources at all.

## `Theme.Sentence.Frame`

The frame is written where it is read: `(+marker role)` in front of the sentence, in the sentence's own face.
Fields of its own, sitting above or beside the line, would say the frame is a property of the row.
It is not; it is the opening of the sentence, and the reading view has always drawn it as one.

A row carrying no frame shows none, so its sentence begins where the reading view begins it.
The card offers one instead, from the strip kept clear beside the row.

## `Theme.Sentence.Control`

Everything a writer needs and a reader does not is laid out whether or not it is being shown, and faded rather than hidden.
Hiding it would give the row one height under the pointer and another away from it.
A card that moves while it is being pointed at cannot be read as the card it will become.
