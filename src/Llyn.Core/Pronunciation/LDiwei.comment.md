# LDiwei.cs

## `public sealed record LDiwei(`

One 音韻地位 category of a language: an initial, a rime or a tone class that placements point to.
It is a shared node, like a Tag, but derived from stored fanqie rows rather than typed by the user.
Every character whose placement carries the part links to the same row, so a category can be browsed.

**Parameters**

- `LDiweiId` — The row's own key.
- `LDiweiLanguage` — The language pack the placements belong to.
- `LDiweiKind` — Which part: `initial`, `rime` or `tone`.
- `LDiweiKey` — The part as the rime books or the hypothesis name it: 來, 寒 or the class `6`.
- `LDiweiCount` — How many entries of the language carry a character placed in this category.

## `public const string LDiweiInitial = "initial";`

The kind of a category made from a placement's initial, such as 來.

## `public const string LDiweiRime = "rime";`

The kind of a category made from a placement's rime, such as 寒, its 重紐 letter dropped.

## `public const string LDiweiTone = "tone";`

The kind of a category made from the tone class the hypothesis gives a placement, such as `6`.

## `public static string LDiweiRimeNormalize(string rime)`

The rime as a category keys it: the trailing 重紐 letter dropped, so 寒A and 寒 name one row.
