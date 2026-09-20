namespace Llyn.Core;

public sealed record LCandidate(
    string LCandidateSource,
    string? LCandidatePhonetic,
    int LCandidateOrder,
    bool LCandidateReached,
    string LCandidateVariety,
    string? LCandidateRespelling = null)
{
    public bool LCandidateRegional => LCandidateVariety.Length > 0;
}
