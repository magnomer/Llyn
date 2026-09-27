using System;
using System.Collections.Generic;
using Llyn.Conduct;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TAtelierRespelling
{
    [Theory]
    [InlineData(false, false, "[", "]")]
    [InlineData(false, true, "[", "]")]
    [InlineData(true, false, "[", "]")]
    [InlineData(true, true, "/", "/")]
    public void RespellingMarkRead_Modes_ReadsBrackets(bool respelled, bool phonemic, string opener, string closer)
    {
        using LEngine engine = TRigFake.TRigFakeStart(TRigFake.TRigFakeBuild());
        using CAtelier atelier = TAtelierRespellingCreate(engine, respelled, phonemic);

        CRespellingMark mark = atelier.CAtelierRespelling.CRespellingMarkRead("en");

        Assert.Equal(new CRespellingMark(respelled, opener, closer), mark);
    }

    [Fact]
    public void RespellingMarkRead_Schemed_ReadsBare()
    {
        using LEngine engine = TRigFake.TRigFakeStart(TRigFake.TRigFakeBuild());
        using CAtelier atelier = TAtelierRespellingCreate(engine, true, true);

        CRespellingMark mark = atelier.CAtelierRespelling.CRespellingMarkRead("en", true);

        Assert.Equal(new CRespellingMark(false, string.Empty, string.Empty), mark);
        Assert.Equal("ipa", CRespelling.CRespellingResolve(mark, "ipa", "respelt"));
    }

    [Theory]
    [InlineData(false, false, "")]
    [InlineData(true, false, "")]
    [InlineData(false, true, "/")]
    [InlineData(true, true, "/")]
    public void RespellingReflexRead_Modes_SlashesOnlyPhonemic(bool respelled, bool phonemic, string slash)
    {
        using LEngine engine = TRigFake.TRigFakeStart(TRigFake.TRigFakeBuild());
        using CAtelier atelier = TAtelierRespellingCreate(engine, respelled, phonemic);

        CRespellingMark mark = atelier.CAtelierRespelling.CRespellingReflexRead("en");

        Assert.Equal(new CRespellingMark(respelled, slash, slash), mark);
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

    private static CAtelier TAtelierRespellingCreate(LEngine engine, bool respelled, bool phonemic)
    {
        return TInterfaceConduct.TAtelierCreate(engine, new Dictionary<string, Func<object?[]?, object?>>
        {
            ["LEngineRespellingCheck"] = _ => respelled,
            ["LEnginePhonemicCheck"] = _ => phonemic,
        });
    }
}
