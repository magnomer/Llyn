using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Llyn.Core;

namespace Llyn.UIShell;

/// <summary>
/// Audio download as the editor shows it: opening the menu starts a search for recordings of the
/// headword, each found recording can be previewed, and taking one downloads it into the workspace
/// and attaches it to the form. This is the shell side of <see cref="LListener"/> — the engine calls
/// back on a worker thread, so every arrival is marshalled onto the dispatcher here.
/// </summary>
public partial class PEditor : LListener
{
    private readonly ObservableCollection<PDownloaderRecording> _pDownloaderRecording = [];
    private CancellationTokenSource? _pDownloaderCancellation;
    private bool _pDownloaderSearching;

    private async void PDownloaderCheckedHandle(object sender, RoutedEventArgs e)
    {
        await PDownloaderStart();
    }

    private void PDownloaderUncheckedHandle(object sender, RoutedEventArgs e)
    {
        PDownloaderCancel();
    }

    private async Task PDownloaderStart()
    {
        PDownloaderCancel();

        string word = PHeadword.Text?.Trim() ?? string.Empty;
        _pDownloaderRecording.Clear();
        _pDownloaderSearching = word.Length > 0;
        PDownloaderMenuUpdate();

        if (word.Length == 0)
        {
            return;
        }

        _pDownloaderCancellation = new CancellationTokenSource();

        try
        {
            // The panel asks and then listens: the search is over when the listener is told it is,
            // never when this call returns. The engine reports the end through LListenerFinish, and
            // reading completion off the awaited task as well gave the menu a second opinion about a
            // search it does not run.
            await _lEngine.LEngineRecordingFind(word, _pLangcodeChoice, this, _pDownloaderCancellation.Token);
        }
        catch (OperationCanceledException)
        {
            // Superseded by a newer discovery or the window closed; ignore.
        }
        catch (Exception)
        {
            // A discovery that could not be started reports no end of its own, so the menu is taken
            // out of its searching state here rather than left running under a search that never began.
            _pDownloaderSearching = false;
            PDownloaderMenuUpdate();
        }
    }

    private void PDownloaderCancel()
    {
        _pDownloaderCancellation?.Cancel();
        _pDownloaderCancellation?.Dispose();
        _pDownloaderCancellation = null;
    }

    // What the menu shows, from the two things it knows: whether the search is still running, and what
    // has arrived so far. They are independent — a source that has already answered does not end the
    // search — which is why the running line follows the search alone. Reading it off "nothing found
    // yet" instead is what left it running under a menu that was plainly finished.
    private void PDownloaderMenuUpdate()
    {
        bool recordings = _pDownloaderRecording.Count > 0;

        PDownloaderMenuList.Visibility = recordings ? Visibility.Visible : Visibility.Collapsed;
        PDownloaderMenuProgress.Visibility = _pDownloaderSearching ? Visibility.Visible : Visibility.Collapsed;

        // The notice is the one line the menu says while it has no rows to show: what it is doing, or
        // that there was nothing to find. With rows on screen it says nothing — a row reports its own
        // download itself.
        if (recordings)
        {
            PDownloaderMenuNotice.Visibility = Visibility.Collapsed;
            return;
        }

        PDownloaderMenuNotice.Text = _pEditorHost.PLocalizationTextRead(
            _pDownloaderSearching ? "Downloader.Searching" : "Downloader.Empty");
        PDownloaderMenuNotice.Visibility = Visibility.Visible;
    }

    internal async void PDownloaderPreviewHandle(object sender, RoutedEventArgs e)
    {
        if (sender is not FrameworkElement { DataContext: PDownloaderRecording recording })
        {
            return;
        }

        try
        {
            // Streaming the remote, token-bearing URL through the media stack is unreliable; fetch it
            // to a local temp file first, then play that.
            string path = await _lEngine.LEngineRecordingPrepare(recording.PDownloaderRecordingModel, CancellationToken.None);
            _pDownloaderPlayer.Open(new Uri(path));
            _pDownloaderPlayer.Play();
        }
        catch (Exception)
        {
            // Preview is best-effort; a failed fetch leaves the menu untouched.
        }
    }

    internal async void PDownloaderMenuHandle(object sender, RoutedEventArgs e)
    {
        if (sender is not FrameworkElement { DataContext: PDownloaderRecording recording })
        {
            return;
        }

        string word = PHeadword.Text?.Trim() ?? string.Empty;
        if (word.Length == 0)
        {
            return;
        }

        string language = _pLangcodeChoice;

        // The download is reported on the row that was taken, not on the status card: that card
        // belongs to the search, and a recording still arriving would overwrite whatever was
        // written there.
        recording.PDownloaderRecordingAction = _pEditorHost.PLocalizationTextRead("Downloader.Saving");
        recording.PDownloaderRecordingReady = false;

        try
        {
            string path = await _lEngine.LEngineRecordingSave(recording.PDownloaderRecordingModel, word, language, CancellationToken.None);
            recording.PDownloaderRecordingAction = _pEditorHost.PLocalizationTextRead("Downloader.Saved");

            if (!string.Equals(PHeadword.Text?.Trim(), word, StringComparison.Ordinal) ||
                !string.Equals(_pLangcodeChoice, language, StringComparison.Ordinal))
            {
                // The form moved on while the bytes came down; the file stays in the workspace, but
                // it is audio of a word the form no longer holds.
                return;
            }

            _pRecording = path;
            _pRecordingSource = recording.PDownloaderRecordingSource;
            // Fetched for the headword as it stands now, so a further edit of it drops this.
            _pRecordingStored = false;
            PPlayback.Visibility = Visibility.Visible;

            // Taking a recording closes the menu, the way taking a pronunciation candidate does.
            PDownloader.IsChecked = false;
        }
        catch (Exception)
        {
            // A failed download leaves the row offering another try.
            recording.PDownloaderRecordingAction = _pEditorHost.PLocalizationTextRead("Downloader.Retry");
            recording.PDownloaderRecordingReady = true;
        }
    }

    void LListener.LListenerSourceStart(string source)
    {
        // Handled the same way as lookup: the running line already covers the whole search.
    }

    void LListener.LListenerRecordingAdd(LRecording recording)
    {
        Dispatcher.Invoke(() =>
        {
            _pDownloaderRecording.Add(new PDownloaderRecording(
                recording,
                _pEditorHost.PLocalizationTextRead("Downloader.Use")));
            PDownloaderMenuUpdate();
        });
    }

    void LListener.LListenerFinish()
    {
        Dispatcher.Invoke(() =>
        {
            _pDownloaderSearching = false;
            PDownloaderMenuUpdate();
        });
    }
}
