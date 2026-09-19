using System;
using System.Collections.Generic;
using System.Globalization;
using Llyn.Core;

namespace Llyn.UIVeneer;

internal sealed class PFellowItem
{
    internal PFellowItem(LFellow fellow)
    {
        PFellowItemId = fellow.LFellowId;
        PFellowItemName = fellow.LFellowName;
        PFellowItemCount = fellow.LFellowShared.ToString(CultureInfo.CurrentCulture);
    }

    internal static IReadOnlyList<PFellowItem> PFellowItemBuild(IReadOnlyList<LFellow> fellows)
    {
        ArgumentNullException.ThrowIfNull(fellows);

        List<PFellowItem> built = new(fellows.Count);
        foreach (LFellow fellow in fellows)
        {
            built.Add(new PFellowItem(fellow));
        }

        return built;
    }

    public long PFellowItemId { get; }

    public string PFellowItemName { get; }

    public string PFellowItemCount { get; }
}
