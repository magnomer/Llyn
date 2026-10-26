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

    private readonly CProfferLantern _cCaretLantern = new();

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
            (kind, cardId, id) => desk.CDeskDraft.CDeskDraftChip?.LQuillSituationRemove(kind, cardId, id));
        CCaretTag = new CCaretTrail(
            desk,
            static card => card.LCardDraftTag.Select(static row => row.LTagDraftId),
            (kind, cardId, id) => desk.CDeskDraft.CDeskDraftChip?.LQuillTagRemove(kind, cardId, id));
        CCaretRegister = new CCaretTrail(
            desk,
            static card => card.LCardDraftRegister.Select(static row => row.LRegisterDraftId),
            (kind, cardId, id) => desk.CDeskDraft.CDeskDraftChip?.LQuillRegisterRemove(kind, cardId, id));
        CCaretTranslation = new CCaretTrail(
            desk, static draft => draft.LCardDraftTranslation, card.CCardTranslationRemove);
        _cCaretTrails = [CCaretSituation, CCaretTag, CCaretRegister, CCaretTranslation];
        foreach (CCaretTrail trail in _cCaretTrails)
        {
            trail.CCaretTrailMoved += () => CCaretMoved?.Invoke();
        }
    }

    public event Action? CCaretMoved;

    public CCaretTrail CCaretSituation { get; }

    public CCaretTrail CCaretTag { get; }

    public CCaretTrail CCaretRegister { get; }

    public CCaretTrail CCaretTranslation { get; }

    public IReadOnlyDictionary<CCaretTrail, long?> CCaretRead(bool collocated, long cardId)
    {
        return _cCaretTrails.ToDictionary(static trail => trail, trail => trail.CCaretTrailRead(collocated, cardId));
    }

    public int CCaretLanternIndex => _cCaretLantern.CProfferLanternIndex;

    public bool CCaretLanternLit => _cCaretLantern.CProfferLanternLit;

    public int? CCaretLanternMove(int step)
    {
        return _cCaretLantern.CProfferLanternMove(step);
    }

    public CProffer CCaretReferenceFind(bool collocated, long cardId, long sentenceId, string text)
    {
        return LCaretLanternShow(_cCaretCard.CCardReferenceFind(collocated, cardId, sentenceId, text));
    }

    public CProffer CCaretSituationAdd(bool collocated, long cardId, string text)
    {
        return LCaretLanternShow(LCaretSituationAdd(collocated, cardId, text, false));
    }

    public CProffer CCaretSituationSettle(bool collocated, long cardId, string text)
    {
        return LCaretLanternShow(LCaretSituationAdd(collocated, cardId, text, true));
    }

    public void CCaretSituationInsert(bool collocated, long cardId, long pickId)
    {
        int position = CCaretSituation.CCaretTrailResolve(collocated, cardId);
        _cCaretDesk.CDeskDraft.CDeskDraftChip?.LQuillSituationInsert(
            collocated ? LCardKind.LCardKindCollocation : LCardKind.LCardKindMeaning, cardId, pickId, position);
    }

    public CProffer CCaretTagAdd(bool collocated, long cardId, string text)
    {
        return LCaretLanternShow(LCaretTagAdd(collocated, cardId, text, false));
    }

    public CProffer CCaretTagSettle(bool collocated, long cardId, string text)
    {
        return LCaretLanternShow(LCaretTagAdd(collocated, cardId, text, true));
    }

    public void CCaretTagInsert(bool collocated, long cardId, long pickId)
    {
        int position = CCaretTag.CCaretTrailResolve(collocated, cardId);
        _cCaretDesk.CDeskDraft.CDeskDraftChip?.LQuillTagInsert(
            collocated ? LCardKind.LCardKindCollocation : LCardKind.LCardKindMeaning, cardId, pickId, position);
    }

    public CProffer CCaretRegisterAdd(bool collocated, long cardId, string text)
    {
        return LCaretLanternShow(LCaretRegisterAdd(collocated, cardId, text, false));
    }

    public CProffer CCaretRegisterSettle(bool collocated, long cardId, string text)
    {
        return LCaretLanternShow(LCaretRegisterAdd(collocated, cardId, text, true));
    }

    public void CCaretRegisterInsert(bool collocated, long cardId, long pickId)
    {
        int position = CCaretRegister.CCaretTrailResolve(collocated, cardId);
        _cCaretDesk.CDeskDraft.CDeskDraftChip?.LQuillRegisterInsert(
            collocated ? LCardKind.LCardKindCollocation : LCardKind.LCardKindMeaning, cardId, pickId, position);
    }

    public CProspect CCaretTranslationAdd(bool collocated, long cardId, string text)
    {
        return LCaretLanternShow(_cCaretCard.CCardTranslationAdd(
            collocated, cardId, text, CCaretTranslation.CCaretTrailResolve(collocated, cardId)));
    }

    public CProspect CCaretTranslationResolve(bool collocated, long cardId, string text, bool offered)
    {
        return LCaretLanternShow(_cCaretCard.CCardTranslationResolve(
            collocated, cardId, text, CCaretTranslation.CCaretTrailResolve(collocated, cardId), offered));
    }

    public void CCaretTranslationInsert(
        bool collocated, long cardId, long? entryId, string headword, string language)
    {
        _cCaretCard.CCardTranslationInsert(
            collocated, cardId, entryId, headword, language, CCaretTranslation.CCaretTrailResolve(collocated, cardId));
    }

    private CProffer LCaretLanternShow(CProffer offer)
    {
        _cCaretLantern.CProfferLanternShow(offer.CProfferShown ? offer.CProfferRows.Count : 0);
        return offer;
    }

    private CProspect LCaretLanternShow(CProspect prospect)
    {
        _cCaretLantern.CProfferLanternShow(
            prospect.CProspectShown ? prospect.CProspectRows.Count + prospect.CProspectLanguages.Count : 0);
        if (prospect.CProspectChosen)
        {
            _ = _cCaretLantern.CProfferLanternSet(0);
        }

        return prospect;
    }

    private CProffer LCaretRegisterAdd(bool collocated, long cardId, string text, bool settled)
    {
        LCardKind kind = collocated ? LCardKind.LCardKindCollocation : LCardKind.LCardKindMeaning;
        int position = CCaretRegister.CCaretTrailResolve(collocated, cardId);
        if (_cCaretDesk.CDeskDraft.CDeskDraftChip is not LQuillChip chip)
        {
            return new CProffer(text, [], false);
        }

        IReadOnlyList<long> before = CCaretRegister.CCaretTrailCopy(collocated, cardId);
        string kept;
        try
        {
            kept = chip.LQuillRegisterAdd(kind, cardId, text, position, settled);
        }
        catch (Exception exception)
        {
            _cCaretDesk.LDeskFailureShow(".UpdateFailed", exception);
            return new CProffer(
                LCaretKeptRead(CCaretRegister, before, (collocated, cardId), (text, settled)), [], false);
        }

        return _cCaretDesk.LDeskRepaintRead(
            _cCaretNoticed,
            () => LCaretProfferRead(chip.LQuillRegisterFind(kind, cardId, kept)),
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

    private CProffer LCaretTagAdd(bool collocated, long cardId, string text, bool settled)
    {
        LCardKind kind = collocated ? LCardKind.LCardKindCollocation : LCardKind.LCardKindMeaning;
        int position = CCaretTag.CCaretTrailResolve(collocated, cardId);
        if (_cCaretDesk.CDeskDraft.CDeskDraftChip is not LQuillChip chip)
        {
            return new CProffer(text, [], false);
        }

        IReadOnlyList<long> before = CCaretTag.CCaretTrailCopy(collocated, cardId);
        string kept;
        try
        {
            kept = chip.LQuillTagAdd(kind, cardId, text, position, settled);
        }
        catch (Exception exception)
        {
            _cCaretDesk.LDeskFailureShow(".UpdateFailed", exception);
            return new CProffer(LCaretKeptRead(CCaretTag, before, (collocated, cardId), (text, settled)), [], false);
        }

        return _cCaretDesk.LDeskRepaintRead(
            _cCaretNoticed,
            () => LCaretProfferRead(chip.LQuillTagFind(kind, cardId, kept)),
            new CProffer(kept, [], false),
            ".FindFailed");
    }

    private static CProffer LCaretProfferRead(LTagOffer offer)
    {
        return new CProffer(
            offer.LTagOfferText,
            offer.LTagOfferRows
                .Select(static row => new CProfferRow(
                    row.LTagRowId, row.LTagRowLead, row.LTagRowMark, row.LTagRowTail))
                .ToList(),
            offer.LTagOfferShown);
    }

    private CProffer LCaretSituationAdd(bool collocated, long cardId, string text, bool settled)
    {
        LCardKind kind = collocated ? LCardKind.LCardKindCollocation : LCardKind.LCardKindMeaning;
        int position = CCaretSituation.CCaretTrailResolve(collocated, cardId);
        if (_cCaretDesk.CDeskDraft.CDeskDraftChip is not LQuillChip chip)
        {
            return new CProffer(text, [], false);
        }

        IReadOnlyList<long> before = CCaretSituation.CCaretTrailCopy(collocated, cardId);
        string kept;
        try
        {
            kept = chip.LQuillSituationAdd(kind, cardId, text, position, settled);
        }
        catch (Exception exception)
        {
            _cCaretDesk.LDeskFailureShow(".UpdateFailed", exception);
            return new CProffer(
                LCaretKeptRead(CCaretSituation, before, (collocated, cardId), (text, settled)), [], false);
        }

        return _cCaretDesk.LDeskRepaintRead(
            _cCaretNoticed,
            () => LCaretProfferRead(chip.LQuillSituationFind(kind, cardId, kept)),
            new CProffer(kept, [], false),
            ".FindFailed");
    }

    private static string LCaretKeptRead(
        CCaretTrail trail, IReadOnlyList<long> before, (bool, long) card, (string, bool) typed)
    {
        (bool collocated, long cardId) = card;
        (string text, bool settled) = typed;
        return before.SequenceEqual(trail.CCaretTrailCopy(collocated, cardId))
            ? text
            : LQuillChip.LQuillKeptRead(text, settled);
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
