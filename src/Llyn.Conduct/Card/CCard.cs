using System;
using System.Collections.Generic;
using System.Linq;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.Conduct;

public sealed class CCard
{
    private readonly CDesk _cCardDesk;

    private readonly LDraftPort _cCardDraftPort;

    private readonly LReferencePort _cCardReferencePort;

    private readonly LSettingsPort _cCardSettingsPort;

    private readonly CEnvoy _cCardEnvoy;

    internal CCard(CDesk desk, LDraftPort drafts, LReferencePort references, LSettingsPort settings, CEnvoy envoy)
    {
        ArgumentNullException.ThrowIfNull(desk);
        ArgumentNullException.ThrowIfNull(drafts);
        ArgumentNullException.ThrowIfNull(references);
        ArgumentNullException.ThrowIfNull(settings);
        ArgumentNullException.ThrowIfNull(envoy);

        _cCardDesk = desk;
        _cCardDraftPort = drafts;
        _cCardReferencePort = references;
        _cCardSettingsPort = settings;
        _cCardEnvoy = envoy;
    }

    public CProspect CCardMentionRead(string word)
    {
        return CMention.LMentionProspectRead(_cCardDesk, word, _cCardEnvoy, _cCardSettingsPort);
    }

    public CProspect CCardTranslationAdd(long cardId, string text, int position)
    {
        ArgumentNullException.ThrowIfNull(text);

        try
        {
            return _cCardDesk.CDeskChip?.LQuillTranslationAdd(cardId, text, position) is LTranslationOffer offer
                ? LCardProspectRead(offer)
                : new CProspect(text, string.Empty, [], false, false, []);
        }
        catch (Exception exception)
        {
            CLedger.LLedgerFailureShow(_cCardEnvoy, _cCardSettingsPort, "Input.TranslationFailed", exception);
            return new CProspect(text, string.Empty, [], false, false, []);
        }
    }

    public CProspect CCardTranslationResolve(long cardId, string text, int position, bool offered)
    {
        ArgumentNullException.ThrowIfNull(text);

        if (_cCardDesk.CDeskChip is not LQuillChip chip)
        {
            return new CProspect(text, string.Empty, [], false, false, []);
        }

        try
        {
            return offered
                ? LCardProspectRead(chip.LQuillTranslationFind(text))
                : new CProspect(
                    chip.LQuillTranslationResolve(cardId, text, position), string.Empty, [], false, false, []);
        }
        catch (Exception exception)
        {
            CLedger.LLedgerFailureShow(_cCardEnvoy, _cCardSettingsPort, "Input.TranslationFailed", exception);
            return new CProspect(text, string.Empty, [], false, false, []);
        }
    }

    public void CCardTranslationInsert(long cardId, long? entryId, string headword, string language, int position)
    {
        try
        {
            if (entryId is long stored)
            {
                _cCardDesk.CDeskChip?.LQuillTranslationInsert(cardId, stored, position);
                return;
            }

            _cCardDesk.CDeskChip?.LQuillTranslationStart(cardId, 0, headword, language, position);
        }
        catch (Exception exception)
        {
            CLedger.LLedgerFailureShow(_cCardEnvoy, _cCardSettingsPort, "Input.TranslationFailed", exception);
        }
    }

    public void CCardTranslationRemove(long cardId, long entryId)
    {
        try
        {
            _cCardDesk.CDeskChip?.LQuillTranslationRemove(cardId, entryId);
        }
        catch (Exception exception)
        {
            CLedger.LLedgerFailureShow(_cCardEnvoy, _cCardSettingsPort, "Input.TranslationFailed", exception);
        }
    }

    internal static CProspect LCardProspectRead(LTranslationOffer offer)
    {
        return new CProspect(
            offer.LTranslationOfferText,
            offer.LTranslationOfferWord,
            offer.LTranslationOfferRows.Select(CCatalog.LCatalogRowRead).ToList(),
            offer.LTranslationOfferShown,
            offer.LTranslationOfferChosen,
            offer.LTranslationOfferLanguages);
    }

    internal static CRegister LCardRegisterRead(LRegister register)
    {
        ArgumentNullException.ThrowIfNull(register);

        return new CRegister(register.LRegisterId, CFolio.CFolioStateRead(register.LRegisterName).CStateValueText);
    }

    public CProffer CCardReferenceFind(long cardId, long sentenceId, string text)
    {
        return _cCardDesk.CDeskChip?.LQuillReferenceFind(cardId, sentenceId, text) is LReferenceOffer offer
            ? LCardProfferRead(offer)
            : new CProffer(text, [], false);
    }

    internal static CProffer LCardProfferRead(LReferenceOffer offer)
    {
        return new CProffer(
            offer.LReferenceOfferText,
            offer.LReferenceOfferRows
                .Select(static row => new CProfferRow(
                    row.LReferenceRowId,
                    row.LReferenceRowLead,
                    row.LReferenceRowMark,
                    row.LReferenceRowTail,
                    row.LReferenceRowCount))
                .ToList(),
            offer.LReferenceOfferShown);
    }

    public IReadOnlyList<CCatalogReference> CCardReferenceRead()
    {
        try
        {
            return COeuvre.COeuvreReferenceRead(_cCardReferencePort.LEngineReferenceFind());
        }
        catch (Exception exception)
        {
            CLedger.LLedgerFailureShow(_cCardEnvoy, _cCardSettingsPort, "Reference.LoadFailed", exception);
            return [];
        }
    }

    internal static CTag LCardTagRead(LTag tag)
    {
        ArgumentNullException.ThrowIfNull(tag);

        return new CTag(tag.LTagId, tag.LTagText);
    }

    public CSlate CCardTagAdd(long cardId, string text, int position, bool settled)
    {
        return _cCardDesk.CDeskChip?.LQuillTagAdd(cardId, text, position, settled) is LTagOffer offer
            ? LCardSlateRead(offer)
            : new CSlate(text, [], false);
    }

    private static CSlate LCardSlateRead(LTagOffer offer)
    {
        return new CSlate(
            offer.LTagOfferText,
            offer.LTagOfferRows
                .Select(static row => new CSlateRow(row.LTagRowId, row.LTagRowLead, row.LTagRowMark, row.LTagRowTail))
                .ToList(),
            offer.LTagOfferShown);
    }

    public void CCardTagInsert(long cardId, long tagId, int position)
    {
        _cCardDesk.CDeskChip?.LQuillTagInsert(cardId, tagId, position);
    }

    public void CCardTagRemove(long cardId, long tagId)
    {
        _cCardDesk.CDeskChip?.LQuillTagRemove(cardId, tagId);
    }

    public CProffer CCardSituationAdd(long cardId, string text, int position, bool settled)
    {
        return _cCardDesk.CDeskChip?.LQuillSituationAdd(cardId, text, position, settled) is LSituationOffer offer
            ? LCardProfferRead(offer)
            : new CProffer(text, [], false);
    }

    private static CProffer LCardProfferRead(LSituationOffer offer)
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

    public void CCardSituationInsert(long cardId, long situationId, int position)
    {
        _cCardDesk.CDeskChip?.LQuillSituationInsert(cardId, situationId, position);
    }

    public void CCardSituationRemove(long cardId, long situationId)
    {
        _cCardDesk.CDeskChip?.LQuillSituationRemove(cardId, situationId);
    }

    public CProffer CCardRegisterAdd(long cardId, string text, int position, bool settled)
    {
        return _cCardDesk.CDeskChip?.LQuillRegisterAdd(cardId, text, position, settled) is LRegisterOffer offer
            ? LCardProfferRead(offer)
            : new CProffer(text, [], false);
    }

    private static CProffer LCardProfferRead(LRegisterOffer offer)
    {
        return new CProffer(
            offer.LRegisterOfferText,
            offer.LRegisterOfferRows
                .Select(static row => new CProfferRow(
                    row.LRegisterRowId, row.LRegisterRowLead, row.LRegisterRowMark, row.LRegisterRowTail))
                .ToList(),
            offer.LRegisterOfferShown);
    }

    public void CCardRegisterInsert(long cardId, long registerId, int position)
    {
        _cCardDesk.CDeskChip?.LQuillRegisterInsert(cardId, registerId, position);
    }

    public void CCardRegisterRemove(long cardId, long registerId)
    {
        _cCardDesk.CDeskChip?.LQuillRegisterRemove(cardId, registerId);
    }

    public void CCardCitationSet(long cardId, long sentenceId, string title)
    {
        try
        {
            _cCardDesk.CDeskChip?.LQuillCitationResolve(cardId, sentenceId, title);
        }
        catch (Exception exception)
        {
            CLedger.LLedgerFailureShow(_cCardEnvoy, _cCardSettingsPort, "Reference.CreateFailed", exception);
        }
    }

    public void CCardEtymologySet(string text)
    {
        _cCardDesk.CDeskEtymology?.LQuillEtymologySet(text);
    }

    public IReadOnlyList<CMentionLabel> CCardEtymologyRead()
    {
        return _cCardDesk.CDeskTenure is LTenure held
            ? CMention.LMentionLabelRead(_cCardDraftPort.LEngineEtymologyResolve(held))
            : [];
    }

    public void CCardEtymonAdd(long? entryId)
    {
        if (entryId is not long stored)
        {
            _cCardEnvoy.CEnvoyFailureShow("Refusal.TargetMissing");
            return;
        }

        _cCardDesk.CDeskEtymology?.LEtymonAdd(stored, int.MaxValue);
    }

    public void CCardEtymonRemove(long entryId)
    {
        _cCardDesk.CDeskEtymology?.LEtymonRemove(entryId);
    }

    public void CCardMentionSave(string text, int start, int length, long? entryId)
    {
        if (entryId is not long stored)
        {
            _cCardEnvoy.CEnvoyFailureShow("Refusal.TargetMissing");
            return;
        }

        CCardMentionSend(_cCardDraftPort.LEngineSpanRead(text, start, length), stored);
    }

    public void CCardMentionDelete(string text, int start, int length)
    {
        CCardMentionSend(
            _cCardDesk.CDeskTenure is LTenure held
                ? new LQuillEtymology(held).LQuillEtymologyFind(_cCardDraftPort.LEngineSpanRead(text, start, length))
                : null,
            0);
    }

    public bool CCardMentionCheck(string text, int start, int length)
    {
        return _cCardDesk.CDeskTenure is LTenure held
            && new LQuillEtymology(held).LQuillEtymologyFind(_cCardDraftPort.LEngineSpanRead(text, start, length))
                is not null;
    }

    private void CCardMentionSend(LMentionDraft? span, long entryId)
    {
        if (span is null)
        {
            return;
        }

        _cCardDesk.CDeskEtymology?.LEtymologyMentionSave(span.LMentionDraftOffset, span.LMentionDraftLength, entryId);
    }
}
