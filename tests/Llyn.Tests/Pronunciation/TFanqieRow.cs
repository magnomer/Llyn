using Llyn.Core;
using Xunit;

namespace Llyn.Tests;

public sealed class TFanqieRow
{
    [Fact]
    public void FanqieRowFormat_PartedRow_FillsEveryColumn()
    {
        LFanqieRow row = TInterface.TFanqieRowCreate(
            "完", "廣韻", "胡官切", "匣", "桓", "桓", "一", "平", true, "ɦuan", "1", 3);

        LFanqieRow shown = row.TFanqieRowFormat("Tone {0}");

        Assert.True(shown.LFanqieRowStored);
        Assert.True(shown.LFanqieRowParted);
        Assert.True(shown.LFanqieRowClassed);
        Assert.Equal("/ɦuan/", shown.LFanqieRowSlashed);
        Assert.Equal("[桓]", shown.LFanqieRowBracketed);
        Assert.Equal("一等", shown.LFanqieRowGraded);
        Assert.Equal("合", shown.LFanqieRowMedial);
        Assert.Equal(string.Empty, shown.LFanqieRowRemainder);
        Assert.Equal("Tone 1", shown.LFanqieRowLabel);
        Assert.Equal("廣韻 /ɦuan/ Tone 1 匣 " + shown.LFanqieRowCell, shown.LFanqieRowSummary);
    }

    [Fact]
    public void FanqieRowFormat_UnpartedRow_KeepsOnlyItsText()
    {
        LFanqieRow row = TInterface.TFanqieRowCreate("完", "集韻", "胡官切");

        LFanqieRow shown = row.TFanqieRowFormat(string.Empty);

        Assert.False(shown.LFanqieRowStored);
        Assert.False(shown.LFanqieRowParted);
        Assert.Equal(string.Empty, shown.LFanqieRowMedial);
        Assert.Equal("胡官切", shown.LFanqieRowRemainder);
        Assert.Equal(string.Empty, shown.LFanqieRowLabel);
        Assert.Equal("集韻 胡官切", shown.LFanqieRowSummary);
    }

    [Fact]
    public void FanqieGroupScan_TwoCharactersTwoBooks_HeadsEachCharacterOnce()
    {
        LFanqieBook guangyun = TInterface.TFanqieBookCreate("廣韻", "web");
        LFanqieBook wiki = TInterface.TFanqieBookCreate("廣韻", "wiki");
        LFanqieBook jiyun = TInterface.TFanqieBookCreate("集韻", "web");
        IReadOnlyList<LFanqieRow> rows =
        [
            TInterface.TFanqieRowCreate("完", "廣韻", "胡官切") with { LFanqieRowSource = "web" },
            TInterface.TFanqieRowCreate("完", "廣韻", "胡官切") with { LFanqieRowSource = "wiki" },
            TInterface.TFanqieRowCreate("全", "集韻", "從緣切") with { LFanqieRowSource = "web" },
            TInterface.TFanqieRowCreate("全", "廣韻", "疾緣切") with { LFanqieRowSource = "web" },
        ];

        IReadOnlyList<LFanqieGroup> groups = TInterface.TFanqieGroupScan(rows, [guangyun, wiki, jiyun]);

        Assert.Equal(["完", "", "全", ""], groups.Select(group => group.LFanqieGroupHeading));
        Assert.Equal(["廣韻", "", "廣韻", "集韻"], groups.Select(group => group.LFanqieGroupLabel));
        Assert.Equal(["web", "wiki", "web", "web"], groups.Select(group => group.LFanqieGroupSource));
        Assert.Equal("從緣切", groups[3].LFanqieGroupRows[0].LFanqieRowText);
    }

    [Fact]
    public void ScriptGroupScan_StylesInPackOrder_GroupsByCharacterThenStyle()
    {
        IReadOnlyList<LScriptImage> images =
        [
            TInterface.TScriptImageCreate("完", "seal", string.Empty),
            TInterface.TScriptImageCreate("完", "seal", "whole"),
            TInterface.TScriptImageCreate("全", "bronze", "entire"),
        ];

        IReadOnlyList<LScriptGroup> groups = TInterface.TScriptGroupScan(
            images, [TInterface.TScriptStyleCreate("bronze"), TInterface.TScriptStyleCreate("seal")]);

        Assert.Equal(["完", "全"], groups.Select(group => group.LScriptGroupHeading));
        Assert.Equal(["seal", "bronze"], groups.Select(group => group.LScriptGroupStyle));
        Assert.Equal(["whole", "entire"], groups.Select(group => group.LScriptGroupGloss));
        Assert.Equal(2, groups[0].LScriptGroupImages.Count);
    }

    [Fact]
    public void FrequencyRank_BandedAndBareRows_RanksFromRareToCore()
    {
        LFrequency bare = TInterface.TFrequencyCreate("one", "12", null);
        LFrequency banded = TInterface.TFrequencyUnitCreate("two", "12", "Everyday", "per million");
        LFrequency rare = TInterface.TFrequencyCreate("three", "1", "Rare");

        Assert.Equal(0, bare.LFrequencyRank);
        Assert.Equal(3, banded.LFrequencyRank);
        Assert.Equal(1, rare.LFrequencyRank);
        Assert.Equal("per million 12", banded.LFrequencyFigure);
        Assert.Equal("12", bare.LFrequencyFigure);
    }

    [Fact]
    public void FanqieReadingFormat_RankedRows_ReadsEveryMarkedRowPerCharacter()
    {
        IReadOnlyList<LFanqieRow> rows =
        [
            TInterface.TFanqieRowCreate("平", "廣韻", "符兵切", reading: "bhien", rank: 2),
            TInterface.TFanqieRowCreate("平", "廣韻", "符兵切", reading: "bhiaeng", rank: 1),
            TInterface.TFanqieRowCreate("安", "廣韻", "烏寒切", reading: "'an", rank: 1),
            TInterface.TFanqieRowCreate("安", "廣韻", "烏寒切", reading: "'anh"),
        ];

        IReadOnlyList<LFanqieGroup> groups = TInterface.TFanqieGroupScan(
            rows, [TInterface.TFanqieBookCreate("廣韻", "廣韻")]);

        Assert.Equal("/bhiaeng, bhien 'an/", TInterface.TFanqieReadingFormat(groups, "平安"));
        Assert.Equal("/bhiaeng, bhien/", TInterface.TFanqieReadingFormat(groups, "平"));
    }

    [Fact]
    public void FanqieReadingFormat_NothingRanked_ReadsNothing()
    {
        IReadOnlyList<LFanqieRow> rows =
        [
            TInterface.TFanqieRowCreate("平", "廣韻", "符兵切", reading: "bhiaeng"),
        ];

        IReadOnlyList<LFanqieGroup> groups = TInterface.TFanqieGroupScan(
            rows, [TInterface.TFanqieBookCreate("廣韻", "廣韻")]);

        Assert.Equal(string.Empty, TInterface.TFanqieReadingFormat(groups, "平"));
    }
}
