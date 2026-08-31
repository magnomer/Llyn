namespace Llyn.Core;

/// <summary>
/// Subscriber contract for a pronunciation lookup. The requester implements this to receive
/// streamed results: each source reports when it starts, every candidate is delivered as it
/// arrives, and the lookup reports once when all sources have finished. Callbacks may arrive on
/// background threads; an implementation that touches UI is responsible for marshalling.
/// </summary>
public interface IPronunciationReceiver
{
    /// <summary>A source has begun searching.</summary>
    void SourceStart(LookupSource source);

    /// <summary>A candidate has arrived from a source.</summary>
    void CandidateAdd(PronunciationCandidate candidate);

    /// <summary>Every source has finished; no further callbacks follow.</summary>
    void LookupStop();
}
