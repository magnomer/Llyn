using System;
using System.Collections.Generic;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

public sealed class QParadigmTable
{
    private QParadigmTable(IReadOnlyList<string> headers, IReadOnlyList<QParadigmLine> lines)
    {
        QParadigmTableHeaders = headers;
        QParadigmTableLines = lines;
    }

    public IReadOnlyList<string> QParadigmTableHeaders { get; }

    public IReadOnlyList<QParadigmLine> QParadigmTableLines { get; }

    internal static QParadigmTable QParadigmTableCreate(CParadigmTable table)
    {
        ArgumentNullException.ThrowIfNull(table);

        List<QParadigmLine> lines = new(table.CParadigmTableLines.Count);
        foreach (CParadigmLine line in table.CParadigmTableLines)
        {
            lines.Add(QParadigmLine.QParadigmLineCreate(line));
        }

        return new QParadigmTable(table.CParadigmTableHeaders, lines);
    }
}
