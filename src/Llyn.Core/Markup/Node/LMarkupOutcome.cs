using System.Collections.Generic;

namespace Llyn.Core;

public sealed record LMarkupOutcome(
    IReadOnlyList<LMarkupOmission> LMarkupOutcomeOmission)
{
    public IReadOnlyList<LMarkupOmission> LMarkupOutcomeOmission { get; init; } = LMarkupOutcomeOmission ?? [];
}
