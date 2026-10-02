# QMarkdownFace.cs

## `public static class QMarkdownFace`

Draws a Markdown note as WPF elements, one per ready block Conduct hands over.
It holds no rule about the dialect, only how each block looks on screen.
Colors come from the theme brushes, so a theme change recolors a drawn note.

## `public static void QMarkdownRefine(Panel target, IReadOnlyList<CMarkdownBlock> blocks, CAtelier atelier)`

Empties `target` and fills it with the ready blocks in reading order.
The panel hears every followed link through one routed handler, added once however often it refreshes.
A new refresh replaces the atelier the panel remembers in `QMarkdownAtelier`.
Conduct parsed them, and the atelier opens a followed link, so the helper names no parser and no shell.
A list item is a grid with its bullet or number in a fixed lead column.
It is indented one step per level.
Numbers count up while consecutive items stay on one level and restart otherwise.
The gap above a block depends on what it is, so list items sit close and headings stand off.

## `public static void QMarkdownRefine(Panel target, string text)`

Empties `target` and fills it with one plain paragraph in the note's text face.
A driver paints a looked-up key this way, since a key's wording carries no Markdown.

## `private static FrameworkElement QMarkdownBlockBuild(CMarkdownBlock block)`

The element for one block, picked by the block's own verdicts, with plain text for a paragraph.

## `private static double QMarkdownGapRead(CMarkdownBlock block)`

The space above a block of that kind.

## `private static double QMarkdownIndentRead(int level)`

The left indent of a list item at nesting `level`, one step per level.

## `private static TextBlock QMarkdownPlainBuild(double size)`

An empty wrapping text block at `size`, with the line height every note paragraph shares.

## `private static TextBlock QMarkdownTextBuild(IReadOnlyList<CMarkdownSpan> spans, double size)`

A wrapping text block holding one inline per span.

## `private static Inline QMarkdownSpanBuild(CMarkdownSpan span)`

One run carrying the span's weight, style and monospace face.
Only an absolute http or https target becomes a hyperlink, so a note cannot launch anything else.
A hyperlink carries only its address and opens nothing itself.

## `private static void QMarkdownLinkObserve(object sender, RequestNavigateEventArgs e)`

Hears a followed link routed up to the panel and hands its address to the atelier's open gate.
The helper starts no process itself.

## `private static TextBlock QMarkdownHeadingBuild(CMarkdownBlock block)`

A heading is a slightly larger semibold text block, whatever its level.

## `private static Grid QMarkdownItemBuild(CMarkdownBlock block)`

One list row, the block's mark in the lead column and the item text beside it.

## `private static Border QMarkdownQuoteBuild(CMarkdownBlock block)`

Muted text behind a left rule.

## `private static Border QMarkdownCodeBuild(CMarkdownBlock block)`

Verbatim monospace text on a soft rounded ground.

## `private static Border QMarkdownRuleBuild()`

A one-pixel line in the theme's line color.

## `private static readonly ConditionalWeakTable<Panel, CAtelier> QMarkdownAtelier`

The atelier each refreshed panel opens followed links with, dropped when the panel is.

## `private static Brush QMarkdownBrushRead(string key)`

The theme brush under `key`, or gray when the theme has none.
