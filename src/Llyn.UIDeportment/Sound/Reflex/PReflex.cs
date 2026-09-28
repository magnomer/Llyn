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
    private readonly ObservableCollection<LReflexItem> _pReflexItem = [];

    private HashSet<string> _pReflexFolded = [];

    private Grid PReflexBlock => (Grid)FindName(nameof(PReflexBlock));

    private StackPanel PReflexTable => (StackPanel)FindName(nameof(PReflexTable));

    private PReflexList PReflex => (PReflexList)FindName(nameof(PReflex));

    private TextBlock PReflexLoading => (TextBlock)FindName(nameof(PReflexLoading));

    private ToggleButton PReflexFold => (ToggleButton)FindName(nameof(PReflexFold));

    private Button PReflexRenewal => (Button)FindName(nameof(PReflexRenewal));

    private void PReflexAttach()
    {
        PReflex.ItemsSource = _pReflexItem;
        QLookItem.QLookItemAttach(PReflex, LReflexItem.LReflexItemApply);
        QField.QFieldCellAttach(PReflex);
        PAccentControl.PAccentControlAttach(PReflex);
        PEditorSound.CommandBindings.Add(new CommandBinding(
            PReflexCommand.PReflexCommandAddition, PReflexAddHandle));
        PEditorSound.CommandBindings.Add(new CommandBinding(
            PReflexCommand.PReflexCommandRemoval, PReflexRemoveHandle));
        PEditorSound.CommandBindings.Add(new CommandBinding(
            PReflexCommand.PReflexCommandMain, PReflexMainHandle));
        PEditorSound.CommandBindings.Add(new CommandBinding(
            PReflexCommand.PReflexCommandAnchor, PReflexAnchorHandle));
        PReflexRenewal.CommandBindings.Add(new CommandBinding(
            PReflexCommand.PReflexCommandRenewal, PReflexRebuildHandle));
        PReflexFold.Checked += PReflexFoldHandle;
        PReflexFold.Unchecked += PReflexFoldHandle;
    }

    internal void PReflexAddHandle(object sender, ExecutedRoutedEventArgs e)
    {
        LReflexItem? row = e.Parameter as LReflexItem;
        int position = row is null ? _pReflexItem.Count : _pReflexItem.IndexOf(row) + 1;
        PEditorRequestSend(new LRequestReflexAddition(
            PEditorDraft, row?.LReflexItemLanguage ?? string.Empty, row?.LReflexItemKind ?? string.Empty, position));
    }

    internal void PReflexRemoveHandle(object sender, ExecutedRoutedEventArgs e)
    {
        if (e.Parameter is LReflexItem row)
        {
            PEditorRequestSend(new LRequestReflexRemoval(PEditorDraft, row.LReflexItemId));
        }
    }

    internal void PReflexMainHandle(object sender, ExecutedRoutedEventArgs e)
    {
        if (e.Parameter is LReflexItem row)
        {
            PEditorRequestSend(new LRequestReflexMain(PEditorDraft, row.LReflexItemId, !row.LReflexItemMain));
        }
    }

    internal void PReflexRebuildHandle(object sender, ExecutedRoutedEventArgs e)
    {
        if (_lEditor.LEditorEntry is not long entry)
        {
            return;
        }

        PReflexTable.MinWidth = PReflexTable.ActualWidth;
        PReflexTable.MinHeight = PReflexTable.ActualHeight;
        try
        {
            _pEditorLectern.LLecternReflexRebuild(entry);
        }
        catch (Exception)
        {
        }

        PReflexPendingShow();
    }

    private void PReflexChangeHandle(object? sender, PropertyChangedEventArgs e)
    {
        if (sender is not LReflexItem row)
        {
            return;
        }

        string field = e.PropertyName ?? string.Empty;
        LRequest? request = field switch
        {
            nameof(LReflexItem.LReflexItemLanguage) =>
                new LRequestReflexLanguage(PEditorDraft, row.LReflexItemId, row.LReflexItemLanguage),
            nameof(LReflexItem.LReflexItemKind) =>
                new LRequestReflexKind(PEditorDraft, row.LReflexItemId, row.LReflexItemKind),
            nameof(LReflexItem.LReflexItemText) => row.LReflexItemRespelled
                ? new LRequestReflexRespelling(PEditorDraft, row.LReflexItemId, row.LReflexItemText)
                : new LRequestReflexText(PEditorDraft, row.LReflexItemId, row.LReflexItemText),
            nameof(LReflexItem.LReflexItemRomanization) =>
                new LRequestReflexRomanization(PEditorDraft, row.LReflexItemId, row.LReflexItemRomanization),
            nameof(LReflexItem.LReflexItemMeaning) =>
                new LRequestReflexMeaning(PEditorDraft, row.LReflexItemId, row.LReflexItemMeaning),
            nameof(LReflexItem.LReflexItemNote) =>
                new LRequestReflexNote(PEditorDraft, row.LReflexItemId, row.LReflexItemNote),
            _ => null,
        };

        if (request is null)
        {
            return;
        }

        if (string.Equals(field, nameof(LReflexItem.LReflexItemLanguage), StringComparison.Ordinal))
        {
            LReflexItem.LReflexLeadApply(_pReflexItem);
        }

        PEditorRequestDefer(request);
    }

    private void PReflexShow(CEntryDraft draft)
    {
        string language = draft.CEntryDraftLanguage;
        PReflexBlock.Visibility = QLook.QLookVisibleRead(_lEditor.LEditorReflexShown);
        PReflexRenewal.Visibility = PReflexBlock.Visibility;

        _pReflexFolded = LReflexItem.LReflexFoldRead(_pEditorHost.PWindowAtelier, language);

        PCard.PCardRowShow(
            _pReflexItem,
            draft.CEntryDraftReflexes,
            static row => row.LReflexItemId,
            static reflex => reflex.CReflexDraftId,
            PReflexCreate,
            PReflexUpdate);

        LReflexItem.LReflexLeadApply(_pReflexItem);
        LReflexItem.LReflexFoldApply(_pReflexItem, PReflexFold, _pEditorLectern.LLecternFoldOpened);
        PReflexAnchorShow();
        PReflexPendingShow();
    }

    internal void PReflexAnchorShow()
    {
        string headword = PHeadword.Text;
        LReflexItem.LReflexAnchorApply(
            _pReflexItem,
            _lEditor.LEditorStudio.CEditorSounding.CSoundingAnchorCheck(headword),
            (anchors, separator) =>
                _lEditor.LEditorStudio.CEditorSounding.CSoundingAnchorFormat(anchors, headword, separator));
    }

    internal void PReflexPendingShow()
    {
        bool pending = false;
        if (_lEditor.LEditorEntry is long entry)
        {
            try
            {
                pending = _pEditorLectern.LLecternReflexCheck(entry);
            }
            catch (Exception)
            {
            }
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
        _pEditorLectern.LLecternFoldSet(QLook.QLookCheckedRead(PReflexFold.IsChecked));
        LReflexItem.LReflexFoldApply(_pReflexItem, PReflexFold, _pEditorLectern.LLecternFoldOpened);
    }

    private void PReflexPrepare(CEntryDraft draft)
    {
        if (draft.CEntryDraftReflected)
        {
            return;
        }

        long? entry = _lEditor.LEditorEntry;
        if (entry is null)
        {
            return;
        }

        try
        {
            _pEditorLectern.LLecternReflexStart(entry.Value);
        }
        catch (Exception)
        {
        }

        PReflexPendingShow();
    }

    private LReflexItem PReflexCreate(CReflexDraft reflex)
    {
        LReflexItem row = LReflexItem.LReflexItemCreate(_pEditorHost.PWindowAtelier, reflex, _pReflexFolded);
        row.PropertyChanged += PReflexChangeHandle;
        return row;
    }

    private LReflexItem PReflexUpdate(LReflexItem row, CReflexDraft reflex)
    {
        return PReflexUpdate(
            row,
            reflex,
            _pEditorHost.PWindowAtelier.CAtelierRespelling.CRespellingReflexRead(
                reflex.CReflexDraftLanguage.Trim()));
    }

    private LReflexItem PReflexUpdate(LReflexItem row, CReflexDraft reflex, CRespellingMark respelling)
    {
        string language = reflex.CReflexDraftLanguage.Trim();
        if (!row.LReflexItemMatch(
            respelling,
            _pReflexFolded.Contains(language),
            reflex.CReflexDraftAnchors))
        {
            row.PropertyChanged -= PReflexChangeHandle;
            return PReflexCreate(reflex);
        }

        row.LReflexItemMain = reflex.CReflexDraftMain;

        row.LReflexItemLanguage = reflex.CReflexDraftLanguage;
        row.LReflexItemKind = reflex.CReflexDraftKind;
        row.LReflexItemText = LReflexItem.LReflexTextRead(reflex, respelling);
        row.LReflexItemRomanization = reflex.CReflexDraftRomanization;
        row.LReflexItemMeaning = reflex.CReflexDraftMeaning;
        row.LReflexItemNote = reflex.CReflexDraftNote;
        row.LReflexItemRegion = reflex.CReflexDraftRegion;

        row.LReflexItemTone = reflex.CReflexDraftTone;
        return row;
    }
}
