using Llyn.Core;

namespace Llyn.Tests;

internal sealed class TReceiverStub : LReceiver
{
    private readonly List<string> _tReceiverStubSources = [];
    private readonly List<LCandidate> _tReceiverStubCandidates = [];
    private int _tReceiverStubFinished;

    internal IReadOnlyList<string> TReceiverStubSources => _tReceiverStubSources;

    internal IReadOnlyList<LCandidate> TReceiverStubCandidates => _tReceiverStubCandidates;

    internal int TReceiverStubFinished => _tReceiverStubFinished;

    public void LReceiverSourceStart(string source, int order)
    {
        lock (_tReceiverStubSources)
        {
            _tReceiverStubSources.Add(source);
        }
    }

    public void LReceiverCandidateAdd(LCandidate candidate)
    {
        lock (_tReceiverStubCandidates)
        {
            _tReceiverStubCandidates.Add(candidate);
        }
    }

    public void LReceiverLookupFinish()
    {
        Interlocked.Increment(ref _tReceiverStubFinished);
    }

    internal void TReceiverStubHandle(LLookupStep step)
    {
        switch (step.LLookupStepKind)
        {
            case LLookupKind.LLookupKindSource:
                LReceiverSourceStart(step.LLookupStepSource, step.LLookupStepOrder);
                break;
            case LLookupKind.LLookupKindCandidate when step.LLookupStepCandidate is LCandidate candidate:
                LReceiverCandidateAdd(candidate);
                break;
            case LLookupKind.LLookupKindEnd:
                LReceiverLookupFinish();
                break;
        }
    }
}
