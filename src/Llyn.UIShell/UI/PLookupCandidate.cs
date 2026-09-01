using Llyn.Core;

namespace Llyn.UIShell;

internal sealed class PLookupCandidate
{
    internal PLookupCandidate(LCandidate candidate, string sourceLabel)
    {
        PLookupCandidateSource = sourceLabel;
        PLookupCandidatePhonetic = candidate.LCandidatePhonetic;
    }

    public string PLookupCandidateSource { get; }

    public string PLookupCandidatePhonetic { get; }
}
