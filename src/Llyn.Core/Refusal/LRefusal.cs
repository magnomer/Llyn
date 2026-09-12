using System;

namespace Llyn.Core;

public sealed class LRefusal : Exception
{
    public const string LRefusalHeadword = "Refusal.HeadwordMissing";

    public const string LRefusalEntry = "Refusal.EntryMissing";

    public const string LRefusalTarget = "Refusal.TargetMissing";

    public const string LRefusalExample = "Refusal.ExampleMissing";

    public const string LRefusalSituation = "Refusal.SituationMissing";

    public const string LRefusalReference = "Refusal.ReferenceMissing";

    public const string LRefusalDraft = "Refusal.DraftMissing";

    public const string LRefusalStale = "Refusal.DraftStale";

    public const string LRefusalCollocation = "Refusal.CollocationNested";

    public const string LRefusalCard = "Refusal.CardMissing";

    public const string LRefusalItem = "Refusal.ItemMissing";

    public const string LRefusalLink = "Refusal.LinkMissing";

    public const string LRefusalScheme = "Refusal.SchemeDoubled";

    public LRefusal(string reason)
        : base(reason)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(reason);

        LRefusalReason = reason;
    }

    public string LRefusalReason { get; }
}
