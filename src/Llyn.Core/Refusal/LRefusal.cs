using System;

namespace Llyn.Core;

/// <summary>
/// A deliberate refusal the shell is meant to present: a request the logic declines for a reason the
/// user can act on, such as a save without a headword.
/// </summary>
/// <remarks>
/// The refusal names its reason with a key, never with a sentence. Display text belongs to the
/// localization catalog the shell owns, so the logic layers stay free of user-facing wording and a
/// refusal reads in whichever interface language is selected. Only deliberate refusals carry a key;
/// an unexpected failure stays an ordinary exception and travels with its own message.
/// </remarks>
public sealed class LRefusal : Exception
{
    /// <summary>Reason key for a save whose entry carries no headword.</summary>
    public const string LRefusalHeadword = "Refusal.HeadwordMissing";

    /// <summary>Refuses a request for <paramref name="reason"/>, a localization key.</summary>
    public LRefusal(string reason)
        : base(reason)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(reason);

        LRefusalReason = reason;
    }

    /// <summary>The localization key naming why the request was refused.</summary>
    public string LRefusalReason { get; }
}
