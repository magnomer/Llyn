using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Llyn.Conduct;
using Llyn.Core;
using Xunit;

namespace Llyn.Tests;

public sealed class TPressWindowProof
{
    [Fact]
    public async Task SettingStale_LayoutKeys_TurnsTrueUntilDrawn()
    {
        await TInterfaceConductPortrait.TPressWindowOpen(new TPressBoard(), static async press =>
        {
            Func<CSPress, CSPressSetting>[] changes =
            [
                static item => item.TPressOrientationSet(CSPressOrientation.CSPressOrientationLandscape),
                static item => item.TPressPaperSelect(item.TPressSettingRead().CSPressSettingPaper
                    .Select(static (paper, index) => (paper, index))
                    .First(static pick => !pick.paper.CSPressPaperChosen)
                    .index),
                static item => item.TPressMarginSet(CSPressMargin.CSPressMarginNarrow),
                static item => item.TPressScaleSet("150"),
                static item => item.TPressBackgroundSet(false),
            ];

            foreach (Func<CSPress, CSPressSetting> change in changes)
            {
                CSPressState? state = await press.TPressDraw();

                Assert.False(state!.CSPressStateSetting.CSPressSettingStale);
                Assert.True(change(press).CSPressSettingStale);
            }
        });
    }

    [Fact]
    public async Task SettingStale_OtherKeys_StaysFalse()
    {
        await TInterfaceConductPortrait.TPressWindowOpen(new TPressBoard(), static async press =>
        {
            await press.TPressDraw();

            Assert.False(press.TPressCopiesSet("2").CSPressSettingStale);
            Assert.False(press.TPressCollationSet(false).CSPressSettingStale);
            Assert.False(press.TPressColorSet(CSPressColor.CSPressColorGray).CSPressSettingStale);
            Assert.False(press.TPressSideSet(CSPressSide.CSPressSideLong).CSPressSettingStale);
            Assert.False(press.TPressRangeSet(CSPressRange.CSPressRangeCustom, "1").CSPressSettingStale);
            Assert.False(press.TPressPageSelect(1).CSPressSettingStale);
        });
    }

    [Fact]
    public async Task Draw_FirstCall_ReturnsPagesOnPaper()
    {
        TPressBoard board = new();
        await TInterfaceConductPortrait.TPressWindowOpen(board, static async press =>
        {
            CSPressPaper paper = press.TPressSettingRead().CSPressSettingPaper
                .Single(static item => item.CSPressPaperChosen);

            CSPressState? state = await press.TPressDraw();

            Assert.Equal(3, state!.CSPressStateCount);
            Assert.Equal(paper.CSPressPaperWidth * 96.0, state.CSPressStateWidth, 6);
            Assert.Equal(paper.CSPressPaperHeight * 96.0, state.CSPressStateHeight, 6);
            Assert.Null(state.CSPressStateFailure);
            Assert.False(state.CSPressStateSetting.CSPressSettingStale);
        });

        Assert.Equal(1, board.TPressBoardCount);
    }

    [Fact]
    public async Task Draw_Unchanged_SkipsSecondRender()
    {
        TPressBoard board = new();
        await TInterfaceConductPortrait.TPressWindowOpen(board, static async press =>
        {
            CSPressState? first = await press.TPressDraw();
            CSPressState? second = await press.TPressDraw();

            Assert.Equal(first!.CSPressStateCount, second!.CSPressStateCount);
        });

        Assert.Equal(1, board.TPressBoardCount);
    }

    [Fact]
    public async Task Draw_LayoutChanged_RendersLatestSetting()
    {
        TPressBoard board = new();
        await TInterfaceConductPortrait.TPressWindowOpen(board, static async press =>
        {
            await press.TPressDraw();
            press.TPressScaleSet("120");
            press.TPressScaleSet("150");

            CSPressState? state = await press.TPressDraw();

            Assert.False(state!.CSPressStateSetting.CSPressSettingStale);
        });

        Assert.Equal(2, board.TPressBoardCount);
        Assert.Equal(150, board.TPressBoardSetting!.LPressSettingScale);
    }

    [Fact]
    public async Task Draw_NewerSettingDuringDraw_SupersedesEarlierDraw()
    {
        TPressBoard board = new() { TPressBoardHold = new TaskCompletionSource() };
        await TInterfaceConductPortrait.TPressWindowOpen(board, async press =>
        {
            Task<CSPressState?> earlier = press.TPressDraw();
            press.TPressScaleSet("150");
            Task<CSPressState?> latest = press.TPressDraw();

            Assert.False(earlier.IsCompleted);
            Assert.Equal(100, board.TPressBoardSetting!.LPressSettingScale);

            board.TPressBoardHold!.SetResult();
            CSPressState? superseded = await earlier;
            CSPressState? drawn = await latest;

            Assert.Null(superseded);
            Assert.Equal(150, drawn!.CSPressStateSetting.CSPressSettingScale);
            Assert.False(drawn.CSPressStateSetting.CSPressSettingStale);
        });

        Assert.Equal(2, board.TPressBoardCount);
        Assert.Equal(150, board.TPressBoardSetting!.LPressSettingScale);
    }

    [Fact]
    public async Task Draw_Landscape_HandsOrientationToBoard()
    {
        TPressBoard board = new();
        await TInterfaceConductPortrait.TPressWindowOpen(board, static async press =>
        {
            CSPressState? portrait = await press.TPressDraw();
            press.TPressOrientationSet(CSPressOrientation.CSPressOrientationLandscape);
            CSPressState? landscape = await press.TPressDraw();

            Assert.Equal(portrait!.CSPressStateWidth, landscape!.CSPressStateHeight);
            Assert.Equal(portrait.CSPressStateHeight, landscape.CSPressStateWidth);
        });

        Assert.Equal(2, board.TPressBoardCount);
        Assert.Equal(LPressOrientation.LPressOrientationLandscape, board.TPressBoardSetting!.LPressSettingOrientation);
    }

    [Fact]
    public async Task Draw_BoardFails_ReturnsFailureState()
    {
        List<string> asked = [];
        LPressBoard board = TEngineFault.TEngineFaultCreate<LPressBoard>(
            new TPressBoard(), "LPressBoard.LPressBoardDraw", false);
        await TInterfaceConductPortrait.TPressWindowOpen(board, static async press =>
        {
            CSPressState? state = await press.TPressDraw();

            Assert.Equal("Print.RenderFailed", state!.CSPressStateFailure);
            Assert.Equal(0, state.CSPressStateCount);
            Assert.True(press.TPressSettingRead().CSPressSettingStale);
            Assert.False(state.CSPressStateSetting.CSPressSettingStale);
        }, asked: asked);

        Assert.Equal(["Print.RenderFailed"], asked);
    }
}
