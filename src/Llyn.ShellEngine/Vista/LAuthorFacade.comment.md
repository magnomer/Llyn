# LAuthorFacade.cs

## `internal sealed class LAuthorFacade`

The engine's facade for Authors.
It provides the workspace browsed by the people its Sources credit.
Every read and write goes through the author clerk under the gate, and every change is announced here.
The vista overloads stay here, because a vista is the shell's and the twin names are numbered per panel.
The author draft starts and commits here too, through the citation clerk.

## `public LAuthorFacade(LEngine engine)`

Stores the engine and its gate for author operations.

## `public IReadOnlyList<LCatalogAuthor> LEngineAuthorFind(string query, LCatalogOrder order)`

Every Author matching `query` as a browsed row carrying its Source count and its citation count.

## `public LCatalogAuthor? LEngineAuthorFind(long id)`

One Author as a browsed row, or null when none has that id.

## `public IReadOnlyList<LCatalogAuthor> LEngineAuthorFind(string query, long except, int limit)`

The rows matching `query` by name, the Author `except` left out, at most `limit` of them.

## `public IReadOnlyList<LCatalogAuthor> LEngineAuthorFind(LVista vista, string uncredited)`

The roll the authors panel lists, headed by the row standing for Sources crediting nobody.
That row is added only while the vista carries no query, and only when such Sources exist.
Its counts are the uncredited Sources and the places citing them.
Its name is the word the panel hands in.

## `public IReadOnlyList<LCatalogAuthor> LEngineAuthorFind(LVista vista)`

The Authors the authors panel's vista lists, with the query and order read off the vista.
Each row carries its chosen mark, true where its id is the one the vista stands on.

## `public IReadOnlyList<LCatalogReference> LEngineOeuvreFind(LVista roll, LVista oeuvre)`

The Sources the authors panel's oeuvre vista lists, under the Author and the kinds the roll vista holds.
Query and order come off the oeuvre vista, the chosen Author and the kind filter off the roll vista.

## `public IReadOnlyList<LCatalogReference> LEngineOeuvreFind(long? author, string query, LCatalogFilter kind, LCatalogOrder order)`

The Sources crediting `author` as browsed rows, narrowed by `query` and by the kinds left shown.
Passing `null` lists every Source, and zero lists the Sources crediting nobody.

## `public IReadOnlyList<LFellow> LEngineFellowFind(long authorId)`

Every Author credited beside `authorId` on some Source, with how many Sources credit the two together.

## `public void LEngineAuthorAbsorb(long kept, long dropped)`

Folds the Author `dropped` into the Author `kept` and deletes the dropped row.
Observers hear of both ids, because a panel standing on the dropped Author must move to the kept one.

## `public LAuthor? LEngineAuthorRead(long id)`

Reads the Author for `id`, or `null` when none has that id.

## `public IReadOnlyList<LAuthor> LEngineBylineFind(long draft, string query, int limit)`

The byline rows a typed credit may already name, at most the limit asked.
The draft is read here by its id, so the editor never carries the credits it holds back down.
No draft offers nothing.

## `public IReadOnlyList<LAuthor> LEngineAuthorRead(long ownerId, LOwner owner)`

Reads the Authors credited on the Reference identified by `ownerId`, in that Reference's own author order.

## `internal void LEngineAuthorDelete(long id, bool detach)`

The same delete, with `detach` dropping every credit first.

## `internal LDraft LEngineAuthorStart(string origin, long? authorId)`

Mints a draft id, writes the first file, and returns the held Author.
An Author that is gone is refused before a file is written, so no draft can point at nothing.

## `internal LAuthor LEngineAuthorCommit(long id)`

Turns a held Author into a stored one, announces it and returns it.
An id from a closed workspace is refused before anything is written.
