using System;
using Llyn.Application;
using Llyn.Core;

namespace Llyn.ShellEngine;

public sealed class LQuillMention
{
    private readonly LTenure _lQuillMentionTenure;

    public LQuillMention(LTenure tenure)
    {
        ArgumentNullException.ThrowIfNull(tenure);

        _lQuillMentionTenure = tenure;
    }

    public void LQuillMentionRemove(long card, long sentence, long mention)
    {
        _lQuillMentionTenure.LTenureRequestApply(
            new LRequestMentionRemoval(_lQuillMentionTenure.LTenureId, card, sentence, mention));
    }

    public LMentionDraft? LQuillMentionFind(long cardId, long sentenceId, LMentionDraft span)
    {
        ArgumentNullException.ThrowIfNull(span);

        return LMentionClerk.LMentionSpanFind(
            _lQuillMentionTenure.LTenureHerald.LTenureKeptRead(), cardId, sentenceId, span);
    }

    public bool LQuillMentionCheck(long cardId, long sentenceId, string text, int start, int length)
    {
        return LQuillMentionFind(cardId, sentenceId, LMentionClerk.LMentionSpanRead(text, start, length)) is not null;
    }

    public bool LQuillSenseCheck(long cardId, long sentenceId, string text, int start, int length)
    {
        return LQuillMentionFind(cardId, sentenceId, LMentionClerk.LMentionSpanRead(text, start, length))
            is { LMentionDraftLinked: true };
    }

    public long? LQuillSenseRead(long cardId, long sentenceId, string text, int start, int length)
    {
        _lQuillMentionTenure.LTenurePersist();
        return LQuillMentionFind(cardId, sentenceId, LMentionClerk.LMentionSpanRead(text, start, length))
            is { LMentionDraftLinked: true } found
            ? found.LMentionDraftEntry
            : null;
    }
}
