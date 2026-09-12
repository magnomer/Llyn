namespace Llyn.Core;

public sealed record LCourt(
    long LCourtId,
    long LCourtOwnerId,
    long LCourtTargetId,
    string LCourtHeadword,
    string LCourtLanguage,
    int LCourtVersion = 0);
