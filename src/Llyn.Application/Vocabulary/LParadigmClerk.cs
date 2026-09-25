using System;
using System.Collections.Generic;
using System.Linq;
using Llyn.Core;

namespace Llyn.Application;

public sealed class LParadigmClerk
{
    private readonly Dictionary<string, LSpeechPack> _lParadigmClerkPacks = new(StringComparer.Ordinal);
    private readonly LEntryVault _lParadigmClerkEntries;
    private readonly LInflectionVault _lParadigmClerkInflections;
    private readonly LLacunaVault _lParadigmClerkLacunae;
    private readonly LMorphologyVault _lParadigmClerkMorphologies;
    private readonly LSpeechVault _lParadigmClerkSpeeches;

    public LParadigmClerk(LRig rig)
    {
        ArgumentNullException.ThrowIfNull(rig);
        _lParadigmClerkEntries = rig.LRigEntries;
        _lParadigmClerkInflections = rig.LRigInflections;
        _lParadigmClerkLacunae = rig.LRigLacunae;
        _lParadigmClerkMorphologies = rig.LRigMorphologies;
        _lParadigmClerkSpeeches = rig.LRigSpeeches;
    }

    public IReadOnlyList<LParadigmSlot> LParadigmClerkRead(LEntry entry)
    {
        ArgumentNullException.ThrowIfNull(entry);

        if (!_lParadigmClerkPacks.TryGetValue(entry.LEntryLanguage, out LSpeechPack? pack))
        {
            pack = _lParadigmClerkSpeeches.LSpeechLoad(entry.LEntryLanguage);
            _lParadigmClerkPacks[entry.LEntryLanguage] = pack;
        }

        IReadOnlyList<LInflection> stored = _lParadigmClerkInflections.LInflectionRead(entry.LEntryId);
        HashSet<long> missed = [];
        foreach (LLacuna lacuna in _lParadigmClerkLacunae.LLacunaRead(entry.LEntryId))
        {
            if (lacuna.LLacunaMorphologyId is long morphologyId)
            {
                missed.Add(morphologyId);
            }
        }

        List<LParadigmSlot> slots = [];
        foreach (LSpeech speech in _lParadigmClerkEntries.LEntrySpeechRead(entry.LEntryId))
        {
            slots.AddRange(LParadigmClerkResolve(speech, pack, stored, missed));
        }

        return slots;
    }

    public IReadOnlyList<LParadigmSlot> LParadigmClerkShow(long entryId)
    {
        if (entryId <= 0)
        {
            return [];
        }

        LEntry? entry = _lParadigmClerkEntries.LEntryRead(entryId);
        if (entry is null)
        {
            return [];
        }

        List<LParadigmSlot> shown = [];
        foreach (LParadigmSlot slot in LParadigmClerkRead(entry))
        {
            if (slot.LParadigmSlotState == LState.LStateSpecified
                && slot.LParadigmSlotInflection is { LInflectionRegular: true })
            {
                continue;
            }

            shown.Add(slot);
        }

        return shown;
    }

    public void LParadigmClerkUpdate(long entryId)
    {
        LEntry? entry = _lParadigmClerkEntries.LEntryRead(entryId);
        if (entry is not null)
        {
            LParadigmClerkUpdate(entry);
        }
    }

    public void LParadigmClerkUpdate(LEntry entry)
    {
        ArgumentNullException.ThrowIfNull(entry);

        if (string.IsNullOrWhiteSpace(entry.LEntryLanguage))
        {
            return;
        }

        LInflectionVault inflections = _lParadigmClerkInflections;
        foreach (LParadigmSlot slot in LParadigmClerkRead(entry))
        {
            if (slot.LParadigmSlotInflection is not LInflection inflection)
            {
                continue;
            }

            bool regular = LParadigmClerkMatch(
                slot.LParadigmSlotParadigm, entry.LEntryHeadword, inflection.LInflectionText);
            if (regular != inflection.LInflectionRegular)
            {
                inflections.LInflectionRegularSave(inflection.LInflectionId, regular);
            }
        }
    }

    private static IReadOnlyList<LParadigm> LParadigmClerkScan(LSpeechPack pack, long speechCode)
    {
        List<long> chain = [];
        long code = speechCode;
        while (code > 0 && chain.Count < pack.LSpeechPackValues.Count && !chain.Contains(code))
        {
            chain.Add(code);
            long parent = 0;
            foreach (LSpeechValue value in pack.LSpeechPackValues)
            {
                if (value.LSpeechValueCode == code)
                {
                    parent = value.LSpeechValueParent;
                    break;
                }
            }

            code = parent;
        }

        List<LParadigm> paradigms = [];
        foreach (long link in chain)
        {
            foreach (LParadigm paradigm in pack.LSpeechPackParadigms)
            {
                if (paradigm.LParadigmSpeechCode == link
                    && !paradigm.LParadigmExcept.Any(chain.Contains))
                {
                    paradigms.Add(paradigm);
                }
            }
        }

        return paradigms;
    }

    private IReadOnlyList<LParadigmSlot> LParadigmClerkResolve(
        LSpeech speech,
        LSpeechPack pack,
        IReadOnlyList<LInflection> stored,
        HashSet<long> missed)
    {
        if (speech.LSpeechValueId is not long speechId)
        {
            return [];
        }

        LSpeechValue? value = _lParadigmClerkSpeeches.LSpeechValueRead(speechId);
        if (value is null)
        {
            return [];
        }

        LMorphologyVault morphologies = _lParadigmClerkMorphologies;
        HashSet<long> taken = [];
        List<LParadigmSlot> slots = [];
        foreach (LParadigm paradigm in LParadigmClerkScan(pack, value.LSpeechValueCode))
        {
            foreach (long code in paradigm.LParadigmMorphology)
            {
                LMorphology? morphology = LMorphologyResolve(
                    pack, morphologies, value.LSpeechValueLanguage, code);
                if (morphology is null || !taken.Add(morphology.LMorphologyId))
                {
                    continue;
                }

                LInflection? inflection = null;
                foreach (LInflection candidate in stored)
                {
                    if (candidate.LInflectionMorphology.Contains(morphology.LMorphologyId)
                        && (candidate.LInflectionSpeechId is null || candidate.LInflectionSpeechId == speechId))
                    {
                        inflection = candidate;
                        break;
                    }
                }

                LState state = LState.LStateSpecified;
                if (inflection is null)
                {
                    state = missed.Contains(morphology.LMorphologyId)
                        ? LState.LStateUnknown
                        : LState.LStateUnspecified;
                }

                slots.Add(new LParadigmSlot(value, morphology, inflection, state, paradigm));
            }
        }

        return slots;
    }

    private static LMorphology? LMorphologyResolve(
        LSpeechPack pack, LMorphologyVault morphologies, string language, long code)
    {
        LMorphology? declared = pack.LSpeechPackMorphology.FirstOrDefault(row => row.LMorphologyCode == code);
        if (declared is null)
        {
            return null;
        }

        LFeature? feature = pack.LSpeechPackFeatures.FirstOrDefault(
            row => row.LFeatureCode == declared.LMorphologyFeatureId);
        if (feature is null)
        {
            return null;
        }

        return morphologies.LMorphologyCodeFind(language, feature.LFeatureSpeechId, feature.LFeatureCode, code);
    }

    public static bool LParadigmClerkMatch(LParadigm paradigm, string headword, string form)
    {
        ArgumentNullException.ThrowIfNull(paradigm);
        if (string.IsNullOrEmpty(headword) || string.IsNullOrEmpty(form))
        {
            return false;
        }

        foreach (LParadigmRule rule in paradigm.LParadigmRegular)
        {
            string? predicted = rule.LParadigmRuleResolve(headword);
            if (predicted is not null && string.Equals(predicted, form, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }

        return false;
    }
}
