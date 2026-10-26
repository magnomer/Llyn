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

    private readonly Dictionary<(object, bool, long), (LTenure?, string, object)> _cCaretAnswer = [];

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

    public CProffer? CCaretReferenceFind(bool collocated, long cardId, long sentenceId, string text)
    {
        string stored = _cCaretCard.CCardCitationRead(collocated, cardId, sentenceId);
        if (string.Equals(text, stored, StringComparison.Ordinal))
        {
            _ = _cCaretAnswer.Remove((sentenceId, collocated, cardId));
            return null;
        }

        return LCaretAnswerRead(
            (sentenceId, collocated, cardId),
            text,
            () => LCaretLanternShow(_cCaretCard.CCardReferenceFind(collocated, cardId, sentenceId, text)),
            static offer => offer.CProfferText);
    }

    public CProffer CCaretSituationAdd(bool collocated, long cardId, string text)
    {
        return LCaretAnswerRead(
            (CCaretSituation, collocated, cardId),
            text,
            () => LCaretLanternShow(LCaretSituationAdd(collocated, cardId, text, false)),
            static offer => offer.CProfferText);
    }

    public CProffer CCaretSituationSettle(bool collocated, long cardId, string text)
    {
        return LCaretAnswerRead(
            (CCaretSituation, collocated, cardId),
            null,
            () => LCaretLanternShow(LCaretSituationAdd(collocated, cardId, text, true)),
            static offer => offer.CProfferText);
    }

    public void CCaretSituationInsert(bool collocated, long cardId, long pickId)
    {
        _ = _cCaretAnswer.Remove((CCaretSituation, collocated, cardId));
        int position = CCaretSituation.CCaretTrailResolve(collocated, cardId);
        _cCaretDesk.CDeskDraft.CDeskDraftChip?.LQuillSituationInsert(
            collocated ? LCardKind.LCardKindCollocation : LCardKind.LCardKindMeaning, cardId, pickId, position);
    }

    public CProffer CCaretTagAdd(bool collocated, long cardId, string text)
    {
        return LCaretAnswerRead(
            (CCaretTag, collocated, cardId),
            text,
            () => LCaretLanternShow(LCaretTagAdd(collocated, cardId, text, false)),
            static offer => offer.CProfferText);
    }

    public CProffer CCaretTagSettle(bool collocated, long cardId, string text)
    {
        return LCaretAnswerRead(
            (CCaretTag, collocated, cardId),
            null,
            () => LCaretLanternShow(LCaretTagAdd(collocated, cardId, text, true)),
            static offer => offer.CProfferText);
    }

    public void CCaretTagInsert(bool collocated, long cardId, long pickId)
    {
        _ = _cCaretAnswer.Remove((CCaretTag, collocated, cardId));
        int position = CCaretTag.CCaretTrailResolve(collocated, cardId);
        _cCaretDesk.CDeskDraft.CDeskDraftChip?.LQuillTagInsert(
            collocated ? LCardKind.LCardKindCollocation : LCardKind.LCardKindMeaning, cardId, pickId, position);
    }

    public CProffer CCaretRegisterAdd(bool collocated, long cardId, string text)
    {
        return LCaretAnswerRead(
            (CCaretRegister, collocated, cardId),
            text,
            () => LCaretLanternShow(LCaretRegisterAdd(collocated, cardId, text, false)),
            static offer => offer.CProfferText);
    }

    public CProffer CCaretRegisterSettle(bool collocated, long cardId, string text)
    {
        return LCaretAnswerRead(
            (CCaretRegister, collocated, cardId),
            null,
            () => LCaretLanternShow(LCaretRegisterAdd(collocated, cardId, text, true)),
            static offer => offer.CProfferText);
    }

    public void CCaretRegisterInsert(bool collocated, long cardId, long pickId)
    {
        _ = _cCaretAnswer.Remove((CCaretRegister, collocated, cardId));
        int position = CCaretRegister.CCaretTrailResolve(collocated, cardId);
        _cCaretDesk.CDeskDraft.CDeskDraftChip?.LQuillRegisterInsert(
            collocated ? LCardKind.LCardKindCollocation : LCardKind.LCardKindMeaning, cardId, pickId, position);
    }

    public CProspect CCaretTranslationAdd(bool collocated, long cardId, string text)
    {
        return LCaretAnswerRead(
            (CCaretTranslation, collocated, cardId),
            text,
            () => LCaretLanternShow(_cCaretCard.CCardTranslationAdd(
                collocated, cardId, text, CCaretTranslation.CCaretTrailResolve(collocated, cardId))),
            static prospect => prospect.CProspectText);
    }

    public CProspect CCaretTranslationResolve(bool collocated, long cardId, string text, bool offered)
    {
        return LCaretAnswerRead(
            (CCaretTranslation, collocated, cardId),
            null,
            () => LCaretLanternShow(_cCaretCard.CCardTranslationResolve(
                collocated, cardId, text, CCaretTranslation.CCaretTrailResolve(collocated, cardId), offered)),
            static prospect => prospect.CProspectText);
    }

    public void CCaretTranslationInsert(
        bool collocated, long cardId, long? entryId, string headword, string language)
    {
        _ = _cCaretAnswer.Remove((CCaretTranslation, collocated, cardId));
        _cCaretCard.CCardTranslationInsert(
            collocated, cardId, entryId, headword, language, CCaretTranslation.CCaretTrailResolve(collocated, cardId));
    }

    private TAnswer LCaretAnswerRead<TAnswer>(
        (object, bool, long) field, string? echo, Func<TAnswer> gate, Func<TAnswer, string> written)
        where TAnswer : class
    {
        LTenure? tenure = _cCaretDesk.CDeskDraft.CDeskDraftTenure;
        if (echo is not null
            && _cCaretAnswer.TryGetValue(field, out (LTenure?, string, object) prior)
            && ReferenceEquals(prior.Item1, tenure)
            && string.Equals(prior.Item2, echo, StringComparison.Ordinal)
            && prior.Item3 is TAnswer repeated)
        {
            return repeated;
        }

        TAnswer answer = gate();
        _cCaretAnswer[field] = (tenure, written(answer), answer);
        return answer;
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

        string kept = chip.LQuillRegisterAdd(kind, cardId, text, position, settled, out Exception? fault);
        if (fault is not null)
        {
            _cCaretDesk.LDeskFailureShow(".UpdateFailed", fault);
            return new CProffer(kept, [], false);
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

        string kept = chip.LQuillTagAdd(kind, cardId, text, position, settled, out Exception? fault);
        if (fault is not null)
        {
            _cCaretDesk.LDeskFailureShow(".UpdateFailed", fault);
            return new CProffer(kept, [], false);
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

        string kept = chip.LQuillSituationAdd(kind, cardId, text, position, settled, out Exception? fault);
        if (fault is not null)
        {
            _cCaretDesk.LDeskFailureShow(".UpdateFailed", fault);
            return new CProffer(kept, [], false);
        }

        return _cCaretDesk.LDeskRepaintRead(
            _cCaretNoticed,
            () => LCaretProfferRead(chip.LQuillSituationFind(kind, cardId, kept)),
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
