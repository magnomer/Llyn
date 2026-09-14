# LEngineAuthor.cs

## `public sealed partial class LEngine`

The authors panel's half of the engine: the workspace browsed by the people its Sources credit.
An Author is independent data, and what it holds is only its name.
So the interesting reading of an Author is the Sources crediting it and the places citing those.
Both are read here as catalogs, with the counts the rows and the orderings need already summed.
Folding two Authors into one lives here too, because it is the one change an Author admits beyond its name.

## `public IReadOnlyList<LCatalogAuthor> LEngineAuthorFind(string query, LCatalogOrder order)`

Reads every Author matching `query` as a browsed row carrying its Source count and its citation count.
The credits of every Source are read once and turned round into the works of every Author.
An Author credited nowhere is still listed, because the workspace still holds it.
The rows come back ordered under `order`, or under the name where an Author does not answer to that ordering.

## `public IReadOnlyList<LCatalogReference> LEngineOeuvreFind(long? author, string query, LCatalogFilter kind, LCatalogOrder order)`

Reads the Sources crediting `author` as browsed rows, narrowed by `query` and by the kinds left shown.
Passing `null` lists every Source, as the sources panel lists every Entry under no chosen Source.
Passing zero lists the Sources crediting nobody, so a missing credit can be found and written.
The kinds are matched by their stored word, which is what the filter remembers between sessions.

## `public void LEngineAuthorAbsorb(long kept, long dropped)`

Folds the Author `dropped` into the Author `kept` and deletes the dropped row.
Every Source crediting the dropped Author credits the kept one afterwards, in the place the dropped one stood.
Observers hear of both ids, because a panel standing on the dropped Author must move to the kept one.

## `private static bool LEngineOeuvreMatch(long? author, IReadOnlyList<LAuthor>? credited)`

Whether a Source with the credits `credited` belongs to the oeuvre asked for.
Null asks for every Source, zero for the uncredited ones, and any other id for the Sources crediting it.

## `private static Dictionary<long, List<LReference>> LEngineWorkRead(IReadOnlyList<LReference> references, IReadOnlyDictionary<long, IReadOnlyList<LAuthor>> credits)`

Turns the credits of every Source round into the Sources of every Author.
One pass over the credits serves every row, so the catalog never queries per Author.
