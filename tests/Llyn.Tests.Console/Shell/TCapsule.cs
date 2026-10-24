using Llyn.Conduct;
using Llyn.ShellEngine;
using Llyn.UIDemeanor;
using Llyn.UIDemeanor.Capsule;
using Xunit;

namespace Llyn.Tests;

public sealed class TCapsule
{
    private static readonly ConsoleKeyInfo TCapsuleEnter = new('\r', ConsoleKey.Enter, false, false, false);
    private static readonly ConsoleKeyInfo TCapsuleEscape = new('\e', ConsoleKey.Escape, false, false, false);
    private static readonly ConsoleKeyInfo TCapsuleExit = new('\0', ConsoleKey.F10, false, false, false);

    private static readonly IReadOnlyDictionary<string, IReadOnlyDictionary<string, IReadOnlyList<string>>>
        TCapsuleSets = new Dictionary<string, IReadOnlyDictionary<string, IReadOnlyList<string>>>
        {
            ["plain"] = new Dictionary<string, IReadOnlyList<string>>(),
            ["fancy"] = new Dictionary<string, IReadOnlyList<string>>(),
        };

    [Fact]
    public void CapsuleConsole_Save_ReadsBack()
    {
        string root = Directory.CreateTempSubdirectory("llyn-capsule-").FullName;
        try
        {
            LCapsuleConsole capsule = new();
            LCapsuleSetting state = new("dusk", "fancy", "wide", @"C:\Notes");

            capsule.LCapsuleConsoleSave(root, state);

            Assert.Equal(state, capsule.LCapsuleConsoleRead(root));
            Assert.False(File.Exists(Path.Combine(root, "console.json.tmp")));
        }
        finally
        {
            Directory.Delete(root, true);
        }
    }

    [Theory]
    [InlineData(null)]
    [InlineData("{ not json")]
    [InlineData("null")]
    public void CapsuleConsole_MissingOrCorrupt_ReadsDefault(string? text)
    {
        string root = Directory.CreateTempSubdirectory("llyn-capsule-").FullName;
        try
        {
            if (text is not null)
            {
                File.WriteAllText(Path.Combine(root, "console.json"), text);
            }

            Assert.Equal(new LCapsuleSetting(), new LCapsuleConsole().LCapsuleConsoleRead(root));
            Assert.Equal(new LCapsuleSetting(), new LCapsuleConsole().LCapsuleConsoleRead(null));
        }
        finally
        {
            Directory.Delete(root, true);
        }
    }

    [Fact]
    public void CapsuleWardrobe_UnknownNames_FallBack()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine, false);
        LCapsuleConsole capsule = new();
        LCapsuleSetting stored = new("lost", "gone", "huge");
        capsule.LCapsuleConsoleSave(atelier.CAtelierPathRead(), stored);
        BTheme spare = new(BTheme.BThemeSpareRead);

        BWardrobe wardrobe = new(
            (BTheme.BThemeDefault, spare), static _ => null, TCapsuleSets, "plain", capsule, atelier);

        Assert.Equal(BTheme.BThemeDefault, wardrobe.BWardrobeTheme.Item1);
        Assert.Equal("plain", wardrobe.BWardrobeGlyphName);
        Assert.Equal("narrow", wardrobe.BWardrobeAmbiguousName);
        Assert.Equal(stored, capsule.LCapsuleConsoleRead(atelier.CAtelierPathRead()));
    }

    [Fact]
    public void CapsuleWardrobe_Restart_KeepsChoices()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine, false);
        string root = atelier.CAtelierPathRead();
        LCapsuleConsole capsule = new();
        capsule.LCapsuleConsoleSave(root, new LCapsuleSetting(LCapsuleSettingFolder: root));
        BTheme spare = new(BTheme.BThemeSpareRead);
        Func<string, BTheme?> themes = name => name == "dusk" ? spare : null;
        BWardrobe first = new((BTheme.BThemeDefault, spare), themes, TCapsuleSets, "plain", capsule, atelier);

        Assert.True(first.BWardrobeThemeSelect("dusk"));
        Assert.True(first.BWardrobeGlyphSelect("fancy"));
        Assert.True(first.BWardrobeAmbiguousSelect("wide"));
        BWardrobe second = new(
            (BTheme.BThemeDefault, spare), themes, TCapsuleSets, "plain", new LCapsuleConsole(), atelier);

        Assert.Equal("dusk", second.BWardrobeTheme.Item1);
        Assert.Equal("fancy", second.BWardrobeGlyphName);
        Assert.Equal("wide", second.BWardrobeAmbiguousName);
        Assert.Equal(root, capsule.LCapsuleConsoleRead(root).LCapsuleSettingFolder);
    }

    [Fact]
    public void CapsulePathfinder_NoFolder_StartsInStored()
    {
        string stored = Directory.CreateTempSubdirectory("llyn-capsule-").FullName;
        try
        {
            TConsoleFake console = new() { TConsoleFakeTerminal = true };
            console.TConsoleFakeKeys.Add(TCapsuleEscape);
            console.TConsoleFakeKeys.Add(TCapsuleEnter);
            console.TConsoleFakeKeys.Add(TCapsuleExit);
            List<string> asked = [];
            string? theme = null;

            object? answer = TEnvoy.TEnvoyRun(
                console,
                envoy =>
                {
                    _ = envoy.CEnvoyFolderRead(string.Empty);
                    LCapsuleConsole capsule = new();
                    capsule.LCapsuleConsoleSave(asked[0], new LCapsuleSetting("dusk", LCapsuleSettingFolder: stored));
                    return envoy.CEnvoyFolderRead(string.Empty);
                },
                settled: engine => theme = new LCapsuleConsole()
                    .LCapsuleConsoleRead(engine.LEngineWorkspace.LEngineWorkspaceRead()).LCapsuleSettingTheme,
                folder: path =>
                {
                    asked.Add(path);
                    return ([], []);
                });

            Assert.Equal(stored.TrimEnd('\\'), asked[^1].TrimEnd('\\'));
            Assert.Equal(stored.TrimEnd('\\'), (answer as string)?.TrimEnd('\\'));
            Assert.Equal("dusk", theme);
        }
        finally
        {
            Directory.Delete(stored, true);
        }
    }

    [Fact]
    public void CapsulePathfinder_FileAccepted_NextStartsInItsFolder()
    {
        string parent = Directory.CreateTempSubdirectory("llyn-capsule-").FullName;
        try
        {
            string file = Path.Combine(parent, "notes.llx");
            File.WriteAllText(file, string.Empty);
            TConsoleFake console = new() { TConsoleFakeTerminal = true };
            foreach (ConsoleKeyInfo key in TPathfinder.TPathfinderTextBuild(file))
            {
                console.TConsoleFakeKeys.Add(key);
            }

            console.TConsoleFakeKeys.Add(TCapsuleEnter);
            console.TConsoleFakeKeys.Add(TCapsuleEscape);
            console.TConsoleFakeKeys.Add(TCapsuleExit);
            List<string> asked = [];

            object? answer = TEnvoy.TEnvoyRun(
                console,
                envoy =>
                {
                    string? opened = envoy.CEnvoyMarkupRead("Markup.Title", [".llx"]);
                    _ = envoy.CEnvoyFolderRead(string.Empty);
                    return opened;
                },
                folder: path =>
                {
                    asked.Add(path);
                    return ([], []);
                });

            Assert.Equal(file, answer);
            Assert.Equal(parent.TrimEnd('\\'), asked[^1].TrimEnd('\\'));
        }
        finally
        {
            Directory.Delete(parent, true);
        }
    }

    [Fact]
    public void CapsulePathfinder_Accepted_StoresFolder()
    {
        string given = Directory.CreateTempSubdirectory("llyn-capsule-").FullName;
        try
        {
            TConsoleFake console = new() { TConsoleFakeTerminal = true };
            console.TConsoleFakeKeys.Add(TCapsuleEnter);
            console.TConsoleFakeKeys.Add(TCapsuleExit);
            LCapsuleSetting? state = null;

            object? answer = TEnvoy.TEnvoyRun(
                console,
                envoy => envoy.CEnvoyFolderRead(given),
                settled: engine => state = new LCapsuleConsole()
                    .LCapsuleConsoleRead(engine.LEngineWorkspace.LEngineWorkspaceRead()),
                folder: static _ => ([], []));

            Assert.NotNull(answer);
            Assert.Equal(answer, state?.LCapsuleSettingFolder);
        }
        finally
        {
            Directory.Delete(given, true);
        }
    }
}
