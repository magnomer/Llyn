using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Llyn.Conduct;
using Llyn.Core;
using Xunit;

namespace Llyn.Tests;

public sealed class TPressWindowOutput
{
    [Fact]
    public async Task Print_Printer_HandsSettingAndAsksNothing()
    {
        TPressBoard board = new();
        List<string> asked = [];
        await TInterfaceConductPortrait.TPressWindowOpen(board, static async press =>
        {
            press.TPressCopiesSet("3");

            Assert.Null(await press.TPressPrint());
        }, asked: asked);

        Assert.Empty(asked);
        Assert.Equal("Office Laser", board.TPressBoardSetting!.LPressSettingPrinter);
        Assert.Equal(3, board.TPressBoardSetting.LPressSettingCopies);
        Assert.Null(board.TPressBoardPath);
    }

    [Fact]
    public async Task Print_PrinterOffline_ReturnsUnavailableKey()
    {
        TPressBoard board = new() { TPressBoardStatus = LPressStatus.LPressStatusOffline };
        await TInterfaceConductPortrait.TPressWindowOpen(board, static async press =>
        {
            Assert.Equal("Print.PrinterUnavailable", await press.TPressPrint());
        });
    }

    [Fact]
    public async Task Print_PrinterFailing_ReturnsFailedKey()
    {
        TPressBoard board = new() { TPressBoardStatus = LPressStatus.LPressStatusError };
        await TInterfaceConductPortrait.TPressWindowOpen(board, static async press =>
        {
            Assert.Equal("Print.Failed", await press.TPressPrint());
        });
    }

    [Fact]
    public async Task Print_SaveAsPdfChosen_ExportsToPickedPath()
    {
        TPressBoard board = new();
        List<string> asked = [];
        await TInterfaceConductPortrait.TPressWindowOpen(board, static async press =>
        {
            press.TPressPrinterSelect(null);

            Assert.Null(await press.TPressPrint());
        }, asked: asked);

        Assert.Equal(["File:water"], asked);
        Assert.Equal("press.pdf", board.TPressBoardPath);
        Assert.Null(board.TPressBoardSetting!.LPressSettingPrinter);
    }

    [Fact]
    public async Task Print_SaveAsPdfDeclined_ExportsNothing()
    {
        TPressBoard board = new();
        List<string> asked = [];
        await TInterfaceConductPortrait.TPressWindowOpen(board, static async press =>
        {
            press.TPressPrinterSelect(null);

            Assert.Null(await press.TPressPrint());
        }, null, asked);

        Assert.Equal(["File:water"], asked);
        Assert.Null(board.TPressBoardPath);
        Assert.Null(board.TPressBoardSetting);
    }

    [Fact]
    public async Task SystemShow_Board_ShowsDialog()
    {
        TPressBoard board = new();
        await TInterfaceConductPortrait.TPressWindowOpen(board, static async press =>
        {
            Assert.Null(await press.TPressSystemShow());
        });

        Assert.True(board.TPressBoardShown);
    }

    [Fact]
    public async Task Print_WhilePrinting_RefusesWithBusyKey()
    {
        TPressBoard board = new() { TPressBoardHold = new TaskCompletionSource() };
        await TInterfaceConductPortrait.TPressWindowOpen(board, async press =>
        {
            Task<string?> printing = press.TPressPrint();

            Assert.Equal("Print.Printing", await press.TPressPrint());

            board.TPressBoardHold!.SetResult();
            Assert.Null(await printing);
        });
    }

    [Fact]
    public async Task Close_Idle_SavesSettingOnce()
    {
        TPressVault vault = new();
        await TInterfaceConductPortrait.TPressWindowOpen(new TPressBoard(), static press =>
        {
            Assert.True(press.TPressClose());
            return Task.CompletedTask;
        }, vault: vault);

        Assert.Equal(1, vault.TPressVaultCount);
        Assert.NotNull(vault.TPressVaultSetting);
    }

    [Fact]
    public async Task Close_StoreFailure_ShowsNoticeAndCloses()
    {
        TPressVault vault = new() { TPressVaultFailure = new InvalidOperationException("The vault refuses.") };
        List<string> asked = [];
        await TInterfaceConductPortrait.TPressWindowOpen(new TPressBoard(), static press =>
        {
            Assert.True(press.TPressClose());
            return Task.CompletedTask;
        }, asked: asked, vault: vault);

        Assert.Equal(["Print.Failed"], asked);
        Assert.Equal(0, vault.TPressVaultCount);
    }

    [Fact]
    public async Task Close_WhilePrinting_RefusesAndSavesNothing()
    {
        TPressBoard board = new() { TPressBoardHold = new TaskCompletionSource() };
        TPressVault vault = new();
        await TInterfaceConductPortrait.TPressWindowOpen(board, async press =>
        {
            Task<string?> printing = press.TPressPrint();

            Assert.False(press.TPressClose());

            board.TPressBoardHold!.SetResult();
            await printing;
        }, vault: vault);

        Assert.Equal(0, vault.TPressVaultCount);
        Assert.Null(vault.TPressVaultSetting);
    }

    [Fact]
    public async Task SettingChanged_Print_ReportsBusyThenIdle()
    {
        TPressBoard board = new() { TPressBoardHold = new TaskCompletionSource() };
        List<bool> idle = [];
        List<string> busy = [];
        await TInterfaceConductPortrait.TPressWindowOpen(board, async press =>
        {
            press.TPressSettingAttach(setting =>
            {
                idle.Add(setting.CSPressSettingIdle);
                busy.Add(setting.CSPressSettingBusy);
            });

            Task<string?> printing = press.TPressPrint();

            Assert.Equal([false], idle);
            Assert.Equal(["Print.Printing"], busy);

            board.TPressBoardHold!.SetResult();

            Assert.Null(await printing);
        });

        Assert.Equal([false, true], idle);
        Assert.Equal(["Print.Printing", "Print.Idle"], busy);
    }
}
