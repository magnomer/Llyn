using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QPlayback
{
    private readonly FrameworkElement _qPlaybackSurface;

    private readonly MediaPlayer _qPlaybackPlayer;

    private CEditor _cEditor = null!;

    internal QPlayback(FrameworkElement surface, MediaPlayer player)
    {
        _qPlaybackSurface = surface;
        _qPlaybackPlayer = player;
        QPlaybackAction.Click += QPlaybackActionObserve;
        QPlaybackAction.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("play", 24));
    }

    private Button QPlaybackAction => QContract.QContractFind<Button>(_qPlaybackSurface, "PPlaybackAction");

    private Border QPlaybackTray => QContract.QContractFind<Border>(_qPlaybackSurface, "PPlayback");

    internal void QPlaybackIntroduce(CEditor editor)
    {
        _cEditor = editor;
        editor.CEditorEntry.CEntryDraftChanged += QPlaybackRefine;
    }

    private void QPlaybackRefine(CEntryDraft _)
    {
        QPlaybackAudioRefine(_cEditor.CEditorPlayback.CPlaybackRead());
    }

    private void QPlaybackActionObserve(object sender, RoutedEventArgs e)
    {
        QPlaybackActionRefine(_cEditor.CEditorPlayback.CPlaybackStart((sender as FrameworkElement)?.Tag as string));
    }

    private void QPlaybackActionRefine(Uri? address)
    {
        if (address is null)
        {
            QPlaybackAudioRefine(_cEditor.CEditorPlayback.CPlaybackRead());
            return;
        }

        _qPlaybackPlayer.Open(address);
        _qPlaybackPlayer.Play();
    }

    private void QPlaybackAudioRefine(CTimbrePlayback playback)
    {
        if (!Equals(QPlaybackAction.Tag, playback.CTimbrePlaybackAudio))
        {
            _qPlaybackPlayer.Stop();
            QPlaybackAction.Tag = playback.CTimbrePlaybackAudio;
            QPlaybackAction.Visibility = QLook.QLookVisibleRead(playback.CTimbrePlaybackAudio is not null);
        }

        QPlaybackTray.Visibility = QLook.QLookVisibleRead(playback.CTimbrePlaybackAudible);
    }
}
