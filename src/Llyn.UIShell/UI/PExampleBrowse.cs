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

    private readonly ObservableCollection<PUsageItem> _pQuotationList = [];

    private readonly ObservableCollection<PCitationItem> _pCitationCatalog = [];

    private readonly ObservableCollection<PLangcodeItem> _pLangcodeItem = [];

    private IReadOnlyDictionary<string, int> _pAnthologyCount = new Dictionary<string, int>();

    private string? _pExcerptExample;

    private string _pRankChoice = "Text";

    private async void PExampleHandle(object sender, DependencyPropertyChangedEventArgs e)
    {
        if (!IsVisible)
        {
            return;
        }

        PAnthology.ItemsSource = _pAnthologyList;
        PQuotation.ItemsSource = _pQuotationList;
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
            "Usage" => examples.OrderByDescending(PAnthologyCountRead),
            _ => examples.OrderBy(
                example => example.LExampleText.LStateValueShow(), StringComparer.CurrentCultureIgnoreCase)
        };
    }

    private int PAnthologyCountRead(LExample example)
    {
        return _pAnthologyCount.TryGetValue(example.LExampleId, out int usage) ? usage : 0;
    }

    private void PAnthologyFind(string query)
    {
        query = query.Trim();

        IReadOnlyList<LExample> read;
        try
        {
            read = _lEngine.LEngineExampleRead();
            _pAnthologyCount = _lEngine.LEngineUsageRead(LOwner.LOwnerExample);
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

            kept |= string.Equals(example.LExampleId, _pExcerptExample, StringComparison.Ordinal);
            _pAnthologyList.Add(new PAnthologyItem(
                example,
                PAnthologyCountRead(example),
                PCitationNameRead(example.LExampleSource),
                unreadable,
                unwritten));
        }

        PAnthologyEmpty.Visibility = _pAnthologyList.Count == 0 ? Visibility.Visible : Visibility.Collapsed;

        if (!kept && _pExcerptExample is not null && PTranscript.Visibility != Visibility.Visible)
        {
            PExampleClear();
        }
    }

    private bool PQueryMatch(LExample example, string query)
    {
        if (PQueryMatch(example.LExampleText.LStateValueShow(), query)
            || PQueryMatch(example.LExampleTranslation.LStateValueShow(), query)
            || PQueryMatch(PCitationNameRead(example.LExampleSource), query))
        {
            return true;
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

        _pExcerptExample = id;
        _pTranscriptExample = example;

        PExcerptValueShow(PExcerptText, example.LExampleText);
        PExcerptLanguage.Text = example.LExampleLanguage;
        PExcerptFlag.Source = PLangcodeIndicator.PLangcodeIndicatorFind(example.LExampleLanguage);
        PExcerptValueShow(PExcerptTranslation, example.LExampleTranslation);
        PExcerptValueShow(
            PExcerptCitation,
            example.LExampleSource,
            PCitationNameRead(example.LExampleSource));

        PQuotationFind(id);

        PExcerptBody.Visibility = Visibility.Visible;
        PExcerptUnselected.Visibility = Visibility.Collapsed;
        PExampleScribe.IsEnabled = true;

        if (PTranscript.Visibility == Visibility.Visible)
        {
            PTranscriptApply(example);
        }
    }

    private void PExcerptValueShow(TextBlock field, LStateValue value, string? shown = null)
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

    private void PQuotationFind(string id)
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

        _pQuotationList.Clear();
        foreach (LUsage usage in read)
        {
            _pQuotationList.Add(new PUsageItem(
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

        PQuotationEmpty.Visibility = _pQuotationList.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
    }

    private void PQuotationHandle(object sender, RoutedEventArgs e)
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

    private void PExampleScribeHandle(object sender, RoutedEventArgs e)
    {
        if (PTranscript.Visibility == Visibility.Visible)
        {
            if (!PExampleLeaveConfirm())
            {
                return;
            }

            PExampleScribeShow(false);

            if (_pExcerptExample is not null)
            {
                PExampleShow(_pExcerptExample);
                return;
            }

            PExampleClear();
            return;
        }

        if (_pExcerptExample is null)
        {
            return;
        }

        PTranscriptApply(_pTranscriptExample);
        PExampleScribeShow(true);
    }

    private void PExampleScribeShow(bool editing)
    {
        PTranscript.Visibility = editing ? Visibility.Visible : Visibility.Collapsed;
        PExcerpt.Visibility = editing ? Visibility.Collapsed : Visibility.Visible;
        PExampleScribe.SetResourceReference(ButtonBase.ContentProperty, editing ? "Scribe.Read" : "Scribe.Edit");
    }

    private bool PExampleLeaveConfirm()
    {
        return _pExampleHost.PWindowDiscardConfirm(PExampleChangeCheck());
    }

    private void PExampleClear()
    {
        _pExcerptExample = null;
        _pTranscriptExample = null;
        _pQuotationList.Clear();

        PExcerptBody.Visibility = Visibility.Collapsed;
        PExcerptUnselected.Visibility = Visibility.Visible;
        PTranscriptApply(null);
        PExampleScribeShow(false);
        PExampleScribe.IsEnabled = false;
    }
}
