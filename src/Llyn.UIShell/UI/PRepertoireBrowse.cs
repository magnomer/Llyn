using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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

    private string _pTierChoice = "Title";

    private LSituation? _pScenarioSituation;


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

        _pTierChoice = choice;
        PTierDropper.IsChecked = false;
        PAtlasFind(PInquest.Text ?? string.Empty);
    }

    private IEnumerable<LSituation> PAtlasSort(IReadOnlyList<LSituation> situations)
    {
        return _pTierChoice switch
        {
            "Kind" => situations.OrderBy(
                situation => situation.LSituationKind.LStateValueShow(), StringComparer.CurrentCultureIgnoreCase),
            "Usage" => situations.OrderByDescending(PAtlasCountRead),
            _ => situations.OrderBy(
                situation => situation.LSituationTitle.LStateValueShow(), StringComparer.CurrentCultureIgnoreCase)
        };
    }

    private int PAtlasCountRead(LSituation situation)
    {
        return _pAtlasCount.TryGetValue(situation.LSituationId, out int usage) ? usage : 0;
    }

    private void PAtlasFind(string query)
    {
        query = query.Trim();

        IReadOnlyList<LSituation> read;
        try
        {
            read = _lEngine.LEngineSituationRead();
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
        foreach (LSituation situation in PAtlasSort(read))
        {
            if (query.Length > 0 && !PInquestMatch(situation, query))
            {
                continue;
            }

            kept |= string.Equals(situation.LSituationId, _pVignetteSituation, StringComparison.Ordinal);
            _pAtlasList.Add(new PAtlasItem(situation, PAtlasCountRead(situation), unreadable, untitled));
        }

        PAtlasEmpty.Visibility = _pAtlasList.Count == 0 ? Visibility.Visible : Visibility.Collapsed;

        if (!kept && _pVignetteSituation is not null && PScenario.Visibility != Visibility.Visible)
        {
            PRepertoireClear();
        }
    }

    private bool PInquestMatch(LSituation situation, string query)
    {
        return PInquestMatch(situation.LSituationTitle.LStateValueShow(), query)
            || PInquestMatch(situation.LSituationDescription.LStateValueShow(), query)
            || PInquestMatch(situation.LSituationKind.LStateValueShow(), query);
    }

    private static bool PInquestMatch(string text, string query)
    {
        return text.Length > 0 && text.IndexOf(query, StringComparison.CurrentCultureIgnoreCase) >= 0;
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
        _pScenarioSituation = situation;

        PVignetteValueShow(PVignetteTitle, situation.LSituationTitle);
        PVignetteValueShow(PVignetteKind, situation.LSituationKind);
        PVignetteValueShow(PVignetteDescription, situation.LSituationDescription);
        POccurrenceFind(id);

        PVignetteBody.Visibility = Visibility.Visible;
        PVignetteUnselected.Visibility = Visibility.Collapsed;
        PRepertoireScribe.IsEnabled = true;

        if (PScenario.Visibility == Visibility.Visible)
        {
            PScenarioApply(situation);
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
    }

    private void PScenarioKindHandle(object sender, TextChangedEventArgs e)
    {
        if (_pScenarioLoading)
        {
            return;
        }

        _pScenarioKindUnreadable = false;
        PScenarioKind.Tag = string.Empty;
    }

    private void PScenarioDescriptionHandle(object sender, TextChangedEventArgs e)
    {
        if (_pScenarioLoading)
        {
            return;
        }

        _pScenarioDescriptionUnreadable = false;
        PScenarioDescription.Tag = string.Empty;
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

        PScenarioRemoval.IsEnabled = situation is not null;

        _pScenarioLoading = false;
    }

    private LSituation PScenarioRead()
    {
        return new LSituation(
            _pScenarioSituation?.LSituationId ?? string.Empty,
            PScenarioValueRead(PScenarioTitle.Text, _pScenarioTitleUnreadable),
            PScenarioValueRead(PScenarioDescription.Text, _pScenarioDescriptionUnreadable),
            PScenarioValueRead(PScenarioKind.Text, _pScenarioKindUnreadable));
    }

    private static LStateValue PScenarioValueRead(string text, bool unreadable)
    {
        if (!string.IsNullOrWhiteSpace(text))
        {
            return LStateValue.LStateValueCreate(text);
        }

        return unreadable ? LStateValue.LStateValueUnknown : LStateValue.LStateValueUnspecified;
    }

    private bool PScenarioChangeCheck()
    {
        LSituation written = PScenarioRead();
        LSituation stored = _pScenarioSituation ?? new LSituation(
            string.Empty,
            LStateValue.LStateValueUnspecified,
            LStateValue.LStateValueUnspecified,
            LStateValue.LStateValueUnspecified);

        return written != stored with { LSituationId = written.LSituationId };
    }

    private void PRepertoireFreshHandle(object sender, RoutedEventArgs e)
    {
        if (!PRepertoireLeaveConfirm())
        {
            return;
        }

        PRepertoireClear();
        _pScenarioSituation = null;
        PScenarioApply(null);
        PRepertoireScribe.IsEnabled = true;
        PRepertoireScribeShow(true);
    }

    private void PRepertoireScribeHandle(object sender, RoutedEventArgs e)
    {
        if (PScenario.Visibility == Visibility.Visible)
        {
            if (!PRepertoireLeaveConfirm())
            {
                return;
            }

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

        PScenarioApply(_pScenarioSituation);
        PRepertoireScribeShow(true);
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

        PScenarioApply(_pScenarioSituation);
    }

    private void PScenarioStoreHandle(object sender, RoutedEventArgs e)
    {
        LSituation written = PScenarioRead();

        try
        {
            if (_pScenarioSituation is null)
            {
                written = _lEngine.LEngineSituationCreate(written);
            }
            else
            {
                _lEngine.LEngineSituationUpdate(written);
            }
        }
        catch (Exception exception)
        {
            _pRepertoireHost.PWindowFailureShow("Situation.SaveFailed", exception);
            return;
        }

        _pScenarioSituation = written;
        _pVignetteSituation = written.LSituationId;

        PAtlasFind(PInquest.Text ?? string.Empty);
        PRepertoireScribeShow(false);
        PRepertoireShow(written.LSituationId);
    }

    private void PScenarioRemovalHandle(object sender, RoutedEventArgs e)
    {
        if (_pScenarioSituation is null)
        {
            return;
        }

        string id = _pScenarioSituation.LSituationId;
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
        _pVignetteSituation = null;
        _pScenarioSituation = null;
        _pOccurrenceList.Clear();

        PVignetteBody.Visibility = Visibility.Collapsed;
        PVignetteUnselected.Visibility = Visibility.Visible;
        PScenarioApply(null);
        PRepertoireScribeShow(false);
        PRepertoireScribe.IsEnabled = false;
    }
}
