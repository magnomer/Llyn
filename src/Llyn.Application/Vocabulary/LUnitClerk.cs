using System.Collections.Generic;
using Llyn.Core;

namespace Llyn.Application;

public static class LUnitClerk
{
    private static readonly LUnit[] LUnitClerkSpaced = [LUnit.LUnitContent, LUnit.LUnitFunction, LUnit.LUnitMorpheme];

    private static readonly LUnit[] LUnitClerkUnspaced = [LUnit.LUnitWord, LUnit.LUnitMorpheme];

    public static IReadOnlyList<LUnit> LUnitScan(bool spaced)
    {
        return spaced ? LUnitClerkSpaced : LUnitClerkUnspaced;
    }

    public static string LUnitFormat(LUnit unit)
    {
        return LUnitKey.LUnitKeyRead(unit);
    }

    public static LUnit LUnitSettle(LUnit unit, bool spaced)
    {
        return unit switch
        {
            LUnit.LUnitContent or LUnit.LUnitFunction when !spaced => LUnit.LUnitWord,
            LUnit.LUnitWord when spaced => LUnit.LUnitEmpty,
            _ => unit,
        };
    }
}
