# LMarkdownFace.cs

## `public static class LMarkdownFace`

Draws a Markdown note as WPF elements, one per block `LMarkdown` reads.
It holds no rule about the dialect, only how each block looks on screen.
Colors come from the theme brushes, so a theme change recolors a drawn note.

## `public static void LMarkdownShow(Panel target, string? markdown, LWindow window)`

Empties `target` and fills it with the note's blocks in reading order.
The window deportment parses the text and opens a link, so the helper names no parser and no shell.
A list item is a grid with its bullet or number in a fixed lead column.
It is indented one step per level.
Numbers count up while consecutive items stay on one level and restart otherwise.
The gap above a block depends on what it is, so list items sit close and headings stand off.

## `private static FrameworkElement LMarkdownBlockBuild(LMarkdownBlock block, LWindow window)`

The element for one block, picked by the block's own verdicts, with plain text for a paragraph.

## `private static double LMarkdownGapRead(LMarkdownBlock block)`

The space above a block of that kind.

## `private static double LMarkdownIndentRead(int level)`

The left indent of a list item at nesting `level`, one step per level.

## `private static TextBlock LMarkdownTextBuild(IReadOnlyList<LMarkdownSpan> spans, double size, LWindow window)`

A wrapping text block holding one inline per span.

## `private static Inline LMarkdownSpanBuild(LMarkdownSpan span, LWindow window)`

One run carrying the span's weight, style and monospace face.
Only an absolute http or https target becomes a hyperlink, so a note cannot launch anything else.
A followed link opens through `window`, so the helper starts no process itself.

## `private static TextBlock LMarkdownHeadingBuild(LMarkdownBlock block, LWindow window)`

A heading is a slightly larger semibold text block, whatever its level.

## `private static Grid LMarkdownItemBuild(LMarkdownBlock block, LWindow window)`

One list row, the block's mark in the lead column and the item text beside it.

## `private static Border LMarkdownQuoteBuild(LMarkdownBlock block, LWindow window)`

Muted text behind a left rule.

## `private static Border LMarkdownCodeBuild(LMarkdownBlock block)`

Verbatim monospace text on a soft rounded ground.

## `private static Border LMarkdownRuleBuild()`

A one-pixel line in the theme's line color.

## `private static Brush LMarkdownBrushRead(string key)`

The theme brush under `key`, or gray when the theme has none.
