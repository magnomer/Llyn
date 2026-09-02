# PSentenceTemplate.xaml

## `Theme.Sentence.Row`

The Example row as the card shows it.
That is the sentence field, with the id the row will be stored under at its right.
It is also the number the row takes while the card carries more than one.
It is also the Source button and the buttons that open and drop rows.
The field's hint reads the unreadable mark when the row holds a sentence it cannot read back.
So an empty-looking field says which kind of empty it is.

## `Theme.Reference.Row`, `Theme.Reference.Item`

One Source in the list a row cites from, and the row it sits in.
Both are offered to the Situation rows as well.
That is why they live in this dictionary rather than beside a single template.

## `Theme.Sentence.Identity`

The box the row's id stands in, hidden until the row has an id.

## `Theme.Sentence.Order`

The number stands ahead of the field, in the label column's gutter.
So an Example field starts exactly where every other field on the card does.
That holds whether or not the rows are numbered.

## `Theme.Sentence.Citation`

What the Source button reads.
It reads the cited Source's name.
It reads the invitation to assign one when the row cites none.
It reads the unreadable mark when the citation cannot be read back.

## `Theme.Sentence.Notice`

The line shown in the Source list while the workspace holds no Sources at all.
