using System;
using System.Collections.Generic;
using Llyn.Application;
using Llyn.Core;

namespace Llyn.ShellEngine;

public sealed class LVocabularyFacade : LParadigmPort, LSentencePort
{
    private readonly LEngine _lVocabularyFacadeEngine;
    private readonly object _lVocabularyFacadeGate;

    public LVocabularyFacade(LEngine engine)
    {
        ArgumentNullException.ThrowIfNull(engine);
        _lVocabularyFacadeEngine = engine;
        _lVocabularyFacadeGate = engine.LEngineGate;
    }

    public IReadOnlyList<LSpeechValue> LEngineSpeechRead(string language)
    {
        lock (_lVocabularyFacadeGate)
        {
            return LVocabularyFacadeStaff.LEngineStaffLanguage.LLanguageStaffVocabulary.LSpeechRead(language);
        }
    }

    public LSpeechValue? LEngineSpeechAdd(string language, string name)
    {
        lock (_lVocabularyFacadeGate)
        {
            return LVocabularyFacadeStaff.LEngineStaffLanguage.LLanguageStaffVocabulary.LSpeechAdd(language, name);
        }
    }

    public LSentenceOrder LEngineOrderRead(string language)
    {
        lock (_lVocabularyFacadeGate)
        {
            return LVocabularyFacadeStaff.LEngineStaffLanguage.LLanguageStaffVocabulary.LSentenceOrderRead(language);
        }
    }

    public IReadOnlyList<string> LEngineParticleRead(string language)
    {
        lock (_lVocabularyFacadeGate)
        {
            return LVocabularyFacadeStaff.LEngineStaffLanguage.LLanguageStaffVocabulary.LSentenceParticleRead(language);
        }
    }

    public IReadOnlyList<string> LEngineDependenceRead(string language)
    {
        lock (_lVocabularyFacadeGate)
        {
            return LVocabularyFacadeStaff.LEngineStaffLanguage.LLanguageStaffVocabulary
                .LSentenceDependenceRead(language);
        }
    }

    internal void LEngineLanguageImport()
    {
        LVocabularyFacadeStaff.LEngineStaffLanguage.LLanguageStaffVocabulary.LLanguageImport();
    }

    public IReadOnlyList<LParadigmRow> LEngineParadigmScan(long entryId)
    {
        lock (_lVocabularyFacadeGate)
        {
            return LVocabularyFacadeStaff.LEngineStaffLanguage.LLanguageStaffParadigm.LParadigmRowRead(entryId);
        }
    }

    public string LEngineLanguageResolve(long entryId)
    {
        lock (_lVocabularyFacadeGate)
        {
            return LVocabularyFacadeStaff.LEngineStaffLanguage.LLanguageStaffParadigm.LParadigmLanguageRead(entryId);
        }
    }

    public LParadigmStatus LEngineParadigmCheck(LParadigmRow row, bool pending, bool enabled)
    {
        return LParadigmClerk.LParadigmClerkCheck(row, pending, enabled);
    }

    public bool LEngineInflectionCheck(long entryId)
    {
        return LVocabularyFacadeStaff.LEngineStaffLanguage.LLanguageStaffLacuna.LLacunaClerkCheck(entryId);
    }

    public void LEngineInflectionStart(long entryId)
    {
        LVocabularyFacadeStaff.LEngineStaffLanguage.LLanguageStaffLacuna.LLacunaClerkStart(entryId);
    }
    private LEngineStaff LVocabularyFacadeStaff => _lVocabularyFacadeEngine.LEngineStaffHeld;
}
