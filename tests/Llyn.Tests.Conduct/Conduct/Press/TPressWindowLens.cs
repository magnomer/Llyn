using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Llyn.Conduct;
using Xunit;

namespace Llyn.Tests;

public sealed class TPressWindowLens
{
    [Fact]
    public async Task LensTextSet_CulturePercentSign_SetsFreeFactor()
    {
        await TInterfaceConductPortrait.TPressWindowOpen(new TPressBoard(), static press =>
        {
            CSLensState lens = press.TLensTextSet("150" + NumberFormatInfo.CurrentInfo.PercentSymbol);

            Assert.Equal(1.5, lens.CSLensStateFactor, 6);
            Assert.Equal(CSLensMode.CSLensModeFree, lens.CSLensStateMode);
            Assert.Equal(1.5.ToString("P0", CultureInfo.CurrentCulture), lens.CSLensStateText);
            return Task.CompletedTask;
        });
    }

    [Fact]
    public async Task LensTextSet_BareNumber_SetsFreeFactor()
    {
        await TInterfaceConductPortrait.TPressWindowOpen(new TPressBoard(), static press =>
        {
            CSLensState lens = press.TLensTextSet("75");

            Assert.Equal(0.75, lens.CSLensStateFactor, 6);
            Assert.Equal(CSLensMode.CSLensModeFree, lens.CSLensStateMode);
            return Task.CompletedTask;
        });
    }

    [Fact]
    public async Task LensTextSet_Word_RefitsCurrentMode()
    {
        await TInterfaceConductPortrait.TPressWindowOpen(new TPressBoard(), static async press =>
        {
            CSPressState? state = await press.TPressDraw();
            press.TLensAreaSet(1000, 800);
            press.TLensModeSet(CSLensMode.CSLensModePage);
            press.TLensLadderMove(1);
            press.TLensModeSet(CSLensMode.CSLensModeWidth);

            CSLensState lens = press.TLensTextSet("wide");

            Assert.Equal(1000 / state!.CSPressStateWidth, lens.CSLensStateFactor, 6);
            Assert.Equal(CSLensMode.CSLensModeWidth, lens.CSLensStateMode);
        });
    }

    [Fact]
    public async Task LensLadderMove_Up_StepsToNextRung()
    {
        await TInterfaceConductPortrait.TPressWindowOpen(new TPressBoard(), static press =>
        {
            press.TLensTextSet("100");

            Assert.Equal(1.1, press.TLensLadderMove(120).CSLensStateFactor, 6);
            Assert.Equal(1.25, press.TLensLadderMove(1).CSLensStateFactor, 6);
            return Task.CompletedTask;
        });
    }

    [Fact]
    public async Task LensLadderMove_Down_StepsToEarlierRung()
    {
        await TInterfaceConductPortrait.TPressWindowOpen(new TPressBoard(), static press =>
        {
            press.TLensTextSet("80");

            CSLensState lens = press.TLensLadderMove(-120);

            Assert.Equal(0.75, lens.CSLensStateFactor, 6);
            Assert.Equal(CSLensMode.CSLensModeFree, lens.CSLensStateMode);
            return Task.CompletedTask;
        });
    }

    [Fact]
    public async Task LensLadderMove_TopRung_ClampsAtFourHundred()
    {
        await TInterfaceConductPortrait.TPressWindowOpen(new TPressBoard(), static press =>
        {
            Assert.Equal(4.0, press.TLensTextSet("1000").CSLensStateFactor, 6);
            Assert.Equal(4.0, press.TLensLadderMove(1).CSLensStateFactor, 6);
            return Task.CompletedTask;
        });
    }

    [Fact]
    public async Task LensLadderMove_BottomRung_ClampsAtTen()
    {
        await TInterfaceConductPortrait.TPressWindowOpen(new TPressBoard(), static press =>
        {
            Assert.Equal(0.1, press.TLensTextSet("1").CSLensStateFactor, 6);
            Assert.Equal(0.1, press.TLensLadderMove(-1).CSLensStateFactor, 6);
            return Task.CompletedTask;
        });
    }

    [Fact]
    public async Task LensModeSet_FitWidth_ReturnsAreaAcrossPaper()
    {
        await TInterfaceConductPortrait.TPressWindowOpen(new TPressBoard(), static async press =>
        {
            CSPressState? state = await press.TPressDraw();
            press.TLensTextSet("100");
            press.TLensAreaSet(600, 400);

            CSLensState lens = press.TLensModeSet(CSLensMode.CSLensModeWidth);

            Assert.Equal(600 / state!.CSPressStateWidth, lens.CSLensStateFactor, 6);
            Assert.Equal(CSLensMode.CSLensModeWidth, lens.CSLensStateMode);
        });
    }

    [Fact]
    public async Task LensModeSet_FitPage_ReturnsSmallerRatio()
    {
        await TInterfaceConductPortrait.TPressWindowOpen(new TPressBoard(), static async press =>
        {
            CSPressState? state = await press.TPressDraw();
            press.TLensAreaSet(600, 400);

            CSLensState lens = press.TLensModeSet(CSLensMode.CSLensModePage);

            double expected = Math.Min(600 / state!.CSPressStateWidth, 400 / state.CSPressStateHeight);
            Assert.Equal(expected, lens.CSLensStateFactor, 6);
            Assert.Equal(CSLensMode.CSLensModePage, lens.CSLensStateMode);
        });
    }

    [Fact]
    public async Task SightSet_ZoomPastDefault_RequestsHigherResolution()
    {
        List<CSPressPage> pages = [];
        double width = 0.0;
        await TInterfaceConductPortrait.TPressWindowOpen(new TPressBoard(), async press =>
        {
            CSPressState? state = await press.TPressDraw();
            width = state!.CSPressStateWidth;
            press.TPressDelaySet(0);
            press.TLensAreaSet(2000, 1500);
            press.TLensTextSet("400");
            press.TPressPageAttach(pages.Add);

            await press.TPressSightSet(1, 1, 1, 1.0);
        });

        int across = BinaryPrimitives.ReadInt32BigEndian(pages[0].CSPressPagePng!.AsSpan(16));
        Assert.True(across > (int)Math.Round(width / 96.0 * 300));
    }
}
