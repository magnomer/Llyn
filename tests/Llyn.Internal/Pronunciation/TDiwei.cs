using System.Collections.Generic;
using Llyn.Core;
using Xunit;

namespace Llyn.Tests;

public sealed class TDiwei
{
    [Fact]
    public void DiweiRankRead_RimeDivisionOrder()
    {
        LHypothesis hypothesis = TInterface.THypothesisCreate(
            new Dictionary<string, string>(),
            new Dictionary<string, string>(),
            new Dictionary<string, IReadOnlyList<LHypothesisTone>>(),
            [
                TInterface.THypothesisPlaceCreate("dental", ["端"]),
                TInterface.THypothesisPlaceCreate("velar", ["見"]),
            ]);

        Assert.Equal(
            [0, 1, 2, 3, 4, int.MaxValue],
            new[] { "一", "二", "三", "四", "五", "" }
                .Select(division => TInterface.TDiweiRankRead(LDiwei.LDiweiInitial, division, hypothesis)));
        Assert.Equal(
            [1, 0, 2, int.MaxValue],
            new[] { "velar", "dental", "labial", "" }
                .Select(place => TInterface.TDiweiRankRead(LDiwei.LDiweiRime, place, hypothesis)));
        Assert.Equal(0, TInterface.TDiweiRankRead(LDiwei.LDiweiRime, "dental", null));
        Assert.Equal(int.MaxValue, TInterface.TDiweiRankNormalize(-1));
        Assert.Equal(3, TInterface.TDiweiRankNormalize(3));
    }

    [Theory]
    [InlineData(false, "l", "爛")]
    [InlineData(true, "L", "蘭")]
    public void DiweiSectionScan_TallyUnderHeading_KeepsOnlyTheShownSet(bool respelled, string text, string character)
    {
        LDiweiSection section = TInterface.TDiweiSectionScan(respelled);

        Assert.True(section.LDiweiSectionSwitched);
        Assert.Equal(respelled, section.LDiweiSectionRespelled);
        LTallyRow row = section.LDiweiSectionTallies[0];
        Assert.Equal(("Cantonese", "literary"), (row.LTallyRowLanguage, row.LTallyRowKind));
        LTallyMark mark = Assert.Single(row.LTallyRowMarks);
        Assert.Equal((text, 1), (mark.LTallyMarkText, mark.LTallyMarkCount));
        Assert.Equal([character], mark.LTallyMarkCharacters);
        Assert.Equal(respelled ? 1 : 2, section.LDiweiSectionTallies.Count);
    }
}
