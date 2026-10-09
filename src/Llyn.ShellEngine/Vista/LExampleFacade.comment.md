# LExampleFacade.cs
Hash: `f20eb09a45abf04e`

## `public sealed class LExampleFacade : LExamplePort`

The engine's facade for Examples.
An Example is independent data owned by nothing, so both card sides may reference the same one.
Every read and write goes through the example clerk under the gate, and every change is announced here.
The vista overload stays here, because a vista is the shell's and the twin names are numbered per panel.
The sentence draft starts and commits here too, through the `LExampleCitation` the citation clerk holds.
It implements the example port itself, so Host hands it to Conduct with no outlet between.

## `internal LExampleFacade(LEngineHearth hearth, LDraftFacade draft)`

Stores the hearth, its gate and the sibling facades it calls, all built by `LEngine` before this one.
The gate, the staff and the shared state are read through the hearth.
It takes its siblings rather than the engine, so it names only the facades it uses.

## `internal LExample? LEngineExampleRead(long id)`

Reads the Example for `id`, or `null` when no Example has that id.

## `public IReadOnlyList<LCatalogExample> LEngineExampleFind(string query, LCatalogOrder order)`

The Examples answering `query`, in `order`, as rows already carrying their cited name and quotation count.

## `public IReadOnlyList<LCatalogExample> LEngineExampleFind(LVista vista, string unknown = "", string unwritten = "")`

The Examples the corpus panel's vista lists, with the query and order read off the vista.
Each row carries its chosen mark, true where its id is the one the vista stands on.
The twin names are read from the sentence.
The panel hands in the words shown for an unknown or unwritten one.
Each row's text is worded by the same rule, so the list shows it ready.

## `internal void LEngineExampleDelete(long id, bool detach)`

Deletes the Example, dropping every reference to it first when the user asked for that.

## `internal LDraft LEngineExampleStart(string origin, long? exampleId)`

Mints a draft id, writes the first file, and returns the held sentence.
An Example that is gone is refused before a file is written, so no draft can point at nothing.

## `internal LExample LEngineExampleCommit(long id)`

Turns a held sentence into a stored Example, announces it and returns it.
An id from a closed workspace is refused before anything is written.

## `public bool LEngineTextMatch(string field, string shown)`

Whether a field showing `field` already shows the text `shown`, a blank field reading as nothing recorded.
The example clerk owns the rule.

## `public (string, IReadOnlyList<LMentionPiece>, string) LEngineLineRead(LSentenceDraft sentence, LSentenceOrder order, string mark, IReadOnlyDictionary<long, string> citations)`

The frame, the sentence divided around its Mentions and the Source line a reading card shows for one sentence row.
The example clerk composes them, so the display holds no composing rule.
