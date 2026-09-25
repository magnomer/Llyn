using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using Llyn.Application;
using Llyn.Core;
using Llyn.ShellEngine;
using Llyn.UIDeportment;

namespace Llyn.UIVeneer;

public partial class PEditor
{
    private readonly ObservableCollection<LReflexItem> _pReflexItem = [];

    private HashSet<string> _pReflexFolded = [];

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
            _lEditor.LEditorLectern.LLecternReflexRebuild(entry);
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

    private void PReflexShow(LEntryDraft draft)
    {
        string language = draft.LEntryDraftLanguage;
        PReflexBlock.Visibility = PLook.PLookVisibleRead(_lEditor.LEditorReflexShown);

        _pReflexFolded = LReflexItem.LReflexFoldRead(_pEditorHost.PWindowDeportment, language);

        PCard.PCardRowShow(
            _pReflexItem,
            draft.LEntryDraftReflexes,
            static row => row.LReflexItemId,
            static reflex => reflex.LReflexDraftId,
            PReflexCreate,
            PReflexUpdate);

        LReflexItem.LReflexLeadApply(_pReflexItem);
        LReflexItem.LReflexFoldApply(_pReflexItem, PReflexFold, _lEditor.LEditorLectern.LLecternFoldOpened);
        PReflexAnchorShow();
        PReflexPendingShow();
    }

    internal void PReflexAnchorShow()
    {
        LWindow window = _pEditorHost.PWindowDeportment;
        LReflexItem.LReflexAnchorApply(window, _pReflexItem, _lEditor.LEditorAnchorRead(), PHeadword.Text);
    }

    internal void PReflexPendingShow()
    {
        bool pending = false;
        if (_lEditor.LEditorEntry is long entry)
        {
            try
            {
                pending = _lEditor.LEditorLectern.LLecternReflexCheck(entry);
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
        PReflexRenewal.Tag = pending;
    }

    internal void PReflexFoldHandle(object sender, RoutedEventArgs e)
    {
        _lEditor.LEditorLectern.LLecternFoldSet(PLook.PLookCheckedRead(PReflexFold.IsChecked));
        LReflexItem.LReflexFoldApply(_pReflexItem, PReflexFold, _lEditor.LEditorLectern.LLecternFoldOpened);
    }

    private void PReflexPrepare(LEntryDraft draft)
    {
        if (draft.LEntryDraftReflected)
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
            _lEditor.LEditorLectern.LLecternReflexStart(entry.Value);
        }
        catch (Exception)
        {
        }

        PReflexPendingShow();
    }

    private LReflexItem PReflexCreate(LReflexDraft reflex)
    {
        LReflexItem row = LReflexItem.LReflexItemCreate(_pEditorHost.PWindowDeportment, reflex, _pReflexFolded);
        row.PropertyChanged += PReflexChangeHandle;
        return row;
    }

    private LReflexItem PReflexUpdate(LReflexItem row, LReflexDraft reflex)
    {
        string language = reflex.LReflexDraftLanguage.Trim();
        LRespellingMark respelling = LRespellingMark.LRespellingMarkRead(_pEditorHost.PWindowDeportment, language);
        if (!row.LReflexItemMatch(
            respelling.LRespellingMarkShown,
            _pEditorHost.PWindowDeportment.LWindowPhonemicCheck(language),
            _pReflexFolded.Contains(language),
            reflex.LReflexDraftAnchors))
        {
            row.PropertyChanged -= PReflexChangeHandle;
            return PReflexCreate(reflex);
        }

        row.LReflexItemMain = reflex.LReflexDraftMain;

        row.LReflexItemLanguage = reflex.LReflexDraftLanguage;
        row.LReflexItemKind = reflex.LReflexDraftKind;
        row.LReflexItemText = LReflexItem.LReflexTextRead(reflex, respelling);
        row.LReflexItemRomanization = reflex.LReflexDraftRomanization;
        row.LReflexItemMeaning = reflex.LReflexDraftMeaning;
        row.LReflexItemNote = reflex.LReflexDraftNote;
        row.LReflexItemRegion = reflex.LReflexDraftRegion;

        row.LReflexItemTone = reflex.LReflexDraftAnatomy.LAnatomyToneIpa;
        return row;
    }
}
