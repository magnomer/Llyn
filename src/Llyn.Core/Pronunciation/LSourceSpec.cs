using System.Collections.Generic;

namespace Llyn.Core;

public sealed record LSourceSpec(
    string LSourceSpecName,
    IReadOnlyList<LSourceAttempt> LSourceSpecAttempts,
    IReadOnlyList<LRespellingRule>? LSourceSpecSpelling = null)
{
    public IReadOnlyList<LRespellingRule> LSourceSpecSpelling { get; init; } = LSourceSpecSpelling ?? [];
}
