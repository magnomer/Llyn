using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
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

        if (QLook.QLookPartFind<CheckBox>(container, "PCueMark") is CheckBox mark)
        {
            mark.IsChecked = cue.QCueItemFlag;
            mark.Click -= QCueStateObserve;
            mark.Click += QCueStateObserve;
        }

        if (QLook.QLookPartFind<CheckBox>(container, "PCueExclusion") is CheckBox exclusion)
        {
            exclusion.IsChecked = cue.QCueItemExcluded;
            exclusion.Click -= QCueStateObserve;
            exclusion.Click += QCueStateObserve;
        }

        if (QLook.QLookPartFind<QIconImage>(container, "PCueEarlierIcon") is QIconImage earlier)
        {
            earlier.QIconSource = QIcon.QIconResolve("earlier", 12);
        }

        if (QLook.QLookPartFind<QIconImage>(container, "PCueLaterIcon") is QIconImage later)
        {
            later.QIconSource = QIcon.QIconResolve("later", 12);
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
            || QLook.QLookPartFind<CheckBox>(container, "PCueMark") is not CheckBox mark
            || QLook.QLookPartFind<CheckBox>(container, "PCueExclusion") is not CheckBox exclusion)
        {
            return;
        }

        _cCue.CCueSet(
            cue.QCueItemId,
            QLook.QLookCheckedRead(mark.IsChecked),
            QLook.QLookCheckedRead(exclusion.IsChecked));
    }
}
