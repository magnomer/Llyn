# LFanqieClerk.cs

## `public sealed class LFanqieClerk`

Fanqie rows fetched per character and book, stored, and rebuilt into diwei.
The pending fetches and the misses are keyed by language and character, since two entries share a character.
The engine's gate is shared, so a fetch that lands writes under the same lock as every other vault call.

## `public LFanqieClerk(LRig rig, LLanguageCache languages, object gate, Action<LSubject, long> raise)`

Reads the entry, fanqie, diwei and language ports, the fanqie source and the clock out of `rig`.

## `public IReadOnlyList<LFanqieBook> LFanqieBookRead(string language)`

The fanqie books the pack of `language` declares, or none for a blank language.

## `public LHypothesis? LHypothesisRead(string language)`

The reconstruction hypothesis the pack of `language` declares, or null.

## `public IReadOnlyList<LFanqieRow> LFanqieClerkRead(long entryId)`

The stored rows of every character of the entry, tone formatted by the localized pattern.

## `public IReadOnlyList<LFanqieGroup> LFanqieClerkDivide(long entryId)`

The stored rows grouped by book in the pack's order.
The stored phonetic series of each character rides on that character's first block.

## `public void LFanqieClerkStart(long entryId)`

Starts a fetch for every character of the entry that has no rows yet.

## `public void LFanqieClerkRebuild(long entryId)`

Forgets the misses of the entry's characters and fetches them again.

## `public bool LFanqieClerkCheck(long entryId)`

Whether a fetch is pending for any character of the entry.

## `public void LFanqieClerkClear()`

Cancels every pending fetch and forgets the misses.

## `public void LDiweiApply()`

Rebuilds the diwei of every listed language that declares books.

## `private async Task<(IReadOnlyList<LFanqieRow> LFanqieFound, bool LFanqieReached)> LFanqieClerkScan(string character, string language, CancellationToken cancellation)`

One request per book, spaced by the book's interval on the clock.

## `private static string LFanqieKeyFormat(string language, string character)`

The pending key of one character in one language.

## `private string LShengfuSeparatorRead(string language)`

The separator the pack rule joins several series with, or empty when it declares no rule.

## `private IReadOnlyList<LShengfu> LShengfuStoredScan(string language, IReadOnlyList<string> characters)`

The series [LShengfuClerk](LShengfuClerk.comment.md) has already stored for those characters.
This clerk only reads them, because the box printing the books is the box printing the series.

## `private void LFanqieClerkStart(long entryId, string language, string character)`

Starts one character's fetch unless it is pending or already missed.

## `private async Task LFanqieClerkRun(long entryId, string language, string character, string key, CancellationTokenSource fetch)`

One fetch admitted through the single-wide gate.
Rows found are stored and the character's diwei reapplied.
A miss is remembered and a failure still raises the bulletin, so the panel stops waiting.
