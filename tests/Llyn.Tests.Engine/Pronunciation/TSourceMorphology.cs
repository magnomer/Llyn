using System.Net;
using Llyn.Core;
using Xunit;

namespace Llyn.Tests;

public sealed class TSourceMorphology
{
    private const string TSourceMorphologyBody =
        "<b class=\"Latn form-of lang-en spast-form-of\" lang=\"en\"><a href=\"./went\">went</a></b>" +
        "<b class=\"Latn form-of lang-en past|part-form-of\" lang=\"en\"><a href=\"./gone\">gone</a></b>" +
        "<b class=\"Latn form-of lang-en p-form-of\" lang=\"en\"><a href=\"./goes\">goes</a></b>";

    private const string TSourceMorphologyPast =
        "class=\"Latn form-of lang-en (?:spast|ed-form)-form-of\"[^>]*><a[^>]*>([^<]+)<";

    private const string TSourceMorphologyParticiple =
        "class=\"Latn form-of lang-en (?:past\\|part|ed-form)-form-of\"[^>]*><a[^>]*>([^<]+)<";

    private const string TSourceMorphologyPlural =
        "class=\"Latn form-of lang-en p-form-of\"[^>]*><a[^>]*>([^<]+)<";

    [Fact]
    public async Task SourceFind_ThreeVarietyReadings_ReturnsFormsKeyedByCode()
    {
        LSource source = TSourceMorphologyCreate(TSourceMorphologyBody, HttpStatusCode.OK, null);

        LAnswer answer = await source.TSourceFind("go", CancellationToken.None);

        Assert.Equal(
            [("5", "went"), ("6", "gone"), ("2", "goes")],
            answer.LAnswerReadings.Select(reading => (reading.LReadingVariety, reading.LReadingPhonetic)));
    }

    [Fact]
    public async Task SourceFind_ConfirmMissed_ReturnsBlankReached()
    {
        LSource source = TSourceMorphologyCreate(TSourceMorphologyBody, HttpStatusCode.OK, "lang-fr");

        LAnswer answer = await source.TSourceFind("go", CancellationToken.None);

        Assert.True(answer.LAnswerEmpty);
        Assert.True(answer.LAnswerReached);
    }

    [Fact]
    public async Task SourceFind_ServerFailure_ReturnsLost()
    {
        LSource source = TSourceMorphologyCreate(string.Empty, HttpStatusCode.InternalServerError, null);

        LAnswer answer = await source.TSourceFind("go", CancellationToken.None);

        Assert.True(answer.LAnswerEmpty);
        Assert.False(answer.LAnswerReached);
    }

    private static LSource TSourceMorphologyCreate(string body, HttpStatusCode status, string? confirm)
    {
        LSourceAttempt attempt = TInterface.TSourceAttemptCreate(
            [TPronunciationHelper.TPronunciationHelperUrl],
            [
                TInterface.TSourceReadingCreate("5", "regex", TSourceMorphologyPast, 1, null, false, 0),
                TInterface.TSourceReadingCreate("6", "regex", TSourceMorphologyParticiple, 1, null, false, 0),
                TInterface.TSourceReadingCreate("2", "regex", TSourceMorphologyPlural, 1, null, false, 0),
            ],
            confirm,
            null,
            null);

        return TInterface.TSourceGenericCreate(
            TInterface.TSourceSpecCreate("Wiktionary", [attempt]),
            TPronunciationHelper.TSourceClientCreate(body, status));
    }
}
