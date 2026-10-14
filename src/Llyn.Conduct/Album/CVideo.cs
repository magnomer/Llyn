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
        _cVideoDesk.CDeskDraft.CDeskDraftEasel?.LEaselVideoSet(videoId, location, true);
    }

    public void CVideoSpanSet(long videoId, string span)
    {
        _cVideoDesk.CDeskDraft.CDeskDraftEasel?.LEaselSpanSet(videoId, span);
    }

    public void CVideoAdd(long? cardId)
    {
        _cVideoDesk.CDeskDraft.CDeskDraftEasel?.LEaselVideoAdd(cardId ?? 0);
    }

    public void CVideoRemove(long videoId)
    {
        _cVideoDesk.CDeskDraft.CDeskDraftEasel?.LEaselVideoRemove(videoId);
    }

    public void CVideoFileSet(long videoId, string? file)
    {
        if (file is null)
        {
            return;
        }

        _cVideoDesk.CDeskDraft.CDeskDraftEasel?.LEaselVideoSet(videoId, file, false);
    }
}
