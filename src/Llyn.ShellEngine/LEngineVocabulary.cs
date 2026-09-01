using System;
using System.Collections.Generic;
using Llyn.Core;
using Llyn.Infrastructure;

namespace Llyn.ShellEngine;

public sealed partial class LEngine
{
    public void LEngineSpeechCreate(LSpeechValue value)
    {
        ArgumentNullException.ThrowIfNull(value);
        new LSpeechArchive(_lEngineDatabase).LSpeechValueCreate(value);
    }

    public string? LEngineSpeechRead(string language, string valueId)
    {
        return new LSpeechArchive(_lEngineDatabase).LSpeechValueRead(language, valueId);
    }

    public IReadOnlyList<LSpeechValue> LEngineSpeechRead(string language)
    {
        return new LSpeechArchive(_lEngineDatabase).LSpeechValueRead(language);
    }

    public string? LEngineSpeechFind(string language, string name)
    {
        return new LSpeechArchive(_lEngineDatabase).LSpeechValueFind(language, name);
    }

    private IReadOnlyList<LSpeech> LEngineSpeechResolve(string entryId, string language, string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return [];
        }

        string typed = text.Trim();
        string? value = string.IsNullOrWhiteSpace(language)
            ? null
            : new LSpeechArchive(_lEngineDatabase).LSpeechValueFind(language, typed);

        return [value is null
            ? new LSpeech(entryId, 0, null, typed)
            : new LSpeech(entryId, 0, value)];
    }

    public void LEngineMorphologyCreate(LMorphology morphology)
    {
        ArgumentNullException.ThrowIfNull(morphology);
        new LMorphologyArchive(_lEngineDatabase).LMorphologyCreate(morphology);
    }

    public LMorphology? LEngineMorphologyRead(
        string language, string speechId, string featureId, string valueId)
    {
        return new LMorphologyArchive(_lEngineDatabase)
            .LMorphologyRead(language, speechId, featureId, valueId);
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
