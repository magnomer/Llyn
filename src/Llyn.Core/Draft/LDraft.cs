using System;

namespace Llyn.Core;

public sealed record LDraft(
    string LDraftId,
    string LDraftOrigin,
    string LDraftEntry,
    LEntryDraft LDraftContent,
    DateTimeOffset LDraftMoment);
