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
    IReadOnlyList<LAuthor>? LDraftAuthor = null,
    LAuthor? LDraftAuthorHeld = null)
{
    public long? LDraftStored => LDraftEntryId == 0 ? null : LDraftEntryId;

    public LTag? LDraftTag { get; init; }

    public LRegister? LDraftRegister { get; init; }

    public IReadOnlyList<LAuthor> LDraftAuthor { get; init; } = LDraftAuthor ?? [];

    public LDraft LDraftNormalize()
    {
        return this with
        {
            LDraftContent = LDraftContent.LEntryDraftNormalize(),
            LDraftExample = LDraftExample?.LExampleNormalize(),
            LDraftSituation = LDraftSituation?.LSituationNormalize(),
            LDraftReference = LDraftReference?.LReferenceNormalize(),
        };
    }
}
