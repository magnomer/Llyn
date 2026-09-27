namespace Llyn.Conduct;

public sealed record CUsage(
    long CUsageId,
    long CUsageEntry,
    string CUsageName,
    string CUsageEpithet,
    string CUsageLanguage,
    CStateValue CUsageTitle,
    bool CUsageQuoted,
    bool CUsageCollocated);
