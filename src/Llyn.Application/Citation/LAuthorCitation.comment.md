# LAuthorCitation.cs
Hash: `cdee2f6c0e2709e3`

## `public sealed class LAuthorCitation`

The start, commit and check of a held Author, one kind of the citation protocol.
`LCitationClerk` builds and holds it beside the three other kinds.

## `public LAuthorCitation(LRig rig, LClaimClerk claims, LAuthorClerk authors, LEntryClerk entries)`

Reads the vault out of `rig` for the session a commit opens.
The claim clerk holds and finishes the draft, and the Author clerk writes the row.
The entry clerk records the revision.

## `public LDraft LAuthorCitationStart(string origin, long? authorId)`

Mints a draft id, writes the first file, and returns the held Author.
With no Author the content is a nameless one under id zero, which is an Author being named first.
With an Author the content is that Author read back, which is a rename.
An Author that is gone is refused before a file is written, so no draft can point at nothing.

## `public LAuthor LAuthorCitationCommit(long id)`

Turns a held Author into a stored one and returns it.
A draft naming no Author is a create, one naming an Author is a rename.
An Author id naming a row since deleted is a create as well, because there is nothing left to rename.
A blank name throws `LRefusal.LRefusalName`, because an Author is its name and the panel guards it too.
The Author and the revision recording it are written in one session.
The draft file is rewritten with the stored id the moment that write returns.
The held file is deleted last, so a failure anywhere above leaves the work recoverable.

## `public bool LAuthorCitationCheck(LDraft draft, LAuthor held)`

Whether the held name differs from the stored one, blanks trimmed.
A draft naming no Author is measured against an empty name.
A draft whose Author has since gone is measured against that same empty name.
