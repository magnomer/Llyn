# LParadigmPort.cs
Hash: `69a99b994b18c278`

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

## `LParadigmShown LEngineParadigmCheck(LParadigmRow row, bool pending, bool enabled, bool held);`

The text and tip key that stand in a paradigm row for its form.
The wording is `LParadigmShown`'s in Core, so the list and the inflection view never disagree.
`held` is true for the editor, whose draft holds the missing forms.
Conduct receives the ready answer and only lays it into its own record.

## `LParadigmView? LEngineInflectionRead(long entryId, bool pending, bool enabled, bool held);`

The entry's inflection view in both shapes, or null without a layout or a slot on its part.
`pending`, `enabled` and `held` feed each cell's text and tip, as they do for `LEngineParadigmCheck`.
Each cell arrives ready, so a reader maps it without a rule.
