# LEngineRequestPanel.cs

## `public sealed partial class LEngine`

The requests of the drafts that hold no entry: the sentence panel and the source panel.
Each edits the one field of the draft that carries its kind, and refuses a draft of another kind.
The situation panel shares the situation routines with the chips and is not here.

## `private static LDraft LEngineExampleChange(LDraft draft, Func<LExample, LExample> change)`

Applies one change to the sentence the draft holds, and refuses a draft holding none.

## `private static LDraft LEngineReferenceChange(LDraft draft, Func<LReference, LReference> change)`

Applies one change to the source the draft holds, and refuses a draft holding none.

## `private static LReference LEngineBodyApply(LReference held, LRequestReferenceBody sent)`

Resolves each written field of the sent source and lays it over the held one, keeping the held id.
Nothing is compared here.
The caller drops the result when it equals what was held.

## `private static LExample LEngineBodyApply(LExample held, LRequestExampleBody sent)`

Resolves the written text and the chosen source of the sent sentence.
The Glosses are left as held, because each is a row the Gloss requests edit on its own.
It lays them, with the language, over the held one, keeping the held id.

## `private static LSituation LEngineBodyApply(LSituation held, LRequestSituationBody sent)`

Resolves the written title, description and kind of the sent situation.
It lays them over the held one, keeping the held id.
The situation panel and the chips both route here through `LEngineSituationChange`.

## `private LDraft LEngineAuthorAdd(LDraft draft, LRequestAuthorAddition request)`

Credits the Author of the typed name at the place asked for.
A name the draft already credits, or the store already holds, is that Author rather than a second one.
Only a name nobody has is minted new, so two Sources by one person credit one Author.
The name is compared folded, as the catalog compares it, because case is not a different person.
An Author already credited is left where it is, as picking one is.
A blank name is an argument error, since the form never offers one.

## `private static LAuthor? LEngineAuthorMatch(IReadOnlyList<LAuthor> authors, string name)`

The Author in the list whose folded name is the folded name given, or none.

## `private LDraft LEngineAuthorInsert(LDraft draft, LRequestAuthorPick request)`

Credits the stored Author named, unless already credited.
An id naming no stored Author is refused.

## `private static LDraft LEngineAuthorRemove(LDraft draft, long authorId)`

Drops the credit carrying the id.

## `private static LDraft LEngineAuthorMove(LDraft draft, LRequestAuthorShift request)`

Moves the credit carrying the id to the place asked for.

## `private static LDraft LEngineAuthorChange(LDraft draft, string name)`

Writes the typed name over the Author an author draft holds.
A draft holding no Author is refused, because the request names work of another kind.

## `private static LDraft LEngineAuthorApply(`

Rewrites the credit list through the routine given, on a draft that holds a source.
Crediting somebody marks the authorship known, because a source with a credit cannot be one whose authors are unknown.
Removing or moving a credit leaves that mark alone.
That is what the form did before the list was held here.
