# LLanguageLoaderRespelling.cs

## `public static partial class LLanguageLoader`

The rewrite side of the pack loader: `cleanup`, `respelling`, `spelling`, `separator` and `transcription`.
The two rewrite blocks and the spelling list share one rule reader.
The transcription schemes read their sources the way a pronunciation source is read.

## `private static IReadOnlyList<LRespelling> LLanguageRespellingScan(JsonElement root, string key, bool scoped)`

One reader serves both rewrite blocks, `cleanup` for the always-on groups and `respelling` for the switched ones.
The two blocks carry the same shape, a list of groups each with a name, optional varieties, and rules.
A missing block reads as an empty list.
`scoped` says the pack declares varieties, and then a group without a non-empty `varieties` list is dropped.
It is dropped the same way a broken regex is, so nothing shared can load for English.
A pack without varieties keeps such groups, so Spanish works without the key.

## `private static LRespelling? LLanguageRespellingRead(JsonElement group)`

A group's `rules` rows are two-string arrays, and a row of any other shape is skipped.
`varieties` names the pack varieties the group applies to, and an absent or empty list applies it to every reading.
That unscoped form survives only in a pack without varieties, as the scan above enforces.
A group with no valid rule is dropped.
A group whose regex fails to compile is dropped too, so one typo never blanks the pack.
The record constructor throws on the bad pattern, and this reader catches it per group.

## `private static IReadOnlyList<LRespellingRule> LLanguageSpellingScan(JsonElement root)`

The pack's `spelling` list as ordered rewrite rules.
Each row is a `[pattern, replacement]` pair, the shape a respelling group's rules take.
They recast a headword into the spelling the pack's sources key on, such as a Latin word without its macrons.
The rules are stamped on every source spec the pack declares, so one list serves every lookup kind.

## `private static IReadOnlyList<LRespellingRule> LLanguageRuleScan(JsonElement rows)`

Reads a list of `[pattern, replacement]` pairs into rules, skipping any row of another shape.
A pattern the regex engine rejects voids the whole list, so a broken pack rewrites nothing rather than half.
Shared by the respelling groups and the spelling list.

## `private static IReadOnlyList<LScheme> LLanguageSchemeScan(JsonElement root)`

The pack declares its transcription schemes under `transcription`, in the order the form shows them.
A missing or empty list turns the transcription rows off for that language.
Blank and repeated names are dropped, so the form never shows a nameless or doubled scheme.

## `private static LScheme? LLanguageSchemeRead(JsonElement scheme)`

A scheme is either a bare name or an object carrying a `name` and a `sources` list.
The `sources` list has the shape of the `pronunciation` list, so a scheme is looked up the way IPA is.
A bare name carries no sources, and the row for it is typed by hand.


## `private static bool LLanguageSeparatorRead(JsonElement root)`

The `separator` value, on unless the pack writes `none`.
A language whose words carry no spaces turns the reading separator off here.
