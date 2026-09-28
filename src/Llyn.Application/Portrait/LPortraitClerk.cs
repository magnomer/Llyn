using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Llyn.Core;

namespace Llyn.Application;

public sealed class LPortraitClerk
{
    private const double LPortraitInch = 96.0;

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

    public async Task LPortraitClerkExport(LPortraitPage portrait, string path, LPortraitMedium format)
    {
        ArgumentNullException.ThrowIfNull(portrait);
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        if (format == LPortraitMedium.LPortraitMediumPdf)
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

    public static LPressTicket LPortraitTicketCreate(
        string printer,
        double? width,
        double? height,
        bool landscape,
        int copies,
        bool collated,
        LPressSide side,
        LPressInk ink)
    {
        LPressPaper paper = width is > 0 and double across && height is > 0 and double down
            ? new LPressPaper(across / LPortraitInch, down / LPortraitInch)
            : LPressPaper.LPressPaperLocal;
        return new LPressTicket(printer, paper, landscape, copies, collated, side, ink);
    }

    public static IReadOnlyList<(LPortraitMedium, string, bool)> LPortraitMediumRead()
    {
        return
        [
            (LPortraitMedium.LPortraitMediumMarkup, ".llx", false),
            (LPortraitMedium.LPortraitMediumHtml, ".html", true),
            (LPortraitMedium.LPortraitMediumMarkdown, ".md", false),
            (LPortraitMedium.LPortraitMediumDocx, ".docx", false),
            (LPortraitMedium.LPortraitMediumPdf, ".pdf", false),
        ];
    }

    public static IReadOnlyList<string> LPortraitKindRead()
    {
        return Enum.GetValues<LReferenceKind>().Select(LReference.LReferenceKindResolve).ToList();
    }

    public static LPortraitLabel LPortraitLabelCreate(IReadOnlyList<string> words)
    {
        ArgumentNullException.ThrowIfNull(words);
        ArgumentOutOfRangeException.ThrowIfNotEqual(words.Count, 22);

        return new LPortraitLabel(
            words[0], words[1], words[2], words[3], words[4], words[5], words[6], words[7], words[8], words[9],
            words[10], words[11], words[12], words[13], words[14], words[15], words[16], words[17], words[18],
            words[19], words[20], words[21]);
    }

    public static LPortraitLegend LPortraitLegendCreate(
        IReadOnlyList<string> words, IReadOnlyDictionary<string, string> kinds)
    {
        ArgumentNullException.ThrowIfNull(words);
        ArgumentNullException.ThrowIfNull(kinds);
        ArgumentOutOfRangeException.ThrowIfNotEqual(words.Count, 13);

        Dictionary<LReferenceKind, string> worded = [];
        foreach (LReferenceKind kind in Enum.GetValues<LReferenceKind>())
        {
            worded[kind] = kinds.GetValueOrDefault(
                LReference.LReferenceKindResolve(kind), LReference.LReferenceKindFormat(kind));
        }

        return new LPortraitLegend(
            words[0], words[1], words[2], words[3], words[4], words[5], words[6], words[7], words[8], words[9],
            words[10], words[11], words[12], worded);
    }

    private IReadOnlyDictionary<long, LPortraitLink> LPortraitTargetScan(IReadOnlyList<long> ids)
    {
        Dictionary<long, LPortraitLink> targets = [];

        try
        {
            foreach (LTranslationTarget target in _lPortraitClerkTranslations.LTranslationTargetRead(ids))
            {
                targets[target.LTranslationTargetId] = new LPortraitLink(
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
