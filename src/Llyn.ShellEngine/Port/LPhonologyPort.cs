using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Llyn.Application;
using Llyn.Core;

namespace Llyn.ShellEngine;

public interface LPhonologyPort
{
    static IReadOnlyList<int> LEngineContourScale => LLanguageClerk.LLanguageContourScale;

    IReadOnlyList<LCatalogPronunciation> LEnginePronunciationFind(LVista vista);

    bool LEngineFanqieCheck(long entryId);

    IReadOnlyList<LFanqieGroup> LEngineFanqieDivide(long entryId);

    IReadOnlyList<LFanqieGroup> LEngineFanqieRead(long entryId);

    string LEngineReadingRead(long entryId, string headword);

    void LEngineFanqieRebuild(long entryId);

    void LEngineFanqieSet(long entryId, long fanqieId, int rank, bool raise);

    bool LEngineScriptCheck(long entryId);

    void LEngineScriptRebuild(long entryId);

    IReadOnlyList<LScriptGroup> LEngineScriptDivide(long entryId);

    IReadOnlyList<LScriptGroup> LEngineScriptRead(long entryId);

    bool LEngineStyleCheck(string language);

    bool LEngineReflexCheck(long entryId);

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

    IReadOnlyList<LContour> LEngineContourRead(string language, string ipa);

    bool LEngineSilentCheck(string language);

    bool LEngineRespellingCheck(string language);

    LAccentSheet LEngineAccentRead(LEntryDraft draft);

    Task<LAccentSheet> LEngineAccentLoad(
        LEntryDraft draft, Func<IReadOnlyList<LEnsignRow>, Action<string, Exception>, Action> store);

    IReadOnlyList<string> LEngineDependenceRead(string language);

    IReadOnlyList<string> LEngineParticleRead(string language);

    LSentenceOrder LEngineOrderRead(string language);

    void LEngineTallySave(bool respelled);

    static LArticulation LEngineConsonantRead()
    {
        return LPronunciationClerk.LPronunciationConsonantRead();
    }

    static LArticulation LEngineVowelRead()
    {
        return LPronunciationClerk.LPronunciationVowelRead();
    }

    static LParadigmStatus LEngineParadigmCheck(LParadigmRow row, bool pending, bool enabled)
    {
        return LParadigmClerk.LParadigmClerkCheck(row, pending, enabled);
    }

    static string? LEngineDiweiRead(bool initial, string key)
    {
        return LDiweiClerk.LDiweiKindRead(!initial, key);
    }
}
