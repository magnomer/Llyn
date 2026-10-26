using System;
using System.Collections.Generic;
using System.Linq;
using Llyn.Application;
using Llyn.Core;

namespace Llyn.ShellEngine;

public sealed class LQuillChip
{
    private readonly LTenure _lQuillChipTenure;

    private readonly LDraftPort _lQuillChipDrafts;

    private readonly LQuillMention _lQuillChipMention;

    public LQuillChip(LTenure tenure, LDraftPort drafts)
    {
        ArgumentNullException.ThrowIfNull(tenure);
        ArgumentNullException.ThrowIfNull(drafts);

        _lQuillChipTenure = tenure;
        _lQuillChipDrafts = drafts;
        _lQuillChipMention = new LQuillMention(tenure);
    }

    public string LQuillTagAdd(LCardKind kind, long card, string text, int position, bool settled)
    {
        ArgumentNullException.ThrowIfNull(text);

        int placed = position;
        return LDraftClerkList.LDraftListParse(text, settled, part =>
        {
            _lQuillChipTenure.LTenureRequestApply(
                new LRequestTagAddition(_lQuillChipTenure.LTenureId, kind, card, part, placed++));
            return true;
        });
    }

    public static string LQuillKeptRead(string text, bool settled)
    {
        return LDraftClerkList.LDraftListParse(text, settled, static _ => true);
    }

    public LTagOffer LQuillTagFind(LCardKind kind, long card, string kept)
    {
        return _lQuillChipDrafts.LEngineTagFind(_lQuillChipTenure, kind, card, kept);
    }

    public void LQuillTagInsert(LCardKind kind, long card, long tag, int position)
    {
        _lQuillChipTenure.LTenureRequestApply(
            new LRequestTagPick(_lQuillChipTenure.LTenureId, kind, card, tag, position));
    }

    public void LQuillTagRemove(LCardKind kind, long card, long tag)
    {
        _lQuillChipTenure.LTenureRequestApply(new LRequestTagRemoval(_lQuillChipTenure.LTenureId, kind, card, tag));
    }

    public string LQuillSituationAdd(LCardKind kind, long card, string text, int position, bool settled)
    {
        ArgumentNullException.ThrowIfNull(text);

        int placed = position;
        return LDraftClerkList.LDraftListParse(text, settled, part =>
        {
            _lQuillChipTenure.LTenureRequestApply(new LRequestSituationAddition(
                _lQuillChipTenure.LTenureId, kind, card, new LStateWritten(part), placed++));
            return true;
        });
    }

    public LSituationOffer LQuillSituationFind(LCardKind kind, long card, string kept)
    {
        return _lQuillChipDrafts.LEngineSituationFind(_lQuillChipTenure, kind, card, kept);
    }

    public void LQuillSituationInsert(LCardKind kind, long card, long situation, int position)
    {
        _lQuillChipTenure.LTenureRequestApply(
            new LRequestSituationPick(_lQuillChipTenure.LTenureId, kind, card, situation, position));
    }

    public void LQuillSituationRemove(LCardKind kind, long card, long situation)
    {
        _lQuillChipTenure.LTenureRequestApply(
            new LRequestSituationRemoval(_lQuillChipTenure.LTenureId, kind, card, situation));
    }

    public string LQuillRegisterAdd(LCardKind kind, long card, string text, int position, bool settled)
    {
        ArgumentNullException.ThrowIfNull(text);

        int placed = position;
        return LDraftClerkList.LDraftListParse(text, settled, part =>
        {
            _lQuillChipTenure.LTenureRequestApply(new LRequestRegisterAddition(
                _lQuillChipTenure.LTenureId, kind, card, new LStateWritten(part), placed++));
            return true;
        });
    }

    public LRegisterOffer LQuillRegisterFind(LCardKind kind, long card, string kept)
    {
        return _lQuillChipDrafts.LEngineRegisterFind(_lQuillChipTenure, kind, card, kept);
    }

    public void LQuillRegisterInsert(LCardKind kind, long card, long register, int position)
    {
        _lQuillChipTenure.LTenureRequestApply(
            new LRequestRegisterPick(_lQuillChipTenure.LTenureId, kind, card, register, position));
    }

    public void LQuillRegisterRemove(LCardKind kind, long card, long register)
    {
        _lQuillChipTenure.LTenureRequestApply(
            new LRequestRegisterRemoval(_lQuillChipTenure.LTenureId, kind, card, register));
    }

    public LReferenceOffer LQuillReferenceFind(LCardKind kind, long card, long sentence, string text)
    {
        ArgumentNullException.ThrowIfNull(text);

        return _lQuillChipDrafts.LEngineReferenceFind(
            _lQuillChipTenure, new LExampleSiteCard(kind, card, sentence), text);
    }

    public void LQuillCitationResolve(LCardKind kind, long card, long sentence, string title)
    {
        ArgumentNullException.ThrowIfNull(title);

        long reference = _lQuillChipDrafts.LEngineCitationResolve(
            _lQuillChipTenure, new LExampleSiteCard(kind, card, sentence), title);
        _lQuillChipTenure.LTenureRequestApply(
            new LRequestSentenceReference(_lQuillChipTenure.LTenureId, kind, card, sentence, reference));
    }

    public LTranslationOffer LQuillTranslationAdd(LCardKind kind, long card, string text, int position)
    {
        ArgumentNullException.ThrowIfNull(text);

        int placed = position;
        string kept = LDraftClerkList.LDraftListParse(text, false, part =>
        {
            long? stored = _lQuillChipTenure.LTenureRead()?.LDraftStored;
            if (_lQuillChipDrafts.LEngineTranslationResolve(part, stored) is not LEntry entry)
            {
                return false;
            }

            LQuillTranslationInsert(kind, card, entry.LEntryId, placed++);
            return true;
        });
        return LQuillProspectFind(kept);
    }

    public LTranslationOffer LQuillTranslationFind(string text)
    {
        ArgumentNullException.ThrowIfNull(text);

        if (LTranslationClerk.LTranslationWordRead(text) is not string word)
        {
            return new LTranslationOffer(text, string.Empty, [], false, false, []);
        }

        long? stored = _lQuillChipTenure.LTenureRead()?.LDraftStored;
        bool chosen = _lQuillChipDrafts.LEngineTranslationResolve(word, stored) is not null;
        return _lQuillChipDrafts.LEngineTranslationFind(_lQuillChipTenure, text, word, chosen);
    }

    public string LQuillTranslationResolve(LCardKind kind, long card, string text, int position)
    {
        ArgumentNullException.ThrowIfNull(text);

        long? stored = _lQuillChipTenure.LTenureRead()?.LDraftStored;
        if (_lQuillChipDrafts.LEngineTranslationResolve(text, stored) is not LEntry entry)
        {
            return text;
        }

        LQuillTranslationInsert(kind, card, entry.LEntryId, position);
        return string.Empty;
    }

    public void LQuillTranslationStart(
        LCardKind kind, long card, long entry, string headword, string language, int position)
    {
        if (entry != 0)
        {
            LQuillTranslationInsert(kind, card, entry, position);
            return;
        }

        string origin = _lQuillChipTenure.LTenureRead()?.LDraftOrigin ?? string.Empty;
        LCourt court = _lQuillChipDrafts.LEngineCourtStart(_lQuillChipTenure.LTenureId, origin, headword, language);
        LQuillTranslationInsert(kind, card, court.LCourtTargetId, position);
    }

    public void LQuillTranslationInsert(LCardKind kind, long card, long entry, int position)
    {
        _lQuillChipTenure.LTenureRequestApply(
            new LRequestTranslationPick(_lQuillChipTenure.LTenureId, kind, card, entry, position));
    }

    public void LQuillTranslationRemove(LCardKind kind, long card, long entry)
    {
        _lQuillChipTenure.LTenureRequestApply(
            new LRequestTranslationRemoval(_lQuillChipTenure.LTenureId, kind, card, entry));

        if (_lQuillChipDrafts.LEngineCourtFind(_lQuillChipTenure.LTenureId, entry) is LCourt court)
        {
            _lQuillChipDrafts.LEngineCourtDelete(court.LCourtId);
            _lQuillChipDrafts.LEngineDraftDelete(entry);
        }
    }

    public IReadOnlyDictionary<(LCardKind, long), IReadOnlyList<LTranslationTarget>> LQuillTranslationRead(
        LEntryDraft draft)
    {
        return LCardFacade.LEngineTranslationResolve(
            draft,
            ids => _lQuillChipTenure.LTenureEngine.LEngineCard.LEngineTargetRead(_lQuillChipTenure.LTenureId, ids));
    }

    public LTranslationOffer LQuillMentionFind(string text)
    {
        ArgumentNullException.ThrowIfNull(text);

        if (LTranslationClerk.LTranslationWordRead(text) is not string word)
        {
            return new LTranslationOffer(text, string.Empty, [], false, false, []);
        }

        IReadOnlyList<LVistaRow> rows = _lQuillChipDrafts.LEngineProspectFind(_lQuillChipTenure, word);
        return new LTranslationOffer(text, word, rows, rows.Count > 0, true, []);
    }

    public void LQuillMentionAdd(
        LCardKind kind, long card, long sentence, string text, int start, int length, long entry)
    {
        LQuillMentionAdd(new LExampleSiteCard(kind, card, sentence), text, start, length, entry);
    }

    public void LQuillSenseSet(
        LCardKind kind, long card, long sentence, string text, int start, int length, long sense)
    {
        LQuillSenseSet(new LExampleSiteCard(kind, card, sentence), text, start, length, sense);
    }

    public void LQuillMentionRemove(LCardKind kind, long card, long sentence, string text, int start, int length)
    {
        LQuillMentionRemove(new LExampleSiteCard(kind, card, sentence), text, start, length);
    }

    internal void LQuillMentionAdd(LExampleSite site, string text, int start, int length, long entry)
    {
        ArgumentNullException.ThrowIfNull(text);

        LMentionDraft span = _lQuillChipDrafts.LEngineSpanRead(text, start, length);
        _lQuillChipTenure.LTenureRequestApply(new LRequestMentionAddition(
            _lQuillChipTenure.LTenureId, site, span.LMentionDraftOffset, span.LMentionDraftLength, entry, 0));
    }

    internal void LQuillSenseSet(LExampleSite site, string text, int start, int length, long sense)
    {
        ArgumentNullException.ThrowIfNull(text);

        _lQuillChipTenure.LTenurePersist();
        if (_lQuillChipMention.LQuillMentionFind(site, _lQuillChipDrafts.LEngineSpanRead(text, start, length))
            is not LMentionDraft found)
        {
            return;
        }

        _lQuillChipTenure.LTenureRequestApply(
            new LRequestMentionSense(_lQuillChipTenure.LTenureId, site, found.LMentionDraftId, sense));
    }

    internal void LQuillMentionRemove(LExampleSite site, string text, int start, int length)
    {
        ArgumentNullException.ThrowIfNull(text);

        _lQuillChipTenure.LTenurePersist();
        if (_lQuillChipMention.LQuillMentionFind(site, _lQuillChipDrafts.LEngineSpanRead(text, start, length))
            is not LMentionDraft found)
        {
            return;
        }

        _lQuillChipTenure.LTenureRequestApply(
            new LRequestMentionRemoval(_lQuillChipTenure.LTenureId, site, found.LMentionDraftId));
    }

    private LTranslationOffer LQuillProspectFind(string kept)
    {
        if (LTranslationClerk.LTranslationWordRead(kept) is not string word)
        {
            return new LTranslationOffer(kept, string.Empty, [], false, false, []);
        }

        return _lQuillChipDrafts.LEngineTranslationFind(_lQuillChipTenure, kept, word, false);
    }
}
