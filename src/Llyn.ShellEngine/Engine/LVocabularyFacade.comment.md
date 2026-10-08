# LVocabularyFacade.cs
Hash: `8a15f6105aee9c44`

## `public sealed class LVocabularyFacade : LParadigmPort, LSentencePort`

The engine facade for vocabulary, inflections and paradigms.
Every call hands the work to the vocabulary, inflection or paradigm clerk.
The vocabulary and paradigm calls take the gate first, and the lacuna clerk holds it for the inflection calls.
The facade stays because the shell calls the engine, and the engine alone holds the gate and the pending fetch.
It implements the paradigm and sentence ports itself, so Host hands it to Conduct with no outlet between.

## `public LVocabularyFacade(LEngine engine)`

Stores the engine and its gate, which the facade uses for its vocabulary operations.

## `public IReadOnlyList<LSpeechValue> LEngineSpeechRead(string language)`

The parts of speech the language declares, in the vocabulary clerk's order.

## `public LSpeechValue? LEngineSpeechAdd(string language, string name)`

Makes a typed part of speech one of the language's presets, or returns the one that already carries the name.

## `public LSentenceOrder LEngineOrderRead(string language)`

Which of a sentence's two frame fields the language's pack writes first.

## `public IReadOnlyList<string> LEngineParticleRead(string language)`

The markers already saved under entries written in the language.

## `public IReadOnlyList<string> LEngineDependenceRead(string language)`

The roles already saved under entries written in the language.

## `internal void LEngineLanguageImport()`

Every language pack's vocabulary written into the workspace, through the clerk.
It runs whenever the engine binds to a workspace, under the constructor or the rig apply.

## `public IReadOnlyList<LParadigmRow> LEngineParadigmScan(long entryId)`

The entry's paradigm slots joined into the rows a paradigm box shows.

## `public string LEngineLanguageResolve(long entryId)`

The language the entry's paradigm is written in, or empty when it has no slots.

## `public LParadigmStatus LEngineParadigmCheck(LParadigmRow row, bool pending, bool enabled)`

What stands in a paradigm row for its form, by the paradigm clerk's rule.
It takes no gate, since the rule reads only the row and the two flags.

## `public bool LEngineInflectionCheck(long entryId)`

Whether an inflection fetch is pending for the entry, through the lacuna clerk.

## `public void LEngineInflectionStart(long entryId)`

Starts the inflection fetch of an entry through the lacuna clerk.
