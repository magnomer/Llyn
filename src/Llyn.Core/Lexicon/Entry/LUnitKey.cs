using System;

namespace Llyn.Core;

public static class LUnitKey
{
    public static LUnit LUnitKeyParse(string? key)
    {
        return key?.Trim().ToLowerInvariant() switch
        {
            "morpheme" => LUnit.LUnitMorpheme,
            "word" => LUnit.LUnitWord,
            "expression" => LUnit.LUnitExpression,
            _ => LUnit.LUnitEmpty,
        };
    }

    public static string LUnitKeyRead(LUnit unit)
    {
        return unit switch
        {
            LUnit.LUnitEmpty => string.Empty,
            LUnit.LUnitMorpheme => "Unit.Morpheme",
            LUnit.LUnitWord => "Unit.Word",
            LUnit.LUnitExpression => "Unit.Expression",
            _ => throw new ArgumentOutOfRangeException(nameof(unit), unit, null),
        };
    }

    public static string LUnitKeyFormat(LUnit unit)
    {
        return unit switch
        {
            LUnit.LUnitEmpty => string.Empty,
            LUnit.LUnitMorpheme => "morpheme",
            LUnit.LUnitWord => "word",
            LUnit.LUnitExpression => "expression",
            _ => throw new ArgumentOutOfRangeException(nameof(unit), unit, null),
        };
    }
}
