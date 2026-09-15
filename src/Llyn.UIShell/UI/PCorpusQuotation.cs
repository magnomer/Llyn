using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using Llyn.Core;

namespace Llyn.UIShell;

public partial class PCorpus
{
    private readonly ObservableCollection<PQuotationItem> _pQuotationList = [];

    private long? _pDisplayEntry;

    private void PQuotationFind()
    {
        IReadOnlyList<LEntry> read;
        try
        {
            read = _lEngine.LEngineEntryFind(
                new LExample(
                    _pExcerptExample ?? 0,
                    string.Empty,
                    LStateValue.LStateValueUnspecified,
                    LStateAnchor.LStateAnchorUnspecified),
                PDredge.Text ?? string.Empty,
                _pGauzeChoice);
        }
        catch (Exception exception)
        {
            _pCorpusHost.PWindowFailureShow("Example.LoadFailed", exception);
            return;
        }

        _pQuotationList.Clear();
        foreach (LEntry entry in read)
        {
            _pQuotationList.Add(new PQuotationItem(
                entry.LEntryId,
                entry.LEntryHeadword,
                entry.LEntryLanguage,
                _lEngine.LEngineEpithetRead(entry.LEntryId)));
        }

        PTwin.PTwinNameApply(
            _pQuotationList,
            row => row.PQuotationItemHeadword,
            (row, name) => row.PQuotationItemName = name,
            row => row.PQuotationItemId);

        PQuotationEmpty.SetResourceReference(
            TextBlock.TextProperty,
            string.IsNullOrWhiteSpace(PDredge.Text) ? "Example.Vacant" : "Example.Unmatched");
        PQuotationEmpty.Visibility = _pQuotationList.Count == 0 ? Visibility.Visible : Visibility.Collapsed;

        PQuotationSelect(_pDisplayEntry);
    }

    private void PQuotationSelect(long? id)
    {
        foreach (PQuotationItem item in _pQuotationList)
        {
            item.PQuotationItemChosen = id is not null
                && item.PQuotationItemId == id;
        }
    }

    private void PQuotationHandle(object sender, RoutedEventArgs e)
    {
        if (sender is not FrameworkElement row || row.DataContext is not PQuotationItem item)
        {
            return;
        }

        if (!PCorpusLeaveConfirm())
        {
            return;
        }

        PQuotationEntryShow(item.PQuotationItemId);
    }

    private void PQuotationEntryShow(long id)
    {
        LEntryDraft? draft;
        try
        {
            draft = _lEngine.LEngineEntryLoad(id);
        }
        catch (Exception exception)
        {
            _pCorpusHost.PWindowFailureShow("List.LoadFailed", exception);
            return;
        }

        if (draft is null)
        {
            PQuotationEntryHide();
            PQuotationFind();
            return;
        }

        bool editing = PTranscript.Visibility == Visibility.Visible
            || PEditor.Visibility == Visibility.Visible;
        PTranscriptDraftCancel();
        PTranscript.Visibility = Visibility.Collapsed;
        PExcerpt.Visibility = Visibility.Collapsed;

        _pDisplayEntry = id;
        PQuotationSelect(id);
        PDisplay.PDisplayShow(id, draft);
        PCorpusMode.IsEnabled = true;
        PCorpusBin.IsEnabled = false;

        if (editing)
        {
            PEditor.PEditorEntryShow(id);
        }

        PQuotationScribeShow(editing);
    }

    private void PQuotationEntryCreate()
    {
        long? example = _pExcerptExample;

        PTranscriptDraftCancel();
        PTranscript.Visibility = Visibility.Collapsed;
        PExcerpt.Visibility = Visibility.Collapsed;

        _pDisplayEntry = null;
        PQuotationSelect(null);
        PDisplay.PDisplayClear();
        PEditor.PEditorReset();
        PCorpusMode.IsEnabled = true;
        PCorpusBin.IsEnabled = false;
        PQuotationScribeShow(true);

        if (example is long id)
        {
            PEditor.PEditorExampleAdd(id);
        }
    }

    private void PQuotationScribeHandle(bool editing)
    {
        if (_pDisplayEntry is not long id)
        {
            if (!editing)
            {
                PQuotationScribeReset();
            }

            return;
        }

        if (editing == (PEditor.Visibility == Visibility.Visible))
        {
            return;
        }

        if (!editing)
        {
            if (!PCorpusLeaveConfirm())
            {
                PQuotationScribeShow(true);
                return;
            }

            PQuotationScribeShow(false);
            PQuotationEntryShow(id);
            return;
        }

        PEditor.PEditorEntryShow(id);
        PQuotationScribeShow(true);
    }

    private void PQuotationScribeReset()
    {
        if (!PCorpusLeaveConfirm())
        {
            PQuotationScribeShow(true);
            return;
        }

        PQuotationScribeShow(false);

        if (_pDisplayEntry is long stored)
        {
            PQuotationEntryShow(stored);
            return;
        }

        PEditor.PEditorReset();

        if (_pExcerptExample is long kept)
        {
            PAnthologyExampleShow(kept);
            return;
        }

        PCorpusClear();
    }

    private void PQuotationScribeShow(bool editing)
    {
        _lEngine.LEngineSplitSave(editing);

        PEditor.Visibility = editing ? Visibility.Visible : Visibility.Collapsed;
        PDisplay.Visibility = editing ? Visibility.Collapsed : Visibility.Visible;
        PCorpusViewer.IsChecked = !editing;
        PCorpusScribe.IsChecked = editing;
    }

    private void PQuotationEntryUpdate(long id)
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
            if (_pExcerptExample is long kept)
            {
                PAnthologyExampleShow(kept);
                return;
            }

            PCorpusClear();
            return;
        }

        PDisplay.PDisplayShow(shown, draft);
    }

    private void PQuotationEntryHide()
    {
        bool editing = PTranscript.Visibility == Visibility.Visible
            || PEditor.Visibility == Visibility.Visible;
        if (PEditor.Visibility == Visibility.Visible)
        {
            PEditor.PEditorReset();
        }

        _pDisplayEntry = null;
        PQuotationSelect(null);
        PDisplay.PDisplayClear();
        PDisplay.Visibility = Visibility.Collapsed;
        PEditor.Visibility = Visibility.Collapsed;
        PCorpusScribeShow(editing);
        PCorpusMode.IsEnabled = _pExcerptExample is not null;
        PCorpusBin.IsEnabled = _pExcerptExample is not null;
    }
}
