using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Llyn.Core;

namespace Llyn.UIShell;

public partial class PExample
{
    private readonly ObservableCollection<PAnthologyItem> _pAnthologyList = [];

    private readonly ObservableCollection<PUsageItem> _pUsageList = [];

    private readonly ObservableCollection<PTranslationItem> _pDisplayTranslation = [];

    private readonly ObservableCollection<PTranslationItem> _pTranslationList = [];

    private readonly ObservableCollection<PReference> _pCitationCatalog = [];

    private readonly ObservableCollection<PLangcodeItem> _pLangcodeItem = [];

    private IReadOnlyDictionary<string, int> _pAnthologyUsage = new Dictionary<string, int>();

    private string? _pDisplayExample;

    private string _pRankChoice = "Text";

    private async void PExampleHandle(object sender, DependencyPropertyChangedEventArgs e)
    {
        if (!IsVisible)
        {
            return;
        }

        PAnthology.ItemsSource = _pAnthologyList;
        PUsage.ItemsSource = _pUsageList;
        PDisplayTranslation.ItemsSource = _pDisplayTranslation;
        PTranslation.ItemsSource = _pTranslationList;
        PCitationList.ItemsSource = _pCitationCatalog;
        PLangcodeList.ItemsSource = _pLangcodeItem;

        await PLangcodeIndicator.PLangcodeIndicatorLoad(_lEngine);

        PLangcodeLoad();
        PCitationFind();
        PAnthologyFind(PQuery.Text ?? string.Empty);
    }

    private void PQueryHandle(object sender, TextChangedEventArgs e)
    {
        PAnthologyFind(PQuery.Text ?? string.Empty);
    }

    private void PRankHandle(object sender, RoutedEventArgs e)
    {
        if (sender is not FrameworkElement { Tag: string choice })
        {
            return;
        }

        _pRankChoice = choice;
        PRankBase.IsChecked = false;
        PAnthologyFind(PQuery.Text ?? string.Empty);
    }

    private IEnumerable<LExample> PAnthologySort(IReadOnlyList<LExample> examples)
    {
        return _pRankChoice switch
        {
            "Language" => examples.OrderBy(
                example => example.LExampleLanguage, StringComparer.CurrentCultureIgnoreCase),
            "Source" => examples.OrderBy(
                example => PCitationNameRead(example.LExampleSource), StringComparer.CurrentCultureIgnoreCase),
            "Usage" => examples.OrderByDescending(PAnthologyUsageRead),
            _ => examples.OrderBy(
                example => example.LExampleText.LStateValueShow(), StringComparer.CurrentCultureIgnoreCase)
        };
    }

    private int PAnthologyUsageRead(LExample example)
    {
        return _pAnthologyUsage.TryGetValue(example.LExampleId, out int usage) ? usage : 0;
    }

    private void PAnthologyFind(string query)
    {
        query = query.Trim();

        IReadOnlyList<LExample> read;
        try
        {
            read = _lEngine.LEngineExampleRead();
            _pAnthologyUsage = _lEngine.LEngineUsageRead(LOwner.LOwnerExample);
        }
        catch (Exception exception)
        {
            _pExampleHost.PWindowFailureShow("Example.LoadFailed", exception);
            return;
        }

        string unreadable = _pExampleHost.PLocalizationTextRead("Display.Unreadable");
        string unwritten = _pExampleHost.PLocalizationTextRead("Example.Unwritten");

        _pAnthologyList.Clear();
        bool kept = false;
        foreach (LExample example in PAnthologySort(read))
        {
            if (query.Length > 0 && !PQueryMatch(example, query))
            {
                continue;
            }

            kept |= string.Equals(example.LExampleId, _pDisplayExample, StringComparison.Ordinal);
            _pAnthologyList.Add(new PAnthologyItem(
                example,
                PAnthologyUsageRead(example),
                PCitationNameRead(example.LExampleSource),
                unreadable,
                unwritten));
        }

        PAnthologyEmpty.Visibility = _pAnthologyList.Count == 0 ? Visibility.Visible : Visibility.Collapsed;

        if (!kept && _pDisplayExample is not null && PEditor.Visibility != Visibility.Visible)
        {
            PExampleClear();
        }
    }

    private bool PQueryMatch(LExample example, string query)
    {
        if (PQueryMatch(example.LExampleText.LStateValueShow(), query)
            || PQueryMatch(example.LExampleLocal ?? string.Empty, query)
            || PQueryMatch(PCitationNameRead(example.LExampleSource), query))
        {
            return true;
        }

        foreach (LTranslation translation in example.LExampleTranslations)
        {
            if (PQueryMatch(translation.LTranslationText, query))
            {
                return true;
            }
        }

        return false;
    }

    private static bool PQueryMatch(string text, string query)
    {
        return text.Length > 0 && text.IndexOf(query, StringComparison.CurrentCultureIgnoreCase) >= 0;
    }

    private void PAnthologyHandle(object sender, RoutedEventArgs e)
    {
        if (sender is not FrameworkElement row || row.DataContext is not PAnthologyItem item)
        {
            return;
        }

        if (!PExampleLeaveConfirm())
        {
            return;
        }

        PExampleShow(item.PAnthologyItemId);
    }

    private void PExampleShow(string id)
    {
        LExample? example;
        try
        {
            example = _lEngine.LEngineExampleRead(id);
        }
        catch (Exception exception)
        {
            _pExampleHost.PWindowFailureShow("Example.LoadFailed", exception);
            return;
        }

        if (example is null)
        {
            PExampleClear();
            PAnthologyFind(PQuery.Text ?? string.Empty);
            return;
        }

        _pDisplayExample = id;
        _pEditorExample = example;

        PDisplayValueShow(PDisplayText, example.LExampleText);
        PDisplayLanguage.Text = example.LExampleLanguage;
        PDisplayFlag.Source = PLangcodeIndicator.PLangcodeIndicatorFind(example.LExampleLanguage);
        PDisplayLocalShow(example.LExampleLocal);
        PDisplayValueShow(
            PDisplayCitation,
            example.LExampleSource,
            PCitationNameRead(example.LExampleSource));

        _pDisplayTranslation.Clear();
        foreach (LTranslation translation in example.LExampleTranslations)
        {
            _pDisplayTranslation.Add(new PTranslationItem(
                translation.LTranslationId,
                translation.LTranslationLanguage,
                translation.LTranslationText));
        }

        PDisplayTranslationEmpty.Visibility =
            _pDisplayTranslation.Count == 0 ? Visibility.Visible : Visibility.Collapsed;

        PUsageFind(id);

        PDisplayBody.Visibility = Visibility.Visible;
        PDisplayUnselected.Visibility = Visibility.Collapsed;
        PScribe.IsEnabled = true;

        if (PEditor.Visibility == Visibility.Visible)
        {
            PEditorApply(example);
        }
    }

    private void PDisplayValueShow(TextBlock field, LStateValue value, string? shown = null)
    {
        string? text = value.LStateValueState switch
        {
            LState.LStateSpecified => shown ?? value.LStateValueShow(),
            LState.LStateUnknown => _pExampleHost.PLocalizationTextRead("Display.Unreadable"),
            _ => null,
        };

        field.Text = text ?? _pExampleHost.PLocalizationTextRead("Example.Unset");
        field.SetResourceReference(
            TextBlock.ForegroundProperty,
            text is null ? "Theme.Muted" : "Theme.Ink");
    }

    private void PDisplayLocalShow(string? local)
    {
        bool written = !string.IsNullOrWhiteSpace(local);
        PDisplayLocal.Text = written ? local : _pExampleHost.PLocalizationTextRead("Example.Unset");
        PDisplayLocal.SetResourceReference(
            TextBlock.ForegroundProperty,
            written ? "Theme.Ink" : "Theme.Muted");
    }

    private void PUsageFind(string id)
    {
        IReadOnlyList<LUsage> read;
        try
        {
            read = _lEngine.LEngineUsageRead(id, LOwner.LOwnerExample);
        }
        catch (Exception exception)
        {
            _pExampleHost.PWindowFailureShow("Example.LoadFailed", exception);
            return;
        }

        string unreadable = _pExampleHost.PLocalizationTextRead("Display.Unreadable");
        string unnamed = _pExampleHost.PLocalizationTextRead("Example.Unnamed");
        string entry = _pExampleHost.PLocalizationTextRead("Example.Entry");
        string sense = _pExampleHost.PLocalizationTextRead("Example.Meaning");
        string collocation = _pExampleHost.PLocalizationTextRead("Example.Collocation");

        _pUsageList.Clear();
        foreach (LUsage usage in read)
        {
            _pUsageList.Add(new PUsageItem(
                usage,
                usage.LUsageOwner switch
                {
                    LOwner.LOwnerEntry => entry,
                    LOwner.LOwnerCollocation => collocation,
                    _ => sense,
                },
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

        if (!PExampleLeaveConfirm())
        {
            return;
        }

        _pExampleHost.PWindowEntryShow(item.PUsageItemEntry);
    }

    private void PScribeHandle(object sender, RoutedEventArgs e)
    {
        if (PEditor.Visibility == Visibility.Visible)
        {
            if (!PExampleLeaveConfirm())
            {
                return;
            }

            PScribeShow(false);

            if (_pDisplayExample is not null)
            {
                PExampleShow(_pDisplayExample);
                return;
            }

            PExampleClear();
            return;
        }

        if (_pDisplayExample is null)
        {
            return;
        }

        PEditorApply(_pEditorExample);
        PScribeShow(true);
    }

    private void PScribeShow(bool editing)
    {
        PEditor.Visibility = editing ? Visibility.Visible : Visibility.Collapsed;
        PDisplay.Visibility = editing ? Visibility.Collapsed : Visibility.Visible;
        PScribe.SetResourceReference(ButtonBase.ContentProperty, editing ? "Scribe.Read" : "Scribe.Edit");
    }

    private bool PExampleLeaveConfirm()
    {
        return _pExampleHost.PWindowDiscardConfirm(PExampleChangeCheck());
    }

    private void PExampleClear()
    {
        _pDisplayExample = null;
        _pEditorExample = null;
        _pUsageList.Clear();
        _pDisplayTranslation.Clear();

        PDisplayBody.Visibility = Visibility.Collapsed;
        PDisplayUnselected.Visibility = Visibility.Visible;
        PEditorApply(null);
        PScribeShow(false);
        PScribe.IsEnabled = false;
    }
}
