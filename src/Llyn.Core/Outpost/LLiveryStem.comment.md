# LLiveryStem.cs
Hash: `94c0be6ac23776ec`

## `public sealed record LLiveryStem(LStemPage LLiveryStemPage, IReadOnlyList<LEntry> LLiveryStemEntry)`

One phonetic series of a language, as the Joplin push shows it in a series note.
`LLiveryClerk.LLiveryClerkBuild` constructs one per series and keeps it in `LLiveryLanguageStem`.

**Parameters**

- `LLiveryStemPage` — The page from `LStemClerk.LStemPageRead` for the series.
- `LLiveryStemEntry` — The entries from `LStemClerk.LStemEntryScan` for that series alone, with an empty query.

## `public const string LLiveryStemKind = "stem";`

The kind a series link names, kept apart from the `LDiwei` kinds so links never collide.
