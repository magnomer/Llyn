# LLanguageLoaderScript.cs

## `public static partial class LLanguageLoader`

The `script` side of the pack loader: the character styles a Han language pack lists.
Each row becomes an [LScriptStyle](../../Llyn.Core/Pronunciation/LScriptStyle.comment.md), in written order.

## `private static IReadOnlyList<LScriptStyle> LLanguageScriptScan(JsonElement root)`

Reads every row of the `script` array, skipping rows the style reader refuses.
A pack without the array yields an empty list, and the reading view shows no script box.

## `private static LScriptStyle? LLanguageScriptRead(JsonElement row)`

One style row: its name, the address the form is posted to, and the pattern reading the answer.
A row missing any of the three is dropped, because none of them has a default worth guessing.
The group numbers, prefix, rewrite rules and gloss pattern are optional.

## `private static IReadOnlyDictionary<string, string> LLanguageFormRead(JsonElement row)`

The `form` object as posted fields, string values only, in written order.
A row without a form posts nothing but still asks the address.

## `private static IReadOnlyList<LRespellingRule> LLanguageRewriteScan(JsonElement row, string key = LLanguageLoaderRewrite)`

The `rewrite` array as ordered rules, each a two-string pair like a respelling rule.
Another key names another such array, as `recast` does for a reflex rule.
A pair whose pattern does not compile is skipped rather than failing the pack.
