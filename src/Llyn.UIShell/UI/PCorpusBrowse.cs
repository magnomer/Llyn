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

    private IReadOnlyDictionary<long, int> _pAnthologyCount = new Dictionary<string, int>();

    private string? _pExcerptExample;

    private LCatalogOrder _pRankChoice;

    private async void PCorpusBulletinHandle(LBulletin bulletin)
    {
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

        string unreadable = _pCorpusHost.PLocalizationTextRead("Display.Unreadable");
        string unwritten = _pCorpusHost.PLocalizationTextRead("Example.Unwritten");

        _pAnthologyList.Clear();
        bool kept = false;
        foreach (LCatalogExample row in read)
        {
            kept |= string.Equals(
                row.LCatalogExampleStored.LExampleId,
                _pExcerptExample,
                StringComparison.Ordinal);
            _pAnthologyList.Add(new PAnthologyItem(
                row.LCatalogExampleStored,
                row.LCatalogExampleUsage,
                row.LCatalogExampleSource,
                unreadable,
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
        PExcerptLanguage.Text = example.LExampleLanguage;
        PExcerptFlag.Source = PEnsign.PEnsignFind(example.LExampleLanguage);
        PExcerptValueShow(PExcerptTranslation, example.LExampleTranslation);
        PExcerptValueShow(
            PExcerptCitation,
            example.LExampleSource,
            PCitationNameRead(example.LExampleSource));

        PQuotationFind(id);

        PExcerptBody.Visibility = Visibility.Visible;
        PExcerptUnselected.Visibility = Visibility.Collapsed;
        PCorpusMode.IsEnabled = true;

        if (PTranscript.Visibility == Visibility.Visible)
        {
            PTranscriptDraftShow(PTranscriptDraftStart(id));
        }
    }

    private void PExcerptValueShow(TextBlock field, LStateValue value, string? shown = null)
    {
        string? text = value.LStateValueState switch
        {
            LState.LStateSpecified => shown ?? value.LStateValueShow(),
            LState.LStateUnknown => _pCorpusHost.PLocalizationTextRead("Display.Unreadable"),
            _ => null,
        };

        field.Text = text ?? _pCorpusHost.PLocalizationTextRead("Example.Unset");
        field.SetResourceReference(
            TextBlock.ForegroundProperty,
            text is null ? "Theme.Muted" : "Theme.Ink");
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

        string unreadable = _pCorpusHost.PLocalizationTextRead("Display.Unreadable");
        string unnamed = _pCorpusHost.PLocalizationTextRead("Example.Unnamed");
        string meaning = _pCorpusHost.PLocalizationTextRead("Example.Meaning");
        string collocation = _pCorpusHost.PLocalizationTextRead("Example.Collocation");

        _pQuotationList.Clear();
        foreach (LUsage usage in read)
        {
            _pQuotationList.Add(new PUsageItem(
                usage,
                usage.LUsageOwner == LOwner.LOwnerCollocation ? collocation : meaning,
                unreadable,
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
                PAnthologyExampleShow(_pExcerptExample);
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
