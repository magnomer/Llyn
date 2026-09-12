# LMarkdownSpan.cs

## `public sealed record LMarkdownSpan(`

One stretch of text inside a block that shares a single styling.
A writer emits it as one run, so no writer re-reads the markers.

**Parameters**

- `LMarkdownSpanText` — The text with every marker and escape already removed.
- `LMarkdownSpanBold` — Whether the stretch sits inside `**` or `__`.
- `LMarkdownSpanItalic` — Whether the stretch sits inside `*` or `_`.
- `LMarkdownSpanCode` — Whether the stretch sits inside backticks, in which case it holds no other style.
- `LMarkdownSpanLink` — The link target when the stretch is a link, else empty.
