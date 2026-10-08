# LSituationCitation.cs
Hash: `4c94190b8ce67d40`

## `public sealed class LSituationCitation`

The start, commit and check of a held Situation, one kind of the citation protocol.
`LCitationClerk` builds and holds it beside the three other kinds.

## `public LSituationCitation(LRig rig, LIdentity identity, LClaimClerk claims, LSituationClerk situations, LRevisionClerk revisions)`

Reads the vault out of `rig` for the session a commit opens.
The issuer names a new Situation and its media rows.
The claim clerk holds and finishes the draft.
The Situation clerk writes the row, and the revision clerk records the revision.

## `public LDraft LSituationCitationStart(string origin, long? situationId)`

Mints a draft id, writes the first file, and returns the held Situation.
With no Situation the content is blank under an id minted for it, which is a context being written first.
With a Situation the content is that Situation read back, which is an edit.
A Situation that is gone is refused before a file is written, so no draft can point at nothing.

## `public LSituation LSituationCitationCommit(long id)`

Turns a held Situation into a stored one and returns it.
A draft naming no Situation is a create, one naming a Situation is a rewrite.
A Situation id naming a record since deleted is a create as well, because there is nothing left to rewrite.
The write and the revision recording it share one session, so the history never misses a stored context.
The draft file is rewritten with the stored id and the stored content the moment that write returns.
The held file is deleted last, so a failure anywhere above leaves the work recoverable.

## `public bool LSituationCitationCheck(LDraft draft, LSituation held)`

Whether a held Situation differs from the one it was started from.
A draft naming no Situation is measured against an empty one.
A draft whose Situation has since gone is measured against that same empty one.

## `private LSituation LSituationCitationNormalize(LSituation content)`

The same Situation with an id minted when it carries none.
Its image and video rows are named the same way, and a row with no location is dropped.
The Situation that came in is returned itself when it was already named and its rows were too.
