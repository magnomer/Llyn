# LLanguageLoader.cs
Hash: `a44fbda5640ba9f3`

## `public sealed class LLanguageLoader : LLanguageVault`

Loads a language pack from `languages/<Lang>/source.json`.
It is resolved against the application's base directory, so packs are drop-in.
Adding or editing a language needs no recompile.
A missing or malformed pack yields a language with both source lists empty rather than throwing.
So one bad pack never breaks the app.
This is the only place source.json is read.
The loaded `LLanguage` carries every language-specific fact onward.
The reading of one declared source, its attempts and its readings, sits in `LSourceLoader`.
The `fanqie` list sits in `LFanqieLoader`, and the `shengfu` block in `LShengfuLoader`.
The `reflex` list and the `order` block sit in `LReflexLoader`.
The `script` list sits in `LScriptLoader`, and the `hypothesis` tables in `LHypothesisLoader`.
The `anatomy` rules sit in `LAnatomyLoader`, and the `tone` rows in `LDescentLoader`.
The reading of the rewrite rules sits in `LRespellingLoader`, and that of the transcription schemes in `LSchemeLoader`.
The typography blocks sit in `LFontLoader`, and the `glyph` section in `LGlyphLoader`.
The shared field readers sit in `LPack`, and side files open through `LPackFile`.
The posted form and request headers of a fetch read through `LEnvelope`.
The flag download and its workspace cache sit in `LEnsignLoader`.

## `private static string? LLanguageEmblemRead(string language, JsonElement root)`

The `flag` value as a country code.
A value ending in `.svg` reads instead as the full path of that file in the pack folder.
A classical language has no country, so its pack ships its own emblem.

## `public LLanguageLoader(string root, HttpClient client)`

Checks the workspace `root` and the `client`, then hands both to the `LEnsignLoader` it builds.
The packs themselves are read beside the program, not under the root.

## `public Task<string?> LLanguageFlagRead(string code, CancellationToken cancellation)`

The port's flag read, answered by `LEnsignFileRead` in `LEnsignLoader`.

## `public string? LLanguageFlagFind(string code)`

The port's stored flag find, answered by `LEnsignFileFind` in `LEnsignLoader`.

## `public IReadOnlyList<string> LLanguageScan()`

The port's listing, answered by `LLanguageLoaderScan`.

## `public LLanguage LLanguageRead(string language)`

The port's pack read, answered by `LLanguageLoaderLoad`.

## `bool LLanguageVault.LLanguageNameValidate(string? language)`

The port's name check, answered by the static `LLanguageNameValidate` the pack loaders share.

## `public static IReadOnlyList<string> LLanguageLoaderScan()`

Scans the `languages/` folder for available packs, returning the name of every language that has a `source.json`.
A pack whose `listed` key is `false` is left off, though it still loads by name.
The set of languages is whatever is on disk, discovered at runtime.
So adding a pack folder makes it selectable with no code change.

## `public static bool LLanguageNameValidate(string? language)`

Whether `language` is a plain folder name a pack could sit under.
A separator, a rooted path, a dot name or a character no file name may hold fails.
A language name comes from an entry, and an entry can come from an imported file.
Every loader that joins the name onto the `languages/` folder asks here first, so no name walks out of it.

## `public static LLanguage LLanguageLoaderLoad(string language)`

A name `LLanguageNameValidate` refuses reads as a blank pack, so no name opens a file outside `languages/`.
The file is read anew on every call, so any caching belongs to the caller.

## Inline notes

### `private static void LLanguageSort(List<string> names)`

English always heads the language list.
Every other pack follows in ordinal order.
The order is a rule about the language set, not a detail of one menu.
So it is settled here, not in the UI.

### `string? flag = root.ValueKind == JsonValueKind.Object ? LPack.LPackTextRead(root, "flag")?.Trim() : null;`

The flag is declared in the pack as an ISO 3166-1 alpha-2 country code.
The image itself is not shipped.
`LEnsignLoader` downloads and caches it on demand.
Absent code means no flag.

### `private static IReadOnlyList<LVariety> LLanguageVarietyScan(JsonElement root)`

The pack declares its regional varieties under `varieties.list`, each with a name and a flag code.
The flag is an ISO 3166-1 alpha-2 code resolved through the same workspace path as the language flag.
A missing block reads as no varieties, and a repeated name keeps its first row.

### `private static LLanguage LLanguageRead(string language, JsonElement root)`

The top-level `tonal` key is read as a plain boolean, and only a literal `true` switches the tone contour on.
The top-level `silent` key is read the same way, and only a literal `true` hides the pronunciation rows.
The top-level `phonemic` key is read the same way, and only a literal `true` puts a respelled reading between slashes.
The top-level `anatomy` key is read by `LAnatomyPackRead` in `LAnatomyLoader`.
The top-level `tone` key is read by `LDescentPackRead` in `LDescentLoader`.
The top-level `order` key is read by `LReflexOrderRead` in `LReflexLoader`.

### `private static bool LLanguageSpacedRead(JsonElement root)`

The `spaced` boolean, which falls back to the `separator` value when absent or not a boolean.
It decides the lexical units only, and never the reading separator or mention spans.

### `private static bool LLanguageSeparatorRead(JsonElement root)`

The `separator` value, on unless the pack writes `none`.
A language whose words carry no spaces turns the reading separator off here.

### `private static bool LLanguageFlaggedCheck(JsonElement root)`

`varieties.shown` chooses how the UI labels each reading, `flag` for the flag image or `text` for the name.
The default is `text`, so a pack that lists varieties without choosing shows names.

### `private static bool LLanguageListedCheck(string file)`

Opens one pack file to ask whether it is listed, so the scan can leave an unlisted pack out.
A file that cannot be opened or parsed counts as listed, and the loader reports it as blank later.

### `private static bool LLanguageListedRead(JsonElement root)`

True unless the top-level `listed` key is a literal `false`.
