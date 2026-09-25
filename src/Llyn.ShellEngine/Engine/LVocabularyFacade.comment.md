# LVocabularyFacade.cs

## `internal sealed class LVocabularyFacade`

The engine facade for vocabulary, inflections and paradigms.
Every call takes the gate and hands the work to the vocabulary, inflection or paradigm clerk.
The facade stays because the shell calls the engine, and the engine alone holds the gate and the pending fetch.

## `public LVocabularyFacade(LEngine engine)`

Stores the engine and its gate, which the facade uses for its vocabulary operations.

## `internal void LEngineLanguageImport()`

Every language pack's vocabulary written into the workspace, through the clerk.
It runs whenever the engine binds to a workspace, under the constructor or the rig apply.

## `public IReadOnlyList<LParadigmSlot> LEngineParadigmShow(long entryId)`

The clerk's display read under the gate, the regular forms dropped.

## `public bool LEngineInflectionCheck(long entryId)`

Whether an inflection fetch is pending for the entry, through the lacuna clerk.

## `public void LEngineInflectionStart(long entryId)`

Starts the inflection fetch of an entry through the lacuna clerk.
