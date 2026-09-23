using System.Collections.Generic;
using Llyn.Application;
using Llyn.Core;

namespace Llyn.ShellEngine;

public sealed partial class LEngine
{
    internal LSpeechValue LEngineSpeechCreate(LSpeechValue value)
    {
        lock (LEngineGate)
        {
            return _lEngineStaff.LEngineStaffVocabulary.LSpeechCreate(value);
        }
    }

    public LSpeechValue? LEngineSpeechRead(long id)
    {
        lock (LEngineGate)
        {
            return _lEngineStaff.LEngineStaffVocabulary.LSpeechRead(id);
        }
    }

    public IReadOnlyList<LSpeechValue> LEngineSpeechRead(string language)
    {
        lock (LEngineGate)
        {
            return _lEngineStaff.LEngineStaffVocabulary.LSpeechRead(language);
        }
    }

    public LSpeechValue? LEngineSpeechAdd(string language, string name)
    {
        lock (LEngineGate)
        {
            return _lEngineStaff.LEngineStaffVocabulary.LSpeechAdd(language, name);
        }
    }

    internal LSpeechValue? LEngineSpeechFind(string language, string name)
    {
        lock (LEngineGate)
        {
            return _lEngineStaff.LEngineStaffVocabulary.LSpeechFind(language, name);
        }
    }

    internal LFeature LEngineFeatureCreate(LFeature feature)
    {
        lock (LEngineGate)
        {
            return _lEngineStaff.LEngineStaffVocabulary.LFeatureCreate(feature);
        }
    }

    internal LMorphology LEngineMorphologyCreate(LMorphology value)
    {
        lock (LEngineGate)
        {
            return _lEngineStaff.LEngineStaffVocabulary.LMorphologyCreate(value);
        }
    }

    internal IReadOnlyList<LFeature> LEngineFeatureRead(long speechValueId)
    {
        lock (LEngineGate)
        {
            return _lEngineStaff.LEngineStaffVocabulary.LFeatureRead(speechValueId);
        }
    }

    internal LFeature? LEngineFeatureFind(long speechValueId, string name)
    {
        lock (LEngineGate)
        {
            return _lEngineStaff.LEngineStaffVocabulary.LFeatureFind(speechValueId, name);
        }
    }

    internal LMorphology? LEngineMorphologyRead(long id)
    {
        lock (LEngineGate)
        {
            return _lEngineStaff.LEngineStaffVocabulary.LMorphologyRead(id);
        }
    }

    internal IReadOnlyList<LMorphology> LEngineMorphologyScan(long featureId)
    {
        lock (LEngineGate)
        {
            return _lEngineStaff.LEngineStaffVocabulary.LMorphologyScan(featureId);
        }
    }

    internal LMorphology? LEngineMorphologyFind(long featureId, string name)
    {
        lock (LEngineGate)
        {
            return _lEngineStaff.LEngineStaffVocabulary.LMorphologyFind(featureId, name);
        }
    }

    public LSentenceOrder LEngineOrderRead(string language)
    {
        lock (LEngineGate)
        {
            return _lEngineStaff.LEngineStaffVocabulary.LSentenceOrderRead(language);
        }
    }

    public IReadOnlyList<string> LEngineParticleRead(string language)
    {
        lock (LEngineGate)
        {
            return _lEngineStaff.LEngineStaffVocabulary.LSentenceParticleRead(language);
        }
    }

    public IReadOnlyList<string> LEngineDependenceRead(string language)
    {
        lock (LEngineGate)
        {
            return _lEngineStaff.LEngineStaffVocabulary.LSentenceDependenceRead(language);
        }
    }

    private void LEngineLanguageImport()
    {
        _lEngineStaff.LEngineStaffVocabulary.LLanguageImport();
    }

    internal IReadOnlyList<LInflection> LEngineInflectionRead(long entryId)
    {
        lock (LEngineGate)
        {
            return _lEngineStaff.LEngineStaffInflection.LInflectionClerkRead(entryId);
        }
    }

    internal void LEngineInflectionSet(long entryId, IReadOnlyList<LInflection> inflections)
    {
        lock (LEngineGate)
        {
            _lEngineStaff.LEngineStaffLacuna.LLacunaClerkCancel(entryId);
            _lEngineStaff.LEngineStaffInflection.LInflectionClerkSet(entryId, inflections);
        }
    }

    internal void LEngineInflectionAppend(long entryId, IReadOnlyList<LInflection> inflections)
    {
        lock (LEngineGate)
        {
            _lEngineStaff.LEngineStaffInflection.LInflectionClerkAppend(entryId, inflections);
        }
    }

    internal void LEngineInflectionMove(long entryId, int position, int target)
    {
        lock (LEngineGate)
        {
            _lEngineStaff.LEngineStaffInflection.LInflectionClerkMove(entryId, position, target);
        }
    }

    internal void LEngineInflectionDelete(long entryId, int position)
    {
        lock (LEngineGate)
        {
            _lEngineStaff.LEngineStaffLacuna.LLacunaClerkCancel(entryId);
            _lEngineStaff.LEngineStaffInflection.LInflectionClerkDelete(entryId, position);
        }
    }

    internal IReadOnlyList<LParadigmSlot> LEngineParadigmRead(long entryId)
    {
        lock (LEngineGate)
        {
            return _lEngineStaff.LEngineStaffParadigm.LParadigmClerkRead(entryId);
        }
    }

    public IReadOnlyList<LParadigmSlot> LEngineParadigmShow(long entryId)
    {
        lock (LEngineGate)
        {
            return _lEngineStaff.LEngineStaffParadigm.LParadigmClerkShow(entryId);
        }
    }

    public bool LEngineInflectionCheck(long entryId)
    {
        return _lEngineStaff.LEngineStaffLacuna.LLacunaClerkCheck(entryId);
    }

    public void LEngineInflectionStart(long entryId)
    {
        _lEngineStaff.LEngineStaffLacuna.LLacunaClerkStart(entryId);
    }

    internal static bool LEngineParadigmMatch(LParadigm paradigm, string headword, string form)
    {
        return LParadigmClerk.LParadigmClerkMatch(paradigm, headword, form);
    }
}
