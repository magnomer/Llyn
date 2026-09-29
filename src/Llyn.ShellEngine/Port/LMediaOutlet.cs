using System;
using System.Threading;
using System.Threading.Tasks;
using Llyn.Core;

namespace Llyn.ShellEngine;

public sealed class LMediaOutlet : LMediaPort
{
    private readonly LEngine _lMediaOutletEngine;

    public LMediaOutlet(LEngine engine)
    {
        ArgumentNullException.ThrowIfNull(engine);
        _lMediaOutletEngine = engine;
    }

    public Uri? LEngineLocationRead(string? location) =>
        _lMediaOutletEngine.LEngineWorkspace.LEngineLocationRead(location);

    public (Uri, string?)? LEngineScreenRead(string? location) =>
        _lMediaOutletEngine.LEngineWorkspace.LEngineScreenRead(location);

    public void LEngineLocationOpen(string target) => _lMediaOutletEngine.LEngineWorkspace.LEngineLocationOpen(target);

    public bool LEngineRecordingExist(string? file) =>
        _lMediaOutletEngine.LEnginePronunciation.LEngineRecordingExist(file);

    public int LEngineRecordingPlay(string? file, double volume) =>
        _lMediaOutletEngine.LEnginePronunciation.LEngineRecordingPlay(file, volume);

    public int LEngineRecordingPlay(LEntryDraft draft, double volume) =>
        _lMediaOutletEngine.LEnginePronunciation.LEngineRecordingPlay(draft, volume);

    public (bool, bool) LEnginePlaybackRead(LEntryDraft draft) =>
        _lMediaOutletEngine.LEnginePronunciation.LEnginePlaybackRead(draft);

    public void LEngineRecordingStop(int ticket) =>
        _lMediaOutletEngine.LEnginePronunciation.LEngineRecordingStop(ticket);

    public void LEngineVolumeSet(double volume) => _lMediaOutletEngine.LEnginePronunciation.LEngineVolumeSet(volume);

    public Task<string> LEngineRecordingPrepare(LRecording recording, CancellationToken cancellation) =>
        _lMediaOutletEngine.LEnginePronunciation.LEngineRecordingPrepare(recording, cancellation);
}
