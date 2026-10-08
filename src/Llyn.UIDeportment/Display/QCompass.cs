using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Animation;
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

    private readonly ItemsControl _qCompassMeanings;

    private readonly ItemsControl _qCompassCollocations;

    private bool _qCompassOpened = true;

    public QCompass(CCompass area, FrameworkElement view)
    {
        ArgumentNullException.ThrowIfNull(area);
        ArgumentNullException.ThrowIfNull(view);

        _qCompassArea = area;
        _qCompassView = view;
        _qCompassContents = QContract.QContractFind<ScrollViewer>(view, "PDisplayContents");
        _qCompassHeader = QContract.QContractFind<Grid>(view, "PDisplayHeader");
        _qCompassColumn = QContract.QContractFind<StackPanel>(view, "PCompass");
        _qCompassSurface = QContract.QContractFind<Border>(view, "PCompassSurface");
        _qCompassMeanings = QContract.QContractFind<ItemsControl>(view, "PDisplayMeaning");
        _qCompassCollocations = QContract.QContractFind<ItemsControl>(view, "PDisplayCollocation");
        ToggleButton toggle = QContract.QContractFind<ToggleButton>(view, "PCompassSwitch");
        ItemsControl list = QContract.QContractFind<ItemsControl>(view, "PCompassList");
        _qCompassSections.Add((CCompassPart.CCompassPartSpeech,
            QContract.QContractFind<StackPanel>(view, "PDisplaySpeechSection")));
        _qCompassSections.Add((CCompassPart.CCompassPartFrequency,
            QContract.QContractFind<StackPanel>(view, "PDisplayFrequencySection")));
        _qCompassSections.Add((CCompassPart.CCompassPartMeaning,
            QContract.QContractFind<StackPanel>(view, "PDisplayMeaningSection")));
        _qCompassSections.Add((CCompassPart.CCompassPartCollocation,
            QContract.QContractFind<StackPanel>(view, "PDisplayCollocationSection")));
        _qCompassSections.Add((CCompassPart.CCompassPartIncoming,
            QContract.QContractFind<StackPanel>(view, "PDisplayIncomingSection")));
        _qCompassSections.Add((CCompassPart.CCompassPartNote,
            QContract.QContractFind<StackPanel>(view, "PDisplayNoteSection")));

        list.ItemsSource = _qCompassRows;
        _qCompassContents.ScrollChanged += QCompassScrollRefine;
        view.SizeChanged += QCompassSizeRefine;
        _qCompassHeader.SizeChanged += QCompassSizeRefine;
        QContract.QContractFind<QIconImage>(view, "PCompassIcon").QIconSource = QIcon.QIconResolve("compass", 24);
        toggle.IsChecked = _qCompassOpened;
        toggle.Click += QCompassSwitchRefine;
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

    private void QCompassTargetRefine(FrameworkElement target)
    {
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

    public void QCompassSpotlightRefine(long id)
    {
        _qCompassContents.Dispatcher.BeginInvoke(
            DispatcherPriority.Loaded, () => QCompassSpotlightRefine(_qCompassArea.CCompassCardFind(id)));
    }

    private void QCompassSpotlightRefine((CCompassPart, int)? place)
    {
        if (place is not (CCompassPart part, int index))
        {
            return;
        }

        ItemsControl cards = part == CCompassPart.CCompassPartCollocation
            ? _qCompassCollocations
            : _qCompassMeanings;
        if (cards.ItemContainerGenerator.ContainerFromIndex(index) is FrameworkElement card)
        {
            QCompassTargetRefine(card);
            ((Storyboard)_qCompassContents.FindResource("Theme.Card.Spotlight")).Begin(card);
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
