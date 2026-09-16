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
The reading of the `script` list, one character style per row, sits in `LLanguageLoaderScript.cs`.
The reading of the `fanqie` list, one rime book per row, sits in `LLanguageLoaderFanqie.cs`.
The reading of the `reflex` list, one fetch rule per borrowing language, sits in `LLanguageLoaderReflex.cs`.
The reading of the `hypothesis` file, the reconstruction tables, sits in `LLanguageLoaderHypothesis.cs`.
The reading of the rewrite rules and the transcription schemes sits in `LLanguageLoaderRespelling.cs`.

## `private static string? LLanguageFlagRead(string language, JsonElement root)`

The `flag` value as a country code.
A value ending in `.svg` reads instead as the full path of that file in the pack folder.
A classical language has no country, so its pack ships its own emblem.

## `public static IReadOnlyList<string> LLanguageLoaderScan()`

Scans the `languages/` folder for available packs, returning the name of every language that has a `source.json`.
A pack whose `listed` key is `false` is left off, though it still loads by name.
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

### `private static LFont LLanguageFontRead(JsonElement root, string key)`

One reader serves both typography blocks the pack declares, `font` for the word and `example` for the sentence.
The two blocks carry the same shape, so a second reader would only repeat this one.
A block the pack omits reads as blank, and the theme's own typography stands.

### `string? flag = root.ValueKind == JsonValueKind.Object ? LLanguageTextRead(root, "flag") : null;`

The flag is declared in the pack as an ISO 3166-1 alpha-2 country code.
The image itself is not shipped.
The engine downloads and caches it on demand.
Absent code means no flag.

### `private static IReadOnlyList<LVariety> LLanguageVarietyScan(JsonElement root)`

The pack declares its regional varieties under `varieties.list`, each with a name and a flag code.
The flag is an ISO 3166-1 alpha-2 code resolved through the same workspace path as the language flag.
A missing block reads as no varieties, and a repeated name keeps its first row.

### `private static LLanguage LLanguageRead(string language, JsonElement root)`

The top-level `tonal` key is read as a plain boolean, and only a literal `true` switches the tone contour on.
The top-level `silent` key is read the same way, and only a literal `true` hides the pronunciation rows.
The top-level `phonemic` key is read the same way, and only a literal `true` puts a respelled reading between slashes.
The top-level `listed` key is read by `LLanguageListedRead`, and only a literal `false` takes the pack off the picker.
The top-level `anatomy` key is read by `LLanguageAnatomyRead` in `LLanguageLoaderAnatomy.cs`.

### `private static bool LLanguageFlaggedCheck(JsonElement root)`

`varieties.shown` chooses how the UI labels each reading, `flag` for the flag image or `text` for the name.
The default is `text`, so a pack that lists varieties without choosing shows names.

### `private static bool LLanguageListedCheck(string file)`

Opens one pack file to ask whether it is listed, so the scan can leave an unlisted pack out.
A file that cannot be opened or parsed counts as listed, and the loader reports it as blank later.

### `private static bool LLanguageListedRead(JsonElement root)`

True unless the top-level `listed` key is a literal `false`.
