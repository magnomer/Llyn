using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Llyn.Conduct;
using Xunit;

namespace Llyn.Tests;

public sealed class TPressWindowChoice
{
    [Fact]
    public async Task SettingRead_Fresh_ListsPickerKeysInOrder()
    {
        await TInterfaceConductPortrait.TPressWindowOpen(new TPressBoard(), static press =>
        {
            CSPressSetting setting = press.TPressSettingRead();

            Assert.Equal(
                ["Print.PagesAll", "Print.PagesCurrent", "Print.PagesCustom"],
                setting.CSPressSettingRange.Select(static choice => choice.CSPressChoiceKey));
            Assert.Equal(
                [
                    "Print.MarginsDefault",
                    "Print.MarginsNone",
                    "Print.MarginsNarrow",
                    "Print.MarginsWide",
                    "Print.MarginsCustom",
                ],
                setting.CSPressSettingMargin.Select(static choice => choice.CSPressChoiceKey));
            Assert.Equal(
                ["Print.SideSingle", "Print.SideLong", "Print.SideShort"],
                setting.CSPressSettingSide!.Select(static choice => choice.CSPressChoiceKey));
            Assert.Single(setting.CSPressSettingRange, static choice => choice.CSPressChoiceChosen);
            Assert.Single(setting.CSPressSettingMargin, static choice => choice.CSPressChoiceChosen);
            Assert.Single(setting.CSPressSettingSide!, static choice => choice.CSPressChoiceChosen);
            return Task.CompletedTask;
        });
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    public async Task RangeSelect_ListedIndex_ChoosesListedRange(int index)
    {
        List<CSPressSetting> raised = [];
        await TInterfaceConductPortrait.TPressWindowOpen(new TPressBoard(), press =>
        {
            CSPressRange listed = press.TPressSettingRead().CSPressSettingRange[index].CSPressChoiceValue;
            press.TPressSettingAttach(raised.Add);

            CSPressSetting setting = press.TPressRangeSelect(index);

            Assert.Equal(listed, setting.CSPressSettingRange[index].CSPressChoiceValue);
            Assert.True(setting.CSPressSettingRange[index].CSPressChoiceChosen);
            return Task.CompletedTask;
        });

        Assert.Single(raised);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(3)]
    public async Task RangeSelect_IndexOutsideList_KeepsRange(int index)
    {
        List<CSPressSetting> raised = [];
        await TInterfaceConductPortrait.TPressWindowOpen(new TPressBoard(), press =>
        {
            CSPressSetting before = press.TPressRangeSet(CSPressRange.CSPressRangeCustom, "2");
            press.TPressSettingAttach(raised.Add);

            CSPressSetting setting = press.TPressRangeSelect(index);

            Assert.Equal(before.CSPressSettingRange, setting.CSPressSettingRange);
            Assert.Equal("2", setting.CSPressSettingText);
            return Task.CompletedTask;
        });

        Assert.Single(raised);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    public async Task MarginSelect_ListedIndex_ChoosesListedMargin(int index)
    {
        List<CSPressSetting> raised = [];
        await TInterfaceConductPortrait.TPressWindowOpen(new TPressBoard(), press =>
        {
            CSPressMargin listed = press.TPressSettingRead().CSPressSettingMargin[index].CSPressChoiceValue;
            press.TPressSettingAttach(raised.Add);

            CSPressSetting setting = press.TPressMarginSelect(index);

            Assert.Equal(listed, setting.CSPressSettingMargin[index].CSPressChoiceValue);
            Assert.True(setting.CSPressSettingMargin[index].CSPressChoiceChosen);
            return Task.CompletedTask;
        });

        Assert.Single(raised);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(5)]
    public async Task MarginSelect_IndexOutsideList_KeepsMargin(int index)
    {
        List<CSPressSetting> raised = [];
        await TInterfaceConductPortrait.TPressWindowOpen(new TPressBoard(), press =>
        {
            CSPressSetting before = press.TPressMarginSet(CSPressMargin.CSPressMarginWide);
            press.TPressSettingAttach(raised.Add);

            CSPressSetting setting = press.TPressMarginSelect(index);

            Assert.Equal(before.CSPressSettingMargin, setting.CSPressSettingMargin);
            return Task.CompletedTask;
        });

        Assert.Single(raised);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    public async Task SideSelect_ListedIndex_ChoosesListedSide(int index)
    {
        List<CSPressSetting> raised = [];
        await TInterfaceConductPortrait.TPressWindowOpen(new TPressBoard(), press =>
        {
            CSPressSide listed = press.TPressSettingRead().CSPressSettingSide![index].CSPressChoiceValue;
            press.TPressSettingAttach(raised.Add);

            CSPressSetting setting = press.TPressSideSelect(index);

            Assert.Equal(listed, setting.CSPressSettingSide![index].CSPressChoiceValue);
            Assert.True(setting.CSPressSettingSide[index].CSPressChoiceChosen);
            return Task.CompletedTask;
        });

        Assert.Single(raised);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(3)]
    public async Task SideSelect_IndexOutsideList_KeepsSide(int index)
    {
        List<CSPressSetting> raised = [];
        await TInterfaceConductPortrait.TPressWindowOpen(new TPressBoard(), press =>
        {
            CSPressSetting before = press.TPressSideSet(CSPressSide.CSPressSideLong);
            press.TPressSettingAttach(raised.Add);

            CSPressSetting setting = press.TPressSideSelect(index);

            Assert.Equal(before.CSPressSettingSide, setting.CSPressSettingSide);
            return Task.CompletedTask;
        });

        Assert.Single(raised);
    }

    [Fact]
    public async Task SideSelect_SaveAsPdf_KeepsNoSides()
    {
        await TInterfaceConductPortrait.TPressWindowOpen(new TPressBoard(), static press =>
        {
            press.TPressPrinterSelect((string?)null);

            Assert.Null(press.TPressSideSelect(0).CSPressSettingSide);
            return Task.CompletedTask;
        });
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    public async Task PaperSelect_ListedIndex_ChoosesListedPaper(int index)
    {
        List<CSPressSetting> raised = [];
        await TInterfaceConductPortrait.TPressWindowOpen(new TPressBoard(), press =>
        {
            string listed = press.TPressSettingRead().CSPressSettingPaper[index].CSPressPaperKey;
            press.TPressSettingAttach(raised.Add);

            CSPressSetting setting = press.TPressPaperSelect(index);

            Assert.Equal(listed, setting.CSPressSettingPaper[index].CSPressPaperKey);
            Assert.True(setting.CSPressSettingPaper[index].CSPressPaperChosen);
            return Task.CompletedTask;
        });

        Assert.Single(raised);
    }

    [Fact]
    public async Task PaperSelect_IndexOutsideList_KeepsPaper()
    {
        List<CSPressSetting> raised = [];
        await TInterfaceConductPortrait.TPressWindowOpen(new TPressBoard(), press =>
        {
            CSPressSetting before = press.TPressPaperSelect(1);
            press.TPressSettingAttach(raised.Add);

            CSPressSetting low = press.TPressPaperSelect(-1);
            CSPressSetting high = press.TPressPaperSelect(before.CSPressSettingPaper.Count);

            Assert.Equal(before.CSPressSettingPaper, low.CSPressSettingPaper);
            Assert.Equal(before.CSPressSettingPaper, high.CSPressSettingPaper);
            return Task.CompletedTask;
        });

        Assert.Equal(2, raised.Count);
    }
}
