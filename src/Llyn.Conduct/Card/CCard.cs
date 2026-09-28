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

    internal CCard(CDesk desk, LDraftPort drafts, LEntryPort entries)
    {
        ArgumentNullException.ThrowIfNull(desk);
        ArgumentNullException.ThrowIfNull(drafts);
        ArgumentNullException.ThrowIfNull(entries);

        _cCardDesk = desk;
        _cCardDraftPort = drafts;
        _cCardEntryPort = entries;
    }

    public IReadOnlyList<CVistaRow> CCardProspectFind(string word)
    {
        return _cCardDraftPort.LEngineProspectFind(word).Select(CPanel.CPanelRowRead).ToList();
    }

    public long? CCardTranslationResolve(string word, long? entryId)
    {
        return _cCardDraftPort.LEngineTranslationResolve(word, entryId)?.LEntryId;
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

    public IReadOnlyList<CRegister> CCardRegisterFind(string word, string language)
    {
        return _cCardEntryPort.LEngineRegisterFind(word, language)
            .Select(static row => new CRegister(row.LRegisterId, row.LRegisterName.LStateValueShown ?? string.Empty))
            .ToList();
    }

    public IReadOnlyList<CCatalogSituation> CCardSituationFind(string word)
    {
        return _cCardEntryPort.LEngineSituationFind(word, LCatalogOrder.LCatalogOrderUsage)
            .Select(CAtlas.CAtlasRowRead)
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

    public IReadOnlyList<CTag> CCardTagFind(string word)
    {
        return _cCardEntryPort.LEngineTagFind(word, LCatalogOrder.LCatalogOrderUsage)
            .Select(static row => new CTag(row.LTagId, row.LTagText))
            .ToList();
    }

    public void CCardTagAdd(long cardId, string text, int position)
    {
        _cCardDesk.CDeskQuill?.LQuillTagAdd(cardId, text, position);
    }

    public void CCardTagInsert(long cardId, long tagId, int position)
    {
        _cCardDesk.CDeskQuill?.LQuillTagInsert(cardId, tagId, position);
    }

    public void CCardTagRemove(long cardId, long tagId)
    {
        _cCardDesk.CDeskQuill?.LQuillTagRemove(cardId, tagId);
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
