using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.UIDeportment;

public partial class PRepertoire
{
    private readonly ObservableCollection<PAtlasItem> _pAtlasList = [];

    private IReadOnlyDictionary<long, int> _pAtlasCount = new Dictionary<long, int>();

    private async void PRepertoireWorkspaceUpdate()
    {
        await LEnsignImage.LEnsignLoad(_pRepertoireHost.PWindowDeportment);
        PRepertoireReset();
    }

    private void PInquestHandle(object sender, TextChangedEventArgs e)
    {
        _lRepertoire.LRepertoireAtlas.LAtlasInquestSet(PInquest.Text ?? string.Empty);
    }

    private void PTierHandle(object sender, RoutedEventArgs e)
    {
        PTierDropper.IsChecked = false;
        _lRepertoire.LRepertoireAtlas.LAtlasTierSet(LChoice.LChoiceOrderRead(sender));
    }

    internal async void PRepertoireVistaRestore()
    {
        LPanel atlas = _lRepertoire.LRepertoireAtlas.LAtlasPanel;
        _lRepertoire.LRepertoireVistaRestore(_pRepertoireHost.PWindowDeportment);
        PRepertoireObserverAttach();
        PDisplay.PDisplayObserverAttach();
        PEditor.PEditorVistaRestore();
        PChoice.PChoiceOrderBuild(
            PTierList,
            "Tier",
            PTierHandle,
            [
                LCatalogOrder.LCatalogOrderName,
                LCatalogOrder.LCatalogOrderKind,
                LCatalogOrder.LCatalogOrderUsage,
            ]);
        PChoice.PChoiceOrderApply(PTierDropdown, atlas.LPanelOrder);
        PMeshRestore();

        await LEnsignImage.LEnsignLoad(_pRepertoireHost.PWindowDeportment);

        PMeshBuild();
        _lRepertoire.LRepertoireAtlas.LAtlasInquestSet(PInquest.Text ?? string.Empty);
        _lRepertoire.LRepertoireOccurrence.LOccurrenceSortieSet(PSortie.Text);
        atlas.LPanelRowsUpdate();
    }

    private void PRepertoireObserverAttach()
    {
        LPanel atlas = _lRepertoire.LRepertoireAtlas.LAtlasPanel;
        LPanel occurrence = _lRepertoire.LRepertoireOccurrence.LOccurrencePanel;
        Action<LBulletin> rows = LObserver.LObserverCreate(this, atlas.LPanelRowsUpdate);
        atlas.LPanelObserverAttach(LSubject.LSubjectVista, rows);
        atlas.LPanelObserverAttach(
            LSubject.LSubjectWorkspace, LObserver.LObserverCreate(this, PRepertoireWorkspaceUpdate));
        atlas.LPanelObserverAttach(LSubject.LSubjectSituation, rows);
        atlas.LPanelObserverAttach(LSubject.LSubjectReflex, rows);
        atlas.LPanelObserverAttach(LSubject.LSubjectSettings, rows);
        occurrence.LPanelObserverAttach(
            LSubject.LSubjectEntry, LObserver.LObserverCreate(this, occurrence.LPanelEntrySelect));
        occurrence.LPanelObserverAttach(LSubject.LSubjectEntry, rows);
        occurrence.LPanelChosenAttach(
            LSubject.LSubjectEntry, LObserver.LObserverCreate(this, _lRepertoire.LRepertoireEntryUpdate));
        occurrence.LPanelObserverAttach(
            LSubject.LSubjectVista, LObserver.LObserverCreate(this, occurrence.LPanelRowsUpdate));
    }

    private void PMeshRestore()
    {
        PMeshMark.Visibility = PLook.PLookVisibleRead(_lRepertoire.LRepertoireAtlas.LAtlasFiltered);
    }

    private void PMeshBuild()
    {
        PChoice.PChoiceFilterBuild(
            PMeshList,
            _lRepertoire.LRepertoireAtlas.LAtlasLanguageRead(),
            _lRepertoire.LRepertoireAtlas.LAtlasPanel.LPanelFilter,
            PMeshHandle);
    }

    private void PInquestClear()
    {
        PInquest.Clear();
        PMeshBuild();
        PMeshRestore();
    }

    private void PSortieHandle(object sender, TextChangedEventArgs e)
    {
        _lRepertoire.LRepertoireOccurrence.LOccurrenceSortieSet(PSortie.Text);
    }

    private void PMeshHandle(object sender, RoutedEventArgs e)
    {
        _lRepertoire.LRepertoireAtlas.LAtlasMeshSet(LChoice.LChoiceFilterRead(PMeshList));
        PMeshRestore();
    }

    private void PAtlasFind()
    {
        IReadOnlyList<LCatalogSituation> read;
        try
        {
            read = _lRepertoire.LRepertoireAtlas.LAtlasRowsRead(
                PLocalizationCatalog.PLocalizationTextRead("Display.Unknown"),
                PLocalizationCatalog.PLocalizationTextRead("Situation.Untitled"));
            _pAtlasCount = _lRepertoire.LRepertoireAtlas.LAtlasUsageRead();
        }
        catch (Exception exception)
        {
            _pRepertoireHost.PWindowFailureShow("Situation.LoadFailed", exception);
            return;
        }

        string unknown = PLocalizationCatalog.PLocalizationTextRead("Display.Unknown");
        string untitled = PLocalizationCatalog.PLocalizationTextRead("Situation.Untitled");

        List<PAtlasItem> fresh = [];
        foreach (LCatalogSituation row in read)
        {
            fresh.Add(new PAtlasItem(
                row.LCatalogSituationStored,
                row.LCatalogSituationName,
                row.LCatalogSituationUsage,
                unknown,
                untitled,
                row.LCatalogSituationChosen));
        }

        LSplice.LSpliceApply(
            _pAtlasList, fresh, PAtlasItem.PAtlasItemMatch, PAtlasItem.PAtlasItemSync);

        PAtlasEmpty.Visibility = PLook.PLookVisibleRead(_pAtlasList.Count == 0);

        PVignetteTally.Text = PRepertoireTallyRead(_lRepertoire.LRepertoireAtlas.LAtlasChosen);
        PScenarioTally.Text = PRepertoireTallyRead(PScenarioDesk.LDeskStoredRead());
        _lRepertoire.LRepertoireRowsApply(read);
    }

    private void PAtlasHandle(object sender, RoutedEventArgs e)
    {
        _lRepertoire.LRepertoireSelect(
            PSender.PSenderSourceRead<PAtlasItem>(e)?.PAtlasItemId, _pRepertoireHost.PVoyageRecord);
    }

    private void PAtlasApply(FrameworkElement container, object item, string? _)
    {
        if (item is not PAtlasItem atlas)
        {
            return;
        }

        if (PLook.PLookPartFind<Button>(container, "PAtlasRow") is Button row)
        {
            if (atlas.PAtlasItemChosen)
            {
                row.Tag = "Chosen";
            }
            else
            {
                row.ClearValue(TagProperty);
            }

            row.Click -= PAtlasHandle;
            row.Click += PAtlasHandle;
        }

        if (PLook.PLookPartFind<PIconImage>(container, "PAtlasIcon") is PIconImage icon)
        {
            icon.PIconSource = PIcon.PIconResolve("situation", 16);
        }

        if (PLook.PLookPartFind<TextBlock>(container, "PAtlasTitle") is TextBlock title)
        {
            title.Text = atlas.PAtlasItemTitle;
        }

        if (PLook.PLookPartFind<TextBlock>(container, "PAtlasKind") is TextBlock kind)
        {
            kind.Text = atlas.PAtlasItemKind;
        }

        if (PLook.PLookPartFind<TextBlock>(container, "PAtlasTally") is TextBlock tally)
        {
            tally.Text = atlas.PAtlasItemCount;
        }
    }

    internal void PAtlasSituationShow(long id)
    {
        _lRepertoire.LRepertoireSituationShow(id);
    }

    private void PRepertoireBinHandle(object sender, RoutedEventArgs e)
    {
        _lRepertoire.LRepertoireDelete();
    }

    private void PRepertoireScribeHandle(object sender, RoutedEventArgs e)
    {
        _lRepertoire.LRepertoireScribeSet(ReferenceEquals(sender, PRepertoireScribe));
    }

    internal void PRepertoireScribeRestore(bool editing)
    {
        _lRepertoire.LRepertoireAtlas.LAtlasPanel.LPanelScribeRestore(editing);
    }

    internal bool PRepertoireLeaveConfirm()
    {
        return _lRepertoire.LRepertoireLeaveConfirm();
    }

    internal long PRepertoireVoyageRead()
    {
        return _lRepertoire.LRepertoireAtlas.LAtlasPanel.LPanelVoyageRead();
    }

    internal void PRepertoireVoyageShow(bool past, bool future)
    {
        PRepertoireEarlier.IsEnabled = past;
        PRepertoireLater.IsEnabled = future;
    }

    private void PRepertoireRetreatHandle(object sender, RoutedEventArgs e)
    {
        _pRepertoireHost.PVoyageRetreatRun();
    }

    private void PRepertoireAdvanceHandle(object sender, RoutedEventArgs e)
    {
        _pRepertoireHost.PVoyageAdvanceRun();
    }
}
