using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.UIShell;

public partial class PCorpus
{
    private readonly ObservableCollection<PAnthologyItem> _pAnthologyList = [];

    private readonly ObservableCollection<PCitationItem> _pCitationCatalog = [];

    private readonly ObservableCollection<PLanguageItem> _pLanguageItem = [];

    private IReadOnlyDictionary<long, int> _pAnthologyCount = new Dictionary<long, int>();

    private LVista? _pCorpusVista;

    private void PTranscriptDraftUpdate(LBulletin bulletin)
    {
        if (bulletin.LBulletinId == PTranscriptDraft)
        {
            PTranscriptDraftRestore();
        }
    }

    private void PTranscriptTenureUpdate(LBulletin bulletin)
    {
        if (bulletin.LBulletinId == PTranscriptDraft)
        {
            PTranscriptChangeUpdate();
        }
    }

    private async void PCorpusWorkspaceUpdate()
    {
        await PEnsign.PEnsignLoad(_lEngine);
        PSpeakerLoad();
        PCorpusReset();
    }

    private void PQueryHandle(object sender, TextChangedEventArgs e)
    {
        _pCorpusVista?.LVistaQuerySet(PQuery.Text ?? string.Empty);
    }

    private void PRankHandle(object sender, RoutedEventArgs e)
    {
        if (sender is not FrameworkElement { Tag: string choice } || _pCorpusVista is null)
        {
            return;
        }

        PRankDropper.IsChecked = false;
        _pCorpusVista.LVistaOrderSet(LCatalog.LCatalogOrderParse(choice, _pCorpusVista.LVistaOrder));
    }

    internal async void PCorpusVistaRestore(LVista vista, LVista quotation)
    {
        _pCorpusVista = vista;
        _pQuotationVista = quotation;
        vista.LVistaObserverAttach(LSubject.LSubjectVista, new PObserver(this, PAnthologyFind));
        vista.LVistaObserverAttach(LSubject.LSubjectDraft, new PObserver(this, PTranscriptDraftUpdate));
        vista.LVistaObserverAttach(LSubject.LSubjectTenure, new PObserver(this, PTranscriptTenureUpdate));
        vista.LVistaObserverAttach(LSubject.LSubjectWorkspace, new PObserver(this, PCorpusWorkspaceUpdate));
        vista.LVistaObserverAttach(LSubject.LSubjectExample, new PObserver(this, PCitationFind));
        vista.LVistaObserverAttach(LSubject.LSubjectExample, new PObserver(this, PAnthologyFind));
        vista.LVistaObserverAttach(LSubject.LSubjectReference, new PObserver(this, PCitationFind));
        vista.LVistaObserverAttach(LSubject.LSubjectReference, new PObserver(this, PAnthologyFind));
        vista.LVistaObserverAttach(LSubject.LSubjectReflex, new PObserver(this, PAnthologyFind));
        vista.LVistaObserverAttach(LSubject.LSubjectSettings, new PObserver(this, PAnthologyFind));
        quotation.LVistaObserverAttach(LSubject.LSubjectEntry, new PObserver(this, PQuotationEntryUpdate));
        quotation.LVistaChosenAttach(LSubject.LSubjectEntry, new PObserver(this, PCorpusEntryUpdate));
        PDisplay.PDisplayVistaRestore(quotation);
        PRankRestore();
        PGauzeRestore();

        await PEnsign.PEnsignLoad(_lEngine);

        PChoice.PChoiceFilterBuild(PGauzeList, _lEngine.LEngineLanguageRead(), vista.LVistaFilter, PGauzeHandle);
        vista.LVistaQuerySet(PQuery.Text ?? string.Empty);
        PSpeakerLoad();
        PCitationFind();
        PAnthologyFind();
    }

    private void PRankRestore()
    {
        if (_pCorpusVista is not null)
        {
            PChoice.PChoiceOrderApply(PRankDropdown, _pCorpusVista.LVistaOrder);
        }
    }

    private void PGauzeRestore()
    {
        bool active = _pCorpusVista?.LVistaFilter.LCatalogFilterActive == true;
        PGauzeMark.Visibility = active ? Visibility.Visible : Visibility.Collapsed;
    }

    private void PDredgeHandle(object sender, TextChangedEventArgs e)
    {
        PQuotationFind();
    }

    private void PGauzeHandle(object sender, RoutedEventArgs e)
    {
        if (_pCorpusVista is null)
        {
            return;
        }

        _pCorpusVista.LVistaFilterSet(PChoice.PChoiceFilterRead(PGauzeList));
        PGauzeRestore();
    }

    private void PAnthologyChosenApply()
    {
        long? chosen = _pCorpusVista?.LVistaChosen;
        foreach (PAnthologyItem item in _pAnthologyList)
        {
            item.PAnthologyItemChosen = chosen is not null
                && item.PAnthologyItemId == chosen;
        }
    }

    private void PAnthologyFind()
    {
        if (_pCorpusVista is null)
        {
            return;
        }

        IReadOnlyList<LCatalogExample> read;
        try
        {
            read = _lEngine.LEngineExampleFind(_pCorpusVista);
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
            kept |= row.LCatalogExampleChosen;
            _pAnthologyList.Add(new PAnthologyItem(
                row.LCatalogExampleStored,
                row.LCatalogExampleUsage,
                unknown,
                unwritten)
            {
                PAnthologyItemChosen = row.LCatalogExampleChosen,
            });
        }

        LTwin.LTwinNameApply(
            _pAnthologyList, row => row.PAnthologyItemText, (row, name) => row.PAnthologyItemName = name);

        PAnthologyEmpty.Visibility = _pAnthologyList.Count == 0 ? Visibility.Visible : Visibility.Collapsed;

        PExcerptTally.Text = PCorpusTallyRead(_pCorpusVista?.LVistaChosen);
        PTranscriptTally.Text = PCorpusTallyRead(PTranscriptExampleRead());

        if (!kept && _pCorpusVista?.LVistaChosen is not null && PTranscript.Visibility != Visibility.Visible)
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
            PAnthologyFind();
            return;
        }

        _pCorpusVista?.LVistaSelect(id);
        PAnthologyChosenApply();
        PQuotationEntryHide();

        PExcerptSentenceShow(example);
        PExcerptLanguage.Text = example.LExampleLanguage;
        PExcerptFlag.Source = PEnsign.PEnsignFind(example.LExampleLanguage);
        PExcerptGlossShow(example.LExampleGloss);
        PExcerptCitationShow(example.LExampleSource);
        PExcerptTally.Text = PCorpusTallyRead(id);

        PQuotationFind();

        PExcerptBody.Visibility = Visibility.Visible;
        PExcerptUnselected.Visibility = Visibility.Collapsed;
        PCorpusMode.IsEnabled = true;
        PCorpusBin.IsEnabled = true;

        if (PTranscript.Visibility == Visibility.Visible)
        {
            PTranscriptDraftShow(PTranscriptDraftStart(id));
        }
    }

    private void PCorpusBinHandle(object sender, RoutedEventArgs e)
    {
        if (_pQuotationVista?.LVistaChosen is not null || _pCorpusVista?.LVistaChosen is not long id)
        {
            return;
        }

        int usage = _pAnthologyCount.TryGetValue(id, out int count) ? count : 0;

        if (!_pCorpusHost.PWindowRemovalConfirm(usage, "Example"))
        {
            return;
        }

        try
        {
            _lEngine.LEngineExampleDelete(id, usage > 0);
        }
        catch (Exception exception)
        {
            _pCorpusHost.PWindowFailureShow("Example.DeleteFailed", exception);
            return;
        }

        PCorpusScribeShow(false);
        PCorpusClear();
        PAnthologyFind();
    }

    private string PCorpusTallyRead(long? id)
    {
        int count = id is long stored && _pAnthologyCount.TryGetValue(stored, out int usage) ? usage : 0;

        return count switch
        {
            0 => _pCorpusHost.PLocalizationTextRead("Example.UsageNone"),
            1 => _pCorpusHost.PLocalizationTextRead("Example.UsageOne"),
            _ => $"{count.ToString(CultureInfo.CurrentCulture)} "
                + _pCorpusHost.PLocalizationTextRead("Example.UsageMany"),
        };
    }

    private void PCorpusStoreHandle(object sender, RoutedEventArgs e)
    {
        if (PEditor.Visibility == Visibility.Visible)
        {
            PEditor.PEditorEntrySave();
            return;
        }

        PTranscriptStoreRun();
    }

    private void PCorpusScribeHandle(object sender, RoutedEventArgs e)
    {
        bool editing = ReferenceEquals(sender, PCorpusScribe);
        if (_pQuotationVista?.LVistaChosen is not null || PEditor.Visibility == Visibility.Visible)
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

            if (_pCorpusVista?.LVistaChosen is long chosen)
            {
                PAnthologyExampleShow(chosen);
                return;
            }

            PCorpusClear();
            return;
        }

        if (_pCorpusVista?.LVistaChosen is not long shown)
        {
            PCorpusClear();
            return;
        }

        PCorpusScribeShow(true);
        PTranscriptDraftShow(PTranscriptDraftStart(shown));
    }

    private void PCorpusScribeShow(bool editing)
    {
        _lEngine.LEngineSplitSave(editing);

        PTranscript.Visibility = editing ? Visibility.Visible : Visibility.Collapsed;
        PExcerpt.Visibility = editing ? Visibility.Collapsed : Visibility.Visible;
        PCorpusViewer.IsChecked = !editing;
        PCorpusScribe.IsChecked = editing;
        PTranscriptChangeUpdate();
    }

    internal void PCorpusScribeRestore(bool editing)
    {
        if (editing && _pCorpusVista?.LVistaChosen is null)
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

    internal long PCorpusVoyageRead()
    {
        return _pCorpusVista?.LVistaChosen ?? 0;
    }

    private void PCorpusClear()
    {
        PTranscriptDraftCancel();

        _pCorpusVista?.LVistaSelect(null);
        PAnthologyChosenApply();
        PQuotationEntryHide();
        PQuotationFind();

        PExcerptBody.Visibility = Visibility.Collapsed;
        PExcerptUnselected.Visibility = Visibility.Visible;
        PTranscriptApply(null);
        PCorpusScribeShow(false);
        PCorpusMode.IsEnabled = false;
        PCorpusBin.IsEnabled = false;
    }
}
