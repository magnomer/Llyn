# PContextTemplate.xaml

## `Theme.Context.Row`

The Situation row as the card shows it, built the way an Example row is.
That is the wording field, with the id the row will be stored under at its right.
It is also the number the row takes while the card carries more than one.
It is also the Source button and the buttons that open and drop rows.
The field's hint reads the unreadable mark when the row holds a wording it cannot read back.
So an empty-looking field says which kind of empty it is.

The Source list itself is the one the Example rows offer.
This dictionary names those styles rather than declaring its own.
So a Source written on either kind of row is on offer to both.

## `Theme.Context.Identity`

The box the row's id stands in, hidden until the row has an id.

## `Theme.Context.Order`

The number stands ahead of the field, in the label column's gutter.
So a Situation field starts exactly where every other field on the card does.
That holds whether or not the rows are numbered.

## `Theme.Context.Citation`

What the Source button reads.
It reads the cited Source's name.
It reads the invitation to assign one when the row cites none.
It reads the unreadable mark when the citation cannot be read back.

## `Theme.Context.Notice`

The line shown in the Source list while the workspace holds no Sources at all.
