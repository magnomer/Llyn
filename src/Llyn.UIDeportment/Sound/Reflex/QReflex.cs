using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QReflex
{
    private readonly FrameworkElement _qReflexSurface;

    private readonly QReflexList _qReflexRow;

    private readonly QAnchor _qReflexAnchor;

    private CKindred _cKindred = null!;

    internal QReflex(FrameworkElement surface, QAnchor anchor)
    {
        _qReflexSurface = surface;
        _qReflexAnchor = anchor;
        ItemsControl list = QContract.QContractFind<ItemsControl>(_qReflexSurface, "PReflex");
        _qReflexRow = new QReflexList(list, QReflexHinge);
        _qReflexRow.QReflexListTyped += QReflexTypeObserve;
        QQuill.QQuillIntroduce(list);
        QAccentControl.QAccentControlAttach(list);
        StackPanel sound = QContract.QContractFind<StackPanel>(_qReflexSurface, "PEditorSound");
        sound.CommandBindings.Add(new CommandBinding(
            QReflexCommand.QReflexCommandAddition, QReflexAddObserve));
        sound.CommandBindings.Add(new CommandBinding(
            QReflexCommand.QReflexCommandRemoval, QReflexRemoveObserve));
        sound.CommandBindings.Add(new CommandBinding(
            QReflexCommand.QReflexCommandMain, QReflexMainObserve));
        sound.CommandBindings.Add(new CommandBinding(
            QReflexCommand.QReflexCommandAnchor, _qReflexAnchor.QAnchorReflexObserve));
        CommandBinding renewal = new(QReflexCommand.QReflexCommandRenewal);
        renewal.Executed += QReflexRebuildRefine;
        renewal.Executed += QReflexRebuildObserve;
        QReflexRenewal.CommandBindings.Add(renewal);
        QReflexHinge.Click += QReflexHingeObserve;
    }

    private Grid QReflexBlock => QContract.QContractFind<Grid>(_qReflexSurface, "PReflexBlock");

    private StackPanel QReflexTable => QContract.QContractFind<StackPanel>(_qReflexSurface, "PReflexTable");

    private TextBlock QReflexLoading => QContract.QContractFind<TextBlock>(_qReflexSurface, "PReflexLoading");

    private ToggleButton QReflexHinge => QContract.QContractFind<ToggleButton>(_qReflexSurface, "PReflexHinge");

    private Button QReflexRenewal => QContract.QContractFind<Button>(_qReflexSurface, "PReflexRenewal");

    internal void QReflexIntroduce(CDesk desk, CEntry entry, CKindred kindred, CSounding sounding)
    {
        _cKindred = kindred;
        desk.CDeskStarted += QReflexAnchorRefine;
        sounding.CSoundingChanged += QReflexAnchorRefine;
        kindred.CKindredChanged += QReflexPendingRefine;
        entry.CEntryDraftChanged += QReflexRefine;
    }

    private void QReflexAddObserve(object sender, ExecutedRoutedEventArgs e)
    {
        _cKindred.CKindredAdd((e.Parameter as QReflexItem)?.QReflexItemId);
    }

    private void QReflexRemoveObserve(object sender, ExecutedRoutedEventArgs e)
    {
        if (e.Parameter is QReflexItem row)
        {
            _cKindred.CKindredRemove(row.QReflexItemId);
        }
    }

    private void QReflexMainObserve(object sender, ExecutedRoutedEventArgs e)
    {
        if (e.Parameter is QReflexItem row)
        {
            _cKindred.CKindredToggle(row.QReflexItemId);
        }
    }

    private void QReflexRebuildRefine(object sender, ExecutedRoutedEventArgs e)
    {
        QReflexTable.MinWidth = QReflexTable.ActualWidth;
        QReflexTable.MinHeight = QReflexTable.ActualHeight;
    }

    private void QReflexRebuildObserve(object sender, ExecutedRoutedEventArgs e)
    {
        _cKindred.CKindredRebuild();
        QReflexPendingRefine();
    }

    private void QReflexTypeObserve(QReflexItem row, CReflexField field, string text)
    {
        CReflexTyped typed = _cKindred.CKindredSet(row.QReflexItemId, field, text);
        row.QReflexTypeRefine(typed.CReflexTypedRow);
        _qReflexRow.QReflexLeadRefine(typed.CReflexTypedHeads);
    }

    private void QReflexRefine(CEntryDraft _)
    {
        QReflexRefine(_cKindred.CKindredRead());
    }

    private void QReflexRefine(CTimbreReflex reflex)
    {
        QReflexBlock.Visibility = QLook.QLookVisibleRead(reflex.CTimbreReflexShown);
        QReflexRenewal.Visibility = QReflexBlock.Visibility;
        _qReflexRow.QReflexListShow(reflex.CTimbreReflexRows, reflex.CTimbreReflexFoldable);
        _qReflexRow.QReflexFoldRefine(reflex.CTimbreReflexOpened);
        QReflexAnchorRefine(reflex.CTimbreReflexAnchor);
        QReflexPendingRefine(reflex.CTimbreReflexPending);
    }

    private void QReflexAnchorRefine()
    {
        QReflexAnchorRefine(_cKindred.CKindredRead().CTimbreReflexAnchor);
    }

    private void QReflexAnchorRefine(CLecternAnchor anchor)
    {
        _qReflexRow.QReflexAnchorRefine(anchor);
    }

    private void QReflexPendingRefine()
    {
        QReflexPendingRefine(_cKindred.CKindredPending);
    }

    private void QReflexPendingRefine(bool pending)
    {
        if (!pending)
        {
            QReflexTable.MinWidth = 0;
            QReflexTable.MinHeight = 0;
        }

        QReflexLoading.Visibility = QLook.QLookVisibleRead(pending);
        QReflexRenewal.SetValue(
            QLook.QLookCueProperty,
            QLook.QLookFirstRead(pending, QLookCue.QLookCuePending, QLookCue.QLookCueBase));
    }

    private void QReflexHingeObserve(object sender, RoutedEventArgs e)
    {
        QReflexHingeRefine(_cKindred.CKindredSpread(QLook.QLookCheckedRead(QReflexHinge.IsChecked)));
    }

    private void QReflexHingeRefine(bool stored)
    {
        if (!stored)
        {
            QReflexHinge.IsChecked = !QLook.QLookCheckedRead(QReflexHinge.IsChecked);
        }
    }
}
