using System;
using System.Collections.Generic;
using Llyn.Core;

namespace Llyn.ShellEngine;

public sealed class LDraftOutlet : LDraftPort
{
    private readonly LEngine _lDraftOutletEngine;

    public LDraftOutlet(LEngine engine)
    {
        ArgumentNullException.ThrowIfNull(engine);
        _lDraftOutletEngine = engine;
    }

    public LTenure LEngineTenureStart(LVista vista, long? id) =>
        _lDraftOutletEngine.LEngineTenure.LEngineTenureStart(vista, id);

    public LTenure LEngineTenureStart(string origin, LSubject subject, long? id) =>
        _lDraftOutletEngine.LEngineTenure.LEngineTenureStart(origin, subject, id);

    public LTenure LEngineOccurrenceStart(LVista vista, long? situation) =>
        _lDraftOutletEngine.LEngineTenure.LEngineOccurrenceStart(vista, situation);

    public LTenure LEngineQuotationStart(LVista vista, long? example) =>
        _lDraftOutletEngine.LEngineTenure.LEngineQuotationStart(vista, example);

    public LTenure LEngineFootnoteStart(LVista vista, long? reference) =>
        _lDraftOutletEngine.LEngineTenure.LEngineFootnoteStart(vista, reference);

    public LTenure LEngineMembershipStart(LVista vista, long? tag) =>
        _lDraftOutletEngine.LEngineTenure.LEngineMembershipStart(vista, tag);

    public LTenure LEngineCohortStart(LVista vista, long? register) =>
        _lDraftOutletEngine.LEngineTenure.LEngineCohortStart(vista, register);

    public void LEngineDraftDelete(long id) => _lDraftOutletEngine.LEngineDraft.LEngineDraftDelete(id);

    public void LEngineObserverAttach(Action<LBulletin> observer) =>
        _lDraftOutletEngine.LEngineObserverAttach(observer);

    public void LEngineObserverDetach(Action<LBulletin> observer) =>
        _lDraftOutletEngine.LEngineObserverDetach(observer);

    public void LEngineLeftoverSweep() => _lDraftOutletEngine.LEngineDraft.LEngineLeftoverSweep();

    public LCourt LEngineCourtStart(long ownerId, string origin, string headword, string language) =>
        _lDraftOutletEngine.LEngineRequest.LEngineCourtStart(ownerId, origin, headword, language);

    public LCourt? LEngineCourtFind(long ownerId, long targetId) =>
        _lDraftOutletEngine.LEngineRequest.LEngineCourtFind(ownerId, targetId);

    public void LEngineCourtDelete(long linkId) => _lDraftOutletEngine.LEngineRequest.LEngineCourtDelete(linkId);

    public LTranslationOffer LEngineTranslationFind(LTenure held, string text, string word, bool chosen) =>
        _lDraftOutletEngine.LEngineCard.LEngineTranslationFind(held, text, word, chosen);

    public IReadOnlyList<LVistaRow> LEngineProspectFind(LTenure held, string word) =>
        _lDraftOutletEngine.LEngineCard.LEngineProspectFind(held, word);

    public IReadOnlyList<LBylineRow> LEngineBylineFind(long draft, string query) =>
        _lDraftOutletEngine.LEngineAuthor.LEngineBylineFind(draft, query);

    public LTagOffer LEngineTagFind(LTenure held, long card, string text) =>
        _lDraftOutletEngine.LEngineCatalog.LEngineTagFind(held, card, text);

    public LRegisterOffer LEngineRegisterFind(LTenure held, long card, string text) =>
        _lDraftOutletEngine.LEngineCatalog.LEngineRegisterFind(held, card, text);

    public LSituationOffer LEngineSituationFind(LTenure held, long card, string text) =>
        _lDraftOutletEngine.LEngineSituation.LEngineSituationFind(held, card, text);

    public LReferenceOffer LEngineReferenceFind(LTenure held, long card, long sentence, string text) =>
        _lDraftOutletEngine.LEngineReference.LEngineReferenceFind(held, card, sentence, text);

    public long LEngineCitationResolve(LTenure held, long card, long sentence, string title)
    {
        ArgumentNullException.ThrowIfNull(held);

        return _lDraftOutletEngine.LEngineReference.LEngineCitationResolve(held.LTenureId, card, sentence, title);
    }

    public string LEngineBylineRead(string? text) => LAuthorFacade.LEngineBylineRead(text);

    public LEntry? LEngineTranslationResolve(string word, long? entryId) =>
        _lDraftOutletEngine.LEngineCard.LEngineTranslationResolve(word, entryId);

    public int LEngineUnitRead(string text, int offset) =>
        _lDraftOutletEngine.LEngineMention.LEngineUnitRead(text, offset);

    public LMentionDraft LEngineSpanRead(string text, int start, int length) =>
        _lDraftOutletEngine.LEngineMention.LEngineSpanRead(text, start, length);

    public bool LEngineSpanCheck(string text, int start, int length) =>
        _lDraftOutletEngine.LEngineMention.LEngineSpanCheck(text, start, length);

    public IReadOnlyDictionary<long, IReadOnlyList<LMentionLabel>> LEngineMentionResolve(LTenure held) =>
        _lDraftOutletEngine.LEngineMention.LEngineMentionResolve(held);

    public IReadOnlyList<LMentionLabel> LEngineMentionResolve(LTenure held, long card, long sentence) =>
        _lDraftOutletEngine.LEngineMention.LEngineMentionResolve(held, card, sentence);

    public IReadOnlyList<LMentionLabel> LEngineEtymologyResolve(LTenure held) =>
        _lDraftOutletEngine.LEngineMention.LEngineEtymologyResolve(held);

    public IReadOnlyList<(long LMeaningId, string LMeaningName, int LMeaningDepth)>? LEngineSenseRead(
        LTenure held, long card, long sentence, string text, int start, int length, string key) =>
        _lDraftOutletEngine.LEngineCard.LEngineSenseRead(held, card, sentence, text, start, length, key);

    public int LEngineOffsetRead(string text, int unit) =>
        _lDraftOutletEngine.LEngineMention.LEngineOffsetRead(text, unit);

    public bool LEngineAnchorCheck(long entryId, string headword) =>
        _lDraftOutletEngine.LEngineReflex.LEngineAnchorCheck(entryId, headword);

    public string LEngineAnchorFormat(long entryId, IReadOnlyList<long> anchors, string headword, string separator) =>
        _lDraftOutletEngine.LEngineReflex.LEngineAnchorFormat(entryId, anchors, headword, separator);
}
