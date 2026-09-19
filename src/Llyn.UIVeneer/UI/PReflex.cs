using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using Llyn.Application;
using Llyn.Core;
using Llyn.ShellEngine;

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
            _lEngine.LEngineReflexRebuild(entry);
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
            nameof(PReflexItem.PReflexItemNote) =>
                new LRequestReflexNote(PEditorDraft, row.PReflexItemId, row.PReflexItemNote),
            nameof(PReflexItem.PReflexItemRemark) =>
                new LRequestReflexRemark(PEditorDraft, row.PReflexItemId, row.PReflexItemRemark),
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

        _pReflexFolded = PDisplay.PReflexFoldRead(_lEngine, language);

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
        PDisplay.PReflexAnchorApply(_pReflexItem, _lEditor.LEditorAnchorRead(), PHeadword.Text);
    }

    internal void PReflexPendingShow()
    {
        bool pending = false;
        if (_lEditor.LEditorEntry is long entry)
        {
            try
            {
                pending = _lEngine.LEngineReflexCheck(entry);
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
            _lEngine.LEngineReflexStart(entry.Value);
        }
        catch (Exception)
        {
        }

        PReflexPendingShow();
    }

    private PReflexItem PReflexCreate(LReflexDraft reflex)
    {
        PReflexItem row = PDisplay.PReflexItemCreate(_pEditorHost, _lEngine, reflex, _pReflexFolded);
        row.PropertyChanged += PReflexChangeHandle;
        return row;
    }

    private PReflexItem PReflexUpdate(PReflexItem row, LReflexDraft reflex)
    {
        string language = reflex.LReflexDraftLanguage.Trim();
        PRespelling respelling = PRespelling.PRespellingRead(_lEngine, language);
        if (!row.PReflexItemMatch(
            respelling.PRespellingShown, _lEngine.LEnginePhonemicCheck(language), _pReflexFolded.Contains(language)))
        {
            row.PropertyChanged -= PReflexChangeHandle;
            return PReflexCreate(reflex);
        }

        row.PReflexItemMain = reflex.LReflexDraftMain;

        row.PReflexItemLanguage = reflex.LReflexDraftLanguage;
        row.PReflexItemKind = reflex.LReflexDraftKind;
        row.PReflexItemText = PReflexItem.PReflexTextRead(reflex, respelling);
        row.PReflexItemNote = reflex.LReflexDraftNote;
        row.PReflexItemRegion = reflex.LReflexDraftRegion;
        row.PReflexItemRemark = reflex.LReflexDraftRemark;

        row.PReflexItemTone = reflex.LReflexDraftAnatomy.LAnatomyToneIpa;
        row.PReflexItemAnchors = reflex.LReflexDraftAnchors;
        return row;
    }

    private void PReflexClear()
    {
        foreach (PReflexItem row in _pReflexItem)
        {
            row.PropertyChanged -= PReflexChangeHandle;
        }

        _pReflexItem.Clear();
        PReflexFold.Visibility = Visibility.Collapsed;
        PReflexLoading.Visibility = Visibility.Collapsed;
        PReflexRenewal.Tag = false;
        PReflexTable.MinWidth = 0;
        PReflexTable.MinHeight = 0;
        PReflexBlock.Visibility = Visibility.Collapsed;
    }
}
