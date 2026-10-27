using System.Collections.Generic;

namespace Llyn.Core;

public sealed record LDraftRelease(
    IReadOnlyList<long> LDraftReleaseDrafts,
    IReadOnlyList<long> LDraftReleaseOwners);
