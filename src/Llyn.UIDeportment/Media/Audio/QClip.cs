using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using Llyn.Conduct;


namespace Llyn.UIDeportment;

internal sealed class QClip
{
    private readonly FrameworkElement _qClipSurface;

    private readonly MediaPlayer _qClipPlayer;

    private CEditor _cEditor = null!;

    internal QClip(FrameworkElement surface, MediaPlayer player)
    {
        _qClipSurface = surface;
        _qClipPlayer = player;
        QLookItem.QLookItemAttach(QClipList, QClipRowApply);
        QClipPopup.Closed += QClipClosedObserve;
        QClipButton.Click += QClipButtonRefine;
        QClipButton.Click += QClipButtonObserve;
        QClipButton.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("download", 24));
        _qClipPlayer.MediaEnded += QClipEndObserve;
        _qClipPlayer.MediaFailed += QClipEndObserve;
    }

    private Popup QClipPopup => QContract.QContractFind<Popup>(_qClipSurface, "PClip");

    private Border QClipProgress => QContract.QContractFind<Border>(_qClipSurface, "PClipProgress");

    private TextBlock QClipNotice => QContract.QContractFind<TextBlock>(_qClipSurface, "PClipNotice");

    private ItemsControl QClipList => QContract.QContractFind<ItemsControl>(_qClipSurface, "PClipList");

    private Button QClipButton => QContract.QContractFind<Button>(_qClipSurface, "PDownloader");

    internal void QClipIntroduce(CEditor editor)
    {
        _cEditor = editor;
        editor.CEditorDesk.CDeskErrand.CErrandClipChanged += QClipRefine;
    }

    private void QClipButtonRefine(object sender, RoutedEventArgs e)
    {
        QClipOpenRefine(QClipButton);
    }

    private void QClipButtonObserve(object sender, RoutedEventArgs e)
    {
        QClipRecordingStart(null);
    }

    private void QClipClosedObserve(object? sender, EventArgs e)
    {
        _cEditor.CEditorDesk.CDeskErrand.CErrandCancel();
    }

    internal void QClipOpenRefine(UIElement anchor)
    {
        QClipPopup.IsOpen = false;
        QClipPopup.PlacementTarget = anchor;
        QClipPopup.IsOpen = true;
    }

    internal void QClipRecordingStart(long? id)
    {
        QClipEnsignRefine(_cEditor.CEditorDesk.CDeskErrand.CErrandRecordingStart(id));
    }

    private async void QClipEnsignRefine(CClipRoll roll)
    {
        QClipRefine(roll);
        QClipRefine(await _cEditor.CEditorDesk.CDeskErrand.CErrandEnsignLoad(QEnsignImage.QEnsignDraw));
    }

    private void QClipRefine(CClipRoll roll)
    {
        QClipList.ItemsSource = QClipItem.QClipItemBuild(roll.CClipRollRows);
        QClipList.Visibility = QLook.QLookVisibleRead(!roll.CClipRollEmpty);
        QClipProgress.Visibility = QLook.QLookVisibleRead(roll.CClipRollSearching);
        QClipNotice.Text = QLocalizationCatalog.QLocalizationTextRead(roll.CClipRollNotice);
        QClipNotice.Visibility = QLook.QLookVisibleRead(roll.CClipRollEmpty);
    }

    private void QClipRowApply(FrameworkElement container, object item, string? _)
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
            QLookItem.QLookItemAttach(readings, QClipReadingApply);
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PClipNote") is TextBlock note)
        {
            note.Text = clip.QClipItemNotice;
            note.Visibility = QLook.QLookVisibleRead(!clip.QClipItemReady);
        }
    }

    private void QClipReadingApply(FrameworkElement container, object item, string? _)
    {
        if (item is not QClipReading reading)
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
            flag.Source = reading.QClipReadingFlag;
            flag.ToolTip = reading.QClipReadingLabel;
            flag.Visibility = QLook.QLookVisibleRead(reading.QClipReadingFlag is not null);
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PClipReadingLabel") is TextBlock label)
        {
            label.Text = reading.QClipReadingLabel;
            label.Visibility = QLook.QLookVisibleRead(
                reading.QClipReadingFlag is null && reading.QClipReadingLabel.Length > 0);
        }

        if (QLook.QLookPartFind<Button>(container, "PClipPreview") is Button preview)
        {
            preview.SetValue(
                QLook.QLookCueProperty,
                reading.QClipReadingPlaying ? QLookCue.QLookCuePlaying
                : reading.QClipReadingFetching ? QLookCue.QLookCueFetching
                : reading.QClipReadingRefused ? QLookCue.QLookCueRefused
                : QLookCue.QLookCueBase);
            preview.Click -= QClipPreviewObserve;
            preview.Click += QClipPreviewObserve;
        }

        if (QLook.QLookPartFind<QIconImage>(container, "PClipPreviewIcon") is QIconImage icon)
        {
            icon.QIconSource = QIcon.QIconResolve("play", 24);
        }

        if (QLook.QLookPartFind<Button>(container, "PClipSelector") is Button selector)
        {
            selector.Content = reading.QClipReadingAction;
            selector.IsEnabled = reading.QClipReadingReady;
            selector.Click -= QClipSelectorObserve;
            selector.Click += QClipSelectorObserve;
        }
    }

    private async void QClipPreviewObserve(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement { DataContext: QClipReading reading })
        {
            QClipPreviewRefine(
                await _cEditor.CEditorDesk.CDeskErrand.CErrandPreviewStart(reading.QClipReadingModel));
        }
    }

    private void QClipPreviewRefine(Uri? address)
    {
        if (address is null)
        {
            return;
        }

        _qClipPlayer.Open(address);
        _qClipPlayer.Play();
    }

    private void QClipEndObserve(object? sender, EventArgs e)
    {
        _cEditor.CEditorDesk.CDeskErrand.CErrandPreviewFinish();
    }

    private async void QClipSelectorObserve(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement { DataContext: QClipReading reading })
        {
            QClipCloseRefine(
                await _cEditor.CEditorDesk.CDeskErrand.CErrandRecordingSave(reading.QClipReadingModel));
        }
    }

    private void QClipCloseRefine(bool attached)
    {
        if (attached)
        {
            QClipPopup.IsOpen = false;
        }
    }
}
