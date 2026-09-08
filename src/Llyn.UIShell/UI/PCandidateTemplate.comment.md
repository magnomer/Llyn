# PCandidateTemplate.xaml

## `Theme.Candidate.Row`

One offered Situation: its wording, with its usage count against the right edge.
The row is a plain surface rather than a button, so the list beneath it keeps its own selection.

The row answers the pointer going down rather than coming up.
A popup takes the window's activation the moment it is pressed, which ends the caret's focus.
Answering on the press attaches the Situation and empties the caret before that happens.
So the wording the user was typing is not committed a second time as a new Situation.

The pieces of the wording are drawn as runs of one line rather than as three blocks.
A run carries no spacing of its own, so the wording reads as one word broken only by weight.
