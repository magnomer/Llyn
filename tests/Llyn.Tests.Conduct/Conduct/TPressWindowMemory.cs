using System.Linq;
using System.Threading.Tasks;
using Llyn.Conduct;
using Xunit;

namespace Llyn.Tests;

public sealed class TPressWindowMemory
{
    [Fact]
    public async Task Restore_SavedSetting_RestoresSetting()
    {
        TPressVault vault = new();
        await TInterfaceConductPortrait.TPressWindowOpen(new TPressBoard(), press =>
        {
            press.TPressPrinterSelect("Plotter");
            press.TPressCopiesSet("3");
            press.TPressScaleSet("150");
            press.TPressOrientationSet(CSPressOrientation.CSPressOrientationLandscape);
            press.TPressBackgroundSet(false);
            press.TPressRangeSet(CSPressRange.CSPressRangeCustom, "1");
            Assert.True(press.TPressClose());
            return Task.CompletedTask;
        }, vault: vault);

        await TInterfaceConductPortrait.TPressWindowOpen(new TPressBoard(), async press =>
        {
            press.TPressOrientationSet(CSPressOrientation.CSPressOrientationPortrait);
            await press.TPressDraw();
            press.TPressRangeSet(CSPressRange.CSPressRangeCustom, "2");

            CSPressSetting setting = await press.TPressRestore();

            Assert.Equal(
                "Plotter",
                setting.CSPressSettingPrinter.Single(static item => item.CSPressPrinterChosen).CSPressPrinterName);
            Assert.Equal(3, setting.CSPressSettingCopies);
            Assert.Equal(150, setting.CSPressSettingScale);
            Assert.Equal(CSPressOrientation.CSPressOrientationLandscape, setting.CSPressSettingOrientation);
            Assert.False(setting.CSPressSettingBackground);
            Assert.Equal(
                CSPressRange.CSPressRangeAll,
                setting.CSPressSettingRange.Single(static choice => choice.CSPressChoiceChosen).CSPressChoiceValue);
            Assert.Null(setting.CSPressSettingText);
            Assert.Equal([true, true, true], setting.CSPressSettingIncluded);
            Assert.True(setting.CSPressSettingStale);
        }, vault: vault);
    }

    [Fact]
    public async Task Restore_NothingStored_KeepsSetting()
    {
        await TInterfaceConductPortrait.TPressWindowOpen(new TPressBoard(), static async press =>
        {
            press.TPressScaleSet("150");

            Assert.Equal(150, (await press.TPressRestore()).CSPressSettingScale);
        });
    }
}
