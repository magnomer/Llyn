using System.Collections.Generic;

namespace Llyn.Conduct;

public sealed record CArticulation(
    IReadOnlyList<string> CArticulationHeaders,
    IReadOnlyList<string> CArticulationSides,
    IReadOnlyList<IReadOnlyList<IReadOnlyList<string>>> CArticulationCells);
