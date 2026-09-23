using System;
using System.Collections.Generic;
using Llyn.Core;

namespace Llyn.ShellEngine;

public interface LPhonologyPort
{
    IReadOnlyList<LCatalogPronunciation> LEnginePronunciationFind(LVista vista);

    bool LEngineFanqieCheck(long entryId);

    void LEngineFanqieStart(long entryId);

    IReadOnlyList<LFanqieGroup> LEngineFanqieDivide(long entryId);

    void LEngineFanqieRebuild(long entryId);

    void LEngineFanqieSet(long entryId, long fanqieId, int rank);

    bool LEngineScriptCheck(long entryId);

    void LEngineScriptStart(long entryId);

    void LEngineScriptRebuild(long entryId);

    IReadOnlyList<LScriptGroup> LEngineScriptDivide(long entryId);

    bool LEngineStyleCheck(string language);

    bool LEngineReflexCheck(long entryId);

    void LEngineReflexStart(long entryId);

    void LEngineReflexRebuild(long entryId);

    IReadOnlyList<LReflexRule> LEngineReflexRead(string language);

    bool LEngineInflectionCheck(long entryId);

    void LEngineInflectionStart(long entryId);

    IReadOnlyList<LParadigmSlot> LEngineParadigmShow(long entryId);

    LDiwei? LEngineDiweiFind(string language, string kind, string key);

    IReadOnlyList<LDiwei> LEngineDiweiFind(LVista vista, string language, string kind);

    LDiwei? LEngineDiweiRead(long? id);

    LDiweiPage LEngineDiweiResolve(long? id, Func<string, string?> localize);

    IReadOnlyList<LVistaRow> LEngineXiaoyunFind(string language, LVista onset, LVista rime, LVista vista);

    LStem? LEngineStemFind(string language, string key);

    IReadOnlyList<LStem> LEngineStemFind(LVista vista, string language);

    string? LEngineStemFind();

    LStem? LEngineStemRead(long? id);

    LStemPage LEngineStemResolve(long? id);

    IReadOnlyList<LVistaRow> LEngineKindredFind(string language, LVista grove, LVista vista);

    bool LEngineBookCheck(string language);

    string? LEngineBookFind();

    bool LEnginePhonemicCheck(string language);

    bool LEngineTonalCheck(string language);

    bool LEngineSilentCheck(string language);

    bool LEngineRespellingCheck(string language);

    bool LEngineFlaggedCheck(LEntryDraft draft);

    IReadOnlyList<string> LEngineSchemeRead(string language);

    LSpeechValue? LEngineSpeechRead(long id);

    IReadOnlyList<LSpeechValue> LEngineSpeechRead(string language);

    LSpeechValue? LEngineSpeechAdd(string language, string name);

    IReadOnlyList<string> LEngineDependenceRead(string language);

    IReadOnlyList<string> LEngineParticleRead(string language);

    LSentenceOrder LEngineOrderRead(string language);

    void LEngineTallySave(bool respelled);
}
