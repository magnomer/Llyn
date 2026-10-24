using Llyn.UIDemeanor;
using Xunit;

namespace Llyn.Tests;

public sealed class TDrive
{
    private static readonly ConsoleKeyInfo TDriveEnter = new('\r', ConsoleKey.Enter, false, false, false);
    private static readonly ConsoleKeyInfo TDriveEscape = new('\e', ConsoleKey.Escape, false, false, false);
    private static readonly ConsoleKeyInfo TDriveDown = new('\0', ConsoleKey.DownArrow, false, false, false);
    private static readonly ConsoleKeyInfo TDriveHotkey = new('\0', ConsoleKey.F1, false, true, false);

    [Fact]
    public void DriveList_Enter_OpensChosenDrive()
    {
        List<string> asked = [];

        string? answer = TPathfinder.TPathfinderRun(
            [],
            _ => [TDriveHotkey, TDriveDown, TDriveEnter, TDriveEnter],
            TDriveFolderConsult,
            out var shots,
            folder: path =>
            {
                asked.Add(path);
                return path.StartsWith("Y:", StringComparison.Ordinal)
                    ? ([], [])
                    : (Directory.GetDirectories(path), Directory.GetFiles(path));
            },
            drives: static () => ["X:\\", "Y:\\"]);

        Assert.Equal("Y:\\", answer);
        Assert.Contains("Y:\\", asked);
        Assert.Contains(shots, static shot => shot.Contains(" Choose a drive ", StringComparison.Ordinal));
    }

    [Fact]
    public void DriveList_Escape_KeepsFolder()
    {
        string? answer = TPathfinder.TPathfinderRun(
            [],
            _ => [TDriveHotkey, TDriveEscape, TDriveEnter],
            TDriveFolderConsult,
            out _,
            drives: static () => ["X:\\"]);

        Assert.Equal("~", answer);
    }

    [Fact]
    public void DriveList_SeamFails_ShowsNothing()
    {
        string? answer = TPathfinder.TPathfinderRun(
            [],
            _ => [TDriveHotkey, TDriveEnter],
            TDriveFolderConsult,
            out var shots,
            drives: static () => throw new IOException("drives"));

        Assert.Equal("~", answer);
        Assert.DoesNotContain(shots, static shot => shot.Contains("could not be read", StringComparison.Ordinal));
        Assert.DoesNotContain(shots, static shot => shot.Contains(" Choose a drive ", StringComparison.Ordinal));
    }

    [Fact]
    public void DriveList_Help_ShowsDriveLegend()
    {
        TPathfinder.TPathfinderRun(
            [],
            _ => [new ConsoleKeyInfo('\0', ConsoleKey.F1, false, false, false), TDriveEscape, TDriveEscape],
            TDriveFolderConsult,
            out var shots,
            drives: static () => ["X:\\"]);

        Assert.Contains(shots, static shot => shot.Contains("Drives", StringComparison.Ordinal));
    }

    private static string? TDriveFolderConsult(BEnvoy envoy, string root)
    {
        return envoy.CEnvoyFolderRead(root);
    }
}
