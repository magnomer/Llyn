using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

public partial class PEditor
{
    private readonly ObservableCollection<QReflexItem> _pReflexItem = [];

    private Grid PReflexBlock => (Grid)FindName(nameof(PReflexBlock));

    private StackPanel PReflexTable => (StackPanel)FindName(nameof(PReflexTable));

    private PReflexList PReflex => (PReflexList)FindName(nameof(PReflex));

    private TextBlock PReflexLoading => (TextBlock)FindName(nameof(PReflexLoading));

    private ToggleButton PReflexFold => (ToggleButton)FindName(nameof(PReflexFold));

    private Button PReflexRenewal => (Button)FindName(nameof(PReflexRenewal));

    private void PReflexAttach()
    {
        PReflex.ItemsSource = _pReflexItem;
        QLookItem.QLookItemAttach(PReflex, QReflexItem.QReflexItemRefine);
        QField.QFieldCellAttach(PReflex);
        PAccentControl.PAccentControlAttach(PReflex);
        PEditorSound.CommandBindings.Add(new CommandBinding(
            PReflexCommand.PReflexCommandAddition, PReflexAddObserve));
        PEditorSound.CommandBindings.Add(new CommandBinding(
            PReflexCommand.PReflexCommandRemoval, PReflexRemoveObserve));
        PEditorSound.CommandBindings.Add(new CommandBinding(
            PReflexCommand.PReflexCommandMain, PReflexMainObserve));
        PEditorSound.CommandBindings.Add(new CommandBinding(
            PReflexCommand.PReflexCommandAnchor, PReflexAnchorObserve));
        CommandBinding renewal = new(PReflexCommand.PReflexCommandRenewal);
        renewal.Executed += PReflexRebuildRefine;
        renewal.Executed += PReflexRebuildObserve;
        PReflexRenewal.CommandBindings.Add(renewal);
        PReflexFold.Checked += PReflexFoldObserve;
        PReflexFold.Unchecked += PReflexFoldObserve;
    }

    private void PReflexAddObserve(object sender, ExecutedRoutedEventArgs e)
    {
        _qEditor.QEditorArea.CEditorTimbre.CTimbreReflexAdd((e.Parameter as QReflexItem)?.QReflexItemId ?? 0);
    }

    private void PReflexRemoveObserve(object sender, ExecutedRoutedEventArgs e)
    {
        if (e.Parameter is QReflexItem row)
        {
            _qEditor.QEditorArea.CEditorTimbre.CTimbreReflexRemove(row.QReflexItemId);
        }
    }

    private void PReflexMainObserve(object sender, ExecutedRoutedEventArgs e)
    {
        if (e.Parameter is QReflexItem row)
        {
            _qEditor.QEditorArea.CEditorTimbre.CTimbreReflexToggle(row.QReflexItemId);
        }
    }

    private void PReflexRebuildRefine(object sender, ExecutedRoutedEventArgs e)
    {
        PReflexTable.MinWidth = PReflexTable.ActualWidth;
        PReflexTable.MinHeight = PReflexTable.ActualHeight;
    }

    private void PReflexRebuildObserve(object sender, ExecutedRoutedEventArgs e)
    {
        _qEditor.QEditorArea.CEditorTimbre.CTimbreReflexRebuild();
        PReflexPendingRefine();
    }

    private void PReflexChangeObserve(object? sender, PropertyChangedEventArgs e)
    {
        if (sender is not QReflexItem row)
        {
            return;
        }

        CTimbre timbre = _qEditor.QEditorArea.CEditorTimbre;
        long id = row.QReflexItemId;
        switch (e.PropertyName)
        {
            case nameof(QReflexItem.QReflexItemLanguage):
                QReflexItem.QReflexLeadRefine(
                    _pReflexItem,
                    timbre.CTimbreReflexSet(id, CReflexField.CReflexFieldLanguage, row.QReflexItemLanguage));
                break;
            case nameof(QReflexItem.QReflexItemKind):
                timbre.CTimbreReflexSet(id, CReflexField.CReflexFieldKind, row.QReflexItemKind);
                break;
            case nameof(QReflexItem.QReflexItemText):
                timbre.CTimbreReflexSet(id, CReflexField.CReflexFieldText, row.QReflexItemText);
                break;
            case nameof(QReflexItem.QReflexItemRomanization):
                timbre.CTimbreReflexSet(id, CReflexField.CReflexFieldRomanization, row.QReflexItemRomanization);
                break;
            case nameof(QReflexItem.QReflexItemMeaning):
                timbre.CTimbreReflexSet(id, CReflexField.CReflexFieldMeaning, row.QReflexItemMeaning);
                break;
            case nameof(QReflexItem.QReflexItemNote):
                timbre.CTimbreReflexSet(id, CReflexField.CReflexFieldNote, row.QReflexItemNote);
                break;
        }
    }

    internal void PReflexRefine(CEntryDraft _)
    {
        PReflexRefine(_qEditor.QEditorArea.CEditorTimbre.CTimbreReflexRead());
    }

    private void PReflexRefine(CTimbreReflex reflex)
    {
        PReflexBlock.Visibility = QLook.QLookVisibleRead(reflex.CTimbreReflexShown);
        PReflexRenewal.Visibility = PReflexBlock.Visibility;
        PCard.PCardRowShow(
            _pReflexItem,
            reflex.CTimbreReflexRows,
            static row => row.QReflexItemId,
            static row => row.CReflexId,
            PReflexRowRefine,
            PReflexStateRefine);
        QReflexItem.QReflexFoldRefine(_pReflexItem, PReflexFold, reflex.CTimbreReflexOpened);
        PReflexAnchorRefine(reflex.CTimbreReflexAnchor);
        PReflexPendingRefine(reflex.CTimbreReflexPending);
    }

    internal void PReflexAnchorRefine()
    {
        PReflexAnchorRefine(_qEditor.QEditorArea.CEditorTimbre.CTimbreReflexRead().CTimbreReflexAnchor);
    }

    private void PReflexAnchorRefine(CLecternAnchor anchor)
    {
        QReflexItem.QReflexAnchorRefine(_pReflexItem, anchor.CLecternAnchorOffered, anchor.CLecternAnchorTexts);
    }

    internal void PReflexPendingRefine()
    {
        PReflexPendingRefine(_qEditor.QEditorArea.CEditorTimbre.CTimbreReflexPending);
    }

    private void PReflexPendingRefine(bool pending)
    {
        if (!pending)
        {
            PReflexTable.MinWidth = 0;
            PReflexTable.MinHeight = 0;
        }

        PReflexLoading.Visibility = QLook.QLookVisibleRead(pending);
        PReflexRenewal.SetValue(
            QLook.QLookCueProperty,
            QLook.QLookFirstRead(pending, QLookCue.QLookCuePending, QLookCue.QLookCueBase));
    }

    private void PReflexFoldObserve(object sender, RoutedEventArgs e)
    {
        _qEditor.QEditorArea.CEditorDisplay.CDisplaySound.CDisplayReflexToggle(
            QLook.QLookCheckedRead(PReflexFold.IsChecked));
    }

    internal void PReflexFoldRefine()
    {
        QReflexItem.QReflexFoldRefine(
            _pReflexItem, PReflexFold, _qEditor.QEditorArea.CEditorDisplay.CDisplaySound.CDisplayFoldOpened);
    }

    private QReflexItem PReflexRowRefine(CReflex reflex)
    {
        QReflexItem row = new(reflex);
        row.PropertyChanged += PReflexChangeObserve;
        return row;
    }

    private static QReflexItem PReflexStateRefine(QReflexItem row, CReflex reflex)
    {
        row.QReflexStateRefine(reflex);
        return row;
    }
}
