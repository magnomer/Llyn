using System;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

public sealed class QParadigmMark
{
    private QParadigmMark(int offset, int length)
    {
        QParadigmMarkOffset = offset;
        QParadigmMarkLength = length;
    }

    public int QParadigmMarkOffset { get; }

    public int QParadigmMarkLength { get; }

    internal static QParadigmMark QParadigmMarkCreate(CParadigmMark mark)
    {
        ArgumentNullException.ThrowIfNull(mark);

        return new QParadigmMark(mark.CParadigmMarkOffset, mark.CParadigmMarkLength);
    }
}
