using System;
using System.Collections.Generic;
using Llyn.Application;
using Llyn.Core;

namespace Llyn.ShellEngine;

public sealed class LQuillChip
{
    private readonly LTenure _lQuillChipTenure;

    private readonly LDraftPort _lQuillChipDrafts;

    public LQuillChip(LTenure tenure, LDraftPort drafts)
    {
        ArgumentNullException.ThrowIfNull(tenure);
        ArgumentNullException.ThrowIfNull(drafts);

        _lQuillChipTenure = tenure;
        _lQuillChipDrafts = drafts;
    }

    public LTagOffer LQuillTagAdd(long card, string text, int position, bool settled)
    {
        ArgumentNullException.ThrowIfNull(text);

        int placed = position;
        string kept = LDraftClerkList.LDraftListParse(text, settled, part =>
        {
            _lQuillChipTenure.LTenureRequestApply(
                new LRequestTagAddition(_lQuillChipTenure.LTenureId, card, part, placed++));
            return true;
        });
        return LQuillTagFind(card, kept);
    }

    public void LQuillTagInsert(long card, long tag, int position)
    {
        _lQuillChipTenure.LTenureRequestApply(new LRequestTagPick(_lQuillChipTenure.LTenureId, card, tag, position));
    }

    public void LQuillTagRemove(long card, long tag)
    {
        _lQuillChipTenure.LTenureRequestApply(new LRequestTagRemoval(_lQuillChipTenure.LTenureId, card, tag));
    }

    public LSituationOffer LQuillSituationAdd(long card, string text, int position, bool settled)
    {
        ArgumentNullException.ThrowIfNull(text);

        int placed = position;
        string kept = LDraftClerkList.LDraftListParse(text, settled, part =>
        {
            _lQuillChipTenure.LTenureRequestApply(new LRequestSituationAddition(
                _lQuillChipTenure.LTenureId, card, new LStateWritten(part), placed++));
            return true;
        });
        return LQuillSituationFind(card, kept);
    }

    public void LQuillSituationInsert(long card, long situation, int position)
    {
        _lQuillChipTenure.LTenureRequestApply(
            new LRequestSituationPick(_lQuillChipTenure.LTenureId, card, situation, position));
    }

    public void LQuillSituationRemove(long card, long situation)
    {
        _lQuillChipTenure.LTenureRequestApply(
            new LRequestSituationRemoval(_lQuillChipTenure.LTenureId, card, situation));
    }

    public LRegisterOffer LQuillRegisterAdd(long card, string text, int position, bool settled)
    {
        ArgumentNullException.ThrowIfNull(text);

        int placed = position;
        string kept = LDraftClerkList.LDraftListParse(text, settled, part =>
        {
            _lQuillChipTenure.LTenureRequestApply(new LRequestRegisterAddition(
                _lQuillChipTenure.LTenureId, card, new LStateWritten(part), placed++));
            return true;
        });
        return LQuillRegisterFind(card, kept);
    }

    public void LQuillRegisterInsert(long card, long register, int position)
    {
        _lQuillChipTenure.LTenureRequestApply(
            new LRequestRegisterPick(_lQuillChipTenure.LTenureId, card, register, position));
    }

    public void LQuillRegisterRemove(long card, long register)
    {
        _lQuillChipTenure.LTenureRequestApply(new LRequestRegisterRemoval(_lQuillChipTenure.LTenureId, card, register));
    }

    public LReferenceOffer LQuillReferenceFind(long card, long sentence, string text)
    {
        ArgumentNullException.ThrowIfNull(text);

        try
        {
            return _lQuillChipDrafts.LEngineReferenceFind(_lQuillChipTenure, card, sentence, text);
        }
        catch (Exception)
        {
            return new LReferenceOffer(text, [], false);
        }
    }

    public void LQuillCitationResolve(long card, long sentence, string title)
    {
        ArgumentNullException.ThrowIfNull(title);

        long reference = _lQuillChipDrafts.LEngineCitationResolve(_lQuillChipTenure, card, sentence, title);
        _lQuillChipTenure.LTenureRequestApply(
            new LRequestSentenceReference(_lQuillChipTenure.LTenureId, card, sentence, reference));
    }

    public void LQuillReferenceResolve(string title)
    {
        ArgumentNullException.ThrowIfNull(title);

        long reference = _lQuillChipDrafts.LEngineCitationResolve(_lQuillChipTenure, 0, 0, title);
        _lQuillChipTenure.LTenureRequestApply(new LRequestExampleReference(_lQuillChipTenure.LTenureId, reference));
    }

    public LTranslationOffer LQuillTranslationAdd(long card, string text, int position)
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

            LQuillTranslationInsert(card, entry.LEntryId, placed++);
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

    public string LQuillTranslationResolve(long card, string text, int position)
    {
        ArgumentNullException.ThrowIfNull(text);

        long? stored = _lQuillChipTenure.LTenureRead()?.LDraftStored;
        if (_lQuillChipDrafts.LEngineTranslationResolve(text, stored) is not LEntry entry)
        {
            return text;
        }

        LQuillTranslationInsert(card, entry.LEntryId, position);
        return string.Empty;
    }

    public void LQuillTranslationStart(long card, long entry, string headword, string language, int position)
    {
        if (entry != 0)
        {
            LQuillTranslationInsert(card, entry, position);
            return;
        }

        string origin = _lQuillChipTenure.LTenureRead()?.LDraftOrigin ?? string.Empty;
        LCourt court = _lQuillChipDrafts.LEngineCourtStart(_lQuillChipTenure.LTenureId, origin, headword, language);
        LQuillTranslationInsert(card, court.LCourtTargetId, position);
    }

    public void LQuillTranslationInsert(long card, long entry, int position)
    {
        _lQuillChipTenure.LTenureRequestApply(
            new LRequestTranslationPick(_lQuillChipTenure.LTenureId, card, entry, position));
    }

    public void LQuillTranslationRemove(long card, long entry)
    {
        _lQuillChipTenure.LTenureRequestApply(
            new LRequestTranslationRemoval(_lQuillChipTenure.LTenureId, card, entry));

        try
        {
            if (_lQuillChipDrafts.LEngineCourtFind(_lQuillChipTenure.LTenureId, entry) is LCourt court)
            {
                _lQuillChipDrafts.LEngineCourtDelete(court.LCourtId);
                _lQuillChipDrafts.LEngineDraftDelete(entry);
            }
        }
        catch (Exception)
        {
        }
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

    public void LQuillMentionAdd(long card, long sentence, string text, int start, int length, long entry)
    {
        ArgumentNullException.ThrowIfNull(text);

        LMentionDraft span = _lQuillChipDrafts.LEngineSpanRead(text, start, length);
        _lQuillChipTenure.LTenureRequestApply(new LRequestMentionAddition(
            _lQuillChipTenure.LTenureId, card, sentence, span.LMentionDraftOffset, span.LMentionDraftLength, entry, 0));
    }

    public void LQuillSenseSet(long card, long sentence, string text, int start, int length, long sense)
    {
        ArgumentNullException.ThrowIfNull(text);

        _lQuillChipTenure.LTenurePersist();
        if (_lQuillChipTenure.LTenureMentionFind(card, sentence, _lQuillChipDrafts.LEngineSpanRead(text, start, length))
            is not LMentionDraft found)
        {
            return;
        }

        _lQuillChipTenure.LTenureRequestApply(new LRequestMentionSense(
            _lQuillChipTenure.LTenureId, card, sentence, found.LMentionDraftId, sense));
    }

    public void LQuillMentionRemove(long card, long sentence, string text, int start, int length)
    {
        ArgumentNullException.ThrowIfNull(text);

        _lQuillChipTenure.LTenurePersist();
        if (_lQuillChipTenure.LTenureMentionFind(card, sentence, _lQuillChipDrafts.LEngineSpanRead(text, start, length))
            is not LMentionDraft found)
        {
            return;
        }

        _lQuillChipTenure.LTenureRequestApply(new LRequestMentionRemoval(
            _lQuillChipTenure.LTenureId, card, sentence, found.LMentionDraftId));
    }

    private LTagOffer LQuillTagFind(long card, string kept)
    {
        try
        {
            return _lQuillChipDrafts.LEngineTagFind(_lQuillChipTenure, card, kept);
        }
        catch (Exception)
        {
            return new LTagOffer(kept, [], false);
        }
    }

    private LSituationOffer LQuillSituationFind(long card, string kept)
    {
        try
        {
            return _lQuillChipDrafts.LEngineSituationFind(_lQuillChipTenure, card, kept);
        }
        catch (Exception)
        {
            return new LSituationOffer(kept, [], false);
        }
    }

    private LRegisterOffer LQuillRegisterFind(long card, string kept)
    {
        try
        {
            return _lQuillChipDrafts.LEngineRegisterFind(_lQuillChipTenure, card, kept);
        }
        catch (Exception)
        {
            return new LRegisterOffer(kept, [], false);
        }
    }

    private LTranslationOffer LQuillProspectFind(string kept)
    {
        if (LTranslationClerk.LTranslationWordRead(kept) is not string word)
        {
            return new LTranslationOffer(kept, string.Empty, [], false, false, []);
        }

        try
        {
            return _lQuillChipDrafts.LEngineTranslationFind(_lQuillChipTenure, kept, word, false);
        }
        catch (Exception)
        {
            return new LTranslationOffer(kept, string.Empty, [], false, false, []);
        }
    }
}
