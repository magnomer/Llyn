using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.UIVeneer;

public partial class PRepertoire
{
    private readonly ObservableCollection<PAtlasItem> _pAtlasList = [];

    private IReadOnlyDictionary<long, int> _pAtlasCount = new Dictionary<long, int>();

    private LVista? _pRepertoireVista;

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
        _pOccurrenceVista?.LVistaQuerySet(PSortie.Text);
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
        vista.LVistaObserverAttach(LSubject.LSubjectWorkspace, new PObserver(this, PRepertoireWorkspaceUpdate));
        vista.LVistaObserverAttach(LSubject.LSubjectSituation, new PObserver(this, PAtlasFind));
        vista.LVistaObserverAttach(LSubject.LSubjectReflex, new PObserver(this, PAtlasFind));
        vista.LVistaObserverAttach(LSubject.LSubjectSettings, new PObserver(this, PAtlasFind));
        occurrence.LVistaObserverAttach(LSubject.LSubjectEntry, new PObserver(this, POccurrenceEntryUpdate));
        occurrence.LVistaChosenAttach(LSubject.LSubjectEntry, new PObserver(this, PRepertoireEntryUpdate));
        occurrence.LVistaObserverAttach(LSubject.LSubjectVista, new PObserver(this, POccurrenceFind));
        PDisplay.PDisplayVistaRestore(occurrence);
        PTierRestore();
        PMeshRestore();

        await PEnsign.PEnsignLoad(_lEngine);

        PChoice.PChoiceFilterBuild(PMeshList, _lEngine.LEngineLanguageRead(), vista.LVistaFilter, PMeshHandle);
        vista.LVistaQuerySet(PInquest.Text ?? string.Empty);
        occurrence.LVistaQuerySet(PSortie.Text);
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
        if (_pRepertoireVista is not LVista vista)
        {
            PMeshMark.Visibility = Visibility.Collapsed;
            return;
        }

        PMeshMark.Visibility = vista.LVistaFiltered ? Visibility.Visible : Visibility.Collapsed;
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
            read = _lEngine.LEngineSituationFind(_pRepertoireVista,
                PLocalizationCatalog.PLocalizationTextRead("Display.Unknown"),
                PLocalizationCatalog.PLocalizationTextRead("Situation.Untitled"));
            _pAtlasCount = _lEngine.LEngineUsageRead(LOwner.LOwnerSituation);
        }
        catch (Exception exception)
        {
            _pRepertoireHost.PWindowFailureShow("Situation.LoadFailed", exception);
            return;
        }

        string unknown = PLocalizationCatalog.PLocalizationTextRead("Display.Unknown");
        string untitled = PLocalizationCatalog.PLocalizationTextRead("Situation.Untitled");

        List<PAtlasItem> fresh = [];
        bool kept = false;
        foreach (LCatalogSituation row in read)
        {
            kept |= row.LCatalogSituationChosen;
            fresh.Add(new PAtlasItem(
                row.LCatalogSituationStored,
                row.LCatalogSituationName,
                row.LCatalogSituationUsage,
                unknown,
                untitled,
                row.LCatalogSituationChosen)
            {
            });
        }

        PSplice.PSpliceApply(
            _pAtlasList, fresh, PAtlasItem.PAtlasItemMatch, PAtlasItem.PAtlasItemSync);

        PAtlasEmpty.Visibility = _pAtlasList.Count == 0 ? Visibility.Visible : Visibility.Collapsed;

        PVignetteTally.Text = PRepertoireTallyRead(_pRepertoireVista?.LVistaChosen);
        PScenarioTally.Text = PRepertoireTallyRead(PScenarioSituationRead());

        if (!kept)
        {
            if (_pRepertoireVista?.LVistaChosen is not null)
            {
                if (PScenario.Visibility != Visibility.Visible)
                {
                    PRepertoireClear();
                }
            }
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
            _pRepertoireVista?.LVistaSelect(id);
            situation = _pRepertoireVista?.LVistaLoad()?.LDraftSituation;
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
        PAtlasFind();
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
        if (_pOccurrenceVista?.LVistaChosen is not null)
        {
            return;
        }

        if (_pRepertoireVista?.LVistaChosen is not long id)
        {
            return;
        }

        int usage = _pAtlasCount.GetValueOrDefault(id);

        if (!_pRepertoireHost.PWindowRemovalConfirm(usage))
        {
            return;
        }

        try
        {
            _pRepertoireVista.LVistaDelete();
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
        _pRepertoireVista?.LVistaEditingSet(editing);

        PScenario.Visibility = editing ? Visibility.Visible : Visibility.Collapsed;
        PVignette.Visibility = editing ? Visibility.Collapsed : Visibility.Visible;
        PRepertoireViewer.IsChecked = !editing;
        PRepertoireScribe.IsChecked = editing;
        PScenarioChangeUpdate();
    }

    internal void PRepertoireScribeRestore(bool editing)
    {
        if (editing)
        {
            if (_pRepertoireVista?.LVistaChosen is null)
            {
                return;
            }
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
        PAtlasFind();
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
