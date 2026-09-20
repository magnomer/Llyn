using System;
using Llyn.Core;

namespace Llyn.Application;

public sealed class LListenerRelay : LListener
{
    private readonly Action<LHarvestStep> _lListenerRelaySink;

    public LListenerRelay(Action<LHarvestStep> sink)
    {
        _lListenerRelaySink = sink ?? throw new ArgumentNullException(nameof(sink));
    }

    public void LListenerSourceStart(string source, int order)
    {
        _lListenerRelaySink(new LHarvestStep(LHarvestKind.LHarvestKindSource, source, order, null));
    }

    public void LListenerRecordingAdd(LRecording recording)
    {
        ArgumentNullException.ThrowIfNull(recording);

        _lListenerRelaySink(new LHarvestStep(
            LHarvestKind.LHarvestKindRecording, recording.LRecordingSource, recording.LRecordingOrder, recording));
    }

    public void LListenerFinish()
    {
        _lListenerRelaySink(new LHarvestStep(LHarvestKind.LHarvestKindEnd, string.Empty, 0, null));
    }
}
