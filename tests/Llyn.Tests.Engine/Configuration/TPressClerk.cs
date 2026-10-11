using Llyn.Application;
using Llyn.Core;
using Xunit;

namespace Llyn.Tests;

public sealed class TPressClerk
{
    [Fact]
    public void PressClerkRestore_NothingStored_ReturnsNull()
    {
        LPressClerk clerk = TInterface.TPressClerkCreate(new TPressVault());

        Assert.Null(clerk.TPressClerkRestore(TInterface.TPressPrinterRead()));
    }

    [Fact]
    public void PressClerkRestore_UnlistedPrinter_TakesDefaultPrinter()
    {
        TPressVault vault = new()
        {
            TPressVaultSetting = TInterface.TPressSettingCreate(null) with
            {
                LPressSettingPrinter = "Gone",
                LPressSettingRange = TInterface.TPressRangeCreate("2"),
            },
        };
        LPressClerk clerk = TInterface.TPressClerkCreate(vault);

        LPressSetting? setting = clerk.TPressClerkRestore(TInterface.TPressPrinterRead());

        Assert.Equal("Office Laser", setting?.LPressSettingPrinter);
        Assert.Equal(string.Empty, setting?.LPressSettingRange.LPressRangeText);
    }

    [Fact]
    public void PressClerkRestore_SaveAsPdf_KeepsNoPrinter()
    {
        TPressVault vault = new() { TPressVaultSetting = TInterface.TPressSettingCreate(null) };
        LPressClerk clerk = TInterface.TPressClerkCreate(vault);

        LPressSetting? setting = clerk.TPressClerkRestore(TInterface.TPressPrinterRead());

        Assert.NotNull(setting);
        Assert.Null(setting.LPressSettingPrinter);
    }

    [Fact]
    public void PressClerkSave_TypedRange_StoresEmptyRange()
    {
        TPressVault vault = new();
        LPressClerk clerk = TInterface.TPressClerkCreate(vault);
        LPressSetting setting = TInterface.TPressSettingCreate(null) with
        {
            LPressSettingScale = 150,
            LPressSettingRange = TInterface.TPressRangeCreate("1-2"),
        };

        clerk.TPressClerkSave(setting);

        Assert.Equal(string.Empty, vault.TPressVaultSetting?.LPressSettingRange.LPressRangeText);
        Assert.Equal(150, vault.TPressVaultSetting?.LPressSettingScale);
    }
}
