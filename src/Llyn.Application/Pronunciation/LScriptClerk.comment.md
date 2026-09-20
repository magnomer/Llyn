# LScriptClerk.cs

## `public sealed class LScriptClerk`

Script images fetched per character and style, and stored.
The pending fetches and the misses are keyed by language and character, since two entries share a character.
The engine's gate is shared, so a fetch that lands writes under the same lock as every other vault call.

## `public LScriptClerk(LRig rig, LLanguageCache languages, object gate, Action<LSubject, long> raise)`

Reads the entry and script ports and the script source out of `rig`.

## `public IReadOnlyList<LScriptStyle> LScriptStyleRead(string language)`

The script styles the pack of `language` declares, or none for a blank language.

## `public IReadOnlyList<LScriptImage> LScriptClerkRead(long entryId)`

The stored images of every character of the entry.

## `public void LScriptClerkStart(long entryId)`

Starts a fetch for every character of the entry that has no images yet.

## `public IReadOnlyList<LScriptGroup> LScriptClerkDivide(long entryId)`

The stored images grouped by style in the pack's order.

## `public bool LScriptClerkCheck(long entryId)`

Whether a fetch is pending for any character of the entry.

## `public async Task<IReadOnlyList<LScriptImage>> LScriptClerkFind(string character, string language, CancellationToken cancellation)`

The images of one character fetched now across the styles.

## `public void LScriptClerkClear()`

Cancels every pending fetch and forgets the misses.

## `private async Task<(IReadOnlyList<LScriptImage> LScriptFound, bool LScriptReached)> LScriptClerkScan(string character, string language, CancellationToken cancellation)`

One request per style, in order.

## `private static string LScriptKeyFormat(string language, string character)`

The pending key of one character in one language.

## `private void LScriptClerkStart(long entryId, string language, string character)`

Starts one character's fetch unless it is pending or already missed.

## `private async Task LScriptClerkRun(long entryId, string language, string character, string key, CancellationTokenSource fetch)`

One fetch admitted through the two-wide gate.
A miss is remembered and a failure still raises the bulletin, so the panel stops waiting.
