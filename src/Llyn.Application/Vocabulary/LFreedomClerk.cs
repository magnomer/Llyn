using System.Collections.Generic;
using Llyn.Core;

namespace Llyn.Application;

public static class LFreedomClerk
{
    private static readonly LFreedom[] LFreedomClerkOffered = [LFreedom.LFreedomDependent, LFreedom.LFreedomStandalone];

    public static IReadOnlyList<LFreedom> LFreedomScan()
    {
        return LFreedomClerkOffered;
    }

    public static string LFreedomFormat(LFreedom freedom)
    {
        return LFreedomKey.LFreedomKeyRead(freedom);
    }
}
