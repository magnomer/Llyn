# LAnatomyLoader.cs

## `internal static class LAnatomyLoader`

The `anatomy` side of the pack loader: the rules that cut a reflex reading into onset, vowel, coda and tone.
The rules become a list of [LAnatomyRule](../../../Llyn.Core/Pronunciation/Anatomy/LAnatomyRule.comment.md), empty without them.
They live in their own file beside `source.json`, since only the Classical Chinese pack declares them.
`LLanguageLoader` calls it.

## `private const string LAnatomyKey = "anatomy";`

The key under which the pack names the file, and the key the file wraps its rows in.

## `public static IReadOnlyList<LAnatomyRule> LAnatomyPackRead(string language, JsonElement root)`

Reads the `anatomy` key: a file name opens that file in the pack folder, an array is read in place.
Any other value, or no key, yields no rules.
The named file is opened through `LPackFile`.
The file holds either the array itself or an object wrapping it under `anatomy`.
A missing or unreadable file yields no rules, and every reflex row keeps a blank anatomy.

## `private static IReadOnlyList<LAnatomyRule> LAnatomyRowScan(JsonElement rows)`

The rules of one array in written order, rows that read `null` left out.

## `private static LAnatomyRule? LAnatomyBlockRead(JsonElement row)`

One rule: `language` the names it serves, `ipa` the pattern over the reading, `respelling` the one over the respelling.
A row without a language or without an `ipa` pattern reads `null`.
A row without a `respelling` pattern cuts the respelling with the `ipa` pattern.

## `private static LAnatomyPattern? LAnatomyPatternRead(JsonElement row, string key)`

One pattern block: `match` the regex, `rewrite` the rules in respelling form, `decompose` the normalization flag.
A missing block, a blank regex or a broken one reads `null`.
