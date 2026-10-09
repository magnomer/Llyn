using System;
using System.Collections.Generic;
using System.Linq;
using Llyn.Core;

namespace Llyn.Conduct;

public sealed record CParadigmLine(
    string CParadigmLineGroup,
    string CParadigmLineLabel,
    IReadOnlyList<CParadigmForm> CParadigmLineForms)
{
    internal static CParadigmLine CParadigmLineCreate(LParadigmLine line, bool held)
    {
        ArgumentNullException.ThrowIfNull(line);

        return new CParadigmLine(
            line.LParadigmLineGroup,
            line.LParadigmLineLabel,
            line.LParadigmLineForms.Select(form => CParadigmForm.CParadigmFormCreate(form, held)).ToList());
    }
}
