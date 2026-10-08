using System;
using Llyn.ShellEngine;

namespace Llyn.Conduct;

public sealed class CPlayback
{
    private readonly CDesk _cPlaybackDesk;

    private readonly LMediaPort _cPlaybackMediaPort;

    internal CPlayback(CDesk desk, LMediaPort media)
    {
        ArgumentNullException.ThrowIfNull(desk);
        ArgumentNullException.ThrowIfNull(media);

        _cPlaybackDesk = desk;
        _cPlaybackMediaPort = media;
    }

    public CTimbrePlayback CPlaybackRead()
    {
        if (_cPlaybackDesk.CDeskTenure?.LTenureRead() is not { } held)
        {
            return new CTimbrePlayback(null, false);
        }

        (string? audio, bool audible) = _cPlaybackMediaPort.LEngineAudioRead(held.LDraftContent);
        return new CTimbrePlayback(audio, audible);
    }

    public Uri? CPlaybackStart(string? audio)
    {
        return _cPlaybackMediaPort.LEngineAudioResolve(audio);
    }

    public Uri? CPlaybackAccentStart(long accent)
    {
        return !_cPlaybackDesk.CDeskFilling && _cPlaybackDesk.CDeskTenure is LTenure held
            ? new LQuillPronunciation(held).LQuillAudioResolve(accent)
            : null;
    }
}
