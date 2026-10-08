using System;
using System.Collections.Generic;
using Llyn.Core;

namespace Llyn.Application;

public sealed class LPortraitClerk
{
    private readonly LLanguageClerk _lPortraitClerkLanguages;
    private readonly LEntryClerk _lPortraitClerkEntries;
    private readonly LVocabularyClerk _lPortraitClerkVocabulary;
    private readonly LTranslationClerk _lPortraitClerkTranslations;
    private readonly LReferenceClerk _lPortraitClerkReferences;
    private readonly LFavoriteClerk _lPortraitClerkFavorites;
    private readonly LFanqieClerk _lPortraitClerkFanqie;
    private readonly LFrequencyClerk _lPortraitClerkFrequencies;
    private readonly LParadigmClerk _lPortraitClerkParadigms;
    private readonly LScriptClerk _lPortraitClerkScripts;
    private readonly Func<LSettings> _lPortraitClerkSettings;

    public LPortraitClerk(
        LLanguageClerk languages,
        LEntryClerk entries,
        LVocabularyClerk vocabulary,
        LTranslationClerk translations,
        LReferenceClerk references,
        LFavoriteClerk favorites,
        LFanqieClerk fanqie,
        LFrequencyClerk frequencies,
        LParadigmClerk paradigms,
        LScriptClerk scripts,
        Func<LSettings> settings)
    {
        ArgumentNullException.ThrowIfNull(languages);
        ArgumentNullException.ThrowIfNull(entries);
        ArgumentNullException.ThrowIfNull(vocabulary);
        ArgumentNullException.ThrowIfNull(translations);
        ArgumentNullException.ThrowIfNull(references);
        ArgumentNullException.ThrowIfNull(favorites);
        ArgumentNullException.ThrowIfNull(fanqie);
        ArgumentNullException.ThrowIfNull(frequencies);
        ArgumentNullException.ThrowIfNull(paradigms);
        ArgumentNullException.ThrowIfNull(scripts);
        ArgumentNullException.ThrowIfNull(settings);
        _lPortraitClerkLanguages = languages;
        _lPortraitClerkEntries = entries;
        _lPortraitClerkVocabulary = vocabulary;
        _lPortraitClerkTranslations = translations;
        _lPortraitClerkReferences = references;
        _lPortraitClerkFavorites = favorites;
        _lPortraitClerkFanqie = fanqie;
        _lPortraitClerkFrequencies = frequencies;
        _lPortraitClerkParadigms = paradigms;
        _lPortraitClerkScripts = scripts;
        _lPortraitClerkSettings = settings;
    }

    public LPortraitPage LPortraitClerkRead(long entryId, LPortraitLabel label, bool fetch = true)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(entryId);
        ArgumentNullException.ThrowIfNull(label);

        LEntryDraft draft = _lPortraitClerkEntries.LEntryClerkLoad(entryId)
            ?? throw new InvalidOperationException("The entry no longer stands in the workspace.");
        string language = draft.LEntryDraftLanguage;

        LSentenceOrder order = LPortraitOrderRead(language);

        List<long> ids = [];
        LPortraitLink.LPortraitLinkRead(draft.LEntryDraftMeanings, ids);
        LPortraitLink.LPortraitLinkRead(draft.LEntryDraftCollocations, ids);
        ids.AddRange(draft.LEntryDraftSources);

        IReadOnlyDictionary<long, LPortraitLink> targets = LPortraitTargetScan(ids);
        IReadOnlyDictionary<long, string> sources = LPortraitSourceScan();

        bool favorite = _lPortraitClerkFavorites.LFavoriteClerkCheck(entryId);

        IReadOnlyList<LFanqieRow> fanqie = LPortraitFanqieScan(entryId, language);

        List<LPortraitSection> sections = [];
        LPortraitClerkCrest.LPortraitGlyphAdd(sections, draft, LPortraitGlyphRead(language), label);
        LPortraitClerkCrest.LPortraitFrequencyAdd(sections, LPortraitFrequencyScan(entryId, fetch), label);
        LPortraitClerkCrest.LPortraitFormAdd(sections, draft, label);
        LPortraitClerkCrest.LPortraitParadigmAdd(sections, LPortraitParadigmScan(entryId), label);
        LPortraitClerkCrest.LPortraitFanqieAdd(sections, fanqie, label);
        LPortraitClerkCard.LPortraitBandAdd(
            sections,
            label.LPortraitLabelMeanings,
            LPortraitCard.LPortraitCardCreate(
                draft.LEntryDraftMeanings, label.LPortraitLabelMeaning, order, language, label, targets, sources));
        LPortraitClerkCard.LPortraitBandAdd(
            sections,
            label.LPortraitLabelCollocations,
            LPortraitCard.LPortraitCardCreate(
                draft.LEntryDraftCollocations,
                label.LPortraitLabelCollocation,
                order,
                language,
                label,
                targets,
                sources));
        LPortraitClerkCard.LPortraitEtymologyAdd(sections, draft, targets, label);
        LPortraitClerkCard.LPortraitIncomingAdd(sections, LPortraitIncomingScan(entryId), label);
        LPortraitClerkCard.LPortraitNoteAdd(sections, draft, label);
        LPortraitClerkCrest.LPortraitScriptAdd(sections, LPortraitScriptScan(entryId, language), label);

        bool respelled = _lPortraitClerkSettings().LSettingsRespelled;
        List<string> chips = [.. LVocabularyClerk.LSpeechShow(draft.LEntryDraftSpeeches)];
        if (label.LPortraitLabelUnit.TryGetValue(draft.LEntryDraftUnit, out string? unit))
        {
            chips.Insert(0, unit);
        }

        return new LPortraitPage(
            draft.LEntryDraftHeadword,
            language,
            chips,
            sections,
            favorite,
            [
                .. LPortraitReading.LPortraitReadingCreate(
                    draft.LEntryDraftPronunciations,
                    _lPortraitClerkLanguages.LLanguageRespellingCheck(language, respelled),
                    _lPortraitClerkLanguages.LLanguagePhonemicCheck(language)),
                .. LPortraitReading.LPortraitReadingCreate(draft.LEntryDraftTranscriptions),
                .. LPortraitReading.LPortraitReadingCreate(
                    draft.LEntryDraftReflexes,
                    spoken => _lPortraitClerkLanguages.LLanguageRespellingCheck(spoken, respelled),
                    _lPortraitClerkLanguages.LLanguagePhonemicCheck,
                    fanqie),
            ]);
    }

    private IReadOnlyDictionary<long, LPortraitLink> LPortraitTargetScan(IReadOnlyList<long> ids)
    {
        Dictionary<long, LPortraitLink> targets = [];

        foreach (LTranslationTarget target in _lPortraitClerkTranslations.LTranslationTargetRead(ids))
        {
            targets[target.LTranslationTargetId] = new LPortraitLink(
                target.LTranslationTargetHeadword,
                target.LTranslationTargetLanguage);
        }

        return targets;
    }

    private IReadOnlyDictionary<long, string> LPortraitSourceScan()
    {
        return _lPortraitClerkReferences.LCitationRead();
    }

    private IReadOnlyList<LFanqieRow> LPortraitFanqieScan(long entryId, string language)
    {
        return _lPortraitClerkFanqie.LFanqieBookRead(language).Count == 0
            ? []
            : _lPortraitClerkFanqie.LFanqieClerkRead(entryId);
    }

    private LSentenceOrder LPortraitOrderRead(string language)
    {
        return _lPortraitClerkVocabulary.LSentenceOrderRead(language);
    }

    private LGlyph? LPortraitGlyphRead(string language)
    {
        return language.Length == 0 ? null : _lPortraitClerkLanguages.LLanguageClerkLoad(language).LLanguageGlyph;
    }

    private IReadOnlyList<LFrequency> LPortraitFrequencyScan(long entryId, bool fetch)
    {
        return _lPortraitClerkFrequencies.LFrequencyClerkRead(entryId, fetch);
    }

    private IReadOnlyList<LParadigmSlot> LPortraitParadigmScan(long entryId)
    {
        return _lPortraitClerkParadigms.LParadigmClerkShow(entryId);
    }

    private IReadOnlyList<LUsage> LPortraitIncomingScan(long entryId)
    {
        return _lPortraitClerkTranslations.LTranslationIncomingRead(
            entryId, _lPortraitClerkSettings().LSettingsEpithet);
    }

    private IReadOnlyList<LScriptImage> LPortraitScriptScan(long entryId, string language)
    {
        return _lPortraitClerkScripts.LScriptStyleRead(language).Count == 0
            ? []
            : _lPortraitClerkScripts.LScriptClerkRead(entryId);
    }
}
