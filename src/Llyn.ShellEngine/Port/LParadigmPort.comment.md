# LParadigmPort.cs
Hash: `cc9d9c84fdee59b9`

## `public interface LParadigmPort`

The slice of the engine a deportment sees when it shows an entry's inflection paradigm.
The inflections are fetched in the background, so the port has a check beside the scan.
`LVocabularyFacade` implements it.

## `bool LEngineInflectionCheck(long entryId);`

Whether the entry's inflections are still being fetched.

## `IReadOnlyList<LParadigmRow> LEngineParadigmScan(long entryId);`

The entry's paradigm rows, each slot joined with its stored inflection.
Slots on the part a layout draws are left to `LEngineInflectionRead`.

## `string LEngineLanguageResolve(long entryId);`

The language the entry's paradigm is written in, or empty when it has no slots.

## `LParadigmStatus LEngineParadigmCheck(LParadigmRow row, bool pending, bool enabled);`

What stands in a paradigm row for its form.
The readers ask the clerk's rule through the port, so the rule has one owner.

## `LParadigmView? LEngineInflectionRead(long entryId, bool pending, bool enabled);`

The entry's inflection view in both shapes, or null without a layout or a slot on its part.
`pending` and `enabled` feed each cell's status, as they do for `LEngineParadigmCheck`.
