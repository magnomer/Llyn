using Llyn.Core;
using Llyn.Infrastructure;
using Xunit;

namespace Llyn.Tests;

public sealed class TPressFile
{
    [Fact]
    public void PressVaultRead_NoFile_ReturnsNull()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        LPressFile file = TInterface.TPressFileCreate(workspace.TWorkspaceFolder);

        Assert.Null(file.TPressFileRead());
    }

    [Fact]
    public void PressVaultSave_ThenRead_RoundTripsSetting()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        LPressFile file = TInterface.TPressFileCreate(workspace.TWorkspaceFolder);
        LPressSetting setting = TInterface.TPressSettingCreate(TInterface.TPressPrinterRead()[0]) with
        {
            LPressSettingCopies = 3,
            LPressSettingScale = 150,
            LPressSettingOrientation = LPressOrientation.LPressOrientationLandscape,
            LPressSettingBackground = false,
        };

        file.TPressFileSave(setting);

        Assert.Equal(setting, file.TPressFileRead());
        Assert.True(File.Exists(Path.Combine(workspace.TWorkspaceFolder, "press.json")));
    }

    [Fact]
    public void PressVaultRead_UnreadableText_ReturnsNull()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        File.WriteAllText(Path.Combine(workspace.TWorkspaceFolder, "press.json"), "{not json");
        LPressFile file = TInterface.TPressFileCreate(workspace.TWorkspaceFolder);

        Assert.Null(file.TPressFileRead());
    }

    [Fact]
    public void PressFileParse_BlankOrMalformed_ReturnsNull()
    {
        Assert.Null(TInterface.TPressFileParse("{not json"));
        Assert.Null(TInterface.TPressFileParse(null));
        Assert.Null(TInterface.TPressFileParse(string.Empty));
    }
}
