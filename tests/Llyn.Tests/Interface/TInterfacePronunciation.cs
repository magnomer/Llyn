using System.Net.Http;
using Llyn.Application;
using Llyn.Core;
using Llyn.Infrastructure;
using Llyn.ShellEngine;

namespace Llyn.Tests;

internal static partial class TInterface
{
    internal static IReadOnlyList<long> TAnchorToggle(IReadOnlyList<long> anchors, long fanqieId, bool anchored) =>
        LAnchor.LAnchorToggle(anchors, fanqieId, anchored);

    internal static LReading TReadingCreate(string variety, string phonetic) =>
        new LReading(variety, phonetic);

    internal static LAnswer TAnswerCreate(IReadOnlyList<LReading> readings) =>
        LAnswer.LAnswerCreate(readings);

    internal static LAnswer TAnswerLostRead() =>
        LAnswer.LAnswerLost;

    internal static string TReadingNormalize(string phonetic) =>
        LReading.LReadingNormalize(phonetic);

    internal static string TDiweiRimeFormat(string rime, string division, bool rounded) =>
        LDiwei.LDiweiRimeFormat(rime, division, rounded);

    internal static string TDiweiChongniuRead(string rime) =>
        LDiwei.LDiweiChongniuRead(rime);

    internal static int TDiweiRankRead(string kind, string heading, LHypothesis? hypothesis) =>
        LDiwei.LDiweiRankRead(kind, heading, hypothesis);

    internal static int TDiweiRankNormalize(int rank) =>
        LDiwei.LDiweiRankNormalize(rank);

    internal static IReadOnlyList<string> TStemKeyScan(string text, string separator) =>
        LStem.LStemKeyScan(text, separator);

    internal static LShengfu TShengfuCreate(string character, string text, string source = "") =>
        new(character, text, source);

    internal static LRespellingRule TRespellingRuleCreate(string pattern, string replacement) =>
        new LRespellingRule(pattern, replacement);

    internal static LRespelling TRespellingCreate(
        string name,
        IReadOnlyList<string> varieties,
        IReadOnlyList<LRespellingRule> rules) =>
        new LRespelling(name, varieties, rules);

    internal static string TRespellingResolve(this LRespelling group, string phonetic, string variety) =>
        group.LRespellingResolve(phonetic, variety);

    internal static string TRespellingScan(IReadOnlyList<LRespelling> groups, string phonetic, string variety) =>
        LRespelling.LRespellingScan(groups, phonetic, variety);

    internal static LAnatomy TAnatomyScan(
        IReadOnlyList<LAnatomyRule> rules, string language, string text, string respelling) =>
        LAnatomy.LAnatomyScan(rules, language, text, respelling);

    internal static bool TAnatomyRuleMatch(this LAnatomyRule rule, string language) =>
        rule.LAnatomyRuleMatch(language);

    internal static IReadOnlyList<string> TAnatomyToneScan(
        IReadOnlyList<LAnatomyTone> rules, string language, string tone) =>
        LAnatomyTone.LAnatomyToneScan(rules, language, tone);

    internal static bool TAnatomyToneMatch(this LAnatomyTone rule, string language) =>
        rule.LAnatomyToneMatch(language);

    internal static LCandidate TCandidateCreate(
        string source,
        string? phonetic,
        int order,
        bool reached,
        string variety) =>
        new LCandidate(source, phonetic, order, reached, variety);

    internal static LSourceReading TSourceReadingCreate(
        string variety,
        string strategy,
        string? pattern,
        int group,
        string? path,
        bool phonetic,
        int skip,
        bool every = false) =>
        new LSourceReading(variety, strategy, pattern, group, path, phonetic, skip, every);

    internal static LSourceAttempt TSourceAttemptCreate(
        IReadOnlyList<string> urls,
        IReadOnlyList<LSourceReading> readings,
        string? guard,
        IReadOnlyDictionary<string, string>? headers,
        string? prefix,
        LSourceReading? follow = null) =>
        new LSourceAttempt(urls, readings, guard, headers, prefix, follow);

    internal static LSourceSpec TSourceSpecCreate(
        string name,
        IReadOnlyList<LSourceAttempt> attempts,
        IReadOnlyList<LRespellingRule>? spelling = null,
        double? total = null,
        double? factor = null,
        double? power = null,
        string? unit = null) =>
        new LSourceSpec(name, attempts, spelling, null, total, factor, power, unit);

    internal static LSource TSourceGenericCreate(LSourceSpec spec, HttpClient client) =>
        new LSourceGeneric(spec, client);

    internal static Task<LAnswer> TSourceFind(this LSource source, string word, CancellationToken cancellation) =>
        source.LSourceFind(word, cancellation);

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

    internal static LTrove TTroveCreate() =>
        new LTrove();

    internal static void TTroveCandidateSave(
        this LTrove trove,
        long session,
        string word,
        string language,
        IReadOnlyList<LCandidate> found)
    {
        trove.LTroveCandidateSave(session, word, language, found);
    }

    internal static IReadOnlyList<LCandidate>? TTroveCandidateRead(
        this LTrove trove,
        long session,
        string word,
        string language) =>
        trove.LTroveCandidateRead(session, word, language);

    internal static void TTroveRecordingSave(
        this LTrove trove,
        long session,
        string word,
        string language,
        IReadOnlyList<LRecording> found)
    {
        trove.LTroveRecordingSave(session, word, language, found);
    }

    internal static IReadOnlyList<LRecording>? TTroveRecordingRead(
        this LTrove trove,
        long session,
        string word,
        string language) =>
        trove.LTroveRecordingRead(session, word, language);

    internal static void TTroveTranscriptionSave(
        this LTrove trove,
        long session,
        string word,
        string language,
        string scheme,
        IReadOnlyList<LCandidate> found)
    {
        trove.LTroveTranscriptionSave(session, word, language, scheme, found);
    }

    internal static IReadOnlyList<LCandidate>? TTroveTranscriptionRead(
        this LTrove trove,
        long session,
        string word,
        string language,
        string scheme) =>
        trove.LTroveTranscriptionRead(session, word, language, scheme);

    internal static void TTroveClear(this LTrove trove, long session)
    {
        trove.LTroveClear(session);
    }

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
        engine.LEnginePronunciation.LEngineRecordingFind(session, word, language, target, sink, cancellation);

    internal static Task TEngineTranscriptionFind(
        this LEngine engine,
        long session,
        string word,
        string language,
        string scheme,
        Action<LLookupStep> sink,
        CancellationToken cancellation) =>
        engine.LEnginePronunciation.LEngineTranscriptionFind(session, word, language, scheme, sink, cancellation);

    internal static IReadOnlyList<LVariety> TEngineVarietyRead(this LEngine engine, string language) =>
        engine.LEngineLanguage.LEngineVarietyRead(language);

    internal static bool TEngineFlaggedCheck(this LEngine engine, string language) =>
        engine.LEngineLanguage.LEngineFlaggedCheck(language);

    internal static bool TEngineTonalCheck(this LEngine engine, string language) =>
        engine.LEngineLanguage.LEngineTonalCheck(language);

    internal static IReadOnlyList<string> TEngineLanguageRead(this LEngine engine) =>
        engine.LEngineLanguage.LEngineLanguageRead();

    internal static IReadOnlyList<LContour> TContourParse(string ipa) =>
        LContour.LContourParse(ipa);

    internal static bool TContourToneCheck(IReadOnlyList<LContour> syllables) =>
        LContour.LContourToneCheck(syllables);

    internal static LFrequency TFrequencyCreate(string source, string raw, string? band) =>
        new LFrequency(source, raw, band);

    internal static long? TFrequencyOnceResolve(LSourceSpec spec, double raw) =>
        LFrequency.LFrequencyOnceResolve(spec, raw);

    internal static string? TFrequencyBandResolve(LSourceSpec spec, double raw) =>
        LFrequency.LFrequencyBandResolve(spec, raw);

    internal static LHypothesis THypothesisCreate(
        IReadOnlyDictionary<string, string> initials,
        IReadOnlyDictionary<string, string> finals,
        IReadOnlyDictionary<string, IReadOnlyList<LHypothesisTone>> tones,
        IReadOnlyList<LHypothesisPlace>? places = null) =>
        new(initials, finals, tones, places);

    internal static LHypothesisPlace THypothesisPlaceCreate(string name, IReadOnlyList<string> initials) =>
        new(name, initials);

    internal static string? THypothesisInitialFind(this LHypothesis hypothesis, LFanqieRow row) =>
        hypothesis.LHypothesisInitialFind(row);

    internal static LHypothesisPlace? THypothesisPlaceFind(this LHypothesis hypothesis, string initial) =>
        hypothesis.LHypothesisPlaceFind(initial);

    internal static int THypothesisRankRead(this LHypothesis hypothesis, string initial) =>
        hypothesis.LHypothesisRankRead(initial);

    internal static LHypothesisTone THypothesisToneCreate(
        string onset, IReadOnlyList<LRespellingRule> rules, string label) =>
        new(onset, rules, label);

    internal static LHypothesisSound? THypothesisResolve(this LHypothesis hypothesis, LFanqieRow row) =>
        hypothesis.LHypothesisResolve(row);

    internal static LFanqieRow TFanqieRowCreate(
        string initial, string rime, string heading, string division, string tone, bool rounded) =>
        new("字", "book", 0, "text", initial, rime, heading, division, tone, rounded);

    internal static LFanqieRow TFanqieRowCreate(
        string character, int position, string initial, string rime, string division, string tone) =>
        new(character, "book", position, "text", initial, rime, rime, division, tone, false);
}
