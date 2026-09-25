# LAuthorClerk.cs

## `public sealed class LAuthorClerk`

The clerk over the Authors a Reference credits.
An Author is independent data, and what it holds is only its name.
So the interesting reading of an Author is the Sources crediting it and the places citing those.
Both are read here as catalogs, with the counts the rows and the orderings need already summed.
Several References may credit one Author, so a credit is a reference to a row, never ownership of it.
An Author no Reference credits is deliberate data and goes only when something says to delete it.
Folding two Authors into one lives here too, because it is the one change an Author admits beyond its name.

## `public LAuthorClerk(LRig rig)`

Reads the author and reference ports out of `rig`.

## `public LAuthor LAuthorClerkCreate(LAuthor author)`

Creates `author` and returns it with its assigned id.
A blank name is refused here rather than in the archive.
This is the door the shell knocks on, and a name left empty there is a slip.

## `public LAuthor? LAuthorClerkRead(long id)`

Reads the Author for `id`, or `null` when none has that id.

## `public IReadOnlyList<LAuthor> LAuthorClerkRead(long ownerId, LOwner owner)`

Reads the Authors credited on the Reference identified by `ownerId`, in that Reference's own author order.
Only a Reference has credits, and any other side is refused.

## `public void LAuthorClerkUpdate(LAuthor author)`

Renames the Author `author` identifies.
Every credit reads the new name.

## `public void LAuthorClerkDelete(long id, bool detach)`

The same delete, with `detach` dropping every credit first.
Deleting an Author never deletes a Reference.

## `public void LAuthorClerkAbsorb(long kept, long dropped)`

Folds the Author `dropped` into the Author `kept` and deletes the dropped row.
Every Source crediting the dropped Author credits the kept one afterwards, in the place the dropped one stood.

## `public IReadOnlyList<LAuthor> LAuthorClerkFind(string query)`

Reads the Authors whose name the typed text matches, by name.
The match is the catalog's own, so wildcards work here as they do in every search box.

## `public IReadOnlyList<LCatalogAuthor> LAuthorClerkFind(string query, LCatalogOrder order)`

Reads every Author matching `query` as a browsed row carrying its Source count and its citation count.
The credits of every Source are read once and turned round into the works of every Author.
An Author credited nowhere is still listed, because the workspace still holds it.

## `public LCatalogAuthor? LAuthorClerkFind(long id)`

One Author as a browsed row, or null when none has that id.

## `public IReadOnlyList<LCatalogAuthor> LAuthorClerkFind(string query, long except, int limit)`

The rows matching `query` by name, the Author `except` left out, at most `limit` of them.
The union section offers these as the Authors one may fold into.

## `public IReadOnlyList<LAuthor> LBylineFind(IReadOnlyList<LAuthor> credited, string query, int limit)`

The byline rows a typed credit may already name, at most the limit asked.
An unnamed Author and one the draft already credits are left out, since neither can be picked.
The names come trimmed, so the byline highlights what was typed without leading blanks.

## `public IReadOnlyList<LFellow> LFellowFind(long authorId)`

Reads every Author credited beside `authorId` on some Source, with how many Sources credit the two together.
The credits of every Source are read once, so the tally never queries per Source.
The rows come by shared count falling, then by name, case aside, so the closest co-author leads.

## `public IReadOnlyList<LAuthor> LAuthorReferenceRead(long referenceId)`

The Authors the store credits on one Reference, in order.

## `public void LAuthorReferenceSave(long referenceId, IReadOnlyList<LAuthor> authors)`

Makes the stored credits of the Reference read as the draft's list.
A credit with a minted id is created by name first.
A credit naming an Author that is gone is refused.
Each is then attached at its place in the list, which also moves one already attached.
A stored credit the list no longer names is detached last.
A name credited twice is attached once, at its first place.

## `public static bool LAuthorClerkMatch(IReadOnlyList<LAuthor> one, IReadOnlyList<LAuthor> other)`

Whether two credit lists name the same Authors in the same order.
Only the ids count, because a renamed Author is still the same credit.

## `public static bool LOeuvreMatch(long? author, IReadOnlyList<LAuthor>? credited)`

Whether a Source with the credits `credited` belongs to the oeuvre asked for.
Null asks for every Source, zero for the uncredited ones, and any other id for the Sources crediting it.

## `private static bool LAuthorCheck(IReadOnlyList<LAuthor> credited, long id)`

Whether the credits already hold the Author.

## `private static Dictionary<long, List<LReference>> LAuthorWorkRead(IReadOnlyList<LReference> references, IReadOnlyDictionary<long, IReadOnlyList<LAuthor>> credits)`

Turns the credits of every Source round into the Sources of every Author.
One pass over the credits serves every row, so the catalog never queries per Author.

## `private static ArgumentOutOfRangeException LAuthorOwnerRaise(LOwner owner)`

The refusal for a side that carries no credits.
