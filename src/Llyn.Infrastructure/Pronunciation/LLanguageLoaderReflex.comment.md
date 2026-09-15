# LLanguageLoaderReflex.cs

## `public static partial class LLanguageLoader`

The reflex side of the pack loader: the `reflex` list naming one fetch rule per borrowing language.

## `private static IReadOnlyList<LReflexRule> LLanguageReflexScan(JsonElement root)`

Every well-formed rule under `reflex`, in written order, or an empty list when the key is absent.

## `private static LReflexRule? LLanguageReflexRead(JsonElement row)`

One rule from one object of the list.
An object lacking a language, an address or a match pattern yields `null`.
`format`, `every`, `busy`, `interval`, `headers`, `epithet`, `clip`, `first` and `rewrite` are optional.
A `form` object turns the fetch into a post.
