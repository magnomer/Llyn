using System;
using System.Threading;
using Llyn.Core;
using Windows.Media.Core;
using Windows.Media.Playback;

namespace Llyn.Core.Windows;

public sealed class LPhonographMedia : LPhonograph
{
    private readonly MediaPlayer _lPhonographPlayer = new();
    private volatile SynchronizationContext? _lPhonographContext = SynchronizationContext.Current;

    public LPhonographMedia()
    {
        _lPhonographPlayer.CommandManager.IsEnabled = false;
        _lPhonographPlayer.Volume = 0.5;
        _lPhonographPlayer.MediaFailed += LPhonographFailHandle;
        _lPhonographPlayer.MediaEnded += LPhonographFinishHandle;
    }

    public event Action? LPhonographFinished;

    public void LPhonographPlay(string file)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(file);

        Uri address = new(file);
        _lPhonographContext = SynchronizationContext.Current ?? _lPhonographContext;
        IMediaPlaybackSource? previous = _lPhonographPlayer.Source;
        _lPhonographPlayer.Source = MediaSource.CreateFromUri(address);
        (previous as IDisposable)?.Dispose();
        _lPhonographPlayer.Play();
    }

    public void LPhonographStop()
    {
        IMediaPlaybackSource? previous = _lPhonographPlayer.Source;
        _lPhonographPlayer.Pause();
        _lPhonographPlayer.Source = null;
        (previous as IDisposable)?.Dispose();
    }

    public void LPhonographVolumeSet(double volume)
    {
        _lPhonographPlayer.Volume = volume;
    }

    private void LPhonographFailHandle(MediaPlayer sender, MediaPlayerFailedEventArgs args)
    {
        LPhonographDispatch(() =>
        {
            LPhonographStop();
            LPhonographFinished?.Invoke();
        });
    }

    private void LPhonographFinishHandle(MediaPlayer sender, object args)
    {
        LPhonographDispatch(() => LPhonographFinished?.Invoke());
    }

    private void LPhonographDispatch(Action step)
    {
        SynchronizationContext? context = _lPhonographContext;
        if (context is null)
        {
            step();
            return;
        }

        context.Post(_ => step(), null);
    }
}
