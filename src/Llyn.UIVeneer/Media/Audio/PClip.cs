using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Llyn.Core;
using Llyn.ShellEngine;

using Llyn.UIDeportment;

namespace Llyn.UIVeneer;

public partial class PEditor
{
    private readonly ObservableCollection<PClipItem> _pClipItem = [];
    private bool _pClipSearching;
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
        PClip.PlacementTarget = anchor;
        PClip.IsOpen = true;
        await PClipStart(target);
    }

    private async Task PClipStart(long target)
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

        try
        {
            if (_lEditor.LEditorFlagged)
            {
                await LEnsignImage.LEnsignVarietyLoad(_pEditorHost.PWindowDeportment, _lEditor);
                if (!PClip.IsOpen)
                {
                    return;
                }
            }

            _lEditor.LEditorClipStart(
                word, target, LObserver.LObserverCreate<LHarvestStep>(this, _lEditor.LEditorClip.LClipStepHandle));
        }
        catch (Exception)
        {
            _pClipSearching = false;
            PClipUpdate();
        }
    }

    private void PClipCancel()
    {
        _lEditor.LEditorClip.LClipCancel();
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

        PClipItem row = new(source, order, PLocalizationCatalog.PLocalizationTextRead("Downloader.Searching"));
        _pClipItem.Insert(position, row);
        return row;
    }

    private PClipReading PClipReadingCreate(LRecording recording)
    {
        string variety = recording.LRecordingVariety;
        string action = PLocalizationCatalog.PLocalizationTextRead("Downloader.Use");
        if (!recording.LRecordingRegional)
        {
            return new PClipReading(recording, string.Empty, null, action);
        }

        return new PClipReading(
            recording,
            LAccentItem.LAccentLabelFormat(variety),
            LAccentItem.LAccentFlagFind(
                _lEditor.LEditorClip.LClipLanguage, _lEditor.LEditorClip.LClipFlagged, variety),
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

        PClipNotice.Text = PLocalizationCatalog.PLocalizationTextRead(
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
            string path = await _pEditorHost.PWindowDeportment.LWindowRecordingPrepare(
                reading.PClipReadingModel, CancellationToken.None);
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
        if (sender is not FrameworkElement { DataContext: PClipReading reading }
            || !reading.PClipReadingReady
            || !_lEditor.LEditorClip.LClipHeld)
        {
            return;
        }

        reading.PClipReadingAction = PLocalizationCatalog.PLocalizationTextRead("Downloader.Saving");
        reading.PClipReadingReady = false;

        try
        {
            bool attached = await _lEditor.LEditorClip.LClipRecordingSave(reading.PClipReadingModel);
            reading.PClipReadingAction = PLocalizationCatalog.PLocalizationTextRead("Downloader.Saved");

            if (!attached)
            {
                return;
            }

            long id = _lEditor.LEditorClip.LClipPrimary
                ? PNotationDraftRead()?.LEntryDraftPronunciation?.LPronunciationDraftId ?? 0
                : _lEditor.LEditorClip.LClipTarget;

            PNotationVarietySend(id, reading.PClipReadingVariety);
            PClip.IsOpen = false;
        }
        catch (Exception)
        {
            reading.PClipReadingAction = PLocalizationCatalog.PLocalizationTextRead("Downloader.Retry");
            reading.PClipReadingReady = true;
        }
    }

    private void PClipSourceHandle(string source, int order)
    {
        PClipPlace(source, order);
        PClipUpdate();
    }

    private void PClipRecordingHandle(LRecording recording)
    {
        PClipPlace(recording.LRecordingSource, recording.LRecordingOrder).PClipItemShow(
            recording,
            recording.LRecordingAddressed ? PClipReadingCreate(recording) : null,
            PLocalizationCatalog.PLocalizationTextRead("Downloader.Missing"),
            PLocalizationCatalog.PLocalizationTextRead("Downloader.Broken"));
        PClipUpdate();
    }

    private void PClipFinishHandle()
    {
        _pClipSearching = false;
        PClipUpdate();
    }
}
