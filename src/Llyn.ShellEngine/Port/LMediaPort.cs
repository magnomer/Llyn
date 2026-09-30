using System;
using Llyn.Application;
using Llyn.Core;

namespace Llyn.ShellEngine;

public interface LMediaPort
{
    Uri? LEngineLocationRead(string? location);

    (Uri, string?)? LEngineScreenRead(string? location);

    void LEngineLocationOpen(string target);

    int LEngineRecordingPlay(string? file, double volume);

    int LEngineRecordingPlay(LEntryDraft draft, double volume);

    (bool, bool) LEnginePlaybackRead(LEntryDraft draft);

    (string?, bool) LEngineAudioRead(LEntryDraft draft);

    Uri? LEngineAudioResolve(string? file);

    void LEngineRecordingStop(int ticket);

    void LEngineVolumeSet(double volume);

    static (TimeSpan, TimeSpan?) LEngineSpanRead(LVideoDraft video)
    {
        return LDraftClerkMedia.LVideoSpanRead(video);
    }
}
