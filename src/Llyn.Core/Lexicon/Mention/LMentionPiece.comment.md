# LMentionPiece.cs
Hash: `666600a76c09993f`

## `public sealed record LMentionPiece(LMention? LMentionPieceStored, string LMentionPieceText)`

One stretch of a sentence as it is drawn.
It is a Mention or the gap between two.
The pieces of one sentence cover its text end to end, and none of them is empty.
The control that draws the sentence keeps one text run per piece.

**Parameters**

- `LMentionPieceStored` — The Mention the piece stands for, or null for a gap.
- `LMentionPieceText` — The characters the piece covers, cut from the sentence in UTF-16 units.
  The control draws it as is, so it never converts a code-point span itself.
