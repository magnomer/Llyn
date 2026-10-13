# Markdown

This folder holds the ready markdown shapes a driver draws.
It stands apart from the engine's markdown, because a driver receives blocks and never parses.

## `CMarkdown`

The one map from a text the engine parses to the blocks and spans a driver draws.
Each area parses the text it already holds.
The display calls it for the shown note, and the atlas for the vignette's description.
The engine's blocks come back in order, each mapped once.
Every verdict is carried unchanged.

## `CMarkdownBlock`

One block of parsed markdown.
Its verdicts are copied from the engine, so the face never judges a block's kind.
The level is the heading level or the list depth.
The mark is the bullet or number a list item shows.
The text is the raw text a code block shows.

## `CMarkdownSpan`

One styled run of a markdown block.
Its address is the link the run opens, null when none.

## Stamp

Commit: `c2f335c331fe`
- `CMarkdown.cs` `fbd372769caae9a1`
- `CMarkdownBlock.cs` `b47cef424818f689`
- `CMarkdownSpan.cs` `f846037c45e5a4e2`
