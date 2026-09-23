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

    public LTenure LEngineTenureStart(LVista vista, long? id) => _lDraftOutletEngine.LEngineTenureStart(vista, id);

    public LTenure LEngineTenureStart(string origin, LSubject subject, long? id) =>
        _lDraftOutletEngine.LEngineTenureStart(origin, subject, id);

    public void LEngineDraftDelete(long id) => _lDraftOutletEngine.LEngineDraftDelete(id);

    public void LEngineObserverAttach(Action<LBulletin> observer) =>
        _lDraftOutletEngine.LEngineObserverAttach(observer);

    public void LEngineObserverDetach(Action<LBulletin> observer) =>
        _lDraftOutletEngine.LEngineObserverDetach(observer);

    public IReadOnlyList<LDraft> LEngineLeftoverRead() => _lDraftOutletEngine.LEngineLeftoverRead();

    public void LEngineLeftoverSweep() => _lDraftOutletEngine.LEngineLeftoverSweep();

    public LCourt LEngineCourtStart(long ownerId, string origin, string headword, string language) =>
        _lDraftOutletEngine.LEngineCourtStart(ownerId, origin, headword, language);

    public LCourt? LEngineCourtFind(long ownerId, long targetId) =>
        _lDraftOutletEngine.LEngineCourtFind(ownerId, targetId);

    public void LEngineCourtDelete(long linkId) => _lDraftOutletEngine.LEngineCourtDelete(linkId);

    public IReadOnlyList<LVistaRow> LEngineProspectFind(string query) => _lDraftOutletEngine.LEngineProspectFind(query);

    public IReadOnlyList<LAuthor> LEngineBylineFind(long draft, string query, int limit) =>
        _lDraftOutletEngine.LEngineBylineFind(draft, query, limit);

    public IReadOnlyDictionary<long, LTranslationTarget> LEngineTargetFind(long ownerId) =>
        _lDraftOutletEngine.LEngineTargetFind(ownerId);

    public LEntry? LEngineTranslationResolve(string word, long? entryId) =>
        _lDraftOutletEngine.LEngineTranslationResolve(word, entryId);

    public IReadOnlyList<LMentionPiece> LEngineMentionDivide(string text, IReadOnlyList<LMention> mentions) =>
        _lDraftOutletEngine.LEngineMentionDivide(text, mentions);

    public int LEngineUnitRead(string text, int offset) => _lDraftOutletEngine.LEngineUnitRead(text, offset);

    public LMentionDraft LEngineSpanRead(string text, int start, int length) =>
        _lDraftOutletEngine.LEngineSpanRead(text, start, length);

    public int LEngineOffsetRead(string text, int unit) => _lDraftOutletEngine.LEngineOffsetRead(text, unit);

    public IReadOnlyList<long> LEngineAnchorToggle(IReadOnlyList<long> anchors, long fanqieId, bool anchored) =>
        _lDraftOutletEngine.LEngineAnchorToggle(anchors, fanqieId, anchored);

    public bool LEngineAnchorMatch(IReadOnlyList<long> one, IReadOnlyList<long> other) =>
        _lDraftOutletEngine.LEngineAnchorMatch(one, other);
}
