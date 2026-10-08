# TInterfaceMarkdown.cs
Hash: `e3d48d9e8ac783fa`

## `internal static class TInterfaceMarkdown`

The relays for the one Conduct map from a text to ready Markdown blocks.
Each relay is transparent and carries no test logic of its own.

## `internal static LMarkdownPort TMarkdownPortCreate() =>`

A fake Markdown port whose parse runs the real engine parser, so a map sees real blocks.

## `internal static IReadOnlyList<CMarkdownBlock> TMarkdownBlockRead(string? text) =>`

Relays the text through the Conduct map over that port.
