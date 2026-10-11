using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Llyn.Conduct;
using Llyn.Core;
using Xunit;

namespace Llyn.Tests;

public sealed class TPressWindowDelay
{
    [Fact]
    public async Task ScaleSet_DelayZero_RaisesOneState()
    {
        TPressBoard board = new();
        List<CSPressState> states = [];
        await TInterfaceConductPortrait.TPressWindowOpen(board, async press =>
        {
            await press.TPressDraw();
            TaskCompletionSource<CSPressState> seen = new(TaskCreationOptions.RunContinuationsAsynchronously);
            press.TPressStateAttach(state =>
            {
                states.Add(state);
                seen.TrySetResult(state);
            });
            press.TPressDelaySet(0);

            press.TPressScaleSet("150");
            CSPressState drawn = await seen.Task.WaitAsync(TimeSpan.FromSeconds(10));

            Assert.Equal(150, drawn.CSPressStateSetting.CSPressSettingScale);
            Assert.False(drawn.CSPressStateSetting.CSPressSettingStale);
        });

        Assert.Single(states);
        Assert.Equal(2, board.TPressBoardCount);
    }

    [Fact]
    public async Task ScaleSet_TwoQuickEdits_RedrawsNewerSetting()
    {
        TPressBoard board = new();
        List<CSPressState> states = [];
        await TInterfaceConductPortrait.TPressWindowOpen(board, async press =>
        {
            await press.TPressDraw();
            TaskCompletionSource<CSPressState> seen = new(TaskCreationOptions.RunContinuationsAsynchronously);
            press.TPressStateAttach(state =>
            {
                states.Add(state);
                seen.TrySetResult(state);
            });

            press.TPressScaleSet("120");
            press.TPressDelaySet(0);
            press.TPressScaleSet("150");
            CSPressState drawn = await seen.Task.WaitAsync(TimeSpan.FromSeconds(10));

            Assert.Equal(150, drawn.CSPressStateSetting.CSPressSettingScale);
        });

        Assert.Single(states);
        Assert.Equal(2, board.TPressBoardCount);
        Assert.Equal(150, board.TPressBoardSetting!.LPressSettingScale);
    }

    [Fact]
    public async Task ScaleSet_BoardFails_ShowsRenderFailed()
    {
        List<string> asked = [];
        List<CSPressState> states = [];
        LPressBoard board = TEngineFault.TEngineFaultCreate<LPressBoard>(
            new TPressBoard(), "LPressBoard.LPressBoardDraw", false);
        await TInterfaceConductPortrait.TPressWindowOpen(board, async press =>
        {
            TaskCompletionSource<CSPressState> seen = new(TaskCreationOptions.RunContinuationsAsynchronously);
            press.TPressStateAttach(state =>
            {
                states.Add(state);
                seen.TrySetResult(state);
            });
            press.TPressDelaySet(0);

            press.TPressScaleSet("150");
            CSPressState drawn = await seen.Task.WaitAsync(TimeSpan.FromSeconds(10));

            Assert.Equal("Print.RenderFailed", drawn.CSPressStateFailure);
            Assert.Equal(0, drawn.CSPressStateCount);
        }, asked: asked);

        Assert.Single(states);
        Assert.Equal(["Print.RenderFailed"], asked);
    }

    [Fact]
    public async Task SightSet_DrawnProof_RaisesWindowPages()
    {
        List<CSPressPage> pages = [];
        await TInterfaceConductPortrait.TPressWindowOpen(new TPressBoard(), async press =>
        {
            await press.TPressDraw();
            press.TPressPageAttach(pages.Add);
            press.TPressDelaySet(0);

            await press.TPressSightSet(0, 0, 0, 1.0);
        });

        Assert.Equal([0, 1], pages.Select(page => page.CSPressPageIndex));
        Assert.All(pages, page => Assert.Equal(new byte[] { 0x89, 0x50, 0x4E, 0x47 }, page.CSPressPagePng!.Take(4)));
    }

    [Fact]
    public async Task SightSet_BeforeDraw_RaisesNothing()
    {
        List<string> asked = [];
        List<int> pages = [];
        await TInterfaceConductPortrait.TPressWindowOpen(new TPressBoard(), async press =>
        {
            press.TPressPageAttach(page => pages.Add(page.CSPressPageIndex));
            press.TPressDelaySet(0);

            await press.TPressSightSet(0, 0, 0, 1.0);
        }, asked: asked);

        Assert.Empty(pages);
        Assert.Empty(asked);
    }

    [Fact]
    public async Task SightSet_SameResolutionAgain_RequestsNothing()
    {
        List<int> pages = [];
        await TInterfaceConductPortrait.TPressWindowOpen(new TPressBoard(), async press =>
        {
            await press.TPressDraw();
            press.TPressDelaySet(0);
            await press.TPressSightSet(1, 1, 1, 1.0);
            press.TPressPageAttach(page => pages.Add(page.CSPressPageIndex));

            await press.TPressSightSet(1, 1, 1, 1.0);
        });

        Assert.Empty(pages);
    }

    [Fact]
    public async Task SightSet_AfterRedraw_RequestsPagesAgain()
    {
        List<int> pages = [];
        await TInterfaceConductPortrait.TPressWindowOpen(new TPressBoard(), async press =>
        {
            await press.TPressDraw();
            press.TPressDelaySet(0);
            await press.TPressSightSet(0, 0, 0, 1.0);
            await press.TPressDraw();
            press.TPressPageAttach(page => pages.Add(page.CSPressPageIndex));

            await press.TPressSightSet(0, 0, 0, 1.0);
        });

        Assert.Equal([0, 1], pages);
    }

    [Fact]
    public async Task GalleyDispose_LaterRequests_RaisesNothing()
    {
        TPressBoard board = new();
        List<CSPressState> states = [];
        List<int> pages = [];
        await TInterfaceConductPortrait.TPressWindowOpen(board, async press =>
        {
            await press.TPressDraw();
            press.TPressStateAttach(states.Add);
            press.TPressPageAttach(page => pages.Add(page.CSPressPageIndex));
            press.TPressDelaySet(0);
            press.TPressGalleyDispose();

            press.TPressScaleSet("150");
            await press.TPressSightSet(0, 0, 0, 1.0);
        });

        Assert.Empty(states);
        Assert.Empty(pages);
        Assert.Equal(1, board.TPressBoardCount);
    }

    [Fact]
    public async Task SightSet_NewTopPage_SelectsCurrentPage()
    {
        CSPressSetting? setting = null;
        await TInterfaceConductPortrait.TPressWindowOpen(new TPressBoard(), async press =>
        {
            await press.TPressDraw();
            press.TPressDelaySet(0);

            await press.TPressSightSet(2, 1, 2, 1.0);
            setting = press.TPressSettingRead();
        });

        Assert.Equal(2, setting?.CSPressSettingPage);
    }

    [Fact]
    public async Task SightSet_SameTopPage_RaisesNoSetting()
    {
        List<CSPressSetting> settings = [];
        await TInterfaceConductPortrait.TPressWindowOpen(new TPressBoard(), async press =>
        {
            await press.TPressDraw();
            press.TPressDelaySet(0);
            await press.TPressSightSet(1, 1, 1, 1.0);
            press.TPressSettingAttach(settings.Add);

            await press.TPressSightSet(1, 0, 1, 1.0);
        });

        Assert.Empty(settings);
    }

    [Fact]
    public async Task SightSet_WindowMoves_RaisesDroppedPages()
    {
        List<int> dropped = [];
        await TInterfaceConductPortrait.TPressWindowOpen(new TPressBoard(), async press =>
        {
            await press.TPressDraw();
            press.TPressDelaySet(0);
            await press.TPressSightSet(0, 0, 0, 1.0);
            press.TPressDropAttach(dropped.Add);

            await press.TPressSightSet(2, 2, 2, 1.0);
        });

        Assert.Equal([0], dropped);
    }

    [Fact]
    public async Task Draw_DeliveredPages_RaisesDroppedPages()
    {
        List<int> dropped = [];
        await TInterfaceConductPortrait.TPressWindowOpen(new TPressBoard(), async press =>
        {
            await press.TPressDraw();
            press.TPressDelaySet(0);
            await press.TPressSightSet(0, 0, 0, 1.0);
            press.TPressDropAttach(dropped.Add);

            await press.TPressDraw();
        });

        Assert.Equal([0, 1], dropped);
    }

    [Fact]
    public async Task SightSet_HigherResolution_DeliversPagesAgain()
    {
        List<int> pages = [];
        List<int> dropped = [];
        await TInterfaceConductPortrait.TPressWindowOpen(new TPressBoard(), async press =>
        {
            await press.TPressDraw();
            press.TPressDelaySet(0);
            await press.TPressSightSet(1, 1, 1, 1.0);
            press.TPressPageAttach(page => pages.Add(page.CSPressPageIndex));
            press.TPressDropAttach(dropped.Add);

            press.TLensAreaSet(2000, 1500);
            press.TLensTextSet("400");
            await press.TPressSightSet(1, 1, 1, 1.0);
        });

        Assert.Equal([0, 1, 2], pages);
        Assert.Empty(dropped);
    }

    [Fact]
    public async Task SightSet_ProofFails_ShowsOneNotice()
    {
        List<string> asked = [];
        List<int> pages = [];
        TPressBoard board = new()
        {
            TPressBoardCover = proof => TEngineFault.TEngineFaultCreate(proof, "LPressProof.LPressProofDraw", false),
        };
        await TInterfaceConductPortrait.TPressWindowOpen(board, async press =>
        {
            await press.TPressDraw();
            press.TPressPageAttach(page => pages.Add(page.CSPressPageIndex));
            press.TPressDelaySet(0);

            await press.TPressSightSet(1, 1, 1, 1.0);
        }, asked: asked);

        Assert.Empty(pages);
        Assert.Equal(["Print.RenderFailed"], asked);
    }
}
