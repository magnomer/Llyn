using System;
using System.Collections.Generic;
using Llyn.Core;

namespace Llyn.Conduct;

public sealed class CVideo
{
    private static readonly IReadOnlyList<string> CVideoSuffix =
        [".mp4", ".m4v", ".mov", ".avi", ".wmv", ".mkv", ".webm"];

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

    public void CVideoAdd(bool collocated, long? cardId)
    {
        _cVideoDesk.CDeskDraft.CDeskDraftEasel?.LEaselVideoAdd(
            collocated ? LCardKind.LCardKindCollocation : LCardKind.LCardKindMeaning, cardId ?? 0);
    }

    public void CVideoRemove(bool collocated, long? cardId, long videoId)
    {
        _cVideoDesk.CDeskDraft.CDeskDraftEasel?.LEaselVideoRemove(
            collocated ? LCardKind.LCardKindCollocation : LCardKind.LCardKindMeaning, cardId ?? 0, videoId);
    }

    public void CVideoFileSet(long videoId, string? file)
    {
        if (file is null)
        {
            return;
        }

        _cVideoDesk.CDeskDraft.CDeskDraftEasel?.LEaselVideoSet(videoId, file, false);
    }

    public void CVideoFileOpen(long videoId)
    {
        CVideoFileSet(videoId, _cVideoDesk.CDeskEnvoy.CEnvoyMediaRead("Card.VideoChoose", CVideoSuffix));
    }
}
