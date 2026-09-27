# CMarkdownSpan.cs

## `public sealed record CMarkdownSpan(`

One styled run of a markdown block.

**Parameters**

- `CMarkdownSpanText`: the run's text.
- `CMarkdownSpanBold`: whether the run is bold.
- `CMarkdownSpanItalic`: whether the run is italic.
- `CMarkdownSpanCode`: whether the run is inline code.
- `CMarkdownSpanAddress`: the link the run opens, null when none.
