# LEngineRequestPanel.cs

## `public sealed partial class LEngine`

The requests of the drafts that hold no entry: the sentence panel and the source panel.
Each edits the one field of the draft that carries its kind, and refuses a draft of another kind.
The situation panel shares the situation routines with the chips and is not here.

## `private static LDraft LEngineExampleChange(LDraft draft, Func<LExample, LExample> change)`

Applies one change to the sentence the draft holds, and refuses a draft holding none.

## `private static LDraft LEngineReferenceChange(LDraft draft, Func<LReference, LReference> change)`

Applies one change to the source the draft holds, and refuses a draft holding none.

## `private LDraft LEngineAuthorAdd(LDraft draft, LRequestAuthorAddition request)`

Credits a new Author with the trimmed name and a minted id, at the place asked for.
A blank name is an argument error, since the form never offers one.

## `private LDraft LEngineAuthorInsert(LDraft draft, LRequestAuthorPick request)`

Credits the stored Author named, unless already credited.
An id naming no stored Author is refused.

## `private static LDraft LEngineAuthorRemove(LDraft draft, long authorId)`

Drops the credit carrying the id.

## `private static LDraft LEngineAuthorMove(LDraft draft, LRequestAuthorShift request)`

Moves the credit carrying the id to the place asked for.

## `private static LDraft LEngineAuthorApply(`

Rewrites the credit list through the routine given, on a draft that holds a source.
Crediting somebody marks the authorship known, because a source with a credit cannot be one whose authors are unknown.
Removing or moving a credit leaves that mark alone.
That is what the form did before the list was held here.
