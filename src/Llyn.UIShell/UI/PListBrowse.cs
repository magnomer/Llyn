using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using Llyn.Core;

namespace Llyn.UIShell;

public partial class PList
{
    private readonly ObservableCollection<PIndexItem> _pIndexList = [];

    private readonly MediaPlayer _pDisplayPlayer = new();

    private string? _pDisplayRecording;

    private string? _pDisplayEntry;

    private string _pOrderChoice = "Headword";

    private void PListHandle(object sender, DependencyPropertyChangedEventArgs e)
    {
        if (!IsVisible)
        {
            return;
        }

        PIndex.ItemsSource = _pIndexList;
        PIndexFind(PInquiry.Text ?? string.Empty);
    }

    private void PInquiryHandle(object sender, TextChangedEventArgs e)
    {
        PIndexFind(PInquiry.Text ?? string.Empty);
    }

    private void POrderHandle(object sender, RoutedEventArgs e)
    {
        if (sender is not FrameworkElement { Tag: string choice })
        {
            return;
        }

        _pOrderChoice = choice;
        POrderBase.IsChecked = false;
        PIndexFind(PInquiry.Text ?? string.Empty);
    }

    private IEnumerable<LEntry> POrderSort(IReadOnlyList<LEntry> entries)
    {
        return _pOrderChoice switch
        {
            "Reverse" => entries.OrderByDescending(
                entry => entry.LEntryHeadword, StringComparer.CurrentCultureIgnoreCase),
            "Recent" => entries.OrderByDescending(
                entry => entry.LEntryAddedUtc ?? string.Empty, StringComparer.Ordinal),
            "Earliest" => entries.OrderBy(
                entry => entry.LEntryAddedUtc ?? string.Empty, StringComparer.Ordinal),
            _ => entries
        };
    }

    private void PIndexFind(string query)
    {
        _pIndexList.Clear();
        foreach (LEntry entry in POrderSort(_lEngine.LEngineEntryFind(query)))
        {
            _pIndexList.Add(new PIndexItem(entry.LEntryId, entry.LEntryHeadword, entry.LEntryLanguage));
        }

        PIndexEmpty.Visibility = _pIndexList.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
    }

    private void PIndexHandle(object sender, RoutedEventArgs e)
    {
        if (sender is not FrameworkElement row || row.DataContext is not PIndexItem item)
        {
            return;
        }

        if (!PListLeaveConfirm())
        {
            return;
        }

        PIndexEntryShow(item.PIndexItemId);
    }

    private void PIndexEntryShow(string id)
    {
        LEntryDraft? draft;
        try
        {
            draft = _lEngine.LEngineEntryLoad(id);
        }
        catch (Exception exception)
        {
            _pListHost.PWindowFailureShow("List.LoadFailed", exception);
            return;
        }

        if (draft is null)
        {
            PDisplayClear();
            PIndexFind(PInquiry.Text ?? string.Empty);
            return;
        }

        _pDisplayEntry = id;
        PDisplayShow(draft);

        if (PEditor.Visibility == Visibility.Visible)
        {
            PEditor.PEditorEntryShow(id);
        }
    }

    private void PIndexEntryUpdate(string id)
    {
        _pDisplayEntry = id;
        PIndexFind(PInquiry.Text ?? string.Empty);

        LEntryDraft? draft;
        try
        {
            draft = _lEngine.LEngineEntryLoad(id);
        }
        catch (Exception)
        {
            return;
        }

        if (draft is not null)
        {
            PDisplayShow(draft);
        }
    }

    private void PEditorEntryRestore()
    {
        if (_pDisplayEntry is null)
        {
            PEditor.PEditorReset();
            return;
        }

        PEditor.PEditorEntryShow(_pDisplayEntry);
    }

    private void PFreshHandle(object sender, RoutedEventArgs e)
    {
        if (!PListLeaveConfirm())
        {
            return;
        }

        PDisplayClear();
        PScribe.IsEnabled = true;
        PScribeShow(true);
    }

    private void PScribeHandle(object sender, RoutedEventArgs e)
    {
        if (PEditor.Visibility == Visibility.Visible)
        {
            if (!PListLeaveConfirm())
            {
                return;
            }

            PScribeShow(false);

            if (_pDisplayEntry is not null)
            {
                PIndexEntryShow(_pDisplayEntry);
            }

            return;
        }

        if (_pDisplayEntry is null)
        {
            return;
        }

        PEditor.PEditorEntryShow(_pDisplayEntry);
        PScribeShow(true);
    }

    private void PScribeShow(bool editing)
    {
        PEditor.Visibility = editing ? Visibility.Visible : Visibility.Collapsed;
        PDisplay.Visibility = editing ? Visibility.Collapsed : Visibility.Visible;
        PScribe.SetResourceReference(ButtonBase.ContentProperty, editing ? "Scribe.Read" : "Scribe.Edit");
    }

    private bool PListLeaveConfirm()
    {
        return _pListHost.PWindowDiscardConfirm(PListChangeCheck());
    }

    private void PDisplayShow(LEntryDraft draft)
    {
        _pDisplayRecording = draft.LEntryDraftAudio.Length > 0 && File.Exists(draft.LEntryDraftAudio)
            ? draft.LEntryDraftAudio
            : null;
        PDisplayPlayback.Visibility = _pDisplayRecording is null ? Visibility.Collapsed : Visibility.Visible;

        PDisplayHeadword.Text = draft.LEntryDraftHeadword;
        PDisplayLanguageShow(draft.LEntryDraftLanguage);
        PDisplayPronunciation.Text = draft.LEntryDraftPronunciation;
        PDisplayPronunciationSurface.Visibility = draft.LEntryDraftPronunciation.Length == 0
            ? Visibility.Collapsed
            : Visibility.Visible;

        PDisplaySpeech.ItemsSource = draft.LEntryDraftSpeeches ?? [];
        PDisplaySense.ItemsSource = draft.LEntryDraftSenses;
        PDisplayCollocation.ItemsSource = draft.LEntryDraftCollocations;
        PDisplaySenseSection.Visibility = draft.LEntryDraftSenses.Count == 0
            ? Visibility.Collapsed
            : Visibility.Visible;
        PDisplayCollocationSection.Visibility = draft.LEntryDraftCollocations.Count == 0
            ? Visibility.Collapsed
            : Visibility.Visible;

        PDisplayNote.Text = draft.LEntryDraftNote;
        PDisplayNoteSection.Visibility = draft.LEntryDraftNote.Length == 0
            ? Visibility.Collapsed
            : Visibility.Visible;

        PDisplayEmpty.Visibility = Visibility.Collapsed;
        PDisplayContents.Visibility = Visibility.Visible;

        PScribe.IsEnabled = true;
    }

    private async void PDisplayLanguageShow(string language)
    {
        PDisplayLanguage.Text = language;
        PDisplayLanguageFlag.Source = null;

        string? path;
        try
        {
            path = await _lEngine.LEngineFlagRead(language, CancellationToken.None);
        }
        catch (Exception)
        {
            return;
        }

        if (!string.Equals(PDisplayLanguage.Text, language, StringComparison.Ordinal))
        {
            return;
        }

        PDisplayLanguageFlag.Source = path is not null && File.Exists(path)
            ? PLangcodeIndicator.PLangcodeIndicatorResolve(path)
            : null;
    }

    private void PDisplayPlaybackHandle(object sender, RoutedEventArgs e)
    {
        if (_pDisplayRecording is null)
        {
            return;
        }

        _pDisplayPlayer.Open(new Uri(_pDisplayRecording));
        _pDisplayPlayer.Play();
    }

    private void PDisplayClear()
    {
        _pDisplayEntry = null;
        _pDisplayRecording = null;
        _pDisplayPlayer.Stop();
        PDisplayLanguage.Text = string.Empty;
        PDisplayLanguageFlag.Source = null;
        PDisplayPlayback.Visibility = Visibility.Collapsed;
        PDisplayPronunciationSurface.Visibility = Visibility.Collapsed;
        PDisplaySpeech.ItemsSource = null;
        PDisplaySense.ItemsSource = null;
        PDisplayCollocation.ItemsSource = null;
        PDisplaySenseSection.Visibility = Visibility.Collapsed;
        PDisplayCollocationSection.Visibility = Visibility.Collapsed;
        PDisplayNoteSection.Visibility = Visibility.Collapsed;
        PDisplayContents.Visibility = Visibility.Collapsed;
        PDisplayEmpty.Visibility = Visibility.Visible;

        PEditor.PEditorReset();
        PScribeShow(false);
        PScribe.IsEnabled = false;
    }
}
