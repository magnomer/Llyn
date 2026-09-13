using System.Net;
using Llyn.Core;
using Xunit;

namespace Llyn.Tests;

public sealed class TSourceGeneric
{
    private const string TSourceGenericBody =
        "<span class=\"uk\"><span class=\"phon\">/təˈmɑːtəʊ/</span></span>" +
        "<span class=\"us\"><span class=\"phon\">/təˈmeɪtoʊ/</span></span>";

    private const string TSourceGenericBritish = "<span class=\"uk\">";
    private const string TSourceGenericAmerican = "<span class=\"us\">";
    private const string TSourceGenericPhon = "<span class=\"phon\">";

    [Fact]
    public async Task SourceFind_TwoReadings_ReturnsBothTagged()
    {
        LSource source = TSourceGenericCreate(
            TSourceGenericBody,
            TPronunciationHelper.TSourceAttemptCreate(
                TPronunciationHelper.TSourceReadingCreate("British", "span", TSourceGenericBritish),
                TPronunciationHelper.TSourceReadingCreate("American", "span", TSourceGenericAmerican)));

        LAnswer answer = await source.TSourceFind("tomato", CancellationToken.None);

        Assert.Equal(
            [("British", "təˈmɑːtəʊ"), ("American", "təˈmeɪtoʊ")],
            answer.LAnswerReadings.Select(reading => (reading.LReadingVariety, reading.LReadingPhonetic)));
    }

    [Fact]
    public async Task SourceFind_SecondAttempt_FillsMissedVariety()
    {
        LSource source = TInterface.TSourceGenericCreate(
            TInterface.TSourceSpecCreate(
                "Stub",
                [
                    TPronunciationHelper.TSourceAttemptCreate(
                        TPronunciationHelper.TSourceReadingCreate("British", "span", TSourceGenericBritish)),
                    TPronunciationHelper.TSourceAttemptCreate(
                        TPronunciationHelper.TSourceReadingCreate("British", "span", "<span class=\"none\">"),
                        TPronunciationHelper.TSourceReadingCreate("American", "span", TSourceGenericAmerican)),
                ]),
            TPronunciationHelper.TSourceClientCreate(TSourceGenericBody, HttpStatusCode.OK));

        LAnswer answer = await source.TSourceFind("tomato", CancellationToken.None);

        Assert.Equal(
            [("British", "təˈmɑːtəʊ"), ("American", "təˈmeɪtoʊ")],
            answer.LAnswerReadings.Select(reading => (reading.LReadingVariety, reading.LReadingPhonetic)));
    }

    [Fact]
    public async Task SourceFind_SkipOne_ReadsSecondSpan()
    {
        LSource source = TSourceGenericCreate(
            TSourceGenericBody,
            TPronunciationHelper.TSourceAttemptCreate(
                TPronunciationHelper.TSourceReadingCreate("American", "span", TSourceGenericPhon, 1)));

        LAnswer answer = await source.TSourceFind("tomato", CancellationToken.None);

        LReading reading = Assert.Single(answer.LAnswerReadings);
        Assert.Equal("American", reading.LReadingVariety);
        Assert.Equal("təˈmeɪtoʊ", reading.LReadingPhonetic);
    }

    [Fact]
    public async Task SourceFind_FlatAttempt_ReturnsOneUntagged()
    {
        LSource source = TSourceGenericCreate(
            TSourceGenericBody,
            TPronunciationHelper.TSourceAttemptCreate(
                TPronunciationHelper.TSourceReadingCreate(string.Empty, "span", TSourceGenericPhon)));

        LAnswer answer = await source.TSourceFind("tomato", CancellationToken.None);

        LReading reading = Assert.Single(answer.LAnswerReadings);
        Assert.Equal(string.Empty, reading.LReadingVariety);
        Assert.Equal("təˈmɑːtəʊ", reading.LReadingPhonetic);
        Assert.Equal("təˈmɑːtəʊ", answer.LAnswerValue);
    }

    [Fact]
    public async Task SourceFind_GuardMissed_ReturnsBlank()
    {
        LSource source = TSourceGenericCreate(
            TSourceGenericBody,
            TPronunciationHelper.TSourceAttemptCreate(
                TPronunciationHelper.TSourceReadingCreate("British", "span", TSourceGenericBritish))
                with { LSourceAttemptGuard = "<h1>{word}</h1>" });

        LAnswer answer = await source.TSourceFind("tomato", CancellationToken.None);

        Assert.True(answer.LAnswerEmpty);
        Assert.True(answer.LAnswerReached);
    }

    [Fact]
    public async Task SourceFind_FollowPointer_ReadsTargetPage()
    {
        LSource source = TInterface.TSourceGenericCreate(
            TInterface.TSourceSpecCreate(
                "Stub",
                [
                    TPronunciationHelper.TSourceFollowCreate(
                        TInterface.TSourceReadingCreate(string.Empty, "regex", "see=([^;]+);", 1, null, false, 0),
                        TInterface.TSourceReadingCreate(string.Empty, "regex", "m=([^;]+);", 1, null, false, 0)),
                ]),
            TPronunciationHelper.TSourceClientCreate(new Dictionary<string, string>
            {
                ["https://example.test/%E4%B8%AD%E5%9B%BD"] = "see=中國;",
                ["https://example.test/%E4%B8%AD%E5%9C%8B"] = "m=zhōngguó;",
            }));

        LAnswer answer = await source.TSourceFind("中国", CancellationToken.None);

        Assert.Equal("zhōngguó", answer.LAnswerValue);
    }

    [Fact]
    public async Task SourceFind_FollowLoop_StopsAtHopLimit()
    {
        LSource source = TInterface.TSourceGenericCreate(
            TInterface.TSourceSpecCreate(
                "Stub",
                [
                    TPronunciationHelper.TSourceFollowCreate(
                        TInterface.TSourceReadingCreate(string.Empty, "regex", "see=([^;]+);", 1, null, false, 0),
                        TInterface.TSourceReadingCreate(string.Empty, "regex", "m=([^;]+);", 1, null, false, 0)),
                ]),
            TPronunciationHelper.TSourceClientCreate(new Dictionary<string, string>
            {
                ["https://example.test/a"] = "see=b;",
                ["https://example.test/b"] = "see=a;",
            }));

        LAnswer answer = await source.TSourceFind("a", CancellationToken.None);

        Assert.True(answer.LAnswerEmpty);
        Assert.True(answer.LAnswerReached);
    }

    [Fact]
    public async Task SourceFind_FollowPointer_CarriesToNextAttempt()
    {
        LSource source = TInterface.TSourceGenericCreate(
            TInterface.TSourceSpecCreate(
                "Stub",
                [
                    TPronunciationHelper.TSourceFollowCreate(
                        TInterface.TSourceReadingCreate(string.Empty, "regex", "see=([^;]+);", 1, null, false, 0),
                        TInterface.TSourceReadingCreate(string.Empty, "regex", "m=([^;]+);", 1, null, false, 0)),
                    TInterface.TSourceAttemptCreate(
                        ["https://example.test/html/{word}"],
                        [TInterface.TSourceReadingCreate(string.Empty, "regex", "b=([^;]+);", 1, null, false, 0)],
                        null,
                        null,
                        null),
                ]),
            TPronunciationHelper.TSourceClientCreate(new Dictionary<string, string>
            {
                ["https://example.test/a"] = "see=b;",
                ["https://example.test/b"] = "none",
                ["https://example.test/html/b"] = "b=ㄅ;",
            }));

        LAnswer answer = await source.TSourceFind("a", CancellationToken.None);

        Assert.Equal("ㄅ", answer.LAnswerValue);
    }

    [Fact]
    public async Task SourceFind_ServerFailure_ReturnsLost()
    {
        LSource source = TSourceGenericCreate(
            string.Empty,
            HttpStatusCode.InternalServerError,
            TPronunciationHelper.TSourceAttemptCreate(
                TPronunciationHelper.TSourceReadingCreate("British", "span", TSourceGenericBritish)));

        LAnswer answer = await source.TSourceFind("tomato", CancellationToken.None);

        Assert.True(answer.LAnswerEmpty);
        Assert.False(answer.LAnswerReached);
    }

    private static LSource TSourceGenericCreate(string body, LSourceAttempt attempt)
    {
        return TSourceGenericCreate(body, HttpStatusCode.OK, attempt);
    }

    private static LSource TSourceGenericCreate(string body, HttpStatusCode status, LSourceAttempt attempt)
    {
        return TInterface.TSourceGenericCreate(
            TInterface.TSourceSpecCreate("Stub", [attempt]),
            TPronunciationHelper.TSourceClientCreate(body, status));
    }
}
