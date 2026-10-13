using System.Collections.Generic;

namespace Llyn.Conduct;

public sealed record CVita(
    string CVitaName,
    bool CVitaNamed,
    string CVitaWork,
    string CVitaTally,
    IReadOnlyList<CFellow> CVitaFellows,
    IReadOnlyList<CUsage> CVitaUsages)
{
    public bool CVitaFellowShown => CVitaFellows.Count > 0;

    public bool CVitaUsageShown => CVitaUsages.Count > 0;
}
