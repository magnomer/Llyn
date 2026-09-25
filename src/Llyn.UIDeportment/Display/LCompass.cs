using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Threading;
using Llyn.Conduct;
using Llyn.Core;

namespace Llyn.UIDeportment;

public sealed class LCompass
{
    private const double LCompassEdge = 28;

    private const double LCompassTop = 20;

    private const double LCompassGap = 8;

    private readonly ObservableCollection<LCompassItem> _lCompassRows = [];

    private readonly LDisplay _lCompassDisplay;

    private readonly FrameworkElement _lCompassView;

    private readonly ScrollViewer _lCompassContents;

    private readonly FrameworkElement _lCompassHeader;

    private readonly FrameworkElement _lCompassColumn;

    private readonly UIElement _lCompassSurface;

    private bool _lCompassOpened = true;

    private FrameworkElement _lCompassSpeech = null!;

    private FrameworkElement _lCompassFrequency = null!;

    private FrameworkElement _lCompassMeaning = null!;

    private ItemsControl _lCompassMeanings = null!;

    private FrameworkElement _lCompassCollocation = null!;

    private ItemsControl _lCompassCollocations = null!;

    private FrameworkElement _lCompassIncoming = null!;

    private FrameworkElement _lCompassNote = null!;

    public LCompass(
        LDisplay display,
        FrameworkElement view,
        ScrollViewer contents,
        FrameworkElement header,
        FrameworkElement compass,
        UIElement surface,
        ToggleButton toggle,
        ItemsControl list)
    {
        ArgumentNullException.ThrowIfNull(display);
        ArgumentNullException.ThrowIfNull(view);
        ArgumentNullException.ThrowIfNull(contents);
        ArgumentNullException.ThrowIfNull(header);
        ArgumentNullException.ThrowIfNull(compass);
        ArgumentNullException.ThrowIfNull(surface);
        ArgumentNullException.ThrowIfNull(toggle);
        ArgumentNullException.ThrowIfNull(list);

        _lCompassDisplay = display;
        _lCompassView = view;
        _lCompassContents = contents;
        _lCompassHeader = header;
        _lCompassColumn = compass;
        _lCompassSurface = surface;

        list.ItemsSource = _lCompassRows;
        contents.ScrollChanged += LCompassScrollHandle;
        view.SizeChanged += LCompassSizeHandle;
        header.SizeChanged += LCompassSizeHandle;
        toggle.IsChecked = _lCompassOpened;
        toggle.Click += LCompassSwitchHandle;
    }

    public double LCompassLead { get; } = 14;

    public void LCompassSectionAttach(
        FrameworkElement speech,
        FrameworkElement frequency,
        FrameworkElement meaning,
        ItemsControl meanings,
        FrameworkElement collocation,
        ItemsControl collocations,
        FrameworkElement incoming,
        FrameworkElement note)
    {
        _lCompassSpeech = speech;
        _lCompassFrequency = frequency;
        _lCompassMeaning = meaning;
        _lCompassMeanings = meanings;
        _lCompassCollocation = collocation;
        _lCompassCollocations = collocations;
        _lCompassIncoming = incoming;
        _lCompassNote = note;
    }

    public void LCompassUpdate()
    {
        _lCompassView.Dispatcher.BeginInvoke(DispatcherPriority.Loaded, LCompassRowCreate);
    }

    public void LCompassClear()
    {
        _lCompassRows.Clear();
        _lCompassColumn.Visibility = Visibility.Collapsed;
    }

    private void LCompassRowCreate()
    {
        List<LCompassItem> rows = [];
        string unknown = LLocalizationCatalog.LLocalizationTextRead("Display.Unknown");

        LCompassSectionAdd(rows, _lCompassSpeech, LLocalizationCatalog.LLocalizationTextRead("Speech.Title"));
        LCompassSectionAdd(rows, _lCompassFrequency, LLocalizationCatalog.LLocalizationTextRead("Frequency.Title"));

        if (LCompassSectionAdd(
                rows,
                _lCompassMeaning, LLocalizationCatalog.LLocalizationTextRead("Display.MeaningPlural")))
        {
            LCompassCardAdd(
                rows,
                _lCompassMeanings, LLocalizationCatalog.LLocalizationTextRead("Display.MeaningSingle"), unknown);
        }

        if (LCompassSectionAdd(
                rows,
                _lCompassCollocation, LLocalizationCatalog.LLocalizationTextRead("Display.Collocation")))
        {
            LCompassCardAdd(
                rows,
                _lCompassCollocations,
                LLocalizationCatalog.LLocalizationTextRead("Display.CollocationSingle"),
                unknown);
        }

        LCompassSectionAdd(rows, _lCompassIncoming, LLocalizationCatalog.LLocalizationTextRead("Display.Translated"));
        LCompassSectionAdd(rows, _lCompassNote, LLocalizationCatalog.LLocalizationTextRead("Display.Note"));

        LCompassNameApply(rows);

        _lCompassRows.Clear();
        foreach (LCompassItem row in rows)
        {
            _lCompassRows.Add(row);
        }

        LCompassPlace();
        LCompassSync();
    }

    private void LCompassNameApply(List<LCompassItem> rows)
    {
        List<string> labels = new(rows.Count);
        foreach (LCompassItem row in rows)
        {
            labels.Add(row.LCompassItemLabel);
        }

        IReadOnlyList<string> names = _lCompassDisplay.LDisplayNameResolve(labels);
        for (int index = 0; index < rows.Count; index++)
        {
            rows[index].LCompassItemName = names[index];
        }
    }

    private static bool LCompassSectionAdd(List<LCompassItem> rows, FrameworkElement section, string label)
    {
        if (section.Visibility != Visibility.Visible)
        {
            return false;
        }

        rows.Add(new LCompassItem(section, label, string.Empty, 0));
        return true;
    }

    private static void LCompassCardAdd(List<LCompassItem> rows, ItemsControl cards, string kind, string unknown)
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

            rows.Add(new LCompassItem(
                container,
                LDisplay.LDisplayTitleRead(card, kind, unknown),
                card.LCardDraftPosition.ToString(CultureInfo.CurrentCulture),
                1));
        }
    }

    private void LCompassPlace()
    {
        bool room = _lCompassContents.Visibility == Visibility.Visible
            && _lCompassContents.ScrollableHeight > 0
            && _lCompassRows.Count > 1;

        _lCompassColumn.Visibility = room ? Visibility.Visible : Visibility.Collapsed;
        _lCompassSurface.Visibility = room && _lCompassOpened
            ? Visibility.Visible
            : Visibility.Collapsed;

        Thickness margin = _lCompassColumn.Margin;
        margin.Top = LCompassTopResolve(margin.Right);
        _lCompassColumn.Margin = margin;
    }

    private double LCompassTopResolve(double side)
    {
        double headerRight = _lCompassContents.Margin.Left + _lCompassHeader.ActualWidth;
        double compassLeft = _lCompassView.ActualWidth - side - _lCompassColumn.Width;

        return headerRight > compassLeft
            ? _lCompassContents.Margin.Top + _lCompassHeader.ActualHeight + LCompassGap
            : LCompassTop;
    }

    private void LCompassSync()
    {
        LCompassItem? current = null;

        foreach (LCompassItem row in _lCompassRows)
        {
            if (LCompassOffsetRead(row.LCompassItemTarget) is not double top)
            {
                continue;
            }

            if (top <= LCompassEdge)
            {
                current = row;
            }
        }

        if (current is null && _lCompassRows.Count > 0)
        {
            current = _lCompassRows[0];
        }

        foreach (LCompassItem row in _lCompassRows)
        {
            row.LCompassItemCurrent = ReferenceEquals(row, current);
        }
    }

    public double? LCompassOffsetRead(FrameworkElement target)
    {
        ArgumentNullException.ThrowIfNull(target);

        if (!target.IsVisible || !_lCompassContents.IsAncestorOf(target))
        {
            return null;
        }

        try
        {
            GeneralTransform placement = target.TransformToAncestor(_lCompassContents);
            return placement.Transform(new Point(0, 0)).Y;
        }
        catch (InvalidOperationException)
        {
            return null;
        }
    }

    public void LCompassScroll(FrameworkElement target)
    {
        if (LCompassOffsetRead(target) is not double top)
        {
            return;
        }

        _lCompassContents.ScrollToVerticalOffset(_lCompassContents.VerticalOffset + top - LCompassLead);
    }

    public void LCompassRowHandle(object sender)
    {
        if (sender is FrameworkElement row && row.DataContext is LCompassItem item)
        {
            LCompassScroll(item.LCompassItemTarget);
        }
    }

    private void LCompassSwitchHandle(object sender, RoutedEventArgs e)
    {
        _lCompassOpened = !_lCompassOpened;
        if (sender is ToggleButton toggle)
        {
            toggle.IsChecked = _lCompassOpened;
        }

        LCompassPlace();
    }

    private void LCompassSizeHandle(object sender, SizeChangedEventArgs e)
    {
        LCompassPlace();
    }

    private void LCompassScrollHandle(object sender, ScrollChangedEventArgs e)
    {
        if (e.ExtentHeightChange != 0 || e.ViewportHeightChange != 0)
        {
            LCompassPlace();
        }

        LCompassSync();
    }
}
