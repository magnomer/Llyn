# CMarkdown.cs

## `public sealed class CMarkdown`

The markdown gate: a note's text as blocks and spans a driver draws without parsing.
It stands on the atelier's entry port, and `CAtelier` builds the one instance every driver shares.

## `internal CMarkdown(CAtelier atelier)`

Only the atelier builds it, so each session has one.

## `public IReadOnlyList<CMarkdownBlock> CMarkdownParse(string? text)`

The engine's blocks for the text, in order, each mapped once.

## `internal static CMarkdownBlock CMarkdownRead(LMarkdownBlock block)`

Carries one engine block and its spans with every verdict unchanged.
It stays internal, since it names engine types.
