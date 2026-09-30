using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Llyn.Application;
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

    public IReadOnlyList<LFanqieGroup> LEngineFanqieDivide(long entryId) =>
        _lPhonologyOutletEngine.LEngineFanqie.LEngineFanqieDivide(entryId);

    public IReadOnlyList<LFanqieGroup> LEngineFanqieRead(long entryId) =>
        _lPhonologyOutletEngine.LEngineFanqie.LEngineFanqieRead(entryId);

    public string LEngineReadingRead(long entryId, string headword) =>
        _lPhonologyOutletEngine.LEngineFanqie.LEngineReadingRead(entryId, headword);

    public void LEngineFanqieRebuild(long entryId) =>
        _lPhonologyOutletEngine.LEngineFanqie.LEngineFanqieRebuild(entryId);

    public void LEngineFanqieSet(long entryId, long fanqieId, int rank, bool raise) =>
        _lPhonologyOutletEngine.LEngineFanqie.LEngineFanqieSet(entryId, fanqieId, rank, raise);

    public bool LEngineScriptCheck(long entryId) =>
        _lPhonologyOutletEngine.LEngineLanguage.LEngineScriptCheck(entryId);

    public void LEngineScriptRebuild(long entryId) =>
        _lPhonologyOutletEngine.LEngineLanguage.LEngineScriptRebuild(entryId);

    public IReadOnlyList<LScriptGroup> LEngineScriptDivide(long entryId) =>
        _lPhonologyOutletEngine.LEngineLanguage.LEngineScriptDivide(entryId);

    public IReadOnlyList<LScriptGroup> LEngineScriptRead(long entryId) =>
        _lPhonologyOutletEngine.LEngineLanguage.LEngineScriptRead(entryId);

    public bool LEngineStyleCheck(string language) =>
        _lPhonologyOutletEngine.LEngineLanguage.LEngineStyleCheck(language);

    public bool LEngineReflexCheck(long entryId) => _lPhonologyOutletEngine.LEngineReflex.LEngineReflexCheck(entryId);

    public void LEngineReflexStart(long entryId) => _lPhonologyOutletEngine.LEngineReflex.LEngineReflexStart(entryId);

    public void LEngineReflexRebuild(long entryId) =>
        _lPhonologyOutletEngine.LEngineReflex.LEngineReflexRebuild(entryId);

    public IReadOnlyList<LReflexGuise> LEngineGuiseRead(string language, IReadOnlyList<string> reflexes) =>
        _lPhonologyOutletEngine.LEngineReflex.LEngineGuiseRead(language, reflexes);

    public bool LEngineInflectionCheck(long entryId) =>
        _lPhonologyOutletEngine.LEngineVocabulary.LEngineInflectionCheck(entryId);

    public IReadOnlyList<LParadigmRow> LEngineParadigmScan(long entryId) =>
        _lPhonologyOutletEngine.LEngineVocabulary.LEngineParadigmScan(entryId);

    public void LEngineSoundStart(long entryId) => _lPhonologyOutletEngine.LEngineLanguage.LEngineSoundStart(entryId);

    public string LEngineLanguageResolve(long entryId) =>
        _lPhonologyOutletEngine.LEngineVocabulary.LEngineLanguageResolve(entryId);

    public (long, bool)? LEngineDiweiFind(string language, string kind, string key) =>
        _lPhonologyOutletEngine.LEngineFanqie.LEngineDiweiFind(language, kind, key);

    public IReadOnlyList<LDiwei> LEngineDiweiFind(LVista vista, long? chosen, bool final) =>
        _lPhonologyOutletEngine.LEngineFanqie.LEngineDiweiFind(vista, chosen, final);

    public LDiweiPage LEngineDiweiResolve(long? id, Func<string, string?> localize) =>
        _lPhonologyOutletEngine.LEngineFanqie.LEngineDiweiResolve(id, localize);

    public long LEngineDiweiResolve(long? id, string character) =>
        _lPhonologyOutletEngine.LEngineFanqie.LEngineDiweiResolve(id, character);

    public IReadOnlyList<LVistaRow> LEngineXiaoyunFind(long? chosen, LVista onset, LVista rime, LVista vista) =>
        _lPhonologyOutletEngine.LEngineFanqie.LEngineXiaoyunFind(chosen, onset, rime, vista);

    public long? LEngineStemFind(string language, string? key) =>
        _lPhonologyOutletEngine.LEngineStem.LEngineStemFind(language, key);

    public IReadOnlyList<LStem> LEngineStemFind(LVista vista) =>
        _lPhonologyOutletEngine.LEngineStem.LEngineStemFind(vista);

    public bool LEngineStemCheck() => _lPhonologyOutletEngine.LEngineStem.LEngineStemCheck();

    public LStemPage LEngineStemResolve(long? id) => _lPhonologyOutletEngine.LEngineStem.LEngineStemResolve(id);

    public long LEngineStemResolve(long? id, string character) =>
        _lPhonologyOutletEngine.LEngineStem.LEngineStemResolve(id, character);

    public IReadOnlyList<LVistaRow> LEngineKindredFind(LVista grove, LVista vista) =>
        _lPhonologyOutletEngine.LEngineStem.LEngineKindredFind(grove, vista);

    public bool LEngineBookCheck(string language) => _lPhonologyOutletEngine.LEngineFanqie.LEngineBookCheck(language);

    public bool LEngineBookCheck() => _lPhonologyOutletEngine.LEngineFanqie.LEngineBookCheck();

    public bool LEnginePhonemicCheck(string language) =>
        _lPhonologyOutletEngine.LEngineSettings.LEnginePhonemicCheck(language);

    public IReadOnlyList<LContour> LEngineContourRead(string language, string ipa) =>
        _lPhonologyOutletEngine.LEngineLanguage.LEngineContourRead(language, ipa);

    public bool LEngineSilentCheck(string language) =>
        _lPhonologyOutletEngine.LEngineLanguage.LEngineSilentCheck(language);

    public bool LEngineRespellingCheck(string language) =>
        _lPhonologyOutletEngine.LEngineSettings.LEngineRespellingCheck(language);

    public (bool, string, string) LEngineMarkRead(string language) =>
        _lPhonologyOutletEngine.LEngineSettings.LEngineMarkRead(language);

    public LAccentSheet LEngineAccentRead(LEntryDraft draft) =>
        _lPhonologyOutletEngine.LEngineLanguage.LEngineAccentRead(draft);

    public Task<LAccentSheet> LEngineAccentLoad(
        LEntryDraft draft, Func<IReadOnlyList<LEnsignRow>, Action<string, Exception>, Action> store) =>
        _lPhonologyOutletEngine.LEngineLanguage.LEngineAccentLoad(draft, store);

    public IReadOnlyList<string> LEngineSchemeRead(string language) =>
        _lPhonologyOutletEngine.LEnginePronunciation.LEngineSchemeRead(language);

    public IReadOnlyList<string> LEngineDependenceRead(string language) =>
        _lPhonologyOutletEngine.LEngineVocabulary.LEngineDependenceRead(language);

    public IReadOnlyList<string> LEngineParticleRead(string language) =>
        _lPhonologyOutletEngine.LEngineVocabulary.LEngineParticleRead(language);

    public LSentenceOrder LEngineOrderRead(string language) =>
        _lPhonologyOutletEngine.LEngineVocabulary.LEngineOrderRead(language);

    public void LEngineTallySave(bool respelled) => _lPhonologyOutletEngine.LEngineSettings.LEngineTallySave(respelled);
}
