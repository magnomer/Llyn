using System;
using System.Collections.Generic;
using Llyn.Core;
using Llyn.Infrastructure;

namespace Llyn.ShellEngine;

public sealed partial class LEngine
{
    internal LSpeechValue LEngineSpeechCreate(LSpeechValue value)
    {
        lock (_lEngineGate)
        {
            ArgumentNullException.ThrowIfNull(value);
            return new LSpeechArchive(_lEngineDatabase).LSpeechValueCreate(value);
        }
    }

    public LSpeechValue? LEngineSpeechRead(long id)
    {
        lock (_lEngineGate)
        {
            return new LSpeechArchive(_lEngineDatabase).LSpeechValueRead(id);
        }
    }

    public IReadOnlyList<LSpeechValue> LEngineSpeechRead(string language)
    {
        lock (_lEngineGate)
        {
            return new LSpeechArchive(_lEngineDatabase).LSpeechValueRead(language);
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
            LSpeechArchive values = new(_lEngineDatabase);

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
            return new LSpeechArchive(_lEngineDatabase).LSpeechValueFind(language, name);
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

        LSpeechArchive values = new(_lEngineDatabase);
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
            return new LMorphologyArchive(_lEngineDatabase).LFeatureCreate(feature);
        }
    }

    internal LMorphology LEngineMorphologyCreate(LMorphology value)
    {
        lock (_lEngineGate)
        {
            ArgumentNullException.ThrowIfNull(value);
            return new LMorphologyArchive(_lEngineDatabase).LMorphologyCreate(value);
        }
    }

    internal IReadOnlyList<LFeature> LEngineFeatureRead(long speechValueId)
    {
        lock (_lEngineGate)
        {
            return speechValueId <= 0
                ? []
                : new LMorphologyArchive(_lEngineDatabase).LFeatureRead(speechValueId);
        }
    }

    internal LFeature? LEngineFeatureFind(long speechValueId, string name)
    {
        lock (_lEngineGate)
        {
            return speechValueId <= 0
                ? null
                : new LMorphologyArchive(_lEngineDatabase).LFeatureFind(speechValueId, name);
        }
    }

    internal LMorphology? LEngineMorphologyRead(long id)
    {
        lock (_lEngineGate)
        {
            return new LMorphologyArchive(_lEngineDatabase).LMorphologyRead(id);
        }
    }

    internal IReadOnlyList<LMorphology> LEngineMorphologyScan(long featureId)
    {
        lock (_lEngineGate)
        {
            return featureId <= 0
                ? []
                : new LMorphologyArchive(_lEngineDatabase).LMorphologyScan(featureId);
        }
    }

    internal LMorphology? LEngineMorphologyFind(long featureId, string name)
    {
        lock (_lEngineGate)
        {
            return featureId <= 0
                ? null
                : new LMorphologyArchive(_lEngineDatabase).LMorphologyFind(featureId, name);
        }
    }

    public LSentenceOrder LEngineOrderRead(string language)
    {
        lock (_lEngineGate)
        {
            return LSentenceLoader.LSentenceLoaderLoad(language);
        }
    }

    public IReadOnlyList<string> LEngineParticleRead(string language)
    {
        lock (_lEngineGate)
        {
            return string.IsNullOrWhiteSpace(language)
                ? []
                : new LSentenceArchive(_lEngineDatabase).LSentenceParticleRead(language);
        }
    }

    public IReadOnlyList<string> LEngineDependenceRead(string language)
    {
        lock (_lEngineGate)
        {
            return string.IsNullOrWhiteSpace(language)
                ? []
                : new LSentenceArchive(_lEngineDatabase).LSentenceDependenceRead(language);
        }
    }

    private void LEngineLanguageImport()
    {
        using LDatabaseSession session = _lEngineDatabase.LDatabaseSessionStart();

        LSpeechArchive speeches = new(_lEngineDatabase);
        LMorphologyArchive morphology = new(_lEngineDatabase);
        foreach (string language in LLanguageLoader.LLanguageLoaderScan())
        {
            LSpeechPack pack = LSpeechLoader.LSpeechLoaderLoad(language);

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

        session.LDatabaseSessionCommit();
    }
}
