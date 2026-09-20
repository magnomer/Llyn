using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using Llyn.Core;

namespace Llyn.UIVeneer;

public partial class PEditor
{
    private readonly MediaPlayer _pDownloaderPlayer = new();

    private string? _pRecording;
    private string? _pRecordingSource;

    private void PPlaybackActionHandle(object sender, RoutedEventArgs e)
    {
        if (_pRecording is null || !_pEditorHost.PWindowDeportment.LWindowRecordingExist(_pRecording))
        {
            PRecordingClear();
            return;
        }

        _pDownloaderPlayer.Open(new Uri(_pRecording));
        _pDownloaderPlayer.Play();
    }

    private void PVolumeHandle(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        _pDownloaderPlayer.Volume = e.NewValue;
        PVolumeCatalog.PVolumeCatalogCurrent.PVolumeCatalogLevel = e.NewValue;
    }

    private void PVolumeSave(object sender, RoutedEventArgs e)
    {
        if (_pEditorHost.PWindowPosture.LPostureVolumeMatch(PVolume.Value))
        {
            return;
        }

        _pEditorHost.PWindowPosture.LPostureVolumeSave(PVolume.Value);
    }

    internal void PVolumeAttach()
    {
        PVolume.AddHandler(Thumb.DragCompletedEvent, new DragCompletedEventHandler(PVolumeSave));
        PVolume.AddHandler(MouseUpEvent, new MouseButtonEventHandler(PVolumeSave), true);
        PVolume.AddHandler(KeyUpEvent, new KeyEventHandler(PVolumeSave), true);
    }

    internal void PVolumeLoad()
    {
        PVolume.Value = _pEditorHost.PWindowPosture.LPostureRead().LPostureStateVolume;
        PVolumeCatalog.PVolumeCatalogCurrent.PVolumeCatalogLevel = PVolume.Value;
    }

    private void PEditorRecordingShow(LEntryDraft draft)
    {
        if (string.Equals(_pRecording ?? string.Empty, draft.LEntryDraftAudio, StringComparison.Ordinal))
        {
            return;
        }

        PRecordingShow(draft.LEntryDraftAudio, draft.LEntryDraftPronunciation?.LPronunciationDraftSource);
    }

    private void PRecordingShow(string audio, string? source)
    {
        if (!_pEditorHost.PWindowDeportment.LWindowRecordingExist(audio))
        {
            PRecordingClear();
            return;
        }

        PRecordingClear();
        _pRecording = audio;
        _pRecordingSource = source;
        PPlaybackAction.Visibility = Visibility.Visible;
    }

    private void PRecordingClear()
    {
        _pRecording = null;
        _pRecordingSource = null;
        _pDownloaderPlayer.Stop();
        PPlaybackAction.Visibility = Visibility.Collapsed;
        PPlaybackTrayShow();
    }

    private void PPlaybackTrayShow()
    {
        bool audible = _pRecording is not null
            || _pAccentItem.Any(static row => row.PAccentItemAudio.Length > 0);
        PPlayback.Visibility = audible ? Visibility.Visible : Visibility.Collapsed;
    }
}
