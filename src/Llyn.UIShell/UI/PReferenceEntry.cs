using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using Llyn.Core;

namespace Llyn.UIShell;

public partial class PReference
{
    private readonly ObservableCollection<PFootnoteItem> _pFootnoteList = [];

    private long? _pDisplayEntry;

    private void PFootnoteFind()
    {
        IReadOnlyList<LEntry> read;
        try
        {
            read = _lEngine.LEngineEntryFind(
                new LReference(
                    _pColophonReference ?? 0,
                    LStateValue.LStateValueUnspecified,
                    LStateValue.LStateValueUnspecified,
                    LReferenceKind.LReferenceKindUnspecified,
                    LStateValue.LStateValueUnspecified,
                    LStateValue.LStateValueUnspecified,
                    LStateMark.LStateMarkUnspecified),
                PRummage.Text ?? string.Empty,
                _pTrellisChoice);
        }
        catch (Exception exception)
        {
            _pReferenceHost.PWindowFailureShow("Source.LoadFailed", exception);
            return;
        }

        _pFootnoteList.Clear();
        foreach (LEntry entry in read)
        {
            _pFootnoteList.Add(new PFootnoteItem(
                entry.LEntryId,
                entry.LEntryHeadword,
                entry.LEntryLanguage,
                _lEngine.LEngineEpithetRead(entry.LEntryId)));
        }

        PTwin.PTwinNameApply(
            _pFootnoteList,
            row => row.PFootnoteItemHeadword,
            (row, name) => row.PFootnoteItemName = name,
            row => row.PFootnoteItemId);

        PFootnoteEmpty.SetResourceReference(
            TextBlock.TextProperty,
            string.IsNullOrWhiteSpace(PRummage.Text) ? "Source.Vacant" : "Source.Unmatched");
        PFootnoteEmpty.Visibility = _pFootnoteList.Count == 0 ? Visibility.Visible : Visibility.Collapsed;

        PFootnoteSelect(_pDisplayEntry);
    }

    private void PFootnoteSelect(long? id)
    {
        foreach (PFootnoteItem item in _pFootnoteList)
        {
            item.PFootnoteItemChosen = id is not null
                && item.PFootnoteItemId == id;
        }
    }

    private void PFootnoteHandle(object sender, RoutedEventArgs e)
    {
        if (e.OriginalSource is not FrameworkElement row || row.DataContext is not PFootnoteItem item)
        {
            return;
        }

        if (!PReferenceLeaveConfirm())
        {
            return;
        }

        PFootnoteEntryShow(item.PFootnoteItemId);
    }

    private void PFootnoteEntryShow(long id)
    {
        LEntryDraft? draft;
        try
        {
            draft = _lEngine.LEngineEntryLoad(id);
        }
        catch (Exception exception)
        {
            _pReferenceHost.PWindowFailureShow("List.LoadFailed", exception);
            return;
        }

        if (draft is null)
        {
            PFootnoteEntryHide();
            PFootnoteFind();
            return;
        }

        bool editing = PImprint.Visibility == Visibility.Visible
            || PEditor.Visibility == Visibility.Visible;
        PImprint.PImprintDraftCancel();
        PImprint.Visibility = Visibility.Collapsed;
        PColophon.Visibility = Visibility.Collapsed;

        _pDisplayEntry = id;
        PFootnoteSelect(id);
        PDisplay.PDisplayShow(id, draft);
        PReferenceMode.IsEnabled = true;
        PReferenceBin.IsEnabled = false;

        if (editing)
        {
            PEditor.PEditorEntryShow(id);
        }

        PFootnoteScribeShow(editing);
    }

    private void PFootnoteEntryCreate()
    {
        long? reference = _pColophonReference;

        PImprint.PImprintDraftCancel();
        PImprint.Visibility = Visibility.Collapsed;
        PColophon.Visibility = Visibility.Collapsed;

        _pDisplayEntry = null;
        PFootnoteSelect(null);
        PDisplay.PDisplayClear();
        PEditor.PEditorReset();
        PReferenceMode.IsEnabled = true;
        PReferenceBin.IsEnabled = false;
        PFootnoteScribeShow(true);

        if (reference is long id)
        {
            PEditor.PEditorReferenceAdd(id);
        }
    }

    private void PFootnoteScribeHandle(bool editing)
    {
        if (_pDisplayEntry is not long id)
        {
            if (!editing)
            {
                PFootnoteScribeReset();
            }

            return;
        }

        if (editing == (PEditor.Visibility == Visibility.Visible))
        {
            return;
        }

        if (!editing)
        {
            if (!PReferenceLeaveConfirm())
            {
                PFootnoteScribeShow(true);
                return;
            }

            PFootnoteScribeShow(false);
            PFootnoteEntryShow(id);
            return;
        }

        PEditor.PEditorEntryShow(id);
        PFootnoteScribeShow(true);
    }

    private void PFootnoteScribeReset()
    {
        if (!PReferenceLeaveConfirm())
        {
            PFootnoteScribeShow(true);
            return;
        }

        PFootnoteScribeShow(false);

        if (_pDisplayEntry is long stored)
        {
            PFootnoteEntryShow(stored);
            return;
        }

        PEditor.PEditorReset();

        if (_pColophonReference is long kept)
        {
            PReferenceShow(kept);
            return;
        }

        PReferenceClear();
    }

    private void PFootnoteScribeShow(bool editing)
    {
        _lEngine.LEngineSplitSave(editing);

        PEditor.Visibility = editing ? Visibility.Visible : Visibility.Collapsed;
        PDisplay.Visibility = editing ? Visibility.Collapsed : Visibility.Visible;
        PReferenceViewer.IsChecked = !editing;
        PReferenceScribe.IsChecked = editing;
    }

    private void PReferenceStoreHandle(object sender, RoutedEventArgs e)
    {
        if (PEditor.Visibility == Visibility.Visible)
        {
            PEditor.PEditorEntrySave();
            return;
        }

        PImprint.PImprintStoreRun();
    }

    private void PFootnoteEntryUpdate(long id)
    {
        if (_pDisplayEntry is not long shown
            || (id > 0 && shown != id))
        {
            return;
        }

        LEntryDraft? draft;
        try
        {
            draft = _lEngine.LEngineEntryLoad(shown);
        }
        catch (Exception)
        {
            return;
        }

        if (draft is null)
        {
            if (_pColophonReference is long kept)
            {
                PReferenceShow(kept);
                return;
            }

            PReferenceClear();
            return;
        }

        PDisplay.PDisplayShow(shown, draft);
    }

    private void PFootnoteEntryHide()
    {
        bool editing = PImprint.Visibility == Visibility.Visible
            || PEditor.Visibility == Visibility.Visible;
        if (PEditor.Visibility == Visibility.Visible)
        {
            PEditor.PEditorReset();
        }

        _pDisplayEntry = null;
        PFootnoteSelect(null);
        PDisplay.PDisplayClear();
        PDisplay.Visibility = Visibility.Collapsed;
        PEditor.Visibility = Visibility.Collapsed;
        PReferenceScribeShow(editing);
        PReferenceMode.IsEnabled = _pColophonReference is not null;
        PReferenceBin.IsEnabled = _pColophonReference is not null;
    }

    private void PReferenceFreshHandle(object sender, RoutedEventArgs e)
    {
        if (!PReferenceLeaveConfirm())
        {
            return;
        }

        if (_pColophonReference is not null || _pDisplayEntry is not null)
        {
            PFootnoteEntryCreate();
            return;
        }

        PReferenceClear();
        PReferenceScribeShow(true);
        PImprint.PImprintDraftOpen(null);
        PReferenceMode.IsEnabled = true;
    }
}
