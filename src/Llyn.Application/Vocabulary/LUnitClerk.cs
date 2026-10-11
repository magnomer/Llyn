using System.Collections.Generic;
using Llyn.Core;

namespace Llyn.Application;

public static class LUnitClerk
{
    private static readonly LUnit[] LUnitClerkOffered = [LUnit.LUnitMorpheme, LUnit.LUnitWord, LUnit.LUnitExpression];

    public static IReadOnlyList<LUnit> LUnitScan()
    {
        return LUnitClerkOffered;
    }

    public static string LUnitFormat(LUnit unit)
    {
        return LUnitKey.LUnitKeyRead(unit);
    }
}
