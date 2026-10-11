# LPressRange.cs
Hash: `b7584b3fe0561822`

## `public sealed record LPressRange(string LPressRangeText)`

The pages to print, kept as the text the browser engine reads, such as `1-3, 5, 8-`.
Empty text means every page.
The parser rewrites typed text into one spelling before a range holds it.

**Parameters**

- `LPressRangeText`: the normalised range text, empty for every page.

## `public static LPressRange? LPressRangeParse(string text, int count)`

Reads typed page text against a document of `count` pages, and answers null when the text is invalid.
Pages count from one, as the reader sees them.
A page past `count` or a span running backwards makes the whole text invalid.
An open end such as `8-` is kept open, so it still means the last page after the layout changes.
The answer is rewritten in one spelling, so equal choices always compare equal.

## `public bool LPressRangeCheck(int page)`

Whether the page at index `page`, counted from zero, falls inside the range.
The index counts from zero because a proof numbers its pages that way.
The view dims a page outside the range rather than hiding it, so page numbers stay put.
Malformed text, which only a hand-built range can hold, includes no page.

## `private static IReadOnlyList<(int LPressRangeFirst, int LPressRangeLast)>? LPressRangeRead(string text)`

Splits range text into spans of page numbers, or null when any piece is malformed.
An open end reads as the largest number, so one comparison covers open and closed spans.
Blank text answers no spans, which both callers treat as every page.
Each span names its first and last page number.
