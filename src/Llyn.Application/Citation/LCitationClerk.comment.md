# LCitationClerk.cs

## `public sealed class LCitationClerk`

The held-draft calls that belong to a citation rather than to an entry.
A citation is one of the independent facts an entry cites.
That ring is an Author, an Example, a Reference or a Situation.
Each is shared data owned by nothing, so its draft names the row and carries no entry.
Only starting, committing and checking differ by kind, and only those live here.
Every edit in between is a request, applied by `LDraftClerk` for all kinds alike.
The claim, the sweep and the discard are one for all kinds, and live in `LClaimClerk`.
The four kinds follow one protocol call for call, so a fifth kind cannot invent a fifth.
The rows themselves are written through the clerk of each kind.

## `public LCitationClerk(LRig rig, LIdentity identity, LClaimClerk claims, LAuthorClerk authors, LExampleClerk examples, LReferenceClerk references, LSituationClerk situations, LEntryClerk entries)`

Reads the vault out of `rig` for the sessions a commit opens.
The issuer names a new row and the claim clerk holds and finishes the draft.
The kind clerks write the rows.
The entry clerk records the revision, so the history is one list whatever kind was stored.

## `public LDraft LAuthorStart(string origin, long? authorId)`

Mints a draft id, writes the first file, and returns the held Author.
With no Author the content is a nameless one under id zero, which is an Author being named first.
With an Author the content is that Author read back, which is a rename.
An Author that is gone is refused before a file is written, so no draft can point at nothing.

## `public LAuthor LAuthorCommit(long id)`

Turns a held Author into a stored one and returns it.
A draft naming no Author is a create, one naming an Author is a rename.
An Author id naming a row since deleted is a create as well, because there is nothing left to rename.
A blank name is refused, because an Author is its name and the panel guards it too.
The Author and the revision recording it are written in one session.
The draft file is rewritten with the stored id the moment that write returns.
The held file is deleted last, so a failure anywhere above leaves the work recoverable.

## `public bool LAuthorCheck(LDraft draft, LAuthor held)`

Whether the held name differs from the stored one, blanks trimmed.
A draft naming no Author is measured against an empty name.
A draft whose Author has since gone is measured against that same empty name.

## `public LDraft LEntryStart(string origin, long? entryId, string language)`

Starts an entry draft: a blank one in `language`, or the entry loaded with its recordings resolved.
An entry that no longer stands is refused.

## `public bool LCitationDraftCheck(LDraft draft)`

Whether a draft of any kind differs from its origin.
An entry draft compares to the stored entry as loaded, or to the blank draft in its language.

## `public bool LCitationLeftoverCheck(LDraft draft)`

Whether a draft of any kind still says exactly what its stored origin says, so a sweep may drop it.
A draft whose origin is gone is not a leftover.

## `public LDraft LExampleStart(string origin, long? exampleId)`

Mints a draft id, writes the first file, and returns the held sentence.
With no Example the content is blank under an id minted for it, which is a sentence being written first.
With an Example the content is that Example read back, which is an edit.
The sentence id is given here rather than at the store, so a recovered draft names the same sentence.
An Example that is gone is refused before a file is written, so no draft can point at nothing.

## `public LExample LExampleCommit(long id)`

Turns a held sentence into a stored Example and returns it.
A draft naming no Example is a create, one naming an Example is a rewrite.
An Example id naming a record since deleted is a create as well, because there is nothing left to rewrite.
The database write runs first and whole, so a refusal from it leaves the file exactly as it was.
The write and the revision recording it share one session, so the history never misses a stored sentence.
The draft file is rewritten with the stored id and the stored sentence the moment that write returns.
Recommitting a file left nameless would store the sentence twice, and the sweep would never collect it.
The held file is deleted last, so a failure anywhere above leaves the work recoverable.
Nothing settles a court here, because a sentence links to no tentative record.

## `public bool LExampleCheck(LDraft draft, LExample held)`

Whether a held sentence differs from the Example it was started from.
A draft naming no Example is measured against an empty sentence in the language the draft already carries.
The language a new sentence opens in was chosen for it rather than typed, so it is not an edit.
A draft whose Example has since gone is measured against that same empty sentence.

## `public LDraft LReferenceStart(string origin, long? referenceId)`

Mints a draft id, writes the first file, and returns the held Reference.
With no Reference the content is blank under an id minted for it, which is a source being written first.
With a Reference the content is that Reference read back, which is an edit.
A Reference that is gone is refused before a file is written, so no draft can point at nothing.
The credits of a stored Reference are read into the draft, so the panel shows them from one place.

## `public LReference LReferenceCommit(long id)`

Turns a held Reference into a stored one and returns it.
A draft naming no Reference is a create, one naming a Reference is a rewrite.
A Reference id naming a record since deleted is a create as well, because there is nothing left to rewrite.
The Reference, its credits and the revision recording them are written in one session.
A kill between them therefore cannot leave a source with no authors or no history.
The draft file is rewritten with the stored id and the stored content the moment that write returns.
The held file is deleted last, so a failure anywhere above leaves the work recoverable.

## `public bool LReferenceCheck(LDraft draft, LReference held)`

Whether a held Reference differs from the one it was started from.
The credits count as well as the stated fields, so adding an Author is a change to save.
A draft naming no Reference is measured against an empty one.
A draft whose Reference has since gone is measured against that same empty one.

## `public LDraft LSituationStart(string origin, long? situationId)`

Mints a draft id, writes the first file, and returns the held Situation.
With no Situation the content is blank under an id minted for it, which is a context being written first.
With a Situation the content is that Situation read back, which is an edit.
A Situation that is gone is refused before a file is written, so no draft can point at nothing.

## `public LSituation LSituationCommit(long id)`

Turns a held Situation into a stored one and returns it.
A draft naming no Situation is a create, one naming a Situation is a rewrite.
A Situation id naming a record since deleted is a create as well, because there is nothing left to rewrite.
The write and the revision recording it share one session, so the history never misses a stored context.
The draft file is rewritten with the stored id and the stored content the moment that write returns.
The held file is deleted last, so a failure anywhere above leaves the work recoverable.

## `public bool LSituationCheck(LDraft draft, LSituation held)`

Whether a held Situation differs from the one it was started from.
A draft naming no Situation is measured against an empty one.
A draft whose Situation has since gone is measured against that same empty one.

## `private void LRevisionRecord(long target, string subject, bool fresh, string? summary)`

Records one revision naming the stored row, as a create when `fresh` and an update otherwise.

## `private LExample LExampleNormalize(LExample content)`

The same sentence with an id minted when it carries none.
The sentence that came in is returned itself when it was already named.
A sentence written by an older launch is what arrives here unnamed.

## `private LReference LReferenceNormalize(LReference content)`

The same Reference with an id minted when it carries none.
The Reference that came in is returned itself when it was already named.

## `private LSituation LSituationNormalize(LSituation content)`

The same Situation with an id minted when it carries none.
Its image and video rows are named the same way, and a row with no location is dropped.
The Situation that came in is returned itself when it was already named and its rows were too.
