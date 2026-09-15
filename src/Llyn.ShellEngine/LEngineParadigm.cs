using System;
using System.Collections.Generic;
using System.Linq;
using Llyn.Core;
using Llyn.Infrastructure;

namespace Llyn.ShellEngine;

public sealed partial class LEngine
{
    public IReadOnlyList<LParadigmSlot> LEngineParadigmRead(long entryId)
    {
        if (entryId <= 0)
        {
            return [];
        }

        lock (_lEngineGate)
        {
            LEntryArchive entries = new(_lEngineDatabase);
            LEntry? entry = entries.LEntryRead(entryId);
            return entry is null ? [] : LEngineParadigmRead(entry);
        }
    }

    private IReadOnlyList<LParadigmSlot> LEngineParadigmRead(LEntry entry)
    {
        if (!_lEngineSpeechPacks.TryGetValue(entry.LEntryLanguage, out LSpeechPack? pack))
        {
            pack = LSpeechLoader.LSpeechLoaderLoad(entry.LEntryLanguage);
            _lEngineSpeechPacks[entry.LEntryLanguage] = pack;
        }

        IReadOnlyList<LInflection> stored = new LInflectionArchive(_lEngineDatabase).LInflectionRead(entry.LEntryId);
        _lEngineInflectionMissed.TryGetValue(entry.LEntryId, out HashSet<long>? missed);
        List<LParadigmSlot> slots = [];
        foreach (LSpeech speech in new LEntryArchive(_lEngineDatabase).LEntrySpeechRead(entry.LEntryId))
        {
            slots.AddRange(LEngineParadigmResolve(speech, pack, stored, missed));
        }

        return slots;
    }

    public IReadOnlyList<LParadigmSlot> LEngineParadigmShow(long entryId)
    {
        if (entryId <= 0)
        {
            return [];
        }

        lock (_lEngineGate)
        {
            LEntry? entry = new LEntryArchive(_lEngineDatabase).LEntryRead(entryId);
            if (entry is null)
            {
                return [];
            }

            List<LParadigmSlot> shown = [];
            foreach (LParadigmSlot slot in LEngineParadigmRead(entry))
            {
                if (slot.LParadigmSlotState == LState.LStateSpecified
                    && slot.LParadigmSlotInflection is LInflection inflection
                    && LEngineParadigmMatch(
                        slot.LParadigmSlotParadigm, entry.LEntryHeadword, inflection.LInflectionText))
                {
                    continue;
                }

                shown.Add(slot);
            }

            return shown;
        }
    }

    private static IReadOnlyList<LParadigm> LEngineParadigmScan(LSpeechPack pack, long speechCode)
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

    private IReadOnlyList<LParadigmSlot> LEngineParadigmResolve(
        LSpeech speech,
        LSpeechPack pack,
        IReadOnlyList<LInflection> stored,
        HashSet<long>? missed)
    {
        if (speech.LSpeechValueId is not long speechId)
        {
            return [];
        }

        LSpeechValue? value = new LSpeechArchive(_lEngineDatabase).LSpeechValueRead(speechId);
        if (value is null)
        {
            return [];
        }

        LMorphologyArchive morphologies = new(_lEngineDatabase);
        HashSet<long> taken = [];
        List<LParadigmSlot> slots = [];
        foreach (LParadigm paradigm in LEngineParadigmScan(pack, value.LSpeechValueCode))
        {
            foreach (long code in paradigm.LParadigmMorphology)
            {
                LMorphology? morphology = LEngineMorphologyResolve(
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
                    state = missed is not null && missed.Contains(morphology.LMorphologyId)
                        ? LState.LStateUnknown
                        : LState.LStateUnspecified;
                }

                slots.Add(new LParadigmSlot(value, morphology, inflection, state, paradigm));
            }
        }

        return slots;
    }

    private static LMorphology? LEngineMorphologyResolve(
        LSpeechPack pack, LMorphologyArchive morphologies, string language, long code)
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

    internal static bool LEngineParadigmMatch(LParadigm paradigm, string headword, string form)
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
