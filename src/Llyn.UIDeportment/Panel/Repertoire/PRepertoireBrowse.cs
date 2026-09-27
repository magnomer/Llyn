using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using Llyn.Conduct;
using Llyn.Core;

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
        _lRepertoire.LRepertoireAtlas.LAtlasTierSet(QChoice.QChoiceOrderRead(sender));
    }

    internal async void PRepertoireVistaRestore()
    {
        LPanel atlas = _lRepertoire.LRepertoireAtlas.LAtlasPanel;
        PRepertoireObserverAttach();
        PDisplay.PDisplayObserverAttach();
        PEditor.PEditorVistaRestore();
        QChoice.QChoiceOrderBuild(
            PTierList,
            "Tier",
            PTierHandle,
            [
                CCatalogOrder.CCatalogOrderName,
                CCatalogOrder.CCatalogOrderKind,
                CCatalogOrder.CCatalogOrderUsage,
            ]);
        QChoice.QChoiceOrderApply(PTierDropdown, atlas.LPanelOrder);
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
        Action<CBulletin> rows = LObserver.LObserverCreate<CBulletin>(this, atlas.LPanelRowsUpdate);
        atlas.LPanelObserverAttach(CSubject.CSubjectVista, rows);
        atlas.LPanelObserverAttach(
            CSubject.CSubjectWorkspace, LObserver.LObserverCreate<CBulletin>(this, PRepertoireWorkspaceUpdate));
        atlas.LPanelObserverAttach(CSubject.CSubjectSituation, rows);
        atlas.LPanelObserverAttach(CSubject.CSubjectReflex, rows);
        atlas.LPanelObserverAttach(CSubject.CSubjectSettings, rows);
        occurrence.LPanelObserverAttach(
            CSubject.CSubjectEntry, LObserver.LObserverCreate<CBulletin>(this, occurrence.LPanelEntrySelect));
        occurrence.LPanelObserverAttach(CSubject.CSubjectEntry, rows);
        occurrence.LPanelChosenAttach(
            CSubject.CSubjectEntry, LObserver.LObserverCreate<CBulletin>(this, _lRepertoire.LRepertoireEntryUpdate));
        occurrence.LPanelObserverAttach(
            CSubject.CSubjectVista, LObserver.LObserverCreate<CBulletin>(this, occurrence.LPanelRowsUpdate));
    }

    private void PMeshRestore()
    {
        PMeshMark.Visibility = QLook.QLookVisibleRead(_lRepertoire.LRepertoireAtlas.LAtlasFiltered);
    }

    private void PMeshBuild()
    {
        QChoice.QChoiceFilterBuild(
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
        _lRepertoire.LRepertoireAtlas.LAtlasMeshSet(QChoice.QChoiceFilterRead(PMeshList));
        PMeshRestore();
    }

    private void PAtlasFind()
    {
        IReadOnlyList<LCatalogSituation> read;
        try
        {
            read = _lRepertoire.LRepertoireAtlas.LAtlasRowsRead(
                QLocalizationCatalog.QLocalizationTextRead("Display.Unknown"),
                QLocalizationCatalog.QLocalizationTextRead("Situation.Untitled"));
            _pAtlasCount = _lRepertoire.LRepertoireAtlas.LAtlasUsageRead();
        }
        catch (Exception exception)
        {
            _pRepertoireHost.PWindowFailureShow("Situation.LoadFailed", exception);
            return;
        }

        string unknown = QLocalizationCatalog.QLocalizationTextRead("Display.Unknown");
        string untitled = QLocalizationCatalog.QLocalizationTextRead("Situation.Untitled");

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

        PAtlasEmpty.Visibility = QLook.QLookVisibleRead(_pAtlasList.Count == 0);

        PVignetteTally.Text = PRepertoireTallyRead(_lRepertoire.LRepertoireAtlas.LAtlasChosen);
        PScenarioTally.Text = PRepertoireTallyRead(PScenarioDesk.LDeskStoredRead());
        _lRepertoire.LRepertoireRowsApply(read);
    }

    private void PAtlasHandle(object sender, RoutedEventArgs e)
    {
        _lRepertoire.LRepertoireSelect(
            QSender.QSenderSourceRead<PAtlasItem>(e)?.PAtlasItemId, _pRepertoireHost.PVoyageRecord);
    }

    private void PAtlasApply(FrameworkElement container, object item, string? _)
    {
        if (item is not PAtlasItem atlas)
        {
            return;
        }

        if (QLook.QLookPartFind<Button>(container, "PAtlasRow") is Button row)
        {
            if (atlas.PAtlasItemChosen)
            {
                row.SetValue(QLook.QLookCueProperty, QLookCue.QLookCueChosen);
            }
            else
            {
                row.ClearValue(QLook.QLookCueProperty);
            }

            row.Click -= PAtlasHandle;
            row.Click += PAtlasHandle;
        }

        if (QLook.QLookPartFind<QIconImage>(container, "PAtlasIcon") is QIconImage icon)
        {
            icon.QIconSource = QIcon.QIconResolve("situation", 16);
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PAtlasTitle") is TextBlock title)
        {
            title.Text = atlas.PAtlasItemTitle;
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PAtlasKind") is TextBlock kind)
        {
            kind.Text = atlas.PAtlasItemKind;
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PAtlasTally") is TextBlock tally)
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
