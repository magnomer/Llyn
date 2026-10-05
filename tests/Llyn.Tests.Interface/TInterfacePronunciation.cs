using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Llyn.Application;
using Llyn.Core;
using Llyn.Infrastructure;
using Llyn.ShellEngine;

namespace Llyn.Tests;

internal static partial class TInterface
{
    internal static LReflexGuise TReflexGuiseCreate(bool respelled, bool phonemic, bool folded) =>
        new(respelled, phonemic, folded);

    internal static LReading TReadingCreate(string variety, string phonetic) =>
        new LReading(variety, phonetic);

    internal static LAnswer TAnswerCreate(IReadOnlyList<LReading> readings) =>
        LAnswer.LAnswerCreate(readings);

    internal static LAnswer TAnswerLostRead() =>
        LAnswer.LAnswerLost;

    internal static string TReadingNormalize(string phonetic) =>
        LReading.LReadingNormalize(phonetic);

    internal static IReadOnlyList<string> TStemKeyScan(string text, string separator) =>
        LStem.LStemKeyScan(text, separator);

    internal static LShengfu TShengfuCreate(string character, string text, string source = "") =>
        new(character, text, source);

    internal static LRespellingRule TRespellingRuleCreate(string pattern, string replacement) =>
        new LRespellingRule(pattern, replacement);

    internal static LRespelling TRespellingCreate(
        IReadOnlyList<string> varieties,
        IReadOnlyList<LRespellingRule> rules) =>
        new LRespelling(varieties, rules);

    internal static string TRespellingResolve(this LRespelling group, string phonetic, string variety) =>
        group.LRespellingResolve(phonetic, variety);

    internal static string TRespellingScan(IReadOnlyList<LRespelling> groups, string phonetic, string variety) =>
        LRespelling.LRespellingScan(groups, phonetic, variety);

    internal static LAnatomy TAnatomyScan(
        IReadOnlyList<LAnatomyRule> rules, string language, string text, string respelling) =>
        LAnatomy.LAnatomyScan(rules, language, text, respelling);

    internal static bool TAnatomyRuleMatch(this LAnatomyRule rule, string language) =>
        rule.LAnatomyRuleMatch(language);

    internal static IReadOnlyList<string> TDescentScan(
        IReadOnlyList<LDescent> rules, string language, string tone) =>
        LDescent.LDescentScan(rules, language, tone);

    internal static bool TDescentMatch(this LDescent rule, string language) =>
        rule.LDescentMatch(language);

    internal static LCandidate TCandidateCreate(
        string source,
        string? phonetic,
        int order,
        bool reached,
        string variety) =>
        new LCandidate(source, phonetic, order, reached, variety);

    internal static LVariety TVarietyCreate(string name, string? flag = null) =>
        new LVariety(name, flag);

    internal static LSeeker TLookupCreate(
        IReadOnlyList<LSource> sources,
        IReadOnlyList<LVariety>? varieties = null,
        IReadOnlyList<LRespelling>? cleanups = null,
        bool literal = false) =>
        new LLookup(sources, varieties ?? [], cleanups ?? [], literal);

    internal static LReceiver TReceiverRespellingCreate(LReceiver inner, IReadOnlyList<LRespelling> groups) =>
        new LReceiverRespelling(inner, groups);

    internal static LReceiver TReceiverRelayCreate(Action<LLookupStep> sink) =>
        new LReceiverRelay(sink);

    internal static LListener TListenerRelayCreate(Action<LHarvestStep> sink) =>
        new LListenerRelay(sink);

    internal static LHarvestStep THarvestStepCreate(
        LHarvestKind kind, string source, int order, LRecording? recording) =>
        new(kind, source, order, recording);

    internal static void TReceiverSourceStart(this LReceiver receiver, string source, int order)
    {
        receiver.LReceiverSourceStart(source, order);
    }

    internal static void TReceiverCandidateAdd(this LReceiver receiver, LCandidate candidate)
    {
        receiver.LReceiverCandidateAdd(candidate);
    }

    internal static void TReceiverLookupFinish(this LReceiver receiver)
    {
        receiver.LReceiverLookupFinish();
    }

    internal static IReadOnlyList<LReading> TReadingScan(
        IReadOnlyList<LReading> readings,
        IReadOnlyList<LVariety> varieties) =>
        LReading.LReadingScan(readings, varieties);

    internal static Task<IReadOnlyList<LCandidate>> TLookupStart(
        this LSeeker seeker,
        string word,
        LReceiver receiver,
        CancellationToken cancellation) =>
        seeker.LSeekerStart(word, receiver, cancellation);

    internal static LRecording TRecordingCreate(
        string source,
        string? address,
        int order,
        bool reached,
        string variety) =>
        new LRecording(source, address, order, reached, variety);

    internal static LHarvest THarvestCreate(
        IReadOnlyList<LSource> sources,
        IReadOnlyList<LVariety>? varieties = null) =>
        new LHarvest(sources, varieties ?? []);

    internal static Task<IReadOnlyList<LRecording>> THarvestStart(
        this LHarvest harvest,
        string word,
        string variety,
        LListener listener,
        CancellationToken cancellation) =>
        harvest.LHarvestStart(word, variety, listener, cancellation);

    internal static LLanguage TLanguageLoad(string language) =>
        LLanguageLoader.LLanguageLoaderLoad(language);

    internal static IReadOnlyList<string> TLanguageScan() =>
        LLanguageLoader.LLanguageLoaderScan();

    internal static IReadOnlyList<string> TGlyphScan(string text) =>
        LGlyph.LGlyphScan(text);

    internal static LGlyph TGlyphCreate(string name, string language) =>
        new(name, language, []);

    internal static IReadOnlyList<LGlyphCell> TGlyphDivide(this LGlyph glyph, LEntryDraft draft) =>
        glyph.LGlyphDivide(draft);

    internal static IReadOnlyList<LTranscriptionDraft> TGlyphOtherRead(
        LGlyph? glyph, IReadOnlyList<LTranscriptionDraft> rows) =>
        LGlyph.LGlyphOtherRead(glyph, rows);

    internal static IReadOnlyList<LRecording> THarvestRecordingScan(
        IReadOnlyList<LRecording> recordings,
        string variety) =>
        LHarvest.LHarvestRecordingScan(recordings, variety);

    internal static Task<string> TWorkspaceRecordingSave(
        LRecording recording,
        string word,
        string language,
        string root,
        HttpClient client,
        CancellationToken cancellation) =>
        new LRecordingArchive(root, client).LRecordingSave(recording, word, language, cancellation);

    internal static Task<string> TWorkspaceRecordingPrepare(
        LRecording recording,
        string root,
        HttpClient client,
        CancellationToken cancellation) =>
        new LRecordingArchive(root, client).LRecordingPrepare(recording, cancellation);

    internal static Task TEngineRecordingFind(
        this LEngine engine,
        long session,
        string word,
        string language,
        long target,
        Action<LHarvestStep> sink,
        CancellationToken cancellation) =>
        engine.LEnginePronunciation.LEngineRecordingFind(
            session, word, language, target, new LListenerRelay(sink), cancellation);

    internal static Task TEngineTranscriptionFind(
        this LEngine engine,
        long session,
        string word,
        string language,
        string scheme,
        Action<LLookupStep> sink,
        CancellationToken cancellation) =>
        engine.LEnginePronunciation.LEngineTranscriptionFind(
            session, word, language, scheme, new LReceiverRelay(sink), cancellation);

    internal static IReadOnlyList<LVariety> TEngineVarietyRead(this LEngine engine, string language) =>
        engine.LEngineLanguage.LEngineVarietyRead(language);

    internal static bool TEngineFlaggedCheck(this LEngine engine, string language) =>
        engine.LEngineLanguage.LEngineFlaggedCheck(language);

    internal static IReadOnlyList<LContour> TEngineContourRead(this LEngine engine, string language, string ipa) =>
        engine.LEngineLanguage.LEngineContourRead(language, ipa);

    internal static IReadOnlyList<string> TEngineLanguageRead(this LEngine engine) =>
        engine.LEngineLanguage.LEngineLanguageRead();

    internal static string TEngineGlossRead(this LEngine engine) =>
        engine.LEngineLanguage.LEngineGlossRead();

    internal static IReadOnlyList<LContour> TContourParse(string ipa) =>
        LContour.LContourParse(ipa);

    internal static bool TContourToneCheck(IReadOnlyList<LContour> syllables) =>
        LContour.LContourToneCheck(syllables);

    internal static LFrequency TFrequencyCreate(string source, string raw, string? band) =>
        new LFrequency(source, raw, band);

    internal static LFrequencyGauge? TFrequencyGaugeResolve(IReadOnlyList<LFrequency> rows, string once) =>
        LFrequencyGauge.LFrequencyGaugeResolve(rows, once);

    internal static LFrequency TFrequencyIntervalCreate(string source, string raw, long once) =>
        new LFrequency(source, raw, null, once);

    internal static long? TFrequencyOnceResolve(LSourceSpec spec, double raw) =>
        LFrequency.LFrequencyOnceResolve(spec, raw);

    internal static string? TFrequencyBandResolve(LSourceSpec spec, double raw) =>
        LFrequency.LFrequencyBandResolve(spec, raw);

    internal static LFrequencyGauge TFrequencyGaugeCreate(int band, string source) =>
        new LFrequencyGauge(band, source);

    internal static IReadOnlyList<string> TFrequencyScaleRead() =>
        LFrequency.LFrequencyScale;

    internal static LContour TContourCreate(string text, IReadOnlyList<int> levels) =>
        new LContour(text, levels);

    internal static LFanqieRow TFanqieRowCreate(
        string initial, string rime, string heading, string division, string tone, bool rounded) =>
        new("字", "book", 0, "text", initial, rime, heading, division, tone, rounded);

    internal static LFanqieRow TFanqieRowCreate(
        string character, int position, string initial, string rime, string division, string tone) =>
        new(character, "book", position, "text", initial, rime, rime, division, tone, false);
}
