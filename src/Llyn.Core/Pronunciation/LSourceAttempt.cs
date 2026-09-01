using System.Collections.Generic;

namespace Llyn.Core;

public sealed record LSourceAttempt(
    IReadOnlyList<string> LSourceAttemptUrls,
    string LSourceAttemptStrategy,
    string? LSourceAttemptPattern,
    int LSourceAttemptGroup,
    string? LSourceAttemptPath,
    string? LSourceAttemptGuard,
    bool LSourceAttemptPhonetic,
    IReadOnlyDictionary<string, string>? LSourceAttemptHeaders,
    string? LSourceAttemptPrefix);
