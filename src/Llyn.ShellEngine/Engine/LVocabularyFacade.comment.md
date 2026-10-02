# LVocabularyFacade.cs
Hash: `07edbb38ea8111d1`

## `internal sealed class LVocabularyFacade`

The engine facade for vocabulary, inflections and paradigms.
Every call hands the work to the vocabulary, inflection or paradigm clerk.
The vocabulary and paradigm calls take the gate first, and the lacuna clerk holds it for the inflection calls.
The facade stays because the shell calls the engine, and the engine alone holds the gate and the pending fetch.

## `public LVocabularyFacade(LEngine engine)`

Stores the engine and its gate, which the facade uses for its vocabulary operations.

## `internal void LEngineLanguageImport()`

Every language pack's vocabulary written into the workspace, through the clerk.
It runs whenever the engine binds to a workspace, under the constructor or the rig apply.

## `public IReadOnlyList<LParadigmRow> LEngineParadigmScan(long entryId)`

The entry's paradigm slots joined into the rows a paradigm box shows.

## `public string LEngineLanguageResolve(long entryId)`

The language the entry's paradigm is written in, or empty when it has no slots.

## `public bool LEngineInflectionCheck(long entryId)`

Whether an inflection fetch is pending for the entry, through the lacuna clerk.

## `public void LEngineInflectionStart(long entryId)`

Starts the inflection fetch of an entry through the lacuna clerk.
