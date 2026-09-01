using System.Collections.Generic;

namespace Llyn.Core;

public sealed record LSourceSpec(
    string LSourceSpecName,
    string LSourceSpecKind,
    IReadOnlyList<LSourceAttempt> LSourceSpecAttempts);
