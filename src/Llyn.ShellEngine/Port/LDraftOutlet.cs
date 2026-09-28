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

    public void LEngineLeftoverSweep() => _lDraftOutletEngine.LEngineDraft.LEngineLeftoverSweep();

    public LCourt LEngineCourtStart(long ownerId, string origin, string headword, string language) =>
        _lDraftOutletEngine.LEngineRequest.LEngineCourtStart(ownerId, origin, headword, language);

    public LCourt? LEngineCourtFind(long ownerId, long targetId) =>
        _lDraftOutletEngine.LEngineRequest.LEngineCourtFind(ownerId, targetId);

    public void LEngineCourtDelete(long linkId) => _lDraftOutletEngine.LEngineRequest.LEngineCourtDelete(linkId);

    public IReadOnlyList<LVistaRow> LEngineProspectFind(string query) =>
        _lDraftOutletEngine.LEngineCard.LEngineProspectFind(query);

    public IReadOnlyList<LAuthor> LEngineBylineFind(long draft, string query) =>
        _lDraftOutletEngine.LEngineAuthor.LEngineBylineFind(draft, query);

    public string LEngineBylineRead(string? text) => LAuthorFacade.LEngineBylineRead(text);

    public LEntry? LEngineTranslationResolve(string word, long? entryId) =>
        _lDraftOutletEngine.LEngineCard.LEngineTranslationResolve(word, entryId);

    public IReadOnlyList<LMentionPiece> LEngineMentionDivide(string text, IReadOnlyList<LMention> mentions) =>
        _lDraftOutletEngine.LEngineMention.LEngineMentionDivide(text, mentions);

    public int LEngineUnitRead(string text, int offset) =>
        _lDraftOutletEngine.LEngineMention.LEngineUnitRead(text, offset);

    public LMentionDraft LEngineSpanRead(string text, int start, int length) =>
        _lDraftOutletEngine.LEngineMention.LEngineSpanRead(text, start, length);

    public bool LEngineSpanCheck(string text, int start, int length) =>
        _lDraftOutletEngine.LEngineMention.LEngineSpanCheck(text, start, length);

    public int LEngineOffsetRead(string text, int unit) =>
        _lDraftOutletEngine.LEngineMention.LEngineOffsetRead(text, unit);

    public bool LEngineAnchorMatch(IReadOnlyList<long> one, IReadOnlyList<long> other) =>
        _lDraftOutletEngine.LEngineReflex.LEngineAnchorMatch(one, other);

    public IReadOnlyList<LAnchorRow> LEngineAnchorScan(
        long entryId, IReadOnlyList<long> anchors, string language, string reflex, string tone) =>
        _lDraftOutletEngine.LEngineReflex.LEngineAnchorScan(entryId, anchors, language, reflex, tone);

    public bool LEngineAnchorCheck(long entryId, string headword) =>
        _lDraftOutletEngine.LEngineReflex.LEngineAnchorCheck(entryId, headword);

    public string LEngineAnchorFormat(long entryId, IReadOnlyList<long> anchors, string headword, string separator) =>
        _lDraftOutletEngine.LEngineReflex.LEngineAnchorFormat(entryId, anchors, headword, separator);

    public bool LEngineAnchorCheck(IReadOnlyList<LFanqieRow> rows, string headword) =>
        _lDraftOutletEngine.LEngineReflex.LEngineAnchorCheck(rows, headword);

    public string LEngineAnchorFormat(
        IReadOnlyList<LFanqieRow> rows, IReadOnlyList<long> anchors, string headword, string separator) =>
        _lDraftOutletEngine.LEngineReflex.LEngineAnchorFormat(rows, anchors, headword, separator);
}
