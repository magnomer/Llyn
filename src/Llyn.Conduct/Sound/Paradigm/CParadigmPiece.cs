using System;
using Llyn.Core;

namespace Llyn.Conduct;

public sealed record CParadigmPiece(string CParadigmPieceText, bool CParadigmPieceMarked, bool CParadigmPieceCut)
{
    internal static CParadigmPiece CParadigmPieceCreate(LParadigmPiece piece)
    {
        ArgumentNullException.ThrowIfNull(piece);

        return new CParadigmPiece(piece.LParadigmPieceText, piece.LParadigmPieceMarked, piece.LParadigmPieceCut);
    }
}
