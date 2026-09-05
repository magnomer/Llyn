using System;

namespace Llyn.Core;

public sealed class LRefusal : Exception
{
    public const string LRefusalHeadword = "Refusal.HeadwordMissing";

    public const string LRefusalEntry = "Refusal.EntryMissing";

    public const string LRefusalTarget = "Refusal.TargetMissing";

    public const string LRefusalDraft = "Refusal.DraftMissing";

    public LRefusal(string reason)
        : base(reason)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(reason);

        LRefusalReason = reason;
    }

    public string LRefusalReason { get; }
}
