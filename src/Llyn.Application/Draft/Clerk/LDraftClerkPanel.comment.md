# LDraftClerkPanel.cs

## `public sealed class LDraftClerkPanel`

The requests of the drafts that hold no entry: the sentence panel and the source panel.
Each edits the one field of the draft that carries its kind, and refuses a draft of another kind.
The situation panel shares the situation routines with the chips and is not here.

## `public LDraftClerkPanel(LAuthorVault authors, LIdentity identity)`

Holds the shelf a credit is looked up on and the issuer that names a new Author.

## `public static LDraft LExampleChange(LDraft draft, Func<LExample, LExample> change)`

Applies one change to the sentence the draft holds, and refuses a draft holding none.

## `public static LDraft LReferenceChange(LDraft draft, Func<LReference, LReference> change)`

Applies one change to the source the draft holds, and refuses a draft holding none.

## `public static LSituation LSituationBodyApply(LSituation held, LRequestSituationBody sent)`

Resolves the written title, description and kind of the sent situation.
It lays them over the held one, keeping the held id.
The situation panel and the chips both route here through `LSituationChange`.

## `public LDraft LAuthorAdd(LDraft draft, LRequestAuthorAddition request)`

Credits the Author of the typed name at the place asked for.
A name the draft already credits, or the store already holds, is that Author rather than a second one.
Only a name nobody has is minted new, so two Sources by one person credit one Author.
The credit the field stood for gives way to the new one, and stays when the name resolves to itself.
The name is compared folded, as the catalog compares it, because case is not a different person.
An Author already credited is left where it is, as picking one is.
A blank name is an argument error, since the form never offers one.

## `private static LAuthor? LAuthorMatch(IReadOnlyList<LAuthor> authors, string name)`

The Author in the list whose folded name is the folded name given, or none.

## `public LDraft LAuthorInsert(LDraft draft, LRequestAuthorPick request)`

Credits the stored Author named, unless already credited.
An id naming no stored Author is refused.
The credit the field stood for gives way to the picked one, and stays when the pick is itself.

## `public static LDraft LAuthorRemove(LDraft draft, long authorId)`

Drops the credit carrying the id.

## `public static LDraft LAuthorMove(LDraft draft, LRequestAuthorShift request)`

Moves the credit carrying the id to the place asked for.

## `public static LDraft LAuthorChange(LDraft draft, string? name)`

Writes the typed name over the Author an author draft holds.
A draft holding no Author is refused, because the request names work of another kind.

## `private static LDraft LAuthorApply(`

Rewrites the credit list through the routine given, on a draft that holds a source.
Crediting somebody marks the authorship known, because a source with a credit cannot be one whose authors are unknown.
Removing or moving a credit leaves that mark alone.
That is what the form did before the list was held here.
