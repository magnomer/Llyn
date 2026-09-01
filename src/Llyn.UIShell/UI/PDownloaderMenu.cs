using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Llyn.Core;

namespace Llyn.UIShell;

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
            await _lEngine.LEngineRecordingFind(word, _pLangcodeChoice, this, _pDownloaderCancellation.Token);
        }
        catch (OperationCanceledException)
        {
        }
        catch (Exception)
        {
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

    private void PDownloaderMenuUpdate()
    {
        bool recordings = _pDownloaderRecording.Count > 0;

        PDownloaderMenuList.Visibility = recordings ? Visibility.Visible : Visibility.Collapsed;
        PDownloaderMenuProgress.Visibility = _pDownloaderSearching ? Visibility.Visible : Visibility.Collapsed;

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
            string path = await _lEngine.LEngineRecordingPrepare(recording.PDownloaderRecordingModel, CancellationToken.None);
            _pDownloaderPlayer.Open(new Uri(path));
            _pDownloaderPlayer.Play();
        }
        catch (Exception)
        {
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

        recording.PDownloaderRecordingAction = _pEditorHost.PLocalizationTextRead("Downloader.Saving");
        recording.PDownloaderRecordingReady = false;

        try
        {
            string path = await _lEngine.LEngineRecordingSave(recording.PDownloaderRecordingModel, word, language, CancellationToken.None);
            recording.PDownloaderRecordingAction = _pEditorHost.PLocalizationTextRead("Downloader.Saved");

            if (!string.Equals(PHeadword.Text?.Trim(), word, StringComparison.Ordinal) ||
                !string.Equals(_pLangcodeChoice, language, StringComparison.Ordinal))
            {
                return;
            }

            _pRecording = path;
            _pRecordingSource = recording.PDownloaderRecordingSource;
            _pRecordingStored = false;
            PPlayback.Visibility = Visibility.Visible;

            PDownloader.IsChecked = false;
        }
        catch (Exception)
        {
            recording.PDownloaderRecordingAction = _pEditorHost.PLocalizationTextRead("Downloader.Retry");
            recording.PDownloaderRecordingReady = true;
        }
    }

    void LListener.LListenerSourceStart(string source)
    {
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
