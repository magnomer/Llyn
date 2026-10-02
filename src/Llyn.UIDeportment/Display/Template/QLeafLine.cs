using System.Collections.Generic;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed record QLeafLine(
    string QLeafLineHead,
    long QLeafLineSentence,
    IReadOnlyList<QMentionPiece> QLeafLinePiece,
    string QLeafLineCitation,
    IReadOnlyList<PGloss> QLeafLineGloss)
{
    internal static IReadOnlyList<QLeafLine> QLeafLineCreate(IReadOnlyList<CLeafLine> lines)
    {
        List<QLeafLine> rows = new(lines.Count);
        foreach (CLeafLine line in lines)
        {
            rows.Add(new QLeafLine(
                line.CLeafLineHead,
                line.CLeafLineSentence,
                QMentionPiece.QMentionPieceCreate(line.CLeafLinePiece),
                line.CLeafLineCitation,
                PGlossConverter.PGlossConverterCreate(line.CLeafLineGloss)));
        }

        return rows;
    }
}
