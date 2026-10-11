using System;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

public sealed class QParadigmPiece
{
    private QParadigmPiece(string text, bool marked, bool cut)
    {
        QParadigmPieceText = text;
        QParadigmPieceMarked = marked;
        QParadigmPieceCut = cut;
    }

    public string QParadigmPieceText { get; }

    public bool QParadigmPieceMarked { get; }

    public bool QParadigmPieceCut { get; }

    internal static QParadigmPiece QParadigmPieceCreate(CParadigmPiece piece)
    {
        ArgumentNullException.ThrowIfNull(piece);

        return new QParadigmPiece(piece.CParadigmPieceText, piece.CParadigmPieceMarked, piece.CParadigmPieceCut);
    }
}
