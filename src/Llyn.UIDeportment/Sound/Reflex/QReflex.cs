using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QReflex
{
    private readonly FrameworkElement _qReflexSurface;

    private readonly ObservableCollection<QReflexItem> _qReflexItem = [];

    private readonly QAnchor _qReflexAnchor;

    private CEditor _cEditor = null!;

    internal QReflex(FrameworkElement surface, QAnchor anchor)
    {
        _qReflexSurface = surface;
        _qReflexAnchor = anchor;
        QReflexList.ItemsSource = _qReflexItem;
        QLookItem.QLookItemAttach(QReflexList, QReflexItem.QReflexItemRefine);
        QQuill.QQuillIntroduce(QReflexList);
        QAccentControl.QAccentControlAttach(QReflexList);
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

    private ItemsControl QReflexList => QContract.QContractFind<ItemsControl>(_qReflexSurface, "PReflex");

    private TextBlock QReflexLoading => QContract.QContractFind<TextBlock>(_qReflexSurface, "PReflexLoading");

    private ToggleButton QReflexFold => QContract.QContractFind<ToggleButton>(_qReflexSurface, "PReflexFold");

    private Button QReflexRenewal => QContract.QContractFind<Button>(_qReflexSurface, "PReflexRenewal");

    internal void QReflexIntroduce(CEditor editor)
    {
        _cEditor = editor;
        editor.CEditorDesk.CDeskStarted += QReflexAnchorRefine;
        editor.CEditorSounding.CSoundingChanged += QReflexAnchorRefine;
        editor.CEditorKindred.CKindredChanged += QReflexPendingRefine;
        editor.CEditorDisplay.CDisplaySound.CDisplayFoldChanged += QReflexFoldRefine;
        editor.CEditorEntry.CEntryDraftChanged += QReflexRefine;
    }

    private void QReflexAddObserve(object sender, ExecutedRoutedEventArgs e)
    {
        _cEditor.CEditorKindred.CKindredAdd((e.Parameter as QReflexItem)?.QReflexItemId);
    }

    private void QReflexRemoveObserve(object sender, ExecutedRoutedEventArgs e)
    {
        if (e.Parameter is QReflexItem row)
        {
            _cEditor.CEditorKindred.CKindredRemove(row.QReflexItemId);
        }
    }

    private void QReflexMainObserve(object sender, ExecutedRoutedEventArgs e)
    {
        if (e.Parameter is QReflexItem row)
        {
            _cEditor.CEditorKindred.CKindredToggle(row.QReflexItemId);
        }
    }

    private void QReflexRebuildRefine(object sender, ExecutedRoutedEventArgs e)
    {
        QReflexTable.MinWidth = QReflexTable.ActualWidth;
        QReflexTable.MinHeight = QReflexTable.ActualHeight;
    }

    private void QReflexRebuildObserve(object sender, ExecutedRoutedEventArgs e)
    {
        _cEditor.CEditorKindred.CKindredRebuild();
        QReflexPendingRefine();
    }

    private void QReflexTypeObserve(QReflexItem row, CReflexField field, string text)
    {
        CReflexTyped typed = _cEditor.CEditorKindred.CKindredSet(row.QReflexItemId, field, text);
        row.QReflexTypeRefine(typed);
        QReflexItem.QReflexLeadRefine(_qReflexItem, typed.CReflexTypedHeads);
    }

    private void QReflexRefine(CEntryDraft _)
    {
        QReflexRefine(_cEditor.CEditorKindred.CKindredRead());
    }

    private void QReflexRefine(CTimbreReflex reflex)
    {
        QReflexBlock.Visibility = QLook.QLookVisibleRead(reflex.CTimbreReflexShown);
        QReflexRenewal.Visibility = QReflexBlock.Visibility;
        PCard.PCardRowShow(
            _qReflexItem,
            reflex.CTimbreReflexRows,
            static row => row.QReflexItemId,
            static row => row.CReflexId,
            QReflexRowRefine,
            QReflexStateRefine);
        QReflexItem.QReflexFoldRefine(_qReflexItem, QReflexFold, reflex.CTimbreReflexOpened);
        QReflexAnchorRefine(reflex.CTimbreReflexAnchor);
        QReflexPendingRefine(reflex.CTimbreReflexPending);
    }

    private void QReflexAnchorRefine()
    {
        QReflexAnchorRefine(_cEditor.CEditorKindred.CKindredRead().CTimbreReflexAnchor);
    }

    private void QReflexAnchorRefine(CLecternAnchor anchor)
    {
        QReflexItem.QReflexAnchorRefine(_qReflexItem, anchor.CLecternAnchorOffered, anchor.CLecternAnchorTexts);
    }

    private void QReflexPendingRefine()
    {
        QReflexPendingRefine(_cEditor.CEditorKindred.CKindredPending);
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
        _cEditor.CEditorDisplay.CDisplaySound.CDisplayReflexToggle(
            QLook.QLookCheckedRead(QReflexFold.IsChecked));
    }

    private void QReflexFoldRefine()
    {
        QReflexItem.QReflexFoldRefine(
            _qReflexItem, QReflexFold, _cEditor.CEditorDisplay.CDisplaySound.CDisplayFoldOpened);
    }

    private QReflexItem QReflexRowRefine(CReflex reflex)
    {
        QReflexItem row = new(reflex);
        row.QReflexItemTyped += QReflexTypeObserve;
        return row;
    }

    private static QReflexItem QReflexStateRefine(QReflexItem row, CReflex reflex)
    {
        row.QReflexStateRefine(reflex);
        return row;
    }
}
