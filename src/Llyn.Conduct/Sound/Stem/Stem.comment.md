# Stem

This folder holds a phonetic series column entry, its member and its page.

## `CStem`, `CStemMember`, `CStemPage`

A phonetic series column entry, its member and its page.
A member with no entry, or an entry with no reflexes, carries no rows and prints as the bare character.
The member's reflex rows are the same `CReflex` rows the entry page shows.
The entry page's per-language fold does not apply to them.
The opened state is the member's own, apart from the entry page's fold.
Folded, the member shows only its character and reading line.

`LStemMemberRead` makes no engine call.
Its rows come from `CRespelling.LRespellingReflexScan` over the guises the engine handed with them.
No entry id travels, since the chip opens its entry by character.
`LStemFoldCheck` folds a member when it has at least one reflex row, and a bare member has no toggle.

## Stamp

Commit: `c2f335c331fe`
- `CStem.cs` `8700686d99f90703`
- `CStemMember.cs` `641d96369ebebefa`
- `CStemPage.cs` `638208580b45e336`
