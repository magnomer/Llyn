# LLanguageLoader.cs

## `public static partial class LLanguageLoader`

Loads a language pack from `languages//source.json`.
It is resolved against the application's base directory, so packs are drop-in.
Adding or editing a language needs no recompile.
A missing or malformed pack yields a language with both source lists empty rather than throwing.
So one bad pack never breaks the app.
This is the only place source.json is read.
The loaded `LLanguage` carries every language-specific fact onward.
The reading of one declared source, its attempts and its readings, sits in `LLanguageLoaderSource.cs`.

## `public static IReadOnlyList<string> LLanguageLoaderScan()`

Scans the `languages/` folder for available packs, returning the name of every language that has a `source.json`.
Language-agnostic: the set of languages is whatever is on disk, discovered at runtime.
So adding a pack folder makes it selectable with no code change.

## `public static bool LLanguageNameValidate(string? language)`

Whether `language` is a plain folder name a pack could sit under.
A separator, a rooted path, a dot name or a character no file name may hold fails.
A language name comes from an entry, and an entry can come from an imported file.
Every loader that joins the name onto the `languages/` folder asks here first, so no name walks out of it.

## Inline notes

### `private static void LLanguageSort(List<string> names)`

English always heads the language list.
Every other pack follows in ordinal order.
The order is a rule about the language set, not a detail of one menu.
So it is settled here, not in the UI.

### `private static IReadOnlyList<LBand> LLanguageBandScan(JsonElement root)`

The pack declares its frequency bands under `bands`, in the order they are tried.
A missing block reads as no bands, and the raw figure then shows without a label.
Order is kept because the first matching band wins.

### `private static LBand? LLanguageBandRead(JsonElement row)`

A band row carries a `name` and either an `upTo` integer or a `match` regex.
`upTo` is read first, so a row carrying both is a limit band.
A row with a blank name, with neither key, or with a regex that fails to compile is skipped.
The regex is compiled once here so one typo never blanks the pack.

### `private static LFont LLanguageFontRead(JsonElement root, string key)`

One reader serves both typography blocks the pack declares, `font` for the word and `example` for the sentence.
The two blocks carry the same shape, so a second reader would only repeat this one.
A block the pack omits reads as blank, and the theme's own typography stands.

### `string? flag = root.ValueKind == JsonValueKind.Object ? LLanguageTextRead(root, "flag") : null;`

The flag is declared in the pack as an ISO 3166-1 alpha-2 country code.
The image itself is not shipped.
The engine downloads and caches it on demand.
Absent code means no flag.

### `private static IReadOnlyList<LScheme> LLanguageSchemeScan(JsonElement root)`

The pack declares its transcription schemes under `transcription`, in the order the form shows them.
A missing or empty list turns the transcription rows off for that language.
Blank and repeated names are dropped, so the form never shows a nameless or doubled scheme.

### `private static LScheme? LLanguageSchemeRead(JsonElement scheme)`

A scheme is either a bare name or an object carrying a `name` and a `sources` list.
The `sources` list has the shape of the `pronunciation` list, so a scheme is looked up the way IPA is.
A bare name carries no sources, and the row for it is typed by hand.

### `private static IReadOnlyList<LVariety> LLanguageVarietyScan(JsonElement root)`

The pack declares its regional varieties under `varieties.list`, each with a name and a flag code.
The flag is an ISO 3166-1 alpha-2 code resolved through the same workspace path as the language flag.
A missing block reads as no varieties, and a repeated name keeps its first row.

### `private static IReadOnlyList<LRespelling> LLanguageRespellingScan(JsonElement root, string key, bool scoped)`

One reader serves both rewrite blocks, `cleanup` for the always-on groups and `respelling` for the switched ones.
The two blocks carry the same shape, a list of groups each with a name, optional varieties, and rules.
A missing block reads as an empty list.
`scoped` says the pack declares varieties, and then a group without a non-empty `varieties` list is dropped.
It is dropped the same way a broken regex is, so nothing shared can load for English.
A pack without varieties keeps such groups, so Spanish works without the key.

### `private static LRespelling? LLanguageRespellingRead(JsonElement group)`

A group's `rules` rows are two-string arrays, and a row of any other shape is skipped.
`varieties` names the pack varieties the group applies to, and an absent or empty list applies it to every reading.
That unscoped form survives only in a pack without varieties, as the scan above enforces.
A group with no valid rule is dropped.
A group whose regex fails to compile is dropped too, so one typo never blanks the pack.
The record constructor throws on the bad pattern, and this reader catches it per group.

### `private static bool LLanguageFlaggedCheck(JsonElement root)`

`varieties.shown` chooses how the UI labels each reading, `flag` for the flag image or `text` for the name.
The default is `text`, so a pack that lists varieties without choosing shows names.
