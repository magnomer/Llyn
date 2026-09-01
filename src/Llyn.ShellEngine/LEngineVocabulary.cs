using System;
using Llyn.Core;
using Llyn.Infrastructure;

namespace Llyn.ShellEngine;

/// <summary>
/// The controlled-vocabulary half of the engine: the parts of speech a language uses and the
/// morphology features each of them takes. Lexical rows store only stable ids — an Entry's part of
/// speech is <c>noun</c>, not the word "noun" — and what an id is called in a language is resolved
/// here, so a display name is never copied onto an entry's rows and renaming one renames it everywhere.
/// <para>
/// The vocabulary is seeded from the language packs, not from anything compiled in. Every pack on disk
/// declaring a <c>vocabulary.json</c> is written into the workspace when the engine binds to it, in one
/// session, keyed by <c>(language, value_id)</c> so a second run rewrites the same rows rather than
/// adding to them. That is why the tables are no longer empty on a fresh workspace, and why adding a
/// language is still a folder rather than a change here.
/// </para>
/// </summary>
public sealed partial class LEngine
{
    /// <summary>
    /// Adds or replaces one part-of-speech vocabulary entry, keyed by its language and value id, with
    /// its display name and display order.
    /// </summary>
    public void LEngineSpeechCreate(LSpeechValue value)
    {
        ArgumentNullException.ThrowIfNull(value);
        new LSpeechArchive(_lEngineDatabase).LSpeechValueCreate(value);
    }

    /// <summary>
    /// Resolves the display name of the part of speech <paramref name="valueId"/> names in
    /// <paramref name="language"/>, or <c>null</c> when that language declares no such part of speech.
    /// </summary>
    public string? LEngineSpeechRead(string language, string valueId)
    {
        return new LSpeechArchive(_lEngineDatabase).LSpeechValueRead(language, valueId);
    }

    /// <summary>
    /// Adds or replaces one morphology vocabulary row, keyed by its language, part of speech, feature
    /// and value, with the display names and order those carry.
    /// </summary>
    public void LEngineMorphologyCreate(LMorphology morphology)
    {
        ArgumentNullException.ThrowIfNull(morphology);
        new LMorphologyArchive(_lEngineDatabase).LMorphologyCreate(morphology);
    }

    /// <summary>
    /// Resolves one morphology vocabulary row — the feature and value display names and their order —
    /// or <c>null</c> when the language declares no such row.
    /// </summary>
    public LMorphology? LEngineMorphologyRead(
        string language, string speechId, string featureId, string valueId)
    {
        return new LMorphologyArchive(_lEngineDatabase)
            .LMorphologyRead(language, speechId, featureId, valueId);
    }

    // Every language pack's vocabulary written into the workspace, in one session: the parts of speech
    // first, then the morphology rows that name them. It runs whenever the engine binds to a workspace,
    // because a workspace may be new, may have been created by an older version with fewer packs
    // installed, or may have had a pack's wording corrected since it was last opened — and each row is
    // keyed by its ids, so writing it again updates the wording instead of duplicating the row.
    //
    // A pack that declares no vocabulary contributes nothing and is not an error: a language with no
    // morphology to declare is an ordinary language, not a broken pack.
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
