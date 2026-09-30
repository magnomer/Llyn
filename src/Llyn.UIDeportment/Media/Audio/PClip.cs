using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Llyn.Conduct;


namespace Llyn.UIDeportment;

public partial class PEditor
{
    private readonly PClipTemplate _pClipTemplate;

    private Popup PClip => (Popup)FindName(nameof(PClip));

    private Border PClipProgress => (Border)FindName(nameof(PClipProgress));

    private TextBlock PClipNotice => (TextBlock)FindName(nameof(PClipNotice));

    private ItemsControl PClipList => (ItemsControl)FindName(nameof(PClipList));

    private Button PDownloader => (Button)FindName(nameof(PDownloader));

    private void PClipAttach()
    {
        QLookItem.QLookItemAttach(PClipList, PClipRowApply);
        PClip.Closed += PClipClosedObserve;
        PDownloader.Click += PDownloaderRefine;
        PDownloader.Click += PDownloaderObserve;
        PDownloader.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("download", 24));
    }

    private void PDownloaderRefine(object sender, RoutedEventArgs e)
    {
        PClipOpenRefine(PDownloader);
    }

    private void PDownloaderObserve(object sender, RoutedEventArgs e)
    {
        PClipEnsignRefine(_qEditor.QEditorArea.CEditorDesk.CDeskErrand.CErrandRecordingStart(0));
    }

    private void PClipClosedObserve(object? sender, EventArgs e)
    {
        _qEditor.QEditorArea.CEditorDesk.CDeskErrand.CErrandCancel();
    }

    private void PClipOpenRefine(UIElement anchor)
    {
        PClip.IsOpen = false;
        PClip.PlacementTarget = anchor;
        PClip.IsOpen = true;
    }

    private async void PClipEnsignRefine(CClipRoll roll)
    {
        PClipRefine(roll);
        PClipRefine(await LEnsignImage.LEnsignLoad(_qEditor.QEditorArea.CEditorDesk.CDeskErrand.CErrandEnsignLoad));
    }

    internal void PClipRefine(CClipRoll roll)
    {
        PClipList.ItemsSource = QClipItem.QClipItemBuild(roll.CClipRollRows);
        PClipList.Visibility = QLook.QLookVisibleRead(!roll.CClipRollEmpty);
        PClipProgress.Visibility = QLook.QLookVisibleRead(roll.CClipRollSearching);
        PClipNotice.Text = QLocalizationCatalog.QLocalizationTextRead(roll.CClipRollNotice);
        PClipNotice.Visibility = QLook.QLookVisibleRead(roll.CClipRollEmpty);
    }

    private void PClipRowApply(FrameworkElement container, object item, string? _)
    {
        if (item is not QClipItem clip)
        {
            return;
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PClipOption") is TextBlock option)
        {
            option.Text = clip.QClipItemSource;
        }

        if (QLook.QLookPartFind<ItemsControl>(container, "PClipReadingList") is ItemsControl readings)
        {
            readings.ItemsSource = clip.QClipItemReading;
            readings.Visibility = QLook.QLookVisibleRead(clip.QClipItemReady);
            QLookItem.QLookItemAttach(readings, PClipReadingApply);
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PClipNote") is TextBlock note)
        {
            note.Text = clip.QClipItemNotice;
            note.Visibility = QLook.QLookVisibleRead(!clip.QClipItemReady);
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
            preview.Click -= PClipPreviewObserve;
            preview.Click += PClipPreviewObserve;
        }

        if (QLook.QLookPartFind<QIconImage>(container, "PClipPreviewIcon") is QIconImage icon)
        {
            icon.QIconSource = QIcon.QIconResolve("play", 24);
        }

        if (QLook.QLookPartFind<Button>(container, "PClipSelector") is Button selector)
        {
            selector.Content = reading.PClipReadingAction;
            selector.IsEnabled = reading.PClipReadingReady;
            selector.Click -= PClipSelectorObserve;
            selector.Click += PClipSelectorObserve;
        }
    }

    private async void PClipPreviewObserve(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement { DataContext: PClipReading reading })
        {
            PClipPreviewRefine(
                await _qEditor.QEditorArea.CEditorDesk.CDeskErrand.CErrandPreviewStart(reading.PClipReadingModel));
        }
    }

    private void PClipPreviewRefine(Uri? address)
    {
        if (address is null)
        {
            return;
        }

        _pDownloaderPlayer.Open(address);
        _pDownloaderPlayer.Play();
    }

    private void PClipEndObserve(object? sender, EventArgs e)
    {
        _qEditor.QEditorArea.CEditorDesk.CDeskErrand.CErrandPreviewFinish();
    }

    private async void PClipSelectorObserve(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement { DataContext: PClipReading reading })
        {
            PClipCloseRefine(
                await _qEditor.QEditorArea.CEditorDesk.CDeskErrand.CErrandRecordingSave(reading.PClipReadingModel));
        }
    }

    private void PClipCloseRefine(bool attached)
    {
        if (attached)
        {
            PClip.IsOpen = false;
        }
    }
}
