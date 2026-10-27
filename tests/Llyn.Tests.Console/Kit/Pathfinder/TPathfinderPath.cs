using Llyn.UIDemeanor;
using Xunit;
using static Llyn.Tests.TPathfinder;

namespace Llyn.Tests;

public sealed class TPathfinderPath
{
    [Fact]
    public void PathfinderMedia_FieldEnter_AcceptsRelativePath()
    {
        string? answer = TPathfinderRun(
            ["b\\", "b\\x.mp3"],
            root => [.. TPathfinderTextBuild(root), TPathfinderEnter, .. TPathfinderTextBuild("b\\x.mp3"),
                TPathfinderEnter],
            TPathfinderMediaConsult,
            out _);

        Assert.Equal("~\\b\\x.mp3", answer);
    }

    [Fact]
    public void PathfinderMedia_OpenWithTypedPath_AcceptsTypedPath()
    {
        string? answer = TPathfinderRun(
            ["a.mp3", "b\\", "b\\a.mp3"],
            root => [.. TPathfinderTextBuild(root), TPathfinderEnter, .. TPathfinderTextBuild("b\\a.mp3"),
                TPathfinderBack, TPathfinderEnter],
            TPathfinderMediaConsult,
            out _);

        Assert.Equal("~\\b\\a.mp3", answer);
    }

    [Fact]
    public void PathfinderMedia_TypedFolderPart_ListsFolder()
    {
        string? answer = TPathfinderRun(
            ["b\\", "b\\x.mp3", "xa.mp3"],
            root => [.. TPathfinderTextBuild(root), TPathfinderEnter, .. TPathfinderTextBuild("b\\x"),
                TPathfinderDown, TPathfinderEnter],
            TPathfinderMediaConsult,
            out var shots);

        Assert.Equal("~\\b\\x.mp3", answer);
        Assert.Contains(shots, static shot => shot.Contains("x.mp3", StringComparison.Ordinal)
            && !shot.Contains("xa.mp3", StringComparison.Ordinal));
    }

    [Fact]
    public void PathfinderMedia_TypedFolderUnreadable_ListsOnlyParent()
    {
        TPathfinderRun(
            ["a.mp3"],
            root => [.. TPathfinderTextBuild(root), TPathfinderEnter, .. TPathfinderTextBuild("absent\\"),
                TPathfinderExit],
            TPathfinderMediaConsult,
            out var shots);

        Assert.Contains("..\\", shots[^1], StringComparison.Ordinal);
        Assert.DoesNotContain("a.mp3", shots[^1], StringComparison.Ordinal);
        Assert.DoesNotContain("The folder could not be read.", shots[^1], StringComparison.Ordinal);
    }

    [Fact]
    public void PathfinderMedia_UnreadableFolderSegment_ListsNothing()
    {
        TPathfinderRun(
            ["a.mp3"],
            root => [.. TPathfinderTextBuild(root), TPathfinderEnter, .. TPathfinderTextBuild("absent\\x"),
                TPathfinderExit],
            TPathfinderMediaConsult,
            out var shots);

        Assert.DoesNotContain("..\\", shots[^1], StringComparison.Ordinal);
    }

    [Theory]
    [InlineData(@"E:\Notes", @"\Music", @"E:\Music")]
    [InlineData(@"E:\Notes", @"/Music", @"E:\Music")]
    [InlineData(@"\\server\share\Notes", @"\Music", @"\\server\share\Music")]
    public void PathfinderPath_RootedWithoutDrive_TakesShownRoot(string folder, string typed, string resolved)
    {
        Assert.Equal(resolved, BSPathfinderPath.BSPathfinderPathResolve(folder, typed));
    }

    [Theory]
    [InlineData(@"D:\Notes", "E:Music", @"E:\Music")]
    [InlineData(@"D:\Notes", @"E:..\Music", @"E:\Music")]
    [InlineData(@"D:\Notes", "E:", @"E:\")]
    public void PathfinderPath_OtherDriveRelative_ReadsFromDriveRoot(string folder, string typed, string resolved)
    {
        Assert.Equal(resolved, BSPathfinderPath.BSPathfinderPathResolve(folder, typed));
    }

    [Theory]
    [InlineData(@"E:\Notes", "E:Music", @"E:\Notes\Music")]
    [InlineData(@"e:\Notes", "E:Music", @"e:\Notes\Music")]
    [InlineData(@"E:\Notes\Words", "E:..", @"E:\Notes")]
    [InlineData(@"E:\Notes", "E:", @"E:\")]
    public void PathfinderPath_ShownDriveRelative_ReadsFromFolderShown(string folder, string typed, string resolved)
    {
        Assert.Equal(resolved, BSPathfinderPath.BSPathfinderPathResolve(folder, typed));
    }

    [Theory]
    [InlineData(@"E:\", "..", @"E:\")]
    [InlineData(@"E:\Notes", @"..\..", @"E:\")]
    [InlineData(@"\\server\share", "..", @"\\server\share\")]
    [InlineData(@"E:\Notes", @"\\server\share\x", @"\\server\share\x")]
    [InlineData(@"E:\Notes", @"F:\x\.\y", @"F:\x\y")]
    public void PathfinderPath_Climb_KeepsRoot(string folder, string typed, string resolved)
    {
        Assert.Equal(resolved, BSPathfinderPath.BSPathfinderPathResolve(folder, typed));
    }
}
