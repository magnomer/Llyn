using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Llyn.Core;

namespace Llyn.Application;

public sealed class LPortraitClerk
{
    private readonly LPortraitVault _lPortraitClerkPortraits;
    private readonly LPress _lPortraitClerkPress;
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
    private readonly LMarkupClerk _lPortraitClerkMarkup;
    private readonly Func<LSettings> _lPortraitClerkSettings;

    public LPortraitClerk(
        LRig rig,
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
        LMarkupClerk markup,
        Func<LSettings> settings)
    {
        ArgumentNullException.ThrowIfNull(rig);
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
        ArgumentNullException.ThrowIfNull(markup);
        ArgumentNullException.ThrowIfNull(settings);
        _lPortraitClerkPortraits = rig.LRigPortrait;
        _lPortraitClerkPress = rig.LRigPress;
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
        _lPortraitClerkMarkup = markup;
        _lPortraitClerkSettings = settings;
    }

    public LPortraitPage LPortraitClerkRead(long entryId, LPortraitLabel label)
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

        bool favorite;
        try
        {
            favorite = _lPortraitClerkFavorites.LFavoriteClerkCheck(entryId);
        }
        catch (Exception)
        {
            favorite = false;
        }

        IReadOnlyList<LFanqieRow> fanqie = LPortraitFanqieScan(entryId, language);

        List<LPortraitSection> sections = [];
        LPortraitClerkCrest.LPortraitGlyphAdd(sections, draft, LPortraitGlyphRead(language), label);
        LPortraitClerkCrest.LPortraitFrequencyAdd(sections, LPortraitFrequencyScan(entryId), label);
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
        return new LPortraitPage(
            draft.LEntryDraftHeadword,
            language,
            LVocabularyClerk.LSpeechShow(draft.LEntryDraftSpeeches),
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

    public async Task LPortraitClerkExport(LPortraitPage portrait, string path, LPortraitFormat format)
    {
        ArgumentNullException.ThrowIfNull(portrait);
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        if (format == LPortraitFormat.LPortraitFormatPdf)
        {
            await _lPortraitClerkPress.LPressSave(_lPortraitClerkPortraits.LPortraitSheetFormat(portrait), path)
                .ConfigureAwait(false);
            return;
        }

        _lPortraitClerkPortraits.LPortraitSave(portrait, format, path);
    }

    public void LPortraitMarkupExport(long entryId, string path)
    {
        _lPortraitClerkMarkup.LMarkupClerkExport([entryId], path);
    }

    public Task LPortraitClerkPrint(LPortraitPage page, LPressTicket ticket)
    {
        ArgumentNullException.ThrowIfNull(page);
        ArgumentNullException.ThrowIfNull(ticket);

        return _lPortraitClerkPress.LPressPrint(_lPortraitClerkPortraits.LPortraitSheetFormat(page), ticket);
    }

    private IReadOnlyDictionary<long, LPortraitLink> LPortraitTargetScan(IReadOnlyList<long> ids)
    {
        Dictionary<long, LPortraitLink> targets = [];

        try
        {
            foreach (LTranslationTarget target in _lPortraitClerkTranslations.LTranslationTargetRead(ids))
            {
                targets[target.LTranslationTargetId] = new LPortraitLink(
                    target.LTranslationTargetId,
                    target.LTranslationTargetHeadword,
                    target.LTranslationTargetLanguage);
            }
        }
        catch (Exception)
        {
            targets.Clear();
        }

        return targets;
    }

    private IReadOnlyDictionary<long, string> LPortraitSourceScan()
    {
        try
        {
            return _lPortraitClerkReferences.LCitationRead();
        }
        catch (Exception)
        {
            return new Dictionary<long, string>();
        }
    }

    private IReadOnlyList<LFanqieRow> LPortraitFanqieScan(long entryId, string language)
    {
        try
        {
            return _lPortraitClerkFanqie.LFanqieBookRead(language).Count == 0
                ? []
                : _lPortraitClerkFanqie.LFanqieClerkRead(entryId);
        }
        catch (Exception)
        {
            return [];
        }
    }

    private LSentenceOrder LPortraitOrderRead(string language)
    {
        try
        {
            return _lPortraitClerkVocabulary.LSentenceOrderRead(language);
        }
        catch (Exception)
        {
            return LSentenceOrder.LSentenceOrderDefault;
        }
    }

    private LGlyph? LPortraitGlyphRead(string language)
    {
        try
        {
            return language.Length == 0 ? null : _lPortraitClerkLanguages.LLanguageClerkLoad(language).LLanguageGlyph;
        }
        catch (Exception)
        {
            return null;
        }
    }

    private IReadOnlyList<LFrequency> LPortraitFrequencyScan(long entryId)
    {
        try
        {
            return _lPortraitClerkFrequencies.LFrequencyClerkRead(entryId);
        }
        catch (Exception)
        {
            return [];
        }
    }

    private IReadOnlyList<LParadigmSlot> LPortraitParadigmScan(long entryId)
    {
        try
        {
            return _lPortraitClerkParadigms.LParadigmClerkShow(entryId);
        }
        catch (Exception)
        {
            return [];
        }
    }

    private IReadOnlyList<LUsage> LPortraitIncomingScan(long entryId)
    {
        try
        {
            return _lPortraitClerkTranslations.LTranslationIncomingRead(
                entryId, _lPortraitClerkSettings().LSettingsEpithet);
        }
        catch (Exception)
        {
            return [];
        }
    }

    private IReadOnlyList<LScriptImage> LPortraitScriptScan(long entryId, string language)
    {
        try
        {
            return _lPortraitClerkScripts.LScriptStyleRead(language).Count == 0
                ? []
                : _lPortraitClerkScripts.LScriptClerkRead(entryId);
        }
        catch (Exception)
        {
            return [];
        }
    }
}
