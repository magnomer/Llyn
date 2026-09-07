using System.Collections.Generic;

namespace Llyn.Core;

public sealed record LSourceSpec(
    string LSourceSpecName,
    IReadOnlyList<LSourceAttempt> LSourceSpecAttempts);
