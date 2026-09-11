using System;

namespace Llyn.Core;

public sealed record LDraft(
    long LDraftId,
    string LDraftOrigin,
    long LDraftEntry,
    LEntryDraft LDraftContent,
    DateTimeOffset LDraftMoment,
    LExample? LDraftExample = null,
    LSituation? LDraftSituation = null,
    LReference? LDraftReference = null);
