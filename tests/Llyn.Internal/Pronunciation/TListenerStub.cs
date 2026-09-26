using Llyn.Application;
using Llyn.Core;

namespace Llyn.Tests;

internal sealed class TListenerStub : LListener
{
    private readonly List<string> _tListenerStubSources = [];
    private readonly List<LRecording> _tListenerStubRecordings = [];
    private int _tListenerStubFinished;

    internal IReadOnlyList<string> TListenerStubSources => _tListenerStubSources;

    internal IReadOnlyList<LRecording> TListenerStubRecordings => _tListenerStubRecordings;

    internal int TListenerStubFinished => _tListenerStubFinished;

    public void LListenerSourceStart(string source, int order)
    {
        lock (_tListenerStubSources)
        {
            _tListenerStubSources.Add(source);
        }
    }

    public void LListenerRecordingAdd(LRecording recording)
    {
        lock (_tListenerStubRecordings)
        {
            _tListenerStubRecordings.Add(recording);
        }
    }

    public void LListenerFinish()
    {
        Interlocked.Increment(ref _tListenerStubFinished);
    }

    internal void TListenerStubHandle(LHarvestStep step)
    {
        switch (step.LHarvestStepKind)
        {
            case LHarvestKind.LHarvestKindSource:
                LListenerSourceStart(step.LHarvestStepSource, step.LHarvestStepOrder);
                break;
            case LHarvestKind.LHarvestKindRecording when step.LHarvestStepRecording is LRecording recording:
                LListenerRecordingAdd(recording);
                break;
            case LHarvestKind.LHarvestKindEnd:
                LListenerFinish();
                break;
        }
    }
}
