# Markdown

Markdown holds the note dialect, the one reader every writer and the engine share.
It is a small CommonMark subset, so a note written here reads the same in any Markdown viewer.
Blocks are headings, paragraphs, bullet and numbered items, quotes, fenced code and rules.
Inline shapes are those of `LMarkdownInline`.

## `LMarkdown`

`LMarkdownNormalize` brings note text to the form the archive stores.
Line ends become `\n`, trailing spaces leave every line, and blank lines leave both ends.
Whitespace-only text becomes empty, which the engine reads as no note at all.
`LMarkdownParse` reads text into flat blocks in reading order.
A blank line ends a paragraph, and a line break inside one is kept, since the editor shows it.
An item's continuation is any indented line that opens no other block.
A fence never closed runs to the end, and code between fences is gathered verbatim.
Closing hashes of a heading are dropped, and a heading takes one to six hashes and a space.
A rule is three or more of one rule character, spaces allowed between.
Item depth is the leading indent in steps of two spaces.

## `LMarkdownBlock`

One block of a parsed note, in reading order.
Blocks are flat, so a nested list is a run of items whose level grows.
`LMarkdownBlockMark` is the lead a list item shows.
It is a bullet or its ordinal with a dot, and empty for every other kind.
The kind verdicts beside it let a view pick an element without reading the kind itself.

## `LMarkdownInline`

Turns the text of one block into styled spans.
It knows code, bold, italic, links and backslash escapes, and nothing else.
`LMarkdownInlineParse` scans once and emits a span every time the style changes.
A marker with no closing partner ahead is kept as literal text, so a stray asterisk never vanishes.
A single underscore between two letters is literal, so a snake_case word keeps its shape.
A code span holds no other style, and its text is taken verbatim.
A link keeps the style around it and carries its target trimmed.

## `LMarkdownKind`

The block shapes the note dialect knows.
A paragraph, heading, bullet item, numbered item and quote carry spans.
A code block carries its text verbatim and a rule carries nothing.

## `LMarkdownSpan`

One stretch of text inside a block that shares a single styling.
A writer emits it as one run, so no writer re-reads the markers.
`LMarkdownSpanAddress` is the link target as an absolute http or https address, else null.
A note link therefore never opens a local file or another scheme.

## Stamp

Commit: `c2f335c331fe`
- `LMarkdown.cs` `bb31d2bb95bbf5db`
- `LMarkdownBlock.cs` `e7255d50d2b302f8`
- `LMarkdownInline.cs` `338516db3f85fad1`
- `LMarkdownKind.cs` `4ff637425b26ccd5`
- `LMarkdownSpan.cs` `50cf65aaa5ddf94a`
