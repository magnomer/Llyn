# LCitationClerk.cs
Hash: `39736932219914eb`

## `public sealed class LCitationClerk`

The held-draft calls that belong to a citation rather than to an entry.
A citation is one of the independent facts an entry cites.
That ring is an Author, an Example, a Reference or a Situation.
Each is shared data owned by nothing, so its draft names the row and carries no entry.
Only starting, committing and checking differ by kind.
Each kind has its own owner for those three, built and held here.
`LAuthorCitation`, `LExampleCitation`, `LReferenceCitation` and `LSituationCitation` follow one protocol call for call.
So a fifth kind cannot invent a fifth protocol.
The checks across all kinds, and the start of an entry draft, live here.
Every edit in between is a request, applied by `LDraftClerk` for all kinds alike.
The claim, the sweep and the discard are one for all kinds, and live in `LClaimClerk`.
The rows themselves are written through the clerk of each kind.

## `public LCitationClerk(LRig rig, LIdentity identity, LClaimClerk claims, LAuthorClerk authors, LExampleClerk examples, LReferenceClerk references, LSituationClerk situations, LEntryClerk entries)`

Builds the four kind owners from `rig`, the issuer, the claim clerk and the kind clerks.
The entry clerk records every revision, so the history is one list whatever kind was stored.
It also loads the entry a draft starts from or is checked against.

## `public LAuthorCitation LCitationClerkAuthor { get; }`

The start, commit and check of a held Author.

## `public LExampleCitation LCitationClerkExample { get; }`

The start, commit and check of a held sentence.

## `public LReferenceCitation LCitationClerkReference { get; }`

The start, commit and check of a held Reference.

## `public LSituationCitation LCitationClerkSituation { get; }`

The start, commit and check of a held Situation.

## `public LDraft LEntryStart(string origin, long? entryId, string language)`

Starts an entry draft.
It is a blank one in `language`, or the entry loaded with its recordings resolved.
An entry that no longer stands is refused.

## `public bool LCitationDraftCheck(LDraft draft)`

Whether a draft of any kind differs from its origin.
A citation draft is measured by the check of its kind owner.
An entry draft compares to the stored entry as loaded, or to the blank draft in its language.

## `public bool LCitationLeftoverCheck(LDraft draft)`

Whether a draft of any kind still says exactly what its stored origin says, so a sweep may drop it.
A draft whose origin is gone is not a leftover.

## `internal static void LRevisionRecord(LEntryClerk entries, long target, string subject, bool fresh, string? summary)`

Records one revision naming the stored row, as a create when `fresh` and an update otherwise.
Every kind owner records through this one call, so the four commits share one rule.
