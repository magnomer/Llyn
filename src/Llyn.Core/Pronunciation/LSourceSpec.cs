using System.Collections.Generic;

namespace Llyn.Core;

public sealed record LSourceSpec(
    string LSourceSpecName,
    IReadOnlyList<LSourceAttempt> LSourceSpecAttempts,
    IReadOnlyList<LRespellingRule>? LSourceSpecSpelling = null,
    IReadOnlyList<LBand>? LSourceSpecBands = null,
    double? LSourceSpecTotal = null,
    double? LSourceSpecFactor = null,
    double? LSourceSpecBase = null,
    string? LSourceSpecUnit = null)
{
    public IReadOnlyList<LRespellingRule> LSourceSpecSpelling { get; init; } = LSourceSpecSpelling ?? [];

    public IReadOnlyList<LBand> LSourceSpecBands { get; init; } = LSourceSpecBands ?? [];
}
