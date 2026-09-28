using System;

namespace Llyn.Conduct;

public sealed record CUsage(
    long CUsageId,
    long CUsageEntry,
    string CUsageName,
    string CUsageEpithet,
    string CUsageLanguage,
    CStateValue CUsageTitle,
    bool CUsageQuoted,
    bool CUsageCollocated)
{
    public string CUsageOwnerKey =>
        CUsageCollocated ? "Display.CollocationSingle" : CUsageQuoted ? "Vita.Example" : "Display.MeaningSingle";

    public string? CUsageTitleKey => CUsageTitle.CStateValueUncertain ? "Display.Unknown" : null;

    public void CUsageOpen(Func<long, bool> exampleSeam, Func<long, bool> entrySeam)
    {
        ArgumentNullException.ThrowIfNull(exampleSeam);
        ArgumentNullException.ThrowIfNull(entrySeam);

        if (CUsageQuoted)
        {
            exampleSeam(CUsageId);
            return;
        }

        entrySeam(CUsageEntry);
    }
}
