# LLanguageLoader.cs

## `public static class LLanguageLoader`

Loads a language pack from `languages//source.json`.
It is resolved against the application's base directory, so packs are drop-in.
Adding or editing a language needs no recompile.
A missing or malformed pack yields a language with both source lists empty rather than throwing.
So one bad pack never breaks the app.
This is the only place source.json is read.
The loaded `LLanguage` carries every language-specific fact onward.

## `public static IReadOnlyList<string> LLanguageLoaderScan()`

Scans the `languages/` folder for available packs, returning the name of every language that has a `source.json`.
Language-agnostic: the set of languages is whatever is on disk, discovered at runtime.
So adding a pack folder makes it selectable with no code change.

## Inline notes

### `private static void LLanguageSort(List<string> names)`

English always heads the language list.
Every other pack follows in ordinal order.
The order is a rule about the language set, not a detail of one menu.
So it is settled here, not in the UI.

### `private static IReadOnlyList<LSourceSpec> LLanguageSourceScan(JsonElement root, string key)`

The pack declares its sources under `pronunciation` and `audio`, and this reads one of those lists.
Neither list is required, and a missing one simply reads as empty.
A source declares no kind, because the list it sits in already says what it is for.

### `private static LFont LLanguageFontRead(JsonElement root, string key)`

One reader serves both typography blocks the pack declares, `font` for the word and `example` for the sentence.
The two blocks carry the same shape, so a second reader would only repeat this one.
A block the pack omits reads as blank, and the theme's own typography stands.

### `string? flag = root.ValueKind == JsonValueKind.Object ? LLanguageTextRead(root, "flag") : null;`

The flag is declared in the pack as an ISO 3166-1 alpha-2 country code.
The image itself is not shipped.
The engine downloads and caches it on demand.
Absent code means no flag.

### `private static IReadOnlyList<string> LLanguageSchemeScan(JsonElement root)`

The pack declares its transcription schemes under `transcription` as a list of names.
A missing or empty list turns the transcription line off for that language.
Blank and repeated names are dropped, so the form never shows a nameless or doubled scheme.

### `private static IReadOnlyList<LSourceReading> LLanguageReadingScan(JsonElement row)`

An attempt declares its extractions either as a `readings` list or as flat keys on the attempt itself.
The flat form is the legacy shape and reads as one reading with an empty variety tag.
Audio attempts always use the flat form, so the harvest path sees one untagged reading.
A reading row without a strategy is dropped, and an attempt with no readings left is dropped too.

### `private static LSourceReading? LLanguageReadingRead(JsonElement element)`

One reader serves both the flat attempt and a row of its `readings` list, because the keys are the same.
`variety` names a variety the pack declares, or is absent for an untagged reading.
`skip` counts earlier matches to pass over, so a page can yield its second variety from its second span.
Both default to empty and zero.

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
