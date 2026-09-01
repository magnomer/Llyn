namespace Llyn.Core;

public interface LReceiver
{
    void LReceiverSourceStart(string source);

    void LReceiverCandidateAdd(LCandidate candidate);

    void LReceiverLookupFinish();
}
