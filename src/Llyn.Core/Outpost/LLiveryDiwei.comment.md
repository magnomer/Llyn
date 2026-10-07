# LLiveryDiwei.cs
Hash: `cead8ec76bdbd658`

## `public sealed record LLiveryDiwei(string LLiveryDiweiKind, LDiweiPage LLiveryDiweiPage, IReadOnlyList<LEntry> LLiveryDiweiEntry)`

One rime-table category of a language, as the Joplin push shows it in a category note.
`LLiveryClerk.LLiveryClerkBuild` constructs one per category and keeps it in `LLiveryLanguageDiwei`.

**Parameters**

- `LLiveryDiweiKind` — One of `LDiwei.LDiweiInitial`, `LDiwei.LDiweiRime` or `LDiwei.LDiweiTone`.
- `LLiveryDiweiPage` — The page from `LDiweiClerk.LDiweiPageRead` with the respelling and tally flags of the read.
- `LLiveryDiweiEntry` — The entries from `LDiweiClerk.LDiweiEntryScan` for that category alone, with an empty query.
