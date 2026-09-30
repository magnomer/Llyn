# LPhonologyPort.cs

## `public interface LPhonologyPort`

The slice of the engine a deportment sees when it asks about sounds and scripts.
It covers the rime-book rows, the script images, the reflex readings and the inflection paradigm of an entry.
Each of those has a check and a read, since the rows are fetched in the background.
One start opens every fetch of an entry, and the reflexes keep a start of their own for the editor.
It also answers the language facts a panel words its fields by.
Those are silent, phonemic, respelled, the tone list, the schemes and the parts of speech.
`LEngine` implements it today, and a phonology clerk takes it over when the parts are dismantled.

## `const int LEngineContourFloor = LLanguageClerk.LLanguageContourFloor;`

The lowest pitch level a tone contour draws, passed up from the rule that parses the levels.
`LEngineContourCeiling` is the highest.

## `const int LEngineContourCeiling = LLanguageClerk.LLanguageContourCeiling;`

The highest pitch level a tone contour draws, passed up from the rule that parses the levels.

## `IReadOnlyList<LContour> LEngineContourRead(string language, string ipa);`

The tone contour syllables of `ipa` in `language`, empty when nothing is drawn.

## `static LArticulation LEngineConsonantRead()`

The IPA consonant chart, which needs no engine, so the input aid reads it before any workspace opens.

## `static LArticulation LEngineVowelRead()`

The IPA vowel chart, which needs no engine, so the input aid reads it before any workspace opens.

## `static LParadigmStatus LEngineParadigmCheck(LParadigmRow row, bool pending, bool enabled)`

What stands in a paradigm row for its form.
It needs no engine, so the readers ask the clerk's rule through the port.

## `static string? LEngineDiweiRead(bool initial, string key)`

The kind of the cell a pressed fanqie key opens, or none for a blank key.
It needs no engine, so both reading views' gates ask it before the navigation opens.

## `bool LEngineFanqieCheck(long entryId);`

Whether the entry's rime-book rows are still being fetched.

## `void LEngineSoundStart(long entryId);`

Starts every background fetch a reading view of the entry shows, in the engine's order.

## `LDiweiPage LEngineDiweiResolve(long? id, Func<string, string?> localize);`

The page of one rime cell with its labels localized through the given lookup.

## `long LEngineDiweiResolve(long? id, string character);`

The entry of a character shown on one rime cell, in the cell's language, made first when none exists.

## `long LEngineStemResolve(long? id, string character);`

The entry of a character in one phonetic series, in the series' language, made first when none exists.

## `LAccentSheet LEngineAccentRead(LEntryDraft draft);`

The pronunciation block of `draft`, ready for the reading view.

## `Task<LAccentSheet> LEngineAccentLoad(`

Loads the variety flags the block of a draft draws, then answers the block.
