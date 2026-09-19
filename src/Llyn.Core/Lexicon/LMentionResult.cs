using System.Collections.Generic;

namespace Llyn.Core;

public sealed record LMentionResult(
    int LMentionResultOffset,
    int LMentionResultLength,
    LMention? LMentionResultStored,
    IReadOnlyList<LTranslationTarget> LMentionResultEntry)
{
    public IReadOnlyList<LTranslationTarget> LMentionResultEntry { get; init; } = LMentionResultEntry ?? [];

    public bool LMentionResultSingle => LMentionResultEntry.Count == 1;

    public bool LMentionResultMany => LMentionResultEntry.Count > 1;

    public long LMentionResultFirst => LMentionResultSingle ? LMentionResultEntry[0].LTranslationTargetId : 0;
}
