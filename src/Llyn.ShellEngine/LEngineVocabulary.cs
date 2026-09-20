using System.Collections.Generic;
using Llyn.Application;
using Llyn.Core;

namespace Llyn.ShellEngine;

public sealed partial class LEngine
{
    internal LSpeechValue LEngineSpeechCreate(LSpeechValue value)
    {
        lock (_lEngineGate)
        {
            return _lEngineVocabularyClerk.LSpeechCreate(value);
        }
    }

    public LSpeechValue? LEngineSpeechRead(long id)
    {
        lock (_lEngineGate)
        {
            return _lEngineVocabularyClerk.LSpeechRead(id);
        }
    }

    public IReadOnlyList<LSpeechValue> LEngineSpeechRead(string language)
    {
        lock (_lEngineGate)
        {
            return _lEngineVocabularyClerk.LSpeechRead(language);
        }
    }

    public LSpeechValue? LEngineSpeechAdd(string language, string name)
    {
        lock (_lEngineGate)
        {
            return _lEngineVocabularyClerk.LSpeechAdd(language, name);
        }
    }

    internal LSpeechValue? LEngineSpeechFind(string language, string name)
    {
        lock (_lEngineGate)
        {
            return _lEngineVocabularyClerk.LSpeechFind(language, name);
        }
    }

    internal LFeature LEngineFeatureCreate(LFeature feature)
    {
        lock (_lEngineGate)
        {
            return _lEngineVocabularyClerk.LFeatureCreate(feature);
        }
    }

    internal LMorphology LEngineMorphologyCreate(LMorphology value)
    {
        lock (_lEngineGate)
        {
            return _lEngineVocabularyClerk.LMorphologyCreate(value);
        }
    }

    internal IReadOnlyList<LFeature> LEngineFeatureRead(long speechValueId)
    {
        lock (_lEngineGate)
        {
            return _lEngineVocabularyClerk.LFeatureRead(speechValueId);
        }
    }

    internal LFeature? LEngineFeatureFind(long speechValueId, string name)
    {
        lock (_lEngineGate)
        {
            return _lEngineVocabularyClerk.LFeatureFind(speechValueId, name);
        }
    }

    internal LMorphology? LEngineMorphologyRead(long id)
    {
        lock (_lEngineGate)
        {
            return _lEngineVocabularyClerk.LMorphologyRead(id);
        }
    }

    internal IReadOnlyList<LMorphology> LEngineMorphologyScan(long featureId)
    {
        lock (_lEngineGate)
        {
            return _lEngineVocabularyClerk.LMorphologyScan(featureId);
        }
    }

    internal LMorphology? LEngineMorphologyFind(long featureId, string name)
    {
        lock (_lEngineGate)
        {
            return _lEngineVocabularyClerk.LMorphologyFind(featureId, name);
        }
    }

    public LSentenceOrder LEngineOrderRead(string language)
    {
        lock (_lEngineGate)
        {
            return _lEngineVocabularyClerk.LSentenceOrderRead(language);
        }
    }

    public IReadOnlyList<string> LEngineParticleRead(string language)
    {
        lock (_lEngineGate)
        {
            return _lEngineVocabularyClerk.LSentenceParticleRead(language);
        }
    }

    public IReadOnlyList<string> LEngineDependenceRead(string language)
    {
        lock (_lEngineGate)
        {
            return _lEngineVocabularyClerk.LSentenceDependenceRead(language);
        }
    }

    private void LEngineLanguageImport()
    {
        _lEngineVocabularyClerk.LLanguageImport();
    }

    internal IReadOnlyList<LInflection> LEngineInflectionRead(long entryId)
    {
        lock (_lEngineGate)
        {
            return _lEngineInflectionClerk.LInflectionClerkRead(entryId);
        }
    }

    internal void LEngineInflectionSet(long entryId, IReadOnlyList<LInflection> inflections)
    {
        lock (_lEngineGate)
        {
            LEngineInflectionReset(entryId);
            _lEngineInflectionClerk.LInflectionClerkSet(entryId, inflections);
        }
    }

    internal void LEngineInflectionAppend(long entryId, IReadOnlyList<LInflection> inflections)
    {
        lock (_lEngineGate)
        {
            _lEngineInflectionClerk.LInflectionClerkAppend(entryId, inflections);
        }
    }

    internal void LEngineInflectionMove(long entryId, int position, int target)
    {
        lock (_lEngineGate)
        {
            _lEngineInflectionClerk.LInflectionClerkMove(entryId, position, target);
        }
    }

    internal void LEngineInflectionDelete(long entryId, int position)
    {
        lock (_lEngineGate)
        {
            LEngineInflectionReset(entryId);
            _lEngineInflectionClerk.LInflectionClerkDelete(entryId, position);
        }
    }

    internal IReadOnlyList<LParadigmSlot> LEngineParadigmRead(long entryId)
    {
        lock (_lEngineGate)
        {
            return _lEngineParadigmClerk.LParadigmClerkRead(entryId);
        }
    }

    public IReadOnlyList<LParadigmSlot> LEngineParadigmShow(long entryId)
    {
        lock (_lEngineGate)
        {
            return _lEngineParadigmClerk.LParadigmClerkShow(entryId);
        }
    }

    private IReadOnlyList<LParadigmSlot> LEngineParadigmRead(LEntry entry)
    {
        return _lEngineParadigmClerk.LParadigmClerkRead(entry);
    }

    private void LEngineParadigmUpdate(LEntry entry)
    {
        _lEngineParadigmClerk.LParadigmClerkUpdate(entry);
    }

    internal static bool LEngineParadigmMatch(LParadigm paradigm, string headword, string form)
    {
        return LParadigmClerk.LParadigmClerkMatch(paradigm, headword, form);
    }
}
