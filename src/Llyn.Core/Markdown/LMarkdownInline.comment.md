# LMarkdownInline.cs

## `public static class LMarkdownInline`

Turns the text of one block into styled spans.
It knows code, bold, italic, links and backslash escapes, and nothing else.

## `public static IReadOnlyList<LMarkdownSpan> LMarkdownInlineParse(string? text)`

Scans `text` once and emits a span every time the style changes.
A marker with no closing partner ahead is kept as literal text, so a stray asterisk never vanishes.
A single underscore between two letters is literal, so a snake_case word keeps its shape.
A code span holds no other style, and its text is taken verbatim.
A link keeps the style around it and carries its target trimmed.

## `private static bool LMarkdownInlineFind(string text, string marker, int from)`

Reports whether `marker` closes somewhere after `from` with at least one letter between.

## `private static bool LMarkdownInlineCheck(string text, int place, int width)`

Reports whether an underscore marker at `place` sits on a word boundary.

## `private static void LMarkdownInlineAdd(List<LMarkdownSpan> spans, StringBuilder buffer, bool bold, bool italic)`

Flushes the gathered text as one span under the current style, or nothing when the buffer is empty.
