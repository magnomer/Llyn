using System.Collections.Generic;

namespace Llyn.Conduct;

public sealed record CMarker(
    IReadOnlyList<string> CMarkerSpeeches,
    string CMarkerTyped,
    CCategory CMarkerCategory,
    IReadOnlyList<(string, bool)> CMarkerUnits);
