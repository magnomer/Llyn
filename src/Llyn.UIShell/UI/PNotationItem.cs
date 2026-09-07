using Llyn.Core;

namespace Llyn.UIShell;

internal sealed class PNotationItem
{
    internal PNotationItem(LCandidate candidate, string sourceLabel)
    {
        PNotationItemSource = sourceLabel;
        PNotationItemReading = candidate.LCandidatePhonetic;
        PNotationItemOrder = candidate.LCandidateOrder;
    }

    public string PNotationItemSource { get; }

    public string PNotationItemReading { get; }

    internal int PNotationItemOrder { get; }
}
