using System.Net.Http;
using Llyn.Application;
using Llyn.Core;
using Llyn.Infrastructure;
using Llyn.ShellEngine;

namespace Llyn.Tests;

internal static partial class TInterface
{
    internal static LReading TReadingCreate(string variety, string phonetic) =>
        new LReading(variety, phonetic);

    internal static LAnswer TAnswerCreate(IReadOnlyList<LReading> readings) =>
        LAnswer.LAnswerCreate(readings);

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
        int skip) =>
        new LSourceReading(variety, strategy, pattern, group, path, phonetic, skip);

    internal static LSourceAttempt TSourceAttemptCreate(
        IReadOnlyList<string> urls,
        IReadOnlyList<LSourceReading> readings,
        string? guard,
        IReadOnlyDictionary<string, string>? headers,
        string? prefix) =>
        new LSourceAttempt(urls, readings, guard, headers, prefix);

    internal static LSourceSpec TSourceSpecCreate(string name, IReadOnlyList<LSourceAttempt> attempts) =>
        new LSourceSpec(name, attempts);

    internal static LSource TSourceGenericCreate(LSourceSpec spec, HttpClient client) =>
        new LSourceGeneric(spec, client);

    internal static Task<LAnswer> TSourceFind(this LSource source, string word, CancellationToken cancellation) =>
        source.LSourceFind(word, cancellation);

    internal static LSeeker TLookupCreate(IReadOnlyList<LSource> sources) =>
        new LLookup(sources);

    internal static Task<IReadOnlyList<LCandidate>> TLookupStart(
        this LSeeker seeker,
        string word,
        LReceiver receiver,
        CancellationToken cancellation) =>
        seeker.LSeekerStart(word, receiver, cancellation);

    internal static LLanguage TLanguageLoad(string language) =>
        LLanguageLoader.LLanguageLoaderLoad(language);

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

    internal static IReadOnlyList<LVariety> TEngineVarietyRead(this LEngine engine, string language) =>
        engine.LEngineVarietyRead(language);

    internal static bool TEngineFlaggedCheck(this LEngine engine, string language) =>
        engine.LEngineFlaggedCheck(language);
}
