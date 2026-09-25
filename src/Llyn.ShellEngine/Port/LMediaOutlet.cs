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

    public void LEngineLocationOpen(string target) => _lMediaOutletEngine.LEngineWorkspace.LEngineLocationOpen(target);

    public bool LEngineRecordingExist(string? file) =>
        _lMediaOutletEngine.LEnginePronunciation.LEngineRecordingExist(file);

    public void LEngineRecordingPlay(string? file) =>
        _lMediaOutletEngine.LEnginePronunciation.LEngineRecordingPlay(file);

    public void LEngineRecordingStop() => _lMediaOutletEngine.LEnginePronunciation.LEngineRecordingStop();

    public void LEngineVolumeSet(double volume) => _lMediaOutletEngine.LEnginePronunciation.LEngineVolumeSet(volume);

    public Task<string> LEngineRecordingPrepare(LRecording recording, CancellationToken cancellation) =>
        _lMediaOutletEngine.LEnginePronunciation.LEngineRecordingPrepare(recording, cancellation);

    public void LEngineRecordingSweep() => _lMediaOutletEngine.LEnginePronunciation.LEngineRecordingSweep();
}
