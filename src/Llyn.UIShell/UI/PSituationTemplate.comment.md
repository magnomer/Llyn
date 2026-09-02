# PSituationTemplate.xaml

## `Theme.Situation.Row`

The Situation row as the card shows it, built the way an Example row is: the wording field, the id the row will be stored under standing at the field's right, the number the row takes while the card carries more than one, the Source button, and the buttons that open and drop rows. The field's hint reads the unreadable mark when the row holds a wording it cannot read back, so an empty-looking field says which kind of empty it is.

The Source list itself is the one the Example rows offer — this dictionary names those styles rather than declaring its own, so a Source written on either kind of row is on offer to both.

## `Theme.Situation.Identity`

The box the row's id stands in, hidden until the row has an id.

## `Theme.Situation.Order`

The number ahead of the field, standing in the label column's gutter so a Situation field starts exactly where every other field on the card does whether or not the rows are numbered.

## `Theme.Situation.Citation`

What the Source button reads: the cited Source's name, the invitation to assign one when the row cites none, and the unreadable mark when the citation cannot be read back.

## `Theme.Situation.Notice`

The line shown in the Source list while the workspace holds no Sources at all.
