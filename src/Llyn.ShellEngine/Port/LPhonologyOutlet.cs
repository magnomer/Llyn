using System;
using System.Collections.Generic;
using Llyn.Core;

namespace Llyn.ShellEngine;

public sealed class LPhonologyOutlet : LPhonologyPort
{
    private readonly LEngine _lPhonologyOutletEngine;

    public LPhonologyOutlet(LEngine engine)
    {
        ArgumentNullException.ThrowIfNull(engine);
        _lPhonologyOutletEngine = engine;
    }

    public IReadOnlyList<LCatalogPronunciation> LEnginePronunciationFind(LVista vista) =>
        _lPhonologyOutletEngine.LEnginePronunciationFind(vista);

    public bool LEngineFanqieCheck(long entryId) => _lPhonologyOutletEngine.LEngineFanqieCheck(entryId);

    public void LEngineFanqieStart(long entryId) => _lPhonologyOutletEngine.LEngineFanqieStart(entryId);

    public IReadOnlyList<LFanqieGroup> LEngineFanqieDivide(long entryId) =>
        _lPhonologyOutletEngine.LEngineFanqieDivide(entryId);

    public void LEngineFanqieRebuild(long entryId) => _lPhonologyOutletEngine.LEngineFanqieRebuild(entryId);

    public void LEngineFanqieSet(long entryId, long fanqieId, int rank) =>
        _lPhonologyOutletEngine.LEngineFanqieSet(entryId, fanqieId, rank);

    public bool LEngineScriptCheck(long entryId) => _lPhonologyOutletEngine.LEngineScriptCheck(entryId);

    public void LEngineScriptStart(long entryId) => _lPhonologyOutletEngine.LEngineScriptStart(entryId);

    public void LEngineScriptRebuild(long entryId) => _lPhonologyOutletEngine.LEngineScriptRebuild(entryId);

    public IReadOnlyList<LScriptGroup> LEngineScriptDivide(long entryId) =>
        _lPhonologyOutletEngine.LEngineScriptDivide(entryId);

    public bool LEngineStyleCheck(string language) => _lPhonologyOutletEngine.LEngineStyleCheck(language);

    public bool LEngineReflexCheck(long entryId) => _lPhonologyOutletEngine.LEngineReflexCheck(entryId);

    public void LEngineReflexStart(long entryId) => _lPhonologyOutletEngine.LEngineReflexStart(entryId);

    public void LEngineReflexRebuild(long entryId) => _lPhonologyOutletEngine.LEngineReflexRebuild(entryId);

    public IReadOnlyList<LReflexRule> LEngineReflexRead(string language) =>
        _lPhonologyOutletEngine.LEngineReflexRead(language);

    public bool LEngineInflectionCheck(long entryId) => _lPhonologyOutletEngine.LEngineInflectionCheck(entryId);

    public void LEngineInflectionStart(long entryId) => _lPhonologyOutletEngine.LEngineInflectionStart(entryId);

    public IReadOnlyList<LParadigmSlot> LEngineParadigmShow(long entryId) =>
        _lPhonologyOutletEngine.LEngineParadigmShow(entryId);

    public LDiwei? LEngineDiweiFind(string language, string kind, string key) =>
        _lPhonologyOutletEngine.LEngineDiweiFind(language, kind, key);

    public IReadOnlyList<LDiwei> LEngineDiweiFind(LVista vista, string language, string kind) =>
        _lPhonologyOutletEngine.LEngineDiweiFind(vista, language, kind);

    public LDiwei? LEngineDiweiRead(long? id) => _lPhonologyOutletEngine.LEngineDiweiRead(id);

    public LDiweiPage LEngineDiweiResolve(long? id, Func<string, string?> localize) =>
        _lPhonologyOutletEngine.LEngineDiweiResolve(id, localize);

    public IReadOnlyList<LVistaRow> LEngineXiaoyunFind(string language, LVista onset, LVista rime, LVista vista) =>
        _lPhonologyOutletEngine.LEngineXiaoyunFind(language, onset, rime, vista);

    public LStem? LEngineStemFind(string language, string key) =>
        _lPhonologyOutletEngine.LEngineStemFind(language, key);

    public IReadOnlyList<LStem> LEngineStemFind(LVista vista, string language) =>
        _lPhonologyOutletEngine.LEngineStemFind(vista, language);

    public string? LEngineStemFind() => _lPhonologyOutletEngine.LEngineStemFind();

    public LStem? LEngineStemRead(long? id) => _lPhonologyOutletEngine.LEngineStemRead(id);

    public LStemPage LEngineStemResolve(long? id) => _lPhonologyOutletEngine.LEngineStemResolve(id);

    public IReadOnlyList<LVistaRow> LEngineKindredFind(string language, LVista grove, LVista vista) =>
        _lPhonologyOutletEngine.LEngineKindredFind(language, grove, vista);

    public bool LEngineBookCheck(string language) => _lPhonologyOutletEngine.LEngineBookCheck(language);

    public string? LEngineBookFind() => _lPhonologyOutletEngine.LEngineBookFind();

    public bool LEnginePhonemicCheck(string language) => _lPhonologyOutletEngine.LEnginePhonemicCheck(language);

    public bool LEngineTonalCheck(string language) => _lPhonologyOutletEngine.LEngineTonalCheck(language);

    public bool LEngineSilentCheck(string language) => _lPhonologyOutletEngine.LEngineSilentCheck(language);

    public bool LEngineRespellingCheck(string language) => _lPhonologyOutletEngine.LEngineRespellingCheck(language);

    public bool LEngineFlaggedCheck(LEntryDraft draft) => _lPhonologyOutletEngine.LEngineFlaggedCheck(draft);

    public IReadOnlyList<LAnatomyTone> LEngineToneRead(string language) =>
        _lPhonologyOutletEngine.LEngineToneRead(language);

    public IReadOnlyList<string> LEngineSchemeRead(string language) =>
        _lPhonologyOutletEngine.LEngineSchemeRead(language);

    public LSpeechValue? LEngineSpeechRead(long id) => _lPhonologyOutletEngine.LEngineSpeechRead(id);

    public IReadOnlyList<LSpeechValue> LEngineSpeechRead(string language) =>
        _lPhonologyOutletEngine.LEngineSpeechRead(language);

    public LSpeechValue? LEngineSpeechAdd(string language, string name) =>
        _lPhonologyOutletEngine.LEngineSpeechAdd(language, name);

    public IReadOnlyList<string> LEngineDependenceRead(string language) =>
        _lPhonologyOutletEngine.LEngineDependenceRead(language);

    public IReadOnlyList<string> LEngineParticleRead(string language) =>
        _lPhonologyOutletEngine.LEngineParticleRead(language);

    public LSentenceOrder LEngineOrderRead(string language) => _lPhonologyOutletEngine.LEngineOrderRead(language);

    public void LEngineTallySave(bool respelled) => _lPhonologyOutletEngine.LEngineTallySave(respelled);
}
