# PCandidateItem.cs

## `internal sealed class PCandidateItem`

One stored Situation offered in the dropdown: the wording it is titled with and the id it is stored under.
The id is what the chip will carry, so choosing a row attaches the stored Situation rather than a copy of its words.

## `public string PCandidateItemLead`

The wording is held in three pieces: what stands before what was typed, what was typed, and what follows.
The row draws the middle piece in weight, so a reader sees why the row is offered rather than guessing.
The comparison is the culture's own, and it reports how long the match ran, because a match under a culture's rules is not always as long as the wording that found it.
A wording the typing does not appear in is held whole in the first piece, so a row is never lost to a search that cannot point at itself.

## `public string PCandidateItemCount`

How many cards already use the Situation, shown beside the wording.
A count of none is shown as nothing, because a Situation nobody uses needs no number to say so.
