using System;
using System.Collections.Generic;
using Llyn.Core;

namespace Llyn.Application;

public sealed class LVocabularyClerk
{
    private readonly LVault _lVocabularyClerkVault;
    private readonly LEntryVault _lVocabularyClerkEntries;
    private readonly LLanguageVault _lVocabularyClerkLanguages;
    private readonly LMorphologyVault _lVocabularyClerkMorphologies;
    private readonly LSentenceVault _lVocabularyClerkSentences;
    private readonly LSpeechVault _lVocabularyClerkSpeeches;

    public LVocabularyClerk(LRig rig)
    {
        ArgumentNullException.ThrowIfNull(rig);
        _lVocabularyClerkVault = rig.LRigVault;
        _lVocabularyClerkEntries = rig.LRigEntries;
        _lVocabularyClerkLanguages = rig.LRigLanguages;
        _lVocabularyClerkMorphologies = rig.LRigMorphologies;
        _lVocabularyClerkSentences = rig.LRigSentences;
        _lVocabularyClerkSpeeches = rig.LRigSpeeches;
    }

    public IReadOnlyList<LSpeechValue> LSpeechRead(string language)
    {
        return _lVocabularyClerkSpeeches.LSpeechValueRead(language);
    }

    public LSpeechValue? LSpeechAdd(string language, string name)
    {
        if (string.IsNullOrWhiteSpace(language) || string.IsNullOrWhiteSpace(name))
        {
            return null;
        }

        string typed = name.Trim();
        LSpeechVault values = _lVocabularyClerkSpeeches;

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

    public static IReadOnlyList<string> LSpeechShow(IReadOnlyList<LSpeechDraft> drafts)
    {
        ArgumentNullException.ThrowIfNull(drafts);

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

    public IReadOnlyList<LSpeech> LSpeechResolve(string language, IReadOnlyList<LSpeechDraft>? drafts)
    {
        if (drafts is null || drafts.Count == 0)
        {
            return [];
        }

        LSpeechVault values = _lVocabularyClerkSpeeches;
        List<LSpeech> speeches = [];
        foreach (LSpeechDraft draft in drafts)
        {
            if (draft.LSpeechDraftValue > 0)
            {
                if (values.LSpeechValueRead(draft.LSpeechDraftValue) is null)
                {
                    throw new LRefusal(LRefusal.LRefusalLink);
                }

                speeches.Add(new LSpeech(draft.LSpeechDraftValue));
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
                ? new LSpeech(null, typed)
                : new LSpeech(value.LSpeechValueId));
        }

        return speeches;
    }

    public void LSpeechUpdate(long entryId, LEntryDraft draft, List<LRevisionDelta> changes)
    {
        ArgumentNullException.ThrowIfNull(draft);
        ArgumentNullException.ThrowIfNull(changes);

        LEntryVault entries = _lVocabularyClerkEntries;
        IReadOnlyList<LSpeech> stored = entries.LEntrySpeechRead(entryId);
        IReadOnlyList<LSpeech> current = LSpeechResolve(draft.LEntryDraftLanguage, draft.LEntryDraftSpeeches);

        if (LSpeechMatch(stored, current))
        {
            return;
        }

        entries.LEntrySpeechSet(entryId, current);
        changes.Add(new LRevisionDelta(
            entryId,
            "speech",
            current.Count == 0 ? "delete" : stored.Count == 0 ? "create" : "update",
            LSpeechFormat(current)));
    }

    public string LSpeechFormat(IReadOnlyList<LSpeech> speeches)
    {
        ArgumentNullException.ThrowIfNull(speeches);

        LSpeechVault values = _lVocabularyClerkSpeeches;
        List<string> names = new(speeches.Count);
        foreach (LSpeech speech in speeches)
        {
            names.Add(speech.LSpeechCustom
                ?? values.LSpeechValueRead(speech.LSpeechValueId ?? 0)?.LSpeechValueName
                ?? string.Empty);
        }

        return string.Join(", ", names);
    }

    public static bool LSpeechMatch(IReadOnlyList<LSpeech> one, IReadOnlyList<LSpeech> other)
    {
        ArgumentNullException.ThrowIfNull(one);
        ArgumentNullException.ThrowIfNull(other);

        if (one.Count != other.Count)
        {
            return false;
        }

        for (int index = 0; index < one.Count; index++)
        {
            if (one[index].LSpeechValueId != other[index].LSpeechValueId ||
                !string.Equals(one[index].LSpeechCustom, other[index].LSpeechCustom, StringComparison.Ordinal))
            {
                return false;
            }
        }

        return true;
    }

    public LSentenceOrder LSentenceOrderRead(string language)
    {
        return _lVocabularyClerkSentences.LSentenceLoad(language);
    }

    public IReadOnlyList<string> LSentenceParticleRead(string language)
    {
        return string.IsNullOrWhiteSpace(language)
            ? []
            : _lVocabularyClerkSentences.LSentenceParticleRead(language);
    }

    public IReadOnlyList<string> LSentenceDependenceRead(string language)
    {
        return string.IsNullOrWhiteSpace(language)
            ? []
            : _lVocabularyClerkSentences.LSentenceDependenceRead(language);
    }

    public void LLanguageImport()
    {
        using LVaultSession session = _lVocabularyClerkVault.LVaultSessionStart();

        LSpeechVault speeches = _lVocabularyClerkSpeeches;
        LMorphologyVault morphology = _lVocabularyClerkMorphologies;
        foreach (string language in _lVocabularyClerkLanguages.LLanguageScan())
        {
            LSpeechPack pack = speeches.LSpeechLoad(language);

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
