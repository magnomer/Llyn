using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using Llyn.Core;

using Llyn.UIDeportment;

namespace Llyn.UIVeneer;

public partial class PEditor
{
    private readonly MediaPlayer _pDownloaderPlayer = new();

    private string? _pRecording;

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
        _pEditorHost.PWindowDeportment.LWindowVolumeSet(e.NewValue);
    }

    private void PVolumeSave(object sender, RoutedEventArgs e)
    {
        _pEditorHost.PWindowDeportment.LWindowVolumeSave();
    }

    internal void PVolumeAttach()
    {
        PVolume.ValueChanged += PVolumeHandle;
        PVolume.AddHandler(Thumb.DragCompletedEvent, new DragCompletedEventHandler(PVolumeSave));
        PVolume.AddHandler(MouseUpEvent, new MouseButtonEventHandler(PVolumeSave), true);
        PVolume.AddHandler(KeyUpEvent, new KeyEventHandler(PVolumeSave), true);
    }

    internal void PVolumeLoad()
    {
        PVolume.Value = _pEditorHost.PWindowDeportment.LWindowPostureRead().LPostureStateVolume;
        _pDownloaderPlayer.Volume = PVolume.Value;
    }

    private void PEditorRecordingShow(LEntryDraft draft)
    {
        if (string.Equals(_pRecording ?? string.Empty, draft.LEntryDraftAudio, StringComparison.Ordinal))
        {
            return;
        }

        PRecordingShow(draft.LEntryDraftAudio);
    }

    private void PRecordingShow(string audio)
    {
        if (!_pEditorHost.PWindowDeportment.LWindowRecordingExist(audio))
        {
            PRecordingClear();
            return;
        }

        PRecordingClear();
        _pRecording = audio;
        PPlaybackAction.Visibility = Visibility.Visible;
    }

    private void PRecordingClear()
    {
        _pRecording = null;
        _pDownloaderPlayer.Stop();
        PPlaybackAction.Visibility = Visibility.Collapsed;
        PPlaybackTrayShow();
    }

    private void PPlaybackTrayShow()
    {
        bool audible = _pRecording is not null
            || _pAccentItem.Any(static row => row.LAccentItemAudio.Length > 0);
        PPlayback.Visibility = audible ? Visibility.Visible : Visibility.Collapsed;
    }
}
