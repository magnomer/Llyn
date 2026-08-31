namespace Llyn.Core;

/// <summary>
/// Subscriber contract for a pronunciation lookup. The requester implements this to receive
/// streamed results: each source reports when it starts, every candidate is delivered as it
/// arrives, and the lookup reports once when all sources have finished. Callbacks may arrive on
/// background threads; an implementation that touches UI is responsible for marshalling.
/// </summary>
public interface LReceiver
{
    /// <summary>A source has begun searching. <paramref name="source"/> is the source's name.</summary>
    void LReceiverSourceStart(string source);

    /// <summary>A candidate has arrived from a source.</summary>
    void LReceiverCandidateAdd(LCandidate candidate);

    /// <summary>Every source has finished; no further callbacks follow.</summary>
    void LReceiverLookupFinish();
}
