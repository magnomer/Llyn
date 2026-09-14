using System.Collections.Generic;

namespace Llyn.Core;

public sealed record LMarkupOutcome(
    IReadOnlyList<LEntry> LMarkupOutcomeEntry,
    IReadOnlyList<LMarkupOmission> LMarkupOutcomeOmission)
{
    public IReadOnlyList<LEntry> LMarkupOutcomeEntry { get; init; } = LMarkupOutcomeEntry ?? [];

    public IReadOnlyList<LMarkupOmission> LMarkupOutcomeOmission { get; init; } = LMarkupOutcomeOmission ?? [];
}
