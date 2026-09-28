# LFanqieFacade.cs

## `internal sealed class LFanqieFacade`

The engine's facade for fanqie, wrapping the fanqie, diwei and tally reads of the fanqie and diwei clerks.
The vista-shaped finds stay here, since a vista is an engine handle.

## `public LFanqieFacade(LEngine engine)`

The facade bound to its engine and the engine's gate.

## `public IReadOnlyList<LFanqieBook> LEngineBookRead(string language)`

The fanqie books of a language.

## `public bool LEngineBookCheck(string language)`

Whether the language declares any book.

## `public string? LEngineBookFind()`

The first listed language that declares a book, or null.

## `public bool LEngineBookCheck()`

True while a listed language declares a book, the verdict the yunjing tab is shown by.

## `public IReadOnlyList<LFanqieGroup> LEngineFanqieDivide(long entryId)`

The rows grouped by book.

## `public IReadOnlyList<LFanqieGroup> LEngineFanqieRead(long entryId)`

The rows grouped by book, after starting the fetch of every character still missing.
The start comes first, so a character fetched long ago is never fetched twice.

## `public string LEngineReadingRead(long entryId, string headword)`

The headword's representative reading, formed from the grouped rows.
It starts the fetch of missing characters first, as the grouped read does.

## `public void LEngineFanqieStart(long entryId)`

Starts the fetch of every character that has no rows.
It starts the phonetic-series fetch too, since one box prints both.

## `public void LEngineFanqieRebuild(long entryId)`

Fetches every character of the entry again, its series along with its rows.

## `public bool LEngineFanqieCheck(long entryId)`

Whether a fetch of rows or of a series is pending for any character of the entry.

## `internal IReadOnlyList<LDiwei> LEngineDiweiRead(string language, string kind)`

The diwei of one kind in one language.

## `public LDiwei? LEngineDiweiRead(long? id)`

One diwei by id, or null for no id.

## `public LDiweiPage LEngineDiweiResolve(long? id, Func<string, string?> localize)`

The page of one diwei, blank for no id.
Respellings show under the respelling setting for the diwei's language, and the tally under its own setting.

## `public long LEngineDiweiResolve(long? id, string character)`

The entry of a character on the diwei's page, in the page's language, made first when none exists.
The entry resolve is the entry facade's own, so a glyph chip anywhere lands on the same entry.
The diwei is read under this facade's gate, and the resolve takes the entry facade's gate afterwards.
With no diwei the page's language is blank, which the resolve refuses.

## `public (long, bool)? LEngineDiweiFind(string language, string kind, string key)`

The identity of the diwei of one kind with the given key, and whether it is a rime.
A diwei never stored answers null.

## `public IReadOnlyList<LDiwei> LEngineDiweiFind(LVista vista, long? chosen, bool final)`

The initials or the rimes as one column, filtered by the vista's query and sorted by its order.
The column lists the language of the chosen diwei, the one the panel reads.
The chosen one is marked, and a chosen id no longer listed clears the choice.
Nothing is listed while no language declares a book.

## `public IReadOnlyList<LVistaRow> LEngineXiaoyunFind(string language, IReadOnlyList<long> diweiIds, string query, LVista? vista = null)`

The entries filed under all of the diwei that match `query`, built as vista rows.

## `public IReadOnlyList<LVistaRow> LEngineXiaoyunFind(long? chosen, LVista onset, LVista rime, LVista vista)`

The entries under the chosen onset and rime, none when neither is chosen.
They are read in the language of the diwei the panel reads.

## `private string LEngineLanguageRead(long? chosen)`

The language the columns list, that of the chosen diwei, else the first language declaring a book.
Empty while neither answers.
