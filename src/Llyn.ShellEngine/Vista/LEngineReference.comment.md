# LEngineReference.cs

## `public sealed partial class LEngine`

The bibliographic half of the engine.
A Reference is the work an Example is drawn from.
A citation is a reference to a row, never ownership of it.
Every read and write goes through the reference clerk under the gate, and every change is announced here.
The vista overload stays here, because a vista is the shell's and the twin names are numbered per panel.
The source draft starts and commits here too, through the citation clerk.

## `internal LReference LEngineReferenceCreate(LReference reference)`

Creates `reference` and returns it with its assigned id.

## `public LReference LEngineCitationCreate(string title)`

Creates a Reference carrying nothing but `title`, for a citation typed where no stored Source answered.
Observers hear of the new Reference at once, because every open citation catalogue must list it.

## `internal LReference? LEngineReferenceRead(long id)`

Reads the Reference for `id`, or `null` when none has that id.

## `internal IReadOnlyList<LReference> LEngineReferenceRead()`

Every Reference the workspace holds.

## `public IReadOnlyList<LCatalogReference> LEngineReferenceFind(string query, LCatalogOrder order)`

The Sources answering `query`, in `order`, as rows already carrying their name, credits and citation count.

## `public IReadOnlyList<LCatalogReference> LEngineReferenceFind(LVista vista)`

The Sources the sources panel's vista lists, with the query and order read off the vista.
Each row carries its chosen mark, true where its id is the one the vista stands on.

## `private static IReadOnlyList<LCatalogReference> LEngineReferenceRead(IReadOnlyList<LCatalogReference> found, long? chosen)`

The found rows with their twin names numbered and the chosen one marked.
The sources panel and the oeuvre list share it.

## `internal IReadOnlyList<LReference> LEngineReferenceRead(long ownerId, LOwner owner)`

Reads the single Reference the Example identified by `ownerId` cites, as a list of one or none.

## `internal void LEngineReferenceUpdate(LReference reference)`

Rewrites the fields of the Reference `reference` identifies.

## `internal void LEngineReferenceAttach(long ownerId, long referenceId, int position, LOwner owner)`

Cites the Reference identified by `referenceId` from the Example identified by `ownerId`.
An Example holds one citation, so `position` orders nothing.

## `internal void LEngineReferenceDetach(long ownerId, long referenceId, LOwner owner)`

Clears the citation the Example identified by `ownerId` holds.

## `internal void LEngineReferenceDelete(long id)`

Deletes the Reference identified by `id`, refused while any Example still cites it, and announces it.

## `internal void LEngineReferenceDelete(long id, bool detach)`

The same delete, with `detach` clearing every citation first.

## `public IReadOnlyDictionary<long, string> LEngineCitationRead()`

The `Author (Year)` line every stored Source is cited under, by id.

## `internal LDraft LEngineReferenceStart(string origin, long? referenceId)`

Mints a draft id, writes the first file, and returns the held Reference with its credits.
A Reference that is gone is refused before a file is written, so no draft can point at nothing.

## `internal LReference LEngineReferenceCommit(long id)`

Turns a held Reference into a stored one, announces it and returns it.
An id from a closed workspace is refused before anything is written.
