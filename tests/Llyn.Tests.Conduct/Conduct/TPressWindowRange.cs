using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Llyn.Conduct;
using Xunit;

namespace Llyn.Tests;

public sealed class TPressWindowRange
{
    [Fact]
    public async Task RangeSet_OpenEnd_IncludesToLastPage()
    {
        await TInterfaceConductPortrait.TPressWindowOpen(new TPressBoard(), static async press =>
        {
            await press.TPressDraw();

            CSPressSetting setting = press.TPressRangeSet(CSPressRange.CSPressRangeCustom, "2-");

            Assert.Equal([false, true, true], setting.CSPressSettingIncluded);
            Assert.Equal("2-", setting.CSPressSettingText);
            Assert.Null(setting.CSPressSettingFailure);
        });
    }

    [Fact]
    public async Task RangeSet_OpenStart_IncludesFromFirstPage()
    {
        await TInterfaceConductPortrait.TPressWindowOpen(new TPressBoard(), static async press =>
        {
            await press.TPressDraw();

            CSPressSetting setting = press.TPressRangeSet(CSPressRange.CSPressRangeCustom, "-2");

            Assert.Equal([true, true, false], setting.CSPressSettingIncluded);
            Assert.Null(setting.CSPressSettingFailure);
        });
    }

    [Fact]
    public async Task RangeSet_SeveralSpans_IncludesEveryNamedPage()
    {
        await TInterfaceConductPortrait.TPressWindowOpen(new TPressBoard(), static async press =>
        {
            await press.TPressDraw();

            CSPressSetting setting = press.TPressRangeSet(CSPressRange.CSPressRangeCustom, "3, 1");

            Assert.Equal([true, false, true], setting.CSPressSettingIncluded);
        });
    }

    [Theory]
    [InlineData("4")]
    [InlineData("0")]
    [InlineData("3-1")]
    [InlineData("pages")]
    public async Task RangeSet_InvalidText_ReportsPagesInvalid(string text)
    {
        await TInterfaceConductPortrait.TPressWindowOpen(new TPressBoard(), async press =>
        {
            await press.TPressDraw();

            CSPressSetting setting = press.TPressRangeSet(CSPressRange.CSPressRangeCustom, text);

            Assert.Equal("Print.PagesInvalid", setting.CSPressSettingFailure);
            Assert.Equal(text, setting.CSPressSettingText);
            Assert.Equal([true, true, true], setting.CSPressSettingIncluded);
        });
    }

    [Fact]
    public async Task PageSelect_CurrentRange_FollowsSelectedPage()
    {
        await TInterfaceConductPortrait.TPressWindowOpen(new TPressBoard(), static async press =>
        {
            await press.TPressDraw();

            CSPressSetting first = press.TPressRangeSet(CSPressRange.CSPressRangeCurrent, string.Empty);
            CSPressSetting third = press.TPressPageSelect(2);
            CSPressSetting all = press.TPressRangeSet(CSPressRange.CSPressRangeAll, string.Empty);

            Assert.Equal([true, false, false], first.CSPressSettingIncluded);
            Assert.Null(first.CSPressSettingText);
            Assert.Equal([false, false, true], third.CSPressSettingIncluded);
            Assert.Equal(2, third.CSPressSettingPage);
            Assert.Equal([true, true, true], all.CSPressSettingIncluded);
        });
    }

    [Fact]
    public async Task PageSelect_TypedNumber_ClampsToPageCount()
    {
        await TInterfaceConductPortrait.TPressWindowOpen(new TPressBoard(), static async press =>
        {
            Assert.Equal(0, press.TPressPageSelect("2").CSPressSettingPage);

            await press.TPressDraw();

            Assert.Equal(2, press.TPressPageSelect("9").CSPressSettingPage);
            Assert.Equal(1, press.TPressPageSelect("2").CSPressSettingPage);
            Assert.Equal(0, press.TPressPageSelect("0").CSPressSettingPage);
        });
    }

    [Fact]
    public async Task PageSelect_NotNumber_KeepsPage()
    {
        await TInterfaceConductPortrait.TPressWindowOpen(new TPressBoard(), static async press =>
        {
            await press.TPressDraw();
            press.TPressPageSelect(1);

            Assert.Equal(1, press.TPressPageSelect("second").CSPressSettingPage);
        });
    }

    [Fact]
    public async Task SettingRead_BeforeDraw_ListsNoPages()
    {
        await TInterfaceConductPortrait.TPressWindowOpen(new TPressBoard(), static press =>
        {
            CSPressSetting setting = press.TPressSettingRead();

            Assert.Empty(setting.CSPressSettingIncluded);
            Assert.True(setting.CSPressSettingStale);
            return Task.CompletedTask;
        });
    }

    [Fact]
    public async Task SettingRead_AfterDraw_IncludesEveryPage()
    {
        await TInterfaceConductPortrait.TPressWindowOpen(new TPressBoard(), static async press =>
        {
            await press.TPressDraw();

            Assert.Equal([true, true, true], press.TPressSettingRead().CSPressSettingIncluded);
        });
    }

    [Fact]
    public async Task SettingRead_OneSheet_UsesSingularTally()
    {
        await TInterfaceConductPortrait.TPressWindowOpen(new TPressBoard(), static async press =>
        {
            await press.TPressDraw();
            CSPressSetting all = press.TPressSettingRead();
            CSPressSetting one = press.TPressRangeSet(CSPressRange.CSPressRangeCustom, "2");
            CSPressSetting save = press.TPressPrinterSelect(null);

            Assert.Equal(3, all.CSPressSettingSheets);
            Assert.Equal("Print.Sheets", all.CSPressSettingTally);
            Assert.Equal(1, one.CSPressSettingSheets);
            Assert.Equal("Print.SheetsOne", one.CSPressSettingTally);
            Assert.Null(save.CSPressSettingSheets);
            Assert.Equal("Print.Sheets", save.CSPressSettingTally);
        });
    }

    [Fact]
    public async Task SettingRead_TwoSided_HalvesSheets()
    {
        await TInterfaceConductPortrait.TPressWindowOpen(new TPressBoard(), static async press =>
        {
            await press.TPressDraw();

            Assert.Equal(2, press.TPressSideSet(CSPressSide.CSPressSideLong).CSPressSettingSheets);
            Assert.Equal(4, press.TPressCopiesSet("2").CSPressSettingSheets);
        });
    }

    [Fact]
    public async Task RangeSet_TextAlone_KeepsCustomRange()
    {
        List<CSPressSetting> raised = [];
        await TInterfaceConductPortrait.TPressWindowOpen(new TPressBoard(), async press =>
        {
            await press.TPressDraw();
            press.TPressRangeSet(CSPressRange.CSPressRangeCustom, "1");
            press.TPressSettingAttach(raised.Add);

            CSPressSetting setting = press.TPressRangeSet("2-");

            Assert.Equal(
                CSPressRange.CSPressRangeCustom,
                setting.CSPressSettingRange.Single(static choice => choice.CSPressChoiceChosen).CSPressChoiceValue);
            Assert.Equal("2-", setting.CSPressSettingText);
            Assert.Equal([false, true, true], setting.CSPressSettingIncluded);
        });

        Assert.Single(raised);
    }
}
