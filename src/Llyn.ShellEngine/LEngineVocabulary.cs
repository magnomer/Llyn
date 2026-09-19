using System;
using System.Collections.Generic;
using Llyn.Core;

namespace Llyn.ShellEngine;

public sealed partial class LEngine
{
    internal LSpeechValue LEngineSpeechCreate(LSpeechValue value)
    {
        lock (_lEngineGate)
        {
            ArgumentNullException.ThrowIfNull(value);
            return _lEngineSpeeches.LSpeechValueCreate(value);
        }
    }

    public LSpeechValue? LEngineSpeechRead(long id)
    {
        lock (_lEngineGate)
        {
            return _lEngineSpeeches.LSpeechValueRead(id);
        }
    }

    public IReadOnlyList<LSpeechValue> LEngineSpeechRead(string language)
    {
        lock (_lEngineGate)
        {
            return _lEngineSpeeches.LSpeechValueRead(language);
        }
    }

    public LSpeechValue? LEngineSpeechAdd(string language, string name)
    {
        lock (_lEngineGate)
        {
            if (string.IsNullOrWhiteSpace(language) || string.IsNullOrWhiteSpace(name))
            {
                return null;
            }

            string typed = name.Trim();
            LSpeechVault values = _lEngineSpeeches;

            LSpeechValue? held = values.LSpeechValueFind(language, typed);
            if (held is not null)
            {
                return held;
            }

            int position = 0;
            foreach (LSpeechValue stored in values.LSpeechValueRead(language))
            {
                position = Math.Max(position, stored.LSpeechValuePosition + 1);
            }

            return values.LSpeechValueCreate(new LSpeechValue(0, language, 0, typed, position));
        }
    }

    internal LSpeechValue? LEngineSpeechFind(string language, string name)
    {
        lock (_lEngineGate)
        {
            return _lEngineSpeeches.LSpeechValueFind(language, name);
        }
    }

    private static IReadOnlyList<string> LEngineSpeechShow(IReadOnlyList<LSpeechDraft> drafts)
    {
        List<string> named = new(drafts.Count);
        foreach (LSpeechDraft draft in drafts)
        {
            if (draft.LSpeechDraftName.Length > 0)
            {
                named.Add(draft.LSpeechDraftName);
            }
        }

        return named;
    }

    private IReadOnlyList<LSpeech> LEngineSpeechResolve(
        long entryId, string language, IReadOnlyList<LSpeechDraft>? drafts)
    {
        if (drafts is null || drafts.Count == 0)
        {
            return [];
        }

        LSpeechVault values = _lEngineSpeeches;
        List<LSpeech> speeches = [];
        foreach (LSpeechDraft draft in drafts)
        {
            if (draft.LSpeechDraftValue > 0)
            {
                if (values.LSpeechValueRead(draft.LSpeechDraftValue) is null)
                {
                    throw new LRefusal(LRefusal.LRefusalLink);
                }

                speeches.Add(new LSpeech(entryId, speeches.Count, draft.LSpeechDraftValue));
                continue;
            }

            if (string.IsNullOrWhiteSpace(draft.LSpeechDraftCustom))
            {
                continue;
            }

            string typed = draft.LSpeechDraftCustom.Trim();
            LSpeechValue? value = string.IsNullOrWhiteSpace(language)
                ? null
                : values.LSpeechValueFind(language, typed);

            speeches.Add(value is null
                ? new LSpeech(entryId, speeches.Count, null, typed)
                : new LSpeech(entryId, speeches.Count, value.LSpeechValueId));
        }

        return speeches;
    }

    internal LFeature LEngineFeatureCreate(LFeature feature)
    {
        lock (_lEngineGate)
        {
            ArgumentNullException.ThrowIfNull(feature);
            return _lEngineMorphologies.LFeatureCreate(feature);
        }
    }

    internal LMorphology LEngineMorphologyCreate(LMorphology value)
    {
        lock (_lEngineGate)
        {
            ArgumentNullException.ThrowIfNull(value);
            return _lEngineMorphologies.LMorphologyCreate(value);
        }
    }

    internal IReadOnlyList<LFeature> LEngineFeatureRead(long speechValueId)
    {
        lock (_lEngineGate)
        {
            return speechValueId <= 0
                ? []
                : _lEngineMorphologies.LFeatureRead(speechValueId);
        }
    }

    internal LFeature? LEngineFeatureFind(long speechValueId, string name)
    {
        lock (_lEngineGate)
        {
            return speechValueId <= 0
                ? null
                : _lEngineMorphologies.LFeatureFind(speechValueId, name);
        }
    }

    internal LMorphology? LEngineMorphologyRead(long id)
    {
        lock (_lEngineGate)
        {
            return _lEngineMorphologies.LMorphologyRead(id);
        }
    }

    internal IReadOnlyList<LMorphology> LEngineMorphologyScan(long featureId)
    {
        lock (_lEngineGate)
        {
            return featureId <= 0
                ? []
                : _lEngineMorphologies.LMorphologyScan(featureId);
        }
    }

    internal LMorphology? LEngineMorphologyFind(long featureId, string name)
    {
        lock (_lEngineGate)
        {
            return featureId <= 0
                ? null
                : _lEngineMorphologies.LMorphologyFind(featureId, name);
        }
    }

    public LSentenceOrder LEngineOrderRead(string language)
    {
        lock (_lEngineGate)
        {
            return _lEngineSentences.LSentenceLoad(language);
        }
    }

    public IReadOnlyList<string> LEngineParticleRead(string language)
    {
        lock (_lEngineGate)
        {
            return string.IsNullOrWhiteSpace(language)
                ? []
                : _lEngineSentences.LSentenceParticleRead(language);
        }
    }

    public IReadOnlyList<string> LEngineDependenceRead(string language)
    {
        lock (_lEngineGate)
        {
            return string.IsNullOrWhiteSpace(language)
                ? []
                : _lEngineSentences.LSentenceDependenceRead(language);
        }
    }

    private void LEngineLanguageImport()
    {
        using LVaultSession session = _lEngineVault.LVaultSessionStart();

        LSpeechVault speeches = _lEngineSpeeches;
        LMorphologyVault morphology = _lEngineMorphologies;
        foreach (string language in _lEngineLanguageVault.LLanguageScan())
        {
            LSpeechPack pack = _lEngineSpeeches.LSpeechLoad(language);

            Dictionary<long, long> speechIds = [];
            foreach (LSpeechValue value in pack.LSpeechPackValues)
            {
                speechIds[value.LSpeechValueCode] = speeches.LSpeechValueCreate(value).LSpeechValueId;
            }

            Dictionary<long, long> featureIds = [];
            foreach (LFeature feature in pack.LSpeechPackFeatures)
            {
                if (!speechIds.TryGetValue(feature.LFeatureSpeechId, out long speechId))
                {
                    continue;
                }

                featureIds[feature.LFeatureCode] = morphology
                    .LFeatureCreate(feature with { LFeatureSpeechId = speechId })
                    .LFeatureId;
            }

            foreach (LMorphology value in pack.LSpeechPackMorphology)
            {
                if (!featureIds.TryGetValue(value.LMorphologyFeatureId, out long featureId))
                {
                    continue;
                }

                morphology.LMorphologyCreate(value with { LMorphologyFeatureId = featureId });
            }
        }

        session.LVaultSessionCommit();
    }
}
