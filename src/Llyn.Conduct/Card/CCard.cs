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

    public long? CCardTranslationResolve(string word)
    {
        return _cCardDraftPort.LEngineTranslationResolve(word, _cCardDesk.CDeskStoredRead())?.LEntryId;
    }

    public string CCardTranslationAdd(long cardId, string text, int position)
    {
        try
        {
            return _cCardDesk.CDeskChip?.LQuillTranslationAdd(cardId, text, position) ?? text;
        }
        catch (Exception exception)
        {
            CLedger.LLedgerFailureShow(_cCardEnvoy, _cCardSettingsPort, "Input.TranslationFailed", exception);
            return text;
        }
    }

    public void CCardTranslationInsert(long cardId, long entryId, int position)
    {
        _cCardDesk.CDeskChip?.LQuillTranslationInsert(cardId, entryId, position);
    }

    public long CCardCourtStart(long ownerId, string origin, string headword, string language)
    {
        return _cCardDraftPort.LEngineCourtStart(ownerId, origin, headword, language).LCourtTargetId;
    }

    public void CCardCourtDelete(long ownerId, long targetId)
    {
        CCardCourtDelete(_cCardDraftPort.LEngineCourtFind(ownerId, targetId), targetId);
    }

    private void CCardCourtDelete(LCourt? link, long targetId)
    {
        if (link is null)
        {
            return;
        }

        _cCardDraftPort.LEngineCourtDelete(link.LCourtId);
        _cCardDraftPort.LEngineDraftDelete(targetId);
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
