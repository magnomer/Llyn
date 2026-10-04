# CMentionPiece.cs
Hash: `6094ea24cbc39f8d`

## `public sealed record CMentionPiece(string CMentionPieceText, bool? CMentionPieceLinked)`

One run of a mention text, cut by the engine at the Mentions.

**Parameters**

- `CMentionPieceText`: the run's text.
- `CMentionPieceLinked`: whether the run's Mention links an entry, null for plain text.
