using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Llyn.Application;
using Llyn.Core;

namespace Llyn.ShellEngine;

public interface LPhonologyPort
{
    IReadOnlyList<LCatalogPronunciation> LEnginePronunciationFind(LVista vista);

    bool LEngineFanqieCheck(long entryId);

    IReadOnlyList<LFanqieGroup> LEngineFanqieDivide(long entryId);

    IReadOnlyList<LFanqieGroup> LEngineFanqieRead(long entryId);

    string LEngineReadingRead(long entryId, string headword);

    void LEngineFanqieRebuild(long entryId);

    void LEngineFanqieSet(long entryId, long fanqieId, int rank);

    bool LEngineScriptCheck(long entryId);

    void LEngineScriptRebuild(long entryId);

    IReadOnlyList<LScriptGroup> LEngineScriptDivide(long entryId);

    IReadOnlyList<LScriptGroup> LEngineScriptRead(long entryId);

    bool LEngineStyleCheck(string language);

    bool LEngineReflexCheck(long entryId);

    void LEngineReflexStart(long entryId);

    void LEngineReflexRebuild(long entryId);

    IReadOnlyList<LReflexGuise> LEngineGuiseRead(string language, IReadOnlyList<string> reflexes);

    bool LEngineInflectionCheck(long entryId);

    IReadOnlyList<LParadigmRow> LEngineParadigmScan(long entryId);

    void LEngineSoundStart(long entryId);

    string LEngineLanguageResolve(long entryId);

    (long, bool)? LEngineDiweiFind(string language, string kind, string key);

    IReadOnlyList<LDiwei> LEngineDiweiFind(LVista vista, long? chosen, bool final);

    LDiweiPage LEngineDiweiResolve(long? id, Func<string, string?> localize);

    long LEngineDiweiResolve(long? id, string character);

    IReadOnlyList<LVistaRow> LEngineXiaoyunFind(long? chosen, LVista onset, LVista rime, LVista vista);

    long? LEngineStemFind(string language, string? key);

    IReadOnlyList<LStem> LEngineStemFind(LVista vista);

    bool LEngineStemCheck();

    LStemPage LEngineStemResolve(long? id);

    long LEngineStemResolve(long? id, string character);

    IReadOnlyList<LVistaRow> LEngineKindredFind(LVista grove, LVista vista);

    bool LEngineBookCheck(string language);

    bool LEngineBookCheck();

    bool LEnginePhonemicCheck(string language);

    bool LEngineTonalCheck(string language);

    bool LEngineSilentCheck(string language);

    bool LEngineRespellingCheck(string language);

    (bool, string, string) LEngineMarkRead(string language);

    LAccentSheet LEngineAccentRead(LEntryDraft draft);

    Task<LAccentSheet> LEngineAccentLoad(
        LEntryDraft draft, Func<IReadOnlyList<LEnsignRow>, Action<string, Exception>, Action> store);

    IReadOnlyList<string> LEngineSchemeRead(string language);

    IReadOnlyList<string> LEngineDependenceRead(string language);

    IReadOnlyList<string> LEngineParticleRead(string language);

    LSentenceOrder LEngineOrderRead(string language);

    void LEngineTallySave(bool respelled);
}
