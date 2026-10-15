using System;
using System.Collections.Generic;
using System.Linq;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.Conduct;

public sealed class CCaret
{
    private readonly CDesk _cCaretDesk;

    private readonly CCard _cCaretCard;

    private readonly CLedgerNoticed _cCaretNoticed;

    private readonly IReadOnlyList<CCaretTrail> _cCaretTrails;

    internal CCaret(CDesk desk, CCard card, CLedgerNoticed noticed)
    {
        ArgumentNullException.ThrowIfNull(desk);
        ArgumentNullException.ThrowIfNull(card);
        ArgumentNullException.ThrowIfNull(noticed);

        _cCaretDesk = desk;
        _cCaretCard = card;
        _cCaretNoticed = noticed;
        CCaretSituation = new CCaretTrail(
            desk,
            static card => card.LCardDraftSituation.Select(static row => row.LSituationDraftId),
            (cardId, id) => desk.CDeskDraft.CDeskDraftChip?.LQuillSituationRemove(cardId, id));
        CCaretTag = new CCaretTrail(
            desk,
            static card => card.LCardDraftTag.Select(static row => row.LTagDraftId),
            (cardId, id) => desk.CDeskDraft.CDeskDraftChip?.LQuillTagRemove(cardId, id));
        CCaretRegister = new CCaretTrail(
            desk,
            static card => card.LCardDraftRegister.Select(static row => row.LRegisterDraftId),
            (cardId, id) => desk.CDeskDraft.CDeskDraftChip?.LQuillRegisterRemove(cardId, id));
        CCaretTranslation = new CCaretTrail(
            desk, static draft => draft.LCardDraftTranslation, card.CCardTranslationRemove);
        _cCaretTrails = [CCaretSituation, CCaretTag, CCaretRegister, CCaretTranslation];
    }

    public CCaretTrail CCaretSituation { get; }

    public CCaretTrail CCaretTag { get; }

    public CCaretTrail CCaretRegister { get; }

    public CCaretTrail CCaretTranslation { get; }

    public IReadOnlyDictionary<CCaretTrail, long?> CCaretRead(long cardId)
    {
        return _cCaretTrails.ToDictionary(static trail => trail, trail => trail.CCaretTrailRead(cardId));
    }

    public CProffer CCaretSituationAdd(long cardId, string text)
    {
        return LCaretSituationAdd(cardId, text, false);
    }

    public CProffer CCaretSituationSettle(long cardId, string text)
    {
        return LCaretSituationAdd(cardId, text, true);
    }

    public void CCaretSituationInsert(long cardId, long pickId)
    {
        int position = CCaretSituation.CCaretTrailResolve(cardId);
        _cCaretDesk.CDeskDraft.CDeskDraftChip?.LQuillSituationInsert(cardId, pickId, position);
    }

    public CSlate CCaretTagAdd(long cardId, string text)
    {
        return LCaretTagAdd(cardId, text, false);
    }

    public CSlate CCaretTagSettle(long cardId, string text)
    {
        return LCaretTagAdd(cardId, text, true);
    }

    public void CCaretTagInsert(long cardId, long pickId)
    {
        int position = CCaretTag.CCaretTrailResolve(cardId);
        _cCaretDesk.CDeskDraft.CDeskDraftChip?.LQuillTagInsert(cardId, pickId, position);
    }

    public CProffer CCaretRegisterAdd(long cardId, string text)
    {
        return LCaretRegisterAdd(cardId, text, false);
    }

    public CProffer CCaretRegisterSettle(long cardId, string text)
    {
        return LCaretRegisterAdd(cardId, text, true);
    }

    public void CCaretRegisterInsert(long cardId, long pickId)
    {
        int position = CCaretRegister.CCaretTrailResolve(cardId);
        _cCaretDesk.CDeskDraft.CDeskDraftChip?.LQuillRegisterInsert(cardId, pickId, position);
    }

    public CProspect CCaretTranslationAdd(long cardId, string text)
    {
        return _cCaretCard.CCardTranslationAdd(cardId, text, CCaretTranslation.CCaretTrailResolve(cardId));
    }

    public CProspect CCaretTranslationResolve(long cardId, string text, bool offered)
    {
        return _cCaretCard.CCardTranslationResolve(
            cardId, text, CCaretTranslation.CCaretTrailResolve(cardId), offered);
    }

    public void CCaretTranslationInsert(long cardId, long? entryId, string headword, string language)
    {
        _cCaretCard.CCardTranslationInsert(
            cardId, entryId, headword, language, CCaretTranslation.CCaretTrailResolve(cardId));
    }

    private CProffer LCaretRegisterAdd(long cardId, string text, bool settled)
    {
        int position = CCaretRegister.CCaretTrailResolve(cardId);
        if (_cCaretDesk.CDeskDraft.CDeskDraftChip is not LQuillChip chip)
        {
            return new CProffer(text, [], false);
        }

        string kept;
        try
        {
            kept = chip.LQuillRegisterAdd(cardId, text, position, settled);
        }
        catch (Exception exception)
        {
            _cCaretDesk.LDeskFailureShow(".UpdateFailed", exception);
            return new CProffer(text, [], false);
        }

        return _cCaretDesk.LDeskRepaintRead(
            _cCaretNoticed,
            () => LCaretProfferRead(chip.LQuillRegisterFind(cardId, kept)),
            new CProffer(kept, [], false),
            ".FindFailed");
    }

    private static CProffer LCaretProfferRead(LRegisterOffer offer)
    {
        return new CProffer(
            offer.LRegisterOfferText,
            offer.LRegisterOfferRows
                .Select(static row => new CProfferRow(
                    row.LRegisterRowId, row.LRegisterRowLead, row.LRegisterRowMark, row.LRegisterRowTail))
                .ToList(),
            offer.LRegisterOfferShown);
    }

    private CSlate LCaretTagAdd(long cardId, string text, bool settled)
    {
        int position = CCaretTag.CCaretTrailResolve(cardId);
        if (_cCaretDesk.CDeskDraft.CDeskDraftChip is not LQuillChip chip)
        {
            return new CSlate(text, [], false);
        }

        string kept;
        try
        {
            kept = chip.LQuillTagAdd(cardId, text, position, settled);
        }
        catch (Exception exception)
        {
            _cCaretDesk.LDeskFailureShow(".UpdateFailed", exception);
            return new CSlate(text, [], false);
        }

        return _cCaretDesk.LDeskRepaintRead(
            _cCaretNoticed,
            () => LCaretSlateRead(chip.LQuillTagFind(cardId, kept)),
            new CSlate(kept, [], false),
            ".FindFailed");
    }

    private static CSlate LCaretSlateRead(LTagOffer offer)
    {
        return new CSlate(
            offer.LTagOfferText,
            offer.LTagOfferRows
                .Select(static row => new CSlateRow(row.LTagRowId, row.LTagRowLead, row.LTagRowMark, row.LTagRowTail))
                .ToList(),
            offer.LTagOfferShown);
    }

    private CProffer LCaretSituationAdd(long cardId, string text, bool settled)
    {
        int position = CCaretSituation.CCaretTrailResolve(cardId);
        if (_cCaretDesk.CDeskDraft.CDeskDraftChip is not LQuillChip chip)
        {
            return new CProffer(text, [], false);
        }

        string kept;
        try
        {
            kept = chip.LQuillSituationAdd(cardId, text, position, settled);
        }
        catch (Exception exception)
        {
            _cCaretDesk.LDeskFailureShow(".UpdateFailed", exception);
            return new CProffer(text, [], false);
        }

        return _cCaretDesk.LDeskRepaintRead(
            _cCaretNoticed,
            () => LCaretProfferRead(chip.LQuillSituationFind(cardId, kept)),
            new CProffer(kept, [], false),
            ".FindFailed");
    }

    private static CProffer LCaretProfferRead(LSituationOffer offer)
    {
        return new CProffer(
            offer.LSituationOfferText,
            offer.LSituationOfferRows
                .Select(static row => new CProfferRow(
                    row.LSituationRowId,
                    row.LSituationRowLead,
                    row.LSituationRowMark,
                    row.LSituationRowTail,
                    row.LSituationRowCount))
                .ToList(),
            offer.LSituationOfferShown);
    }
}
