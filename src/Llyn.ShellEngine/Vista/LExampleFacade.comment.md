# LExampleFacade.cs

## `internal sealed class LExampleFacade`

The engine's facade for Examples.
An Example is independent data owned by nothing, so both card sides may reference the same one.
Every read and write goes through the example clerk under the gate, and every change is announced here.
The vista overload stays here, because a vista is the shell's and the twin names are numbered per panel.
The sentence draft starts and commits here too, through the citation clerk.

## `public LExampleFacade(LEngine engine)`

Stores the engine and its gate for example operations.

## `internal LExample LEngineExampleCreate(LExample example)`

Creates `example` and returns it with its assigned id.

## `internal LExample? LEngineExampleRead(long id)`

Reads the Example for `id`, or `null` when no Example has that id.

## `internal IReadOnlyList<LExample> LEngineExampleRead()`

Every Example in the workspace, for the panel that browses the shared stock of sentences itself.

## `public IReadOnlyList<LCatalogExample> LEngineExampleFind(string query, LCatalogOrder order)`

The Examples answering `query`, in `order`, as rows already carrying their cited name and quotation count.

## `public IReadOnlyList<LCatalogExample> LEngineExampleFind(LVista vista, string unknown = "", string unwritten = "")`

The Examples the corpus panel's vista lists, with the query and order read off the vista.
Each row carries its chosen mark, true where its id is the one the vista stands on.
The twin names are read from the sentence.
The panel hands in the words shown for an unknown or unwritten one.

## `internal IReadOnlyList<LExample> LEngineExampleRead(long ownerId, LOwner owner)`

Reads the Examples the Meaning or Collocation identified by `ownerId` references, in the order that side holds them.

## `internal IReadOnlyList<LSentence> LEngineSentenceRead(long meaningId)`

The sentence rows one Meaning holds, in its order.

## `internal IReadOnlyList<LSentence> LEngineSentenceRead(long ownerId, LOwner owner)`

The sentence rows the Meaning or Collocation identified by `ownerId` holds.

## `internal void LEngineExampleUpdate(LExample example)`

Rewrites the language, the sentence and the translation of the Example `example` identifies.

## `internal void LEngineExampleUpdate(long exampleId, LStateAnchor reference)`

Sets or clears the single Source the Example identified by `exampleId` cites.

## `internal void LEngineExampleAttach(long ownerId, long exampleId, int position, LOwner owner)`

References the Example from the Meaning or Collocation identified by `ownerId`, at `position`.

## `internal void LEngineExampleDetach(long ownerId, long exampleId, LOwner owner)`

Removes one side's reference to an Example, leaving the Example and its other references.

## `internal void LEngineExampleRemove(long ownerId, long exampleId, LOwner owner)`

Removes one side's reference to an Example and deletes the Example when that was its last reference.
The detach, the count, the delete and the owner's stamp share one session.
So the row is judged against the references as they stand at that moment.

## `internal void LEngineExampleDelete(long id)`

Deletes the Example identified by `id`, refused while any card still references it, and announces it.

## `internal void LEngineExampleDelete(long id, bool detach)`

Deletes the Example, dropping every reference to it first when the user asked for that.

## `internal LDraft LEngineExampleStart(string origin, long? exampleId)`

Mints a draft id, writes the first file, and returns the held sentence.
An Example that is gone is refused before a file is written, so no draft can point at nothing.

## `internal LExample LEngineExampleCommit(long id)`

Turns a held sentence into a stored Example, announces it and returns it.
An id from a closed workspace is refused before anything is written.
