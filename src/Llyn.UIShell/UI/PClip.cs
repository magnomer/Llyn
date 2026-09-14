using System;
using System.Collections.ObjectModel;
using System.Linq;
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
    private bool _pClipFlagged;
    private string _pClipLanguage = string.Empty;
    private long _pClipTarget;
    private PClipReading? _pClipPreview;

    private async void PDownloaderHandle(object sender, RoutedEventArgs e)
    {
        await PClipOpen(PDownloader, 0);
    }

    private void PClipClosedHandle(object? sender, EventArgs e)
    {
        PClipCancel();
        PClipPreviewClear();
    }

    private async Task PClipOpen(UIElement anchor, long target)
    {
        PClip.IsOpen = false;
        _pClipTarget = target;
        PClip.PlacementTarget = anchor;
        PClip.IsOpen = true;
        await PClipStart();
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
        CancellationToken cancellation = _pClipCancellation.Token;

        try
        {
            _pClipLanguage = _pSpeakerChoice;
            _pClipFlagged = _lEngine.LEngineFlaggedCheck(_pClipLanguage);
            if (_pClipFlagged)
            {
                await PEnsign.PEnsignVarietyLoad(
                    _lEngine,
                    _pClipLanguage,
                    _lEngine.LEngineVarietyRead(_pClipLanguage).Select(variety => variety.LVarietyName));
                cancellation.ThrowIfCancellationRequested();
            }

            await _lEngine.LEngineRecordingFind(
                _pEditorDraft, word, _pClipLanguage, _pClipTarget, this, cancellation);
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

    private PClipItem PClipPlace(string source, int order)
    {
        int position = 0;
        while (position < _pClipItem.Count &&
            _pClipItem[position].PClipItemOrder < order)
        {
            position++;
        }

        if (position < _pClipItem.Count && _pClipItem[position].PClipItemOrder == order)
        {
            return _pClipItem[position];
        }

        PClipItem row = new(source, order, _pEditorHost.PLocalizationTextRead("Downloader.Searching"));
        _pClipItem.Insert(position, row);
        return row;
    }

    private PClipReading PClipReadingCreate(LRecording recording)
    {
        string variety = recording.LRecordingVariety;
        string action = _pEditorHost.PLocalizationTextRead("Downloader.Use");
        if (variety.Length == 0)
        {
            return new PClipReading(recording, string.Empty, null, action);
        }

        return new PClipReading(
            recording,
            PAccentItem.PAccentLabelFormat(_pEditorHost, variety),
            PAccentItem.PAccentFlagFind(_pClipLanguage, _pClipFlagged, variety),
            action);
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
        if (sender is not FrameworkElement { DataContext: PClipReading reading })
        {
            return;
        }

        PClipPreviewClear();
        _pClipPreview = reading;
        reading.PClipReadingRefused = false;
        reading.PClipReadingFetching = true;

        try
        {
            string path = await _lEngine.LEngineRecordingPrepare(reading.PClipReadingModel, CancellationToken.None);
            if (_pClipPreview != reading)
            {
                return;
            }

            reading.PClipReadingFetching = false;
            reading.PClipReadingPlaying = true;
            _pDownloaderPlayer.Open(new Uri(path));
            _pDownloaderPlayer.Play();
        }
        catch (Exception)
        {
            reading.PClipReadingFetching = false;
            reading.PClipReadingRefused = true;
        }
    }

    private void PClipPreviewClear()
    {
        if (_pClipPreview is null)
        {
            return;
        }

        _pClipPreview.PClipReadingFetching = false;
        _pClipPreview.PClipReadingPlaying = false;
        _pClipPreview = null;
    }

    private void PClipEndHandle(object? sender, EventArgs e)
    {
        PClipPreviewClear();
    }

    internal async void PClipSelectorHandle(object sender, RoutedEventArgs e)
    {
        if (sender is not FrameworkElement { DataContext: PClipReading reading } || !reading.PClipReadingReady)
        {
            return;
        }

        string word = PHeadword.Text?.Trim() ?? string.Empty;
        if (word.Length == 0)
        {
            return;
        }

        string language = _pClipLanguage;
        long target = _pClipTarget;
        LRecording recording = reading.PClipReadingModel;

        reading.PClipReadingAction = _pEditorHost.PLocalizationTextRead("Downloader.Saving");
        reading.PClipReadingReady = false;

        try
        {
            string path = await _lEngine.LEngineRecordingSave(recording, word, language, CancellationToken.None);
            reading.PClipReadingAction = _pEditorHost.PLocalizationTextRead("Downloader.Saved");

            if (!string.Equals(PHeadword.Text?.Trim(), word, StringComparison.Ordinal) ||
                !string.Equals(_pSpeakerChoice, language, StringComparison.Ordinal))
            {
                return;
            }

            long id = target;
            if (target == 0)
            {
                _pRecording = path;
                _pRecordingSource = recording.LRecordingSource;
                _pRecordingStored = false;
                PPlaybackAction.Visibility = Visibility.Visible;
                PEditorAudioSend();
                id = PNotationDraftRead()?.LEntryDraftPronunciation?.LPronunciationDraftId ?? 0;
            }
            else
            {
                _pRecordingFresh.Add(target);
                PEditorRequestSend(
                    new LRequestPronunciationAudio(_pEditorDraft, target, path, recording.LRecordingSource));
            }

            PNotationVarietySend(id, reading.PClipReadingVariety);
            PPlaybackTrayShow();
            PVolumeLoad();
            PClip.IsOpen = false;
        }
        catch (Exception)
        {
            reading.PClipReadingAction = _pEditorHost.PLocalizationTextRead("Downloader.Retry");
            reading.PClipReadingReady = true;
        }
    }

    void LListener.LListenerSourceStart(string source, int order)
    {
        Dispatcher.BeginInvoke(() =>
        {
            PClipPlace(source, order);
            PClipUpdate();
        });
    }

    void LListener.LListenerRecordingAdd(LRecording recording)
    {
        Dispatcher.BeginInvoke(() =>
        {
            PClipPlace(recording.LRecordingSource, recording.LRecordingOrder).PClipItemShow(
                recording,
                string.IsNullOrEmpty(recording.LRecordingAddress) ? null : PClipReadingCreate(recording),
                _pEditorHost.PLocalizationTextRead("Downloader.Missing"),
                _pEditorHost.PLocalizationTextRead("Downloader.Broken"));
            PClipUpdate();
        });
    }

    void LListener.LListenerFinish()
    {
        Dispatcher.BeginInvoke(() =>
        {
            _pClipSearching = false;
            PClipUpdate();
        });
    }
}
