# LExampleCitation.cs
Hash: `1d239544b9b6979b`

## `public sealed class LExampleCitation`

The start, commit and check of a held sentence, one kind of the citation protocol.
`LCitationClerk` builds and holds it beside the three other kinds.

## `public LExampleCitation(LRig rig, LIdentity identity, LClaimClerk claims, LExampleClerk examples, LEntryClerk entries)`

Reads the vault out of `rig` for the session a commit opens.
The issuer names a new sentence and the claim clerk holds and finishes the draft.
The Example clerk writes the row, and the entry clerk records the revision.

## `public LDraft LExampleCitationStart(string origin, long? exampleId)`

Mints a draft id, writes the first file, and returns the held sentence.
With no Example the content is blank under an id minted for it, which is a sentence being written first.
With an Example the content is that Example read back, which is an edit.
The sentence id is given here rather than at the store, so a recovered draft names the same sentence.
An Example that is gone is refused before a file is written, so no draft can point at nothing.

## `public LExample LExampleCitationCommit(long id)`

Turns a held sentence into a stored Example and returns it.
A draft naming no Example is a create, one naming an Example is a rewrite.
An Example id naming a record since deleted is a create as well, because there is nothing left to rewrite.
The database write runs first and whole, so a refusal from it leaves the file exactly as it was.
The write and the revision recording it share one session, so the history never misses a stored sentence.
The draft file is rewritten with the stored id and the stored sentence the moment that write returns.
Recommitting a file left nameless would store the sentence twice, and the sweep would never collect it.
The held file is deleted last, so a failure anywhere above leaves the work recoverable.
Nothing settles a court here, because a sentence links to no tentative record.

## `public bool LExampleCitationCheck(LDraft draft, LExample held)`

Whether a held sentence differs from the Example it was started from.
A draft naming no Example is measured against an empty sentence in the language the draft already carries.
The language a new sentence opens in was chosen for it rather than typed, so it is not an edit.
A draft whose Example has since gone is measured against that same empty sentence.

## `private LExample LExampleCitationNormalize(LExample content)`

The same sentence with an id minted when it carries none.
The sentence that came in is returned itself when it was already named.
A sentence written by an older launch is what arrives here unnamed.
