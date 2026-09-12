# LEngineReference.cs

## `public sealed partial class LEngine`

The bibliographic half of the engine.
A Reference is the work an Example is drawn from.
The Authors credited on it belong here too.
Both are independent shared data.
Several Entries may cite one Reference, and several References may credit one Author.
So a citation and a credit are references to a row, never ownership of it.

The two sides of a citation differ in shape, which is why `LOwner` says which is meant.
An Example cites at most one Reference, and an Entry reaches one only through the Examples its cards quote.
An Example cites at most one, a single column with nothing to order.
So attaching there replaces whatever it cited before.

Nothing here deletes a row on the strength of a reference going.
A Reference no Example cites and an Author no Reference credits are both deliberate data.
Unlike a card's Examples and Tags, they are not created as a side effect of typing.
So they go only when something says to delete them.

## `public LReference LEngineReferenceCreate(LReference reference)`

Creates `reference` and returns it with its assigned id.
Its fields carry the three-state distinction the model gives them: unspecified, deliberately unknown, or a value.

## `public LReference? LEngineReferenceRead(long id)`

Reads the Reference for `id`, or `null` when none has that id.

## `public IReadOnlyList<LReference> LEngineReferenceRead(long ownerId, LOwner owner)`

Reads the single Reference the Example identified by `ownerId` cites, as a list of one or none.
An Example cites at most one, so its list holds either that one or nothing.

## `public void LEngineReferenceUpdate(LReference reference)`

Rewrites the fields of the Reference `reference` identifies.

## `public void LEngineReferenceAttach(long ownerId, long referenceId, int position, LOwner owner)`

Cites the Reference identified by `referenceId` from the Example identified by `ownerId`.
An Example holds one citation, so `position` orders nothing.
An Example holds one citation, so the position is not used.
Whatever it cited before is replaced.

## `public void LEngineReferenceDetach(long ownerId, long referenceId, LOwner owner)`

Clears the citation the Example identified by `ownerId` holds.
The Reference and its other citations survive.

## `public void LEngineReferenceDelete(long id)`

Deletes the Reference identified by `id` together with the author credits it owns.
Refused while any Example still cites it — clear those citations first.
The Authors it credited survive.
Only the credits between them and this Reference go.

## `public LAuthor LEngineAuthorCreate(LAuthor author)`

Creates `author` and returns it with its assigned id.

A blank name is refused here rather than in the archive.
This is the door the shell knocks on, and a name left empty there is a slip.
An import writes through the archive instead, where a nameless author is a row the format allows.

## `public LAuthor? LEngineAuthorRead(long id)`

Reads the Author for `id`, or `null` when none has that id.

## `public IReadOnlyList<LAuthor> LEngineAuthorRead()`

Reads every Author the workspace holds, by name.
The Sources panel offers the whole shelf for crediting, so it asks for it once.

## `public IReadOnlyList<LAuthor> LEngineAuthorRead(long ownerId, LOwner owner)`

Reads the Authors credited on the Reference identified by `ownerId`, in that Reference's own author order.

## `public IReadOnlyDictionary<string, IReadOnlyList<LAuthor>> LEngineAuthorRead(LOwner owner)`

Reads the credits of every Reference at once, each in that Reference's own order.
`LOwner` names the kind being asked about, as the usage seam does, and only a Reference has credits.

## `public void LEngineAuthorUpdate(LAuthor author)`

Renames the Author `author` identifies.
Every credit reads the new name.

## `public void LEngineAuthorAttach(long referenceId, long authorId, int position)`

Credits the Author identified by `authorId` on the Reference identified by `referenceId` at `position` in that Reference's author order.
The Author row stays available to every other Reference.

## `public void LEngineAuthorDetach(long referenceId, long authorId)`

Removes one Reference's credit for an Author.
The Author and its other credits survive.
An Author is deliberate data, so losing a credit is not a reason to delete the person.

## `public void LEngineReferenceDelete(long id, bool detach)`

The same delete, with `detach` clearing every citation first.
The detaching, the count, and the delete stand in one store session.

## `public void LEngineAuthorDelete(long id, bool detach)`

The same delete, with `detach` dropping every credit first.
Deleting an Author never deletes a Reference.

## `public void LEngineAuthorDelete(long id)`

Deletes the Author identified by `id`.
Refused while any Reference still credits the Author — detach every credit first.
Deleting an Author never deletes a Reference.

## `public IReadOnlyList<LCatalogReference> LEngineReferenceFind(string query, LCatalogOrder order)`

The Sources answering `query`, in `order`, as rows already carrying their name, credits and citation count.
The credits and the counts are read whole rather than one Source at a time.
Both are needed to order and to match, so the browsing panel decides neither.
