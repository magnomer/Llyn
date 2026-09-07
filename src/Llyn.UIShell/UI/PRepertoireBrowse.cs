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

    private readonly ObservableCollection<PUsageItem> _pOccurrenceList = [];


    private IReadOnlyDictionary<string, int> _pAtlasCount = new Dictionary<string, int>();

    private string? _pVignetteSituation;

    private LCatalogOrder _pTierChoice;

    private bool _pScenarioTitleUnreadable;

    private bool _pScenarioKindUnreadable;

    private bool _pScenarioDescriptionUnreadable;

    private bool _pScenarioLoading;

    private async void PRepertoireBulletinHandle(LBulletin bulletin)
    {
        if (bulletin.LBulletinSubject == LSubject.LSubjectWorkspace)
        {
            await PEnsign.PEnsignLoad(_lEngine);
            PRepertoireReset();
            return;
        }

        PAtlasFind(PInquest.Text ?? string.Empty);
    }

    private void PInquestHandle(object sender, TextChangedEventArgs e)
    {
        PAtlasFind(PInquest.Text ?? string.Empty);
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

        string unreadable = _pRepertoireHost.PLocalizationTextRead("Display.Unreadable");
        string untitled = _pRepertoireHost.PLocalizationTextRead("Situation.Untitled");

        _pAtlasList.Clear();
        bool kept = false;
        foreach (LCatalogSituation row in read)
        {
            kept |= string.Equals(
                row.LCatalogSituationStored.LSituationId,
                _pVignetteSituation,
                StringComparison.Ordinal);
            _pAtlasList.Add(new PAtlasItem(
                row.LCatalogSituationStored,
                row.LCatalogSituationUsage,
                unreadable,
                untitled));
        }

        PAtlasEmpty.Visibility = _pAtlasList.Count == 0 ? Visibility.Visible : Visibility.Collapsed;

        if (!kept && _pVignetteSituation is not null && PScenario.Visibility != Visibility.Visible)
        {
            PRepertoireClear();
        }
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

    private void PRepertoireShow(string id)
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

        PVignetteValueShow(PVignetteTitle, situation.LSituationTitle);
        PVignetteValueShow(PVignetteKind, situation.LSituationKind);
        PVignetteValueShow(PVignetteDescription, situation.LSituationDescription);
        POccurrenceFind(id);

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
            LState.LStateSpecified => shown ?? value.LStateValueShow(),
            LState.LStateUnknown => _pRepertoireHost.PLocalizationTextRead("Display.Unreadable"),
            _ => null,
        };

        field.Text = text ?? _pRepertoireHost.PLocalizationTextRead("Situation.Unset");
        field.SetResourceReference(
            TextBlock.ForegroundProperty,
            text is null ? "Theme.Muted" : "Theme.Ink");
    }

    private void POccurrenceFind(string id)
    {
        IReadOnlyList<LUsage> read;
        try
        {
            read = _lEngine.LEngineUsageRead(id, LOwner.LOwnerSituation);
        }
        catch (Exception exception)
        {
            _pRepertoireHost.PWindowFailureShow("Situation.LoadFailed", exception);
            return;
        }

        string unreadable = _pRepertoireHost.PLocalizationTextRead("Display.Unreadable");
        string unnamed = _pRepertoireHost.PLocalizationTextRead("Situation.Unnamed");
        string meaning = _pRepertoireHost.PLocalizationTextRead("Situation.Meaning");
        string collocation = _pRepertoireHost.PLocalizationTextRead("Situation.Collocation");

        _pOccurrenceList.Clear();
        foreach (LUsage usage in read)
        {
            _pOccurrenceList.Add(new PUsageItem(
                usage,
                usage.LUsageOwner == LOwner.LOwnerCollocation ? collocation : meaning,
                unreadable,
                unnamed));
        }

        POccurrenceEmpty.Visibility = _pOccurrenceList.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
    }

    private void POccurrenceHandle(object sender, RoutedEventArgs e)
    {
        if (sender is not FrameworkElement row || row.DataContext is not PUsageItem item)
        {
            return;
        }

        if (!PRepertoireLeaveConfirm())
        {
            return;
        }

        _pRepertoireHost.PWindowEntryShow(item.PUsageItemEntry);
    }

    private void PScenarioTitleHandle(object sender, TextChangedEventArgs e)
    {
        if (_pScenarioLoading)
        {
            return;
        }

        _pScenarioTitleUnreadable = false;
        PScenarioTitle.Tag = string.Empty;
        PScenarioChangeDefer();
    }

    private void PScenarioKindHandle(object sender, TextChangedEventArgs e)
    {
        if (_pScenarioLoading)
        {
            return;
        }

        _pScenarioKindUnreadable = false;
        PScenarioKind.Tag = string.Empty;
        PScenarioChangeDefer();
    }

    private void PScenarioDescriptionHandle(object sender, TextChangedEventArgs e)
    {
        if (_pScenarioLoading)
        {
            return;
        }

        _pScenarioDescriptionUnreadable = false;
        PScenarioDescription.Tag = string.Empty;
        PScenarioChangeDefer();
    }

    private void PScenarioApply(LSituation? situation)
    {
        _pScenarioLoading = true;

        string unreadable = _pRepertoireHost.PLocalizationTextRead("Display.Unreadable");

        PScenarioTitle.Text = situation?.LSituationTitle.LStateValueShow() ?? string.Empty;
        PScenarioKind.Text = situation?.LSituationKind.LStateValueShow() ?? string.Empty;
        PScenarioDescription.Text = situation?.LSituationDescription.LStateValueShow() ?? string.Empty;

        _pScenarioTitleUnreadable = situation?.LSituationTitle.LStateValueState == LState.LStateUnknown;
        _pScenarioKindUnreadable = situation?.LSituationKind.LStateValueState == LState.LStateUnknown;
        _pScenarioDescriptionUnreadable = situation?.LSituationDescription.LStateValueState == LState.LStateUnknown;

        PScenarioTitle.Tag = _pScenarioTitleUnreadable ? unreadable : string.Empty;
        PScenarioKind.Tag = _pScenarioKindUnreadable ? unreadable : string.Empty;
        PScenarioDescription.Tag = _pScenarioDescriptionUnreadable ? unreadable : string.Empty;

        string? stored = PScenarioSituationRead();
        PScenarioRemoval.IsEnabled = stored is not null;

        _pScenarioLoading = false;

        PScenarioChangeUpdate();
    }

    private LSituation PScenarioRead(LSituation held)
    {
        return held with
        {
            LSituationTitle =
                LStateValue.LStateValueResolve(PScenarioTitle.Text, _pScenarioTitleUnreadable),
            LSituationDescription = LStateValue.LStateValueResolve(
                PScenarioDescription.Text, _pScenarioDescriptionUnreadable),
            LSituationKind =
                LStateValue.LStateValueResolve(PScenarioKind.Text, _pScenarioKindUnreadable),
        };
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
                PRepertoireShow(_pVignetteSituation);
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

    private void PScenarioDiscardHandle(object sender, RoutedEventArgs e)
    {
        if (!PRepertoireLeaveConfirm())
        {
            return;
        }

        PScenarioDraftShow(PScenarioDraftStart(PScenarioSituationRead()));
    }

    private void PScenarioStoreHandle(object sender, RoutedEventArgs e)
    {
        PScenarioChangeSave();

        string held = _pScenarioDraft;
        if (held.Length == 0)
        {
            return;
        }

        LSituation stored;
        try
        {
            stored = _lEngine.LEngineSituationCommit(held);
        }
        catch (Exception exception)
        {
            _pRepertoireHost.PWindowFailureShow("Situation.SaveFailed", exception);
            return;
        }

        _pScenarioDraft = string.Empty;
        _pVignetteSituation = stored.LSituationId;

        PAtlasFind(PInquest.Text ?? string.Empty);
        PRepertoireScribeShow(false);
        PRepertoireShow(stored.LSituationId);
    }

    private void PScenarioRemovalHandle(object sender, RoutedEventArgs e)
    {
        if (PScenarioSituationRead() is not string id)
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

    private bool PRepertoireLeaveConfirm()
    {
        return _pRepertoireHost.PWindowDiscardConfirm(PRepertoireChangeCheck());
    }

    private void PRepertoireClear()
    {
        PScenarioDraftCancel();

        _pVignetteSituation = null;
        _pOccurrenceList.Clear();

        PVignetteBody.Visibility = Visibility.Collapsed;
        PVignetteUnselected.Visibility = Visibility.Visible;
        PScenarioApply(null);
        PRepertoireScribeShow(false);
        PRepertoireMode.IsEnabled = false;
    }
}
