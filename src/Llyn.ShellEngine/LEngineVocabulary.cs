using System;
using System.Collections.Generic;
using System.Text;
using Llyn.Core;
using Llyn.Infrastructure;

namespace Llyn.ShellEngine;

public sealed partial class LEngine
{
    public void LEngineSpeechCreate(LSpeechValue value)
    {
        lock (_lEngineGate)
        {
            ArgumentNullException.ThrowIfNull(value);
            new LSpeechArchive(_lEngineDatabase).LSpeechValueCreate(value);
        }
    }

    public string? LEngineSpeechRead(string language, string valueId)
    {
        lock (_lEngineGate)
        {
            return new LSpeechArchive(_lEngineDatabase).LSpeechValueRead(language, valueId);
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

            IReadOnlyList<LSpeechValue> stored = values.LSpeechValueRead(language);
            HashSet<string> taken = new(StringComparer.Ordinal);
            int position = 0;
            foreach (LSpeechValue held in stored)
            {
                if (string.Equals(held.LSpeechValueName.Trim(), typed, StringComparison.OrdinalIgnoreCase))
                {
                    return held;
                }

                taken.Add(held.LSpeechValueId);
                position = Math.Max(position, held.LSpeechValuePosition);
            }

            string id = LEngineSpeechNormalize(typed);
            if (taken.Contains(id))
            {
                id = $"{id}_{position + 1}";
            }

            LSpeechValue created = new(language, id, typed, position + 1);
            values.LSpeechValueCreate(created);
            return created;
        }
    }

    public string? LEngineSpeechFind(string language, string name)
    {
        lock (_lEngineGate)
        {
            return new LSpeechArchive(_lEngineDatabase).LSpeechValueFind(language, name);
        }
    }

    private static string LEngineSpeechNormalize(string name)
    {
        StringBuilder built = new(name.Length + 7);
        built.Append("custom_");
        foreach (char letter in name)
        {
            built.Append(char.IsLetterOrDigit(letter) ? char.ToLowerInvariant(letter) : '_');
        }

        return built.ToString();
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
        string entryId, string language, IReadOnlyList<LSpeechDraft>? drafts)
    {
        if (drafts is null || drafts.Count == 0)
        {
            return [];
        }

        LSpeechArchive values = new(_lEngineDatabase);
        List<LSpeech> speeches = [];
        foreach (LSpeechDraft draft in drafts)
        {
            if (draft.LSpeechDraftValue is string declared && declared.Length > 0)
            {
                speeches.Add(new LSpeech(entryId, speeches.Count, declared));
                continue;
            }

            if (string.IsNullOrWhiteSpace(draft.LSpeechDraftCustom))
            {
                continue;
            }

            string typed = draft.LSpeechDraftCustom.Trim();
            string? value = string.IsNullOrWhiteSpace(language)
                ? null
                : values.LSpeechValueFind(language, typed);

            speeches.Add(value is null
                ? new LSpeech(entryId, speeches.Count, null, typed)
                : new LSpeech(entryId, speeches.Count, value));
        }

        return speeches;
    }

    public void LEngineMorphologyCreate(LMorphology morphology)
    {
        lock (_lEngineGate)
        {
            ArgumentNullException.ThrowIfNull(morphology);
            new LMorphologyArchive(_lEngineDatabase).LMorphologyCreate(morphology);
        }
    }

    public LMorphology? LEngineMorphologyRead(
        string language, string speechId, string featureId, string valueId)
    {
        lock (_lEngineGate)
        {
            return new LMorphologyArchive(_lEngineDatabase)
                .LMorphologyRead(language, speechId, featureId, valueId);
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
            foreach (LSpeechValue value in pack.LSpeechPackValues)
            {
                speeches.LSpeechValueCreate(value);
            }

            foreach (LMorphology row in pack.LSpeechPackMorphology)
            {
                morphology.LMorphologyCreate(row);
            }
        }

        session.LDatabaseSessionCommit();
    }
}
