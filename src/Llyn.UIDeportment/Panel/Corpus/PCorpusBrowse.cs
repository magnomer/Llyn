using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.UIDeportment;

public partial class PCorpus
{
    private readonly ObservableCollection<PAnthologyItem> _pAnthologyList = [];

    private readonly ObservableCollection<PCitationItem> _pCitationCatalog = [];

    private readonly ObservableCollection<PLanguageItem> _pLanguageItem = [];

    private IReadOnlyDictionary<long, int> _pAnthologyCount = new Dictionary<long, int>();

    private async void PCorpusWorkspaceUpdate()
    {
        await LEnsignImage.LEnsignLoad(_pCorpusHost.PWindowDeportment);
        PSpeakerLoad();
        PCorpusReset();
    }

    private void PQueryHandle(object sender, TextChangedEventArgs e)
    {
        _lCorpus.LCorpusAnthology.LAnthologyQuerySet(PQuery.Text ?? string.Empty);
    }

    private void PRankHandle(object sender, RoutedEventArgs e)
    {
        PRankDropper.IsChecked = false;
        _lCorpus.LCorpusAnthology.LAnthologyRankSet(LChoice.LChoiceOrderRead(sender));
    }

    internal async void PCorpusVistaRestore()
    {
        LPanel anthology = _lCorpus.LCorpusAnthology.LAnthologyPanel;
        _lCorpus.LCorpusVistaRestore(_pCorpusHost.PWindowDeportment);
        PCorpusObserverAttach();
        PDisplay.PDisplayObserverAttach();
        PEditor.PEditorVistaRestore();
        PChoice.PChoiceOrderBuild(
            PRankList,
            "Rank",
            PRankHandle,
            [
                LCatalogOrder.LCatalogOrderText,
                LCatalogOrder.LCatalogOrderLanguage,
                LCatalogOrder.LCatalogOrderSource,
                LCatalogOrder.LCatalogOrderUsage,
            ]);
        PChoice.PChoiceOrderApply(PRankDropdown, anthology.LPanelOrder);
        PGauzeRestore();

        await LEnsignImage.LEnsignLoad(_pCorpusHost.PWindowDeportment);

        PGauzeBuild();
        _lCorpus.LCorpusAnthology.LAnthologyQuerySet(PQuery.Text ?? string.Empty);
        _lCorpus.LCorpusQuotation.LQuotationDredgeSet(PDredge.Text);
        PSpeakerLoad();
        PCitationFind();
        anthology.LPanelRowsUpdate();
    }

    private void PCorpusObserverAttach()
    {
        LPanel anthology = _lCorpus.LCorpusAnthology.LAnthologyPanel;
        LPanel quotation = _lCorpus.LCorpusQuotation.LQuotationPanel;
        Action<LBulletin> rows = LObserver.LObserverCreate(this, anthology.LPanelRowsUpdate);
        Action<LBulletin> citation = LObserver.LObserverCreate(this, PCitationFind);
        anthology.LPanelObserverAttach(LSubject.LSubjectVista, rows);
        anthology.LPanelObserverAttach(
            LSubject.LSubjectWorkspace, LObserver.LObserverCreate(this, PCorpusWorkspaceUpdate));
        anthology.LPanelObserverAttach(LSubject.LSubjectExample, citation);
        anthology.LPanelObserverAttach(LSubject.LSubjectExample, rows);
        anthology.LPanelObserverAttach(LSubject.LSubjectReference, citation);
        anthology.LPanelObserverAttach(LSubject.LSubjectReference, rows);
        anthology.LPanelObserverAttach(LSubject.LSubjectReflex, rows);
        anthology.LPanelObserverAttach(LSubject.LSubjectSettings, rows);
        quotation.LPanelObserverAttach(
            LSubject.LSubjectEntry, LObserver.LObserverCreate(this, quotation.LPanelEntrySelect));
        quotation.LPanelObserverAttach(LSubject.LSubjectEntry, citation);
        quotation.LPanelObserverAttach(LSubject.LSubjectEntry, rows);
        quotation.LPanelChosenAttach(
            LSubject.LSubjectEntry, LObserver.LObserverCreate(this, _lCorpus.LCorpusEntryUpdate));
        quotation.LPanelObserverAttach(
            LSubject.LSubjectVista, LObserver.LObserverCreate(this, quotation.LPanelRowsUpdate));
    }

    private void PGauzeRestore()
    {
        PGauzeMark.Visibility = PLook.PLookVisibleRead(_lCorpus.LCorpusAnthology.LAnthologyFiltered);
    }

    private void PGauzeBuild()
    {
        PChoice.PChoiceFilterBuild(
            PGauzeList,
            _lCorpus.LCorpusAnthology.LAnthologyLanguageRead(),
            _lCorpus.LCorpusAnthology.LAnthologyPanel.LPanelFilter,
            PGauzeHandle);
    }

    private void PQueryClear()
    {
        PQuery.Clear();
        PGauzeBuild();
        PGauzeRestore();
    }

    private void PDredgeHandle(object sender, TextChangedEventArgs e)
    {
        _lCorpus.LCorpusQuotation.LQuotationDredgeSet(PDredge.Text);
    }

    private void PGauzeHandle(object sender, RoutedEventArgs e)
    {
        _lCorpus.LCorpusAnthology.LAnthologyGauzeSet(LChoice.LChoiceFilterRead(PGauzeList));
        PGauzeRestore();
    }

    private void PAnthologyFind()
    {
        IReadOnlyList<LCatalogExample> read;
        try
        {
            read = _lCorpus.LCorpusAnthology.LAnthologyRowsRead(
                PLocalizationCatalog.PLocalizationTextRead("Display.Unknown"),
                PLocalizationCatalog.PLocalizationTextRead("Example.Unwritten"));
            _pAnthologyCount = _lCorpus.LCorpusAnthology.LAnthologyUsageRead();
        }
        catch (Exception exception)
        {
            _pCorpusHost.PWindowFailureShow("Example.LoadFailed", exception);
            return;
        }

        string unknown = PLocalizationCatalog.PLocalizationTextRead("Display.Unknown");
        string unwritten = PLocalizationCatalog.PLocalizationTextRead("Example.Unwritten");

        List<PAnthologyItem> fresh = [];
        foreach (LCatalogExample row in read)
        {
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

        LSplice.LSpliceApply(
            _pAnthologyList, fresh, PAnthologyItem.PAnthologyItemMatch, PAnthologyItem.PAnthologyItemSync);

        PAnthologyEmpty.Visibility = PLook.PLookVisibleRead(_pAnthologyList.Count == 0);

        PExcerptTally.Text = PCorpusTallyRead(_lCorpus.LCorpusAnthology.LAnthologyChosen);
        PTranscriptTally.Text = PCorpusTallyRead(PTranscriptDesk.LDeskStoredRead());
        _lCorpus.LCorpusRowsApply(read);
    }

    private void PAnthologyHandle(object sender, RoutedEventArgs e)
    {
        _lCorpus.LCorpusSelect(
            PSender.PSenderSourceRead<PAnthologyItem>(e)?.PAnthologyItemId, _pCorpusHost.PVoyageRecord);
    }

    private void PAnthologyApply(FrameworkElement container, object item, string? _)
    {
        if (item is not PAnthologyItem anthology)
        {
            return;
        }

        if (PLook.PLookPartFind<Button>(container, "PAnthologyRow") is Button row)
        {
            if (anthology.PAnthologyItemChosen)
            {
                row.Tag = "Chosen";
            }
            else
            {
                row.ClearValue(TagProperty);
            }

            row.Click -= PAnthologyHandle;
            row.Click += PAnthologyHandle;
        }

        if (PLook.PLookPartFind<Image>(container, "PAnthologyFlag") is Image flag)
        {
            flag.Source = anthology.PAnthologyItemFlag;
        }

        if (PLook.PLookPartFind<TextBlock>(container, "PAnthologyName") is TextBlock name)
        {
            name.Text = anthology.PAnthologyItemName;
        }

        if (PLook.PLookPartFind<TextBlock>(container, "PAnthologyLanguage") is TextBlock language)
        {
            language.Text = anthology.PAnthologyItemLanguage;
        }

        if (PLook.PLookPartFind<TextBlock>(container, "PAnthologyCount") is TextBlock count)
        {
            count.Text = anthology.PAnthologyItemCount;
        }
    }

    internal void PAnthologyExampleShow(long id)
    {
        _lCorpus.LCorpusExampleShow(id);
    }

    private void PCorpusBinHandle(object sender, RoutedEventArgs e)
    {
        _lCorpus.LCorpusDelete();
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
        _lCorpus.LCorpusSave();
    }

    private void PCorpusScribeHandle(object sender, RoutedEventArgs e)
    {
        _lCorpus.LCorpusScribeSet(ReferenceEquals(sender, PCorpusScribe));
    }

    internal void PCorpusScribeRestore(bool editing)
    {
        _lCorpus.LCorpusAnthology.LAnthologyPanel.LPanelScribeRestore(editing);
    }

    internal bool PCorpusLeaveConfirm()
    {
        return _lCorpus.LCorpusLeaveConfirm();
    }

    internal long PCorpusVoyageRead()
    {
        return _lCorpus.LCorpusAnthology.LAnthologyPanel.LPanelVoyageRead();
    }

    internal void PCorpusVoyageShow(bool past, bool future)
    {
        PCorpusEarlier.IsEnabled = past;
        PCorpusLater.IsEnabled = future;
    }

    private void PCorpusRetreatHandle(object sender, RoutedEventArgs e)
    {
        _pCorpusHost.PVoyageRetreatRun();
    }

    private void PCorpusAdvanceHandle(object sender, RoutedEventArgs e)
    {
        _pCorpusHost.PVoyageAdvanceRun();
    }
}
