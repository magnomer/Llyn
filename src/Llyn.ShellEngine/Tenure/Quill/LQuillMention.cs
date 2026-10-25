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

    public void LQuillMentionRemove(LCardKind kind, long card, long sentence, long mention)
    {
        _lQuillMentionTenure.LTenureRequestApply(new LRequestMentionRemoval(
            _lQuillMentionTenure.LTenureId, new LExampleSiteCard(kind, card, sentence), mention));
    }

    public void LQuillMentionRemove(long mention)
    {
        _lQuillMentionTenure.LTenureRequestApply(
            new LRequestMentionRemoval(_lQuillMentionTenure.LTenureId, new LExampleSiteDraft(), mention));
    }

    public LMentionDraft? LQuillMentionFind(LExampleSite site, LMentionDraft span)
    {
        ArgumentNullException.ThrowIfNull(span);

        return LMentionClerk.LMentionSpanFind(_lQuillMentionTenure.LTenureHerald.LTenureKeptRead(), site, span);
    }

    public bool LQuillMentionCheck(LCardKind kind, long cardId, long sentenceId, string text, int start, int length)
    {
        return LQuillMentionFind(
                new LExampleSiteCard(kind, cardId, sentenceId), LMentionClerk.LMentionSpanRead(text, start, length))
            is not null;
    }

    public bool LQuillMentionCheck(string text, int start, int length)
    {
        return LQuillMentionFind(new LExampleSiteDraft(), LMentionClerk.LMentionSpanRead(text, start, length))
            is not null;
    }

    public bool LQuillSenseCheck(LCardKind kind, long cardId, long sentenceId, string text, int start, int length)
    {
        return LQuillMentionFind(
                new LExampleSiteCard(kind, cardId, sentenceId), LMentionClerk.LMentionSpanRead(text, start, length))
            is { LMentionDraftLinked: true };
    }

    public bool LQuillSenseCheck(string text, int start, int length)
    {
        return LQuillMentionFind(new LExampleSiteDraft(), LMentionClerk.LMentionSpanRead(text, start, length))
            is { LMentionDraftLinked: true };
    }

    public long? LQuillSenseRead(LExampleSite site, string text, int start, int length)
    {
        _lQuillMentionTenure.LTenurePersist();
        return LQuillMentionFind(site, LMentionClerk.LMentionSpanRead(text, start, length))
            is { LMentionDraftLinked: true } found
            ? found.LMentionDraftEntry
            : null;
    }
}
