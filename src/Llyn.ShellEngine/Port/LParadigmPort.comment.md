# LParadigmPort.cs
Hash: `93efdf4627478dea`

## `public interface LParadigmPort`

The slice of the engine a deportment sees when it shows an entry's inflection paradigm.
The inflections are fetched in the background, so the port has a check beside the scan.
`LVocabularyFacade` implements it.

## `bool LEngineInflectionCheck(long entryId);`

Whether the entry's inflections are still being fetched.

## `IReadOnlyList<LParadigmRow> LEngineParadigmScan(long entryId);`

The entry's paradigm rows, each slot joined with its stored inflection.

## `string LEngineLanguageResolve(long entryId);`

The language the entry's paradigm is written in, or empty when it has no slots.

## `LParadigmStatus LEngineParadigmCheck(LParadigmRow row, bool pending, bool enabled);`

What stands in a paradigm row for its form.
The readers ask the clerk's rule through the port, so the rule has one owner.
