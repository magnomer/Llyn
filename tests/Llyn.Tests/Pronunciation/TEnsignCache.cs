using Llyn.Application;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TEnsignCache
{
    [Fact]
    public void EnsignMissingRead_UnaskedKeys_ReturnsEachOnce()
    {
        TInterface.TEnsignClear();

        string[] missing = TInterface.TEnsignMissingRead(["English", "Korean", "English"], out _);

        Assert.Equal(["English", "Korean"], missing);
    }

    [Fact]
    public void EnsignPathAdd_PresentFile_KeepsPath()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        string path = Path.Combine(workspace.TWorkspaceFolder, "en.svg");
        File.WriteAllText(path, "<svg />");
        TInterface.TEnsignClear();
        string[] missing = TInterface.TEnsignMissingRead(["English"], out int age);

        IReadOnlyList<LEnsignRow> kept = TInterface.TEnsignPathAdd(age, missing, [path]);

        Assert.Equal([TInterface.TEnsignRowCreate("English", path)], kept);
        Assert.Equal(path, TInterface.TEnsignPathRead("English"));
        Assert.Empty(TInterface.TEnsignMissingRead(["English"], out _));
    }

    [Fact]
    public void EnsignPathAdd_MissingFile_RecordsNothingForKey()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        TInterface.TEnsignClear();
        string[] missing = TInterface.TEnsignMissingRead(["Korean"], out int age);

        IReadOnlyList<LEnsignRow> kept = TInterface.TEnsignPathAdd(
            age, missing, [Path.Combine(workspace.TWorkspaceFolder, "ko.svg")]);

        Assert.Empty(kept);
        Assert.Null(TInterface.TEnsignPathRead("Korean"));
        Assert.Empty(TInterface.TEnsignMissingRead(["Korean"], out _));
    }

    [Fact]
    public void EnsignPathAdd_StaleAge_RecordsNothing()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        string path = Path.Combine(workspace.TWorkspaceFolder, "en.svg");
        File.WriteAllText(path, "<svg />");
        TInterface.TEnsignClear();
        string[] missing = TInterface.TEnsignMissingRead(["English"], out int age);
        TInterface.TEnsignClear();

        IReadOnlyList<LEnsignRow> kept = TInterface.TEnsignPathAdd(age, missing, [path]);

        Assert.Empty(kept);
        Assert.Null(TInterface.TEnsignPathRead("English"));
        Assert.Equal(["English"], TInterface.TEnsignMissingRead(["English"], out _));
    }

    [Fact]
    public void EnsignKeyFormat_LanguageAndVariety_JoinsWithSlash()
    {
        Assert.Equal("English/Scottish", TInterface.TEnsignKeyFormat("English", "Scottish"));
    }

    [Fact]
    public async Task EnsignLoad_UnflaggedVariety_RecordsAsAsked()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        TInterface.TEnsignClear();

        IReadOnlyList<LEnsignRow> kept = await engine.TEngineEnsignLoad("English", ["Nowhere"]);

        Assert.Empty(kept);
        Assert.Empty(TInterface.TEnsignMissingRead(["English/Nowhere"], out _));
    }

    [Fact]
    public void EnsignPathDelete_PresentFile_RemovesFile()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        string path = Path.Combine(workspace.TWorkspaceFolder, "broken.svg");
        File.WriteAllText(path, "<svg");

        TInterface.TEnsignPathDelete(path);
        TInterface.TEnsignPathDelete(path);

        Assert.False(File.Exists(path));
    }
}
