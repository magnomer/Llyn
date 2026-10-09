using System;
using System.Collections.Generic;
using Llyn.Core;

namespace Llyn.Application;

public sealed class LInflectionClerk
{
    private readonly LInflectionVault _lInflectionClerkInflections;
    private readonly LLacunaVault _lInflectionClerkLacunae;
    private readonly LMorphologyVault _lInflectionClerkMorphologies;
    private readonly LSpeechVault _lInflectionClerkSpeeches;

    public LInflectionClerk(LRig rig)
    {
        ArgumentNullException.ThrowIfNull(rig);
        _lInflectionClerkInflections = rig.LRigLexicon.LRigLexiconInflections;
        _lInflectionClerkLacunae = rig.LRigLexicon.LRigLexiconLacunae;
        _lInflectionClerkMorphologies = rig.LRigLexicon.LRigLexiconMorphologies;
        _lInflectionClerkSpeeches = rig.LRigLexicon.LRigLexiconSpeeches;
    }

    public void LInflectionClerkValidate(IReadOnlyList<LInflection> inflections)
    {
        ArgumentNullException.ThrowIfNull(inflections);

        LSpeechVault speeches = _lInflectionClerkSpeeches;
        LMorphologyVault morphologies = _lInflectionClerkMorphologies;
        foreach (LInflection inflection in inflections)
        {
            if (inflection.LInflectionSpeechId is long speechId
                && speeches.LSpeechValueRead(speechId) is null)
            {
                throw new LRefusal(LRefusal.LRefusalLink);
            }

            foreach (long morphologyId in inflection.LInflectionMorphology)
            {
                if (morphologies.LMorphologyRead(morphologyId) is null)
                {
                    throw new LRefusal(LRefusal.LRefusalLink);
                }
            }
        }
    }

    public void LInflectionClerkReset(long entryId)
    {
        _lInflectionClerkLacunae.LLacunaDelete(entryId);
    }

    public void LInflectionClerkUpdate(long entryId, LEntryDraft draft, List<LRevisionDelta> changes)
    {
        ArgumentNullException.ThrowIfNull(draft);
        ArgumentNullException.ThrowIfNull(changes);

        LInflectionVault inflections = _lInflectionClerkInflections;
        IReadOnlyList<LInflection> stored = inflections.LInflectionRead(entryId);
        IReadOnlyList<LInflection> current = draft.LEntryDraftInflections;

        if (LInflectionClerkMatch(stored, current))
        {
            return;
        }

        LInflectionClerkValidate(current);
        inflections.LInflectionSet(entryId, current);
        changes.Add(new LRevisionDelta(
            entryId,
            "inflection",
            current.Count == 0 ? "delete" : stored.Count == 0 ? "create" : "update",
            null));
    }

    public static bool LInflectionClerkMatch(IReadOnlyList<LInflection> stored, IReadOnlyList<LInflection> current)
    {
        ArgumentNullException.ThrowIfNull(stored);
        ArgumentNullException.ThrowIfNull(current);

        if (stored.Count != current.Count)
        {
            return false;
        }

        for (int index = 0; index < stored.Count; index++)
        {
            if (!string.Equals(
                    stored[index].LInflectionText, current[index].LInflectionText, StringComparison.Ordinal)
                || !string.Equals(
                    stored[index].LInflectionLocal,
                    current[index].LInflectionLocal,
                    StringComparison.Ordinal)
                || stored[index].LInflectionSpeechId != current[index].LInflectionSpeechId
                || stored[index].LInflectionMorphology.Count != current[index].LInflectionMorphology.Count)
            {
                return false;
            }

            for (int place = 0; place < stored[index].LInflectionMorphology.Count; place++)
            {
                if (stored[index].LInflectionMorphology[place] != current[index].LInflectionMorphology[place])
                {
                    return false;
                }
            }
        }

        return true;
    }
}
