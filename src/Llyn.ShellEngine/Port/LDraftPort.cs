using System;
using System.Collections.Generic;
using Llyn.Core;

namespace Llyn.ShellEngine;

public interface LDraftPort
{
    LTenure LEngineTenureStart(LVista vista, long? id);

    LTenure LEngineTenureStart(string origin, LSubject subject, long? id);

    void LEngineDraftDelete(long id);

    void LEngineObserverAttach(Action<LBulletin> observer);

    void LEngineObserverDetach(Action<LBulletin> observer);

    IReadOnlyList<LDraft> LEngineLeftoverRead();

    void LEngineLeftoverSweep();

    LCourt LEngineCourtStart(long ownerId, string origin, string headword, string language);

    LCourt? LEngineCourtFind(long ownerId, long targetId);

    void LEngineCourtDelete(long linkId);

    IReadOnlyList<LVistaRow> LEngineProspectFind(string query);

    IReadOnlyList<LAuthor> LEngineBylineFind(long draft, string query, int limit);

    IReadOnlyDictionary<long, LTranslationTarget> LEngineTargetFind(long ownerId);

    LEntry? LEngineTranslationResolve(string word, long? entryId);

    IReadOnlyList<LMentionPiece> LEngineMentionDivide(string text, IReadOnlyList<LMention> mentions);

    int LEngineUnitRead(string text, int offset);

    LMentionDraft LEngineSpanRead(string text, int start, int length);

    int LEngineOffsetRead(string text, int unit);

    IReadOnlyList<long> LEngineAnchorToggle(IReadOnlyList<long> anchors, long fanqieId, bool anchored);

    bool LEngineAnchorMatch(IReadOnlyList<long> one, IReadOnlyList<long> other);

    IReadOnlyList<LAnchorRow> LEngineAnchorScan(
        IReadOnlyList<LFanqieRow> rows, IReadOnlyList<long> anchors, string language, string reflex, string tone);

    bool LEngineAnchorCheck(IReadOnlyList<LFanqieRow> rows, string headword);

    string LEngineAnchorFormat(
        IReadOnlyList<LFanqieRow> rows, IReadOnlyList<long> anchors, string headword, string separator);
}
