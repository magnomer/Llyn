using Xunit;

namespace Llyn.Tests;

public sealed class TUsher
{
    [Fact]
    public void UsherPathExist_PresentFile_ReturnsTrue()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        string path = Path.Combine(workspace.TWorkspaceFolder, "flag.svg");
        File.WriteAllText(path, "<svg />");

        Assert.True(TInterface.TUsherPathExist(path));
    }

    [Fact]
    public void UsherPathExist_MissingFile_ReturnsFalse()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();

        Assert.False(TInterface.TUsherPathExist(Path.Combine(workspace.TWorkspaceFolder, "flag.svg")));
        Assert.False(TInterface.TUsherPathExist(null));
    }

    [Fact]
    public void UsherFolderOpen_MissingFolder_Throws()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        string missing = Path.Combine(workspace.TWorkspaceFolder, "absent");

        Assert.ThrowsAny<Exception>(() => TInterface.TUsherFolderOpen(missing));
    }
}
