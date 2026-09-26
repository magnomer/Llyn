using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using Llyn.Core;

namespace Llyn.UIDeportment;

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

    internal static void PFellowItemApply(FrameworkElement container, object item, string? _)
    {
        if (item is not PFellowItem fellow)
        {
            return;
        }

        if (PLook.PLookPartFind<PIconImage>(container, "PFellowIcon") is PIconImage icon)
        {
            icon.PIconSource = PIcon.PIconResolve("guild", 16);
        }

        if (PLook.PLookPartFind<TextBlock>(container, "PFellowName") is TextBlock name)
        {
            name.Text = fellow.PFellowItemName;
        }

        if (PLook.PLookPartFind<TextBlock>(container, "PFellowCount") is TextBlock count)
        {
            count.Text = fellow.PFellowItemCount;
        }
    }

    public long PFellowItemId { get; }

    public string PFellowItemName { get; }

    public string PFellowItemCount { get; }
}
