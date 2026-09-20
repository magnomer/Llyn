using System;
using System.Threading;
using System.Threading.Tasks;
using Llyn.Core;

namespace Llyn.ShellEngine;

public interface LMediaPort
{
    Uri? LEngineLocationRead(string? location);

    bool LEngineRecordingExist(string? file);

    Task<string> LEngineRecordingPrepare(LRecording recording, CancellationToken cancellation);

    void LEngineRecordingSweep();
}
