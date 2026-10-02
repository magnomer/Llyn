# LReferenceFacade.cs
Hash: `8cbd0714c8059af7`

## `internal sealed class LReferenceFacade`

The bibliographic half of the engine.
A Reference is the work an Example is drawn from.
A citation is a reference to a row, never ownership of it.
Every read and write goes through the reference clerk under the gate, and every change is announced here.
The vista overload stays here, because a vista is the shell's and the twin names are numbered per panel.
The source draft starts and commits here too, through the citation clerk.

## `public LReferenceFacade(LEngine engine)`

Stores the engine and its gate for reference operations.

## `public long LEngineCitationResolve(long draftId, long cardId, long sentenceId, string title)`

The Reference a typed citation names, found or created under one hold of the gate.
The Reference cited now is read off the held draft, so no shell carries it in.
Zero card and sentence ids name the draft's own Example, as on the corpus panel.
The clerk decides the match, and only a created Reference is announced.

## `internal LReference? LEngineReferenceRead(long id)`

Reads the Reference for `id`, or `null` when none has that id.

## `public IReadOnlyList<LCatalogReference> LEngineReferenceFind(string query, LCatalogOrder order)`

The Sources answering `query`, in `order`, as rows already carrying their name, credits and citation count.

## `public LReferenceOffer LEngineReferenceFind(LTenure held, long card, long sentence, string text)`

The stored Sources a card sentence's citation field offers, ready to show.
The held draft names the Source the sentence cites, so its own byline offers nothing.

## `public IReadOnlyList<LCatalogReference> LEngineReferenceFind(LVista vista)`

The Sources the sources panel's vista lists, with the query and order read off the vista.
Each row carries its chosen mark, true where its id is the one the vista stands on.

## `internal IReadOnlyList<LCatalogReference> LEngineReferenceRead(IReadOnlyList<LCatalogReference> found, long? chosen)`

The found rows with their twin names numbered and the chosen one marked.
The sources panel and the oeuvre list share it.

## `internal void LEngineReferenceDelete(long id, bool detach)`

Deletes the Reference and announces it, with `detach` clearing every citation first.

## `public LColophon LEngineColophonRead(LDraft draft)`

The reference clerk's read sheet of a Source draft, under the gate.
The citation line counts the Source the draft holds, which is the one its panel has chosen.

## `public static LImprint LEngineImprintRead(LDraft? draft)`

The reference clerk's edit sheet of a Source draft, or of a blank Source with no draft.
It reads no store, so it takes no gate.

## `public IReadOnlyDictionary<long, string> LEngineCitationRead(LEntryDraft shown)`

The line each Source the shown entry cites is shown under, by id, ready for the display.

## `public string LEngineCitationRead(LDraft? draft)`

The line the Source a draft's own Example cites is shown under, ready for the corpus.

## `internal LDraft LEngineReferenceStart(string origin, long? referenceId)`

Mints a draft id, writes the first file, and returns the held Reference with its credits.
A Reference that is gone is refused before a file is written, so no draft can point at nothing.

## `internal LReference LEngineReferenceCommit(long id)`

Turns a held Reference into a stored one, announces it and returns it.
An id from a closed workspace is refused before anything is written.
