# LLanguageLoader.cs

## `public static class LLanguageLoader`

Loads a language pack from `languages//source.json`, resolved against the application's base directory so packs are drop-in: adding or editing a language needs no recompile. A missing or malformed pack yields a language with no sources rather than throwing, so one bad pack never breaks the app. This is the only place source.json is read; the loaded `LLanguage` carries every language-specific fact onward.

## `public static IReadOnlyList<string> LLanguageLoaderScan()`

Scans the `languages/` folder for available packs, returning the name of every language that has a `source.json`. Language-agnostic: the set of languages is whatever is on disk, discovered at runtime, so adding a pack folder makes it selectable with no code change.

## Inline notes

### `private static void LLanguageSort(List<string> names)`

English always heads the language list; every other pack follows in ordinal order. The order is a rule about the language set, not a detail of one menu, so it is settled here, not in the UI.

### `string? flag = root.ValueKind == JsonValueKind.Object ? LLanguageTextRead(root, "flag") : null;`

The flag is declared in the pack as an ISO 3166-1 alpha-2 country code; the image itself is not shipped. The engine downloads and caches it on demand. Absent code means no flag.
