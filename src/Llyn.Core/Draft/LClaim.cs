using System;

namespace Llyn.Core;

public sealed record LClaim(
    string LClaimDraft,
    int LClaimProcess,
    DateTimeOffset LClaimMoment);
