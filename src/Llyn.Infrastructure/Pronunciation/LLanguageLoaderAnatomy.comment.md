# LLanguageLoaderAnatomy.cs

## `public static partial class LLanguageLoader`

The `anatomy` side of the pack loader: the rules that cut a reflex reading into onset, vowel, coda and tone.
The rules become a list of [LAnatomyRule](../../Llyn.Core/Pronunciation/LAnatomyRule.comment.md), empty without them.
They live in their own file beside `source.json`, since only the Classical Chinese pack declares them.

## `private const string LLanguageLoaderAnatomy = "anatomy";`

The key under which the pack names the file, and the key the file wraps its rows in.

## `private static IReadOnlyList<LAnatomyRule> LLanguageAnatomyRead(string language, JsonElement root)`

Reads the `anatomy` key: a file name opens that file in the pack folder, an array is read in place.
Any other value, or no key, yields no rules.

## `private static IReadOnlyList<LAnatomyRule> LLanguageAnatomyLoad(string language, string file)`

Opens the named file in the language's pack folder and reads its rows.
The file holds either the array itself or an object wrapping it under `anatomy`.
The name must be a bare file name, so a pack cannot point outside its folder.
A missing or unreadable file yields no rules, and every reflex row keeps a blank anatomy.

## `private static IReadOnlyList<LAnatomyRule> LLanguageAnatomyScan(JsonElement rows)`

The rules of one array in written order, rows that read `null` left out.

## `private static LAnatomyRule? LLanguageBlockRead(JsonElement row)`

One rule: `language` the names it serves, `ipa` the pattern over the reading, `respelling` the one over the respelling.
A row without a language or without an `ipa` pattern reads `null`.
A row without a `respelling` pattern cuts the respelling with the `ipa` pattern.

## `private static IReadOnlyList<string> LLanguageNameScan(JsonElement row)`

The `language` value as a list: one name for a string, every string for an array, trimmed and blanks dropped.

## `private static LAnatomyPattern? LLanguagePatternRead(JsonElement row, string key)`

One pattern block: `match` the regex, `rewrite` the rules in respelling form, `decompose` the normalization flag.
A missing block, a blank regex or a broken one reads `null`.
