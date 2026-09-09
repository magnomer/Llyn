using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using Llyn.Core;

namespace Llyn.UIShell;

public partial class PDisplay
{
    private const double PCompassEdge = 28;

    private const double PCompassLead = 14;

    private readonly ObservableCollection<PCompassItem> _pCompassRows = [];

    private bool _pCompassFold;

    private void PCompassAttach()
    {
        PCompassList.ItemsSource = _pCompassRows;
        PDisplayContents.ScrollChanged += PCompassScrollHandle;
    }

    private void PCompassUpdate()
    {
        Dispatcher.BeginInvoke(DispatcherPriority.Loaded, PCompassRowCreate);
    }

    private void PCompassClear()
    {
        _pCompassRows.Clear();
        PCompass.Visibility = Visibility.Collapsed;
    }

    private void PCompassRowCreate()
    {
        _pCompassRows.Clear();

        string unreadable = _pDisplayHost.PLocalizationTextRead("Display.Unreadable");

        PCompassSectionAdd(PDisplaySpeechSection, _pDisplayHost.PLocalizationTextRead("Speech.Title"));

        if (PCompassSectionAdd(PDisplayMeaningSection, _pDisplayHost.PLocalizationTextRead("Display.MeaningPlural")))
        {
            PCompassCardAdd(PDisplayMeaning, _pDisplayHost.PLocalizationTextRead("Display.MeaningSingle"), unreadable);
        }

        if (PCompassSectionAdd(PDisplayCollocationSection, _pDisplayHost.PLocalizationTextRead("Display.Collocation")))
        {
            PCompassCardAdd(PDisplayCollocation, _pDisplayHost.PLocalizationTextRead("Display.CollocationSingle"), unreadable);
        }

        PCompassSectionAdd(PDisplayIncomingSection, _pDisplayHost.PLocalizationTextRead("Display.Translated"));
        PCompassSectionAdd(PDisplayNoteSection, _pDisplayHost.PLocalizationTextRead("Display.Note"));

        PTwin.PTwinNameApply(
            _pCompassRows, row => row.PCompassItemLabel, (row, name) => row.PCompassItemName = name);

        PCompassPlace();
        PCompassSync();
    }

    private bool PCompassSectionAdd(FrameworkElement section, string label)
    {
        if (section.Visibility != Visibility.Visible)
        {
            return false;
        }

        _pCompassRows.Add(new PCompassItem(section, label, string.Empty, 0));
        return true;
    }

    private void PCompassCardAdd(ItemsControl cards, string kind, string unreadable)
    {
        for (int index = 0; index < cards.Items.Count; index++)
        {
            if (cards.ItemContainerGenerator.ContainerFromIndex(index) is not FrameworkElement container)
            {
                continue;
            }

            if (cards.Items[index] is not LCardDraft card)
            {
                continue;
            }

            string label = card.LCardDraftTitle.LStateValueState switch
            {
                LState.LStateSpecified => card.LCardDraftTitle.LStateValueShow(),
                LState.LStateUnknown => unreadable,
                _ => kind,
            };

            _pCompassRows.Add(new PCompassItem(
                container,
                label,
                card.LCardDraftPosition.ToString(CultureInfo.CurrentCulture),
                1));
        }
    }

    private void PCompassPlace()
    {
        bool room = PDisplayContents.Visibility == Visibility.Visible
            && PDisplayContents.ScrollableHeight > 0
            && _pCompassRows.Count > 1;

        PCompass.Visibility = room ? Visibility.Visible : Visibility.Collapsed;
        PCompassSurface.Visibility = room && !_pCompassFold ? Visibility.Visible : Visibility.Collapsed;
    }

    private void PCompassSync()
    {
        PCompassItem? current = null;

        foreach (PCompassItem row in _pCompassRows)
        {
            if (PCompassOffsetRead(row.PCompassItemTarget) is not double top)
            {
                continue;
            }

            if (top <= PCompassEdge)
            {
                current = row;
            }
        }

        if (current is null && _pCompassRows.Count > 0)
        {
            current = _pCompassRows[0];
        }

        foreach (PCompassItem row in _pCompassRows)
        {
            row.PCompassItemCurrent = ReferenceEquals(row, current);
        }
    }

    private double? PCompassOffsetRead(FrameworkElement target)
    {
        if (!target.IsVisible || !PDisplayContents.IsAncestorOf(target))
        {
            return null;
        }

        try
        {
            GeneralTransform placement = target.TransformToAncestor(PDisplayContents);
            return placement.Transform(new Point(0, 0)).Y;
        }
        catch (InvalidOperationException)
        {
            return null;
        }
    }

    private void PCompassScroll(PCompassItem item)
    {
        if (PCompassOffsetRead(item.PCompassItemTarget) is not double top)
        {
            return;
        }

        PDisplayContents.ScrollToVerticalOffset(PDisplayContents.VerticalOffset + top - PCompassLead);
    }

    private void PCompassRowHandle(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement row && row.DataContext is PCompassItem item)
        {
            PCompassScroll(item);
        }
    }

    private void PCompassSwitchHandle(object sender, RoutedEventArgs e)
    {
        _pCompassFold = PCompassSwitch.IsChecked != true;
        PCompassPlace();
    }

    private void PCompassScrollHandle(object sender, ScrollChangedEventArgs e)
    {
        if (e.ExtentHeightChange != 0 || e.ViewportHeightChange != 0)
        {
            PCompassPlace();
        }

        PCompassSync();
    }
}
