using System;
using System.Collections.Generic;
using System.Linq;

namespace Llyn.Conduct;

internal sealed record CSGalleyWindow(
    int CSGalleyWindowTurn,
    int CSGalleyWindowCount,
    int CSGalleyWindowLow,
    int CSGalleyWindowHigh,
    int? CSGalleyWindowResolution,
    IReadOnlySet<CSGalleyLeaf> CSGalleyWindowHeld)
{
    internal CSGalleyWindow LSGalleyWindowMove(int first, int last, int? resolution)
    {
        int low = Math.Max(first - 1, 0);
        int high = Math.Min(last + 1, CSGalleyWindowCount - 1);
        return this with
        {
            CSGalleyWindowLow = low,
            CSGalleyWindowHigh = high,
            CSGalleyWindowResolution = resolution,
            CSGalleyWindowHeld = new HashSet<CSGalleyLeaf>(CSGalleyWindowHeld.Where(
                leaf => leaf.CSGalleyLeafPage >= low && leaf.CSGalleyLeafPage <= high)),
        };
    }

    internal bool LSGalleyWindowCheck(CSGalleyLeaf leaf)
    {
        return leaf.CSGalleyLeafPage >= CSGalleyWindowLow
            && leaf.CSGalleyLeafPage <= CSGalleyWindowHigh
            && leaf.CSGalleyLeafResolution == CSGalleyWindowResolution
            && !CSGalleyWindowHeld.Contains(leaf);
    }

    internal CSGalleyWindow LSGalleyWindowAdd(CSGalleyLeaf leaf)
    {
        return this with
        {
            CSGalleyWindowHeld = new HashSet<CSGalleyLeaf>(
                CSGalleyWindowHeld.Where(held => held.CSGalleyLeafPage != leaf.CSGalleyLeafPage))
            {
                leaf,
            },
        };
    }
}
