# LMentionPiece.cs

## `public sealed record LMentionPiece(`

One stretch of a sentence as it is drawn: a Mention or the gap between two.
The pieces of one sentence cover its text end to end, and none of them is empty.
The control that draws the sentence keeps one text run per piece.
So a click lands on a run that already knows where it stands.

**Parameters**

- `LMentionPieceOffset` — The code-point offset the piece begins at, counted from zero.
- `LMentionPieceLength` — The number of code points the piece covers, always above zero.
- `LMentionPieceStored` — The Mention the piece stands for, or null for a gap.
