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
        _lPhonologyOutletEngine.LEnginePronunciation.LEnginePronunciationFind(vista);

    public bool LEngineFanqieCheck(long entryId) => _lPhonologyOutletEngine.LEngineFanqie.LEngineFanqieCheck(entryId);

    public void LEngineFanqieStart(long entryId) => _lPhonologyOutletEngine.LEngineFanqie.LEngineFanqieStart(entryId);

    public IReadOnlyList<LFanqieGroup> LEngineFanqieDivide(long entryId) =>
        _lPhonologyOutletEngine.LEngineFanqie.LEngineFanqieDivide(entryId);

    public void LEngineFanqieRebuild(long entryId) =>
        _lPhonologyOutletEngine.LEngineFanqie.LEngineFanqieRebuild(entryId);

    public void LEngineFanqieSet(long entryId, long fanqieId, int rank) =>
        _lPhonologyOutletEngine.LEngineFanqie.LEngineFanqieSet(entryId, fanqieId, rank);

    public bool LEngineScriptCheck(long entryId) =>
        _lPhonologyOutletEngine.LEngineLanguage.LEngineScriptCheck(entryId);

    public void LEngineScriptStart(long entryId) =>
        _lPhonologyOutletEngine.LEngineLanguage.LEngineScriptStart(entryId);

    public void LEngineScriptRebuild(long entryId) =>
        _lPhonologyOutletEngine.LEngineLanguage.LEngineScriptRebuild(entryId);

    public IReadOnlyList<LScriptGroup> LEngineScriptDivide(long entryId) =>
        _lPhonologyOutletEngine.LEngineLanguage.LEngineScriptDivide(entryId);

    public bool LEngineStyleCheck(string language) =>
        _lPhonologyOutletEngine.LEngineLanguage.LEngineStyleCheck(language);

    public bool LEngineReflexCheck(long entryId) => _lPhonologyOutletEngine.LEngineReflex.LEngineReflexCheck(entryId);

    public void LEngineReflexStart(long entryId) => _lPhonologyOutletEngine.LEngineReflex.LEngineReflexStart(entryId);

    public void LEngineReflexRebuild(long entryId) =>
        _lPhonologyOutletEngine.LEngineReflex.LEngineReflexRebuild(entryId);

    public IReadOnlyList<LReflexRule> LEngineReflexRead(string language) =>
        _lPhonologyOutletEngine.LEngineReflex.LEngineReflexRead(language);

    public bool LEngineInflectionCheck(long entryId) =>
        _lPhonologyOutletEngine.LEngineVocabulary.LEngineInflectionCheck(entryId);

    public void LEngineInflectionStart(long entryId) =>
        _lPhonologyOutletEngine.LEngineVocabulary.LEngineInflectionStart(entryId);

    public IReadOnlyList<LParadigmSlot> LEngineParadigmShow(long entryId) =>
        _lPhonologyOutletEngine.LEngineVocabulary.LEngineParadigmShow(entryId);

    public LDiwei? LEngineDiweiFind(string language, string kind, string key) =>
        _lPhonologyOutletEngine.LEngineFanqie.LEngineDiweiFind(language, kind, key);

    public IReadOnlyList<LDiwei> LEngineDiweiFind(LVista vista, string language, string kind) =>
        _lPhonologyOutletEngine.LEngineFanqie.LEngineDiweiFind(vista, language, kind);

    public LDiwei? LEngineDiweiRead(long? id) => _lPhonologyOutletEngine.LEngineFanqie.LEngineDiweiRead(id);

    public LDiweiPage LEngineDiweiResolve(long? id, Func<string, string?> localize) =>
        _lPhonologyOutletEngine.LEngineFanqie.LEngineDiweiResolve(id, localize);

    public IReadOnlyList<LVistaRow> LEngineXiaoyunFind(string language, LVista onset, LVista rime, LVista vista) =>
        _lPhonologyOutletEngine.LEngineFanqie.LEngineXiaoyunFind(language, onset, rime, vista);

    public LStem? LEngineStemFind(string language, string key) =>
        _lPhonologyOutletEngine.LEngineStem.LEngineStemFind(language, key);

    public IReadOnlyList<LStem> LEngineStemFind(LVista vista, string language) =>
        _lPhonologyOutletEngine.LEngineStem.LEngineStemFind(vista, language);

    public string? LEngineStemFind() => _lPhonologyOutletEngine.LEngineStem.LEngineStemFind();

    public LStem? LEngineStemRead(long? id) => _lPhonologyOutletEngine.LEngineStem.LEngineStemRead(id);

    public LStemPage LEngineStemResolve(long? id) => _lPhonologyOutletEngine.LEngineStem.LEngineStemResolve(id);

    public IReadOnlyList<LVistaRow> LEngineKindredFind(string language, LVista grove, LVista vista) =>
        _lPhonologyOutletEngine.LEngineStem.LEngineKindredFind(language, grove, vista);

    public bool LEngineBookCheck(string language) => _lPhonologyOutletEngine.LEngineFanqie.LEngineBookCheck(language);

    public string? LEngineBookFind() => _lPhonologyOutletEngine.LEngineFanqie.LEngineBookFind();

    public bool LEnginePhonemicCheck(string language) =>
        _lPhonologyOutletEngine.LEngineSettings.LEnginePhonemicCheck(language);

    public bool LEngineTonalCheck(string language) =>
        _lPhonologyOutletEngine.LEngineLanguage.LEngineTonalCheck(language);

    public bool LEngineSilentCheck(string language) =>
        _lPhonologyOutletEngine.LEngineLanguage.LEngineSilentCheck(language);

    public bool LEngineRespellingCheck(string language) =>
        _lPhonologyOutletEngine.LEngineSettings.LEngineRespellingCheck(language);

    public bool LEngineFlaggedCheck(LEntryDraft draft) =>
        _lPhonologyOutletEngine.LEngineLanguage.LEngineFlaggedCheck(draft);

    public IReadOnlyList<string> LEngineSchemeRead(string language) =>
        _lPhonologyOutletEngine.LEnginePronunciation.LEngineSchemeRead(language);

    public LSpeechValue? LEngineSpeechRead(long id) => _lPhonologyOutletEngine.LEngineVocabulary.LEngineSpeechRead(id);

    public IReadOnlyList<LSpeechValue> LEngineSpeechRead(string language) =>
        _lPhonologyOutletEngine.LEngineVocabulary.LEngineSpeechRead(language);

    public LSpeechValue? LEngineSpeechAdd(string language, string name) =>
        _lPhonologyOutletEngine.LEngineVocabulary.LEngineSpeechAdd(language, name);

    public IReadOnlyList<string> LEngineDependenceRead(string language) =>
        _lPhonologyOutletEngine.LEngineVocabulary.LEngineDependenceRead(language);

    public IReadOnlyList<string> LEngineParticleRead(string language) =>
        _lPhonologyOutletEngine.LEngineVocabulary.LEngineParticleRead(language);

    public LSentenceOrder LEngineOrderRead(string language) =>
        _lPhonologyOutletEngine.LEngineVocabulary.LEngineOrderRead(language);

    public void LEngineTallySave(bool respelled) => _lPhonologyOutletEngine.LEngineSettings.LEngineTallySave(respelled);
}
