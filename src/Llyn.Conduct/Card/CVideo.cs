using System;

namespace Llyn.Conduct;

public sealed class CVideo
{
    private readonly CDesk _cVideoDesk;

    internal CVideo(CDesk desk)
    {
        ArgumentNullException.ThrowIfNull(desk);

        _cVideoDesk = desk;
    }

    public void CVideoLocationSet(long videoId, string location)
    {
        _cVideoDesk.CDeskEasel?.LEaselVideoSet(videoId, location, true);
    }

    public void CVideoSpanSet(long videoId, string span)
    {
        _cVideoDesk.CDeskEasel?.LEaselSpanSet(videoId, span);
    }
}
