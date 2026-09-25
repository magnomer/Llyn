# LExampleFacade.cs

## `internal sealed class LExampleFacade`

The engine's facade for Examples.
An Example is independent data owned by nothing, so both card sides may reference the same one.
Every read and write goes through the example clerk under the gate, and every change is announced here.
The vista overload stays here, because a vista is the shell's and the twin names are numbered per panel.
The sentence draft starts and commits here too, through the citation clerk.

## `public LExampleFacade(LEngine engine)`

Stores the engine and its gate for example operations.

## `internal LExample? LEngineExampleRead(long id)`

Reads the Example for `id`, or `null` when no Example has that id.

## `public IReadOnlyList<LCatalogExample> LEngineExampleFind(string query, LCatalogOrder order)`

The Examples answering `query`, in `order`, as rows already carrying their cited name and quotation count.

## `public IReadOnlyList<LCatalogExample> LEngineExampleFind(LVista vista, string unknown = "", string unwritten = "")`

The Examples the corpus panel's vista lists, with the query and order read off the vista.
Each row carries its chosen mark, true where its id is the one the vista stands on.
The twin names are read from the sentence.
The panel hands in the words shown for an unknown or unwritten one.

## `internal void LEngineExampleDelete(long id, bool detach)`

Deletes the Example, dropping every reference to it first when the user asked for that.

## `internal LDraft LEngineExampleStart(string origin, long? exampleId)`

Mints a draft id, writes the first file, and returns the held sentence.
An Example that is gone is refused before a file is written, so no draft can point at nothing.

## `internal LExample LEngineExampleCommit(long id)`

Turns a held sentence into a stored Example, announces it and returns it.
An id from a closed workspace is refused before anything is written.
