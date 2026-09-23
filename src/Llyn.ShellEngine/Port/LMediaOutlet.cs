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

    public Uri? LEngineLocationRead(string? location) => _lMediaOutletEngine.LEngineLocationRead(location);

    public void LEngineLocationOpen(string target) => _lMediaOutletEngine.LEngineLocationOpen(target);

    public bool LEngineRecordingExist(string? file) => _lMediaOutletEngine.LEngineRecordingExist(file);

    public Task<string> LEngineRecordingPrepare(LRecording recording, CancellationToken cancellation) =>
        _lMediaOutletEngine.LEngineRecordingPrepare(recording, cancellation);

    public void LEngineRecordingSweep() => _lMediaOutletEngine.LEngineRecordingSweep();
}
