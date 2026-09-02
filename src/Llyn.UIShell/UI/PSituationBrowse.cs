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

    private readonly ObservableCollection<PUsageItem> _pUsageList = [];

    private readonly ObservableCollection<PReference> _pCitationCatalog = [];

    private IReadOnlyDictionary<string, int> _pAtlasUsage = new Dictionary<string, int>();

    private string? _pDisplaySituation;

    private string _pTierChoice = "Title";

    private LSituation? _pEditorSituation;

    private string _pEditorCitation = string.Empty;

    private bool _pEditorTitleUnreadable;

    private bool _pEditorKindUnreadable;

    private bool _pEditorDescriptionUnreadable;

    private bool _pEditorLoading;

    private async void PSituationHandle(object sender, DependencyPropertyChangedEventArgs e)
    {
        if (!IsVisible)
        {
            return;
        }

        PAtlas.ItemsSource = _pAtlasList;
        PUsage.ItemsSource = _pUsageList;
        PCitationList.ItemsSource = _pCitationCatalog;

        await PLangcodeIndicator.PLangcodeIndicatorLoad(_lEngine);

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
            "Usage" => situations.OrderByDescending(PAtlasUsageRead),
            _ => situations.OrderBy(
                situation => situation.LSituationTitle.LStateValueShow(), StringComparer.CurrentCultureIgnoreCase)
        };
    }

    private int PAtlasUsageRead(LSituation situation)
    {
        return _pAtlasUsage.TryGetValue(situation.LSituationId, out int usage) ? usage : 0;
    }

    private void PAtlasFind(string query)
    {
        query = query.Trim();

        IReadOnlyList<LSituation> read;
        try
        {
            read = _lEngine.LEngineSituationRead();
            _pAtlasUsage = _lEngine.LEngineUsageRead();
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

            kept |= string.Equals(situation.LSituationId, _pDisplaySituation, StringComparison.Ordinal);
            _pAtlasList.Add(new PAtlasItem(situation, PAtlasUsageRead(situation), unreadable, untitled));
        }

        PAtlasEmpty.Visibility = _pAtlasList.Count == 0 ? Visibility.Visible : Visibility.Collapsed;

        if (!kept && _pDisplaySituation is not null && PEditor.Visibility != Visibility.Visible)
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

        _pDisplaySituation = id;
        _pEditorSituation = situation;

        PDisplayValueShow(PDisplayTitle, situation.LSituationTitle);
        PDisplayValueShow(PDisplayKind, situation.LSituationKind);
        PDisplayValueShow(PDisplayDescription, situation.LSituationDescription);
        PDisplayValueShow(
            PDisplayCitation,
            situation.LSituationSource,
            PCitationNameRead(situation.LSituationSource));

        PUsageFind(id);

        PDisplayBody.Visibility = Visibility.Visible;
        PDisplayUnselected.Visibility = Visibility.Collapsed;
        PScribe.IsEnabled = true;

        if (PEditor.Visibility == Visibility.Visible)
        {
            PEditorApply(situation);
        }
    }

    private void PDisplayValueShow(TextBlock field, LStateValue value, string? shown = null)
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

    private void PUsageFind(string id)
    {
        IReadOnlyList<LUsage> read;
        try
        {
            read = _lEngine.LEngineUsageRead(id);
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

        _pUsageList.Clear();
        foreach (LUsage usage in read)
        {
            _pUsageList.Add(new PUsageItem(
                usage,
                usage.LUsageOwner == LOwner.LOwnerCollocation ? collocation : sense,
                unreadable,
                unnamed));
        }

        PUsageEmpty.Visibility = _pUsageList.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
    }

    private void PUsageHandle(object sender, RoutedEventArgs e)
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
            _pCitationCatalog.Add(PReference.PReferenceCreate(reference));
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

        foreach (PReference row in _pCitationCatalog)
        {
            if (string.Equals(row.PReferenceId, id, StringComparison.Ordinal))
            {
                return row.PReferenceName;
            }
        }

        return id;
    }

    private void PCitationHandle(object sender, SelectionChangedEventArgs e)
    {
        if (_pEditorLoading || PCitationList.SelectedValue is not string id)
        {
            return;
        }

        _pEditorCitation = id;
        PCitation.IsChecked = false;
        PCitationUpdate();
    }

    private void PCitationClearHandle(object sender, RoutedEventArgs e)
    {
        _pEditorCitation = string.Empty;
        PCitationList.SelectedValue = null;
        PCitation.IsChecked = false;
        PCitationUpdate();
    }

    private void PCitationUpdate()
    {
        PCitationName.Text = _pEditorCitation.Length == 0
            ? _pSituationHost.PLocalizationTextRead("Reference.Assign")
            : PCitationNameRead(LStateValue.LStateValueCreate(_pEditorCitation));
    }

    private void PEditorTitleHandle(object sender, TextChangedEventArgs e)
    {
        if (_pEditorLoading)
        {
            return;
        }

        _pEditorTitleUnreadable = false;
        PEditorTitle.Tag = string.Empty;
    }

    private void PEditorKindHandle(object sender, TextChangedEventArgs e)
    {
        if (_pEditorLoading)
        {
            return;
        }

        _pEditorKindUnreadable = false;
        PEditorKind.Tag = string.Empty;
    }

    private void PEditorDescriptionHandle(object sender, TextChangedEventArgs e)
    {
        if (_pEditorLoading)
        {
            return;
        }

        _pEditorDescriptionUnreadable = false;
        PEditorDescription.Tag = string.Empty;
    }

    private void PEditorApply(LSituation? situation)
    {
        _pEditorLoading = true;

        string unreadable = _pSituationHost.PLocalizationTextRead("Display.Unreadable");

        PEditorTitle.Text = situation?.LSituationTitle.LStateValueShow() ?? string.Empty;
        PEditorKind.Text = situation?.LSituationKind.LStateValueShow() ?? string.Empty;
        PEditorDescription.Text = situation?.LSituationDescription.LStateValueShow() ?? string.Empty;

        _pEditorTitleUnreadable = situation?.LSituationTitle.LStateValueState == LState.LStateUnknown;
        _pEditorKindUnreadable = situation?.LSituationKind.LStateValueState == LState.LStateUnknown;
        _pEditorDescriptionUnreadable = situation?.LSituationDescription.LStateValueState == LState.LStateUnknown;

        PEditorTitle.Tag = _pEditorTitleUnreadable ? unreadable : string.Empty;
        PEditorKind.Tag = _pEditorKindUnreadable ? unreadable : string.Empty;
        PEditorDescription.Tag = _pEditorDescriptionUnreadable ? unreadable : string.Empty;

        _pEditorCitation = situation?.LSituationSource.LStateValueShow() ?? string.Empty;
        PCitationList.SelectedValue = _pEditorCitation.Length == 0 ? null : _pEditorCitation;
        PCitationUpdate();

        PRemoval.IsEnabled = situation is not null;

        _pEditorLoading = false;
    }

    private LSituation PEditorRead()
    {
        return new LSituation(
            _pEditorSituation?.LSituationId ?? string.Empty,
            PEditorValueRead(PEditorTitle.Text, _pEditorTitleUnreadable),
            PEditorValueRead(PEditorDescription.Text, _pEditorDescriptionUnreadable),
            PEditorValueRead(PEditorKind.Text, _pEditorKindUnreadable),
            PEditorValueRead(_pEditorCitation, false));
    }

    private static LStateValue PEditorValueRead(string text, bool unreadable)
    {
        if (!string.IsNullOrWhiteSpace(text))
        {
            return LStateValue.LStateValueCreate(text);
        }

        return unreadable ? LStateValue.LStateValueUnknown : LStateValue.LStateValueUnspecified;
    }

    private bool PEditorChangeCheck()
    {
        LSituation written = PEditorRead();
        LSituation stored = _pEditorSituation ?? new LSituation(
            string.Empty,
            LStateValue.LStateValueUnspecified,
            LStateValue.LStateValueUnspecified,
            LStateValue.LStateValueUnspecified,
            LStateValue.LStateValueUnspecified);

        return written != stored with { LSituationId = written.LSituationId };
    }

    private void PFreshHandle(object sender, RoutedEventArgs e)
    {
        if (!PSituationLeaveConfirm())
        {
            return;
        }

        PSituationClear();
        _pEditorSituation = null;
        PEditorApply(null);
        PScribe.IsEnabled = true;
        PScribeShow(true);
    }

    private void PScribeHandle(object sender, RoutedEventArgs e)
    {
        if (PEditor.Visibility == Visibility.Visible)
        {
            if (!PSituationLeaveConfirm())
            {
                return;
            }

            PScribeShow(false);

            if (_pDisplaySituation is not null)
            {
                PSituationShow(_pDisplaySituation);
                return;
            }

            PSituationClear();
            return;
        }

        if (_pDisplaySituation is null)
        {
            return;
        }

        PEditorApply(_pEditorSituation);
        PScribeShow(true);
    }

    private void PScribeShow(bool editing)
    {
        PEditor.Visibility = editing ? Visibility.Visible : Visibility.Collapsed;
        PDisplay.Visibility = editing ? Visibility.Collapsed : Visibility.Visible;
        PScribe.SetResourceReference(ButtonBase.ContentProperty, editing ? "Scribe.Read" : "Scribe.Edit");
    }

    private void PDiscardHandle(object sender, RoutedEventArgs e)
    {
        if (!PSituationLeaveConfirm())
        {
            return;
        }

        PEditorApply(_pEditorSituation);
    }

    private void PStoreHandle(object sender, RoutedEventArgs e)
    {
        LSituation written = PEditorRead();

        try
        {
            if (_pEditorSituation is null)
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

        _pEditorSituation = written;
        _pDisplaySituation = written.LSituationId;

        PAtlasFind(PInquest.Text ?? string.Empty);
        PScribeShow(false);
        PSituationShow(written.LSituationId);
    }

    private void PRemovalHandle(object sender, RoutedEventArgs e)
    {
        if (_pEditorSituation is null)
        {
            return;
        }

        string id = _pEditorSituation.LSituationId;
        int usage = _pAtlasUsage.TryGetValue(id, out int count) ? count : 0;

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

        PScribeShow(false);
        PSituationClear();
        PAtlasFind(PInquest.Text ?? string.Empty);
    }

    private bool PSituationLeaveConfirm()
    {
        return _pSituationHost.PWindowDiscardConfirm(PSituationChangeCheck());
    }

    private void PSituationClear()
    {
        _pDisplaySituation = null;
        _pEditorSituation = null;
        _pUsageList.Clear();

        PDisplayBody.Visibility = Visibility.Collapsed;
        PDisplayUnselected.Visibility = Visibility.Visible;
        PEditorApply(null);
        PScribeShow(false);
        PScribe.IsEnabled = false;
    }
}
