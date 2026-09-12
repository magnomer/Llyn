using System;
using System.Collections.Generic;

namespace Llyn.Core;

public sealed record LDraft(
    long LDraftId,
    string LDraftOrigin,
    long LDraftEntryId,
    LEntryDraft LDraftContent,
    DateTimeOffset LDraftMoment,
    LExample? LDraftExample = null,
    LSituation? LDraftSituation = null,
    LReference? LDraftReference = null,
    int LDraftVersion = 0,
    IReadOnlyList<LAuthor>? LDraftAuthor = null)
{
    public IReadOnlyList<LAuthor> LDraftAuthor { get; init; } = LDraftAuthor ?? [];
}
