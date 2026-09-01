using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using Llyn.Core;

namespace Llyn.UIShell;

/// <summary>
/// Browsing behavior of the list panel: the search box refills the entry index, and a chosen index
/// row is loaded back from the workspace and rendered read-only in the display area. This is the
/// read half of the entry round trip — the input panel writes an entry, this reads it back.
/// </summary>
public partial class PWindow
{
    private readonly ObservableCollection<PIndexItem> _pIndexList = [];

    // Full path of the audio the shown entry owns, or null when it has none — what the display's play
    // button plays, kept apart from the input panel's own transient recording.
    private string? _pDisplayRecording;

    // The panel opens with whatever the database already holds, so a save made in the input panel is
    // visible the moment the tab is switched to. Rebinding is idempotent, which keeps the binding out
    // of the window constructor and beside the code that owns it.
    private void PListHandle(object sender, DependencyPropertyChangedEventArgs e)
    {
        if (!PList.IsVisible)
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

    // Refills the index with the entries matching the typed text; an empty box lists everything. The
    // display is not touched: the search says which entries are offered, never which one is shown.
    private void PIndexFind(string query)
    {
        _pIndexList.Clear();
        foreach (LEntry entry in _lEngine.LEngineEntryFind(query))
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

        LEntryDraft? draft;
        try
        {
            draft = _lEngine.LEngineEntryLoad(item.PIndexItemId);
        }
        catch (Exception exception)
        {
            PWindowFailureShow("List.LoadFailed", exception);
            return;
        }

        if (draft is null)
        {
            // The entry went away between the search and the click; the row is stale, so the list is
            // re-read rather than left offering a row that no longer loads.
            PDisplayClear();
            PIndexFind(PInquiry.Text ?? string.Empty);
            return;
        }

        PDisplayShow(draft);
    }

    private void PDisplayShow(LEntryDraft draft)
    {
        // The audio the entry was saved with, as a full path in the workspace open now. The button
        // appears only when that file is actually there, so a workspace whose audio folder was removed
        // shows no play control rather than one that fails on click.
        _pDisplayRecording = draft.LEntryDraftAudio.Length > 0 && File.Exists(draft.LEntryDraftAudio)
            ? draft.LEntryDraftAudio
            : null;
        PDisplayPlayback.Visibility = _pDisplayRecording is null ? Visibility.Collapsed : Visibility.Visible;

        PDisplayHeadword.Text = draft.LEntryDraftHeadword;
        PDisplayLanguage.Text = draft.LEntryDraftLanguage;
        PDisplayPronunciation.Text = draft.LEntryDraftPronunciation;
        PDisplayPronunciation.Visibility = draft.LEntryDraftPronunciation.Length == 0
            ? Visibility.Collapsed
            : Visibility.Visible;

        // The draft records carry exactly the fields the read view shows, so the templates bind to
        // them directly instead of copying each one into a second row model.
        PDisplaySense.ItemsSource = draft.LEntryDraftSenses;
        PDisplayCollocation.ItemsSource = draft.LEntryDraftCollocations;

        PDisplayNote.Text = draft.LEntryDraftNote;
        PDisplayNote.Visibility = draft.LEntryDraftNote.Length == 0 ? Visibility.Collapsed : Visibility.Visible;

        PDisplayEmpty.Visibility = Visibility.Collapsed;
        PDisplayContents.Visibility = Visibility.Visible;
    }

    private void PDisplayPlaybackHandle(object sender, RoutedEventArgs e)
    {
        if (_pDisplayRecording is null)
        {
            return;
        }

        _pDownloaderPlayer.Open(new Uri(_pDisplayRecording));
        _pDownloaderPlayer.Play();
    }

    // The display must not assume an entry is selected: with none chosen it shows only its prompt.
    private void PDisplayClear()
    {
        _pDisplayRecording = null;
        _pDownloaderPlayer.Stop();
        PDisplayPlayback.Visibility = Visibility.Collapsed;
        PDisplaySense.ItemsSource = null;
        PDisplayCollocation.ItemsSource = null;
        PDisplayContents.Visibility = Visibility.Collapsed;
        PDisplayEmpty.Visibility = Visibility.Visible;
    }
}
