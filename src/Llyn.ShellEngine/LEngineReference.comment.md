# LEngineReference.cs

## `public sealed partial class LEngine`

The bibliographic half of the engine.
A Reference is the work an Entry or an Example is drawn from.
The Authors credited on it belong here too.
Both are independent shared data.
Several Entries may cite one Reference, and several References may credit one Author.
So a citation and a credit are references to a row, never ownership of it.

The two sides of a citation differ in shape, which is why `LOwner` says which is meant.
An Entry cites any number of References and holds their order.
An Example cites at most one, a single column with nothing to order.
So attaching there replaces whatever it cited before.

Nothing here deletes a row on the strength of a reference going.
A Reference no Entry cites and an Author no Reference credits are both deliberate data.
Unlike a card's Examples and Tags, they are not created as a side effect of typing.
So they go only when something says to delete them.

## `public LReference LEngineReferenceCreate(LReference reference)`

Creates `reference` and returns it with its assigned id.
Its fields carry the three-state distinction the model gives them: unspecified, deliberately unknown, or a value.

## `public LReference? LEngineReferenceRead(string id)`

Reads the Reference for `id`, or `null` when none has that id.

## `public IReadOnlyList<LReference> LEngineReferenceRead(string ownerId, LOwner owner)`

Reads the References the Entry or Example identified by `ownerId` cites, in citation order.
An Example cites at most one, so its list holds either that one or nothing.

## `public void LEngineReferenceUpdate(LReference reference)`

Rewrites the fields of the Reference `reference` identifies.

## `public void LEngineReferenceAttach(string ownerId, string referenceId, int position, LOwner owner)`

Cites the Reference identified by `referenceId` from the Entry or Example identified by `ownerId`.
An Entry holds the citation at `position` among its own.
An Example holds one citation, so the position is not used.
Whatever it cited before is replaced.

## `public void LEngineReferenceDetach(string ownerId, string referenceId, LOwner owner)`

Removes an Entry's citation of the Reference identified by `referenceId`, or clears the one an Example cites.
The Reference and its other citations survive.

## `public void LEngineReferenceDelete(string id)`

Deletes the Reference identified by `id` together with the author credits it owns.
Refused while any Entry or Example still cites it — remove those citations first.
The Authors it credited survive.
Only the credits between them and this Reference go.

## `public LAuthor LEngineAuthorCreate(LAuthor author)`

Creates `author` and returns it with its assigned id.

## `public LAuthor? LEngineAuthorRead(string id)`

Reads the Author for `id`, or `null` when none has that id.

## `public IReadOnlyList<LAuthor> LEngineAuthorRead(string ownerId, LOwner owner)`

Reads the Authors credited on the Reference identified by `ownerId`, in that Reference's own author order.

## `public void LEngineAuthorUpdate(LAuthor author)`

Renames the Author `author` identifies.
Every credit reads the new name.

## `public void LEngineAuthorAttach(string referenceId, string authorId, int position)`

Credits the Author identified by `authorId` on the Reference identified by `referenceId` at `position` in that Reference's author order.
The Author row stays available to every other Reference.

## `public void LEngineAuthorDetach(string referenceId, string authorId)`

Removes one Reference's credit for an Author.
The Author and its other credits survive.
An Author is deliberate data, so losing a credit is not a reason to delete the person.

## `public void LEngineAuthorDelete(string id)`

Deletes the Author identified by `id`.
Refused while any Reference still credits the Author — detach every credit first.
Deleting an Author never deletes a Reference.
