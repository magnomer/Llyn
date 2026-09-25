using System.Collections.Generic;
using System.Linq;

namespace Llyn.Core;

public sealed record LMentionDraft(
    long LMentionDraftId,
    int LMentionDraftOffset,
    int LMentionDraftLength,
    long LMentionDraftEntry,
    long LMentionDraftSense = 0)
{
    public static LMentionDraft LMentionDraftCreate(LMention mention)
    {
        return new LMentionDraft(
            mention.LMentionId,
            mention.LMentionOffset,
            mention.LMentionLength,
            mention.LMentionEntryId,
            mention.LMentionSenseId);
    }

    public bool LMentionDraftLinked => LMentionDraftEntry != 0;

    public LMention LMentionDraftResolve()
    {
        return new LMention(
            LMentionDraftId, LMentionDraftOffset, LMentionDraftLength, LMentionDraftEntry, LMentionDraftSense);
    }

    public static IReadOnlyList<LMentionDraft> LMentionDraftSort(IReadOnlyList<LMentionDraft> mentions)
    {
        return mentions.OrderBy(static mention => mention.LMentionDraftOffset).ToList();
    }
}
