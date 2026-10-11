using System;

namespace Llyn.Core;

public static class LFreedomKey
{
    public static LFreedom LFreedomKeyParse(string? key)
    {
        return key?.Trim().ToLowerInvariant() switch
        {
            "dependent" => LFreedom.LFreedomDependent,
            "standalone" => LFreedom.LFreedomStandalone,
            _ => LFreedom.LFreedomEmpty,
        };
    }

    public static string LFreedomKeyRead(LFreedom freedom)
    {
        return freedom switch
        {
            LFreedom.LFreedomEmpty => string.Empty,
            LFreedom.LFreedomDependent => "Freedom.Dependent",
            LFreedom.LFreedomStandalone => "Freedom.Standalone",
            _ => throw new ArgumentOutOfRangeException(nameof(freedom), freedom, null),
        };
    }

    public static string LFreedomKeyFormat(LFreedom freedom)
    {
        return freedom switch
        {
            LFreedom.LFreedomEmpty => string.Empty,
            LFreedom.LFreedomDependent => "dependent",
            LFreedom.LFreedomStandalone => "standalone",
            _ => throw new ArgumentOutOfRangeException(nameof(freedom), freedom, null),
        };
    }
}
