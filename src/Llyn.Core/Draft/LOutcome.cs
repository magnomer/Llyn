using System.Collections.Generic;

namespace Llyn.Core;

public sealed record LOutcome(
    LEntry LOutcomeEntry,
    IReadOnlyDictionary<long, long> LOutcomeIdentity);
