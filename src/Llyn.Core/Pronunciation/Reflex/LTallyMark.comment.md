# LTallyMark.cs

## `public sealed record LTallyMark(string LTallyMarkText, IReadOnlyList<string> LTallyMarkCharacters)`

One part and the characters whose readings take it, printed as the part with a raised count.
A character with two readings taking the same part is listed once, and one taking two parts under each.
The characters are kept so the view can open a list of them from the part.

**Parameters**

- `LTallyMarkText` — The part as cut, such as `ㄹ`, `l` or `an`.
- `LTallyMarkCharacters` — The distinct characters of the division taking it, in placement order.

## `public int LTallyMarkCount`

How many characters take the part, the printed number.

## `public static IReadOnlyList<LTallyMark> LTallyMarkScan(`

The marks of one language from its parts and the characters under each, count descending, then part text.
A missing or empty table gives no marks.
