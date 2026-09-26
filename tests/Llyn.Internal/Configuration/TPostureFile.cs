using Llyn.Core;
using Llyn.Infrastructure;
using Xunit;

namespace Llyn.Tests;

public sealed class TPostureFile
{
    [Fact]
    public void PostureRead_NoFile_ReturnsNull()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        LPostureFile file = TInterface.TPostureFileCreate(workspace.TWorkspaceFolder);

        Assert.Null(file.TPostureFileRead("posture"));
    }

    [Fact]
    public void PostureSave_ThenRead_RoundTripsState()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        LPostureFile file = TInterface.TPostureFileCreate(workspace.TWorkspaceFolder);
        LPostureState state = TInterface.TPostureStateCreate(
            TInterface.TWindowStateCreate(10, 20, 800, 600, false), mode: "Corpus", split: true, volume: 0.5);

        file.TPostureFileSave("posture", state);

        Assert.Equal(state, file.TPostureFileRead("posture"));
        Assert.True(File.Exists(Path.Combine(workspace.TWorkspaceFolder, "posture.json")));
    }

    [Fact]
    public void PostureRead_LegacyKey_ReadsSameShape()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        File.WriteAllText(Path.Combine(workspace.TWorkspaceFolder, "settings.json"), "{ \"mode\": \"Tenor\" }");
        LPostureFile file = TInterface.TPostureFileCreate(workspace.TWorkspaceFolder);

        Assert.Equal("Tenor", file.TPostureFileRead("settings")?.LPostureStateMode);
    }

    [Fact]
    public void PostureRead_LockedFile_RaisesVaultFault()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        string path = Path.Combine(workspace.TWorkspaceFolder, "posture.json");
        File.WriteAllText(path, "{ }");
        LPostureFile file = TInterface.TPostureFileCreate(workspace.TWorkspaceFolder);

        using (new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.None))
        {
            Assert.Throws<LVaultFault>(() => file.TPostureFileRead("posture"));
        }
    }
}
