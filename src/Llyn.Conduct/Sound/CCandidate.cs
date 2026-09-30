namespace Llyn.Conduct;

public sealed record CCandidate(
    string CCandidateSource,
    string? CCandidatePhonetic,
    int CCandidateOrder,
    bool CCandidateReached,
    string CCandidateVariety,
    string? CCandidateRespelling)
{
    public bool CCandidateRegional => CCandidateVariety.Length > 0;

    public bool CCandidateNotated => !string.IsNullOrEmpty(CCandidatePhonetic);
}
