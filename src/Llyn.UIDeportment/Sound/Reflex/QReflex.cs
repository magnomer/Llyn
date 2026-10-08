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

    private CDisplaySound _cDisplaySound = null!;

    private CKindred _cKindred = null!;

    internal QReflex(FrameworkElement surface, QAnchor anchor)
    {
        _qReflexSurface = surface;
        _qReflexAnchor = anchor;
        ItemsControl list = QContract.QContractFind<ItemsControl>(_qReflexSurface, "PReflex");
        _qReflexRow = new QReflexList(list, QReflexFold);
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
        QReflexFold.Checked += QReflexFoldObserve;
        QReflexFold.Unchecked += QReflexFoldObserve;
    }

    private Grid QReflexBlock => QContract.QContractFind<Grid>(_qReflexSurface, "PReflexBlock");

    private StackPanel QReflexTable => QContract.QContractFind<StackPanel>(_qReflexSurface, "PReflexTable");

    private TextBlock QReflexLoading => QContract.QContractFind<TextBlock>(_qReflexSurface, "PReflexLoading");

    private ToggleButton QReflexFold => QContract.QContractFind<ToggleButton>(_qReflexSurface, "PReflexFold");

    private Button QReflexRenewal => QContract.QContractFind<Button>(_qReflexSurface, "PReflexRenewal");

    internal void QReflexIntroduce(
        CDesk desk, CDisplaySound displaySound, CEntry entry, CKindred kindred, CSounding sounding)
    {
        _cDisplaySound = displaySound;
        _cKindred = kindred;
        desk.CDeskStarted += QReflexAnchorRefine;
        sounding.CSoundingChanged += QReflexAnchorRefine;
        kindred.CKindredChanged += QReflexPendingRefine;
        displaySound.CDisplayFoldChanged += QReflexFoldRefine;
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
        _qReflexRow.QReflexListShow(reflex.CTimbreReflexRows);
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

    private void QReflexFoldObserve(object sender, RoutedEventArgs e)
    {
        _cDisplaySound.CDisplayReflexToggle(
            QLook.QLookCheckedRead(QReflexFold.IsChecked));
    }

    private void QReflexFoldRefine()
    {
        _qReflexRow.QReflexFoldRefine(_cDisplaySound.CDisplayFoldOpened);
    }
}
