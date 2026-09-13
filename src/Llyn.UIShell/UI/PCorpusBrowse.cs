using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using Llyn.Core;

namespace Llyn.UIShell;

public partial class PCorpus
{
    private readonly ObservableCollection<PAnthologyItem> _pAnthologyList = [];

    private readonly ObservableCollection<PQuotationItem> _pQuotationList = [];

    private readonly ObservableCollection<PCitationItem> _pCitationCatalog = [];

    private readonly ObservableCollection<PLanguageItem> _pLanguageItem = [];

    private IReadOnlyDictionary<long, int> _pAnthologyCount = new Dictionary<long, int>();

    private long? _pExcerptExample;

    private long? _pDisplayEntry;

    private LCatalogOrder _pRankChoice;

    private LCatalogFilter _pGauzeChoice = LCatalogFilter.LCatalogFilterEmpty;

    private async void PCorpusBulletinHandle(LBulletin bulletin)
    {
        if (bulletin.LBulletinSubject == LSubject.LSubjectDraft)
        {
            if (bulletin.LBulletinId == _pTranscriptDraft)
            {
                PTranscriptDraftRestore();
            }

            return;
        }

        if (bulletin.LBulletinSubject == LSubject.LSubjectWorkspace)
        {
            await PEnsign.PEnsignLoad(_lEngine);
            PSpeakerLoad();
            PCorpusReset();
            return;
        }

        PCitationFind();
        PAnthologyFind(PQuery.Text ?? string.Empty);
        PQuotationEntryUpdate(bulletin.LBulletinId);
    }

    private void PQueryHandle(object sender, TextChangedEventArgs e)
    {
        PAnthologyFind(PQuery.Text ?? string.Empty);
    }

    private void PRankHandle(object sender, RoutedEventArgs e)
    {
        if (sender is not FrameworkElement { Tag: string choice })
        {
            return;
        }

        _pRankChoice = LCatalog.LCatalogOrderParse(choice, _pRankChoice);
        _lEngine.LEngineRankSave(_pRankChoice);
        PRankDropper.IsChecked = false;
        PAnthologyFind(PQuery.Text ?? string.Empty);
    }

    internal async void PRankRestore(LCatalogOrder order)
    {
        _pRankChoice = order;
        PChoice.PChoiceOrderApply(PRankDropdown, order);

        await PEnsign.PEnsignLoad(_lEngine);

        PSpeakerLoad();
        PCitationFind();
        PAnthologyFind(PQuery.Text ?? string.Empty);
    }

    private void PDredgeHandle(object sender, TextChangedEventArgs e)
    {
        PQuotationFind();
    }

    private void PGauzeHandle(object sender, RoutedEventArgs e)
    {
        _pGauzeChoice = PChoice.PChoiceFilterRead(PGauzeList);
        _lEngine.LEngineGauzeSave(_pGauzeChoice);
        PGauzeMark.Visibility = _pGauzeChoice.LCatalogFilterActive ? Visibility.Visible : Visibility.Collapsed;
        PQuotationFind();
    }

    internal async void PGauzeRestore(LCatalogFilter filter)
    {
        _pGauzeChoice = filter;
        PGauzeMark.Visibility = filter.LCatalogFilterActive ? Visibility.Visible : Visibility.Collapsed;

        await PEnsign.PEnsignLoad(_lEngine);

        PChoice.PChoiceFilterBuild(PGauzeList, _lEngine.LEngineLanguageRead(), filter, PGauzeHandle);
    }

    private void PAnthologySelect(long? id)
    {
        foreach (PAnthologyItem item in _pAnthologyList)
        {
            item.PAnthologyItemChosen = id is not null
                && item.PAnthologyItemId == id;
        }
    }

    private void PAnthologyFind(string query)
    {
        IReadOnlyList<LCatalogExample> read;
        try
        {
            read = _lEngine.LEngineExampleFind(query, _pRankChoice);
            _pAnthologyCount = _lEngine.LEngineUsageRead(LOwner.LOwnerExample);
        }
        catch (Exception exception)
        {
            _pCorpusHost.PWindowFailureShow("Example.LoadFailed", exception);
            return;
        }

        string unknown = _pCorpusHost.PLocalizationTextRead("Display.Unknown");
        string unwritten = _pCorpusHost.PLocalizationTextRead("Example.Unwritten");

        _pAnthologyList.Clear();
        bool kept = false;
        foreach (LCatalogExample row in read)
        {
            kept |= row.LCatalogExampleStored.LExampleId == _pExcerptExample;
            _pAnthologyList.Add(new PAnthologyItem(
                row.LCatalogExampleStored,
                row.LCatalogExampleUsage,
                row.LCatalogExampleSource,
                unknown,
                unwritten));
        }

        PTwin.PTwinNameApply(
            _pAnthologyList, row => row.PAnthologyItemText, (row, name) => row.PAnthologyItemName = name);

        PAnthologyEmpty.Visibility = _pAnthologyList.Count == 0 ? Visibility.Visible : Visibility.Collapsed;

        PAnthologySelect(_pExcerptExample);

        if (!kept && _pExcerptExample is not null && PTranscript.Visibility != Visibility.Visible)
        {
            PCorpusClear();
        }

        PQuotationFind();
    }

    private void PAnthologyHandle(object sender, RoutedEventArgs e)
    {
        if (sender is not FrameworkElement row || row.DataContext is not PAnthologyItem item)
        {
            return;
        }

        if (!PCorpusLeaveConfirm())
        {
            return;
        }

        PAnthologyExampleShow(item.PAnthologyItemId);
    }

    internal void PAnthologyExampleShow(long id)
    {
        LExample? example;
        try
        {
            example = _lEngine.LEngineExampleRead(id);
        }
        catch (Exception exception)
        {
            _pCorpusHost.PWindowFailureShow("Example.LoadFailed", exception);
            return;
        }

        if (example is null)
        {
            PCorpusClear();
            PAnthologyFind(PQuery.Text ?? string.Empty);
            return;
        }

        _pExcerptExample = id;
        PAnthologySelect(id);
        PQuotationEntryHide();

        PExcerptValueShow(PExcerptText, example.LExampleText);
        PExcerptText.PMentionLanguage = example.LExampleLanguage;
        PExcerptText.PMentionMention = example.LExampleText.LStateValueState == LState.LStateSpecified
            ? example.LExampleMention
            : [];
        PExcerptLanguage.Text = example.LExampleLanguage;
        PExcerptFlag.Source = PEnsign.PEnsignFind(example.LExampleLanguage);
        PExcerptGlossShow(example.LExampleGloss);
        PExcerptAnchorShow(
            PExcerptCitation,
            example.LExampleSource,
            PCitationNameRead(example.LExampleSource.LStateAnchorShow()));

        PQuotationFind();

        PExcerptBody.Visibility = Visibility.Visible;
        PExcerptUnselected.Visibility = Visibility.Collapsed;
        PCorpusMode.IsEnabled = true;

        if (PTranscript.Visibility == Visibility.Visible)
        {
            PTranscriptDraftShow(PTranscriptDraftStart(id));
        }
    }

    private void PExcerptAnchorShow(TextBlock field, LStateAnchor value, string? shown)
    {
        PExcerptTextShow(
            field,
            value.LStateAnchorState switch
            {
                LState.LStateUnknown => _pCorpusHost.PLocalizationTextRead("Display.Unknown"),
                LState.LStateSpecified when !string.IsNullOrEmpty(shown) => shown,
                _ => null,
            });
    }

    private void PExcerptValueShow(TextBlock field, LStateValue value)
    {
        PExcerptTextShow(
            field,
            value.LStateValueState switch
            {
                _ when value.LStateValueUnreadable => value.LStateValueShow(),
                LState.LStateSpecified => value.LStateValueShow(),
                LState.LStateUnknown => _pCorpusHost.PLocalizationTextRead("Display.Unknown"),
                _ => null,
            });
    }

    private void PExcerptTextShow(TextBlock field, string? text)
    {
        string shown = text ?? _pCorpusHost.PLocalizationTextRead("Example.Unset");
        if (field is PMention sentence)
        {
            sentence.PMentionText = shown;
        }
        else
        {
            field.Text = shown;
        }

        field.SetResourceReference(
            TextBlock.ForegroundProperty,
            text is null ? "Theme.Muted" : "Theme.Ink");
    }

    private void PExcerptMentionHandle(object? sender, PMentionArgument e)
    {
        if (_pExcerptExample is not long id || !PCorpusLeaveConfirm())
        {
            return;
        }

        LMentionResult result;
        try
        {
            result = _lEngine.LEngineMentionFind(id, e.PMentionArgumentOffset);
        }
        catch (Exception exception)
        {
            _pCorpusHost.PWindowFailureShow("Mention.FindFailed", exception);
            return;
        }

        _pCorpusHost.PWindowMentionHandle(PExcerptText, result);
    }

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
                entry.LEntryId, entry.LEntryHeadword, entry.LEntryLanguage));
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

        if (editing)
        {
            PEditor.PEditorEntryShow(id);
        }

        PQuotationScribeShow(editing);
    }

    private void PQuotationScribeHandle(bool editing)
    {
        if (_pDisplayEntry is not long id || editing == (PEditor.Visibility == Visibility.Visible))
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

    private void PQuotationScribeShow(bool editing)
    {
        _lEngine.LEngineSplitSave(editing);

        PEditor.Visibility = editing ? Visibility.Visible : Visibility.Collapsed;
        PDisplay.Visibility = editing ? Visibility.Collapsed : Visibility.Visible;
        PCorpusStore.Visibility = editing ? Visibility.Visible : Visibility.Collapsed;
        PCorpusViewer.IsChecked = !editing;
        PCorpusScribe.IsChecked = editing;
    }

    private void PCorpusStoreHandle(object sender, RoutedEventArgs e)
    {
        PEditor.PEditorEntrySave();
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
        PCorpusStore.Visibility = Visibility.Collapsed;
        PCorpusScribeShow(editing);
        PCorpusMode.IsEnabled = _pExcerptExample is not null;
    }

    private void PCorpusScribeHandle(object sender, RoutedEventArgs e)
    {
        bool editing = ReferenceEquals(sender, PCorpusScribe);
        if (_pDisplayEntry is not null)
        {
            PQuotationScribeHandle(editing);
            return;
        }

        if (editing == (PTranscript.Visibility == Visibility.Visible))
        {
            return;
        }

        if (!editing)
        {
            if (!PCorpusLeaveConfirm())
            {
                PCorpusScribeShow(true);
                return;
            }

            PTranscriptDraftCancel();
            PCorpusScribeShow(false);

            if (_pExcerptExample is not null)
            {
                PAnthologyExampleShow(_pExcerptExample.Value);
                return;
            }

            PCorpusClear();
            return;
        }

        if (_pExcerptExample is null)
        {
            PCorpusClear();
            return;
        }

        PCorpusScribeShow(true);
        PTranscriptDraftShow(PTranscriptDraftStart(_pExcerptExample));
    }

    private void PCorpusScribeShow(bool editing)
    {
        _lEngine.LEngineSplitSave(editing);

        PTranscript.Visibility = editing ? Visibility.Visible : Visibility.Collapsed;
        PExcerpt.Visibility = editing ? Visibility.Collapsed : Visibility.Visible;
        PCorpusViewer.IsChecked = !editing;
        PCorpusScribe.IsChecked = editing;
    }

    internal void PCorpusScribeRestore(bool editing)
    {
        if (editing && _pExcerptExample is null)
        {
            return;
        }

        if (editing)
        {
            PCorpusMode.IsEnabled = true;
        }

        PCorpusScribeShow(editing);
    }

    internal bool PCorpusLeaveConfirm()
    {
        return _pCorpusHost.PWindowDiscardConfirm(PCorpusChangeCheck(), PCorpusDraftFinish);
    }

    private void PCorpusClear()
    {
        PTranscriptDraftCancel();

        _pExcerptExample = null;
        PAnthologySelect(null);
        PQuotationEntryHide();
        PQuotationFind();

        PExcerptBody.Visibility = Visibility.Collapsed;
        PExcerptUnselected.Visibility = Visibility.Visible;
        PTranscriptApply(null);
        PCorpusScribeShow(false);
        PCorpusMode.IsEnabled = false;
    }
}
