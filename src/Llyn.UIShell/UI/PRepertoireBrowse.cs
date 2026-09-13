using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using Llyn.Core;

namespace Llyn.UIShell;

public partial class PRepertoire
{
    private readonly ObservableCollection<PAtlasItem> _pAtlasList = [];

    private readonly ObservableCollection<POccurrenceItem> _pOccurrenceList = [];

    private IReadOnlyDictionary<long, int> _pAtlasCount = new Dictionary<long, int>();

    private long? _pVignetteSituation;

    private long? _pDisplayEntry;

    private LCatalogOrder _pTierChoice;

    private LCatalogFilter _pMeshChoice = LCatalogFilter.LCatalogFilterEmpty;

    private async void PRepertoireBulletinHandle(LBulletin bulletin)
    {
        if (bulletin.LBulletinSubject == LSubject.LSubjectDraft)
        {
            if (bulletin.LBulletinId == _pScenarioDraft)
            {
                PScenarioDraftRestore();
            }

            return;
        }

        if (bulletin.LBulletinSubject == LSubject.LSubjectWorkspace)
        {
            await PEnsign.PEnsignLoad(_lEngine);
            PRepertoireReset();
            return;
        }

        PAtlasFind(PInquest.Text ?? string.Empty);
        POccurrenceEntryUpdate(bulletin.LBulletinId);
    }

    private void PInquestHandle(object sender, TextChangedEventArgs e)
    {
        PAtlasFind(PInquest.Text ?? string.Empty);
    }

    private void PSortieHandle(object sender, TextChangedEventArgs e)
    {
        POccurrenceFind();
    }

    private void PMeshHandle(object sender, RoutedEventArgs e)
    {
        _pMeshChoice = PChoice.PChoiceFilterRead(PMeshList);
        _lEngine.LEngineMeshSave(_pMeshChoice);
        PMeshMark.Visibility = _pMeshChoice.LCatalogFilterActive ? Visibility.Visible : Visibility.Collapsed;
        POccurrenceFind();
    }

    internal async void PMeshRestore(LCatalogFilter filter)
    {
        _pMeshChoice = filter;
        PMeshMark.Visibility = filter.LCatalogFilterActive ? Visibility.Visible : Visibility.Collapsed;

        await PEnsign.PEnsignLoad(_lEngine);

        PChoice.PChoiceFilterBuild(PMeshList, _lEngine.LEngineLanguageRead(), filter, PMeshHandle);
    }

    private void PTierHandle(object sender, RoutedEventArgs e)
    {
        if (sender is not FrameworkElement { Tag: string choice })
        {
            return;
        }

        _pTierChoice = LCatalog.LCatalogOrderParse(choice, _pTierChoice);
        _lEngine.LEngineTierSave(_pTierChoice);
        PTierDropper.IsChecked = false;
        PAtlasFind(PInquest.Text ?? string.Empty);
    }

    internal async void PTierRestore(LCatalogOrder order)
    {
        _pTierChoice = order;
        PChoice.PChoiceOrderApply(PTierDropdown, order);

        await PEnsign.PEnsignLoad(_lEngine);

        PAtlasFind(PInquest.Text ?? string.Empty);
    }

    private void PAtlasSelect(long? id)
    {
        foreach (PAtlasItem item in _pAtlasList)
        {
            item.PAtlasItemChosen = id is not null
                && item.PAtlasItemId == id;
        }
    }

    private void PAtlasFind(string query)
    {
        IReadOnlyList<LCatalogSituation> read;
        try
        {
            read = _lEngine.LEngineSituationFind(query, _pTierChoice);
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
            PAtlasFind(PInquest.Text ?? string.Empty);
            return;
        }

        _pVignetteSituation = id;
        PAtlasSelect(id);
        POccurrenceEntryHide();

        PVignetteValueShow(PVignetteTitle, situation.LSituationTitle);
        PVignetteValueShow(PVignetteKind, situation.LSituationKind);
        PVignetteValueShow(PVignetteDescription, situation.LSituationDescription);
        POccurrenceFind();

        PVignetteBody.Visibility = Visibility.Visible;
        PVignetteUnselected.Visibility = Visibility.Collapsed;
        PRepertoireMode.IsEnabled = true;

        if (PScenario.Visibility == Visibility.Visible)
        {
            PScenarioDraftShow(PScenarioDraftStart(id));
        }
    }

    private void PVignetteValueShow(TextBlock field, LStateValue value, string? shown = null)
    {
        string? text = value.LStateValueState switch
        {
            _ when value.LStateValueUnreadable => value.LStateValueShow(),
            LState.LStateSpecified => shown ?? value.LStateValueShow(),
            LState.LStateUnknown => _pRepertoireHost.PLocalizationTextRead("Display.Unknown"),
            _ => null,
        };

        field.Text = text ?? _pRepertoireHost.PLocalizationTextRead("Situation.Unset");
        field.SetResourceReference(
            TextBlock.ForegroundProperty,
            text is null ? "Theme.Muted" : "Theme.Ink");
    }

    private void POccurrenceFind()
    {
        IReadOnlyList<LEntry> read;
        try
        {
            read = _lEngine.LEngineEntryFind(
                new LSituation(_pVignetteSituation ?? 0, LStateValue.LStateValueUnspecified, LStateValue.LStateValueUnspecified, LStateValue.LStateValueUnspecified),
                PSortie.Text ?? string.Empty,
                _pMeshChoice);
        }
        catch (Exception exception)
        {
            _pRepertoireHost.PWindowFailureShow("Situation.LoadFailed", exception);
            return;
        }

        _pOccurrenceList.Clear();
        foreach (LEntry entry in read)
        {
            _pOccurrenceList.Add(new POccurrenceItem(
                entry.LEntryId, entry.LEntryHeadword, entry.LEntryLanguage));
        }

        PTwin.PTwinNameApply(
            _pOccurrenceList,
            row => row.POccurrenceItemHeadword,
            (row, name) => row.POccurrenceItemName = name,
            row => row.POccurrenceItemId);

        POccurrenceEmpty.SetResourceReference(
            TextBlock.TextProperty,
            string.IsNullOrWhiteSpace(PSortie.Text) ? "Situation.Vacant" : "Situation.Unmatched");
        POccurrenceEmpty.Visibility = _pOccurrenceList.Count == 0 ? Visibility.Visible : Visibility.Collapsed;

        POccurrenceSelect(_pDisplayEntry);
    }

    private void POccurrenceSelect(long? id)
    {
        foreach (POccurrenceItem item in _pOccurrenceList)
        {
            item.POccurrenceItemChosen = id is not null
                && item.POccurrenceItemId == id;
        }
    }

    private void POccurrenceHandle(object sender, RoutedEventArgs e)
    {
        if (sender is not FrameworkElement row || row.DataContext is not POccurrenceItem item)
        {
            return;
        }

        if (!PRepertoireLeaveConfirm())
        {
            return;
        }

        POccurrenceEntryShow(item.POccurrenceItemId);
    }

    private void POccurrenceEntryShow(long id)
    {
        LEntryDraft? draft;
        try
        {
            draft = _lEngine.LEngineEntryLoad(id);
        }
        catch (Exception exception)
        {
            _pRepertoireHost.PWindowFailureShow("List.LoadFailed", exception);
            return;
        }

        if (draft is null)
        {
            POccurrenceEntryHide();
            POccurrenceFind();
            return;
        }

        bool editing = PScenario.Visibility == Visibility.Visible
            || PEditor.Visibility == Visibility.Visible;
        PScenarioDraftCancel();
        PScenario.Visibility = Visibility.Collapsed;
        PVignette.Visibility = Visibility.Collapsed;

        _pDisplayEntry = id;
        POccurrenceSelect(id);
        PDisplay.PDisplayShow(id, draft);
        PRepertoireMode.IsEnabled = true;

        if (editing)
        {
            PEditor.PEditorEntryShow(id);
        }

        POccurrenceScribeShow(editing);
    }

    private void POccurrenceScribeHandle(bool editing)
    {
        if (_pDisplayEntry is not long id || editing == (PEditor.Visibility == Visibility.Visible))
        {
            return;
        }

        if (!editing)
        {
            if (!PRepertoireLeaveConfirm())
            {
                POccurrenceScribeShow(true);
                return;
            }

            POccurrenceScribeShow(false);
            POccurrenceEntryShow(id);
            return;
        }

        PEditor.PEditorEntryShow(id);
        POccurrenceScribeShow(true);
    }

    private void POccurrenceScribeShow(bool editing)
    {
        _lEngine.LEngineSplitSave(editing);

        PEditor.Visibility = editing ? Visibility.Visible : Visibility.Collapsed;
        PDisplay.Visibility = editing ? Visibility.Collapsed : Visibility.Visible;
        PRepertoireStore.Visibility = editing ? Visibility.Visible : Visibility.Collapsed;
        PRepertoireViewer.IsChecked = !editing;
        PRepertoireScribe.IsChecked = editing;
    }

    private void PRepertoireStoreHandle(object sender, RoutedEventArgs e)
    {
        PEditor.PEditorEntrySave();
    }

    private void POccurrenceEntryUpdate(long id)
    {
        if (_pDisplayEntry is not long shown
            || (id > 0 && shown != id))
        {
            return;
        }

        LEntryDraft? draft;
        try
        {
            draft = _lEngine.LEngineEntryLoad(shown);
        }
        catch (Exception)
        {
            return;
        }

        if (draft is null)
        {
            if (_pVignetteSituation is long kept)
            {
                PRepertoireShow(kept);
                return;
            }

            PRepertoireClear();
            return;
        }

        PDisplay.PDisplayShow(shown, draft);
    }

    private void POccurrenceEntryHide()
    {
        bool editing = PScenario.Visibility == Visibility.Visible
            || PEditor.Visibility == Visibility.Visible;
        if (PEditor.Visibility == Visibility.Visible)
        {
            PEditor.PEditorReset();
        }

        _pDisplayEntry = null;
        POccurrenceSelect(null);
        PDisplay.PDisplayClear();
        PDisplay.Visibility = Visibility.Collapsed;
        PEditor.Visibility = Visibility.Collapsed;
        PRepertoireStore.Visibility = Visibility.Collapsed;
        PRepertoireScribeShow(editing);
        PRepertoireMode.IsEnabled = _pVignetteSituation is not null;
    }

    private void PRepertoireFreshHandle(object sender, RoutedEventArgs e)
    {
        if (!PRepertoireLeaveConfirm())
        {
            return;
        }

        PRepertoireClear();
        PRepertoireScribeShow(true);
        PScenarioDraftShow(PScenarioDraftStart(null));
        PRepertoireMode.IsEnabled = true;
    }

    private void PRepertoireScribeHandle(object sender, RoutedEventArgs e)
    {
        bool editing = ReferenceEquals(sender, PRepertoireScribe);
        if (_pDisplayEntry is not null)
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

    private void PRepertoireClear()
    {
        PScenarioDraftCancel();

        _pVignetteSituation = null;
        PAtlasSelect(null);
        POccurrenceEntryHide();
        POccurrenceFind();

        PVignetteBody.Visibility = Visibility.Collapsed;
        PVignetteUnselected.Visibility = Visibility.Visible;
        PScenarioApply(null);
        PRepertoireScribeShow(false);
        PRepertoireMode.IsEnabled = false;
    }
}
