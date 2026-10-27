using Llyn.Infrastructure;
using Llyn.ShellEngine;
using Llyn.UIDemeanor;
using Xunit;

namespace Llyn.Tests;

public sealed class TTheme
{
    [Theory]
    [InlineData("console-blue")]
    [InlineData("console-dark")]
    public void ThemeLoaderLoad_ConsoleTheme_HoldsEveryRole(string name)
    {
        Func<string, string> read = LThemeLoader.LThemeLoaderLoad(name).LThemeColorRead;
        List<string> missing = [];

        _ = new BTheme(role =>
        {
            try
            {
                return read(role);
            }
            catch (KeyNotFoundException)
            {
                missing.Add(role);
                throw;
            }
        });

        Assert.Empty(missing);
    }

    [Fact]
    public void ThemeSpareRead_ConsoleBlue_MatchesFile()
    {
        Func<string, string> read = LThemeLoader.LThemeLoaderLoad(BTheme.BThemeDefault).LThemeColorRead;
        List<string> differing = [];

        _ = new BTheme(role =>
        {
            if (read(role) != BTheme.BThemeSpareRead(role))
            {
                differing.Add(role);
            }

            return read(role);
        });

        Assert.Empty(differing);
    }

    [Fact]
    public void ThemeLoaderLoad_UnknownName_Throws()
    {
        Assert.Throws<FileNotFoundException>(() => LThemeLoader.LThemeLoaderLoad("console-missing"));
    }

    [Fact]
    public void ThemeRead_MissingRole_DrawsAsText()
    {
        BTheme theme = new(role => role == "text" ? "gray on black" : throw new KeyNotFoundException(role));

        Assert.Equal("\e[37;40m", theme.BThemeRead("side"));
        Assert.Equal("\e[37;40m", theme.BThemeRead("unknown"));
    }

    [Fact]
    public void ThemeRead_TrueColour_FollowsConsoleColour()
    {
        BTheme theme = new(_ => "white #FFFFFF on darkBlue #000080");

        Assert.Equal("\e[97;44;38;2;255;255;255;48;2;0;0;128m", theme.BThemeRead("text"));
    }

    [Theory]
    [InlineData("cyan")]
    [InlineData("purple on black")]
    [InlineData("3 on black")]
    [InlineData("cyan #00FF on black")]
    public void Theme_MalformedValue_Throws(string value)
    {
        Assert.Throws<FormatException>(() => new BTheme(_ => value));
    }

    [Fact]
    public void Theme_MissingText_Throws()
    {
        Assert.Throws<KeyNotFoundException>(() => new BTheme(role => throw new KeyNotFoundException(role)));
    }

    [Fact]
    public void BootstrapIntroduce_BrokenTheme_FallsBackWithNotice()
    {
        const string missing = "The theme 'console-blue' is missing.";
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        TConsoleFake console = new() { TConsoleFakeTerminal = true };
        console.TConsoleFakeKeys.Add(new ConsoleKeyInfo('\e', ConsoleKey.Escape, false, false, false));
        console.TConsoleFakeKeys.Add(new ConsoleKeyInfo('\0', ConsoleKey.F10, false, false, false));

        int code = TInvocation.TInvocationRun(
            console,
            _ => throw new FileNotFoundException(missing),
            "en",
            workspace.TWorkspaceFolder,
            (_, _) => new LEngine(
                workspace.TWorkspaceRigCreate(), _ => workspace.TWorkspaceRigCreate(), static _ => { }, true),
            TInterfaceConduct.TAtelierCreate);

        Assert.Equal(0, code);
        string shot = string.Join('\n', console.TConsoleFakeShots[0]);
        Assert.Contains($"║  {"The console theme could not be loaded.",-52}  ║", shot);
        Assert.Contains($"║  {missing,-52}  ║", shot);
        Assert.Empty(console.TConsoleFakeError);
        Assert.DoesNotContain(new BTheme(BTheme.BThemeSpareRead).BThemeRead("alert"), console.TConsoleFakeFrames[0]);
        Assert.Contains("\e[96;44m", console.TConsoleFakeFrames[0]);
    }
}
