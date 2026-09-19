using Llyn.Application;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TEnsign
{
    [Fact]
    public void EnsignMissingRead_UnaskedKeys_ReturnsEachOnce()
    {
        LEnsign ensign = TInterface.TEnsignCreate(new TUsherFake());

        string[] missing = ensign.TEnsignMissingRead(["English", "Korean", "English"], out _);

        Assert.Equal(["English", "Korean"], missing);
    }

    [Fact]
    public void EnsignPathAdd_PresentFile_KeepsPath()
    {
        TUsherFake usher = new();
        usher.TUsherPresent.Add("C:/flags/en.svg");
        LEnsign ensign = TInterface.TEnsignCreate(usher);
        string[] missing = ensign.TEnsignMissingRead(["English"], out int age);

        IReadOnlyList<LEnsignRow> kept = ensign.TEnsignPathAdd(age, missing, ["C:/flags/en.svg"]);

        Assert.Equal([TInterface.TEnsignRowCreate("English", "C:/flags/en.svg")], kept);
        Assert.Equal("C:/flags/en.svg", ensign.TEnsignPathRead("English"));
        Assert.Empty(ensign.TEnsignMissingRead(["English"], out _));
    }

    [Fact]
    public void EnsignPathAdd_MissingFile_RecordsNothingForKey()
    {
        LEnsign ensign = TInterface.TEnsignCreate(new TUsherFake());
        string[] missing = ensign.TEnsignMissingRead(["Korean"], out int age);

        IReadOnlyList<LEnsignRow> kept = ensign.TEnsignPathAdd(age, missing, ["C:/flags/ko.svg"]);

        Assert.Empty(kept);
        Assert.Null(ensign.TEnsignPathRead("Korean"));
        Assert.Empty(ensign.TEnsignMissingRead(["Korean"], out _));
    }

    [Fact]
    public void EnsignPathAdd_StaleAge_RecordsNothing()
    {
        TUsherFake usher = new();
        usher.TUsherPresent.Add("C:/flags/en.svg");
        LEnsign ensign = TInterface.TEnsignCreate(usher);
        string[] missing = ensign.TEnsignMissingRead(["English"], out int age);
        ensign.TEnsignClear();

        IReadOnlyList<LEnsignRow> kept = ensign.TEnsignPathAdd(age, missing, ["C:/flags/en.svg"]);

        Assert.Empty(kept);
        Assert.Null(ensign.TEnsignPathRead("English"));
        Assert.Equal(["English"], ensign.TEnsignMissingRead(["English"], out _));
    }

    [Fact]
    public void EnsignKeyFormat_LanguageAndVariety_JoinsWithSlash()
    {
        Assert.Equal("English/Scottish", TInterface.TEnsignKeyFormat("English", "Scottish"));
    }

    [Fact]
    public async Task EnsignLoad_UnflaggedVariety_ReturnsNoRow()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        IReadOnlyList<LEnsignRow> kept = await engine.TEngineEnsignLoad("English", ["Nowhere"]);

        Assert.Empty(kept);
    }

    [Fact]
    public void EnsignPathDelete_BrokenFile_ForwardsOnce()
    {
        TUsherFake usher = new();
        LEnsign ensign = TInterface.TEnsignCreate(usher);

        ensign.TEnsignPathDelete("C:/flags/broken.svg");

        Assert.Equal(["C:/flags/broken.svg"], usher.TUsherDeleted);
    }

    private sealed class TUsherFake : LUsher
    {
        internal HashSet<string> TUsherPresent { get; } = new(StringComparer.Ordinal);

        internal List<string> TUsherDeleted { get; } = [];

        public bool LUsherPathExist(string? path) => path is not null && TUsherPresent.Contains(path);

        public void LUsherPathDelete(string path) => TUsherDeleted.Add(path);
    }
}
