using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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

    private readonly Dictionary<CCompassPart, FrameworkElement> _qCompassSections;

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
        _qCompassSections = new()
        {
            [CCompassPart.CCompassPartSpeech] = QContract.QContractFind<StackPanel>(view, "PDisplaySpeechSection"),
            [CCompassPart.CCompassPartFrequency] =
                QContract.QContractFind<StackPanel>(view, "PDisplayFrequencySection"),
            [CCompassPart.CCompassPartMeaning] = QContract.QContractFind<StackPanel>(view, "PDisplayMeaningSection"),
            [CCompassPart.CCompassPartCollocation] =
                QContract.QContractFind<StackPanel>(view, "PDisplayCollocationSection"),
            [CCompassPart.CCompassPartIncoming] = QContract.QContractFind<StackPanel>(view, "PDisplayIncomingSection"),
            [CCompassPart.CCompassPartNote] = QContract.QContractFind<StackPanel>(view, "PDisplayNoteSection"),
        };
        list.ItemsSource = _qCompassRows;
        _qCompassContents.ScrollChanged += QCompassScrollRefine;
        view.SizeChanged += QCompassSizeRefine;
        _qCompassHeader.SizeChanged += QCompassSizeRefine;
        QContract.QContractFind<QIconImage>(view, "PCompassIcon").QIconSource = QIcon.QIconResolve("compass", 24);
        toggle.IsChecked = _qCompassOpened;
        toggle.Click += QCompassSwitchRefine;
        area.CCompassSought += (part, index) => _qCompassContents.Dispatcher.BeginInvoke(
            DispatcherPriority.Loaded, () => QCompassSpotlightRefine(part, index));
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
        QCompassRowsRefine(_qCompassArea.CCompassRead(QLocalizationCatalog.QLocalizationTextRead));
        QCompassColumnRefine();
        QCompassCurrentRefine();
    }

    private void QCompassRowsRefine(IReadOnlyList<CCompassRow> rows)
    {
        _qCompassRows.Clear();
        IReadOnlyList<CCompassPart> shown = _qCompassArea.CCompassShown;
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
                foreach (CCompassPart part in shown)
                {
                    if (part == row.CCompassRowPart)
                    {
                        target = _qCompassSections.GetValueOrDefault(part);
                    }
                }
            }

            if (target is not null)
            {
                _qCompassRows.Add(new QCompassItem(
                    target, row.CCompassRowName, row.CCompassRowNumber, row.CCompassRowDepth, row.CCompassRowMuted));
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
        int current = CCompass.CCompassLitRead(
        [
            .. _qCompassRows.Select(row => (
                row.QCompassItemMuted,
                QCompassOffsetDraw(row.QCompassItemTarget) is double top && top <= QCompassEdge)),
        ]);

        for (int index = 0; index < _qCompassRows.Count; index++)
        {
            _qCompassRows[index].QCompassItemCurrent = index == current;
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

    private void QCompassSpotlightRefine(CCompassPart part, int index)
    {
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
