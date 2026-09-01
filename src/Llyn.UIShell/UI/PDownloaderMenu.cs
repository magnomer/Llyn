using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Llyn.Core;

namespace Llyn.UIShell;

/// <summary>
/// Audio download as the input panel shows it: opening the menu starts a search for recordings of the
/// headword, each found recording can be previewed, and choosing one downloads it into the workspace
/// and attaches it to the form. This is the shell side of <see cref="LListener"/> — the engine calls
/// back on a worker thread, so every arrival is marshalled onto the dispatcher here.
/// </summary>
public partial class PInput : LListener
{
    private readonly ObservableCollection<PDownloaderRecording> _pDownloaderRecording = [];
    private CancellationTokenSource? _lHarvestCancellation;
    private bool _lHarvestSearching;

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
        _lHarvestSearching = word.Length > 0;
        PDownloaderStatusUpdate();

        if (word.Length == 0)
        {
            return;
        }

        _lHarvestCancellation = new CancellationTokenSource();
        CancellationToken token = _lHarvestCancellation.Token;

        try
        {
            await _lEngine.LEngineRecordingFind(word, _pLangcodeChoice, this, token);
        }
        catch (OperationCanceledException)
        {
            // Superseded by a newer discovery or the window closed; ignore.
        }
        finally
        {
            if (!token.IsCancellationRequested)
            {
                _lHarvestSearching = false;
                PDownloaderStatusUpdate();
            }
        }
    }

    private void PDownloaderCancel()
    {
        _lHarvestCancellation?.Cancel();
        _lHarvestCancellation?.Dispose();
        _lHarvestCancellation = null;
    }

    private void PDownloaderStatusUpdate()
    {
        bool hasRecordings = _pDownloaderRecording.Count > 0;
        PDownloaderMenuList.Visibility = hasRecordings ? Visibility.Visible : Visibility.Collapsed;

        if (_lHarvestSearching && !hasRecordings)
        {
            PDownloaderMenuStatus.Text = _pInputHost.PLocalizationTextRead("Downloader.Searching");
            PDownloaderStatusCard.Visibility = Visibility.Visible;
            PDownloaderProgress.Visibility = Visibility.Visible;
        }
        else if (!_lHarvestSearching && !hasRecordings)
        {
            PDownloaderMenuStatus.Text = _pInputHost.PLocalizationTextRead("Downloader.Empty");
            PDownloaderStatusCard.Visibility = Visibility.Visible;
            PDownloaderProgress.Visibility = Visibility.Collapsed;
        }
        else
        {
            PDownloaderStatusCard.Visibility = Visibility.Collapsed;
        }
    }

    internal async void PDownloaderPlayHandle(object sender, RoutedEventArgs e)
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
        PDownloaderMenuStatus.Text = _pInputHost.PLocalizationTextRead("Downloader.Saving");
        PDownloaderStatusCard.Visibility = Visibility.Visible;
        PDownloaderProgress.Visibility = Visibility.Visible;

        try
        {
            string path = await _lEngine.LEngineRecordingSave(recording.PDownloaderRecordingModel, word, language, CancellationToken.None);
            PDownloaderMenuStatus.Text = _pInputHost.PLocalizationTextRead("Downloader.Saved");
            PDownloaderProgress.Visibility = Visibility.Collapsed;

            if (string.Equals(PHeadword.Text?.Trim(), word, StringComparison.Ordinal) &&
                string.Equals(_pLangcodeChoice, language, StringComparison.Ordinal))
            {
                _pRecording = path;
                _pRecordingSource = recording.PDownloaderRecordingSource;
                // Fetched for the headword as it stands now, so a further edit of it drops this.
                _pRecordingStored = false;
                PPlayback.Visibility = Visibility.Visible;
            }
        }
        catch (Exception)
        {
            // A failed download leaves the menu open with a failure notice; the user can retry.
            PDownloaderMenuStatus.Text = _pInputHost.PLocalizationTextRead("Downloader.Failed");
            PDownloaderProgress.Visibility = Visibility.Collapsed;
        }
    }

    void LListener.LListenerSourceStart(string source)
    {
        // Handled the same way as lookup: the shared "Searching…" status already covers per-source starts.
    }

    void LListener.LListenerRecordingAdd(LRecording recording)
    {
        Dispatcher.Invoke(() =>
        {
            _pDownloaderRecording.Add(new PDownloaderRecording(recording));
            PDownloaderStatusUpdate();
        });
    }

    void LListener.LListenerFinish()
    {
        Dispatcher.Invoke(() =>
        {
            _lHarvestSearching = false;
            PDownloaderStatusUpdate();
        });
    }
}
