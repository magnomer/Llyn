# PMarkdown.cs

## `internal static class PMarkdown`

Draws a Markdown note as WPF elements, one per block `LMarkdown` reads.
It holds no rule about the dialect, only how each block looks on screen.
Colors come from the theme brushes, so a theme change recolors a drawn note.

## `internal static void PMarkdownShow(Panel target, string? markdown)`

Empties `target` and fills it with the note's blocks in reading order.
A list item is a grid with its bullet or number in a fixed lead column.
It is indented one step per level.
Numbers count up while consecutive items stay on one level and restart otherwise.
The gap above a block depends on what it is, so list items sit close and headings stand off.

## `private static double PMarkdownGapRead(LMarkdownBlock block)`

The space above a block of that kind.

## `private static TextBlock PMarkdownTextBuild(IReadOnlyList<LMarkdownSpan> spans, double size)`

A wrapping text block holding one inline per span.

## `private static Inline PMarkdownSpanBuild(LMarkdownSpan span)`

One run carrying the span's weight, style and monospace face.
Only an absolute http or https target becomes a hyperlink, so a note cannot launch anything else.

## `private static void PMarkdownLinkHandle(object sender, RequestNavigateEventArgs e)`

Opens the link in the system browser.

## `private static TextBlock PMarkdownHeadingBuild(LMarkdownBlock block)`

A heading is a slightly larger semibold text block, whatever its level.

## `private static Grid PMarkdownItemBuild(LMarkdownBlock block, string mark)`

One list row, `mark` in the lead column and the item text beside it.

## `private static Border PMarkdownQuoteBuild(LMarkdownBlock block)`

Muted text behind a left rule.

## `private static Border PMarkdownCodeBuild(LMarkdownBlock block)`

Verbatim monospace text on a soft rounded ground.

## `private static Border PMarkdownRuleBuild()`

A one-pixel line in the theme's line color.

## `private static Brush PMarkdownBrushRead(string key)`

The theme brush under `key`, or gray when the theme has none.
