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
    private readonly ObservableCollection<PReflexItem> _pReflexItem = [];

    private HashSet<string> _pReflexFolded = [];

    internal void PReflexAddHandle(object sender, ExecutedRoutedEventArgs e)
    {
        PReflexItem? row = e.Parameter as PReflexItem;
        int position = row is null ? _pReflexItem.Count : _pReflexItem.IndexOf(row) + 1;
        PEditorRequestSend(new LRequestReflexAddition(
            PEditorDraft, row?.PReflexItemLanguage ?? string.Empty, row?.PReflexItemKind ?? string.Empty, position));
    }

    internal void PReflexRemoveHandle(object sender, ExecutedRoutedEventArgs e)
    {
        if (e.Parameter is PReflexItem row)
        {
            PEditorRequestSend(new LRequestReflexRemoval(PEditorDraft, row.PReflexItemId));
        }
    }

    internal void PReflexMainHandle(object sender, ExecutedRoutedEventArgs e)
    {
        if (e.Parameter is PReflexItem row)
        {
            PEditorRequestSend(new LRequestReflexMain(PEditorDraft, row.PReflexItemId, !row.PReflexItemMain));
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
            _lEditor.LEditorDisplay.LDisplayReflexRebuild(entry);
        }
        catch (Exception)
        {
        }

        PReflexPendingShow();
    }

    private void PReflexChangeHandle(object? sender, PropertyChangedEventArgs e)
    {
        if (sender is not PReflexItem row)
        {
            return;
        }

        string field = e.PropertyName ?? string.Empty;
        LRequest? request = field switch
        {
            nameof(PReflexItem.PReflexItemLanguage) =>
                new LRequestReflexLanguage(PEditorDraft, row.PReflexItemId, row.PReflexItemLanguage),
            nameof(PReflexItem.PReflexItemKind) =>
                new LRequestReflexKind(PEditorDraft, row.PReflexItemId, row.PReflexItemKind),
            nameof(PReflexItem.PReflexItemText) => row.PReflexItemRespelled
                ? new LRequestReflexRespelling(PEditorDraft, row.PReflexItemId, row.PReflexItemText)
                : new LRequestReflexText(PEditorDraft, row.PReflexItemId, row.PReflexItemText),
            nameof(PReflexItem.PReflexItemRomanization) =>
                new LRequestReflexRomanization(PEditorDraft, row.PReflexItemId, row.PReflexItemRomanization),
            nameof(PReflexItem.PReflexItemMeaning) =>
                new LRequestReflexMeaning(PEditorDraft, row.PReflexItemId, row.PReflexItemMeaning),
            nameof(PReflexItem.PReflexItemNote) =>
                new LRequestReflexNote(PEditorDraft, row.PReflexItemId, row.PReflexItemNote),
            _ => null,
        };

        if (request is null)
        {
            return;
        }

        if (string.Equals(field, nameof(PReflexItem.PReflexItemLanguage), StringComparison.Ordinal))
        {
            PDisplay.PReflexLeadApply(_pReflexItem);
        }

        PEditorRequestDefer(request);
    }

    private void PReflexShow(LEntryDraft draft)
    {
        string language = draft.LEntryDraftLanguage;
        PReflexBlock.Visibility = PLook.PLookVisibleRead(_lEditor.LEditorReflexShown);

        _pReflexFolded = PDisplay.PReflexFoldRead(_pEditorHost.PWindowDeportment, language);

        PCard.PCardRowShow(
            _pReflexItem,
            draft.LEntryDraftReflexes,
            static row => row.PReflexItemId,
            static reflex => reflex.LReflexDraftId,
            PReflexCreate,
            PReflexUpdate);

        PDisplay.PReflexLeadApply(_pReflexItem);
        PDisplay.PReflexFoldApply(_pReflexItem, PReflexFold, _lEditor.LEditorDisplay.LDisplayFoldOpened);
        PReflexAnchorShow();
        PReflexPendingShow();
    }

    internal void PReflexAnchorShow()
    {
        LWindow window = _pEditorHost.PWindowDeportment;
        PDisplay.PReflexAnchorApply(window, _pReflexItem, _lEditor.LEditorAnchorRead(), PHeadword.Text);
    }

    internal void PReflexPendingShow()
    {
        bool pending = false;
        if (_lEditor.LEditorEntry is long entry)
        {
            try
            {
                pending = _lEditor.LEditorDisplay.LDisplayReflexCheck(entry);
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
        _lEditor.LEditorDisplay.LDisplayFoldSet(PLook.PLookCheckedRead(PReflexFold.IsChecked));
        PDisplay.PReflexFoldApply(_pReflexItem, PReflexFold, _lEditor.LEditorDisplay.LDisplayFoldOpened);
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
            _lEditor.LEditorDisplay.LDisplayReflexStart(entry.Value);
        }
        catch (Exception)
        {
        }

        PReflexPendingShow();
    }

    private PReflexItem PReflexCreate(LReflexDraft reflex)
    {
        PReflexItem row = PDisplay.PReflexItemCreate(_pEditorHost, reflex, _pReflexFolded);
        row.PropertyChanged += PReflexChangeHandle;
        return row;
    }

    private PReflexItem PReflexUpdate(PReflexItem row, LReflexDraft reflex)
    {
        string language = reflex.LReflexDraftLanguage.Trim();
        PRespelling respelling = PRespelling.PRespellingRead(_pEditorHost.PWindowDeportment, language);
        if (!row.PReflexItemMatch(
            respelling.PRespellingShown,
            _pEditorHost.PWindowDeportment.LWindowPhonemicCheck(language),
            _pReflexFolded.Contains(language)))
        {
            row.PropertyChanged -= PReflexChangeHandle;
            return PReflexCreate(reflex);
        }

        row.PReflexItemMain = reflex.LReflexDraftMain;

        row.PReflexItemLanguage = reflex.LReflexDraftLanguage;
        row.PReflexItemKind = reflex.LReflexDraftKind;
        row.PReflexItemText = PReflexItem.PReflexTextRead(reflex, respelling);
        row.PReflexItemRomanization = reflex.LReflexDraftRomanization;
        row.PReflexItemMeaning = reflex.LReflexDraftMeaning;
        row.PReflexItemNote = reflex.LReflexDraftNote;
        row.PReflexItemRegion = reflex.LReflexDraftRegion;

        row.PReflexItemTone = reflex.LReflexDraftAnatomy.LAnatomyToneIpa;
        row.PReflexItemAnchors = reflex.LReflexDraftAnchors;
        return row;
    }
}
