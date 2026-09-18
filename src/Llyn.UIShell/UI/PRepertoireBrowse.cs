using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.UIShell;

public partial class PRepertoire
{
    private readonly ObservableCollection<PAtlasItem> _pAtlasList = [];

    private IReadOnlyDictionary<long, int> _pAtlasCount = new Dictionary<long, int>();

    private LVista? _pRepertoireVista;

    private void PScenarioDraftUpdate(LBulletin bulletin)
    {
        if (bulletin.LBulletinId == PScenarioDraft)
        {
            PScenarioDraftRestore();
        }
    }

    private void PScenarioTenureUpdate(LBulletin bulletin)
    {
        if (bulletin.LBulletinId == PScenarioDraft)
        {
            PScenarioChangeUpdate();
        }
    }

    private async void PRepertoireWorkspaceUpdate()
    {
        await PEnsign.PEnsignLoad(_lEngine);
        PRepertoireReset();
    }

    private void PInquestHandle(object sender, TextChangedEventArgs e)
    {
        _pRepertoireVista?.LVistaQuerySet(PInquest.Text ?? string.Empty);
    }

    private void PSortieHandle(object sender, TextChangedEventArgs e)
    {
        POccurrenceFind();
    }

    private void PMeshHandle(object sender, RoutedEventArgs e)
    {
        if (_pRepertoireVista is null)
        {
            return;
        }

        _pRepertoireVista.LVistaFilterSet(PChoice.PChoiceFilterRead(PMeshList));
        PMeshRestore();
    }

    private void PTierHandle(object sender, RoutedEventArgs e)
    {
        if (sender is not FrameworkElement { Tag: string choice } || _pRepertoireVista is null)
        {
            return;
        }

        PTierDropper.IsChecked = false;
        _pRepertoireVista.LVistaOrderSet(LCatalog.LCatalogOrderParse(choice, _pRepertoireVista.LVistaOrder));
    }

    internal async void PRepertoireVistaRestore(LVista vista, LVista occurrence)
    {
        _pRepertoireVista = vista;
        _pOccurrenceVista = occurrence;
        vista.LVistaObserverAttach(LSubject.LSubjectVista, new PObserver(this, PAtlasFind));
        vista.LVistaObserverAttach(LSubject.LSubjectDraft, new PObserver(this, PScenarioDraftUpdate));
        vista.LVistaObserverAttach(LSubject.LSubjectTenure, new PObserver(this, PScenarioTenureUpdate));
        vista.LVistaObserverAttach(LSubject.LSubjectWorkspace, new PObserver(this, PRepertoireWorkspaceUpdate));
        vista.LVistaObserverAttach(LSubject.LSubjectSituation, new PObserver(this, PAtlasFind));
        vista.LVistaObserverAttach(LSubject.LSubjectReflex, new PObserver(this, PAtlasFind));
        vista.LVistaObserverAttach(LSubject.LSubjectSettings, new PObserver(this, PAtlasFind));
        occurrence.LVistaObserverAttach(LSubject.LSubjectEntry, new PObserver(this, POccurrenceEntryUpdate));
        occurrence.LVistaChosenAttach(LSubject.LSubjectEntry, new PObserver(this, PRepertoireEntryUpdate));
        PDisplay.PDisplayVistaRestore(occurrence);
        PTierRestore();
        PMeshRestore();

        await PEnsign.PEnsignLoad(_lEngine);

        PChoice.PChoiceFilterBuild(PMeshList, _lEngine.LEngineLanguageRead(), vista.LVistaFilter, PMeshHandle);
        vista.LVistaQuerySet(PInquest.Text ?? string.Empty);
        PAtlasFind();
    }

    private void PTierRestore()
    {
        if (_pRepertoireVista is not null)
        {
            PChoice.PChoiceOrderApply(PTierDropdown, _pRepertoireVista.LVistaOrder);
        }
    }

    private void PMeshRestore()
    {
        bool active = _pRepertoireVista?.LVistaFilter.LCatalogFilterActive == true;
        PMeshMark.Visibility = active ? Visibility.Visible : Visibility.Collapsed;
    }

    private void PAtlasChosenApply()
    {
        long? chosen = _pRepertoireVista?.LVistaChosen;
        foreach (PAtlasItem item in _pAtlasList)
        {
            item.PAtlasItemChosen = chosen is not null
                && item.PAtlasItemId == chosen;
        }
    }

    private void PAtlasFind()
    {
        if (_pRepertoireVista is null)
        {
            return;
        }

        IReadOnlyList<LCatalogSituation> read;
        try
        {
            read = _lEngine.LEngineSituationFind(_pRepertoireVista);
            _pAtlasCount = _lEngine.LEngineUsageRead(LOwner.LOwnerSituation);
        }
        catch (Exception exception)
        {
            _pRepertoireHost.PWindowFailureShow("Situation.LoadFailed", exception);
            return;
        }

        string unknown = _pRepertoireHost.PLocalizationTextRead("Display.Unknown");
        string untitled = _pRepertoireHost.PLocalizationTextRead("Situation.Untitled");

        _pAtlasList.Clear();
        bool kept = false;
        foreach (LCatalogSituation row in read)
        {
            kept |= row.LCatalogSituationChosen;
            _pAtlasList.Add(new PAtlasItem(
                row.LCatalogSituationStored,
                row.LCatalogSituationUsage,
                unknown,
                untitled)
            {
                PAtlasItemChosen = row.LCatalogSituationChosen,
            });
        }

        PAtlasEmpty.Visibility = _pAtlasList.Count == 0 ? Visibility.Visible : Visibility.Collapsed;

        PVignetteTally.Text = PRepertoireTallyRead(_pRepertoireVista?.LVistaChosen);
        PScenarioTally.Text = PRepertoireTallyRead(PScenarioSituationRead());

        if (!kept && _pRepertoireVista?.LVistaChosen is not null && PScenario.Visibility != Visibility.Visible)
        {
            PRepertoireClear();
        }

        POccurrenceFind();
    }

    private void PAtlasHandle(object sender, RoutedEventArgs e)
    {
        if (sender is not FrameworkElement row || row.DataContext is not PAtlasItem item)
        {
            return;
        }

        if (!PRepertoireLeaveConfirm())
        {
            return;
        }

        PRepertoireShow(item.PAtlasItemId);
    }

    internal void PAtlasSituationShow(long id)
    {
        PRepertoireShow(id);
    }

    private void PRepertoireShow(long id)
    {
        LSituation? situation;
        try
        {
            situation = _lEngine.LEngineSituationRead(id);
        }
        catch (Exception exception)
        {
            _pRepertoireHost.PWindowFailureShow("Situation.LoadFailed", exception);
            return;
        }

        if (situation is null)
        {
            PRepertoireClear();
            PAtlasFind();
            return;
        }

        _pRepertoireVista?.LVistaSelect(id);
        PAtlasChosenApply();
        POccurrenceEntryHide();

        PVignetteTitleShow(situation.LSituationTitle);
        PVignetteKindShow(situation.LSituationKind);
        PVignetteDescriptionShow(situation.LSituationDescription);
        PVignetteMediaShow(situation);
        PVignetteTally.Text = PRepertoireTallyRead(id);
        POccurrenceFind();

        PVignetteBody.Visibility = Visibility.Visible;
        PVignetteUnselected.Visibility = Visibility.Collapsed;
        PRepertoireMode.IsEnabled = true;
        PRepertoireBin.IsEnabled = true;

        if (PScenario.Visibility == Visibility.Visible)
        {
            PScenarioDraftShow(PScenarioDraftStart(id));
        }
    }

    private void PRepertoireBinHandle(object sender, RoutedEventArgs e)
    {
        if (_pOccurrenceVista?.LVistaChosen is not null || _pRepertoireVista?.LVistaChosen is not long id)
        {
            return;
        }

        int usage = _pAtlasCount.TryGetValue(id, out int count) ? count : 0;

        if (!_pRepertoireHost.PWindowRemovalConfirm(usage))
        {
            return;
        }

        try
        {
            _lEngine.LEngineSituationDelete(id, usage > 0);
        }
        catch (Exception exception)
        {
            _pRepertoireHost.PWindowFailureShow("Situation.DeleteFailed", exception);
            return;
        }

        PRepertoireScribeShow(false);
        PRepertoireClear();
        PAtlasFind();
    }

    private void PRepertoireScribeHandle(object sender, RoutedEventArgs e)
    {
        bool editing = ReferenceEquals(sender, PRepertoireScribe);
        if (_pOccurrenceVista?.LVistaChosen is not null || PEditor.Visibility == Visibility.Visible)
        {
            POccurrenceScribeHandle(editing);
            return;
        }

        if (editing == (PScenario.Visibility == Visibility.Visible))
        {
            return;
        }

        if (!editing)
        {
            if (!PRepertoireLeaveConfirm())
            {
                PRepertoireScribeShow(true);
                return;
            }

            PScenarioDraftCancel();
            PRepertoireScribeShow(false);

            if (_pRepertoireVista?.LVistaChosen is long chosen)
            {
                PRepertoireShow(chosen);
                return;
            }

            PRepertoireClear();
            return;
        }

        if (_pRepertoireVista?.LVistaChosen is not long shown)
        {
            PRepertoireClear();
            return;
        }

        PRepertoireScribeShow(true);
        PScenarioDraftShow(PScenarioDraftStart(shown));
    }

    private void PRepertoireScribeShow(bool editing)
    {
        _lEngine.LEngineSplitSave(editing);

        PScenario.Visibility = editing ? Visibility.Visible : Visibility.Collapsed;
        PVignette.Visibility = editing ? Visibility.Collapsed : Visibility.Visible;
        PRepertoireViewer.IsChecked = !editing;
        PRepertoireScribe.IsChecked = editing;
        PScenarioChangeUpdate();
    }

    internal void PRepertoireScribeRestore(bool editing)
    {
        if (editing && _pRepertoireVista?.LVistaChosen is null)
        {
            return;
        }

        if (editing)
        {
            PRepertoireMode.IsEnabled = true;
        }

        PRepertoireScribeShow(editing);
    }

    internal bool PRepertoireLeaveConfirm()
    {
        return _pRepertoireHost.PWindowDiscardConfirm(PRepertoireChangeCheck(), PRepertoireDraftFinish);
    }

    internal long PRepertoireVoyageRead()
    {
        return _pRepertoireVista?.LVistaChosen ?? 0;
    }

    private void PRepertoireClear()
    {
        PScenarioDraftCancel();

        _pRepertoireVista?.LVistaSelect(null);
        PAtlasChosenApply();
        POccurrenceEntryHide();
        POccurrenceFind();

        PVignetteBody.Visibility = Visibility.Collapsed;
        PVignetteUnselected.Visibility = Visibility.Visible;
        PVignetteMediaShow(null);
        PScenarioApply(null);
        PRepertoireScribeShow(false);
        PRepertoireMode.IsEnabled = false;
        PRepertoireBin.IsEnabled = false;
    }
}
