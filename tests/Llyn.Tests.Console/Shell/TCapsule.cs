using Llyn.Conduct;
using Llyn.ShellEngine;
using Llyn.UIDemeanor;
using Llyn.UIDemeanor.Capsule;
using Llyn.UITerminal;
using Xunit;

namespace Llyn.Tests;

public sealed class TCapsule
{
    internal static readonly ConsoleKeyInfo TCapsuleEnter = new('\r', ConsoleKey.Enter, false, false, false);
    internal static readonly ConsoleKeyInfo TCapsuleEscape = new('\e', ConsoleKey.Escape, false, false, false);
    internal static readonly ConsoleKeyInfo TCapsuleExit = new('\0', ConsoleKey.F10, false, false, false);

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
    public void Wardrobe_UnknownNames_FallBack()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine, false);
        LCapsuleConsole capsule = new();
        LCapsuleSetting stored = new("lost", "gone", "huge");
        capsule.LCapsuleConsoleSave(atelier.CAtelierPathRead(), stored);
        BTheme spare = new(BTheme.BThemeSpareRead);

        BWardrobe wardrobe = new(
            (BTheme.BThemeDefault, spare), static _ => null, static _ => { }, TCapsuleSets, "plain", capsule, atelier);

        Assert.Equal(BTheme.BThemeDefault, wardrobe.BWardrobeTheme.Item1);
        Assert.Equal("plain", wardrobe.BWardrobeGlyphName);
        Assert.Equal("narrow", wardrobe.BWardrobeAmbiguousName);
        Assert.Equal(stored, capsule.LCapsuleConsoleRead(atelier.CAtelierPathRead()));
    }

    [Fact]
    public void WardrobeThemeSelect_ChosenThemeFails_Reports()
    {
        List<string> reported = [];
        BTheme spare = new(BTheme.BThemeSpareRead);
        BWardrobe wardrobe = new(
            (BTheme.BThemeDefault, spare), static _ => throw new FileNotFoundException("gone"), reported.Add,
            TCapsuleSets, "plain", new LCapsuleConsole(), null);

        Assert.False(wardrobe.BWardrobeThemeSelect("console-dark"));
        Assert.Equal(["gone"], reported);
        Assert.Equal(BTheme.BThemeDefault, wardrobe.BWardrobeTheme.Item1);
    }

    [Fact]
    public void Wardrobe_Restart_KeepsChoices()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine, false);
        string root = atelier.CAtelierPathRead();
        LCapsuleConsole capsule = new();
        capsule.LCapsuleConsoleSave(root, new LCapsuleSetting(LCapsuleSettingFolder: root));
        BTheme spare = new(BTheme.BThemeSpareRead);
        Func<string, BTheme?> themes = name => name == "dusk" ? spare : null;
        BWardrobe first = new(
            (BTheme.BThemeDefault, spare), themes, static _ => { }, TCapsuleSets, "plain", capsule, atelier);

        Assert.True(first.BWardrobeThemeSelect("dusk"));
        Assert.True(first.BWardrobeGlyphSelect("fancy"));
        Assert.True(first.BWardrobeAmbiguousSelect("wide"));
        BWardrobe second = new(
            (BTheme.BThemeDefault, spare), themes, static _ => { }, TCapsuleSets, "plain", new LCapsuleConsole(),
            atelier);

        Assert.Equal("dusk", second.BWardrobeTheme.Item1);
        Assert.Equal("fancy", second.BWardrobeGlyphName);
        Assert.Equal("wide", second.BWardrobeAmbiguousName);
        Assert.Equal(root, capsule.LCapsuleConsoleRead(root).LCapsuleSettingFolder);
    }

    [Fact]
    public void CapsuleWindow_WorkspaceChange_AdoptsItsLook()
    {
        using TWorkspace first = TWorkspace.TWorkspacePrepare();
        using TWorkspace second = TWorkspace.TWorkspacePrepare();
        LCapsuleSetting stored = new(LCapsuleSettingGlyph: "ascii");
        new LCapsuleConsole().LCapsuleConsoleSave(second.TWorkspaceFolder, stored);
        BTheme spare = new(BTheme.BThemeSpareRead);

        TConsoleFake console = TCapsuleChangeRun(first, second, _ => spare);

        Assert.Contains(
            console.TConsoleFakeShots, static grid => string.Join('\n', grid).Contains($"+{new string('-', 25)} Llyn"));
        Assert.Equal(stored, new LCapsuleConsole().LCapsuleConsoleRead(second.TWorkspaceFolder));
        Assert.False(File.Exists(Path.Combine(first.TWorkspaceFolder, "console.json")));
    }

    [Fact]
    public void CapsuleWindow_WorkspaceChangeMissingValue_ResetsToDefault()
    {
        using TWorkspace first = TWorkspace.TWorkspacePrepare();
        using TWorkspace second = TWorkspace.TWorkspacePrepare();
        LCapsuleConsole capsule = new();
        capsule.LCapsuleConsoleSave(first.TWorkspaceFolder, new LCapsuleSetting("dusk", "fancy", "wide"));
        LCapsuleSetting stored = new(LCapsuleSettingFolder: second.TWorkspaceFolder);
        capsule.LCapsuleConsoleSave(second.TWorkspaceFolder, stored);
        BTheme spare = new(BTheme.BThemeSpareRead);
        BWardrobe wardrobe = new(
            (BTheme.BThemeDefault, spare), _ => spare, static _ => { }, TCapsuleSets, "plain", capsule, null);
        wardrobe.BWardrobeRootRefine(first.TWorkspaceFolder);
        Assert.Equal("dusk", wardrobe.BWardrobeTheme.Item1);

        wardrobe.BWardrobeRootRefine(second.TWorkspaceFolder);

        Assert.Equal(BTheme.BThemeDefault, wardrobe.BWardrobeTheme.Item1);
        Assert.Equal("plain", wardrobe.BWardrobeGlyphName);
        Assert.Equal("narrow", wardrobe.BWardrobeAmbiguousName);
        Assert.Equal(stored, capsule.LCapsuleConsoleRead(second.TWorkspaceFolder));
    }

    [Fact]
    public void CapsuleWindow_WorkspaceChangeStaleName_ResetsToDefault()
    {
        using TWorkspace first = TWorkspace.TWorkspacePrepare();
        using TWorkspace second = TWorkspace.TWorkspacePrepare();
        LCapsuleConsole capsule = new();
        capsule.LCapsuleConsoleSave(first.TWorkspaceFolder, new LCapsuleSetting("dusk", "fancy", "wide"));
        LCapsuleSetting stored = new("lost", "gone", "huge", second.TWorkspaceFolder);
        capsule.LCapsuleConsoleSave(second.TWorkspaceFolder, stored);
        BTheme spare = new(BTheme.BThemeSpareRead);
        BWardrobe wardrobe = new(
            (BTheme.BThemeDefault, spare), name => name == "dusk" ? spare : null, static _ => { }, TCapsuleSets,
            "plain", capsule, null);
        wardrobe.BWardrobeRootRefine(first.TWorkspaceFolder);
        Assert.Equal("dusk", wardrobe.BWardrobeTheme.Item1);

        wardrobe.BWardrobeRootRefine(second.TWorkspaceFolder);

        Assert.Equal(BTheme.BThemeDefault, wardrobe.BWardrobeTheme.Item1);
        Assert.Equal("plain", wardrobe.BWardrobeGlyphName);
        Assert.Equal("narrow", wardrobe.BWardrobeAmbiguousName);
        Assert.Equal(stored, capsule.LCapsuleConsoleRead(second.TWorkspaceFolder));
    }

    [Fact]
    public void CapsuleWindow_WorkspaceChangeStaleTheme_FallsBackQuietly()
    {
        using TWorkspace first = TWorkspace.TWorkspacePrepare();
        using TWorkspace second = TWorkspace.TWorkspacePrepare();
        LCapsuleSetting stored = new("console-gone");
        new LCapsuleConsole().LCapsuleConsoleSave(second.TWorkspaceFolder, stored);
        BTheme spare = new(BTheme.BThemeSpareRead);

        TConsoleFake console = TCapsuleChangeRun(
            first, second, name => name == "console-gone" ? throw new FileNotFoundException("gone") : spare);

        Assert.Contains(
            console.TConsoleFakeShots, static grid => string.Join('\n', grid).Contains("Delete this entry?"));
        Assert.DoesNotContain(
            console.TConsoleFakeShots, static grid => string.Join('\n', grid).Contains("could not be loaded"));
        Assert.Empty(console.TConsoleFakeError);
        Assert.Equal(stored, new LCapsuleConsole().LCapsuleConsoleRead(second.TWorkspaceFolder));
    }

    [Fact]
    public void InvocationRun_StaleTheme_FallsBackQuietly()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        new LCapsuleConsole().LCapsuleConsoleSave(workspace.TWorkspaceFolder, new LCapsuleSetting("console-gone"));
        TConsoleFake console = new() { TConsoleFakeTerminal = true };
        console.TConsoleFakeKeys.Add(TCapsuleExit);
        console.TConsoleFakeKeys.Add(TCapsuleEscape);
        console.TConsoleFakeKeys.Add(TCapsuleExit);

        int code = TInvocation.TInvocationRun(console, workspace);

        Assert.Equal(0, code);
        Assert.Empty(console.TConsoleFakeError);
        Assert.Equal("console-gone", new LCapsuleConsole()
            .LCapsuleConsoleRead(workspace.TWorkspaceFolder).LCapsuleSettingTheme);
    }

    [Fact]
    public void CapsuleConsole_ReadOnlyFile_LeavesNoPending()
    {
        string root = Directory.CreateTempSubdirectory("llyn-capsule-").FullName;
        string path = Path.Combine(root, "console.json");
        try
        {
            LCapsuleConsole capsule = new();
            capsule.LCapsuleConsoleSave(root, new LCapsuleSetting("dusk"));
            File.SetAttributes(path, FileAttributes.ReadOnly);

            Exception? raised = Record.Exception(
                () => capsule.LCapsuleConsoleSave(root, new LCapsuleSetting("dark")));
            Assert.True(raised is UnauthorizedAccessException or IOException, raised?.ToString());

            Assert.Equal("dusk", capsule.LCapsuleConsoleRead(root).LCapsuleSettingTheme);
            Assert.False(File.Exists(path + ".tmp"));
        }
        finally
        {
            File.SetAttributes(path, FileAttributes.Normal);
            Directory.Delete(root, true);
        }
    }

    private static TConsoleFake TCapsuleChangeRun(TWorkspace first, TWorkspace second, Func<string, BTheme?> themes)
    {
        using LEngine engine = new(
            first.TWorkspaceRigCreate(), _ => second.TWorkspaceRigCreate(), static _ => { }, true);
        TConsoleFake console = new() { TConsoleFakeTerminal = true };
        console.TConsoleFakeKeys.Add(TCapsuleEnter);
        console.TConsoleFakeKeys.Add(TCapsuleExit);
        console.TConsoleFakeKeys.Add(TCapsuleEscape);
        console.TConsoleFakeKeys.Add(TCapsuleExit);
        console.TConsoleFakeKeys.Add(TCapsuleEscape);
        console.TConsoleFakeKeys.Add(TCapsuleExit);
        SynchronizationContext? context = SynchronizationContext.Current;
        BLoop loop = new(static _ => { });
        SynchronizationContext.SetSynchronizationContext(loop);
        try
        {
            BLocalizationCatalog catalog = new();
            catalog.BLocalizationCatalogApply(TInterface.TLocalizationLoad("en"));
            ABootstrap bootstrap = new();
            BEnvoy envoy = new(console, catalog, bootstrap);
            using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine, false);
            LCapsuleConsole capsule = new();
            BWindow window = new(
                console,
                loop,
                catalog,
                atelier,
                envoy,
                bootstrap,
                (BTheme.BThemeDefault, new BTheme(BTheme.BThemeSpareRead)),
                themes,
                capsule);
            envoy.BEnvoyWindowAttach(
                window, atelier, File.Exists, static _ => ([], []), static () => [], capsule);
            loop.BLoopDefer(window.BWindowOpen);
            loop.BLoopDefer(window.BWindowAtelierOpen);
            loop.BLoopDefer(() =>
            {
                _ = atelier.CAtelierWorkspace.CWorkspaceChange(second.TWorkspaceFolder, envoy);
                _ = envoy.CEnvoyConfirm("Scribe.DeleteConfirm");
            });
            loop.BLoopBatchAttach(window.BWindowBatchObserve);
            loop.BLoopKeyAttach(console, window.BWindowKeyObserve);
            loop.BLoopRun(window.BWindowExit.WaitAsync(TimeSpan.FromSeconds(30)));
            Assert.True(window.BWindowExit.IsCompleted);
        }
        finally
        {
            SynchronizationContext.SetSynchronizationContext(context);
        }

        return console;
    }
}
