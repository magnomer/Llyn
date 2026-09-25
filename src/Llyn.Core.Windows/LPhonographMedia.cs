using System;
using System.Windows.Media;
using Llyn.Core;

namespace Llyn.Core.Windows;

public sealed class LPhonographMedia : LPhonograph
{
    private readonly MediaPlayer _lPhonographPlayer = new();

    public LPhonographMedia()
    {
        _lPhonographPlayer.MediaFailed += LPhonographFailHandle;
    }

    public void LPhonographPlay(string file)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(file);

        if (!_lPhonographPlayer.CheckAccess())
        {
            _lPhonographPlayer.Dispatcher.Invoke(() => LPhonographPlay(file));
            return;
        }

        _lPhonographPlayer.Open(new Uri(file));
        _lPhonographPlayer.Play();
    }

    public void LPhonographStop()
    {
        if (!_lPhonographPlayer.CheckAccess())
        {
            _lPhonographPlayer.Dispatcher.Invoke(LPhonographStop);
            return;
        }

        _lPhonographPlayer.Stop();
        _lPhonographPlayer.Close();
    }

    public void LPhonographVolumeSet(double volume)
    {
        if (!_lPhonographPlayer.CheckAccess())
        {
            _lPhonographPlayer.Dispatcher.Invoke(() => LPhonographVolumeSet(volume));
            return;
        }

        _lPhonographPlayer.Volume = volume;
    }

    private void LPhonographFailHandle(object? sender, ExceptionEventArgs e)
    {
        _lPhonographPlayer.Close();
    }
}
