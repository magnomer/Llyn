using System;

namespace Llyn.Core;

public sealed record LClaim(
    long LClaimDraft,
    int LClaimProcess,
    DateTimeOffset LClaimMoment);
