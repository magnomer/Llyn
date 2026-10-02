# CMentionPiece.cs

## `public sealed record CMentionPiece(`

One run of a mention text, cut by the engine at the Mentions.

**Parameters**

- `CMentionPieceOffset`: where the run starts in the text.
- `CMentionPieceText`: the run's text.
- `CMentionPieceLinked`: whether the run's Mention links an entry, null for plain text.
