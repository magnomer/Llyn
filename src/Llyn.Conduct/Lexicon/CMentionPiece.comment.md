# CMentionPiece.cs
Hash: `ad0aea6a01028a5e`

## `public sealed record CMentionPiece(int CMentionPieceOffset, string CMentionPieceText, bool? CMentionPieceLinked)`

One run of a mention text, cut by the engine at the Mentions.

**Parameters**

- `CMentionPieceOffset`: where the run starts in the text.
- `CMentionPieceText`: the run's text.
- `CMentionPieceLinked`: whether the run's Mention links an entry, null for plain text.
