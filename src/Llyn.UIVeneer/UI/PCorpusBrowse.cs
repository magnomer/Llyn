using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.UIVeneer;

public partial class PCorpus
{
    private readonly ObservableCollection<PAnthologyItem> _pAnthologyList = [];

    private readonly ObservableCollection<PCitationItem> _pCitationCatalog = [];

    private readonly ObservableCollection<PLanguageItem> _pLanguageItem = [];

    private IReadOnlyDictionary<long, int> _pAnthologyCount = new Dictionary<long, int>();

    private async void PCorpusWorkspaceUpdate()
    {
        await PEnsign.PEnsignLoad(_pCorpusHost.PWindowDeportment);
        PSpeakerLoad();
        PCorpusReset();
    }

    private void PQueryHandle(object sender, TextChangedEventArgs e)
    {
        _lCorpus.LCorpusQuerySet(PQuery.Text ?? string.Empty);
    }

    private void PRankHandle(object sender, RoutedEventArgs e)
    {
        if (sender is not FrameworkElement { Tag: string choice })
        {
            return;
        }

        PRankDropper.IsChecked = false;
        _lCorpus.LCorpusRankSet(choice);
    }

    internal async void PCorpusVistaRestore()
    {
        _lCorpus.LCorpusVistaRestore(_pCorpusHost.PWindowPosture);
        _lCorpus.LCorpusObserverAttach(LSubject.LSubjectVista, PObserver.PObserverCreate(this, PAnthologyFind));
        _lCorpus.LCorpusObserverAttach(
            LSubject.LSubjectWorkspace, PObserver.PObserverCreate(this, PCorpusWorkspaceUpdate));
        _lCorpus.LCorpusObserverAttach(LSubject.LSubjectExample, PObserver.PObserverCreate(this, PCitationFind));
        _lCorpus.LCorpusObserverAttach(LSubject.LSubjectExample, PObserver.PObserverCreate(this, PAnthologyFind));
        _lCorpus.LCorpusObserverAttach(LSubject.LSubjectReference, PObserver.PObserverCreate(this, PCitationFind));
        _lCorpus.LCorpusObserverAttach(LSubject.LSubjectReference, PObserver.PObserverCreate(this, PAnthologyFind));
        _lCorpus.LCorpusObserverAttach(LSubject.LSubjectReflex, PObserver.PObserverCreate(this, PAnthologyFind));
        _lCorpus.LCorpusObserverAttach(LSubject.LSubjectSettings, PObserver.PObserverCreate(this, PAnthologyFind));
        _lCorpus.LCorpusQuotationAttach(LSubject.LSubjectEntry, PObserver.PObserverCreate(this, PQuotationEntryUpdate));
        _lCorpus.LCorpusEntryAttach(LSubject.LSubjectEntry, PObserver.PObserverCreate(this, PCorpusEntryUpdate));
        _lCorpus.LCorpusQuotationAttach(LSubject.LSubjectVista, PObserver.PObserverCreate(this, PQuotationFind));
        PDisplay.PDisplayObserverAttach();
        PEditor.PEditorVistaRestore();
        PChoice.PChoiceOrderApply(PRankDropdown, _lCorpus.LCorpusOrder);
        PGauzeRestore();

        await PEnsign.PEnsignLoad(_pCorpusHost.PWindowDeportment);

        PChoice.PChoiceFilterBuild(PGauzeList, _lCorpus.LCorpusLanguageRead(), _lCorpus.LCorpusFilter, PGauzeHandle);
        _lCorpus.LCorpusQuerySet(PQuery.Text ?? string.Empty);
        _lCorpus.LCorpusDredgeSet(PDredge.Text);
        PSpeakerLoad();
        PCitationFind();
        PAnthologyFind();
    }

    private void PGauzeRestore()
    {
        PGauzeMark.Visibility = _lCorpus.LCorpusFiltered ? Visibility.Visible : Visibility.Collapsed;
    }

    private void PDredgeHandle(object sender, TextChangedEventArgs e)
    {
        _lCorpus.LCorpusDredgeSet(PDredge.Text);
    }

    private void PGauzeHandle(object sender, RoutedEventArgs e)
    {
        _lCorpus.LCorpusGauzeSet(PChoice.PChoiceFilterRead(PGauzeList));
        PGauzeRestore();
    }

    private void PAnthologyFind()
    {
        IReadOnlyList<LCatalogExample> read;
        try
        {
            read = _lCorpus.LCorpusRowsRead(
                PLocalizationCatalog.PLocalizationTextRead("Display.Unknown"),
                PLocalizationCatalog.PLocalizationTextRead("Example.Unwritten"));
            _pAnthologyCount = _lCorpus.LCorpusUsageRead();
        }
        catch (Exception exception)
        {
            _pCorpusHost.PWindowFailureShow("Example.LoadFailed", exception);
            return;
        }

        string unknown = PLocalizationCatalog.PLocalizationTextRead("Display.Unknown");
        string unwritten = PLocalizationCatalog.PLocalizationTextRead("Example.Unwritten");

        List<PAnthologyItem> fresh = [];
        bool kept = false;
        foreach (LCatalogExample row in read)
        {
            kept |= row.LCatalogExampleChosen;
            fresh.Add(new PAnthologyItem(
                row.LCatalogExampleStored,
                row.LCatalogExampleUsage,
                unknown,
                unwritten,
                row.LCatalogExampleChosen)
            {
                PAnthologyItemName = row.LCatalogExampleName,
            });
        }

        PSplice.PSpliceApply(
            _pAnthologyList, fresh, PAnthologyItem.PAnthologyItemMatch, PAnthologyItem.PAnthologyItemSync);

        PAnthologyEmpty.Visibility = _pAnthologyList.Count == 0 ? Visibility.Visible : Visibility.Collapsed;

        PExcerptTally.Text = PCorpusTallyRead(_lCorpus.LCorpusChosen);
        PTranscriptTally.Text = PCorpusTallyRead(PTranscriptExampleRead());

        if (!kept)
        {
            if (_lCorpus.LCorpusChosen is not null)
            {
                if (PTranscript.Visibility != Visibility.Visible)
                {
                    PCorpusClear();
                }
            }
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
            _lCorpus.LCorpusSelect(id);
            example = _lCorpus.LCorpusLoad();
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

        _lCorpus.LCorpusSelect(id);
        PAnthologyFind();
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
        if (_lCorpus.LCorpusQuotationChosen is not null)
        {
            return;
        }

        if (_lCorpus.LCorpusChosen is not long id)
        {
            return;
        }

        int usage = _pAnthologyCount.GetValueOrDefault(id);

        if (!_pCorpusHost.PWindowRemovalConfirm(usage, "Example"))
        {
            return;
        }

        try
        {
            _lCorpus.LCorpusDelete();
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
            0 => PLocalizationCatalog.PLocalizationTextRead("Example.UsageNone"),
            1 => PLocalizationCatalog.PLocalizationTextRead("Example.UsageOne"),
            _ => string.Concat(
                count.ToString(CultureInfo.CurrentCulture),
                " ",
                PLocalizationCatalog.PLocalizationTextRead("Example.UsageMany")),
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
        if (_lCorpus.LCorpusQuotationChosen is not null || PEditor.Visibility == Visibility.Visible)
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

            if (_lCorpus.LCorpusChosen is long chosen)
            {
                PAnthologyExampleShow(chosen);
                return;
            }

            PCorpusClear();
            return;
        }

        if (_lCorpus.LCorpusChosen is not long shown)
        {
            PCorpusClear();
            return;
        }

        PCorpusScribeShow(true);
        PTranscriptDraftShow(PTranscriptDraftStart(shown));
    }

    private void PCorpusScribeShow(bool editing)
    {
        _lCorpus.LCorpusTranscriptSet(editing);

        PTranscript.Visibility = editing ? Visibility.Visible : Visibility.Collapsed;
        PExcerpt.Visibility = editing ? Visibility.Collapsed : Visibility.Visible;
        PCorpusViewer.IsChecked = !editing;
        PCorpusScribe.IsChecked = editing;
        PTranscriptChangeUpdate();
    }

    internal void PCorpusScribeRestore(bool editing)
    {
        if (editing)
        {
            if (_lCorpus.LCorpusChosen is null)
            {
                return;
            }
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
        return _lCorpus.LCorpusChosen ?? 0;
    }

    private void PCorpusClear()
    {
        PTranscriptDraftCancel();

        _lCorpus.LCorpusSelect(null);
        PAnthologyFind();
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
