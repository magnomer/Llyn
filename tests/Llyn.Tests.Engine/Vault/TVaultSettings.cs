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
        settings.TSettingsSave(TInterface.TSettingsCreate("ko", true, false, true, false, true, "Korean"));
        LSettings read = settings.TSettingsRead();

        Assert.True(settings.TSettingsExist());
        Assert.Equal("ko", read.LSettingsLocalization);
        Assert.True(read.LSettingsRespelled);
        Assert.False(read.LSettingsFrequency);
        Assert.True(read.LSettingsMorphology);
        Assert.False(read.LSettingsEpithet);
        Assert.True(read.LSettingsTally);
        Assert.Equal("Korean", read.LSettingsGloss);
    }

    [Fact]
    public void SettingsSave_AfterLockedRead_KeepsStoredFile()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        LSettingsVault settings = TInterface.TSettingsVaultCreate(workspace.TWorkspaceFolder);
        settings.TSettingsSave(TInterface.TSettingsCreate("ko"));
        string path = Path.Combine(workspace.TWorkspaceFolder, "settings.json");
        string before = File.ReadAllText(path);

        using (new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.None))
        {
            Assert.Equal("en", settings.TSettingsRead().LSettingsLocalization);
        }

        Assert.ThrowsAny<Exception>(() => settings.TSettingsSave(TInterface.TSettingsCreate("fr")));
        Assert.Equal(before, File.ReadAllText(path));
    }

    [Fact]
    public void SettingsRead_NothingStored_ReturnsDefaults()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        LSettingsVault settings = TInterface.TSettingsVaultCreate(workspace.TWorkspaceFolder);

        LSettings read = settings.TSettingsRead();

        Assert.Equal("en", read.LSettingsLocalization);
        Assert.False(read.LSettingsRespelled);
        Assert.Equal("English", read.LSettingsGloss);
    }
}
