# PSlateTemplate.xaml

## `Theme.Slate.Row`

One offered tag: its text alone, because a tag carries no count of its own.
The row is a plain surface rather than a button, so the list beneath it keeps its own selection.
The Deportment class of the same name loads this markup, and the editor's fill writes the runs.

## `PSlateRow`

The row answers the pointer going down rather than coming up.
A popup takes the window's activation the moment it is pressed, which ends the caret's focus.
Answering on the press writes the tag and empties the caret before that happens.
So the text the user was typing is not committed a second time as a new tag.

## `PSlateLead`

The pieces of the text are drawn as runs of one line rather than as three blocks.
A run carries no spacing of its own, so the text reads as one word broken only by weight.
