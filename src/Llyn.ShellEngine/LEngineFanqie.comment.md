# LEngineFanqie.cs

## `public sealed partial class LEngine`

The fanqie side of the engine: the rime-book placements of the characters an entry is written with.
A character is fetched once, when a display asks for what is not stored, and kept in the workspace database.
The fetch runs one character at a time, because the site refuses posts that follow each other closely.

## `private readonly SemaphoreSlim _lEngineFanqieGate = new(1, 1);`

Admits one character's fetch at a time, so the interval between posts holds across characters.

## `private readonly Dictionary<string, CancellationTokenSource> _lEngineFanqiePending = new(StringComparer.Ordinal);`

The fetches under way, keyed by language and character, each with the token that abandons it.

## `private readonly HashSet<string> _lEngineFanqieMissed = new(StringComparer.Ordinal);`

The characters every book answered for yet none placed, asked once per session.

## `private DateTime _lEngineFanqieStamp = DateTime.MinValue;`

The moment the next post may go out, pushed forward by each post's interval and never pulled back.
A book without an interval, asked between two posts of a throttled one, leaves that one's wait standing.

## `public IReadOnlyList<LFanqieBook> LEngineBookRead(string language)`

The rime books the language's pack lists, empty for a blank language.

## `public LHypothesis? LEngineHypothesisRead(string language)`

The reconstruction the language's pack declares, or `null` for a blank language or a pack without one.

## `public IReadOnlyList<LFanqieRow> LEngineFanqieRead(long entryId)`

Every stored row of every character of the entry's headword, in headword order.
A language whose pack lists no book reads empty.

## `public void LEngineFanqieStart(long entryId)`

Starts a fetch for every character of the entry's headword that has nothing stored.
A language whose pack lists no book starts nothing.

## `public void LEngineFanqieRebuild(long entryId)`

Fetches every character of the entry again, on the user's request.
The stored rows stand until the fetch lands, and the save rewrites them under their ids.
So the anchors reflex rows hold on them survive the rebuild.
A character the sites once answered nothing for is asked again too.

## `public bool LEngineFanqieCheck(long entryId)`

Whether a fetch runs for any character of the entry.

## `public async Task<IReadOnlyList<LFanqieRow>> LEngineFanqieFind(`

Fetches the character's rows from every book without storing them, for a caller that wants the answer itself.

## `private async Task<(IReadOnlyList<LFanqieRow> LFanqieFound, bool LFanqieReached)> LEngineFanqieScan(`

Posts each book in pack order, waiting out the interval before each post, and gathers the rows.
Reached is true when any book answered, and false when every book was silent or busy.

## `private static string LFanqieKeyFormat(string language, string character)`

The pending and missed key of one character in one language.

## `private void LEngineFanqieStart(long entryId, string language, string character)`

Starts a fetch for the character unless one runs or the character was already missed this session.

## `private void LEngineFanqieClear()`

Abandons every fetch under way and forgets the missed characters, on workspace change and disposal.

## `private async Task LEngineFanqieRun(`

The fetch itself: waits for the gate, scans the books, stores what was found and raises the fanqie bulletin.
The bulletin is raised whether or not anything was found, so a display waiting on it can stop waiting.
A silent or busy answer stores nothing and leaves the character to be asked again.
A reached answer with nothing found marks the character missed.
A fetch abandoned by a workspace change stores nothing and raises nothing.

### `new LDiweiArchive(_lEngineDatabase).LDiweiApply(`

Stored rows are placed into their categories at once, so the links never lag the placements.
