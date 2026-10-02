# LPhonologyPort.cs
Hash: `02b4c8efee9c384a`

## `public interface LPhonologyPort`

The slice of the engine a deportment sees when it asks about sounds and scripts.
It covers the rime-book rows, the script images, the reflex readings and the inflection paradigm of an entry.
Each of those has a check and a read, since the rows are fetched in the background.
One start opens every fetch of an entry, and the rime, script and reflex fetches each have a rebuild.
It also answers the language facts a panel words its fields by.
Those are silent, phonemic, respelled, the tone list, the schemes and the parts of speech.
`LPhonologyOutlet` implements it today, and a phonology clerk takes it over when the parts are dismantled.

## `static IReadOnlyList<int> LEngineContourScale`

The pitch levels a tone contour draws, highest first, passed up from the rule that parses the levels.

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

## `IReadOnlyList<LFanqieGroup> LEngineFanqieDivide(long entryId);`

The entry's stored rime-book rows grouped by book, starting no fetch.

## `IReadOnlyList<LFanqieGroup> LEngineFanqieRead(long entryId);`

The rows grouped by book, after starting the fetch of every character still missing.
The start comes first, so a character fetched long ago is never fetched twice.

## `string LEngineReadingRead(long entryId, string headword);`

The headword's representative reading, formed from the grouped rows.
It starts the fetch of missing characters first, as `LEngineFanqieRead` does.

## `void LEngineFanqieRebuild(long entryId);`

Fetches every character of the entry again, its phonetic series along with its rows.

## `IReadOnlyList<LScriptGroup> LEngineScriptDivide(long entryId);`

The entry's stored script images grouped by style, starting no fetch.

## `IReadOnlyList<LScriptGroup> LEngineScriptRead(long entryId);`

The images grouped by style, after starting the fetch of every character still missing.

## `IReadOnlyList<LReflexGuise> LEngineGuiseRead(string language, IReadOnlyList<string> reflexes);`

How each reflex row prints, one answer per language in `reflexes`, in order.
The fold set is the one the pack of the entry's `language` declares.

## `void LEngineSoundStart(long entryId);`

Starts every background fetch a reading view of the entry shows, in the engine's order.

## `string LEngineLanguageResolve(long entryId);`

The language the entry's paradigm is written in, or empty when it has no slots.

## `LDiweiPage LEngineDiweiResolve(long? id, Func<string, string?> localize);`

The page of one rime cell with its labels localized through the given lookup.

## `long LEngineDiweiResolve(long? id, string character);`

The entry of a character shown on one rime cell, in the cell's language, made first when none exists.

## `IReadOnlyList<LVistaRow> LEngineXiaoyunFind(long? chosen, LVista onset, LVista rime, LVista vista);`

The entries under the chosen onset and rime, none when neither is chosen.
They are read in the language of the diwei the panel reads.

## `bool LEngineStemCheck();`

True while a loaded pack declares a phonetic series source, the verdict the xiesheng tab shows by.

## `LStemPage LEngineStemResolve(long? id);`

The page of the chosen series, or the blank page when nothing was chosen.

## `long LEngineStemResolve(long? id, string character);`

The entry of a character in one phonetic series, in the series' language, made first when none exists.

## `IReadOnlyList<LVistaRow> LEngineKindredFind(LVista grove, LVista vista);`

The entry rows of the series the column chose, in its language, narrowed by the list's own query.
Nothing is listed while no series is chosen.

## `bool LEngineSilentCheck(string language);`

Whether the pack declares the language silent, so the input panel hides its pronunciation rows.
A blank language answers false, so an empty desk keeps its pronunciation rows.

## `bool LEngineRespellingCheck(string language);`

Whether respellings show for `language`, which needs the setting on and respelling groups in the pack.

## `LAccentSheet LEngineAccentRead(LEntryDraft draft);`

The pronunciation block of `draft`, ready for the reading view.

## `Task<LAccentSheet> LEngineAccentLoad(`

Loads the variety flags the block of a draft draws, then answers the block.

## `void LEngineTallySave(bool respelled);`

Persists whether a category page's tally lines print the respelling set.
The page reads the switch on every fill, so the choice survives a restart and a category change.
