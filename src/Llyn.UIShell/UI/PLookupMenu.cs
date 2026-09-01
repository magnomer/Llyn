using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Llyn.Core;

namespace Llyn.UIShell;

/// <summary>
/// Pronunciation lookup as the editor shows it: opening the menu starts a search for the
/// headword, candidates stream in from the engine and fill the list, and picking one writes it into
/// the pronunciation field. This is the shell side of <see cref="LReceiver"/> — the engine calls back
/// on a worker thread, so every arrival is marshalled onto the dispatcher here.
/// </summary>
public partial class PEditor : LReceiver
{
    private readonly ObservableCollection<PLookupCandidate> _pLookupCandidate = [];
    private CancellationTokenSource? _pLookupCancellation;
    private bool _pLookupSearching;

    private async void PLookupCheckedHandle(object sender, RoutedEventArgs e)
    {
        await PLookupStart();
    }

    private void PLookupUncheckedHandle(object sender, RoutedEventArgs e)
    {
        PLookupCancel();
    }

    internal void PLookupMenuHandle(object sender, RoutedEventArgs e)
    {
        if (sender is not FrameworkElement { DataContext: PLookupCandidate candidate })
        {
            return;
        }

        PPronunciation.Text = candidate.PLookupCandidatePhonetic;
        PLookup.IsChecked = false;
    }

    private async Task PLookupStart()
    {
        PLookupCancel();

        string word = PHeadword.Text?.Trim() ?? string.Empty;
        _pLookupCandidate.Clear();
        _pLookupSearching = word.Length > 0;
        PLookupMenuUpdate();

        if (word.Length == 0)
        {
            return;
        }

        _pLookupCancellation = new CancellationTokenSource();

        try
        {
            // The panel asks and then listens: the search is over when the receiver is told it is,
            // never when this call returns. The two are not the same moment — the engine reports the
            // end through LReceiverLookupFinish, and reading completion off the awaited task as well
            // gave the menu a second opinion about a search it does not run.
            await _lEngine.LEnginePronunciationFind(word, _pLangcodeChoice, this, _pLookupCancellation.Token);
        }
        catch (OperationCanceledException)
        {
            // Superseded by a newer lookup or the window closed; ignore.
        }
        catch (Exception)
        {
            // A lookup that could not be started reports no end of its own, so the menu is taken out
            // of its searching state here rather than left running under a search that never began.
            _pLookupSearching = false;
            PLookupMenuUpdate();
        }
    }

    private void PLookupCancel()
    {
        _pLookupCancellation?.Cancel();
        _pLookupCancellation?.Dispose();
        _pLookupCancellation = null;
    }

    // What the menu shows, from the two things it knows: whether the search is still running, and
    // what has arrived so far. They are independent — a source that has already answered does not end
    // the search — which is why the running line follows the search alone. Reading it off "nothing
    // found yet" instead is what left it running under a menu that was plainly finished.
    private void PLookupMenuUpdate()
    {
        bool candidates = _pLookupCandidate.Count > 0;

        PLookupMenuList.Visibility = candidates ? Visibility.Visible : Visibility.Collapsed;
        PLookupMenuProgress.Visibility = _pLookupSearching ? Visibility.Visible : Visibility.Collapsed;

        // The notice is the one line the menu says while it has no rows to show: what it is doing, or
        // that there was nothing to find. With rows on screen it says nothing.
        if (candidates)
        {
            PLookupMenuNotice.Visibility = Visibility.Collapsed;
            return;
        }

        PLookupMenuNotice.Text = _pEditorHost.PLocalizationTextRead(
            _pLookupSearching ? "Lookup.Searching" : "Lookup.Empty");
        PLookupMenuNotice.Visibility = Visibility.Visible;
    }

    void LReceiver.LReceiverSourceStart(string source)
    {
        // Each source's arrival is surfaced through LReceiverCandidateAdd; the running line is already
        // shown for the whole search, so no per-source update is needed here.
    }

    void LReceiver.LReceiverCandidateAdd(LCandidate candidate)
    {
        Dispatcher.Invoke(() =>
        {
            _pLookupCandidate.Add(new PLookupCandidate(candidate, candidate.LCandidateSource));
            PLookupMenuUpdate();
        });
    }

    void LReceiver.LReceiverLookupFinish()
    {
        Dispatcher.Invoke(() =>
        {
            _pLookupSearching = false;
            PLookupMenuUpdate();
        });
    }
}
