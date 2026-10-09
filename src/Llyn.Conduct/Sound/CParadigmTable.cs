using System;
using System.Collections.Generic;
using System.Linq;
using Llyn.Core;

namespace Llyn.Conduct;

public sealed record CParadigmTable(
    IReadOnlyList<string> CParadigmTableHeaders,
    IReadOnlyList<CParadigmLine> CParadigmTableLines)
{
    internal static CParadigmTable CParadigmTableCreate(LParadigmTable table, bool held)
    {
        ArgumentNullException.ThrowIfNull(table);

        return new CParadigmTable(
            table.LParadigmTableHeaders,
            table.LParadigmTableLines.Select(line => CParadigmLine.CParadigmLineCreate(line, held)).ToList());
    }
}
