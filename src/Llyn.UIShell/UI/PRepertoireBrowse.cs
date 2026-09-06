using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Llyn.Core;

namespace Llyn.UIShell;

public partial class PRepertoire
{
    private readonly ObservableCollection<PAtlasItem> _pAtlasList = [];

    private readonly ObservableCollection<PUsageItem> _pOccurrenceList = [];


    private IReadOnlyDictionary<string, int> _pAtlasCount = new Dictionary<string, int>();

    private string? _pVignetteSituation;

    private LCatalogOrder _pTierChoice = LCatalogOrder.LCatalogOrderName;

    private bool _pScenarioTitleUnreadable;

    private bool _pScenarioKindUnreadable;

    private bool _pScenarioDescriptionUnreadable;

    private bool _pScenarioLoading;

    private async void PRepertoireHandle(object sender, DependencyPropertyChangedEventArgs e)
    {
        if (!IsVisible)
        {
            return;
        }

        PAtlas.ItemsSource = _pAtlasList;
        POccurrence.ItemsSource = _pOccurrenceList;

        await PEnsign.PEnsignLoad(_lEngine);

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

        _pTierChoice = LCatalog.LCatalogOrderParse(choice, LCatalogOrder.LCatalogOrderName);
        PTierDropper.IsChecked = false;
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
        PRepertoireScribe.IsEnabled = true;

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
            LSituationTitle = PScenarioValueRead(PScenarioTitle.Text, _pScenarioTitleUnreadable),
            LSituationDescription =
                PScenarioValueRead(PScenarioDescription.Text, _pScenarioDescriptionUnreadable),
            LSituationKind = PScenarioValueRead(PScenarioKind.Text, _pScenarioKindUnreadable),
        };
    }

    private static LStateValue PScenarioValueRead(string text, bool unreadable)
    {
        if (!string.IsNullOrWhiteSpace(text))
        {
            return LStateValue.LStateValueCreate(text);
        }

        return unreadable ? LStateValue.LStateValueUnknown : LStateValue.LStateValueUnspecified;
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
        PRepertoireScribe.IsEnabled = true;
    }

    private void PRepertoireScribeHandle(object sender, RoutedEventArgs e)
    {
        if (PScenario.Visibility == Visibility.Visible)
        {
            if (!PRepertoireLeaveConfirm())
            {
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
            return;
        }

        PRepertoireScribeShow(true);
        PScenarioDraftShow(PScenarioDraftStart(_pVignetteSituation));
    }

    private void PRepertoireScribeShow(bool editing)
    {
        PScenario.Visibility = editing ? Visibility.Visible : Visibility.Collapsed;
        PVignette.Visibility = editing ? Visibility.Collapsed : Visibility.Visible;
        PRepertoireScribe.SetResourceReference(ButtonBase.ContentProperty, editing ? "Scribe.Read" : "Scribe.Edit");
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
        PRepertoireScribe.IsEnabled = false;
    }
}
