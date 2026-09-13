using System.Net;
using System.Net.Http;
using Llyn.Core;

namespace Llyn.Tests;

internal static class TPronunciationHelper
{
    internal const string TPronunciationHelperUrl = "https://example.test/{word}";

    internal static HttpClient TSourceClientCreate(string body, HttpStatusCode status) =>
        new(new TSourceHandler(body, status));

    internal static LSourceReading TSourceReadingCreate(string variety, string strategy, string match, int skip = 0) =>
        TInterface.TSourceReadingCreate(variety, strategy, match, 0, null, true, skip);

    internal static LSourceAttempt TSourceAttemptCreate(params LSourceReading[] readings) =>
        TInterface.TSourceAttemptCreate([TPronunciationHelperUrl], readings, null, null, null);

    internal static LSource TSourceStubCreate(params LReading[] readings) =>
        new TSourceStub(TInterface.TAnswerCreate(readings));

    internal static LSource TSourceLostCreate() =>
        new TSourceStub(TInterface.TAnswerLostRead());

    internal static TReceiverStub TReceiverCreate() =>
        new();

    internal static TListenerStub TListenerCreate() =>
        new();
}
