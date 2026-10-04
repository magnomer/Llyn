# LDiweiPage.cs
Hash: `db55061a6b4e8db3`

## `public sealed record LDiweiPage(string LDiweiPageLanguage, string LDiweiPageKey, IReadOnlyList<LDiweiSection> LDiweiPageSections)`

The page of one Diwei category as the engine composes it.
It holds the language, the key, and the sorted sections.
The blank page stands for no category, so the veneer never branches on null.
