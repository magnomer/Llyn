using System.Collections.Generic;
using Llyn.Core;
using Xunit;

namespace Llyn.Tests;

public sealed class TDiwei
{
    [Fact]
    public void DiweiRankRead_RimeDivisionOrder()
    {
        LHypothesis hypothesis = TInterfaceFanqie.THypothesisCreate(
            new Dictionary<string, string>(),
            new Dictionary<string, string>(),
            new Dictionary<string, IReadOnlyList<LHypothesisTone>>(),
            [
                TInterfaceFanqie.THypothesisPlaceCreate("dental", ["端"]),
                TInterfaceFanqie.THypothesisPlaceCreate("velar", ["見"]),
            ]);

        Assert.Equal(
            [0, 1, 2, 3, 4, int.MaxValue],
            new[] { "一", "二", "三", "四", "五", "" }
                .Select(division => TInterfaceFanqie.TDiweiRankRead(LDiwei.LDiweiInitial, division, hypothesis)));
        Assert.Equal(
            [1, 0, 2, int.MaxValue],
            new[] { "velar", "dental", "labial", "" }
                .Select(place => TInterfaceFanqie.TDiweiRankRead(LDiwei.LDiweiRime, place, hypothesis)));
        Assert.Equal(0, TInterfaceFanqie.TDiweiRankRead(LDiwei.LDiweiRime, "dental", null));
        Assert.Equal(int.MaxValue, TInterfaceFanqie.TDiweiRankNormalize(-1));
        Assert.Equal(3, TInterfaceFanqie.TDiweiRankNormalize(3));
    }

    [Theory]
    [InlineData(false, "l", "爛")]
    [InlineData(true, "L", "蘭")]
    public void DiweiSectionScan_TallyUnderHeading_KeepsOnlyTheShownSet(bool respelled, string text, string character)
    {
        LDiweiSection section = TInterfaceFanqie.TDiweiSectionScan(respelled, static _ => null);

        Assert.True(section.LDiweiSectionSwitched);
        Assert.Equal(respelled, section.LDiweiSectionRespelled);
        LTallyRow row = section.LDiweiSectionTallies[0];
        Assert.Equal(("Cantonese", "literary"), (row.LTallyRowLanguage, row.LTallyRowKind));
        LTallyMark mark = Assert.Single(row.LTallyRowMarks);
        Assert.Equal((text, 1), (mark.LTallyMarkText, mark.LTallyMarkCount));
        Assert.Equal([character], mark.LTallyMarkCharacters);
        Assert.Equal(respelled ? 1 : 2, section.LDiweiSectionTallies.Count);
    }

    [Fact]
    public void DiweiSectionScan_CharactersStoredOutOfCodepointOrder_ListsLineInCodepointOrder()
    {
        LDiweiSection section = Assert.Single(TInterfaceFanqie.TDiweiSectionScan(
        [
            TInterface.TFanqieRowCreate("\U00020000", "book", "text", "來", "寒", "寒", "一", "平", id: 1),
            TInterface.TFanqieRowCreate("\uF900", "book", "text", "來", "寒", "寒", "一", "平", id: 2),
            TInterface.TFanqieRowCreate("林", "book", "text", "來", "寒", "寒", "一", "平", id: 3),
            TInterface.TFanqieRowCreate("\uF900", "book", "text", "來", "寒", "寒", "一", "平", id: 4),
        ]));

        LDiweiLine line = Assert.Single(section.LDiweiSectionLines);
        Assert.Equal(["林", "\uF900", "\U00020000"], line.LDiweiLineCharacters);
    }

    [Fact]
    public void DiweiSectionScan_HeadingsTiedOnRank_ListsByHeadingCodepoint()
    {
        IReadOnlyList<LDiweiSection> sections = TInterfaceFanqie.TDiweiSectionScan(
            [
                TInterface.TFanqieRowCreate("甲", "book", "text", "來", "寒", "寒", string.Empty, "平", id: 1),
                TInterface.TFanqieRowCreate("乙", "book", "text", "來", "寒", "寒", "\U00020000", "平", id: 2),
                TInterface.TFanqieRowCreate("丙", "book", "text", "來", "寒", "寒", "\uF900", "平", id: 3),
                TInterface.TFanqieRowCreate("丁", "book", "text", "來", "寒", "寒", "二", "平", id: 4),
            ]);

        Assert.Equal(
            ["二", "\uF900", "\U00020000", string.Empty], sections.Select(section => section.LDiweiSectionLabel));
    }

    [Fact]
    public void DiweiSectionScan_LinesTiedOnRank_ListsByLabelCodepointThenUnroundedFirst()
    {
        LDiweiSection section = Assert.Single(TInterfaceFanqie.TDiweiSectionScan(
        [
            TInterface.TFanqieRowCreate("丹", "book", "text", "來", "寒", "寒", "一", "平", true, id: 1),
            TInterface.TFanqieRowCreate("爛", "book", "text", "來", "寒", "寒", "一", "平", false, id: 2),
            TInterface.TFanqieRowCreate("金", "book", "text", "來", "\U00020000", "\U00020000", "一", "平", id: 3),
            TInterface.TFanqieRowCreate("林", "book", "text", "來", "\uF900", "\uF900", "一", "平", id: 4),
            TInterface.TFanqieRowCreate("侵", "book", "text", "來", "侵", "侵", "一", "平", id: 5),
        ]));

        Assert.Equal(
            [("侵", false), ("寒", false), ("寒", true), ("\uF900", false), ("\U00020000", false)],
            section.LDiweiSectionLines.Select(line => (line.LDiweiLineLabel, line.LDiweiLineRounded)));
    }

    [Fact]
    public void TallyMarkScan_CharactersStoredOutOfCodepointOrder_ListsMarkInCodepointOrder()
    {
        IReadOnlyList<LTallyMark> marks = TInterfaceFanqie.TTallyMarkScan(
            new Dictionary<string, List<string>> { ["l"] = ["\U00020000", "\uF900", "林"] });

        LTallyMark mark = Assert.Single(marks);
        Assert.Equal(["林", "\uF900", "\U00020000"], mark.LTallyMarkCharacters);
    }

    [Theory]
    [InlineData(null, "一")]
    [InlineData("Division {1}", "Division I")]
    [InlineData("{0}등", "一등")]
    public void DiweiSectionScan_DivisionHeading_LabelsItByTheLocalizedPattern(string? pattern, string label)
    {
        LDiweiSection section = TInterfaceFanqie.TDiweiSectionScan(
            false, key => key == "Yunjing.Division" ? pattern : null);

        Assert.Equal(label, section.LDiweiSectionLabel);
    }
}
