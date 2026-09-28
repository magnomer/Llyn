# LLanguageClerk.cs

## `public sealed class LLanguageClerk`

The language packs and flags of one rig.
The listed languages are read once and kept until the rig changes.

## `public LLanguageClerk(LRig rig, LLanguageCache languages, LTrailClerk trail)`

Reads the language port out of `rig` and keeps the pack cache and the trail clerk.

## `public IReadOnlyList<string> LLanguageClerkRead()`

The language names the workspace lists, scanned once.

## `public LLanguage LLanguageClerkLoad(string language)`

The pack of `language` through the cache.

## `public bool LLanguageRespellingCheck(string language, bool respelled)`

Whether respellings show for `language`.
The setting must be on and the pack must declare respelling groups.

## `public string LLanguagePronunciationRead(LEntryDraft draft, bool respelled)`

The primary reading of `draft` as a field shows it, respelled when the setting and its pack both respell.
A draft without a reading answers empty.

## `public LAccentRow LLanguageAccentRead(LPronunciationDraft? spoken, bool respelled)`

One pronunciation as the reading view prints it, its reading resolved by the draft's own rule.
`respelled` is the pack's answer for the switch, so the caller asks it once for every row.
No pronunciation answers a blank row.

## `public bool LLanguagePhonemicCheck(string language)`

Whether the pack of `language` is phonemic.

## `public Task<string?> LLanguageFlagRead(string language, CancellationToken cancellation)`

The flag image path of `language`, fetched when the code is remote.

## `public Task<string?> LVarietyFlagRead(string language, string variety, CancellationToken cancellation)`

The flag image path of one variety of `language`, or null when the variety declares none.

## `private async Task<string?> LLanguageFlagResolve(string? code, CancellationToken cancellation)`

A rooted code is a local file and answers itself when it exists.
Any other code is fetched through the language port.
