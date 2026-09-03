using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Llyn.Core;

namespace Llyn.UIShell;

public partial class PSituation
{
    private readonly ObservableCollection<PAtlasItem> _pAtlasList = [];

    private readonly ObservableCollection<PUsageItem> _pOccurrenceList = [];

    private readonly ObservableCollection<PCitationItem> _pCitationCatalog = [];

    private IReadOnlyDictionary<string, int> _pAtlasCount = new Dictionary<string, int>();

    private string? _pVignetteSituation;

    private string _pTierChoice = "Title";

    private LSituation? _pScenarioSituation;

    private string _pScenarioCitation = string.Empty;

    private bool _pScenarioTitleUnreadable;

    private bool _pScenarioKindUnreadable;

    private bool _pScenarioDescriptionUnreadable;

    private bool _pScenarioLoading;

    private async void PSituationHandle(object sender, DependencyPropertyChangedEventArgs e)
    {
        if (!IsVisible)
        {
            return;
        }

        PAtlas.ItemsSource = _pAtlasList;
        POccurrence.ItemsSource = _pOccurrenceList;
        PCitationList.ItemsSource = _pCitationCatalog;

        await PEnsign.PEnsignLoad(_lEngine);

        PCitationFind();
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
        PTierBase.IsChecked = false;
        PAtlasFind(PInquest.Text ?? string.Empty);
    }

    private IEnumerable<LSituation> PAtlasSort(IReadOnlyList<LSituation> situations)
    {
        return _pTierChoice switch
        {
            "Kind" => situations.OrderBy(
                situation => situation.LSituationKind.LStateValueShow(), StringComparer.CurrentCultureIgnoreCase),
            "Source" => situations.OrderBy(
                situation => PCitationNameRead(situation.LSituationSource), StringComparer.CurrentCultureIgnoreCase),
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
            _pSituationHost.PWindowFailureShow("Situation.LoadFailed", exception);
            return;
        }

        string unreadable = _pSituationHost.PLocalizationTextRead("Display.Unreadable");
        string untitled = _pSituationHost.PLocalizationTextRead("Situation.Untitled");

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
            PSituationClear();
        }
    }

    private bool PInquestMatch(LSituation situation, string query)
    {
        return PInquestMatch(situation.LSituationTitle.LStateValueShow(), query)
            || PInquestMatch(situation.LSituationDescription.LStateValueShow(), query)
            || PInquestMatch(situation.LSituationKind.LStateValueShow(), query)
            || PInquestMatch(PCitationNameRead(situation.LSituationSource), query);
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

        if (!PSituationLeaveConfirm())
        {
            return;
        }

        PSituationShow(item.PAtlasItemId);
    }

    private void PSituationShow(string id)
    {
        LSituation? situation;
        try
        {
            situation = _lEngine.LEngineSituationRead(id);
        }
        catch (Exception exception)
        {
            _pSituationHost.PWindowFailureShow("Situation.LoadFailed", exception);
            return;
        }

        if (situation is null)
        {
            PSituationClear();
            PAtlasFind(PInquest.Text ?? string.Empty);
            return;
        }

        _pVignetteSituation = id;
        _pScenarioSituation = situation;

        PVignetteValueShow(PVignetteTitle, situation.LSituationTitle);
        PVignetteValueShow(PVignetteKind, situation.LSituationKind);
        PVignetteValueShow(PVignetteDescription, situation.LSituationDescription);
        PVignetteValueShow(
            PVignetteCitation,
            situation.LSituationSource,
            PCitationNameRead(situation.LSituationSource));

        POccurrenceFind(id);

        PVignetteBody.Visibility = Visibility.Visible;
        PVignetteUnselected.Visibility = Visibility.Collapsed;
        PSituationScribe.IsEnabled = true;

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
            LState.LStateUnknown => _pSituationHost.PLocalizationTextRead("Display.Unreadable"),
            _ => null,
        };

        field.Text = text ?? _pSituationHost.PLocalizationTextRead("Situation.Unset");
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
            _pSituationHost.PWindowFailureShow("Situation.LoadFailed", exception);
            return;
        }

        string unreadable = _pSituationHost.PLocalizationTextRead("Display.Unreadable");
        string unnamed = _pSituationHost.PLocalizationTextRead("Situation.Unnamed");
        string sense = _pSituationHost.PLocalizationTextRead("Situation.Meaning");
        string collocation = _pSituationHost.PLocalizationTextRead("Situation.Collocation");

        _pOccurrenceList.Clear();
        foreach (LUsage usage in read)
        {
            _pOccurrenceList.Add(new PUsageItem(
                usage,
                usage.LUsageOwner == LOwner.LOwnerCollocation ? collocation : sense,
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

        if (!PSituationLeaveConfirm())
        {
            return;
        }

        _pSituationHost.PWindowEntryShow(item.PUsageItemEntry);
    }

    private void PCitationFind()
    {
        IReadOnlyList<LReference> read;
        try
        {
            read = _lEngine.LEngineReferenceRead();
        }
        catch (Exception exception)
        {
            _pSituationHost.PWindowFailureShow("Situation.LoadFailed", exception);
            return;
        }

        _pCitationCatalog.Clear();
        foreach (LReference reference in read)
        {
            _pCitationCatalog.Add(PCitationItem.PCitationItemCreate(reference));
        }

        PCitationEmpty.Visibility = _pCitationCatalog.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
    }

    private string PCitationNameRead(LStateValue source)
    {
        string id = source.LStateValueShow();
        if (id.Length == 0)
        {
            return string.Empty;
        }

        foreach (PCitationItem row in _pCitationCatalog)
        {
            if (string.Equals(row.PCitationItemId, id, StringComparison.Ordinal))
            {
                return row.PCitationItemName;
            }
        }

        return id;
    }

    private void PCitationHandle(object sender, SelectionChangedEventArgs e)
    {
        if (_pScenarioLoading || PCitationList.SelectedValue is not string id)
        {
            return;
        }

        _pScenarioCitation = id;
        PCitation.IsChecked = false;
        PCitationUpdate();
    }

    private void PCitationClearHandle(object sender, RoutedEventArgs e)
    {
        _pScenarioCitation = string.Empty;
        PCitationList.SelectedValue = null;
        PCitation.IsChecked = false;
        PCitationUpdate();
    }

    private void PCitationUpdate()
    {
        PCitationName.Text = _pScenarioCitation.Length == 0
            ? _pSituationHost.PLocalizationTextRead("Reference.Assign")
            : PCitationNameRead(LStateValue.LStateValueCreate(_pScenarioCitation));
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

        string unreadable = _pSituationHost.PLocalizationTextRead("Display.Unreadable");

        PScenarioTitle.Text = situation?.LSituationTitle.LStateValueShow() ?? string.Empty;
        PScenarioKind.Text = situation?.LSituationKind.LStateValueShow() ?? string.Empty;
        PScenarioDescription.Text = situation?.LSituationDescription.LStateValueShow() ?? string.Empty;

        _pScenarioTitleUnreadable = situation?.LSituationTitle.LStateValueState == LState.LStateUnknown;
        _pScenarioKindUnreadable = situation?.LSituationKind.LStateValueState == LState.LStateUnknown;
        _pScenarioDescriptionUnreadable = situation?.LSituationDescription.LStateValueState == LState.LStateUnknown;

        PScenarioTitle.Tag = _pScenarioTitleUnreadable ? unreadable : string.Empty;
        PScenarioKind.Tag = _pScenarioKindUnreadable ? unreadable : string.Empty;
        PScenarioDescription.Tag = _pScenarioDescriptionUnreadable ? unreadable : string.Empty;

        _pScenarioCitation = situation?.LSituationSource.LStateValueShow() ?? string.Empty;
        PCitationList.SelectedValue = _pScenarioCitation.Length == 0 ? null : _pScenarioCitation;
        PCitationUpdate();

        PScenarioRemoval.IsEnabled = situation is not null;

        _pScenarioLoading = false;
    }

    private LSituation PScenarioRead()
    {
        return new LSituation(
            _pScenarioSituation?.LSituationId ?? string.Empty,
            PScenarioValueRead(PScenarioTitle.Text, _pScenarioTitleUnreadable),
            PScenarioValueRead(PScenarioDescription.Text, _pScenarioDescriptionUnreadable),
            PScenarioValueRead(PScenarioKind.Text, _pScenarioKindUnreadable),
            PScenarioValueRead(_pScenarioCitation, false));
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
            LStateValue.LStateValueUnspecified,
            LStateValue.LStateValueUnspecified);

        return written != stored with { LSituationId = written.LSituationId };
    }

    private void PSituationFreshHandle(object sender, RoutedEventArgs e)
    {
        if (!PSituationLeaveConfirm())
        {
            return;
        }

        PSituationClear();
        _pScenarioSituation = null;
        PScenarioApply(null);
        PSituationScribe.IsEnabled = true;
        PSituationScribeShow(true);
    }

    private void PSituationScribeHandle(object sender, RoutedEventArgs e)
    {
        if (PScenario.Visibility == Visibility.Visible)
        {
            if (!PSituationLeaveConfirm())
            {
                return;
            }

            PSituationScribeShow(false);

            if (_pVignetteSituation is not null)
            {
                PSituationShow(_pVignetteSituation);
                return;
            }

            PSituationClear();
            return;
        }

        if (_pVignetteSituation is null)
        {
            return;
        }

        PScenarioApply(_pScenarioSituation);
        PSituationScribeShow(true);
    }

    private void PSituationScribeShow(bool editing)
    {
        PScenario.Visibility = editing ? Visibility.Visible : Visibility.Collapsed;
        PVignette.Visibility = editing ? Visibility.Collapsed : Visibility.Visible;
        PSituationScribe.SetResourceReference(ButtonBase.ContentProperty, editing ? "Scribe.Read" : "Scribe.Edit");
    }

    private void PScenarioDiscardHandle(object sender, RoutedEventArgs e)
    {
        if (!PSituationLeaveConfirm())
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
            _pSituationHost.PWindowFailureShow("Situation.SaveFailed", exception);
            return;
        }

        _pScenarioSituation = written;
        _pVignetteSituation = written.LSituationId;

        PAtlasFind(PInquest.Text ?? string.Empty);
        PSituationScribeShow(false);
        PSituationShow(written.LSituationId);
    }

    private void PScenarioRemovalHandle(object sender, RoutedEventArgs e)
    {
        if (_pScenarioSituation is null)
        {
            return;
        }

        string id = _pScenarioSituation.LSituationId;
        int usage = _pAtlasCount.TryGetValue(id, out int count) ? count : 0;

        if (!_pSituationHost.PWindowRemovalConfirm(usage))
        {
            return;
        }

        try
        {
            _lEngine.LEngineSituationDelete(id, usage > 0);
        }
        catch (Exception exception)
        {
            _pSituationHost.PWindowFailureShow("Situation.DeleteFailed", exception);
            return;
        }

        PSituationScribeShow(false);
        PSituationClear();
        PAtlasFind(PInquest.Text ?? string.Empty);
    }

    private bool PSituationLeaveConfirm()
    {
        return _pSituationHost.PWindowDiscardConfirm(PSituationChangeCheck());
    }

    private void PSituationClear()
    {
        _pVignetteSituation = null;
        _pScenarioSituation = null;
        _pOccurrenceList.Clear();

        PVignetteBody.Visibility = Visibility.Collapsed;
        PVignetteUnselected.Visibility = Visibility.Visible;
        PScenarioApply(null);
        PSituationScribeShow(false);
        PSituationScribe.IsEnabled = false;
    }
}
