using System;
using System.Collections.Generic;
using Llyn.Core;

namespace Llyn.ShellEngine;

public interface LDraftPort
{
    LTenure LEngineTenureStart(LVista vista, long? id);

    LTenure LEngineTenureStart(string origin, LSubject subject, long? id);

    LTenure LEngineOccurrenceStart(LVista vista, long? situation);

    LTenure LEngineQuotationStart(LVista vista, long? example);

    LTenure LEngineFootnoteStart(LVista vista, long? reference);

    LTenure LEngineMembershipStart(LVista vista, long? tag);

    LTenure LEngineCohortStart(LVista vista, long? register);

    void LEngineDraftDelete(long id);

    void LEngineObserverAttach(Action<LBulletin> observer);

    void LEngineObserverDetach(Action<LBulletin> observer);

    void LEngineLeftoverSweep();

    LCourt LEngineCourtStart(long ownerId, string origin, string headword, string language);

    LCourt? LEngineCourtFind(long ownerId, long targetId);

    void LEngineCourtDelete(long linkId);

    LTranslationOffer LEngineTranslationFind(LTenure held, string text, string word, bool chosen);

    IReadOnlyList<LVistaRow> LEngineProspectFind(LTenure held, string word);

    IReadOnlyList<LAuthor> LEngineBylineFind(long draft, string query);

    LTagOffer LEngineTagFind(LTenure held, long card, string text);

    LRegisterOffer LEngineRegisterFind(LTenure held, long card, string text);

    LSituationOffer LEngineSituationFind(LTenure held, long card, string text);

    LReferenceOffer LEngineReferenceFind(LTenure held, long card, long sentence, string text);

    long LEngineCitationResolve(LTenure held, long card, long sentence, string title);

    string LEngineBylineRead(string? text);

    LEntry? LEngineTranslationResolve(string word, long? entryId);

    IReadOnlyList<LMentionPiece> LEngineMentionDivide(string text, IReadOnlyList<LMention> mentions);

    int LEngineUnitRead(string text, int offset);

    LMentionDraft LEngineSpanRead(string text, int start, int length);

    bool LEngineSpanCheck(string text, int start, int length);

    IReadOnlyDictionary<long, IReadOnlyList<LMentionLabel>> LEngineMentionResolve(LTenure held);

    IReadOnlyList<LMentionLabel> LEngineMentionResolve(LTenure held, long card, long sentence);

    IReadOnlyList<(long LMeaningId, string LMeaningName, int LMeaningDepth)>? LEngineSenseRead(
        LTenure held, long card, long sentence, string text, int start, int length, string key);

    int LEngineOffsetRead(string text, int unit);

    bool LEngineAnchorCheck(long entryId, string headword);

    string LEngineAnchorFormat(long entryId, IReadOnlyList<long> anchors, string headword, string separator);
}
