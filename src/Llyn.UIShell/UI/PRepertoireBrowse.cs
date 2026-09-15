using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using Llyn.Core;

namespace Llyn.UIShell;

public partial class PRepertoire
{
    private readonly ObservableCollection<PAtlasItem> _pAtlasList = [];

    private IReadOnlyDictionary<long, int> _pAtlasCount = new Dictionary<long, int>();

    private long? _pVignetteSituation;

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

        if (bulletin.LBulletinSubject == LSubject.LSubjectEntry
            && bulletin.LBulletinId > 0
            && _pDisplayEntry is null
            && IsVisible
            && PEditor.Visibility == Visibility.Visible)
        {
            _pDisplayEntry = bulletin.LBulletinId;
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
            PAtlasFind(PInquest.Text ?? string.Empty);
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
        PAtlasFind(PInquest.Text ?? string.Empty);
    }

    private string? PVignetteTextRead(LStateValue value)
    {
        return value.LStateValueState switch
        {
            _ when value.LStateValueUnreadable => value.LStateValueShow(),
            LState.LStateSpecified => value.LStateValueShow(),
            LState.LStateUnknown => _pRepertoireHost.PLocalizationTextRead("Display.Unknown"),
            _ => null,
        };
    }

    private void PVignetteTitleShow(LStateValue value)
    {
        string? text = PVignetteTextRead(value);

        PVignetteTitle.Text = text ?? _pRepertoireHost.PLocalizationTextRead("Situation.Untitled");
        PVignetteTitle.SetResourceReference(
            TextBlock.ForegroundProperty,
            text is null ? "Theme.Muted" : "Theme.Ink");
    }

    private void PVignetteKindShow(LStateValue value)
    {
        string? text = PVignetteTextRead(value);

        PVignetteKind.Text = text ?? string.Empty;
        PVignetteChip.Visibility = text is null ? Visibility.Collapsed : Visibility.Visible;
    }

    private void PVignetteDescriptionShow(LStateValue value)
    {
        string? text = PVignetteTextRead(value);

        PMarkdown.PMarkdownShow(PVignetteDescription, text);
        PVignetteDescriptionSection.Visibility = text is null ? Visibility.Collapsed : Visibility.Visible;
    }

    private void PVignetteMediaShow(LSituation? situation)
    {
        PVignettePicture.DataContext = situation;
        PVignetteVideo.DataContext = situation;
    }

    private string PRepertoireTallyRead(long? id)
    {
        int count = id is long stored && _pAtlasCount.TryGetValue(stored, out int usage) ? usage : 0;

        return count switch
        {
            0 => _pRepertoireHost.PLocalizationTextRead("Situation.UsageNone"),
            1 => _pRepertoireHost.PLocalizationTextRead("Situation.UsageOne"),
            _ => $"{count.ToString(CultureInfo.CurrentCulture)} "
                + _pRepertoireHost.PLocalizationTextRead("Situation.UsageMany"),
        };
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
