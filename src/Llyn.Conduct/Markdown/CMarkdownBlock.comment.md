# CMarkdownBlock.cs
Hash: `b47cef424818f689`

## `public sealed record CMarkdownBlock(bool CMarkdownBlockHeaded, bool CMarkdownBlockListed, bool CMarkdownBlockQuoted, bool CMarkdownBlockFenced, bool CMarkdownBlockRuled, int CMarkdownBlockLevel, string CMarkdownBlockMark, string CMarkdownBlockText, IReadOnlyList<CMarkdownSpan> CMarkdownBlockSpan)`

One block of parsed markdown, as the markdown face builds it.
Its verdicts are copied from the engine, so the face never judges a block's kind.

**Parameters**

- `CMarkdownBlockHeaded`: whether the block is a heading.
- `CMarkdownBlockListed`: whether the block is a bullet or numbered item.
- `CMarkdownBlockQuoted`: whether the block is a quote.
- `CMarkdownBlockFenced`: whether the block is fenced code.
- `CMarkdownBlockRuled`: whether the block is a rule.
- `CMarkdownBlockLevel`: the heading level or the list depth.
- `CMarkdownBlockMark`: the bullet or number a list item shows.
- `CMarkdownBlockText`: the raw text a code block shows.
- `CMarkdownBlockSpan`: the styled runs of the block's text.
