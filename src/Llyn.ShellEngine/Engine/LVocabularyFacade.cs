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

    internal LSpeechValue LEngineSpeechCreate(LSpeechValue value)
    {
        lock (_lVocabularyFacadeGate)
        {
            return LVocabularyFacadeStaff.LEngineStaffVocabulary.LSpeechCreate(value);
        }
    }

    public LSpeechValue? LEngineSpeechRead(long id)
    {
        lock (_lVocabularyFacadeGate)
        {
            return LVocabularyFacadeStaff.LEngineStaffVocabulary.LSpeechRead(id);
        }
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

    internal LSpeechValue? LEngineSpeechFind(string language, string name)
    {
        lock (_lVocabularyFacadeGate)
        {
            return LVocabularyFacadeStaff.LEngineStaffVocabulary.LSpeechFind(language, name);
        }
    }

    internal LFeature LEngineFeatureCreate(LFeature feature)
    {
        lock (_lVocabularyFacadeGate)
        {
            return LVocabularyFacadeStaff.LEngineStaffVocabulary.LFeatureCreate(feature);
        }
    }

    internal LMorphology LEngineMorphologyCreate(LMorphology value)
    {
        lock (_lVocabularyFacadeGate)
        {
            return LVocabularyFacadeStaff.LEngineStaffVocabulary.LMorphologyCreate(value);
        }
    }

    internal IReadOnlyList<LFeature> LEngineFeatureRead(long speechValueId)
    {
        lock (_lVocabularyFacadeGate)
        {
            return LVocabularyFacadeStaff.LEngineStaffVocabulary.LFeatureRead(speechValueId);
        }
    }

    internal LFeature? LEngineFeatureFind(long speechValueId, string name)
    {
        lock (_lVocabularyFacadeGate)
        {
            return LVocabularyFacadeStaff.LEngineStaffVocabulary.LFeatureFind(speechValueId, name);
        }
    }

    internal LMorphology? LEngineMorphologyRead(long id)
    {
        lock (_lVocabularyFacadeGate)
        {
            return LVocabularyFacadeStaff.LEngineStaffVocabulary.LMorphologyRead(id);
        }
    }

    internal IReadOnlyList<LMorphology> LEngineMorphologyScan(long featureId)
    {
        lock (_lVocabularyFacadeGate)
        {
            return LVocabularyFacadeStaff.LEngineStaffVocabulary.LMorphologyScan(featureId);
        }
    }

    internal LMorphology? LEngineMorphologyFind(long featureId, string name)
    {
        lock (_lVocabularyFacadeGate)
        {
            return LVocabularyFacadeStaff.LEngineStaffVocabulary.LMorphologyFind(featureId, name);
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

    internal IReadOnlyList<LInflection> LEngineInflectionRead(long entryId)
    {
        lock (_lVocabularyFacadeGate)
        {
            return LVocabularyFacadeStaff.LEngineStaffInflection.LInflectionClerkRead(entryId);
        }
    }

    internal void LEngineInflectionSet(long entryId, IReadOnlyList<LInflection> inflections)
    {
        lock (_lVocabularyFacadeGate)
        {
            LVocabularyFacadeStaff.LEngineStaffLacuna.LLacunaClerkCancel(entryId);
            LVocabularyFacadeStaff.LEngineStaffInflection.LInflectionClerkSet(entryId, inflections);
        }
    }

    internal void LEngineInflectionAppend(long entryId, IReadOnlyList<LInflection> inflections)
    {
        lock (_lVocabularyFacadeGate)
        {
            LVocabularyFacadeStaff.LEngineStaffInflection.LInflectionClerkAppend(entryId, inflections);
        }
    }

    internal void LEngineInflectionMove(long entryId, int position, int target)
    {
        lock (_lVocabularyFacadeGate)
        {
            LVocabularyFacadeStaff.LEngineStaffInflection.LInflectionClerkMove(entryId, position, target);
        }
    }

    internal void LEngineInflectionDelete(long entryId, int position)
    {
        lock (_lVocabularyFacadeGate)
        {
            LVocabularyFacadeStaff.LEngineStaffLacuna.LLacunaClerkCancel(entryId);
            LVocabularyFacadeStaff.LEngineStaffInflection.LInflectionClerkDelete(entryId, position);
        }
    }

    internal IReadOnlyList<LParadigmSlot> LEngineParadigmRead(long entryId)
    {
        lock (_lVocabularyFacadeGate)
        {
            return LVocabularyFacadeStaff.LEngineStaffParadigm.LParadigmClerkRead(entryId);
        }
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

    internal static bool LEngineParadigmMatch(LParadigm paradigm, string headword, string form)
    {
        return LParadigmClerk.LParadigmClerkMatch(paradigm, headword, form);
    }
    private LEngineStaff LVocabularyFacadeStaff => _lVocabularyFacadeEngine.LEngineStaffHeld;
}
