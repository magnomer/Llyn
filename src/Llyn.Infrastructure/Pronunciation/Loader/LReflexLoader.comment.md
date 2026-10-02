# LReflexLoader.cs

## `internal static class LReflexLoader`

The reflex side of the pack loader: the `reflex` list naming one fetch rule per borrowing language.
`LLanguageLoader` calls it.

## `private const string LReflexKey = "reflex";`

The key of the rule list.

## `private const string LReflexRecast = "recast";`

The key of a rule's romanization rewrite array.

## `public static IReadOnlyList<LReflexRule> LReflexPackScan(JsonElement root)`

Every well-formed rule under `reflex`, in written order, or an empty list when the key is absent.

## `private static LReflexRule? LReflexClauseRead(JsonElement row)`

One rule from one object of the list.
An object lacking a language, an address or a match pattern yields `null`.
`format`, `every`, `busy`, `interval`, `headers`, `epithet`, `clip`, `first` and `rewrite` are optional.
So are `region`, `split`, `gloss`, `until` and `folded`.
They are read as the rule's place, its cut, its gloss pattern, its gloss bound and its fold.
`recast` is read like `rewrite` but over the romanization, and `superscript` raises the romanization's digits.
A `form` object turns the fetch into a post.
