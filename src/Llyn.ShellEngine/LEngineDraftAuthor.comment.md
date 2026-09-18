# LEngineDraftAuthor.cs

## `public sealed partial class LEngine`

The held-draft calls that belong to an Author rather than to an entry, a sentence, a situation or a source.
An Author is a shared row edited alone, so an author draft carries no entry, no source and no credits.
It follows `LEngineDraftReference.cs` call for call, so the fifth kind cannot invent a fifth protocol.
The shared claim, sweep, recovery and discard read `LDraftAuthorHeld` and branch where the kinds part.
Only starting, committing and checking differ by kind, and only those live here.

## `internal LDraft LEngineAuthorStart(string origin, long? authorId)`

Mints a draft id, writes the first file, and returns the held Author.
With no Author the content is a nameless one under id zero, which is an Author being named first.
With an Author the content is that Author read back, which is a rename.
An Author that is gone is refused before a file is written, so no draft can point at nothing.
A claim naming this process is written beside the draft, as every other kind of draft does.

## `internal LAuthor LEngineAuthorCommit(long id)`

Turns a held Author into a stored one and returns it.
A draft naming no Author is a create, one naming an Author is a rename.
An Author id naming a row since deleted is a create as well, because there is nothing left to rename.
A blank name is refused, because an Author is its name and the panel guards it too.
The Author and the revision recording it are written in one session.
The draft file is rewritten with the stored id the moment that write returns, as the source commit does.
The held file is deleted last, so a failure anywhere above leaves the work recoverable.

## `private bool LEngineAuthorCheck(LDraft draft, LAuthor held)`

Whether the held name differs from the stored one, blanks trimmed.
A draft naming no Author is measured against an empty name.
A draft whose Author has since gone is measured against that same empty name.
This is the domain rule the authors panel used to keep in its own comparison.
