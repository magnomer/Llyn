# LReferenceCitation.cs
Hash: `063db9078cb801da`

## `public sealed class LReferenceCitation`

The start, commit and check of a held Reference, one kind of the citation protocol.
`LCitationClerk` builds and holds it beside the three other kinds.

## `public LReferenceCitation(LRig rig, LIdentity identity, LClaimClerk claims, LAuthorClerk authors, LReferenceClerk references, LEntryClerk entries)`

Reads the vault out of `rig` for the session a commit opens.
The issuer names a new Reference and the claim clerk holds and finishes the draft.
The Reference clerk writes the row and the Author clerk writes its credits.
The entry clerk records the revision.

## `public LDraft LReferenceCitationStart(string origin, long? referenceId)`

Mints a draft id, writes the first file, and returns the held Reference.
With no Reference the content is blank under an id minted for it, which is a source being written first.
With a Reference the content is that Reference read back, which is an edit.
A Reference that is gone is refused before a file is written, so no draft can point at nothing.
The credits of a stored Reference are read into the draft, so the panel shows them from one place.

## `public LReference LReferenceCitationCommit(long id)`

Turns a held Reference into a stored one and returns it.
A draft naming no Reference is a create, one naming a Reference is a rewrite.
A Reference id naming a record since deleted is a create as well, because there is nothing left to rewrite.
The Reference, its credits and the revision recording them are written in one session.
A kill between them therefore cannot leave a source with no authors or no history.
The draft file is rewritten with the stored id and the stored content the moment that write returns.
The held file is deleted last, so a failure anywhere above leaves the work recoverable.

## `public bool LReferenceCitationCheck(LDraft draft, LReference held)`

Whether a held Reference differs from the one it was started from.
The credits count as well as the stated fields, so adding an Author is a change to save.
A draft naming no Reference is measured against an empty one.
A draft whose Reference has since gone is measured against that same empty one.

## `private LReference LReferenceCitationNormalize(LReference content)`

The same Reference with an id minted when it carries none.
The Reference that came in is returned itself when it was already named.
