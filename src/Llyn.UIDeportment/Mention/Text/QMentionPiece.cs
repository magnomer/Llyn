using System.Collections.Generic;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed record QMentionPiece(string QMentionPieceText, int QMentionPieceOffset, bool? QMentionPieceLinked)
{
    internal static IReadOnlyList<QMentionPiece> QMentionPieceCreate(IReadOnlyList<CMentionPiece> pieces)
    {
        List<QMentionPiece> rows = new(pieces.Count);
        foreach (CMentionPiece piece in pieces)
        {
            rows.Add(new QMentionPiece(piece.CMentionPieceText, piece.CMentionPieceOffset, piece.CMentionPieceLinked));
        }

        return rows;
    }

    internal static IReadOnlyList<QMentionPiece> QMentionPieceCreate(string text)
    {
        return [new QMentionPiece(text, 0, null)];
    }
}
