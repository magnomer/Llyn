# CStemMember.cs
Hash: `641d96369ebebefa`

## `public sealed record CStemMember(string CStemMemberCharacter, string CStemMemberReading, IReadOnlyList<CReflex> CStemMemberReflexes, bool CStemMemberOpened, bool CStemMemberFoldable)`

One member of a phonetic series, as the xiesheng reader prints it under the series key.
A member with no entry, or an entry with no reflexes, carries no rows and prints as the bare character.

**Parameters**

- `CStemMemberCharacter`: the member character.
- `CStemMemberReading`: the representative line, such as `/ʔak, ʔo, ʔoh/`, empty when no reading is marked.
- `CStemMemberReflexes`: the reflex rows, the same `CReflex` rows the entry page's reflex table shows.
  The entry page's per-language fold does not apply to them.
- `CStemMemberOpened`: whether this member's reflex rows are opened on this series page.
  It is the member's own state, apart from the entry page's fold.
  Folded, the member shows only its character and reading line.
- `CStemMemberFoldable`: whether the member has any reflex row, as `LStemFoldCheck` answers it.

## `internal static IReadOnlyList<CStemMember> LStemMemberRead(IReadOnlyList<LStemMember> members)`

The engine's members mapped in order.
No entry id travels, since the chip opens its entry by character.

## `private static CStemMember LStemMemberRead(LStemMember member)`

One member mapped.
Its rows are built by `CRespelling.LRespellingReflexScan` from the guises the engine handed with them.
Its fold state is copied as the engine answered it.
Its foldability is read from those rows by `LStemFoldCheck`.
So no engine call is made here.

## `private static bool LStemFoldCheck(IReadOnlyList<CReflex> rows)`

The series fold verdict: a member folds when it has at least one reflex row.
A bare member has no toggle.
The entry page's `CReflex.LReflexFoldCheck` does not apply here.
