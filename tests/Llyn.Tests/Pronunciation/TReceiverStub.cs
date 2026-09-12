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
}
