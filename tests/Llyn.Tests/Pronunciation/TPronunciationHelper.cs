using System.Net;
using System.Net.Http;
using Llyn.Core;

namespace Llyn.Tests;

internal static class TPronunciationHelper
{
    internal const string TPronunciationHelperUrl = "https://example.test/{word}";

    internal static HttpClient TSourceClientCreate(string body, HttpStatusCode status) =>
        new(new TSourceHandler(body, status));

    internal static HttpClient TSourceClientCreate(IReadOnlyDictionary<string, string> pages) =>
        new(new TSourceHandler(pages));

    internal static HttpClient TSourceClientCreate(string body, Task gate) =>
        new(new TSourceHandler(body, HttpStatusCode.OK, gate));

    internal static LSourceReading TSourceReadingCreate(
        string variety,
        string strategy,
        string match,
        int skip = 0,
        bool every = false) =>
        TInterface.TSourceReadingCreate(variety, strategy, match, 0, null, true, skip, every);

    internal static LSourceAttempt TSourceAttemptCreate(params LSourceReading[] readings) =>
        TInterface.TSourceAttemptCreate([TPronunciationHelperUrl], readings, null, null, null);

    internal static LSourceAttempt TSourceFollowCreate(LSourceReading follow, params LSourceReading[] readings) =>
        TInterface.TSourceAttemptCreate([TPronunciationHelperUrl], readings, null, null, null, follow);

    internal static LSource TSourceStubCreate(params LReading[] readings) =>
        new TSourceStub(TInterface.TAnswerCreate(readings));

    internal static LSource TSourceLostCreate() =>
        new TSourceStub(TInterface.TAnswerLostRead());

    internal static TReceiverStub TReceiverCreate() =>
        new();

    internal static TListenerStub TListenerCreate() =>
        new();
}
