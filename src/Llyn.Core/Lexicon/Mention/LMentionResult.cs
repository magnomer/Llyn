using System.Collections.Generic;

namespace Llyn.Core;

public sealed record LMentionResult(
    int LMentionResultOffset,
    LMention? LMentionResultStored,
    IReadOnlyList<LTranslationTarget> LMentionResultEntry)
{
    public IReadOnlyList<LTranslationTarget> LMentionResultEntry { get; init; } = LMentionResultEntry ?? [];
}
