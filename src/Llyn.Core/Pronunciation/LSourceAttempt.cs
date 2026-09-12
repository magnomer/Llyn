using System.Collections.Generic;

namespace Llyn.Core;

public sealed record LSourceAttempt(
    IReadOnlyList<string> LSourceAttemptUrls,
    IReadOnlyList<LSourceReading> LSourceAttemptReadings,
    string? LSourceAttemptGuard,
    IReadOnlyDictionary<string, string>? LSourceAttemptHeaders,
    string? LSourceAttemptPrefix);
