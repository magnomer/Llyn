using System;
using System.Windows.Media;
using Llyn.Core;

namespace Llyn.Core.Windows;

public sealed class LPhonographMedia : LPhonograph
{
    private readonly MediaPlayer _lPhonographPlayer = new();

    public void LPhonographPlay(string file)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(file);

        _lPhonographPlayer.Open(new Uri(file));
        _lPhonographPlayer.Play();
    }

    public void LPhonographStop()
    {
        _lPhonographPlayer.Stop();
    }

    public void LPhonographVolumeSet(double volume)
    {
        _lPhonographPlayer.Volume = volume;
    }
}
