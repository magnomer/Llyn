using System.Collections.Generic;

namespace Llyn.Conduct;

public sealed record CShelfRoll(
    IReadOnlyList<CCatalogReference> CShelfRollRows,
    bool CShelfRollEmpty,
    string CShelfRollTally);
