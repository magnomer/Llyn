using System.Collections.Generic;

namespace Llyn.Conduct;

public sealed record CMentionResult(
    int CMentionResultOffset,
    CMentionMark? CMentionResultStored,
    IReadOnlyList<CTranslationTarget> CMentionResultEntry)
{
    public bool CMentionResultSingle => CMentionResultEntry.Count == 1;

    public bool CMentionResultMany => CMentionResultEntry.Count > 1;

    public long CMentionResultFirst => CMentionResultSingle ? CMentionResultEntry[0].CTranslationTargetId : 0;
}
