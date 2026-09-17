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

    private async void PRepertoireBulletinHandle(LBulletin bulletin)
    {
        if (bulletin.LBulletinSubject == LSubject.LSubjectVista)
        {
            if (_pRepertoireVista is not null && bulletin.LBulletinId == _pRepertoireVista.LVistaId)
            {
                PAtlasFind();
            }

            return;
        }

        if (bulletin.LBulletinSubject == LSubject.LSubjectDraft)
        {
            if (bulletin.LBulletinId == PScenarioDraft)
            {
                PScenarioDraftRestore();
            }

            return;
        }

        if (bulletin.LBulletinSubject == LSubject.LSubjectTenure)
        {
            if (bulletin.LBulletinId == PScenarioDraft)
            {
                PScenarioChangeUpdate();
            }

            return;
        }

        if (bulletin.LBulletinSubject == LSubject.LSubjectWorkspace)
        {
            await PEnsign.PEnsignLoad(_lEngine);
            PRepertoireReset();
            return;
        }

        if (bulletin.LBulletinSubject == LSubject.LSubjectEntry
            && bulletin.LBulletinId > 0
            && _pDisplayEntry is null
            && IsVisible
            && PEditor.Visibility == Visibility.Visible)
        {
            _pDisplayEntry = bulletin.LBulletinId;
        }

        if (!PBulletin.PBulletinEntryCheck(bulletin.LBulletinSubject))
        {
            return;
        }

        PAtlasFind();
        POccurrenceEntryUpdate(bulletin.LBulletinId);
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

    internal async void PRepertoireVistaRestore(LVista vista)
    {
        _pRepertoireVista = vista;
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

    private void PAtlasSelect(long? id)
    {
        foreach (PAtlasItem item in _pAtlasList)
        {
            item.PAtlasItemChosen = id is not null
                && item.PAtlasItemId == id;
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
            kept |= row.LCatalogSituationStored.LSituationId == _pVignetteSituation;
            _pAtlasList.Add(new PAtlasItem(
                row.LCatalogSituationStored,
                row.LCatalogSituationUsage,
                unknown,
                untitled));
        }

        PAtlasEmpty.Visibility = _pAtlasList.Count == 0 ? Visibility.Visible : Visibility.Collapsed;

        PAtlasSelect(_pVignetteSituation);
        PVignetteTally.Text = PRepertoireTallyRead(_pVignetteSituation);
        PScenarioTally.Text = PRepertoireTallyRead(PScenarioSituationRead());

        if (!kept && _pVignetteSituation is not null && PScenario.Visibility != Visibility.Visible)
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

        _pVignetteSituation = id;
        PAtlasSelect(id);
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
        if (_pDisplayEntry is not null || _pVignetteSituation is not long id)
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
        if (_pDisplayEntry is not null || PEditor.Visibility == Visibility.Visible)
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

            if (_pVignetteSituation is not null)
            {
                PRepertoireShow(_pVignetteSituation.Value);
                return;
            }

            PRepertoireClear();
            return;
        }

        if (_pVignetteSituation is null)
        {
            PRepertoireClear();
            return;
        }

        PRepertoireScribeShow(true);
        PScenarioDraftShow(PScenarioDraftStart(_pVignetteSituation));
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
        if (editing && _pVignetteSituation is null)
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
        return _pVignetteSituation ?? 0;
    }

    private void PRepertoireClear()
    {
        PScenarioDraftCancel();

        _pVignetteSituation = null;
        PAtlasSelect(null);
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
