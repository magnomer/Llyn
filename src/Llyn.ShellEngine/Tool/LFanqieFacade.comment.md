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

## `public LHypothesis? LEngineHypothesisRead(string language)`

The reconstruction hypothesis of a language, or null.

## `public IReadOnlyList<LFanqieRow> LEngineFanqieRead(long entryId)`

The stored fanqie rows of every character of an entry.

## `public IReadOnlyList<LFanqieGroup> LEngineFanqieDivide(long entryId)`

The rows grouped by book.

## `public void LEngineFanqieStart(long entryId)`

Starts the fetch of every character that has no rows.
It starts the phonetic-series fetch too, since one box prints both.

## `public void LEngineFanqieRebuild(long entryId)`

Fetches every character of the entry again, its series along with its rows.

## `public bool LEngineFanqieCheck(long entryId)`

Whether a fetch of rows or of a series is pending for any character of the entry.

## `internal Task<IReadOnlyList<LFanqieRow>> LEngineFanqieFind(string character, string language, CancellationToken cancellation)`

The rows of one character fetched now.

## `internal IReadOnlyList<LDiwei> LEngineDiweiRead(string language, string kind)`

The diwei of one kind in one language.

## `public LDiwei? LEngineDiweiRead(long? id)`

One diwei by id, or null for no id.

## `public LDiweiPage LEngineDiweiResolve(long? id, Func<string, string?> localize)`

The page of one diwei, blank for no id.
Respellings show under the respelling setting for the diwei's language, and the tally under its own setting.

## `public LDiwei? LEngineDiweiFind(string language, string kind, string key)`

The diwei of one kind with the given key.

## `public IReadOnlyList<LDiwei> LEngineDiweiFind(LVista vista, string language, string kind)`

The diwei of one kind filtered by the vista's query and sorted by its order, the chosen one marked.
A chosen id no longer listed clears the choice.

## `public IReadOnlyList<LFanqieRow> LEngineFanqieRead(LDiwei diwei)`

The fanqie rows filed under a diwei.

## `internal IReadOnlyList<long> LEngineDiweiScan(string language, IReadOnlyList<long> diweiIds)`

The entry ids filed under all of the diwei.

## `public IReadOnlyList<LVistaRow> LEngineXiaoyunFind(string language, IReadOnlyList<long> diweiIds, string query, LVista? vista = null)`

The entries filed under all of the diwei that match `query`, built as vista rows.

## `public IReadOnlyList<LVistaRow> LEngineXiaoyunFind(string language, LVista onset, LVista rime, LVista vista)`

The entries under the chosen onset and rime, none when neither is chosen.

## `internal void LEngineDiweiRebuild()`

Rebuilds the diwei of every language that declares books.

## `public IReadOnlyList<LTally> LEngineTallyRead(LDiwei diwei)`

The reflexes anchored to the diwei's rows, tallied.
