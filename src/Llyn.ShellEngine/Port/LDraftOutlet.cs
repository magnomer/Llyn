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

    public void LEngineDraftDelete(long id) => _lDraftOutletEngine.LEngineDraft.LEngineDraftDelete(id);

    public void LEngineObserverAttach(Action<LBulletin> observer) =>
        _lDraftOutletEngine.LEngineObserverAttach(observer);

    public void LEngineObserverDetach(Action<LBulletin> observer) =>
        _lDraftOutletEngine.LEngineObserverDetach(observer);

    public IReadOnlyList<LDraft> LEngineLeftoverRead() => _lDraftOutletEngine.LEngineDraft.LEngineLeftoverRead();

    public void LEngineLeftoverSweep() => _lDraftOutletEngine.LEngineDraft.LEngineLeftoverSweep();

    public LCourt LEngineCourtStart(long ownerId, string origin, string headword, string language) =>
        _lDraftOutletEngine.LEngineRequest.LEngineCourtStart(ownerId, origin, headword, language);

    public LCourt? LEngineCourtFind(long ownerId, long targetId) =>
        _lDraftOutletEngine.LEngineRequest.LEngineCourtFind(ownerId, targetId);

    public void LEngineCourtDelete(long linkId) => _lDraftOutletEngine.LEngineRequest.LEngineCourtDelete(linkId);

    public IReadOnlyList<LVistaRow> LEngineProspectFind(string query) =>
        _lDraftOutletEngine.LEngineCard.LEngineProspectFind(query);

    public IReadOnlyList<LAuthor> LEngineBylineFind(long draft, string query, int limit) =>
        _lDraftOutletEngine.LEngineAuthor.LEngineBylineFind(draft, query, limit);

    public IReadOnlyDictionary<long, LTranslationTarget> LEngineTargetFind(long ownerId) =>
        _lDraftOutletEngine.LEngineCard.LEngineTargetFind(ownerId);

    public LEntry? LEngineTranslationResolve(string word, long? entryId) =>
        _lDraftOutletEngine.LEngineCard.LEngineTranslationResolve(word, entryId);

    public IReadOnlyList<LMentionPiece> LEngineMentionDivide(string text, IReadOnlyList<LMention> mentions) =>
        _lDraftOutletEngine.LEngineMention.LEngineMentionDivide(text, mentions);

    public int LEngineUnitRead(string text, int offset) =>
        _lDraftOutletEngine.LEngineMention.LEngineUnitRead(text, offset);

    public LMentionDraft LEngineSpanRead(string text, int start, int length) =>
        _lDraftOutletEngine.LEngineMention.LEngineSpanRead(text, start, length);

    public int LEngineOffsetRead(string text, int unit) =>
        _lDraftOutletEngine.LEngineMention.LEngineOffsetRead(text, unit);

    public IReadOnlyList<long> LEngineAnchorToggle(IReadOnlyList<long> anchors, long fanqieId, bool anchored) =>
        _lDraftOutletEngine.LEngineReflex.LEngineAnchorToggle(anchors, fanqieId, anchored);

    public bool LEngineAnchorMatch(IReadOnlyList<long> one, IReadOnlyList<long> other) =>
        _lDraftOutletEngine.LEngineReflex.LEngineAnchorMatch(one, other);
}
