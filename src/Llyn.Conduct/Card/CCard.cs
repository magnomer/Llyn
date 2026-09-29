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

    private readonly LEntryPort _cCardEntryPort;

    private readonly LSettingsPort _cCardSettingsPort;

    private readonly CEnvoy _cCardEnvoy;

    internal CCard(CDesk desk, LDraftPort drafts, LEntryPort entries, LSettingsPort settings, CEnvoy envoy)
    {
        ArgumentNullException.ThrowIfNull(desk);
        ArgumentNullException.ThrowIfNull(drafts);
        ArgumentNullException.ThrowIfNull(entries);
        ArgumentNullException.ThrowIfNull(settings);
        ArgumentNullException.ThrowIfNull(envoy);

        _cCardDesk = desk;
        _cCardDraftPort = drafts;
        _cCardEntryPort = entries;
        _cCardSettingsPort = settings;
        _cCardEnvoy = envoy;
    }

    public IReadOnlyList<CVistaRow> CCardProspectFind(string word)
    {
        return _cCardDraftPort.LEngineProspectFind(word).Select(CPanel.CPanelRowRead).ToList();
    }

    public CProspect CCardTranslationAdd(long cardId, string text, int position)
    {
        ArgumentNullException.ThrowIfNull(text);

        try
        {
            return _cCardDesk.CDeskChip?.LQuillTranslationAdd(cardId, text, position) is LTranslationOffer offer
                ? LCardProspectRead(offer)
                : new CProspect(text, string.Empty, [], false, false);
        }
        catch (Exception exception)
        {
            CLedger.LLedgerFailureShow(_cCardEnvoy, _cCardSettingsPort, "Input.TranslationFailed", exception);
            return new CProspect(text, string.Empty, [], false, false);
        }
    }

    public CProspect CCardTranslationResolve(long cardId, string text, int position, bool offered)
    {
        ArgumentNullException.ThrowIfNull(text);

        if (_cCardDesk.CDeskChip is not LQuillChip chip)
        {
            return new CProspect(text, string.Empty, [], false, false);
        }

        try
        {
            return offered
                ? LCardProspectRead(chip.LQuillTranslationFind(text))
                : new CProspect(chip.LQuillTranslationResolve(cardId, text, position), string.Empty, [], false, false);
        }
        catch (Exception exception)
        {
            CLedger.LLedgerFailureShow(_cCardEnvoy, _cCardSettingsPort, "Input.TranslationFailed", exception);
            return new CProspect(text, string.Empty, [], false, false);
        }
    }

    public void CCardTranslationInsert(long cardId, long entryId, string headword, string language, int position)
    {
        try
        {
            _cCardDesk.CDeskChip?.LQuillTranslationStart(cardId, entryId, headword, language, position);
        }
        catch (Exception exception)
        {
            CLedger.LLedgerFailureShow(_cCardEnvoy, _cCardSettingsPort, "Input.TranslationFailed", exception);
        }
    }

    public void CCardTranslationRemove(long cardId, long entryId)
    {
        _cCardDesk.CDeskChip?.LQuillTranslationRemove(cardId, entryId);
    }

    private static CProspect LCardProspectRead(LTranslationOffer offer)
    {
        return new CProspect(
            offer.LTranslationOfferText,
            offer.LTranslationOfferWord,
            offer.LTranslationOfferRows.Select(CPanel.CPanelRowRead).ToList(),
            offer.LTranslationOfferShown,
            offer.LTranslationOfferChosen);
    }

    public IReadOnlyList<CRegister> CCardRegisterFind(long cardId, string word, string language)
    {
        return _cCardEntryPort.LEngineRegisterFind(_cCardDesk.CDeskTenure, cardId, word, language)
            .Select(LCardRegisterRead)
            .ToList();
    }

    internal static CRegister LCardRegisterRead(LRegister register)
    {
        ArgumentNullException.ThrowIfNull(register);

        return new CRegister(register.LRegisterId, CFolio.CFolioStateRead(register.LRegisterName).CStateValueText);
    }

    public IReadOnlyList<CCatalogSituation> CCardSituationFind(long cardId, string word)
    {
        return _cCardEntryPort.LEngineSituationFind(
                _cCardDesk.CDeskTenure, cardId, word, LCatalogOrder.LCatalogOrderUsage)
            .Select(CAtlas.LAtlasRowRead)
            .ToList();
    }

    public IReadOnlyList<CCatalogReference> CCardReferenceFind(string word)
    {
        return COeuvre.COeuvreReferenceRead(
            _cCardEntryPort.LEngineReferenceFind(word, LCatalogOrder.LCatalogOrderUsage));
    }

    public IReadOnlyList<CCatalogReference> CCardReferenceFind()
    {
        return COeuvre.COeuvreReferenceRead(_cCardEntryPort.LEngineReferenceFind());
    }

    public IReadOnlyList<CTag> CCardTagFind(long cardId, string word)
    {
        return _cCardEntryPort.LEngineTagFind(
                _cCardDesk.CDeskTenure, cardId, word, LCatalogOrder.LCatalogOrderUsage)
            .Select(LCardTagRead)
            .ToList();
    }

    internal static CTag LCardTagRead(LTag tag)
    {
        ArgumentNullException.ThrowIfNull(tag);

        return new CTag(tag.LTagId, tag.LTagText);
    }

    public string CCardTagAdd(long cardId, string text, int position, bool settled)
    {
        return _cCardDesk.CDeskChip?.LQuillTagAdd(cardId, text, position, settled) ?? text;
    }

    public void CCardTagInsert(long cardId, long tagId, int position)
    {
        _cCardDesk.CDeskChip?.LQuillTagInsert(cardId, tagId, position);
    }

    public void CCardTagRemove(long cardId, long tagId)
    {
        _cCardDesk.CDeskChip?.LQuillTagRemove(cardId, tagId);
    }

    public string CCardSituationAdd(long cardId, string text, int position, bool settled)
    {
        return _cCardDesk.CDeskChip?.LQuillSituationAdd(cardId, text, position, settled) ?? text;
    }

    public void CCardSituationInsert(long cardId, long situationId, int position)
    {
        _cCardDesk.CDeskChip?.LQuillSituationInsert(cardId, situationId, position);
    }

    public void CCardSituationRemove(long cardId, long situationId)
    {
        _cCardDesk.CDeskChip?.LQuillSituationRemove(cardId, situationId);
    }

    public string CCardRegisterAdd(long cardId, string text, int position, bool settled)
    {
        return _cCardDesk.CDeskChip?.LQuillRegisterAdd(cardId, text, position, settled) ?? text;
    }

    public void CCardRegisterInsert(long cardId, long registerId, int position)
    {
        _cCardDesk.CDeskChip?.LQuillRegisterInsert(cardId, registerId, position);
    }

    public void CCardCitationSet(long cardId, long sentenceId, string title)
    {
        long reference = _cCardEntryPort.LEngineCitationResolve(_cCardDesk.CDeskId, cardId, sentenceId, title);
        _cCardDesk.CDeskQuill?.LQuillCitationSet(cardId, sentenceId, reference);
    }

    public void CCardEtymologySet(string text)
    {
        _cCardDesk.CDeskQuill?.LQuillEtymologySet(text);
    }

    public void CCardEtymonAdd(long entryId)
    {
        _cCardDesk.CDeskQuill?.LQuillEtymonAdd(entryId, int.MaxValue);
    }

    public void CCardEtymonRemove(long entryId)
    {
        _cCardDesk.CDeskQuill?.LQuillEtymonRemove(entryId);
    }

    public void CCardMentionSave(string text, int start, int length, long entryId)
    {
        CCardMentionSend(_cCardDraftPort.LEngineSpanRead(text, start, length), entryId);
    }

    public void CCardMentionDelete(string text, int start, int length)
    {
        CCardMentionSend(
            _cCardDesk.CDeskTenure?.LTenureEtymologyFind(_cCardDraftPort.LEngineSpanRead(text, start, length)), 0);
    }

    public bool CCardMentionCheck(string text, int start, int length)
    {
        return _cCardDesk.CDeskTenure?.LTenureEtymologyFind(_cCardDraftPort.LEngineSpanRead(text, start, length))
            is not null;
    }

    private void CCardMentionSend(LMentionDraft? span, long entryId)
    {
        if (span is null)
        {
            return;
        }

        _cCardDesk.CDeskQuill?.LQuillMentionSave(span.LMentionDraftOffset, span.LMentionDraftLength, entryId);
    }

    public static string CCardOrderRead(
        CSentenceOrder? order, CStateValue particle, CStateValue dependence, CStateValue text, string mark)
    {
        ArgumentNullException.ThrowIfNull(particle);
        ArgumentNullException.ThrowIfNull(dependence);
        ArgumentNullException.ThrowIfNull(text);
        ArgumentNullException.ThrowIfNull(mark);

        bool leading = (order?.CSentenceOrderParticle ?? 0) == 0;
        string before = CCardLegibleRead(leading ? particle : dependence, mark);
        string after = CCardLegibleRead(leading ? dependence : particle, mark);
        string frame = before.Length == 0 || after.Length == 0 ? before + after : before + " " + after;
        string head = frame.Length == 0 ? string.Empty : "(+" + frame + ")";
        string written = CCardLegibleRead(text, mark);
        return head.Length == 0 || written.Length == 0 ? head + written : head + " " + written;
    }

    private static string CCardLegibleRead(CStateValue state, string mark)
    {
        return state.CStateValueLegible ? state.CStateValueText : state.CStateValueUncertain ? mark : string.Empty;
    }
}
