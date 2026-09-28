using System;
using System.Collections.Generic;
using System.Linq;
using Llyn.Conduct;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TAtelierRespelling
{
    [Theory]
    [InlineData(false, "[", "]")]
    [InlineData(true, "/", "/")]
    public void RespellingMarkRead_EngineMark_CopiesTheSwitchAndBrackets(bool respelled, string opener, string closer)
    {
        List<string> asked = [];
        using LEngine engine = TRigFake.TRigFakeStart(TRigFake.TRigFakeBuild());
        using CAtelier atelier = TAtelierRespellingCreate(engine, (respelled, opener, closer), asked);

        CRespellingMark mark = atelier.CAtelierRespelling.CRespellingMarkRead("en");

        Assert.Equal(new CRespellingMark(respelled, opener, closer), mark);
        Assert.Equal(["en"], asked);
    }

    [Fact]
    public void RespellingMarkRead_Schemed_ReadsBare()
    {
        using LEngine engine = TRigFake.TRigFakeStart(TRigFake.TRigFakeBuild());
        using CAtelier atelier = TAtelierRespellingCreate(engine, (true, "/", "/"), []);

        CRespellingMark mark = atelier.CAtelierRespelling.CRespellingMarkRead("en", true);

        Assert.Equal(new CRespellingMark(false, string.Empty, string.Empty), mark);
        Assert.Equal("ipa", CRespelling.CRespellingResolve(mark, "ipa", "respelt"));
    }

    [Theory]
    [InlineData(false, false, "")]
    [InlineData(true, false, "")]
    [InlineData(false, true, "/")]
    [InlineData(true, true, "/")]
    public void RespellingReflexScan_Modes_SlashesOnlyPhonemic(bool respelled, bool phonemic, string slash)
    {
        using LEngine engine = TRigFake.TRigFakeStart(TRigFake.TRigFakeBuild());
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(
            engine,
            new Dictionary<string, Func<object?[]?, object?>>
            {
                ["LEngineGuiseRead"] = _ => new[] { TInterface.TReflexGuiseCreate(respelled, phonemic, false) },
            });

        CReflex reflex = Assert.Single(
            atelier.CAtelierRespelling.CRespellingReflexScan("Chinese", [TAtelierReflexCreate(1, "Wu", "ipa", "")]));

        Assert.Equal(new CRespellingMark(respelled, slash, slash), reflex.CReflexMark);
    }

    [Fact]
    public void RespellingReflexScan_TwoRows_AnswersEachRowReadyToShow()
    {
        List<object?[]?> asked = [];
        using LEngine engine = TRigFake.TRigFakeStart(TRigFake.TRigFakeBuild());
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(
            engine,
            new Dictionary<string, Func<object?[]?, object?>>
            {
                ["LEngineGuiseRead"] = args =>
                {
                    asked.Add(args);
                    return new[]
                    {
                        TInterface.TReflexGuiseCreate(true, false, false),
                        TInterface.TReflexGuiseCreate(true, false, true),
                    };
                },
            });

        IReadOnlyList<CReflex> reflexes = atelier.CAtelierRespelling.CRespellingReflexScan(
            "Chinese", [TAtelierReflexCreate(4, " Wu", "ipa", "respelt"), TAtelierReflexCreate(5, "Jin", "ipa", "")]);

        Assert.Equal("Chinese", asked.Single()![0]);
        Assert.Equal([" Wu", "Jin"], (IReadOnlyList<string>)asked.Single()![1]!);
        Assert.Equal([4L, 5L], reflexes.Select(static reflex => reflex.CReflexId));
        Assert.Equal(["respelt", "ipa"], reflexes.Select(static reflex => reflex.CReflexText));
        Assert.Equal([false, true], reflexes.Select(static reflex => reflex.CReflexFolded));
        Assert.Equal([true, true], reflexes.Select(static reflex => reflex.CReflexLead));
        Assert.Equal(" Wu", reflexes[0].CReflexLanguage);
        Assert.Equal([7L], reflexes[0].CReflexAnchors);
        Assert.Equal("55", reflexes[0].CReflexTone);
    }

    [Theory]
    [InlineData(true, "respelt", "respelt")]
    [InlineData(true, "", "ipa")]
    [InlineData(true, null, "ipa")]
    [InlineData(false, "respelt", "ipa")]
    public void RespellingResolve_Mark_PicksShownText(bool shown, string? respelling, string expected)
    {
        CRespellingMark mark = new(shown, "[", "]");

        Assert.Equal(expected, CRespelling.CRespellingResolve(mark, "ipa", respelling));
    }

    [Fact]
    public void RespellingResolve_Pronunciation_ReadsRespelling()
    {
        CRespellingMark mark = new(true, "[", "]");
        CPronunciationDraft spoken = new(1, "ipa", "respelt", "US", string.Empty);

        Assert.Equal("respelt", CRespelling.CRespellingResolve(mark, spoken));
    }

    [Fact]
    public void RespellingAccentRead_Pronunciation_AnswersTheRowReadyToShow()
    {
        CRespellingMark mark = new(true, "[", "]");
        CPronunciationDraft spoken = new(4, "ipa", "respelt", "Scottish", "clip.mp3");

        CAccent accent = CRespelling.CRespellingAccentRead(mark, "English", spoken);

        Assert.Equal(4, accent.CAccentId);
        Assert.Equal("respelt", accent.CAccentText);
        Assert.Equal("clip.mp3", accent.CAccentAudio);
        Assert.Equal(CSounding.CSoundingVarietyRead("English", "Scottish"), accent.CAccentVariety);
    }

    private static CReflexDraft TAtelierReflexCreate(long id, string language, string text, string respelling)
    {
        return new CReflexDraft(
            id, language, string.Empty, text, respelling, string.Empty, string.Empty, string.Empty, false, string.Empty,
            [7], "55");
    }

    private static CAtelier TAtelierRespellingCreate(
        LEngine engine, (bool, string, string) mark, List<string> asked)
    {
        return TInterfaceConduct.TAtelierCreate(engine, new Dictionary<string, Func<object?[]?, object?>>
        {
            ["LEngineMarkRead"] = args =>
            {
                asked.Add((string)args![0]!);
                return mark;
            },
        });
    }
}
