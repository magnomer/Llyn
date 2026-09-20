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

    private async void PRepertoireWorkspaceUpdate()
    {
        await PEnsign.PEnsignLoad(_pRepertoireHost.PWindowDeportment);
        PRepertoireReset();
    }

    private void PInquestHandle(object sender, TextChangedEventArgs e)
    {
        _lRepertoire.LRepertoireInquestSet(PInquest.Text ?? string.Empty);
    }

    private void PSortieHandle(object sender, TextChangedEventArgs e)
    {
        _lRepertoire.LRepertoireSortieSet(PSortie.Text);
    }

    private void PMeshHandle(object sender, RoutedEventArgs e)
    {
        _lRepertoire.LRepertoireMeshSet(PChoice.PChoiceFilterRead(PMeshList));
        PMeshRestore();
    }

    private void PTierHandle(object sender, RoutedEventArgs e)
    {
        if (sender is not FrameworkElement { Tag: string choice })
        {
            return;
        }

        PTierDropper.IsChecked = false;
        _lRepertoire.LRepertoireTierSet(choice);
    }

    internal async void PRepertoireVistaRestore()
    {
        _lRepertoire.LRepertoireVistaRestore(_pRepertoireHost.PWindowPosture);
        _lRepertoire.LRepertoireObserverAttach(LSubject.LSubjectVista, PObserver.PObserverCreate(this, PAtlasFind));
        _lRepertoire.LRepertoireObserverAttach(
            LSubject.LSubjectWorkspace, PObserver.PObserverCreate(this, PRepertoireWorkspaceUpdate));
        _lRepertoire.LRepertoireObserverAttach(LSubject.LSubjectSituation, PObserver.PObserverCreate(this, PAtlasFind));
        _lRepertoire.LRepertoireObserverAttach(LSubject.LSubjectReflex, PObserver.PObserverCreate(this, PAtlasFind));
        _lRepertoire.LRepertoireObserverAttach(LSubject.LSubjectSettings, PObserver.PObserverCreate(this, PAtlasFind));
        _lRepertoire.LRepertoireOccurrenceAttach(
            LSubject.LSubjectEntry, PObserver.PObserverCreate(this, POccurrenceEntryUpdate));
        _lRepertoire.LRepertoireEntryAttach(
            LSubject.LSubjectEntry, PObserver.PObserverCreate(this, PRepertoireEntryUpdate));
        _lRepertoire.LRepertoireOccurrenceAttach(
            LSubject.LSubjectVista, PObserver.PObserverCreate(this, POccurrenceFind));
        PDisplay.PDisplayObserverAttach();
        PEditor.PEditorVistaRestore();
        PChoice.PChoiceOrderApply(PTierDropdown, _lRepertoire.LRepertoireOrder);
        PMeshRestore();

        await PEnsign.PEnsignLoad(_pRepertoireHost.PWindowDeportment);

        PChoice.PChoiceFilterBuild(
            PMeshList, _lRepertoire.LRepertoireLanguageRead(), _lRepertoire.LRepertoireFilter, PMeshHandle);
        _lRepertoire.LRepertoireInquestSet(PInquest.Text ?? string.Empty);
        _lRepertoire.LRepertoireSortieSet(PSortie.Text);
        PAtlasFind();
    }

    private void PMeshRestore()
    {
        PMeshMark.Visibility = _lRepertoire.LRepertoireFiltered ? Visibility.Visible : Visibility.Collapsed;
    }

    private void PAtlasFind()
    {
        IReadOnlyList<LCatalogSituation> read;
        try
        {
            read = _lRepertoire.LRepertoireRowsRead(
                PLocalizationCatalog.PLocalizationTextRead("Display.Unknown"),
                PLocalizationCatalog.PLocalizationTextRead("Situation.Untitled"));
            _pAtlasCount = _lRepertoire.LRepertoireUsageRead();
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

        PVignetteTally.Text = PRepertoireTallyRead(_lRepertoire.LRepertoireChosen);
        PScenarioTally.Text = PRepertoireTallyRead(PScenarioSituationRead());

        if (!kept)
        {
            if (_lRepertoire.LRepertoireChosen is not null)
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
            _lRepertoire.LRepertoireSelect(id);
            situation = _lRepertoire.LRepertoireLoad();
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

        _lRepertoire.LRepertoireSelect(id);
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
        if (_lRepertoire.LRepertoireOccurrenceChosen is not null)
        {
            return;
        }

        if (_lRepertoire.LRepertoireChosen is not long id)
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
            _lRepertoire.LRepertoireDelete();
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
        if (_lRepertoire.LRepertoireOccurrenceChosen is not null || PEditor.Visibility == Visibility.Visible)
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

            if (_lRepertoire.LRepertoireChosen is long chosen)
            {
                PRepertoireShow(chosen);
                return;
            }

            PRepertoireClear();
            return;
        }

        if (_lRepertoire.LRepertoireChosen is not long shown)
        {
            PRepertoireClear();
            return;
        }

        PRepertoireScribeShow(true);
        PScenarioDraftShow(PScenarioDraftStart(shown));
    }

    private void PRepertoireScribeShow(bool editing)
    {
        _lRepertoire.LRepertoireScenarioSet(editing);

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
            if (_lRepertoire.LRepertoireChosen is null)
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
        return _lRepertoire.LRepertoireChosen ?? 0;
    }

    private void PRepertoireClear()
    {
        PScenarioDraftCancel();

        _lRepertoire.LRepertoireSelect(null);
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
