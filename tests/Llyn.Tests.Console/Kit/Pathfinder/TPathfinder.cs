using Llyn.UIDemeanor;
using Xunit;

namespace Llyn.Tests;

public sealed class TPathfinder
{
    private static readonly ConsoleKeyInfo TPathfinderEnter = new('\r', ConsoleKey.Enter, false, false, false);
    private static readonly ConsoleKeyInfo TPathfinderEscape = new('\e', ConsoleKey.Escape, false, false, false);
    private static readonly ConsoleKeyInfo TPathfinderExit = new('\0', ConsoleKey.F10, false, false, false);
    private static readonly ConsoleKeyInfo TPathfinderDown = new('\0', ConsoleKey.DownArrow, false, false, false);
    private static readonly ConsoleKeyInfo TPathfinderTab = new('\t', ConsoleKey.Tab, false, false, false);
    private static readonly ConsoleKeyInfo TPathfinderBack = new('\t', ConsoleKey.Tab, true, false, false);
    private static readonly ConsoleKeyInfo TPathfinderErase = new('\b', ConsoleKey.Backspace, false, false, false);

    [Fact]
    public void PathfinderFolder_FieldEnter_ChoosesFolderShown()
    {
        string? answer = TPathfinderRun([], _ => [TPathfinderEnter], TPathfinderFolderConsult, out _);

        Assert.Equal("~", answer);
    }

    [Fact]
    public void PathfinderWorkspace_Escape_AnswersNull()
    {
        string? answer = TPathfinderRun(
            [], _ => [TPathfinderEscape], static (envoy, root) => envoy.CEnvoyWorkspaceRead(root), out var shots);

        Assert.Null(answer);
        Assert.Contains(shots, static shot => shot.Contains(" Workspace ", StringComparison.Ordinal));
    }

    [Fact]
    public void PathfinderFolder_CancelButton_AnswersNull()
    {
        ConsoleKeyInfo right = new('\0', ConsoleKey.RightArrow, false, false, false);

        string? answer = TPathfinderRun(
            [], _ => [TPathfinderTab, TPathfinderTab, right, TPathfinderEnter], TPathfinderFolderConsult, out _);

        Assert.Null(answer);
    }

    [Fact]
    public void PathfinderFolder_TabTabEnter_PressesChoose()
    {
        string? answer = TPathfinderRun(
            ["b\\"],
            _ => [TPathfinderTab, TPathfinderTab, TPathfinderKeyBuild('x'), TPathfinderEnter],
            TPathfinderFolderConsult,
            out _);

        Assert.Equal("~", answer);
    }

    [Fact]
    public void PathfinderFolder_ShiftTabFromField_ReachesButtons()
    {
        string? answer = TPathfinderRun(
            ["b\\"], _ => [TPathfinderBack, TPathfinderKeyBuild('x'), TPathfinderEnter], TPathfinderFolderConsult,
            out _);

        Assert.Equal("~", answer);
    }

    [Fact]
    public void PathfinderFolder_ShiftTabThenTab_LandsInField()
    {
        string? answer = TPathfinderRun(
            ["b\\"],
            _ => [TPathfinderBack, TPathfinderTab, TPathfinderKeyBuild('b'), TPathfinderEnter, TPathfinderExit],
            TPathfinderFolderConsult,
            out _);

        Assert.Equal("~\\b", answer);
    }

    [Fact]
    public void PathfinderFolder_ListEnter_EntersFolder()
    {
        string? answer = TPathfinderRun(
            ["b\\"],
            _ => [TPathfinderDown, TPathfinderDown, TPathfinderEnter, TPathfinderBack, TPathfinderEnter],
            TPathfinderFolderConsult,
            out _);

        Assert.Equal("~\\b", answer);
    }

    [Fact]
    public void PathfinderFolder_ListEnterOnParent_Climbs()
    {
        string? answer = TPathfinderRun(
            ["b\\"],
            _ => [TPathfinderDown, TPathfinderEnter, TPathfinderBack, TPathfinderEnter],
            static (envoy, root) => envoy.CEnvoyFolderRead(Path.Combine(root, "b")),
            out _);

        Assert.Equal("~", answer);
    }

    [Fact]
    public void PathfinderFolder_ListBackspace_Climbs()
    {
        string? answer = TPathfinderRun(
            ["b\\"],
            _ => [TPathfinderDown, TPathfinderErase, TPathfinderBack, TPathfinderEnter],
            static (envoy, root) => envoy.CEnvoyFolderRead(Path.Combine(root, "b")),
            out _);

        Assert.Equal("~", answer);
    }

    [Fact]
    public void PathfinderMedia_Open_ListsFoldersAndAskedSuffixes()
    {
        TPathfinderRun(
            ["zz\\", "a.mp3", "c.txt"],
            root => [.. TPathfinderTextBuild(root), TPathfinderEnter, TPathfinderEscape],
            TPathfinderMediaConsult,
            out var shots);

        Assert.Contains("..\\", shots[^1], StringComparison.Ordinal);
        Assert.Contains("zz\\", shots[^1], StringComparison.Ordinal);
        Assert.Contains("a.mp3", shots[^1], StringComparison.Ordinal);
        Assert.DoesNotContain("c.txt", shots[^1], StringComparison.Ordinal);
    }

    [Fact]
    public void PathfinderMedia_ListEnter_AcceptsFile()
    {
        string? answer = TPathfinderRun(
            ["zz\\", "a.mp3"],
            root => [.. TPathfinderTextBuild(root), TPathfinderEnter, TPathfinderDown, TPathfinderDown, TPathfinderDown,
                TPathfinderEnter],
            TPathfinderMediaConsult,
            out _);

        Assert.Equal("~\\a.mp3", answer);
    }

    [Fact]
    public void PathfinderMedia_Typing_NarrowsList()
    {
        TPathfinderRun(
            ["zz\\", "a.mp3", "b.mp3"],
            root => [.. TPathfinderTextBuild(root), TPathfinderEnter, TPathfinderKeyBuild('a'), TPathfinderExit],
            TPathfinderMediaConsult,
            out var shots);

        Assert.Contains("a.mp3", shots[^1], StringComparison.Ordinal);
        Assert.DoesNotContain("b.mp3", shots[^1], StringComparison.Ordinal);
        Assert.DoesNotContain("zz\\", shots[^1], StringComparison.Ordinal);
    }

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
    public void PathfinderFolder_Help_ListsPathKeys()
    {
        ConsoleKeyInfo help = new('\0', ConsoleKey.F1, false, false, false);

        TPathfinderRun(
            [],
            _ => [help, TPathfinderEscape, TPathfinderDown, help, TPathfinderEscape, TPathfinderEscape],
            TPathfinderFolderConsult,
            out var shots);

        Assert.Contains(
            shots, static shot => shot.Contains("next or previous item.") && !shot.Contains("Parent folder"));
        Assert.Contains(shots, static shot => shot.Contains("Parent folder"));
    }

    [Fact]
    public void PathfinderMedia_MissingFile_ShowsAlertAndStays()
    {
        string? answer = TPathfinderRun(
            ["a.mp3"],
            root => [.. TPathfinderTextBuild(root + "\\absent.mp3"), TPathfinderEnter, TPathfinderEscape],
            TPathfinderMediaConsult,
            out var shots);

        string[] first = shots[0].Split('\n');
        string[] last = shots[^1].Split('\n');
        int alert = Array.FindIndex(
            last, static row => row.Contains("The file could not be found.", StringComparison.Ordinal));
        Assert.Null(answer);
        Assert.True(alert > 0);
        Assert.Contains("\\absent.mp3", shots[^1], StringComparison.Ordinal);
        Assert.Equal(first[alert + 1], last[alert + 1]);
    }

    [Fact]
    public void PathfinderFolder_UnreadableStart_ShowsParentWithAlert()
    {
        string? answer = TPathfinderRun(
            ["locked\\"],
            _ => [TPathfinderEnter],
            static (envoy, root) => envoy.CEnvoyFolderRead(Path.Combine(root, "locked")),
            out var shots,
            TPathfinderListingRead);

        Assert.Equal("~", answer);
        Assert.Contains("The folder could not be read.", shots[^1], StringComparison.Ordinal);
    }

    [Fact]
    public void PathfinderFolder_UnreadableEntry_StaysWithAlert()
    {
        string? answer = TPathfinderRun(
            ["locked\\"],
            _ => [TPathfinderDown, TPathfinderDown, TPathfinderEnter, TPathfinderTab, TPathfinderEnter],
            TPathfinderFolderConsult,
            out var shots,
            TPathfinderListingRead);

        Assert.Equal("~", answer);
        Assert.Contains(shots, static shot => shot.Contains("The folder could not be read.", StringComparison.Ordinal));
    }

    [Fact]
    public void PathfinderFolder_EmptyFolder_ListsOnlyParent()
    {
        TPathfinderRun([], _ => [TPathfinderExit], TPathfinderFolderConsult, out var shots);

        string[] rows = shots[^1].Split('\n');
        int parent = Array.FindIndex(rows, static row => row.Contains("..\\", StringComparison.Ordinal));
        Assert.Equal($"{"..\\",-52}", rows[parent][13..65]);
        Assert.Equal(new string(' ', 52), rows[parent + 1][13..65]);
    }

    [Fact]
    public void PathfinderFolder_LongFolder_ClipsLeftWithEllipsis()
    {
        string deep = new('d', 70);

        TPathfinderRun(
            [deep + "\\"],
            _ => [TPathfinderExit],
            (envoy, root) => envoy.CEnvoyFolderRead(Path.Combine(root, deep)),
            out var shots,
            prime: static window =>
            {
                foreach (ConsoleKey key in (ConsoleKey[])[ConsoleKey.O, ConsoleKey.DownArrow, ConsoleKey.RightArrow,
                    ConsoleKey.DownArrow, ConsoleKey.DownArrow, ConsoleKey.Enter])
                {
                    window.BWindowKeyObserve(new ConsoleKeyInfo(
                        key == ConsoleKey.O ? 'o' : '\0', key, false, key == ConsoleKey.O, false));
                }
            });

        Assert.Contains($"  ~{new string('d', 51)}  |", shots[^1], StringComparison.Ordinal);
    }

    [Fact]
    public void PathfinderMedia_WideName_ClipsByCells()
    {
        TPathfinderRun(
            [new string('東', 30) + ".mp3"],
            root => [.. TPathfinderTextBuild(root), TPathfinderEnter, TPathfinderExit],
            TPathfinderMediaConsult,
            out var shots);

        Assert.Contains($"  {new string('東', 26)}  ║", shots[^1], StringComparison.Ordinal);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Pathfinder_NoFolder_StartsInWorkspace(bool markup)
    {
        TConsoleFake console = new() { TConsoleFakeTerminal = true };
        console.TConsoleFakeKeys.Add(TPathfinderEscape);
        console.TConsoleFakeKeys.Add(TPathfinderExit);
        List<string> asked = [];
        string workspace = string.Empty;

        TEnvoy.TEnvoyRun(
            console,
            envoy => markup ? envoy.CEnvoyMarkupRead("Markup.Title", [".llx"]) : envoy.CEnvoyFolderRead(string.Empty),
            settled: engine => workspace = engine.LEngineWorkspace.LEngineWorkspaceRead(),
            folder: path =>
            {
                asked.Add(path);
                return ([], []);
            });

        Assert.Equal(workspace.TrimEnd('\\'), asked[0].TrimEnd('\\'));
        string title = markup ? " Choose a markup file " : "[ Choose ]";
        Assert.Contains(console.TConsoleFakeShots, shot => string.Concat(shot).Contains(title));
    }

    internal static string? TPathfinderRun(
        string[] entries,
        Func<string, IEnumerable<ConsoleKeyInfo>> keys,
        Func<BEnvoy, string, string?> ask,
        out IReadOnlyList<string> shots,
        Func<string, (IReadOnlyList<string>, IReadOnlyList<string>)>? folder = null,
        Action<BWindow>? prime = null, Func<string[]>? drives = null)
    {
        string root = Directory.CreateTempSubdirectory("llyn-pathfinder-").FullName;
        try
        {
            foreach (string entry in entries)
            {
                string path = Path.Combine(root, entry);
                if (entry.EndsWith('\\'))
                {
                    Directory.CreateDirectory(path);
                }
                else
                {
                    File.WriteAllText(path, string.Empty);
                }
            }

            TConsoleFake console = new() { TConsoleFakeTerminal = true };
            Task feeder = Task.Run(() =>
            {
                SpinWait.SpinUntil(
                    () => console.TConsoleFakeShots.Count > 0
                        && string.Concat(console.TConsoleFakeShots[^1])
                            .Contains("[ Cancel ]", StringComparison.Ordinal),
                    5000);
                foreach (ConsoleKeyInfo key in keys(root))
                {
                    int count = console.TConsoleFakeFrames.Count;
                    console.TConsoleFakeKeys.Add(key);
                    SpinWait.SpinUntil(() => console.TConsoleFakeFrames.Count > count, 5000);
                }

                console.TConsoleFakeKeys.Add(TPathfinderExit);
            });

            object? answer = TEnvoy.TEnvoyRun(
                console, envoy => ask(envoy, root), prime: prime, folder: folder, drives: drives);
            feeder.Wait();
            shots = [.. console.TConsoleFakeShots.Select(static shot => string.Join('\n', shot))
                .Where(static shot => shot.Contains("[ Cancel ]", StringComparison.Ordinal)
                    || shot.Contains("[ Close ]", StringComparison.Ordinal))];
            return (answer as string)?.Replace(root, "~", StringComparison.Ordinal);
        }
        finally
        {
            Directory.Delete(root, true);
        }
    }

    private static string? TPathfinderFolderConsult(BEnvoy envoy, string root)
    {
        return envoy.CEnvoyFolderRead(root);
    }

    private static string? TPathfinderMediaConsult(BEnvoy envoy, string _)
    {
        return envoy.CEnvoyMediaRead("Cue.Pick", [".mp3"]);
    }

    private static (IReadOnlyList<string>, IReadOnlyList<string>) TPathfinderListingRead(string path)
    {
        return path.EndsWith("locked", StringComparison.OrdinalIgnoreCase)
            ? throw new UnauthorizedAccessException(path)
            : (Directory.GetDirectories(path), Directory.GetFiles(path));
    }

    internal static ConsoleKeyInfo TPathfinderKeyBuild(char letter)
    {
        return new ConsoleKeyInfo(letter, ConsoleKey.A, false, false, false);
    }

    internal static IEnumerable<ConsoleKeyInfo> TPathfinderTextBuild(string text)
    {
        return text.Select(TPathfinderKeyBuild);
    }
}
