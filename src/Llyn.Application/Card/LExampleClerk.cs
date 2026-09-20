using System;
using System.Collections.Generic;
using System.Linq;
using Llyn.Core;

namespace Llyn.Application;

public sealed class LExampleClerk
{
    private readonly LExampleVault _lExampleClerkExamples;
    private readonly LGlossVault _lExampleClerkGlosses;
    private readonly LMentionVault _lExampleClerkMentions;

    public LExampleClerk(LRig rig)
    {
        ArgumentNullException.ThrowIfNull(rig);
        _lExampleClerkExamples = rig.LRigExamples;
        _lExampleClerkGlosses = rig.LRigGlosses;
        _lExampleClerkMentions = rig.LRigMentions;
    }

    public LExample? LExampleClerkResolve(
        LSentenceDraft draft, string language, long ownerId, bool collocation, Dictionary<long, long> identity)
    {
        ArgumentNullException.ThrowIfNull(draft);

        if (draft.LSentenceDraftExample is not LExampleDraft written
            || (written.LExampleDraftText.LStateValueEmpty && written.LExampleDraftId <= 0))
        {
            return null;
        }

        return LExampleClerkResolve(written, language, ownerId, collocation, identity);
    }

    public LExample LExampleClerkResolve(
        LExampleDraft written, string language, long ownerId, bool collocation, Dictionary<long, long> identity)
    {
        ArgumentNullException.ThrowIfNull(written);
        ArgumentNullException.ThrowIfNull(language);
        ArgumentNullException.ThrowIfNull(identity);

        LExampleVault examples = _lExampleClerkExamples;
        IReadOnlyList<LMentionDraft> drafts = LMentionResolve(written);
        IReadOnlyList<LMention> mentions = LDraftClerkMention.LMentionRead(drafts);
        IReadOnlyList<LGloss> glosses = LDraftClerkGloss.LGlossRead(written.LExampleDraftGloss);

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

            if ((!sameText || !sameSource) && LExampleShareCheck(stored.LExampleId, ownerId, collocation))
            {
                LExample forked = examples.LExampleCreate(new LExample(
                    0,
                    stored.LExampleLanguage,
                    written.LExampleDraftText,
                    written.LExampleDraftReference,
                    glosses,
                    mentions));
                LGlossRecord(identity, written.LExampleDraftGloss, forked.LExampleGloss);
                LMentionRecord(identity, drafts, forked.LExampleMention);
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
                LGlossVault rows = _lExampleClerkGlosses;
                rows.LGlossExampleSave(stored.LExampleId, glosses);
                stored = stored with { LExampleGloss = rows.LGlossExampleRead(stored.LExampleId) };
                LGlossRecord(identity, written.LExampleDraftGloss, stored.LExampleGloss);
            }

            if (!sameText || !sameMention)
            {
                LMentionVault rows = _lExampleClerkMentions;
                rows.LMentionExampleSave(stored.LExampleId, mentions);
                stored = stored with { LExampleMention = rows.LMentionExampleRead(stored.LExampleId) };
                LMentionRecord(identity, drafts, stored.LExampleMention);
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
        LIdentity.LIdentityRecord(identity, written.LExampleDraftId, created.LExampleId);
        LGlossRecord(identity, written.LExampleDraftGloss, created.LExampleGloss);
        LMentionRecord(identity, drafts, created.LExampleMention);
        return created;
    }

    public static IReadOnlyList<LMentionDraft> LMentionResolve(LExampleDraft written)
    {
        ArgumentNullException.ThrowIfNull(written);

        int length = LMentionClerk.LMentionRuneRead(written.LExampleDraftText.LStateValueShow()).Count;
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

    private static void LMentionRecord(
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
                    LIdentity.LIdentityRecord(identity, draft.LMentionDraftId, mention.LMentionId);
                    break;
                }
            }
        }
    }

    private bool LExampleShareCheck(long exampleId, long ownerId, bool collocation)
    {
        LOwner owner = collocation ? LOwner.LOwnerCollocation : LOwner.LOwnerMeaning;
        foreach (LUsage usage in _lExampleClerkExamples.LExampleUsageRead(exampleId))
        {
            if (usage.LUsageId != ownerId || usage.LUsageOwner != owner)
            {
                return true;
            }
        }

        return false;
    }

    private static void LGlossRecord(
        Dictionary<long, long> identity, IReadOnlyList<LGlossDraft> drafts, IReadOnlyList<LGloss> stored)
    {
        for (int index = 0; index < drafts.Count && index < stored.Count; index++)
        {
            if (drafts[index].LGlossDraftId < 0)
            {
                LIdentity.LIdentityRecord(identity, drafts[index].LGlossDraftId, stored[index].LGlossId);
            }
        }
    }
}
