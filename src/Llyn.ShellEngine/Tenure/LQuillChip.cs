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

    public string LQuillTagAdd(long card, string text, int position, bool settled)
    {
        ArgumentNullException.ThrowIfNull(text);

        int placed = position;
        return LDraftClerkList.LDraftListParse(text, settled, part =>
        {
            _lQuillChipTenure.LTenureRequestApply(
                new LRequestTagAddition(_lQuillChipTenure.LTenureId, card, part, placed++));
            return true;
        });
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

    public string LQuillTranslationAdd(long card, string text, int position)
    {
        ArgumentNullException.ThrowIfNull(text);

        int placed = position;
        return LDraftClerkList.LDraftListParse(text, false, part =>
        {
            long? stored = _lQuillChipTenure.LTenureRead()?.LDraftStored;
            if (_lQuillChipDrafts.LEngineTranslationResolve(part, stored) is not LEntry entry)
            {
                return false;
            }

            LQuillTranslationInsert(card, entry.LEntryId, placed++);
            return true;
        });
    }

    public void LQuillTranslationInsert(long card, long entry, int position)
    {
        _lQuillChipTenure.LTenureRequestApply(
            new LRequestTranslationPick(_lQuillChipTenure.LTenureId, card, entry, position));
    }
}
