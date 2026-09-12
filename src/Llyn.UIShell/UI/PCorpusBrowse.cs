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

    private readonly ObservableCollection<PUsageItem> _pQuotationList = [];

    private readonly ObservableCollection<PCitationItem> _pCitationCatalog = [];

    private readonly ObservableCollection<PLanguageItem> _pLanguageItem = [];

    private IReadOnlyDictionary<long, int> _pAnthologyCount = new Dictionary<long, int>();

    private long? _pExcerptExample;

    private LCatalogOrder _pRankChoice;

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

        PExcerptValueShow(PExcerptText, example.LExampleText);
        PExcerptText.PMentionLanguage = example.LExampleLanguage;
        PExcerptText.PMentionMention = example.LExampleText.LStateValueState == LState.LStateSpecified
            ? example.LExampleMention
            : [];
        PExcerptLanguage.Text = example.LExampleLanguage;
        PExcerptFlag.Source = PEnsign.PEnsignFind(example.LExampleLanguage);
        PExcerptValueShow(PExcerptTranslation, example.LExampleTranslation);
        PExcerptAnchorShow(
            PExcerptCitation,
            example.LExampleSource,
            PCitationNameRead(example.LExampleSource.LStateAnchorShow()));

        PQuotationFind(id);

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

    private void PQuotationFind(long id)
    {
        IReadOnlyList<LUsage> read;
        try
        {
            read = _lEngine.LEngineUsageRead(id, LOwner.LOwnerExample);
        }
        catch (Exception exception)
        {
            _pCorpusHost.PWindowFailureShow("Example.LoadFailed", exception);
            return;
        }

        string unknown = _pCorpusHost.PLocalizationTextRead("Display.Unknown");
        string unnamed = _pCorpusHost.PLocalizationTextRead("Example.Unnamed");
        string meaning = _pCorpusHost.PLocalizationTextRead("Example.Meaning");
        string collocation = _pCorpusHost.PLocalizationTextRead("Example.Collocation");

        _pQuotationList.Clear();
        foreach (LUsage usage in read)
        {
            _pQuotationList.Add(new PUsageItem(
                usage,
                usage.LUsageOwner == LOwner.LOwnerCollocation ? collocation : meaning,
                unknown,
                unnamed));
        }

        PTwin.PTwinNameApply(
            _pQuotationList, row => row.PUsageItemHeadword, (row, name) => row.PUsageItemName = name);

        PQuotationEmpty.Visibility = _pQuotationList.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
    }

    private void PQuotationHandle(object sender, RoutedEventArgs e)
    {
        if (sender is not FrameworkElement row || row.DataContext is not PUsageItem item)
        {
            return;
        }

        if (!PCorpusLeaveConfirm())
        {
            return;
        }

        _pCorpusHost.PWindowEntryShow(item.PUsageItemEntry);
    }

    private void PCorpusScribeHandle(object sender, RoutedEventArgs e)
    {
        bool editing = ReferenceEquals(sender, PCorpusScribe);
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
        return _pCorpusHost.PWindowDiscardConfirm(PCorpusChangeCheck());
    }

    private void PCorpusClear()
    {
        PTranscriptDraftCancel();

        _pExcerptExample = null;
        PAnthologySelect(null);
        _pQuotationList.Clear();

        PExcerptBody.Visibility = Visibility.Collapsed;
        PExcerptUnselected.Visibility = Visibility.Visible;
        PTranscriptApply(null);
        PCorpusScribeShow(false);
        PCorpusMode.IsEnabled = false;
    }
}
