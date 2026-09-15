using Llyn.Core;
using Xunit;

namespace Llyn.Tests;

public sealed class TRespelling
{
    [Theory]
    [InlineData("ˈdɪf.ər.əns", "ˈdɪfərəns")]
    [InlineData("ˈdɪf ər əns", "ˈdɪfərəns")]
    [InlineData("ˌɪn.təˈnæʃ.ən.əl", "ˌɪntəˈnæʃənəl")]
    [InlineData(" ˈpəʊkə feɪs ", "ˈpəʊkəfeɪs")]
    [InlineData("ˈt͡ʃiːz.keɪk", "ˈt͡ʃiːzkeɪk")]
    [InlineData("ˈti·tʃər", "ˈtitʃər")]
    [InlineData("ˈti\u2027tʃər", "ˈtitʃər")]
    [InlineData("f\u2009ɛ\u2009j\u2009s", "fɛjs")]
    [InlineData("é", "é")]
    public void ReadingNormalize_SourceForm_DropsDotsAndSpaces(string phonetic, string expected)
    {
        Assert.Equal(expected, TInterface.TReadingNormalize(phonetic));
    }

    [Fact]
    public void RespellingRuleCreate_SamePatternAndReplacement_AreEqual()
    {
        LRespellingRule left = TInterface.TRespellingRuleCreate("eɪ", "ej");
        LRespellingRule right = TInterface.TRespellingRuleCreate("eɪ", "ej");
        LRespellingRule other = TInterface.TRespellingRuleCreate("eɪ", "ɛj");

        Assert.Equal(left, right);
        Assert.Equal(left.GetHashCode(), right.GetHashCode());
        Assert.NotEqual(left, other);
    }

    [Fact]
    public void RespellingResolve_TwoRules_RunsInWrittenOrder()
    {
        LRespelling group = TInterface.TRespellingCreate(
            "Chain",
            [],
            [TInterface.TRespellingRuleCreate("ɛ", "e"), TInterface.TRespellingRuleCreate("eə", "eː")]);

        Assert.Equal("beː", group.TRespellingResolve("bɛə", string.Empty));
    }

    [Fact]
    public void RespellingResolve_ScopedGroup_SkipsOtherVarieties()
    {
        LRespelling group = TInterface.TRespellingCreate(
            "British", ["British"], [TInterface.TRespellingRuleCreate("æ", "a")]);

        Assert.Equal("hæt", group.TRespellingResolve("hæt", string.Empty));
        Assert.Equal("hæt", group.TRespellingResolve("hæt", "American"));
        Assert.Equal("hat", group.TRespellingResolve("hæt", "British"));
    }

    [Fact]
    public void RespellingResolve_UnscopedGroup_AppliesToEveryVariety()
    {
        LRespelling group = TInterface.TRespellingCreate(
            "Shared", [], [TInterface.TRespellingRuleCreate("æ", "a")]);

        Assert.Equal("hat", group.TRespellingResolve("hæt", string.Empty));
        Assert.Equal("hat", group.TRespellingResolve("hæt", "American"));
    }

    [Fact]
    public void RespellingResolve_DecomposedInput_MatchesComposedPattern()
    {
        LRespelling group = TInterface.TRespellingCreate(
            "Accent", [], [TInterface.TRespellingRuleCreate("é", "E")]);

        Assert.Equal("E", group.TRespellingResolve("é", string.Empty));
        Assert.Equal("E", group.TRespellingResolve("é", string.Empty));
    }

    [Fact]
    public void RespellingScan_TwoGroups_ChainsOutput()
    {
        LRespelling[] groups =
        [
            TInterface.TRespellingCreate("First", [], [TInterface.TRespellingRuleCreate("ɛ", "e")]),
            TInterface.TRespellingCreate("Second", [], [TInterface.TRespellingRuleCreate("eə", "eː")]),
        ];

        Assert.Equal("beː", TInterface.TRespellingScan(groups, "bɛə", string.Empty));
        Assert.Equal("bɛə", TInterface.TRespellingScan([], "bɛə", string.Empty));
    }

    [Theory]
    [InlineData("feɪs", "British")]
    [InlineData("feɪs", "American")]
    [InlineData("ˈhæpi", "British")]
    [InlineData("ˈhæpi", "American")]
    [InlineData("ɡəʊ", "British")]
    [InlineData("ɡoʊ", "American")]
    [InlineData("bɜːd", "British")]
    [InlineData("bɝd", "American")]
    [InlineData("bɝːd", "American")]
    [InlineData("bɜːrd", "American")]
    [InlineData("bɜrd", "American")]
    [InlineData("ˈtiːtʃɚ", "American")]
    [InlineData("ˈtitʃər", "American")]
    [InlineData("fil", "American")]
    [InlineData("ˈɪnfluəns", "British")]
    [InlineData("ju", "American")]
    [InlineData("bɛə", "British")]
    [InlineData("beə", "British")]
    [InlineData("təˈmɑːtəʊ", "British")]
    [InlineData("təˈmeɪtoʊ", "American")]
    [InlineData("ˈtʃiːzkeɪk", "British")]
    [InlineData("ˈtʃiːzkeɪk", "American")]
    public void RespellingScan_EnglishPack_IsIdempotent(string phonetic, string variety)
    {
        IReadOnlyList<LRespelling> groups = TInterface.TLanguageLoad("English").LLanguageRespellings;

        string once = TInterface.TRespellingScan(groups, phonetic, variety);

        Assert.Equal(once, TInterface.TRespellingScan(groups, once, variety));
    }

    [Theory]
    [InlineData("feɪs", "British", "fejs")]
    [InlineData("ˈhæpi", "British", "ˈhapij")]
    [InlineData("ˈhæpi", "American", "ˈhæpij")]
    [InlineData("ɡəʊ", "British", "ɡəw")]
    [InlineData("ɡoʊ", "American", "ɡow")]
    [InlineData("bɜːd", "British", "bəːd")]
    [InlineData("bɝd", "American", "bərd")]
    [InlineData("bɝːd", "American", "bərd")]
    [InlineData("bɜːrd", "American", "bərd")]
    [InlineData("bɜrd", "American", "bərd")]
    [InlineData("ˈtiːtʃɚ", "American", "ˈtijtʃər")]
    [InlineData("ˈtitʃər", "American", "ˈtijtʃər")]
    [InlineData("fil", "American", "fijl")]
    [InlineData("fiːl", "British", "fijl")]
    [InlineData("ˈɪnfluəns", "British", "ˈɪnfluwəns")]
    [InlineData("ju", "American", "juw")]
    [InlineData("pʊt", "British", "pʊt")]
    [InlineData("pʊt", "American", "pʊt")]
    [InlineData("kɪt", "British", "kɪt")]
    [InlineData("bɛə", "British", "beː")]
    [InlineData("beə", "British", "beː")]
    [InlineData("feɪs", "American", "fejs")]
    [InlineData("bɛd", "American", "bed")]
    [InlineData("feɪs", "", "feɪs")]
    public void RespellingScan_EnglishPack_RecastsKnownForms(string phonetic, string variety, string expected)
    {
        IReadOnlyList<LRespelling> groups = TInterface.TLanguageLoad("English").LLanguageRespellings;

        Assert.Equal(expected, TInterface.TRespellingScan(groups, phonetic, variety));
    }

    [Theory]
    [InlineData("ʈ͡ʂʊŋ⁵⁵", "tʂuŋ⁵⁵")]
    [InlineData("t͡ɕi⁵⁵", "tsi⁵⁵")]
    [InlineData("xän⁵¹", "han⁵¹")]
    [InlineData("xän⁵¹", "han⁵¹")]
    [InlineData("t͡sz̩⁵¹", "tsə⁵¹")]
    [InlineData("kɤ⁵¹", "kə⁵¹")]
    [InlineData("kuɤ³⁵", "kə³⁵")]
    [InlineData("pɪn⁵⁵", "pin⁵⁵")]
    [InlineData("pʊ⁵¹", "pu⁵¹")]
    [InlineData("jɛ⁵¹", "je⁵¹")]
    [InlineData("pʰɔ⁵¹", "pʰo⁵¹")]
    [InlineData("pɑŋ⁵⁵", "paŋ⁵⁵")]
    [InlineData("mä⁵⁵", "ma⁵⁵")]
    public void RespellingScan_MandarinPack_RecastsKnownForms(string phonetic, string expected)
    {
        IReadOnlyList<LRespelling> groups = TInterface.TLanguageLoad("Mandarin").LLanguageRespellings;

        Assert.Equal(expected, TInterface.TRespellingScan(groups, phonetic, string.Empty));
        Assert.Equal(expected, TInterface.TRespellingScan(groups, expected, string.Empty));
    }
}
