# LMarkdownPort.cs
Hash: `d966d5c1e2a4e15c`

## `public interface LMarkdownPort`

The slice of the engine a deportment sees when it shows a note written in markdown.
`LDraftFacade` implements it.

## `IReadOnlyList<LMarkdownBlock> LEngineMarkdownParse(string? text);`

The note divided into markdown blocks, none for a missing note.
