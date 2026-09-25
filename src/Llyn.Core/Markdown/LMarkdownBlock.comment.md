# LMarkdownBlock.cs

## `public sealed record LMarkdownBlock(`

One block of a parsed note, in reading order.
Blocks are flat: a nested list is a run of items whose level grows.

**Parameters**

- `LMarkdownBlockKind` — The block shape.
- `LMarkdownBlockLevel` — A heading's depth from 1 to 6, or a list item's nesting depth from 0.
- `LMarkdownBlockSpan` — The styled stretches of a text block, empty for code and rule.
- `LMarkdownBlockText` — The verbatim body of a code block, empty for every other kind.

## `public string LMarkdownBlockMark => LMarkdownBlockKind switch`

The lead a list item shows, a bullet or its ordinal with a dot, and empty for every other kind.
The kind verdicts beside it let a view pick an element without reading the kind itself.
