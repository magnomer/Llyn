using System;

namespace Llyn.Core;

public static class LUnitKey
{
    public static LUnit LUnitKeyParse(string? key)
    {
        return key?.Trim().ToLowerInvariant() switch
        {
            "content" => LUnit.LUnitContent,
            "function" => LUnit.LUnitFunction,
            "morpheme" => LUnit.LUnitMorpheme,
            "word" => LUnit.LUnitWord,
            _ => LUnit.LUnitEmpty,
        };
    }

    public static string LUnitKeyRead(LUnit unit)
    {
        return unit switch
        {
            LUnit.LUnitEmpty => string.Empty,
            LUnit.LUnitContent => "Unit.Content",
            LUnit.LUnitFunction => "Unit.Function",
            LUnit.LUnitMorpheme => "Unit.Morpheme",
            LUnit.LUnitWord => "Unit.Word",
            _ => throw new ArgumentOutOfRangeException(nameof(unit), unit, null),
        };
    }

    public static string LUnitKeyFormat(LUnit unit)
    {
        return unit switch
        {
            LUnit.LUnitEmpty => string.Empty,
            LUnit.LUnitContent => "content",
            LUnit.LUnitFunction => "function",
            LUnit.LUnitMorpheme => "morpheme",
            LUnit.LUnitWord => "word",
            _ => throw new ArgumentOutOfRangeException(nameof(unit), unit, null),
        };
    }
}
