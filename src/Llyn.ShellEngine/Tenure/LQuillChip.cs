using System;
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

    public string LQuillSituationAdd(long card, string text, int position, bool settled)
    {
        ArgumentNullException.ThrowIfNull(text);

        int placed = position;
        return LDraftClerkList.LDraftListParse(text, settled, part =>
        {
            _lQuillChipTenure.LTenureRequestApply(new LRequestSituationAddition(
                _lQuillChipTenure.LTenureId, card, new LStateWritten(part), placed++));
            return true;
        });
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

    public string LQuillRegisterAdd(long card, string text, int position, bool settled)
    {
        ArgumentNullException.ThrowIfNull(text);

        int placed = position;
        return LDraftClerkList.LDraftListParse(text, settled, part =>
        {
            _lQuillChipTenure.LTenureRequestApply(new LRequestRegisterAddition(
                _lQuillChipTenure.LTenureId, card, new LStateWritten(part), placed++));
            return true;
        });
    }

    public void LQuillRegisterInsert(long card, long register, int position)
    {
        _lQuillChipTenure.LTenureRequestApply(
            new LRequestRegisterPick(_lQuillChipTenure.LTenureId, card, register, position));
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
            return new LTranslationOffer(text, string.Empty, [], false, false);
        }

        long? stored = _lQuillChipTenure.LTenureRead()?.LDraftStored;
        bool chosen = _lQuillChipDrafts.LEngineTranslationResolve(word, stored) is not null;
        return new LTranslationOffer(text, word, _lQuillChipDrafts.LEngineProspectFind(word), true, chosen);
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

    private LTranslationOffer LQuillProspectFind(string kept)
    {
        if (LTranslationClerk.LTranslationWordRead(kept) is not string word)
        {
            return new LTranslationOffer(kept, string.Empty, [], false, false);
        }

        try
        {
            return new LTranslationOffer(kept, word, _lQuillChipDrafts.LEngineProspectFind(word), true, false);
        }
        catch (Exception)
        {
            return new LTranslationOffer(kept, string.Empty, [], false, false);
        }
    }
}
