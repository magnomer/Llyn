using System;
using System.Collections.Generic;
using System.Windows;
using Llyn.Core;

namespace Llyn.UIVeneer;

public partial class PDisplay
{
    private void PDisplayIncomingShow(long id)
    {
        _pDisplayIncoming.Clear();

        IReadOnlyList<LUsage> incoming;
        try
        {
            incoming = _lLectern.LLecternIncomingRead(id);
        }
        catch (Exception)
        {
            incoming = [];
        }

        string unknown = PLocalizationCatalog.PLocalizationTextRead("Display.Unknown");
        string meaning = PLocalizationCatalog.PLocalizationTextRead("Display.MeaningSingle");
        string collocation = PLocalizationCatalog.PLocalizationTextRead("Display.CollocationSingle");

        foreach (LUsage usage in incoming)
        {
            _pDisplayIncoming.Add(new PUsageItem(
                usage,
                usage.LUsageCollocated ? collocation : meaning,
                unknown,
                string.Empty,
                _lLectern.LLecternEpithetRead(usage.LUsageEntry)));
        }

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
