namespace Llyn.Core;

public interface LReceiver
{
    void LReceiverSourceStart(string source, int order);

    void LReceiverCandidateAdd(LCandidate candidate);

    void LReceiverLookupFinish();
}
