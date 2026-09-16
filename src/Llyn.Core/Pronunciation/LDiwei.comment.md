# LDiwei.cs

## `public sealed record LDiwei(`

One 音韻地位 category of a language: an initial, a rime or a tone class that placements point to.
It is a shared node, like a Tag, but derived from stored fanqie rows rather than typed by the user.
Every character whose placement carries the part links to the same row, so a category can be browsed.

**Parameters**

- `LDiweiId` — The row's own key.
- `LDiweiLanguage` — The language pack the placements belong to.
- `LDiweiKind` — Which part: `initial`, `rime` or `tone`.
- `LDiweiKey` — The part as the category names it: 來, `寒 I W` or the class `6`.
- `LDiweiCount` — How many entries of the language carry a character placed in this category.

## `public const string LDiweiInitial = "initial";`

The kind of a category made from a placement's initial, such as 來.

## `public const string LDiweiRime = "rime";`

The kind of a category made from a placement's rime, division and 開合, such as `寒 I W`.

## `public const string LDiweiTone = "tone";`

The kind of a category made from the tone class the hypothesis gives a placement, such as `6`.

## `private const string LDiweiRounded = "W";`

The mark ending a rime key of a 合口 placement, so 開 and 合 name two rows.

## `private static readonly string[] LDiweiDivisions = ["一", "二", "三", "四"];`

The division words the rime books print, in the order the Roman numerals stand for.

## `private static readonly string[] LDiweiRomans = ["I", "II", "III", "IV"];`

The Roman numeral each division becomes inside a rime key.

## `public static string LDiweiRimeNormalize(string rime)`

The rime as a category keys it: the trailing 重紐 letter dropped, so 寒A and 寒 name one row.

## `public static string LDiweiRimeFormat(string rime, string division, bool rounded)`

The key of a rime category: rime without 重紐 letter, division as Roman numeral, `W` when 合口.
So 模 一 開 keys `模 I` and 寒 一 合 keys `寒 I W`.
Every division and 開合 gets its own row.
A division outside 一 to 四 stays as printed.
Empty with an empty rime.
