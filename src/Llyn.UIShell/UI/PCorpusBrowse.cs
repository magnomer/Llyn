using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Llyn.Core;

namespace Llyn.UIShell;

public partial class PCorpus
{
    private readonly ObservableCollection<PAnthologyItem> _pAnthologyList = [];

    private readonly ObservableCollection<PUsageItem> _pQuotationList = [];

    private readonly ObservableCollection<PCitationItem> _pCitationCatalog = [];

    private readonly ObservableCollection<PLanguageItem> _pLanguageItem = [];

    private IReadOnlyDictionary<string, int> _pAnthologyCount = new Dictionary<string, int>();

    private string? _pExcerptExample;

    private string _pRankChoice = "Text";

    private async void PCorpusHandle(object sender, DependencyPropertyChangedEventArgs e)
    {
        if (!IsVisible)
        {
            return;
        }

        PAnthology.ItemsSource = _pAnthologyList;
        PQuotation.ItemsSource = _pQuotationList;
        PCitationList.ItemsSource = _pCitationCatalog;
        PLanguageList.ItemsSource = _pLanguageItem;

        await PEnsign.PEnsignLoad(_lEngine);

        PSpeakerLoad();
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
        PRankDropper.IsChecked = false;
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
            _pCorpusHost.PWindowFailureShow("Example.LoadFailed", exception);
            return;
        }

        string unreadable = _pCorpusHost.PLocalizationTextRead("Display.Unreadable");
        string unwritten = _pCorpusHost.PLocalizationTextRead("Example.Unwritten");

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
            PCorpusClear();
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

        if (!PCorpusLeaveConfirm())
        {
            return;
        }

        PCorpusShow(item.PAnthologyItemId);
    }

    private void PCorpusShow(string id)
    {
        LExample? example;
        try
        {
            example = _lEngine.LEngineExampleRead(id);
        }
        catch (Exception exception)
        {
            _pCorpusHost.PWindowFailureShow("Example.LoadFailed", exception);
            return;
        }

        if (example is null)
        {
            PCorpusClear();
            PAnthologyFind(PQuery.Text ?? string.Empty);
            return;
        }

        _pExcerptExample = id;
        _pTranscriptExample = example;

        PExcerptValueShow(PExcerptText, example.LExampleText);
        PExcerptLanguage.Text = example.LExampleLanguage;
        PExcerptFlag.Source = PEnsign.PEnsignFind(example.LExampleLanguage);
        PExcerptValueShow(PExcerptTranslation, example.LExampleTranslation);
        PExcerptValueShow(
            PExcerptCitation,
            example.LExampleSource,
            PCitationNameRead(example.LExampleSource));

        PQuotationFind(id);

        PExcerptBody.Visibility = Visibility.Visible;
        PExcerptUnselected.Visibility = Visibility.Collapsed;
        PCorpusScribe.IsEnabled = true;

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
            LState.LStateUnknown => _pCorpusHost.PLocalizationTextRead("Display.Unreadable"),
            _ => null,
        };

        field.Text = text ?? _pCorpusHost.PLocalizationTextRead("Example.Unset");
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
            _pCorpusHost.PWindowFailureShow("Example.LoadFailed", exception);
            return;
        }

        string unreadable = _pCorpusHost.PLocalizationTextRead("Display.Unreadable");
        string unnamed = _pCorpusHost.PLocalizationTextRead("Example.Unnamed");
        string entry = _pCorpusHost.PLocalizationTextRead("Example.Entry");
        string meaning = _pCorpusHost.PLocalizationTextRead("Example.Meaning");
        string collocation = _pCorpusHost.PLocalizationTextRead("Example.Collocation");

        _pQuotationList.Clear();
        foreach (LUsage usage in read)
        {
            _pQuotationList.Add(new PUsageItem(
                usage,
                usage.LUsageOwner switch
                {
                    LOwner.LOwnerEntry => entry,
                    LOwner.LOwnerCollocation => collocation,
                    _ => meaning,
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

        if (!PCorpusLeaveConfirm())
        {
            return;
        }

        _pCorpusHost.PWindowEntryShow(item.PUsageItemEntry);
    }

    private void PCorpusScribeHandle(object sender, RoutedEventArgs e)
    {
        if (PTranscript.Visibility == Visibility.Visible)
        {
            if (!PCorpusLeaveConfirm())
            {
                return;
            }

            PCorpusScribeShow(false);

            if (_pExcerptExample is not null)
            {
                PCorpusShow(_pExcerptExample);
                return;
            }

            PCorpusClear();
            return;
        }

        if (_pExcerptExample is null)
        {
            return;
        }

        PTranscriptApply(_pTranscriptExample);
        PCorpusScribeShow(true);
    }

    private void PCorpusScribeShow(bool editing)
    {
        PTranscript.Visibility = editing ? Visibility.Visible : Visibility.Collapsed;
        PExcerpt.Visibility = editing ? Visibility.Collapsed : Visibility.Visible;
        PCorpusScribe.SetResourceReference(ButtonBase.ContentProperty, editing ? "Scribe.Read" : "Scribe.Edit");
    }

    private bool PCorpusLeaveConfirm()
    {
        return _pCorpusHost.PWindowDiscardConfirm(PCorpusChangeCheck());
    }

    private void PCorpusClear()
    {
        _pExcerptExample = null;
        _pTranscriptExample = null;
        _pQuotationList.Clear();

        PExcerptBody.Visibility = Visibility.Collapsed;
        PExcerptUnselected.Visibility = Visibility.Visible;
        PTranscriptApply(null);
        PCorpusScribeShow(false);
        PCorpusScribe.IsEnabled = false;
    }
}
