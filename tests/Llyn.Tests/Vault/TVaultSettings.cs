using Llyn.Core;
using Xunit;

namespace Llyn.Tests;

public sealed class TVaultSettings
{
    [Fact]
    public void SettingsRead_AfterSave_ReturnsStoredSettings()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        LSettingsVault settings = TInterface.TSettingsVaultCreate(workspace.TWorkspaceFolder);

        Assert.False(settings.TSettingsExist());
        settings.TSettingsSave(TInterface.TSettingsCreate("ko", true, false, true, false, true));
        LSettings read = settings.TSettingsRead();

        Assert.True(settings.TSettingsExist());
        Assert.Equal("ko", read.LSettingsLocalization);
        Assert.True(read.LSettingsRespelled);
        Assert.False(read.LSettingsFrequency);
        Assert.True(read.LSettingsMorphology);
        Assert.False(read.LSettingsEpithet);
        Assert.True(read.LSettingsTally);
    }

    [Fact]
    public void SettingsRead_NothingStored_ReturnsDefaults()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        LSettingsVault settings = TInterface.TSettingsVaultCreate(workspace.TWorkspaceFolder);

        LSettings read = settings.TSettingsRead();

        Assert.Equal("en", read.LSettingsLocalization);
        Assert.False(read.LSettingsRespelled);
    }
}
