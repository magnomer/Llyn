# LShengfuClerk.cs
Hash: `c3236ed8904fb14b`

## `public sealed class LShengfuClerk`

Fills and keeps the phonetic series of the characters an entry is written with.
It fetches one character at a time behind one gate, as the fanqie clerk does for rime books.
The rows it stores are read back by the fanqie clerk, which prints them above each character's books.
Storing a row also links its character to the Stem series the row names.

## `public LShengfuClerk(LRig rig, LLanguageCache languages, object gate, Action<LSubject, long> raise)`

Takes the ports and the fault log off the rig.
It also takes the shared engine gate and the notice the shell refreshes on.

## `public LShengfuRule? LShengfuRuleRead(string language)`

The pack rule of that language, or null when the pack declares no series source.

## `public void LShengfuClerkStart(long entryId)`

Starts a fetch for each character of the headword that has no series stored yet.
A character already fetched, pending, or known to have none is left alone.

## `public void LShengfuClerkRebuild(long entryId)`

Forgets what was missing and fetches every character of the headword again.

## `public bool LShengfuClerkCheck(long entryId)`

True while a character of the headword is still being fetched.

## `public void LStemApply()`

Builds the series of every pack that declares a series source again.
A workspace opens through it, so a workspace whose rows predate the links still gets them.

## `public void LShengfuClerkClear()`

Cancels every pending fetch and forgets what was missing, as a workspace closes.

## `private string LShengfuSeparatorRead(string language)`

The separator the pack rule joins several series with, or empty when it declares no rule.

## `private static string LShengfuKeyFormat(string language, string character)`

The pending key of one character in one language.

## `private async Task<(LShengfu? LShengfuFound, bool LShengfuReached)> LShengfuClerkScan(string character, string language, CancellationToken cancellation)`

One request for the character, spaced by the rule's interval from the last one.
The wait before it runs on the rig's clock, so a harness can make it virtual.
A language whose pack declares no rule answers nothing and is not reached.

## `private void LShengfuClerkStart(long entryId, string language, string character)`

Starts one character's fetch unless it is pending or already missed.

## `private async Task LShengfuClerkRun(long entryId, string language, string character, string key, CancellationTokenSource fetch)`

One fetch admitted through the single-wide gate.
A series found is stored and linked to its Stem keys.
A miss is remembered and a failure still raises the bulletin, so the panel stops waiting.
The fetch is never awaited, so a failure is recorded in the fault log rather than lost unobserved.
Cancellation by the fetch's own token is not a failure and is not recorded.
The bulletin names the fanqie subject, because the fanqie panel prints the series.
