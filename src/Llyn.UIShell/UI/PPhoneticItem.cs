using Llyn.Core;

namespace Llyn.UIShell;

internal sealed class PPhoneticItem
{
    internal PPhoneticItem(LCandidate candidate, string sourceLabel)
    {
        PPhoneticItemSource = sourceLabel;
        PPhoneticItemReading = candidate.LCandidatePhonetic;
    }

    public string PPhoneticItemSource { get; }

    public string PPhoneticItemReading { get; }
}
