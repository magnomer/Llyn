# LDiwei.cs

## `public sealed record LDiwei(`

One 音韻地位 category of a language: an initial, a rime or a tone class that placements point to.
It is a shared node, like a Tag, but derived from stored fanqie rows rather than typed by the user.
Every character whose placement carries the part links to the same row, so a category can be browsed.

**Parameters**

- `LDiweiId` — The row's own key.
- `LDiweiLanguage` — The language pack the placements belong to.
- `LDiweiKind` — Which part: `initial`, `rime` or `tone`.
- `LDiweiKey` — The part as the category names it: 來, `寒W I` or the class `6`.
- `LDiweiCount` — How many entries of the language carry a character placed in this category.

## `public const string LDiweiInitial = "initial";`

The kind of a category made from a placement's initial, such as 來.

## `public const string LDiweiRime = "rime";`

The kind of a category made from a placement's rime, division and 開合, such as `寒W I`.

## `public const string LDiweiTone = "tone";`

The kind of a category made from the tone class the hypothesis gives a placement, such as `6`.

## `private const string LDiweiRounded = "W";`

The mark ending a rime key of a 合口 placement, so 開 and 合 name two rows.

## `private static readonly string[] LDiweiDivisions = ["一", "二", "三", "四"];`

The division words the rime books print, in the order the Roman numerals stand for.

## `private static readonly string[] LDiweiRomans = ["I", "II", "III", "IV"];`

The Roman numeral each division becomes inside a rime key.

## `public static string LDiweiChongniuRead(string rime)`

The 重紐 letter the source hung on a rime, such as the `A` of 寒A, or empty.
The letter tells the two halves of a 重紐 pair apart, and `X` marks a row outside the pair.

## `public static string LDiweiRimeNormalize(string rime)`

The rime as a category keys it: the trailing 重紐 letter dropped, so 寒A and 寒 name one row.

## `public static string LDiweiRimeFormat(string rime, string division, bool rounded)`

The key of a rime category: rime without 重紐 letter, `W` when 合口, then division as Roman numeral.
So 模 一 開 keys `模 I` and 寒 一 合 keys `寒W I`.
The 合口 mark rides on the rime itself, since it names the rime rather than the division.
Every division and 開合 gets its own row.
A division outside 一 to 四 stays as printed.
Empty with an empty rime.

## `public static string LDiweiDivisionFormat(string division)`

The Roman numeral of a division 一 to 四, or the division as printed.
The rime key and the division label both print it, so it is written once.

## `public static int LDiweiRankRead(string kind, string heading, LHypothesis? hypothesis)`

The sorting rank of a category page section: its table or pack position, then unknown ones, then blank.
An initial's page sections by division, ranked in table order.
A rime's page sections by place, ranked as the hypothesis lists the places.

## `public static int LDiweiRankNormalize(int rank)`

An unlisted rank turned into the last position, so it sorts after every listed one.
