# PBylineTemplate.xaml

## `Theme.Byline.Row`

One offered author: the name alone.
The row is a plain surface rather than a button, so the list beneath it keeps its own selection.

The row answers the pointer going down rather than coming up.
A popup takes the window's activation the moment it is pressed, which ends the field's focus.
Answering on the press credits the Author before that happens.
So the field losing focus finds its row already replaced, and puts nothing back.

The pieces of the name are drawn as runs of one line rather than as three blocks.
A run carries no spacing of its own, so the name reads as one word broken only by weight.
