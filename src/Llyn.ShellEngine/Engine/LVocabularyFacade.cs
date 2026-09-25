using System;
using System.Collections.Generic;
using Llyn.Application;
using Llyn.Core;

namespace Llyn.ShellEngine;

internal sealed class LVocabularyFacade
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
            return LVocabularyFacadeStaff.LEngineStaffVocabulary.LSpeechRead(language);
        }
    }

    public LSpeechValue? LEngineSpeechAdd(string language, string name)
    {
        lock (_lVocabularyFacadeGate)
        {
            return LVocabularyFacadeStaff.LEngineStaffVocabulary.LSpeechAdd(language, name);
        }
    }

    public LSentenceOrder LEngineOrderRead(string language)
    {
        lock (_lVocabularyFacadeGate)
        {
            return LVocabularyFacadeStaff.LEngineStaffVocabulary.LSentenceOrderRead(language);
        }
    }

    public IReadOnlyList<string> LEngineParticleRead(string language)
    {
        lock (_lVocabularyFacadeGate)
        {
            return LVocabularyFacadeStaff.LEngineStaffVocabulary.LSentenceParticleRead(language);
        }
    }

    public IReadOnlyList<string> LEngineDependenceRead(string language)
    {
        lock (_lVocabularyFacadeGate)
        {
            return LVocabularyFacadeStaff.LEngineStaffVocabulary.LSentenceDependenceRead(language);
        }
    }

    internal void LEngineLanguageImport()
    {
        LVocabularyFacadeStaff.LEngineStaffVocabulary.LLanguageImport();
    }

    public IReadOnlyList<LParadigmSlot> LEngineParadigmShow(long entryId)
    {
        lock (_lVocabularyFacadeGate)
        {
            return LVocabularyFacadeStaff.LEngineStaffParadigm.LParadigmClerkShow(entryId);
        }
    }

    public bool LEngineInflectionCheck(long entryId)
    {
        return LVocabularyFacadeStaff.LEngineStaffLacuna.LLacunaClerkCheck(entryId);
    }

    public void LEngineInflectionStart(long entryId)
    {
        LVocabularyFacadeStaff.LEngineStaffLacuna.LLacunaClerkStart(entryId);
    }
    private LEngineStaff LVocabularyFacadeStaff => _lVocabularyFacadeEngine.LEngineStaffHeld;
}
