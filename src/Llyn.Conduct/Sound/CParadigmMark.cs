using System;
using Llyn.Core;

namespace Llyn.Conduct;

public sealed record CParadigmMark(int CParadigmMarkOffset, int CParadigmMarkLength)
{
    internal static CParadigmMark CParadigmMarkCreate(LInflectionMark mark)
    {
        ArgumentNullException.ThrowIfNull(mark);

        return new CParadigmMark(mark.LInflectionMarkOffset, mark.LInflectionMarkLength);
    }
}
