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
    public async Task SourceFind_FlatAttempt_ReturnsEveryUntagged()
    {
        LSource source = TSourceGenericCreate(
            TSourceGenericBody,
            TPronunciationHelper.TSourceAttemptCreate(
                TPronunciationHelper.TSourceReadingCreate(string.Empty, "span", TSourceGenericPhon, every: true)));

        LAnswer answer = await source.TSourceFind("tomato", CancellationToken.None);

        Assert.All(answer.LAnswerReadings, reading => Assert.Equal(string.Empty, reading.LReadingVariety));
        Assert.Equal(["təˈmɑːtəʊ", "təˈmeɪtoʊ"], answer.LAnswerReadings.Select(reading => reading.LReadingPhonetic));
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
    public async Task SourceFind_DecodedAttempt_MatchesEntityWrittenCharacters()
    {
        const string body = "<title>&#x6574; - site</title><td>&#x56DB;&#x5E93;</td><td>&#x6574;</td><td>48825</td>";
        LSourceReading count = TInterface.TSourceReadingCreate(
            "1", "regex", "<td>四库</td><td>{word}</td><td>(\\d+)</td>", 1, null, false, 0);
        LSource source = TSourceGenericCreate(
            body,
            TPronunciationHelper.TSourceAttemptCreate(count)
                with { LSourceAttemptGuard = "<title>{word} - ", LSourceAttemptDecoded = true });
        LSource plain = TSourceGenericCreate(
            body,
            TPronunciationHelper.TSourceAttemptCreate(count) with { LSourceAttemptGuard = "<title>{word} - " });

        LAnswer decoded = await source.TSourceFind("整", CancellationToken.None);
        LAnswer raw = await plain.TSourceFind("整", CancellationToken.None);

        Assert.Equal("48825", Assert.Single(decoded.LAnswerReadings).LReadingPhonetic);
        Assert.True(raw.LAnswerEmpty);
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
    public async Task SourceFind_RepeatedPattern_ReturnsEveryMatch()
    {
        LSource source = TSourceGenericCreate(
            TSourceGenericPhon + "/zɪŋ²²/</span>" + TSourceGenericPhon + "/lɛːŋ³³/</span>" +
            TSourceGenericPhon + "/zɪŋ²²/</span>",
            TPronunciationHelper.TSourceAttemptCreate(
                TPronunciationHelper.TSourceReadingCreate(string.Empty, "span", TSourceGenericPhon, every: true)));

        LAnswer answer = await source.TSourceFind("靚", CancellationToken.None);

        Assert.Equal(["zɪŋ²²", "lɛːŋ³³"], answer.LAnswerReadings.Select(reading => reading.LReadingPhonetic));
    }

    [Fact]
    public async Task SourceFind_RepeatedPattern_ReturnsFirstMatchOnly()
    {
        LSource source = TSourceGenericCreate(
            TSourceGenericPhon + "/breɪk/</span>" + TSourceGenericPhon + "/brəʊk/</span>" +
            TSourceGenericPhon + "/ˈbrəʊkən/</span>",
            TPronunciationHelper.TSourceAttemptCreate(
                TPronunciationHelper.TSourceReadingCreate(string.Empty, "span", TSourceGenericPhon)));

        LAnswer answer = await source.TSourceFind("break", CancellationToken.None);

        Assert.Equal("breɪk", Assert.Single(answer.LAnswerReadings).LReadingPhonetic);
    }

    [Fact]
    public async Task SourceFind_RepeatedPattern_SkipsThenReadsFirst()
    {
        LSource source = TSourceGenericCreate(
            "x=a;x=b;x=c;",
            TPronunciationHelper.TSourceAttemptCreate(
                TInterface.TSourceReadingCreate(string.Empty, "regex", "x=([^;]+);", 1, null, false, 1)));

        LAnswer answer = await source.TSourceFind("x", CancellationToken.None);

        Assert.Equal("b", Assert.Single(answer.LAnswerReadings).LReadingPhonetic);
    }

    [Fact]
    public async Task SourceFind_CommaInsideSpan_SplitsAlternatives()
    {
        LSource source = TSourceGenericCreate(
            TSourceGenericPhon + "/hɔːk̚² saːŋ⁵⁵/, /hɔːk̚² sɐŋ⁵⁵/</span>",
            TPronunciationHelper.TSourceAttemptCreate(
                TPronunciationHelper.TSourceReadingCreate(string.Empty, "span", TSourceGenericPhon)));

        LAnswer answer = await source.TSourceFind("學生", CancellationToken.None);

        Assert.Equal(
            ["hɔːk̚² saːŋ⁵⁵", "hɔːk̚² sɐŋ⁵⁵"],
            answer.LAnswerReadings.Select(reading => reading.LReadingPhonetic));
    }

    [Fact]
    public async Task SourceFind_FollowPointer_KeepsOwnReadingToo()
    {
        LSource source = TInterface.TSourceGenericCreate(
            TInterface.TSourceSpecCreate(
                "Stub",
                [
                    TPronunciationHelper.TSourceFollowCreate(
                        TInterface.TSourceReadingCreate(string.Empty, "regex", "see=([^;]+);", 1, null, false, 0),
                        TInterface.TSourceReadingCreate(string.Empty, "regex", "c=([^;]+);", 1, null, true, 0)),
                ]),
            TPronunciationHelper.TSourceClientCreate(new Dictionary<string, string>
            {
                ["https://example.test/%E5%90%AC"] = "c=jan2;see=聽;",
                ["https://example.test/%E8%81%BD"] = "c=teng1,ting3;",
            }));

        LAnswer answer = await source.TSourceFind("听", CancellationToken.None);

        Assert.Equal(["jan2", "teng1", "ting3"], answer.LAnswerReadings.Select(reading => reading.LReadingPhonetic));
    }

    [Fact]
    public async Task SourceFind_WordToken_MatchesHeadwordLiterally()
    {
        LSource source = TSourceGenericCreate(
            "file=a.b.mp3;file=a+b.mp3;",
            TPronunciationHelper.TSourceAttemptCreate(
                TInterface.TSourceReadingCreate(string.Empty, "regex", "file=({word}\\.mp3);", 1, null, false, 0)));

        LAnswer answer = await source.TSourceFind("a+b", CancellationToken.None);

        Assert.Equal("a+b.mp3", Assert.Single(answer.LAnswerReadings).LReadingPhonetic);
    }

    [Fact]
    public async Task SourceFind_SpellingRules_RecastHeadwordBeforeToken()
    {
        LSource source = TInterface.TSourceGenericCreate(
            TInterface.TSourceSpecCreate(
                "Stub",
                [
                    TPronunciationHelper.TSourceAttemptCreate(
                        TInterface.TSourceReadingCreate(
                            string.Empty, "regex", "file=({word}\\.mp3);", 1, null, false, 0)),
                ],
                [TInterface.TRespellingRuleCreate("[āă]", "a"), TInterface.TRespellingRuleCreate("ō", "o")]),
            TPronunciationHelper.TSourceClientCreate("file=rosa.mp3;file=rōsā.mp3;", HttpStatusCode.OK));

        LAnswer answer = await source.TSourceFind("rōsā", CancellationToken.None);

        Assert.Equal("rosa.mp3", Assert.Single(answer.LAnswerReadings).LReadingPhonetic);
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
