using Llyn.Core;
using Xunit;

namespace Llyn.Tests;

public sealed class THypothesis
{
    private static readonly IReadOnlyList<LRespellingRule> THypothesisStops =
    [
        TInterface.TRespellingRuleCreate("ng$", "k"),
        TInterface.TRespellingRuleCreate("n$", "t"),
    ];

    private static readonly LHypothesis THypothesisTables = TInterface.THypothesisCreate(
        new Dictionary<string, string>
        {
            ["疑"] = "ng", ["見"] = "k", ["端"] = "t", ["定"] = "d", ["匣"] = "x", ["來"] = "l",
        },
        new Dictionary<string, string>
        {
            ["模 一"] = "o",
            ["寒 一"] = "an",
            ["寒 一 合"] = "wan",
            ["真 三"] = "in",
            ["真 三 合"] = "win",
            ["唐 一"] = "ang",
        },
        new Dictionary<string, IReadOnlyList<LHypothesisTone>>
        {
            ["平"] =
            [
                TInterface.THypothesisToneCreate("^[ptcskʔh]", [], "1"),
                TInterface.THypothesisToneCreate(
                    "^[bdqzg]", [TInterface.TRespellingRuleCreate("^(.)", "$1h")], "2"),
                TInterface.THypothesisToneCreate("", [], "2"),
            ],
            ["上"] =
            [
                TInterface.THypothesisToneCreate("^[ptcskʔh]", [TInterface.TRespellingRuleCreate("$", "ʔ")], "3"),
                TInterface.THypothesisToneCreate("^[mnjwl]", [TInterface.TRespellingRuleCreate("$", "ʔ")], "4S"),
                TInterface.THypothesisToneCreate("", [TInterface.TRespellingRuleCreate("$", "ʔ")], "4"),
            ],
            ["入"] =
            [
                TInterface.THypothesisToneCreate("^[ptcskʔh]", THypothesisStops, "7"),
                TInterface.THypothesisToneCreate("", THypothesisStops, "8"),
            ],
        },
        [
            TInterface.THypothesisPlaceCreate("dental", ["端", "定"]),
            TInterface.THypothesisPlaceCreate("velar", ["疑", "見", "匣"]),
        ]);

    [Theory]
    [InlineData("疑", "模", "一", "平", false, "ngo", "2")]
    [InlineData("見", "寒", "一", "平", false, "kan", "1")]
    [InlineData("見", "寒", "一", "平", true, "kwan", "1")]
    [InlineData("見", "真B", "三", "平", true, "kwin", "1")]
    [InlineData("見", "真A", "三", "平", false, "kin", "1")]
    [InlineData("定", "唐", "一", "平", false, "dhang", "2")]
    [InlineData("匣", "唐", "一", "平", false, "xang", "2")]
    [InlineData("見", "模", "一", "上", false, "koʔ", "3")]
    [InlineData("來", "模", "一", "上", false, "loʔ", "4S")]
    [InlineData("定", "模", "一", "上", false, "doʔ", "4")]
    [InlineData("端", "唐", "一", "入", false, "tak", "7")]
    [InlineData("見", "寒", "一", "入", true, "kwat", "7")]
    [InlineData("定", "唐", "一", "入", false, "dak", "8")]
    [InlineData("疑", "模", "一", "入", false, "ngo", "8")]
    public void HypothesisResolve_KnownParts_JoinsOnsetFinalAndTone(
        string initial, string rime, string division, string tone, bool rounded, string text, string label)
    {
        LFanqieRow row = TInterface.TFanqieRowCreate(initial, rime, "heading", division, tone, rounded);

        LHypothesisSound? sound = THypothesisTables.THypothesisResolve(row);

        Assert.NotNull(sound);
        Assert.Equal((text, label), (sound.LHypothesisSoundText, sound.LHypothesisSoundClass));
    }

    [Fact]
    public void HypothesisResolve_ToneOutsideTable_ReadsBlankClass()
    {
        LFanqieRow row = TInterface.TFanqieRowCreate("見", "模", "heading", "一", "去", false);

        LHypothesisSound? sound = THypothesisTables.THypothesisResolve(row);

        Assert.NotNull(sound);
        Assert.Equal(("ko", ""), (sound.LHypothesisSoundText, sound.LHypothesisSoundClass));
    }

    [Theory]
    [InlineData("曉", "模", "一")]
    [InlineData("疑", "魚", "三")]
    [InlineData("疑", "模", "三")]
    [InlineData("", "模", "一")]
    [InlineData("疑", "", "一")]
    public void HypothesisResolve_UnknownPart_ReadsNull(string initial, string rime, string division)
    {
        LFanqieRow row = TInterface.TFanqieRowCreate(initial, rime, "heading", division, "平", false);

        Assert.Null(THypothesisTables.THypothesisResolve(row));
    }

    [Theory]
    [InlineData("見", "k")]
    [InlineData("來", "l")]
    [InlineData("曉", null)]
    [InlineData("", null)]
    public void HypothesisInitialFind_TableLookup_ReadsOnsetAlone(string initial, string? onset)
    {
        LFanqieRow row = TInterface.TFanqieRowCreate(initial, "模", "模", "一", "平", false);

        Assert.Equal(onset, THypothesisTables.THypothesisInitialFind(row));
    }

    [Theory]
    [InlineData("端", "dental", 0)]
    [InlineData("定", "dental", 1)]
    [InlineData("疑", "velar", 2)]
    [InlineData("匣", "velar", 4)]
    [InlineData("來", null, -1)]
    public void HypothesisPlaceFind_PackOrder_RanksAcrossPlaces(string initial, string? place, int rank)
    {
        Assert.Equal(place, THypothesisTables.THypothesisPlaceFind(initial)?.LHypothesisLocusName);
        Assert.Equal(rank, THypothesisTables.THypothesisRankRead(initial));
    }

    [Fact]
    public void HypothesisResolve_RoundedWithoutRoundedKey_ReadsPlainFinal()
    {
        LFanqieRow row = TInterface.TFanqieRowCreate("疑", "模", "模", "一", "平", true);

        Assert.Equal("ngo", THypothesisTables.THypothesisResolve(row)?.LHypothesisSoundText);
    }
}
