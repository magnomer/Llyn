# CMarkdown.cs

## `internal static class CMarkdown`

The one map from a text the engine parses to the blocks and spans a driver draws.
Each area parses the text it already holds, so a driver receives ready blocks and never parses.

## `internal static IReadOnlyList<CMarkdownBlock> LMarkdownParse(LEntryPort entries, string? text)`

The engine's blocks for the text, in order, each mapped once.
The display calls it for the shown note, and the atlas for the vignette's description.

## `private static CMarkdownBlock LMarkdownRead(LMarkdownBlock block)`

Carries one engine block and its spans with every verdict unchanged.
