using System.Collections.Generic;

namespace Llyn.Conduct;

public sealed record CGuildRoll(
    IReadOnlyList<CCatalogAuthor> CGuildRollRows,
    bool CGuildRollEmpty,
    CVita CGuildRollVita,
    IReadOnlyList<CReferenceKind> CGuildRollKind);
