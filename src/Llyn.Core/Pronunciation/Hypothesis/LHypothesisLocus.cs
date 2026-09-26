using System.Collections.Generic;

namespace Llyn.Core;

public sealed record LHypothesisLocus(
    string LHypothesisLocusName,
    IReadOnlyList<string> LHypothesisLocusInitials)
{
    public string LHypothesisLocusName { get; init; } = LHypothesisLocusName ?? string.Empty;

    public IReadOnlyList<string> LHypothesisLocusInitials { get; init; } = LHypothesisLocusInitials ?? [];
}
