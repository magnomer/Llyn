using System.Collections.Generic;
using System.Linq;
using Llyn.Core;
using Llyn.Infrastructure;

namespace Llyn.ShellEngine;

public sealed partial class LEngine
{
    private static IEnumerable<LSentenceDraft> LEngineSentenceRead(IReadOnlyList<LSentenceDraft> drafts)
    {
        foreach (LSentenceDraft draft in drafts)
        {
            if (!draft.LSentenceDraftEmpty)
            {
                yield return draft;
            }
        }
    }

    private LExample? LEngineExampleResolve(
        LExampleArchive examples,
        LSentenceDraft draft,
        string language,
        long ownerId,
        bool collocation,
        Dictionary<long, long> identity)
    {
        if (draft.LSentenceDraftExample is not LExampleDraft written
            || (written.LExampleDraftText.LStateValueEmpty && written.LExampleDraftId <= 0))
        {
            return null;
        }

        return LEngineExampleResolve(examples, written, language, ownerId, collocation, identity);
    }

    private LExample LEngineExampleResolve(
        LExampleArchive examples,
        LExampleDraft written,
        string language,
        long ownerId,
        bool collocation,
        Dictionary<long, long> identity)
    {
        IReadOnlyList<LMentionDraft> drafts = LEngineMentionResolve(written);
        IReadOnlyList<LMention> mentions = LEngineMentionRead(drafts);
        IReadOnlyList<LGloss> glosses = LEngineGlossRead(written.LExampleDraftGloss);

        if (written.LExampleDraftId > 0)
        {
            LExample stored = examples.LExampleRead(written.LExampleDraftId)
                ?? throw new LRefusal(LRefusal.LRefusalLink);

            bool sameText = stored.LExampleText == written.LExampleDraftText;
            bool sameSource = stored.LExampleSource == written.LExampleDraftReference;
            bool sameGloss = stored.LExampleGloss.SequenceEqual(glosses);
            bool sameMention = stored.LExampleMention.SequenceEqual(mentions);
            if (sameText && sameSource && sameGloss && sameMention)
            {
                return stored;
            }

            if ((!sameText || !sameSource) && LEngineShareCheck(stored.LExampleId, ownerId, collocation))
            {
                LExample forked = examples.LExampleCreate(new LExample(
                    0,
                    stored.LExampleLanguage,
                    written.LExampleDraftText,
                    written.LExampleDraftReference,
                    glosses,
                    mentions));
                LEngineGlossRecord(identity, written.LExampleDraftGloss, forked.LExampleGloss);
                LEngineMentionRecord(identity, drafts, forked.LExampleMention);
                return forked;
            }

            if (!sameText)
            {
                examples.LExampleTextUpdate(stored.LExampleId, written.LExampleDraftText);
                stored = stored with { LExampleText = written.LExampleDraftText };
            }

            if (!sameSource)
            {
                examples.LExampleSourceUpdate(stored.LExampleId, written.LExampleDraftReference);
                stored = stored with { LExampleSource = written.LExampleDraftReference };
            }

            if (!sameGloss)
            {
                LGlossArchive rows = new(_lEngineDatabase);
                rows.LGlossExampleSave(stored.LExampleId, glosses);
                stored = stored with { LExampleGloss = rows.LGlossExampleRead(stored.LExampleId) };
                LEngineGlossRecord(identity, written.LExampleDraftGloss, stored.LExampleGloss);
            }

            if (!sameText || !sameMention)
            {
                LMentionArchive rows = new(_lEngineDatabase);
                rows.LMentionExampleSave(stored.LExampleId, mentions);
                stored = stored with { LExampleMention = rows.LMentionExampleRead(stored.LExampleId) };
                LEngineMentionRecord(identity, drafts, stored.LExampleMention);
            }

            return stored;
        }

        LExample created = examples.LExampleCreate(new LExample(
            0,
            written.LExampleDraftLanguage.Length == 0 ? language : written.LExampleDraftLanguage,
            written.LExampleDraftText,
            written.LExampleDraftReference,
            glosses,
            mentions));
        LEngineIdentityRecord(identity, written.LExampleDraftId, created.LExampleId);
        LEngineGlossRecord(identity, written.LExampleDraftGloss, created.LExampleGloss);
        LEngineMentionRecord(identity, drafts, created.LExampleMention);
        return created;
    }

    private static IReadOnlyList<LMentionDraft> LEngineMentionResolve(LExampleDraft written)
    {
        int length = LEngineRuneRead(written.LExampleDraftText.LStateValueShow()).Count;
        List<LMentionDraft> kept = new(written.LExampleDraftMention.Count);
        foreach (LMentionDraft mention in written.LExampleDraftMention)
        {
            if (mention.LMentionDraftOffset >= 0
                && mention.LMentionDraftLength > 0
                && mention.LMentionDraftOffset + mention.LMentionDraftLength <= length)
            {
                kept.Add(mention);
            }
        }

        return LMentionDraft.LMentionDraftSort(kept);
    }

    private static void LEngineMentionRecord(
        Dictionary<long, long> identity, IReadOnlyList<LMentionDraft> drafts, IReadOnlyList<LMention> stored)
    {
        foreach (LMentionDraft draft in drafts)
        {
            if (draft.LMentionDraftId >= 0)
            {
                continue;
            }

            foreach (LMention mention in stored)
            {
                if (mention.LMentionOffset == draft.LMentionDraftOffset)
                {
                    LEngineIdentityRecord(identity, draft.LMentionDraftId, mention.LMentionId);
                    break;
                }
            }
        }
    }

    private bool LEngineShareCheck(long exampleId, long ownerId, bool collocation)
    {
        LOwner owner = collocation ? LOwner.LOwnerCollocation : LOwner.LOwnerMeaning;
        foreach (LUsage usage in new LExampleLink(_lEngineDatabase).LExampleUsageRead(exampleId))
        {
            if (usage.LUsageId != ownerId || usage.LUsageOwner != owner)
            {
                return true;
            }
        }

        return false;
    }
}
