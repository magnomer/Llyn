using Llyn.Core;

namespace Llyn.UIShell;

/// <summary>
/// Presentation item for one pronunciation candidate shown in <c>PLookupMenu</c>. Wraps the domain
/// <see cref="LCandidate"/> with the localized source label the menu row displays.
/// </summary>
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
