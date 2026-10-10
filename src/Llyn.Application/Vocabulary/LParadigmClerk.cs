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
    private readonly LLanguageCache _lParadigmClerkLanguages;
    private readonly LLacunaVault _lParadigmClerkLacunae;
    private readonly LMorphologyVault _lParadigmClerkMorphologies;
    private readonly LSpeechVault _lParadigmClerkSpeeches;

    public LParadigmClerk(LRig rig, LLanguageCache languages)
    {
        ArgumentNullException.ThrowIfNull(rig);
        ArgumentNullException.ThrowIfNull(languages);
        _lParadigmClerkEntries = rig.LRigEntries;
        _lParadigmClerkInflections = rig.LRigLexicon.LRigLexiconInflections;
        _lParadigmClerkLanguages = languages;
        _lParadigmClerkLacunae = rig.LRigLexicon.LRigLexiconLacunae;
        _lParadigmClerkMorphologies = rig.LRigLexicon.LRigLexiconMorphologies;
        _lParadigmClerkSpeeches = rig.LRigLexicon.LRigLexiconSpeeches;
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
        HashSet<string> missedCells = new(StringComparer.Ordinal);
        foreach (LLacuna lacuna in _lParadigmClerkLacunae.LLacunaRead(entry.LEntryId))
        {
            if (lacuna.LLacunaCell.Length > 0)
            {
                missedCells.Add(lacuna.LLacunaCell);
            }
            else if (lacuna.LLacunaMorphologyId is long morphologyId)
            {
                missed.Add(morphologyId);
            }
        }

        List<LParadigmSlot> slots = [];
        foreach (LSpeech speech in _lParadigmClerkEntries.LEntrySpeechRead(entry.LEntryId))
        {
            slots.AddRange(LParadigmClerkResolve(speech, pack, stored, missed, missedCells));
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

    public IReadOnlyList<LParadigmRow> LParadigmRowRead(long entryId)
    {
        IReadOnlyList<LParadigmSlot> shown = LParadigmClerkShow(entryId);
        LEntry? entry = shown.Count == 0 ? null : _lParadigmClerkEntries.LEntryRead(entryId);
        if (entry is null || LParadigmClerkFind(entry)?.LLanguageLayout is not LInflectionLayout layout)
        {
            return LParadigmRow.LParadigmRowScan(shown);
        }

        return LParadigmRow.LParadigmRowScan([.. shown.Where(
            slot => slot.LParadigmSlotParadigm.LParadigmSpeechCode != layout.LInflectionLayoutPart)]);
    }

    public string LParadigmLanguageRead(long entryId)
    {
        LEntry? entry = entryId <= 0 ? null : _lParadigmClerkEntries.LEntryRead(entryId);
        return entry is null ? string.Empty : LParadigm.LParadigmLanguageRead(LParadigmClerkRead(entry));
    }

    public LParadigmView? LParadigmClerkBuild(long entryId, bool pending, bool enabled, bool held, bool custom)
    {
        LEntry? entry = entryId <= 0 ? null : _lParadigmClerkEntries.LEntryRead(entryId);
        LLanguage? language = entry is null ? null : LParadigmClerkFind(entry);
        if (entry is null || language?.LLanguageLayout is not LInflectionLayout layout)
        {
            return null;
        }

        return LParadigmView.LParadigmViewScan(
            layout,
            LParadigmClerkRead(entry),
            pending,
            enabled,
            held,
            custom,
            language.LLanguageInflection,
            entry.LEntryHeadword);
    }

    private LLanguage? LParadigmClerkFind(LEntry entry)
    {
        return string.IsNullOrWhiteSpace(entry.LEntryLanguage)
            ? null
            : _lParadigmClerkLanguages.LLanguageCacheRead(entry.LEntryLanguage);
    }

    public void LParadigmClerkUpdate(LEntry entry)
    {
        ArgumentNullException.ThrowIfNull(entry);

        if (LParadigmClerkFind(entry) is not LLanguage language)
        {
            return;
        }

        LInflectionVault inflections = _lParadigmClerkInflections;
        LInflectionBook? book = language.LLanguageInflection;
        foreach (LParadigmSlot slot in LParadigmClerkRead(entry))
        {
            if (slot.LParadigmSlotInflection is not LInflection inflection)
            {
                continue;
            }

            if (book is null)
            {
                bool matched = LParadigmClerkMatch(
                    slot.LParadigmSlotParadigm, entry.LEntryHeadword, inflection.LInflectionText);
                if (matched != inflection.LInflectionRegular)
                {
                    inflections.LInflectionRegularSave(inflection.LInflectionId, matched);
                }

                continue;
            }

            string? prediction = book.LInflectionBookResolve(entry.LEntryHeadword, slot.LParadigmSlotCodes);
            IReadOnlyList<LInflectionMark>? marks = prediction is null
                ? null
                : LInflectionDifference.LInflectionDifferenceScan(
                    book.LInflectionBookFolds, prediction, inflection.LInflectionText);
            string stamp = book.LInflectionBookStamp;
            bool regular = marks is null
                ? LParadigmClerkMatch(slot.LParadigmSlotParadigm, entry.LEntryHeadword, inflection.LInflectionText)
                : marks.Count == 0 && inflection.LInflectionText.Length > 0;
            if (regular != inflection.LInflectionRegular
                || !string.Equals(prediction, inflection.LInflectionPrediction, StringComparison.Ordinal)
                || !string.Equals(stamp, inflection.LInflectionStamp, StringComparison.Ordinal)
                || !(marks ?? []).SequenceEqual(inflection.LInflectionMarks ?? [])
                || (marks is null) != (inflection.LInflectionMarks is null))
            {
                inflections.LInflectionAnalysisSave(inflection.LInflectionId, prediction, marks, stamp, regular);
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
        HashSet<long> missed,
        HashSet<string> missedCells)
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
        HashSet<string> taken = new(StringComparer.Ordinal);
        List<LParadigmSlot> slots = [];
        foreach (LParadigm paradigm in LParadigmClerkScan(pack, value.LSpeechValueCode))
        {
            foreach (IReadOnlyList<long> cell in paradigm.LParadigmCells)
            {
                List<LMorphology> resolved = [];
                foreach (long code in cell)
                {
                    LMorphology? morphology = LMorphologyResolve(
                        pack, morphologies, value.LSpeechValueLanguage, code);
                    if (morphology is null)
                    {
                        break;
                    }

                    resolved.Add(morphology);
                }

                if (resolved.Count == 0
                    || resolved.Count != cell.Count
                    || !taken.Add(string.Join("+", resolved.Select(static row => row.LMorphologyId).Order())))
                {
                    continue;
                }

                LInflection? inflection = null;
                foreach (LInflection candidate in stored)
                {
                    if (resolved.All(row => candidate.LInflectionMorphology.Contains(row.LMorphologyId))
                        && (candidate.LInflectionSpeechId is null || candidate.LInflectionSpeechId == speechId))
                    {
                        inflection = candidate;
                        break;
                    }
                }

                LParadigmSlot slot = new(value, resolved[0], inflection, LState.LStateSpecified, paradigm, resolved);
                if (inflection is null)
                {
                    bool unknown = (resolved.Count == 1 && missed.Contains(resolved[0].LMorphologyId))
                        || missedCells.Contains(slot.LParadigmSlotKey);
                    slot = slot with
                    {
                        LParadigmSlotState = unknown ? LState.LStateUnknown : LState.LStateUnspecified,
                    };
                }

                slots.Add(slot);
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

    public static LParadigmShown LParadigmClerkCheck(LParadigmRow row, bool pending, bool enabled, bool held)
    {
        ArgumentNullException.ThrowIfNull(row);

        return row.LParadigmRowFirst.LParadigmSlotShow(pending, enabled, held);
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
