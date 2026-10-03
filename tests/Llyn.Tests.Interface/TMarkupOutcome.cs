using Llyn.Core;

namespace Llyn.Tests;

internal sealed record TMarkupOutcome(
    IReadOnlyList<LEntry> TMarkupOutcomeEntry,
    IReadOnlyList<LMarkupOmission> TMarkupOutcomeOmission);
