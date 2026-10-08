# LQuillReference.cs
Hash: `926d3f4719bbe6a2`

## `public sealed class LQuillReference`

The typed edits of a held Source, each building exactly one request.
A driver hands it raw values, and it chooses whether the request is sent or deferred.
The Source's credits live here too, since only a Source lists Authors.

## `private readonly LTenure _lQuillReferenceTenure;`

The tenure every request is built for and handed to.

## `public LQuillReference(LTenure tenure)`

Builds the edits over one tenure, which they never swap.

## `public void LReferenceTitleSet(string text)`

Defers the held Source's typed title.

## `public void LReferenceYearSet(string text)`

Defers the held Source's typed year.

## `public void LReferenceUrlSet(string text)`

Defers the held Source's typed address.

## `public void LReferenceNoteSet(string text)`

Defers the held Source's typed note.

## `public void LReferenceKindSet(string? tag)`

Sends the kind a menu tag names at once, since it came from a click.
The reference clerk decides the kind, and a tag the Source already has sends nothing.

## `public bool LReferenceAuthorAdd(string? name, int position, long former)`

Sends the credit of a typed name at `position`, in place of the credit `former` stood for.
The draft clerk trims the name and resolves it to an Author.
A blank name sends nothing and answers false, as `LAuthorNameCheck` rules.

## `public bool LReferenceAuthorInsert(long author, int position, long former)`

Sends the credit of a stored Author at `position`, in place of the credit `former` stood for.
Picking the Author `former` already names sends nothing and answers false, as `LDraftFormerCheck` rules.

## `public void LReferenceAuthorRemove(long author)`

Sends the removal of one credit.

## `public void LReferenceAuthorMove(long author, int position)`

Sends one credit to a new place, which the draft clerk clamps to the list.
