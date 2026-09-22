using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using Llyn.Core;

namespace Llyn.UIVeneer;

public partial class PDisplay
{
    private const double PCompassEdge = 28;

    private const double PCompassLead = 14;

    private const double PCompassTop = 20;

    private const double PCompassGap = 8;

    private readonly ObservableCollection<PCompassItem> _pCompassRows = [];

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

        string unknown = PLocalizationCatalog.PLocalizationTextRead("Display.Unknown");

        PCompassSectionAdd(PDisplaySpeechSection, PLocalizationCatalog.PLocalizationTextRead("Speech.Title"));
        PCompassSectionAdd(PDisplayFrequencySection, PLocalizationCatalog.PLocalizationTextRead("Frequency.Title"));

        if (PCompassSectionAdd(
                PDisplayMeaningSection, PLocalizationCatalog.PLocalizationTextRead("Display.MeaningPlural")))
        {
            PCompassCardAdd(
                PDisplayMeaning, PLocalizationCatalog.PLocalizationTextRead("Display.MeaningSingle"), unknown);
        }

        if (PCompassSectionAdd(
                PDisplayCollocationSection, PLocalizationCatalog.PLocalizationTextRead("Display.Collocation")))
        {
            PCompassCardAdd(
                PDisplayCollocation, PLocalizationCatalog.PLocalizationTextRead("Display.CollocationSingle"), unknown);
        }

        PCompassSectionAdd(PDisplayIncomingSection, PLocalizationCatalog.PLocalizationTextRead("Display.Translated"));
        PCompassSectionAdd(PDisplayNoteSection, PLocalizationCatalog.PLocalizationTextRead("Display.Note"));

        PCompassNameApply();

        PCompassPlace();
        PCompassSync();
    }

    private void PCompassNameApply()
    {
        List<string> labels = new(_pCompassRows.Count);
        foreach (PCompassItem row in _pCompassRows)
        {
            labels.Add(row.PCompassItemLabel);
        }

        IReadOnlyList<string> names = _lDisplay.LDisplayNameResolve(labels);
        for (int index = 0; index < names.Count; index++)
        {
            _pCompassRows[index].PCompassItemName = names[index];
        }
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

    private void PCompassCardAdd(ItemsControl cards, string kind, string unknown)
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

            string label = card.LCardDraftTitle.LStateValueUncertain
                ? unknown
                : card.LCardDraftTitle.LStateValueShown ?? kind;

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
        PCompassSurface.Visibility = PLook.PLookVisibleRead(room && PLook.PLookCheckedRead(PCompassSwitch.IsChecked));

        Thickness margin = PCompass.Margin;
        margin.Top = PCompassTopResolve(margin.Right);
        PCompass.Margin = margin;
    }

    private double PCompassTopResolve(double side)
    {
        double headerRight = PDisplayContents.Margin.Left + PDisplayHeader.ActualWidth;
        double compassLeft = ActualWidth - side - PCompass.Width;

        return headerRight > compassLeft
            ? PDisplayContents.Margin.Top + PDisplayHeader.ActualHeight + PCompassGap
            : PCompassTop;
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

    internal void PCompassRowHandle(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement row && row.DataContext is PCompassItem item)
        {
            PCompassScroll(item);
        }
    }

    private void PCompassSwitchHandle(object sender, RoutedEventArgs e)
    {
        PCompassPlace();
    }

    private void PCompassSizeHandle(object sender, SizeChangedEventArgs e)
    {
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
