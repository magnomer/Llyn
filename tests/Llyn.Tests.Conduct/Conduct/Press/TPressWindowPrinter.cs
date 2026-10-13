using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Llyn.Conduct;
using Xunit;

namespace Llyn.Tests;

public sealed class TPressWindowPrinter
{
    [Fact]
    public async Task SettingRead_Fresh_ListsSaveAsPdfFirst()
    {
        await TInterfaceConductPortrait.TPressWindowOpen(new TPressBoard(), static press =>
        {
            CSPressSetting setting = press.TPressSettingRead();

            Assert.Equal(
                [
                    new CSPressPrinter(null, "Print.SaveAsPdf", false),
                    new CSPressPrinter("Office Laser", "Print.PrinterDefault", true),
                    new CSPressPrinter("Plotter", "Plotter", false),
                ],
                setting.CSPressSettingPrinter);
            Assert.Equal("Print.Print", setting.CSPressSettingAction);
            Assert.Equal(1, setting.CSPressSettingCopies);
            Assert.Equal(100, setting.CSPressSettingScale);
            Assert.True(setting.CSPressSettingBackground);
            Assert.Equal(
                CSPressRange.CSPressRangeAll,
                setting.CSPressSettingRange.Single(static choice => choice.CSPressChoiceChosen).CSPressChoiceValue);
            Assert.Null(setting.CSPressSettingHint);
            return Task.CompletedTask;
        });
    }

    [Fact]
    public async Task Restore_QueueFails_OpensWithSaveAsPdfAndShowsFailure()
    {
        List<string> asked = [];
        int windows = 0;

        await TInterfaceConductPortrait.TPressWindowOpen(
            new TPressBoard(),
            press =>
            {
                windows++;
                CSPressSetting setting = press.TPressSettingRead();

                Assert.Equal([new CSPressPrinter(null, "Print.SaveAsPdf", true)], setting.CSPressSettingPrinter);
                Assert.Equal("Print.Save", setting.CSPressSettingAction);
                return Task.CompletedTask;
            },
            asked: asked,
            failure: new InvalidOperationException("The spooler is down."));

        Assert.Equal(1, windows);
        Assert.Equal(["Print.Failed"], asked);
    }

    [Fact]
    public async Task PrinterSelect_SaveAsPdf_HidesPrinterOnlyFields()
    {
        await TInterfaceConductPortrait.TPressWindowOpen(new TPressBoard(), static press =>
        {
            Assert.True(press.TPressCopiesSet("2").CSPressSettingCollation);
            Assert.Equal("Print.CapabilityUnknown", press.TPressPrinterSelect("Plotter").CSPressSettingHint);

            CSPressSetting setting = press.TPressPrinterSelect(null);

            Assert.Equal("Print.Save", setting.CSPressSettingAction);
            Assert.Null(setting.CSPressSettingCopies);
            Assert.Null(setting.CSPressSettingCollation);
            Assert.Null(setting.CSPressSettingColor);
            Assert.Null(setting.CSPressSettingSide);
            Assert.Null(setting.CSPressSettingSheets);
            Assert.Null(setting.CSPressSettingHint);
            Assert.Equal(
                [true, false, false],
                setting.CSPressSettingPrinter.Select(static item => item.CSPressPrinterChosen));
            return Task.CompletedTask;
        });
    }

    [Fact]
    public async Task PrinterSelect_IndexPastList_KeepsPrinter()
    {
        await TInterfaceConductPortrait.TPressWindowOpen(new TPressBoard(), static press =>
        {
            CSPressSetting before = press.TPressPrinterSelect(-1);
            CSPressSetting after = press.TPressPrinterSelect(before.CSPressSettingPrinter.Count);

            Assert.Equal(before.CSPressSettingPrinter, after.CSPressSettingPrinter);
            return Task.CompletedTask;
        });
    }

    [Fact]
    public async Task PrinterSelect_Index_ChoosesListedPrinter()
    {
        await TInterfaceConductPortrait.TPressWindowOpen(new TPressBoard(), static press =>
        {
            CSPressSetting setting = press.TPressPrinterSelect(2);

            Assert.Equal(
                "Plotter",
                setting.CSPressSettingPrinter.Single(static printer => printer.CSPressPrinterChosen)
                    .CSPressPrinterName);
            return Task.CompletedTask;
        });
    }

    [Fact]
    public async Task PrinterSelect_NoSelection_KeepsPrinter()
    {
        await TInterfaceConductPortrait.TPressWindowOpen(new TPressBoard(), static press =>
        {
            CSPressSetting setting = press.TPressPrinterSelect(-1);

            Assert.Equal(
                "Office Laser",
                setting.CSPressSettingPrinter.Single(static printer => printer.CSPressPrinterChosen)
                    .CSPressPrinterName);
            return Task.CompletedTask;
        });
    }

    [Fact]
    public async Task CopiesSet_BeyondPrinterMaximum_ClampsToMaximum()
    {
        await TInterfaceConductPortrait.TPressWindowOpen(new TPressBoard(), static press =>
        {
            Assert.Equal(5, press.TPressCopiesSet("9").CSPressSettingCopies);
            return Task.CompletedTask;
        });
    }

    [Fact]
    public async Task CopiesSet_BelowOne_ClampsToOne()
    {
        await TInterfaceConductPortrait.TPressWindowOpen(new TPressBoard(), static press =>
        {
            press.TPressCopiesSet("3");

            Assert.Equal(1, press.TPressCopiesSet("0").CSPressSettingCopies);
            Assert.Equal(1, press.TPressCopiesSet("-4").CSPressSettingCopies);
            return Task.CompletedTask;
        });
    }

    [Fact]
    public async Task CopiesSet_NotNumber_KeepsCopies()
    {
        await TInterfaceConductPortrait.TPressWindowOpen(new TPressBoard(), static press =>
        {
            press.TPressCopiesSet("3");

            Assert.Equal(3, press.TPressCopiesSet("many").CSPressSettingCopies);
            return Task.CompletedTask;
        });
    }

    [Fact]
    public async Task CopiesSet_UnknownPrinter_ClampsToLimit()
    {
        await TInterfaceConductPortrait.TPressWindowOpen(new TPressBoard(), static press =>
        {
            press.TPressPrinterSelect("Plotter");

            Assert.Equal(500, press.TPressCopiesSet("500").CSPressSettingCopies);
            Assert.Equal(999, press.TPressCopiesSet("1000").CSPressSettingCopies);
            return Task.CompletedTask;
        });
    }

    [Fact]
    public async Task CollationSet_SeveralCopies_StoresChoice()
    {
        await TInterfaceConductPortrait.TPressWindowOpen(new TPressBoard(), static press =>
        {
            Assert.Null(press.TPressSettingRead().CSPressSettingCollation);
            Assert.True(press.TPressCopiesSet("2").CSPressSettingCollation);
            Assert.False(press.TPressCollationSet(false).CSPressSettingCollation);
            return Task.CompletedTask;
        });
    }

    [Fact]
    public async Task ScaleSet_OutOfRange_ClampsToLimits()
    {
        await TInterfaceConductPortrait.TPressWindowOpen(new TPressBoard(), static press =>
        {
            Assert.Equal(200, press.TPressScaleSet("500").CSPressSettingScale);
            Assert.Equal(10, press.TPressScaleSet("5").CSPressSettingScale);
            Assert.Equal(150, press.TPressScaleSet("150").CSPressSettingScale);
            return Task.CompletedTask;
        });
    }

    [Fact]
    public async Task ScaleSet_NotNumber_KeepsScale()
    {
        await TInterfaceConductPortrait.TPressWindowOpen(new TPressBoard(), static press =>
        {
            press.TPressScaleSet("150");

            Assert.Equal(150, press.TPressScaleSet("big").CSPressSettingScale);
            return Task.CompletedTask;
        });
    }

    [Fact]
    public async Task PrinterSelect_UnknownCapabilities_UsesStandardPapers()
    {
        await TInterfaceConductPortrait.TPressWindowOpen(new TPressBoard(), static press =>
        {
            Assert.Equal(
                ["Print.PaperA4", "Print.PaperLetter"],
                press.TPressSettingRead().CSPressSettingPaper.Select(static paper => paper.CSPressPaperKey));

            CSPressSetting setting = press.TPressPrinterSelect("Plotter");

            Assert.Equal(
                [
                    "Print.PaperA3", "Print.PaperA4", "Print.PaperA5", "Print.PaperB5",
                    "Print.PaperLetter", "Print.PaperLegal", "Print.PaperTabloid",
                ],
                setting.CSPressSettingPaper.Select(static paper => paper.CSPressPaperKey));
            return Task.CompletedTask;
        });
    }

    [Fact]
    public async Task PrinterSelect_UnknownCapabilities_ShowsHint()
    {
        await TInterfaceConductPortrait.TPressWindowOpen(new TPressBoard(), static press =>
        {
            Assert.Null(press.TPressSettingRead().CSPressSettingHint);

            CSPressSetting setting = press.TPressPrinterSelect("Plotter");

            Assert.Equal("Print.CapabilityUnknown", setting.CSPressSettingHint);
            return Task.CompletedTask;
        });
    }

    [Fact]
    public async Task PrinterSelect_UnknownCapabilities_HidesCollation()
    {
        await TInterfaceConductPortrait.TPressWindowOpen(new TPressBoard(), static press =>
        {
            Assert.True(press.TPressCopiesSet("2").CSPressSettingCollation);

            CSPressSetting setting = press.TPressPrinterSelect("Plotter");

            Assert.Equal(2, setting.CSPressSettingCopies);
            Assert.Null(setting.CSPressSettingCollation);
            return Task.CompletedTask;
        });
    }

    [Fact]
    public async Task ColorSet_UnknownPrinter_FallsBackToFull()
    {
        await TInterfaceConductPortrait.TPressWindowOpen(new TPressBoard(), static press =>
        {
            press.TPressPrinterSelect("Plotter");

            CSPressSetting setting = press.TPressColorSet(CSPressColor.CSPressColorGray);

            Assert.Equal(CSPressColor.CSPressColorFull, setting.CSPressSettingColor);
            return Task.CompletedTask;
        });
    }

    [Fact]
    public async Task SideSet_UnknownPrinter_FallsBackToSingle()
    {
        await TInterfaceConductPortrait.TPressWindowOpen(new TPressBoard(), static press =>
        {
            press.TPressPrinterSelect("Plotter");

            CSPressSetting setting = press.TPressSideSet(CSPressSide.CSPressSideLong);

            Assert.Equal(
                CSPressSide.CSPressSideSingle,
                setting.CSPressSettingSide!.Single(static choice => choice.CSPressChoiceChosen).CSPressChoiceValue);
            return Task.CompletedTask;
        });
    }

    [Fact]
    public async Task ColorSet_ReportingPrinter_KeepsGray()
    {
        await TInterfaceConductPortrait.TPressWindowOpen(new TPressBoard(), static press =>
        {
            CSPressSetting setting = press.TPressColorSet(CSPressColor.CSPressColorGray);

            Assert.Equal(CSPressColor.CSPressColorGray, setting.CSPressSettingColor);
            return Task.CompletedTask;
        });
    }

    [Fact]
    public async Task SideSet_ReportingPrinter_KeepsLongEdge()
    {
        await TInterfaceConductPortrait.TPressWindowOpen(new TPressBoard(), static press =>
        {
            CSPressSetting setting = press.TPressSideSet(CSPressSide.CSPressSideLong);

            Assert.Equal(
                CSPressSide.CSPressSideLong,
                setting.CSPressSettingSide!.Single(static choice => choice.CSPressChoiceChosen).CSPressChoiceValue);
            return Task.CompletedTask;
        });
    }

    [Fact]
    public async Task MarginSet_TypedValues_StoresCustomMargin()
    {
        await TInterfaceConductPortrait.TPressWindowOpen(new TPressBoard(), static press =>
        {
            CSPressSetting setting = press.TPressMarginSet("1", "2", "3", "4");

            Assert.Equal(
                CSPressMargin.CSPressMarginCustom,
                setting.CSPressSettingMargin.Single(static choice => choice.CSPressChoiceChosen).CSPressChoiceValue);
            Assert.Equal(1.0, setting.CSPressSettingTop!.Value, 6);
            Assert.Equal(2.0, setting.CSPressSettingBottom!.Value, 6);
            Assert.Equal(3.0, setting.CSPressSettingLeft!.Value, 6);
            Assert.Equal(4.0, setting.CSPressSettingRight!.Value, 6);
            return Task.CompletedTask;
        });
    }

    [Fact]
    public async Task MarginSet_TypedPresetValues_PicksPreset()
    {
        await TInterfaceConductPortrait.TPressWindowOpen(new TPressBoard(), static press =>
        {
            CSPressSetting setting = press.TPressMarginSet("0", "0", "0", "0");

            Assert.Equal(
                CSPressMargin.CSPressMarginNone,
                setting.CSPressSettingMargin.Single(static choice => choice.CSPressChoiceChosen).CSPressChoiceValue);
            Assert.Null(setting.CSPressSettingTop);
            return Task.CompletedTask;
        });
    }

    [Fact]
    public async Task MarginSet_NotNumber_KeepsMargin()
    {
        await TInterfaceConductPortrait.TPressWindowOpen(new TPressBoard(), static press =>
        {
            press.TPressMarginSet(CSPressMargin.CSPressMarginWide);

            Assert.Equal(
                CSPressMargin.CSPressMarginWide,
                press.TPressMarginSet("1", "wide", "1", "1").CSPressSettingMargin
                    .Single(static choice => choice.CSPressChoiceChosen).CSPressChoiceValue);
            return Task.CompletedTask;
        });
    }

    [Fact]
    public async Task SideSet_UnreportedShortEdge_FallsBackToSingle()
    {
        await TInterfaceConductPortrait.TPressWindowOpen(new TPressBoard(), static press =>
        {
            CSPressSetting setting = press.TPressSideSet(CSPressSide.CSPressSideShort);

            Assert.Equal(
                CSPressSide.CSPressSideSingle,
                setting.CSPressSettingSide!.Single(static choice => choice.CSPressChoiceChosen).CSPressChoiceValue);
            return Task.CompletedTask;
        });
    }
}
