using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Llyn.Core;

namespace Llyn.Application;

public sealed class LExampleClerk
{
    private readonly LExampleVault _lExampleClerkExamples;
    private readonly LGlossVault _lExampleClerkGlosses;
    private readonly LMentionVault _lExampleClerkMentions;
    private readonly LSentenceVault _lExampleClerkSentences;
    private readonly LReferenceClerk _lExampleClerkReferences;

    public LExampleClerk(LRig rig, LReferenceClerk references)
    {
        ArgumentNullException.ThrowIfNull(rig);
        ArgumentNullException.ThrowIfNull(references);
        _lExampleClerkExamples = rig.LRigExamples;
        _lExampleClerkGlosses = rig.LRigGlosses;
        _lExampleClerkMentions = rig.LRigMentions;
        _lExampleClerkSentences = rig.LRigSentences;
        _lExampleClerkReferences = references;
    }

    public static LExample LExampleClerkBlank =>
        new(
            0,
            string.Empty,
            LStateValue.LStateValueUnspecified,
            LStateAnchor.LStateAnchorUnspecified);

    public LExample LExampleClerkCreate(LExample example)
    {
        ArgumentNullException.ThrowIfNull(example);
        return _lExampleClerkExamples.LExampleCreate(example);
    }

    public LExample? LExampleClerkRead(long id)
    {
        return _lExampleClerkExamples.LExampleRead(id);
    }

    public IReadOnlyList<LExample> LExampleClerkRead()
    {
        return _lExampleClerkExamples.LExampleRead();
    }

    public IReadOnlyList<LExample> LExampleClerkRead(long ownerId, LOwner owner)
    {
        return owner switch
        {
            LOwner.LOwnerMeaning or LOwner.LOwnerCollocation =>
                [.. LSentenceRead(ownerId, owner)
                    .Select(sentence => sentence.LSentenceExample)
                    .OfType<LExample>()],
            _ => throw LExampleOwnerRaise(owner),
        };
    }

    public LPortraitPage? LExampleClerkRead(long id, LPortraitLegend legend)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);
        ArgumentNullException.ThrowIfNull(legend);

        LExample? example = _lExampleClerkExamples.LExampleRead(id);
        if (example is null)
        {
            return null;
        }

        LReference? cited = example.LExampleSource.LStateAnchorState == LState.LStateSpecified
            ? _lExampleClerkReferences.LReferenceClerkRead(example.LExampleSource.LStateAnchorShow())
            : null;
        return LExamplePageRead(example, cited, _lExampleClerkExamples.LExampleReferenceRead(id), legend);
    }

    public IReadOnlyList<LCatalogExample> LExampleClerkFind(string query, LCatalogOrder order)
    {
        ArgumentNullException.ThrowIfNull(query);
        query = query.Trim();

        IReadOnlyList<LExample> read = _lExampleClerkExamples.LExampleRead();
        IReadOnlyDictionary<long, int> usage = _lExampleClerkExamples.LExampleReferenceRead();
        IReadOnlyDictionary<long, string> cited = _lExampleClerkReferences.LCitationRead();

        List<LCatalogExample> rows = [];
        foreach (LExample example in read)
        {
            usage.TryGetValue(example.LExampleId, out int counted);

            LCatalogExample row = LCatalogExample.LCatalogExampleCreate(
                example,
                LExampleSourceRead(cited, example.LExampleSource),
                counted);
            if (row.LCatalogExampleMatch(query))
            {
                rows.Add(row);
            }
        }

        return LCatalogExample.LCatalogExampleSort(rows, order);
    }

    public IReadOnlyList<LSentence> LSentenceRead(long meaningId)
    {
        return _lExampleClerkSentences.LSentenceMeaningRead(meaningId);
    }

    public IReadOnlyList<LSentence> LSentenceRead(long ownerId, LOwner owner)
    {
        return owner switch
        {
            LOwner.LOwnerMeaning => _lExampleClerkSentences.LSentenceMeaningRead(ownerId),
            LOwner.LOwnerCollocation => _lExampleClerkSentences.LSentenceCollocationRead(ownerId),
            _ => throw LExampleOwnerRaise(owner),
        };
    }

    public void LExampleClerkUpdate(LExample example)
    {
        ArgumentNullException.ThrowIfNull(example);
        _lExampleClerkExamples.LExampleUpdate(example);
    }

    public void LExampleClerkUpdate(long exampleId, LStateAnchor reference)
    {
        _lExampleClerkExamples.LExampleSourceUpdate(exampleId, reference);
    }

    public void LExampleClerkAttach(long ownerId, long exampleId, int position, LOwner owner)
    {
        switch (owner)
        {
            case LOwner.LOwnerMeaning:
                _lExampleClerkSentences.LSentenceMeaningAttach(ownerId, exampleId, position);
                return;
            case LOwner.LOwnerCollocation:
                _lExampleClerkSentences.LSentenceCollocationAttach(ownerId, exampleId, position);
                return;
            default:
                throw LExampleOwnerRaise(owner);
        }
    }

    public void LExampleClerkDetach(long ownerId, long exampleId, LOwner owner)
    {
        switch (owner)
        {
            case LOwner.LOwnerMeaning:
                _lExampleClerkSentences.LSentenceMeaningDetach(ownerId, exampleId);
                return;
            case LOwner.LOwnerCollocation:
                _lExampleClerkSentences.LSentenceCollocationDetach(ownerId, exampleId);
                return;
            default:
                throw LExampleOwnerRaise(owner);
        }
    }

    public void LExampleClerkRemove(long ownerId, long exampleId, LOwner owner)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(exampleId);

        LExampleClerkDetach(ownerId, exampleId, owner);
        if (_lExampleClerkExamples.LExampleReferenceRead(exampleId) == 0)
        {
            _lExampleClerkExamples.LExampleDelete(exampleId);
        }
    }

    public void LExampleClerkDelete(long id)
    {
        _lExampleClerkExamples.LExampleDelete(id);
    }

    public void LExampleClerkDelete(long id, bool detach)
    {
        _lExampleClerkExamples.LExampleDelete(id, detach);
    }

    public static bool LExampleClerkMatch(LExample one, LExample other)
    {
        ArgumentNullException.ThrowIfNull(one);
        ArgumentNullException.ThrowIfNull(other);

        return string.Equals(one.LExampleLanguage, other.LExampleLanguage, StringComparison.Ordinal)
            && one.LExampleText == other.LExampleText
            && one.LExampleSource == other.LExampleSource
            && one.LExampleGloss.SequenceEqual(other.LExampleGloss)
            && one.LExampleMention.SequenceEqual(other.LExampleMention);
    }

    public static LPortraitPage LExamplePageRead(
        LExample example, LReference? cited, int count, LPortraitLegend legend)
    {
        ArgumentNullException.ThrowIfNull(example);
        ArgumentNullException.ThrowIfNull(legend);

        string mark = legend.LPortraitLegendUnknown;
        List<LPortraitSection> sections = [];

        List<LPortraitLine> glosses = [];
        foreach (LGloss gloss in example.LExampleGloss)
        {
            glosses.Add(new LPortraitLine(
                gloss.LGlossLanguage, LPortraitText.LPortraitTextRead(gloss.LGlossText, mark)));
        }

        if (glosses.Count > 0)
        {
            sections.Add(LPortraitSection.LPortraitSectionCreate(legend.LPortraitLegendTranslation, glosses));
        }

        string source = cited?.LReferenceNameRead() ?? string.Empty;

        if (source.Length > 0)
        {
            sections.Add(LPortraitSection.LPortraitSectionCreate(legend.LPortraitLegendSource, source));
        }

        return new LPortraitPage(
            LPortraitText.LPortraitTitleRead(example.LExampleText, legend.LPortraitLegendUnwritten, mark),
            example.LExampleLanguage,
            [legend.LPortraitTallyFormat(count)],
            sections);
    }

    private static string LExampleSourceRead(IReadOnlyDictionary<long, string> named, LStateAnchor source)
    {
        long id = source.LStateAnchorShow();
        if (id == 0)
        {
            return string.Empty;
        }

        return named.TryGetValue(id, out string? name)
            ? name
            : id.ToString(CultureInfo.InvariantCulture);
    }

    private static ArgumentOutOfRangeException LExampleOwnerRaise(LOwner owner)
    {
        return new ArgumentOutOfRangeException(
            nameof(owner), owner, "This entity has no reference from that kind of row.");
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
