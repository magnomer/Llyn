using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Llyn.Application;
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
            PReflexCommand.PReflexCommandAddition, PReflexAddHandle));
        PEditorSound.CommandBindings.Add(new CommandBinding(
            PReflexCommand.PReflexCommandRemoval, PReflexRemoveHandle));
        PEditorSound.CommandBindings.Add(new CommandBinding(
            PReflexCommand.PReflexCommandMain, PReflexMainHandle));
        PEditorSound.CommandBindings.Add(new CommandBinding(
            PReflexCommand.PReflexCommandAnchor, PReflexAnchorObserve));
        PReflexRenewal.CommandBindings.Add(new CommandBinding(
            PReflexCommand.PReflexCommandRenewal, PReflexRebuildHandle));
        PReflexFold.Checked += PReflexFoldHandle;
        PReflexFold.Unchecked += PReflexFoldHandle;
    }

    internal void PReflexAddHandle(object sender, ExecutedRoutedEventArgs e)
    {
        QReflexItem? row = e.Parameter as QReflexItem;
        int position = row is null ? _pReflexItem.Count : _pReflexItem.IndexOf(row) + 1;
        PEditorRequestSend(new LRequestReflexAddition(
            PEditorDraft, row?.QReflexItemLanguage ?? string.Empty, row?.QReflexItemKind ?? string.Empty, position));
    }

    internal void PReflexRemoveHandle(object sender, ExecutedRoutedEventArgs e)
    {
        if (e.Parameter is QReflexItem row)
        {
            PEditorRequestSend(new LRequestReflexRemoval(PEditorDraft, row.QReflexItemId));
        }
    }

    internal void PReflexMainHandle(object sender, ExecutedRoutedEventArgs e)
    {
        if (e.Parameter is QReflexItem row)
        {
            PEditorRequestSend(new LRequestReflexMain(PEditorDraft, row.QReflexItemId, !row.QReflexItemMain));
        }
    }

    internal void PReflexRebuildHandle(object sender, ExecutedRoutedEventArgs e)
    {
        if (_qEditor.QEditorArea.CEditorEntry is null)
        {
            return;
        }

        PReflexTable.MinWidth = PReflexTable.ActualWidth;
        PReflexTable.MinHeight = PReflexTable.ActualHeight;
        try
        {
            _qEditor.QEditorArea.CEditorTimbre.CTimbreReflexRebuild();
        }
        catch (Exception)
        {
        }

        PReflexPendingShow();
    }

    private void PReflexChangeHandle(object? sender, PropertyChangedEventArgs e)
    {
        if (sender is not QReflexItem row)
        {
            return;
        }

        string field = e.PropertyName ?? string.Empty;
        LRequest? request = field switch
        {
            nameof(QReflexItem.QReflexItemLanguage) =>
                new LRequestReflexLanguage(PEditorDraft, row.QReflexItemId, row.QReflexItemLanguage),
            nameof(QReflexItem.QReflexItemKind) =>
                new LRequestReflexKind(PEditorDraft, row.QReflexItemId, row.QReflexItemKind),
            nameof(QReflexItem.QReflexItemText) => row.QReflexItemRespelled
                ? new LRequestReflexRespelling(PEditorDraft, row.QReflexItemId, row.QReflexItemText)
                : new LRequestReflexText(PEditorDraft, row.QReflexItemId, row.QReflexItemText),
            nameof(QReflexItem.QReflexItemRomanization) =>
                new LRequestReflexRomanization(PEditorDraft, row.QReflexItemId, row.QReflexItemRomanization),
            nameof(QReflexItem.QReflexItemMeaning) =>
                new LRequestReflexMeaning(PEditorDraft, row.QReflexItemId, row.QReflexItemMeaning),
            nameof(QReflexItem.QReflexItemNote) =>
                new LRequestReflexNote(PEditorDraft, row.QReflexItemId, row.QReflexItemNote),
            _ => null,
        };

        if (request is null)
        {
            return;
        }

        if (string.Equals(field, nameof(QReflexItem.QReflexItemLanguage), StringComparison.Ordinal))
        {
            QReflexItem.QReflexLeadRefine(
                _pReflexItem, _qEditor.QEditorArea.CEditorLeadRead(row.QReflexItemId, row.QReflexItemLanguage));
        }

        PEditorRequestDefer(request);
    }

    internal void PReflexShow(CEntryDraft draft)
    {
        string language = draft.CEntryDraftLanguage;
        PReflexBlock.Visibility = QLook.QLookVisibleRead(_qEditor.QEditorArea.CEditorTimbre.CTimbreReflexShown);
        PReflexRenewal.Visibility = PReflexBlock.Visibility;

        PCard.PCardRowShow(
            _pReflexItem,
            _pEditorHost.PWindowAtelier.CAtelierRespelling.CRespellingReflexScan(language, draft.CEntryDraftReflexes),
            static row => row.QReflexItemId,
            static reflex => reflex.CReflexId,
            PReflexCreate,
            PReflexUpdate);

        QReflexItem.QReflexFoldRefine(
            _pReflexItem, PReflexFold, _qEditor.QEditorArea.CEditorDisplay.LDisplaySound.LDisplayFoldOpened);
        PReflexAnchorShow();
        PReflexPendingShow();
    }

    internal void PReflexAnchorShow()
    {
        string headword = PHeadword.Text;
        QReflexItem.QReflexAnchorRefine(
            _pReflexItem,
            _qEditor.QEditorArea.CEditorSounding.CSoundingAnchorCheck(headword),
            anchors => _qEditor.QEditorArea.CEditorSounding.CSoundingAnchorFormat(anchors, headword));
    }

    internal void PReflexPendingShow()
    {
        bool pending = false;
        try
        {
            pending = _qEditor.QEditorArea.CEditorTimbre.CTimbreReflexPending;
        }
        catch (Exception)
        {
        }

        if (!pending)
        {
            PReflexTable.MinWidth = 0;
            PReflexTable.MinHeight = 0;
        }

        PReflexLoading.Visibility = pending ? Visibility.Visible : Visibility.Collapsed;
        PReflexRenewal.SetValue(
            QLook.QLookCueProperty,
            QLook.QLookFirstRead(pending, QLookCue.QLookCuePending, QLookCue.QLookCueBase));
    }

    internal void PReflexFoldHandle(object sender, RoutedEventArgs e)
    {
        _qEditor.QEditorArea.CEditorDisplay.CDisplaySound.CDisplayReflexToggle(
            QLook.QLookCheckedRead(PReflexFold.IsChecked));
        QReflexItem.QReflexFoldRefine(
            _pReflexItem, PReflexFold, _qEditor.QEditorArea.CEditorDisplay.LDisplaySound.LDisplayFoldOpened);
    }

    internal void PReflexPrepare(CEntryDraft draft)
    {
        if (draft.CEntryDraftReflected)
        {
            return;
        }

        if (_qEditor.QEditorArea.CEditorEntry is null)
        {
            return;
        }

        try
        {
            _qEditor.QEditorArea.CEditorTimbre.CTimbreReflexStart();
        }
        catch (Exception)
        {
        }

        PReflexPendingShow();
    }

    private QReflexItem PReflexCreate(CReflex reflex)
    {
        QReflexItem row = new(reflex);
        row.PropertyChanged += PReflexChangeHandle;
        return row;
    }

    private static QReflexItem PReflexUpdate(QReflexItem row, CReflex reflex)
    {
        row.QReflexStateRefine(reflex);
        return row;
    }
}
