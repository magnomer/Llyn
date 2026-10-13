using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Llyn.Core;
using Llyn.Infrastructure;

namespace Llyn.Tests;

internal static class TInterfaceSource
{
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

    internal static Task<(IReadOnlyList<LReflexDraft> LReflexFound, bool LReflexReached)> TReflexSourceFind(
        HttpClient client, string pattern, string character) =>
        new LReflexSourceHttp(client).LReflexSourceFind(
            new LReflexRule(
                "Xiang", "https://example.test/wiki/{word}", new Dictionary<string, string>(), pattern),
            character,
            CancellationToken.None);

    internal static Task<(LShengfu? LShengfuFound, bool LShengfuReached)> TShengfuSourceFind(
        HttpClient client, string pattern, string character) =>
        new LShengfuSourceHttp(client).LShengfuSourceFind(
            new LShengfuRule("https://example.test/series/{word}", pattern, new Dictionary<string, string>()),
            character,
            CancellationToken.None);

    internal static Task<(IReadOnlyList<LScriptImage> LScriptFound, bool LScriptReached)> TScriptSourceFind(
        HttpClient client, string pattern, string character) =>
        new LScriptSourceHttp(client).LScriptSourceFind(
            new LScriptStyle(
                "Seal", "https://example.test/seal/search", new Dictionary<string, string>(), pattern, 0, 0),
            character,
            CancellationToken.None);

    internal static Task<(IReadOnlyList<LFanqieRow> LFanqieFound, bool LFanqieReached)> TFanqieSourceFind(
        HttpClient client, string pattern, string character) =>
        new LFanqieSourceHttp(client).LFanqieSourceFind(
            new LFanqieBook(
                "Broad",
                "https://example.test/wiki/{word}",
                new Dictionary<string, string>(),
                "{word}",
                LFanqieBookLine: pattern),
            character,
            CancellationToken.None);
}
