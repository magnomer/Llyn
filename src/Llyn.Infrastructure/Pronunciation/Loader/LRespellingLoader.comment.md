# LRespellingLoader.cs
Hash: `962f1ea399dc8d38`

## `internal static class LRespellingLoader`

The rewrite side of the pack loader.
It reads `cleanup`, `respelling`, `spelling` and the `rewrite` arrays of a row.
The two rewrite blocks, the spelling list and a row's rewrite array share one rule reader.
`LLanguageLoader` calls it for the rewrite blocks and the spelling list.
`LAnatomyLoader` and `LHypothesisLoader` call its rule reader.
`LReflexLoader` and `LScriptLoader` call its rewrite reader.

## `public const string LRespellingCleanup = "cleanup";`

The key of the always-on rewrite groups.

## `public const string LRespellingKey = "respelling";`

The key of the rewrite groups switched by variety.

## `private const string LRespellingSpelling = "spelling";`

The key of the spelling list.

## `private const string LRespellingRewrite = "rewrite";`

The default key of a row's rewrite array.

## `public static IReadOnlyList<LRespelling> LRespellingPackScan(JsonElement root, string key, bool scoped)`

One reader serves both rewrite blocks, `cleanup` for the always-on groups and `respelling` for the switched ones.
The two blocks carry the same shape, a list of groups each with optional varieties and rules.
A missing block reads as an empty list.
`scoped` says the pack declares varieties, and then a group without a non-empty `varieties` list is dropped.
It is dropped the same way a broken regex is, so nothing shared can load for English.
A pack without varieties keeps such groups, so Spanish works without the key.

## `private static LRespelling? LRespellingRowRead(JsonElement group)`

A group's `rules` rows are two-string arrays, and a row of any other shape is skipped.
`varieties` names the pack varieties the group applies to, and an absent or empty list applies it to every reading.
That unscoped form survives only in a pack without varieties, as the scan above enforces.
A group with no valid rule is dropped.
A group whose regex fails to compile is dropped too, so one typo never blanks the pack.
The record constructor throws on the bad pattern, and the rule scan catches it and voids that group's list.

## `public static IReadOnlyList<LRespellingRule> LRespellingSpellingScan(JsonElement root)`

The pack's `spelling` list as ordered rewrite rules.
Each row is a `[pattern, replacement]` pair, the shape a respelling group's rules take.
They recast a headword into the spelling the pack's sources key on, such as a Latin word without its macrons.
The rules are stamped on every source spec the pack declares, so one list serves every lookup kind.

## `public static IReadOnlyList<LRespellingRule> LRespellingRuleScan(JsonElement rows)`

Reads a list of `[pattern, replacement]` pairs into rules, skipping any row of another shape.
A pattern the regex engine rejects voids the whole list, so a broken pack rewrites nothing rather than half.
Shared by the respelling groups, the spelling list, and the anatomy and hypothesis tables.

## `public static IReadOnlyList<LRespellingRule> LRespellingRewriteScan(JsonElement row, string key = LRespellingRewrite)`

The `rewrite` array as ordered rules, each a two-string pair like a respelling rule.
Another key names another such array, as `recast` does for a reflex rule.
A pair whose pattern does not compile is skipped rather than failing the pack.
