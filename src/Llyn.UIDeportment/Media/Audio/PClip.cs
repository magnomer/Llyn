using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Llyn.Conduct;


namespace Llyn.UIDeportment;

public partial class PEditor
{
    private readonly PClipTemplate _pClipTemplate;
    private readonly ObservableCollection<PClipItem> _pClipItem = [];
    private bool _pClipSearching;
    private PClipReading? _pClipPreview;

    private Popup PClip => (Popup)FindName(nameof(PClip));

    private Border PClipProgress => (Border)FindName(nameof(PClipProgress));

    private TextBlock PClipNotice => (TextBlock)FindName(nameof(PClipNotice));

    private ItemsControl PClipList => (ItemsControl)FindName(nameof(PClipList));

    private Button PDownloader => (Button)FindName(nameof(PDownloader));

    private void PClipAttach()
    {
        PClipList.ItemsSource = _pClipItem;
        QLookItem.QLookItemAttach(PClipList, PClipRowApply);
        PClip.Closed += PClipClosedHandle;
        PDownloader.Click += PDownloaderHandle;
        PDownloader.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("download", 24));
    }

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
            if (_qEditor.QEditorArea.CEditorTimbre.CTimbreFlagged)
            {
                await LEnsignImage.LEnsignVarietyLoad(
                    _pEditorHost.PWindowAtelier,
                    _qEditor.QEditorArea.CEditorLanguage,
                    _qEditor.QEditorArea.CEditorTimbre.CTimbreVarietyNames);
                if (!PClip.IsOpen)
                {
                    return;
                }
            }

            _qEditor.QEditorArea.CEditorDesk.CDeskErrand.CErrandRecordingStart(
                word,
                target,
                LObserver.LObserverCreate<CHarvestStep>(
                    this, _qEditor.QEditorArea.CEditorDesk.CDeskErrand.CErrandHarvestResonate));
        }
        catch (Exception)
        {
            _pClipSearching = false;
            PClipUpdate();
        }
    }

    private void PClipCancel()
    {
        _qEditor.QEditorArea.CEditorDesk.CDeskErrand.CErrandCancel();
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

        PClipItem row = new(source, order, QLocalizationCatalog.QLocalizationTextRead("Downloader.Searching"));
        _pClipItem.Insert(position, row);
        return row;
    }

    private PClipReading PClipReadingCreate(CRecording recording)
    {
        string variety = recording.CRecordingVariety;
        string action = QLocalizationCatalog.QLocalizationTextRead("Downloader.Use");
        if (!recording.CRecordingRegional)
        {
            return new PClipReading(recording, string.Empty, null, action);
        }

        CVariety regional = CSounding.CSoundingVarietyRead(
            _qEditor.QEditorArea.CEditorDesk.CDeskErrand.CErrandRecordingLanguage, variety);
        return new PClipReading(
            recording,
            QAccentItem.QAccentLabelRefine(regional),
            QAccentItem.QAccentEnsignRefine(
                regional, _qEditor.QEditorArea.CEditorDesk.CDeskErrand.CErrandRecordingFlagged),
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

        PClipNotice.Text = QLocalizationCatalog.QLocalizationTextRead(
            _pClipSearching ? "Downloader.Searching" : "Downloader.Empty");
        PClipNotice.Visibility = Visibility.Visible;
    }

    private void PClipRowApply(FrameworkElement container, object item, string? _)
    {
        if (item is not PClipItem clip)
        {
            return;
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PClipOption") is TextBlock option)
        {
            option.Text = clip.PClipItemSource;
        }

        if (QLook.QLookPartFind<ItemsControl>(container, "PClipReadingList") is ItemsControl readings)
        {
            readings.ItemsSource = clip.PClipItemReading;
            readings.Visibility = QLook.QLookVisibleRead(clip.PClipItemReady);
            QLookItem.QLookItemAttach(readings, PClipReadingApply);
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PClipNote") is TextBlock note)
        {
            note.Text = clip.PClipItemNotice;
            note.Visibility = QLook.QLookVisibleRead(!clip.PClipItemReady);
        }
    }

    private void PClipReadingApply(FrameworkElement container, object item, string? _)
    {
        if (item is not PClipReading reading)
        {
            return;
        }

        if (QLook.QLookPartFind<ColumnDefinition>(container, "PClipColumn") is ColumnDefinition column)
        {
            column.SharedSizeGroup = string.Concat(
                "PClipColumn", ItemsControl.GetAlternationIndex(container).ToString(CultureInfo.InvariantCulture));
        }

        if (QLook.QLookPartFind<Image>(container, "PClipReadingFlag") is Image flag)
        {
            flag.Source = reading.PClipReadingFlag;
            flag.ToolTip = reading.PClipReadingLabel;
            flag.Visibility = QLook.QLookVisibleRead(reading.PClipReadingFlag is not null);
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PClipReadingLabel") is TextBlock label)
        {
            label.Text = reading.PClipReadingLabel;
            label.Visibility = QLook.QLookVisibleRead(
                reading.PClipReadingFlag is null && reading.PClipReadingLabel.Length > 0);
        }

        if (QLook.QLookPartFind<Button>(container, "PClipPreview") is Button preview)
        {
            preview.SetValue(
                QLook.QLookCueProperty,
                reading.PClipReadingPlaying ? QLookCue.QLookCuePlaying
                : reading.PClipReadingFetching ? QLookCue.QLookCueFetching
                : reading.PClipReadingRefused ? QLookCue.QLookCueRefused
                : QLookCue.QLookCueBase);
            preview.Click -= _pClipTemplate.PClipPreviewHandle;
            preview.Click += _pClipTemplate.PClipPreviewHandle;
        }

        if (QLook.QLookPartFind<QIconImage>(container, "PClipPreviewIcon") is QIconImage icon)
        {
            icon.QIconSource = QIcon.QIconResolve("play", 24);
        }

        if (QLook.QLookPartFind<Button>(container, "PClipSelector") is Button selector)
        {
            selector.Content = reading.PClipReadingAction;
            selector.IsEnabled = reading.PClipReadingReady;
            selector.Click -= _pClipTemplate.PClipSelectorHandle;
            selector.Click += _pClipTemplate.PClipSelectorHandle;
        }
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
            string path = await _pEditorHost.PWindowAtelier.CAtelierRecordingPrepare(
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
            || !_qEditor.QEditorArea.CEditorDesk.CDeskErrand.CErrandRecordingHeld)
        {
            return;
        }

        reading.PClipReadingAction = QLocalizationCatalog.QLocalizationTextRead("Downloader.Saving");
        reading.PClipReadingReady = false;

        try
        {
            bool attached = await _qEditor.QEditorArea.CEditorDesk.CDeskErrand.CErrandRecordingSave(
                reading.PClipReadingModel);
            reading.PClipReadingAction = QLocalizationCatalog.QLocalizationTextRead("Downloader.Saved");

            if (!attached)
            {
                return;
            }

            _qEditor.QEditorArea.CEditorVarietySet(
                _qEditor.QEditorArea.CEditorDesk.CDeskErrand.CErrandRecordingPrimary,
                _qEditor.QEditorArea.CEditorDesk.CDeskErrand.CErrandRecordingTarget,
                reading.PClipReadingVariety);
            PClip.IsOpen = false;
        }
        catch (Exception)
        {
            reading.PClipReadingAction = QLocalizationCatalog.QLocalizationTextRead("Downloader.Retry");
            reading.PClipReadingReady = true;
        }
    }

    internal void PClipSourceHandle(string source, int order)
    {
        PClipPlace(source, order);
        PClipUpdate();
    }

    internal void PClipRecordingHandle(CRecording recording)
    {
        PClipPlace(recording.CRecordingSource, recording.CRecordingOrder).PClipItemShow(
            recording,
            recording.CRecordingAddressed ? PClipReadingCreate(recording) : null,
            QLocalizationCatalog.QLocalizationTextRead("Downloader.Missing"),
            QLocalizationCatalog.QLocalizationTextRead("Downloader.Broken"));
        PClipUpdate();
    }

    internal void PClipFinishHandle()
    {
        _pClipSearching = false;
        PClipUpdate();
    }
}
