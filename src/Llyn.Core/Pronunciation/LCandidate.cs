namespace Llyn.Core;

public sealed record LCandidate(
    string LCandidateSource,
    string? LCandidatePhonetic,
    int LCandidateOrder,
    bool LCandidateReached);
