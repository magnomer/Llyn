# LFanqiePort.cs
Hash: `90813a8d5788c0cb`

## `public interface LFanqiePort`

The slice of the engine a deportment sees when it shows an entry's rime-book rows.
The rows are fetched in the background, so the port has a check beside each read.
`LFanqieFacade` implements it.

## `bool LEngineFanqieCheck(long entryId);`

Whether the entry's rime-book rows are still being fetched.

## `IReadOnlyList<LFanqieGroup> LEngineFanqieDivide(long entryId);`

The entry's stored rime-book rows grouped by book, starting no fetch.

## `IReadOnlyList<LFanqieGroup> LEngineFanqieRead(long entryId);`

The rows grouped by book, after starting the fetch of every character still missing.
A character that already has stored rows, or a fetch under way, is never fetched again.

## `string LEngineReadingRead(long entryId, string headword);`

The headword's representative reading, formed from the grouped rows.
It starts the fetch of missing characters first, as `LEngineFanqieRead` does.

## `void LEngineFanqieRebuild(long entryId);`

Fetches every character of the entry again, its phonetic series along with its rows.

## `void LEngineFanqieSet(long entryId, long fanqieId, int rank, bool raise);`

Moves one rime-book row up or down among the entry's representatives.
The fanqie clerk resolves the new rank from the held `rank` and the raise flag.

## `bool LEngineBookCheck(string language);`

Whether the language's pack declares at least one rime book.

## `bool LEngineBookCheck();`

Whether any loaded pack declares a rime book, the verdict the yunjing tab shows by.
