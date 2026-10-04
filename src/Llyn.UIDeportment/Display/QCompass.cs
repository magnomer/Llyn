using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Threading;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

public sealed class QCompass
{
    private const double QCompassEdge = 28;

    private const double QCompassTop = 20;

    private const double QCompassGap = 8;

    private const double QCompassLead = 14;

    private readonly ObservableCollection<QCompassItem> _qCompassRows = [];

    private readonly List<(CCompassPart, FrameworkElement)> _qCompassSections = [];


    private readonly CCompass _qCompassArea;

    private readonly FrameworkElement _qCompassView;

    private readonly ScrollViewer _qCompassContents;

    private readonly FrameworkElement _qCompassHeader;

    private readonly FrameworkElement _qCompassColumn;

    private readonly UIElement _qCompassSurface;

    private bool _qCompassOpened = true;

    private ItemsControl _qCompassMeanings = null!;

    private ItemsControl _qCompassCollocations = null!;

    public QCompass(
        CCompass area,
        FrameworkElement view,
        ScrollViewer contents,
        FrameworkElement header,
        FrameworkElement compass,
        UIElement surface,
        ToggleButton toggle,
        ItemsControl list)
    {
        ArgumentNullException.ThrowIfNull(area);
        ArgumentNullException.ThrowIfNull(view);
        ArgumentNullException.ThrowIfNull(contents);
        ArgumentNullException.ThrowIfNull(header);
        ArgumentNullException.ThrowIfNull(compass);
        ArgumentNullException.ThrowIfNull(surface);
        ArgumentNullException.ThrowIfNull(toggle);
        ArgumentNullException.ThrowIfNull(list);

        _qCompassArea = area;
        _qCompassView = view;
        _qCompassContents = contents;
        _qCompassHeader = header;
        _qCompassColumn = compass;
        _qCompassSurface = surface;

        list.ItemsSource = _qCompassRows;
        contents.ScrollChanged += QCompassScrollRefine;
        view.SizeChanged += QCompassSizeRefine;
        header.SizeChanged += QCompassSizeRefine;
        toggle.IsChecked = _qCompassOpened;
        toggle.Click += QCompassSwitchRefine;
    }

    public void QCompassSectionIntroduce(
        FrameworkElement speech,
        FrameworkElement frequency,
        FrameworkElement meaning,
        ItemsControl meanings,
        FrameworkElement collocation,
        ItemsControl collocations,
        FrameworkElement incoming,
        FrameworkElement note)
    {
        _qCompassSections.Clear();
        _qCompassSections.Add((CCompassPart.CCompassPartSpeech, speech));
        _qCompassSections.Add((CCompassPart.CCompassPartFrequency, frequency));
        _qCompassSections.Add((CCompassPart.CCompassPartMeaning, meaning));
        _qCompassSections.Add((CCompassPart.CCompassPartCollocation, collocation));
        _qCompassSections.Add((CCompassPart.CCompassPartIncoming, incoming));
        _qCompassSections.Add((CCompassPart.CCompassPartNote, note));
        _qCompassMeanings = meanings;
        _qCompassCollocations = collocations;
    }

    public void QCompassRefine()
    {
        _qCompassView.Dispatcher.BeginInvoke(DispatcherPriority.Loaded, QCompassListRefine);
    }

    public void QCompassEmptyRefine()
    {
        _qCompassRows.Clear();
        _qCompassColumn.Visibility = Visibility.Collapsed;
    }

    private void QCompassListRefine()
    {
        List<CCompassPart> parts = [];
        foreach ((CCompassPart part, FrameworkElement section) in _qCompassSections)
        {
            if (section.Visibility == Visibility.Visible)
            {
                parts.Add(part);
            }
        }

        QCompassRowsRefine(_qCompassArea.CCompassRead(parts, QLocalizationCatalog.QLocalizationTextRead));
        QCompassColumnRefine();
        QCompassCurrentRefine();
    }

    private void QCompassRowsRefine(IReadOnlyList<CCompassRow> rows)
    {
        _qCompassRows.Clear();
        foreach (CCompassRow row in rows)
        {
            FrameworkElement? target = null;
            if (row.CCompassRowCard is int card)
            {
                ItemsControl cards = row.CCompassRowPart == CCompassPart.CCompassPartCollocation
                    ? _qCompassCollocations
                    : _qCompassMeanings;
                target = cards.ItemContainerGenerator.ContainerFromIndex(card) as FrameworkElement;
            }
            else
            {
                foreach ((CCompassPart part, FrameworkElement section) in _qCompassSections)
                {
                    if (part == row.CCompassRowPart)
                    {
                        target = section;
                    }
                }
            }

            if (target is not null)
            {
                _qCompassRows.Add(new QCompassItem(
                    target, row.CCompassRowName, row.CCompassRowNumber, row.CCompassRowDepth));
            }
        }
    }

    private void QCompassColumnRefine()
    {
        bool room = _qCompassContents.Visibility == Visibility.Visible
            && _qCompassContents.ScrollableHeight > 0
            && _qCompassRows.Count > 1;

        _qCompassColumn.Visibility = room ? Visibility.Visible : Visibility.Collapsed;
        _qCompassSurface.Visibility = room && _qCompassOpened
            ? Visibility.Visible
            : Visibility.Collapsed;

        Thickness margin = _qCompassColumn.Margin;
        margin.Top = QCompassTopDraw(margin.Right);
        _qCompassColumn.Margin = margin;
    }

    private double QCompassTopDraw(double side)
    {
        double headerRight = _qCompassContents.Margin.Left + _qCompassHeader.ActualWidth;
        double compassLeft = _qCompassView.ActualWidth - side - _qCompassColumn.Width;

        return headerRight > compassLeft
            ? _qCompassContents.Margin.Top + _qCompassHeader.ActualHeight + QCompassGap
            : QCompassTop;
    }

    private void QCompassCurrentRefine()
    {
        QCompassItem? current = null;

        foreach (QCompassItem row in _qCompassRows)
        {
            if (QCompassOffsetDraw(row.QCompassItemTarget) is not double top)
            {
                continue;
            }

            if (top <= QCompassEdge)
            {
                current = row;
            }
        }

        if (current is null && _qCompassRows.Count > 0)
        {
            current = _qCompassRows[0];
        }

        foreach (QCompassItem row in _qCompassRows)
        {
            row.QCompassItemCurrent = ReferenceEquals(row, current);
        }
    }

    private double? QCompassOffsetDraw(FrameworkElement target)
    {
        if (!target.IsVisible || !_qCompassContents.IsAncestorOf(target))
        {
            return null;
        }

        try
        {
            GeneralTransform placement = target.TransformToAncestor(_qCompassContents);
            return placement.Transform(new Point(0, 0)).Y;
        }
        catch (InvalidOperationException)
        {
            return null;
        }
    }

    public void QCompassTargetRefine(FrameworkElement target)
    {
        ArgumentNullException.ThrowIfNull(target);

        if (QCompassOffsetDraw(target) is not double top)
        {
            return;
        }

        _qCompassContents.ScrollToVerticalOffset(_qCompassContents.VerticalOffset + top - QCompassLead);
    }

    public void QCompassRowRefine(object sender)
    {
        if (sender is FrameworkElement row && row.DataContext is QCompassItem item)
        {
            QCompassTargetRefine(item.QCompassItemTarget);
        }
    }

    private void QCompassSwitchRefine(object sender, RoutedEventArgs e)
    {
        _qCompassOpened = !_qCompassOpened;
        if (sender is ToggleButton toggle)
        {
            toggle.IsChecked = _qCompassOpened;
        }

        QCompassColumnRefine();
    }

    private void QCompassSizeRefine(object sender, SizeChangedEventArgs e)
    {
        QCompassColumnRefine();
    }

    private void QCompassScrollRefine(object sender, ScrollChangedEventArgs e)
    {
        if (e.ExtentHeightChange != 0 || e.ViewportHeightChange != 0)
        {
            QCompassColumnRefine();
        }

        QCompassCurrentRefine();
    }
}
