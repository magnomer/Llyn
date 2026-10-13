using System;
using System.Collections.Generic;
using System.Linq;
using Llyn.Core;

namespace Llyn.Conduct;

public sealed record CParadigmForm(
    IReadOnlyList<CParadigmPiece> CParadigmFormPiece,
    string? CParadigmFormTip)
{
    internal static CParadigmForm CParadigmFormCreate(LParadigmForm form)
    {
        ArgumentNullException.ThrowIfNull(form);

        return new CParadigmForm(
            form.LParadigmFormPiece.Select(CParadigmPiece.CParadigmPieceCreate).ToArray(),
            form.LParadigmFormTip);
    }
}
