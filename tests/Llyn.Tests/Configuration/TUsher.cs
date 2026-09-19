using Llyn.Core;
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
        LUsher usher = TInterface.TUsherFileCreate();

        Assert.True(usher.TUsherPathExist(path));
    }

    [Fact]
    public void UsherPathExist_MissingFile_ReturnsFalse()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        LUsher usher = TInterface.TUsherFileCreate();

        Assert.False(usher.TUsherPathExist(Path.Combine(workspace.TWorkspaceFolder, "flag.svg")));
        Assert.False(usher.TUsherPathExist(null));
    }

    [Fact]
    public void UsherPathDelete_PresentFile_RemovesFile()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        string path = Path.Combine(workspace.TWorkspaceFolder, "broken.svg");
        File.WriteAllText(path, "<svg");
        LUsher usher = TInterface.TUsherFileCreate();

        usher.TUsherPathDelete(path);
        usher.TUsherPathDelete(path);

        Assert.False(File.Exists(path));
    }
}
