using System;
using System.Threading;
using System.Threading.Tasks;
using Llyn.Core;

namespace Llyn.ShellEngine;

public interface LMediaPort
{
    Uri? LEngineLocationRead(string? location);

    void LEngineLocationOpen(string target);

    bool LEngineRecordingExist(string? file);

    void LEngineRecordingPlay(string? file);

    void LEngineRecordingStop();

    void LEngineVolumeSet(double volume);

    Task<string> LEngineRecordingPrepare(LRecording recording, CancellationToken cancellation);

    void LEngineRecordingSweep();
}
