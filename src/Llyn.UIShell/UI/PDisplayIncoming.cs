using System;
using System.Collections.Generic;
using System.Windows;
using Llyn.Core;

namespace Llyn.UIShell;

public partial class PDisplay
{
    private void PDisplayIncomingShow(long id)
    {
        _pDisplayIncoming.Clear();

        IReadOnlyList<LUsage> incoming;
        try
        {
            incoming = _lEngine.LEngineIncomingRead(id);
        }
        catch (Exception)
        {
            incoming = [];
        }

        string unknown = _pDisplayHost.PLocalizationTextRead("Display.Unknown");
        string meaning = _pDisplayHost.PLocalizationTextRead("Display.MeaningSingle");
        string collocation = _pDisplayHost.PLocalizationTextRead("Display.CollocationSingle");

        foreach (LUsage usage in incoming)
        {
            _pDisplayIncoming.Add(new PUsageItem(
                usage,
                usage.LUsageOwner == LOwner.LOwnerCollocation ? collocation : meaning,
                unknown,
                string.Empty));
        }

        PTwin.PTwinNameApply(
            _pDisplayIncoming, row => row.PUsageItemHeadword, (row, name) => row.PUsageItemName = name);

        PDisplayIncomingSection.Visibility = _pDisplayIncoming.Count == 0
            ? Visibility.Collapsed
            : Visibility.Visible;
    }

    private void PDisplayIncomingHandle(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement row && row.DataContext is PUsageItem item)
        {
            _pDisplayHost.PWindowEntryShow(item.PUsageItemEntry);
        }
    }
}
