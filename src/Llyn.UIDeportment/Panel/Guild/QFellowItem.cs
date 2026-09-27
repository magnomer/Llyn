using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QFellowItem
{
    internal QFellowItem(CFellow fellow)
    {
        QFellowItemId = fellow.CFellowId;
        QFellowItemName = fellow.CFellowName;
        QFellowItemCount = fellow.CFellowShared.ToString(CultureInfo.CurrentCulture);
    }

    internal static IReadOnlyList<QFellowItem> QFellowItemBuild(IReadOnlyList<CFellow> fellows)
    {
        ArgumentNullException.ThrowIfNull(fellows);

        List<QFellowItem> built = new(fellows.Count);
        foreach (CFellow fellow in fellows)
        {
            built.Add(new QFellowItem(fellow));
        }

        return built;
    }

    internal static void QFellowItemApply(FrameworkElement container, object item, string? _)
    {
        if (item is not QFellowItem fellow)
        {
            return;
        }

        if (QLook.QLookPartFind<QIconImage>(container, "PFellowIcon") is QIconImage icon)
        {
            icon.QIconSource = QIcon.QIconResolve("guild", 16);
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PFellowName") is TextBlock name)
        {
            name.Text = fellow.QFellowItemName;
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PFellowCount") is TextBlock count)
        {
            count.Text = fellow.QFellowItemCount;
        }
    }

    public long QFellowItemId { get; }

    public string QFellowItemName { get; }

    public string QFellowItemCount { get; }
}
