using System;
using System.Collections.Generic;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

public sealed class QParadigmForm
{
    private QParadigmForm(IReadOnlyList<QParadigmPiece> piece, string? tip)
    {
        QParadigmFormPiece = piece;
        QParadigmFormTip = tip;
    }

    public IReadOnlyList<QParadigmPiece> QParadigmFormPiece { get; }

    public string? QParadigmFormTip { get; }

    internal static QParadigmForm QParadigmFormCreate(CParadigmForm form)
    {
        ArgumentNullException.ThrowIfNull(form);

        List<QParadigmPiece> pieces = new(form.CParadigmFormPiece.Count);
        foreach (CParadigmPiece piece in form.CParadigmFormPiece)
        {
            pieces.Add(QParadigmPiece.QParadigmPieceCreate(piece));
        }

        return new QParadigmForm(pieces, form.CParadigmFormTip);
    }
}
