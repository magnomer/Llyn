# QLeafLine.cs

## `internal sealed record QLeafLine(string QLeafLineHead, long QLeafLineSentence, IReadOnlyList<QMentionPiece> QLeafLinePiece, string QLeafLineCitation, IReadOnlyList<PGloss> QLeafLineGloss)`

One example line of a reading card, as the line fill paints it.
It carries plain copies of the ready line, so the fill never holds a Conduct record.

**Parameters**

- `QLeafLineHead`: the frame printed before the sentence.
- `QLeafLineSentence`: the sentence row id, which a click hands back to the find gate.
- `QLeafLinePiece`: the sentence's runs, as the mention text draws them.
- `QLeafLineCitation`: the byline printed after the sentence.
- `QLeafLineGloss`: the Gloss rows, wrapped as the editor's rows.

## `internal static IReadOnlyList<QLeafLine> QLeafLineCreate(IReadOnlyList<CLeafLine> lines)`

The example lines of a card, one per ready line, in the order Conduct gave them.
The Glosses are wrapped through [PGlossConverter](../../Editor/Gloss/PGlossConverter.comment.md), so both modes share one flag resolver.
It only copies, so the driver neither cuts nor judges a line.
