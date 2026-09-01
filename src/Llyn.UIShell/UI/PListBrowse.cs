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

/// <summary>
/// Browsing behavior of the list panel: the search box and the ordering menu refill the entry index,
/// and a chosen index row is loaded back from the workspace and rendered read-only in the display
/// area, which the mode toggle swaps for the editor. This is the read half of the entry round trip —
/// the input panel writes an entry, this reads it back and corrects it.
/// </summary>
public partial class PList
{
    private readonly ObservableCollection<PIndexItem> _pIndexList = [];

    // This panel's own playback. The input panel has one too, and sharing a single player made the
    // two panels each other's business: clearing the input recording — which typing a headword does —
    // stopped whatever the List tab was playing.
    private readonly MediaPlayer _pDisplayPlayer = new();

    // Full path of the audio the shown entry owns, or null when it has none — what the display's play
    // button plays, kept apart from the input panel's own transient recording.
    private string? _pDisplayRecording;

    // The entry the right-hand side stands on, or null when none is selected. The display may show it
    // and the editor may be correcting it; either way it is the one entry this panel is on.
    private string? _pDisplayEntry;

    // Which ordering the index is listed in. It is the tag the chosen menu row carries, not the words
    // that row showed: the ordering is the same one whatever language names it.
    private string _pOrderChoice = "Headword";

    // The panel opens with whatever the database already holds, so a save made in the input panel is
    // visible the moment the tab is switched to. Rebinding is idempotent, which keeps the binding out
    // of the window constructor and beside the code that owns it.
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

    // A chosen ordering renames the button and re-lists the index. Nothing about the selected entry
    // changes: an ordering says in which order the entries are offered, never which one is shown.
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

    // The found entries in the order the menu asked for. The engine returns them by headword already,
    // so that ordering is the one that costs nothing; a timestamp an entry does not carry sorts as
    // empty, which puts an entry of unknown age at the end of a newest-first list rather than in the
    // middle of it.
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

    // Refills the index with the entries matching the typed text; an empty box lists everything. The
    // display is not touched: the search says which entries are offered, never which one is shown.
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

        // Selecting another entry leaves whatever is being written behind, so it is asked about first:
        // the row that was clicked is not worth the correction that was typed.
        if (!PListLeaveConfirm())
        {
            return;
        }

        PIndexEntryShow(item.PIndexItemId);
    }

    // Puts the whole right-hand side on one entry: the display is filled from the store, and an editor
    // that is open moves onto the same entry, since the entry being edited is the selected one.
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
            // The entry went away between the search and the click; the row is stale, so the list is
            // re-read rather than left offering a row that no longer loads.
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

    // After a store: the headword the index lists and the text the display shows may both have changed,
    // so each is read again from what was written. The editor keeps the entry it is on — the user
    // corrected it, they did not leave it.
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
            // The write went through; a display that cannot be filled again is not worth reporting as
            // a failure that did not happen.
            return;
        }

        if (draft is not null)
        {
            PDisplayShow(draft);
        }
    }

    // The discard the editor hands over: the selected entry comes back as it is stored. With no entry
    // selected there is nothing stored to come back to, so the form is emptied instead.
    private void PEditorEntryRestore()
    {
        if (_pDisplayEntry is null)
        {
            PEditor.PEditorReset();
            return;
        }

        PEditor.PEditorEntryShow(_pDisplayEntry);
    }

    // The mode toggle: reading becomes writing on the selected entry, and writing goes back to reading
    // what is actually stored. Leaving the editor is what the unsaved question stands in front of.
    private void PScribeHandle(object sender, RoutedEventArgs e)
    {
        if (PEditor.Visibility == Visibility.Visible)
        {
            if (!PListLeaveConfirm())
            {
                return;
            }

            PScribeShow(false);

            // A correction that was given up leaves the editor holding text the store never took, so
            // the display is filled from the store again rather than from what was on screen.
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

    // Which of the two halves the broader side shows, and what the toggle then offers.
    private void PScribeShow(bool editing)
    {
        PEditor.Visibility = editing ? Visibility.Visible : Visibility.Collapsed;
        PDisplay.Visibility = editing ? Visibility.Collapsed : Visibility.Visible;
        PScribe.SetResourceReference(ButtonBase.ContentProperty, editing ? "Scribe.Read" : "Scribe.Edit");
    }

    // The question put before the editing state is left: selecting another entry, toggling back to the
    // display, or anything else that would leave typed corrections behind. Nothing unsaved means
    // nothing to ask about.
    private bool PListLeaveConfirm()
    {
        return _pListHost.PWindowDiscardConfirm(PListChangeCheck());
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
        PDisplayLanguageShow(draft.LEntryDraftLanguage);
        PDisplayPronunciation.Text = draft.LEntryDraftPronunciation;
        PDisplayPronunciationSurface.Visibility = draft.LEntryDraftPronunciation.Length == 0
            ? Visibility.Collapsed
            : Visibility.Visible;

        // The draft records carry exactly the fields the read view shows, so the templates bind to
        // them directly instead of copying each one into a second row model.
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

        // An entry is selected now, so it can be written as well as read.
        PScribe.IsEnabled = true;
    }

    // Keeps the language on the same visual line as the headword and gives it the flag resolved by the
    // same path as the editor's picker. Flag lookup is asynchronous, so a result is painted only while
    // the display is still showing the language that asked for it.
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

    // The display must not assume an entry is selected: with none chosen it shows only its prompt, and
    // there is nothing to write, so the editor is closed and the toggle offers nothing.
    private void PDisplayClear()
    {
        _pDisplayEntry = null;
        _pDisplayRecording = null;
        _pDisplayPlayer.Stop();
        PDisplayLanguage.Text = string.Empty;
        PDisplayLanguageFlag.Source = null;
        PDisplayPlayback.Visibility = Visibility.Collapsed;
        PDisplayPronunciationSurface.Visibility = Visibility.Collapsed;
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
