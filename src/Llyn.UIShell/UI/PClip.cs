using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Llyn.Core;

namespace Llyn.UIShell;

public partial class PEditor : LListener
{
    private readonly ObservableCollection<PClipItem> _pClipItem = [];
    private CancellationTokenSource? _pClipCancellation;
    private bool _pClipSearching;

    private async void PDownloaderCheckedHandle(object sender, RoutedEventArgs e)
    {
        await PClipStart();
    }

    private void PDownloaderUncheckedHandle(object sender, RoutedEventArgs e)
    {
        PClipCancel();
    }

    private async Task PClipStart()
    {
        PClipCancel();

        string word = PHeadword.Text?.Trim() ?? string.Empty;
        _pClipItem.Clear();
        _pClipSearching = word.Length > 0;
        PClipUpdate();

        if (word.Length == 0)
        {
            return;
        }

        _pClipCancellation = new CancellationTokenSource();

        try
        {
            await _lEngine.LEngineRecordingFind(word, _pSpeakerChoice, this, _pClipCancellation.Token);
        }
        catch (OperationCanceledException)
        {
        }
        catch (Exception)
        {
            _pClipSearching = false;
            PClipUpdate();
        }
    }

    private void PClipCancel()
    {
        _pClipCancellation?.Cancel();
        _pClipCancellation?.Dispose();
        _pClipCancellation = null;
    }

    private void PClipUpdate()
    {
        bool recordings = _pClipItem.Count > 0;

        PClipList.Visibility = recordings ? Visibility.Visible : Visibility.Collapsed;
        PClipProgress.Visibility = _pClipSearching ? Visibility.Visible : Visibility.Collapsed;

        if (recordings)
        {
            PClipNotice.Visibility = Visibility.Collapsed;
            return;
        }

        PClipNotice.Text = _pEditorHost.PLocalizationTextRead(
            _pClipSearching ? "Downloader.Searching" : "Downloader.Empty");
        PClipNotice.Visibility = Visibility.Visible;
    }

    internal async void PClipPreviewHandle(object sender, RoutedEventArgs e)
    {
        if (sender is not FrameworkElement { DataContext: PClipItem recording })
        {
            return;
        }

        try
        {
            string path = await _lEngine.LEngineRecordingPrepare(recording.PClipItemModel, CancellationToken.None);
            _pDownloaderPlayer.Open(new Uri(path));
            _pDownloaderPlayer.Play();
        }
        catch (Exception)
        {
        }
    }

    internal async void PClipSelectorHandle(object sender, RoutedEventArgs e)
    {
        if (sender is not FrameworkElement { DataContext: PClipItem recording })
        {
            return;
        }

        string word = PHeadword.Text?.Trim() ?? string.Empty;
        if (word.Length == 0)
        {
            return;
        }

        string language = _pSpeakerChoice;

        recording.PClipItemAction = _pEditorHost.PLocalizationTextRead("Downloader.Saving");
        recording.PClipItemReady = false;

        try
        {
            string path = await _lEngine.LEngineRecordingSave(recording.PClipItemModel, word, language, CancellationToken.None);
            recording.PClipItemAction = _pEditorHost.PLocalizationTextRead("Downloader.Saved");

            if (!string.Equals(PHeadword.Text?.Trim(), word, StringComparison.Ordinal) ||
                !string.Equals(_pSpeakerChoice, language, StringComparison.Ordinal))
            {
                return;
            }

            _pRecording = path;
            _pRecordingSource = recording.PClipItemSource;
            _pRecordingStored = false;
            PPlayback.Visibility = Visibility.Visible;

            PDownloader.IsChecked = false;
        }
        catch (Exception)
        {
            recording.PClipItemAction = _pEditorHost.PLocalizationTextRead("Downloader.Retry");
            recording.PClipItemReady = true;
        }
    }

    void LListener.LListenerSourceStart(string source)
    {
    }

    void LListener.LListenerRecordingAdd(LRecording recording)
    {
        Dispatcher.Invoke(() =>
        {
            _pClipItem.Add(new PClipItem(
                recording,
                _pEditorHost.PLocalizationTextRead("Downloader.Use")));
            PClipUpdate();
        });
    }

    void LListener.LListenerFinish()
    {
        Dispatcher.Invoke(() =>
        {
            _pClipSearching = false;
            PClipUpdate();
        });
    }
}
