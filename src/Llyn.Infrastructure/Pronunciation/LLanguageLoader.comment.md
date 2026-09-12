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
