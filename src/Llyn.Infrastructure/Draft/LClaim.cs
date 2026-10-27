using System;

namespace Llyn.Infrastructure;

internal sealed record LClaim(
    long LClaimDraft,
    int LClaimProcess,
    DateTimeOffset LClaimMoment);
