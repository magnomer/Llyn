using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QCue
{
    private readonly UserControl _qCueSurface;

    private readonly ObservableCollection<QCueItem> _qCueList = [];

    private CCue _cCue = null!;

    internal QCue(UserControl surface)
    {
        ArgumentNullException.ThrowIfNull(surface);

        _qCueSurface = surface;
        QCueFresh.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("new", 24));
        QCueRemoval.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("delete", 24));
        QCueFresh.Click += QCueFreshObserve;
        QCueRemoval.Click += QCueRemovalObserve;
    }

    private ListBox QCueView => QContract.QContractFind<ListBox>(_qCueSurface, "PCue");

    private Button QCueFresh => QContract.QContractFind<Button>(_qCueSurface, "PCueFresh");

    private Button QCueRemoval => QContract.QContractFind<Button>(_qCueSurface, "PCueRemoval");

    internal void QCueIntroduce(CCue cue)
    {
        ArgumentNullException.ThrowIfNull(cue);

        _cCue = cue;
        _cCue.CCueChanged += QCueRefine;

        QCueView.ItemsSource = _qCueList;
        QLookItem.QLookItemAttach(QCueView, QCueItemRefine);
    }

    internal void QCueRefine()
    {
        List<QCueItem> fresh = [];
        foreach (CCueRow row in _cCue.CCueRead())
        {
            fresh.Add(new QCueItem(
                row.CCueRowId,
                row.CCueRowPosition,
                row.CCueRowLine,
                row.CCueRowOrphaned,
                row.CCueRowFlag,
                row.CCueRowExcluded));
        }

        QSplice.QSpliceRefine(_qCueList, fresh, QCueItem.QCueItemMatch, QCueItem.QCueItemSync);
    }

    private void QCueItemRefine(FrameworkElement container, object item, string? _)
    {
        if (item is not QCueItem cue)
        {
            return;
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PCueText") is TextBlock text)
        {
            text.Text = cue.QCueItemText;
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PCueOrphan") is TextBlock orphan)
        {
            orphan.Visibility = QLook.QLookVisibleRead(cue.QCueItemOrphaned);
        }

        if (QLook.QLookPartFind<ToggleButton>(container, "PCueMark") is ToggleButton mark)
        {
            mark.IsChecked = cue.QCueItemFlag;
            mark.Click -= QCueStateObserve;
            mark.Click += QCueStateObserve;
        }

        if (QLook.QLookPartFind<QIconImage>(container, "PCueMarkIcon") is QIconImage markIcon)
        {
            markIcon.QIconSource = QIcon.QIconResolve("star", 16);
            markIcon.IsEnabled = cue.QCueItemFlag;
        }

        if (QLook.QLookPartFind<ToggleButton>(container, "PCueExclusion") is ToggleButton exclusion)
        {
            exclusion.IsChecked = cue.QCueItemExcluded;
            exclusion.Click -= QCueStateObserve;
            exclusion.Click += QCueStateObserve;
        }

        if (QLook.QLookPartFind<QIconImage>(container, "PCueExclusionIcon") is QIconImage exclusionIcon)
        {
            exclusionIcon.QIconSource = QIcon.QIconResolve("filter", 16);
            exclusionIcon.IsEnabled = cue.QCueItemExcluded;
        }

        if (QLook.QLookPartFind<QIconImage>(container, "PCueEarlierIcon") is QIconImage earlier)
        {
            earlier.QIconSource = QIcon.QIconResolve("earlier", 16);
        }

        if (QLook.QLookPartFind<QIconImage>(container, "PCueLaterIcon") is QIconImage later)
        {
            later.QIconSource = QIcon.QIconResolve("later", 16);
        }

        QCueHandleRefine(container, "PCueEarlier", QCueEarlierObserve);
        QCueHandleRefine(container, "PCueLater", QCueLaterObserve);
    }

    private static void QCueHandleRefine(FrameworkElement container, string part, RoutedEventHandler observe)
    {
        if (QLook.QLookPartFind<Button>(container, part) is Button handle)
        {
            handle.Click -= observe;
            handle.Click += observe;
        }
    }

    private void QCueFreshObserve(object sender, RoutedEventArgs e)
    {
        _cCue.CCueCreate();
    }

    private void QCueRemovalObserve(object sender, RoutedEventArgs e)
    {
        List<long> cues = [];
        foreach (object chosen in QCueView.SelectedItems)
        {
            if (chosen is QCueItem cue)
            {
                cues.Add(cue.QCueItemId);
            }
        }

        _cCue.CCueRemove(cues);
    }

    private void QCueEarlierObserve(object sender, RoutedEventArgs e)
    {
        if (QSender.QSenderItemRead<QCueItem>(sender) is QCueItem cue)
        {
            _cCue.CCueMove(cue.QCueItemId, cue.QCueItemPosition - 1);
        }
    }

    private void QCueLaterObserve(object sender, RoutedEventArgs e)
    {
        if (QSender.QSenderItemRead<QCueItem>(sender) is QCueItem cue)
        {
            _cCue.CCueMove(cue.QCueItemId, cue.QCueItemPosition + 1);
        }
    }

    private void QCueStateObserve(object sender, RoutedEventArgs e)
    {
        if (QSender.QSenderItemRead<QCueItem>(sender) is not QCueItem cue
            || QCueView.ItemContainerGenerator.ContainerFromItem(cue) is not FrameworkElement container
            || QLook.QLookPartFind<ToggleButton>(container, "PCueMark") is not ToggleButton mark
            || QLook.QLookPartFind<ToggleButton>(container, "PCueExclusion") is not ToggleButton exclusion)
        {
            return;
        }

        _cCue.CCueSet(
            cue.QCueItemId,
            QLook.QLookCheckedRead(mark.IsChecked),
            QLook.QLookCheckedRead(exclusion.IsChecked));
    }
}
