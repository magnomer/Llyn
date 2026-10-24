using Llyn.Conduct;
using Llyn.ShellEngine;
using Llyn.UIDemeanor;
using Llyn.UIDemeanor.Capsule;
using Llyn.UITerminal;
using Xunit;

namespace Llyn.Tests;

public sealed class TLibraryRail
{
    [Fact]
    public void LibraryKeymap_RailKeys_LiveAsTheGuiRail()
    {
        foreach (bool visible in (bool[])[true, false])
        {
            TLibrary.TLibraryRun(
                ["fire", "stone"],
                visible,
                (library, _, _) =>
                {
                    Func<BKeymapLive[]> live = () =>
                    [
                        .. ((ConsoleKey[])[ConsoleKey.F7, ConsoleKey.F2, ConsoleKey.F4, ConsoleKey.F8]).Select(
                            key => library.BLibraryKeymap.BKeymapFind(
                                BKeymapFocus.BKeymapFocusWindow,
                                new ConsoleKeyInfo('\0', key, false, false, false))?.BKeymapRowLive()
                                ?? BKeymapLive.BKeymapLiveHidden),
                    ];
                    BKeymapLive dim = visible ? BKeymapLive.BKeymapLiveDim : BKeymapLive.BKeymapLiveHidden;
                    BKeymapLive shown = visible ? BKeymapLive.BKeymapLiveShown : BKeymapLive.BKeymapLiveHidden;
                    Assert.Equal([shown, dim, dim, dim], live());

                    library.BLibraryObserve(TLibrary.TLibraryEnter);

                    Assert.Equal(visible ? [shown, dim, shown, shown] : [shown, dim, dim, dim], live());
                });
        }
    }

    [Fact]
    public void LibraryKeymap_F8_AsksBeforeDeleting()
    {
        List<string> asked = [];
        TLibrary.TLibraryRun(
            ["fire", "stone"],
            true,
            (library, _, _) =>
            {
                ConsoleKeyInfo key = new('\0', ConsoleKey.F8, false, false, false);
                BKeymapRow row = library.BLibraryKeymap.BKeymapFind(BKeymapFocus.BKeymapFocusWindow, key)!;
                Assert.Equal(BKeymapLive.BKeymapLiveDim, row.BKeymapRowLive());

                library.BLibraryObserve(TLibrary.TLibraryEnter);
                Assert.Equal(BKeymapLive.BKeymapLiveShown, row.BKeymapRowLive());
                row.BKeymapRowAction(key);

                Assert.Equal(["Scribe.DeleteConfirm"], asked);
                Assert.Contains("fire", TLibrary.TLibraryDraw(library));
            },
            asked);
    }

    [Fact]
    public void LibraryKeymap_F7ThenF4_OpensAndClosesTheFreshDraft()
    {
        TLibrary.TLibraryRun(
            ["fire"],
            true,
            (library, _, _) =>
            {
                Func<ConsoleKey, BKeymapRow> find = pressed => library.BLibraryKeymap.BKeymapFind(
                    BKeymapFocus.BKeymapFocusWindow, new ConsoleKeyInfo('\0', pressed, false, false, false))!;
                ConsoleKeyInfo f4 = new('\0', ConsoleKey.F4, false, false, false);
                Assert.Contains("Select an entry to read it", TLibrary.TLibraryDraw(library));

                find(ConsoleKey.F7).BKeymapRowAction(new ConsoleKeyInfo('\0', ConsoleKey.F7, false, false, false));

                Assert.DoesNotContain("Select an entry to read it", TLibrary.TLibraryDraw(library));
                Assert.Equal(BKeymapLive.BKeymapLiveShown, find(ConsoleKey.F4).BKeymapRowLive());

                find(ConsoleKey.F4).BKeymapRowAction(f4);

                Assert.Contains("Select an entry to read it", TLibrary.TLibraryDraw(library));
                Assert.Equal(BKeymapLive.BKeymapLiveDim, find(ConsoleKey.F4).BKeymapRowLive());
            });
    }

    [Fact]
    public void LibraryDraw_QueryMatchesNothing_ShowsUnmatchedNotice()
    {
        TLibrary.TLibraryRun(
            ["fire", "stone"],
            true,
            (library, _, _) =>
            {
                library.BLibraryObserve(TLibrary.TLibraryEnter);

                library.BLibraryObserve(new ConsoleKeyInfo('q', ConsoleKey.Q, false, false, false));
                BLoop loop = Assert.IsType<BLoop>(SynchronizationContext.Current);
                TaskCompletionSource settled = new();
                loop.BLoopDefer(settled.SetResult);
                loop.BLoopRun(settled.Task.WaitAsync(TimeSpan.FromSeconds(10)));

                string drawn = TLibrary.TLibraryDraw(library);
                Assert.Contains("No entries match", drawn);
                Assert.DoesNotContain("No entries yet", drawn);
                Assert.DoesNotContain("Select an entry to read it", drawn);
            });
    }

    [Fact]
    public void LibraryDraw_WorkspaceEmpty_CentresTheNotice()
    {
        TLibrary.TLibraryRun(
            [],
            true,
            (library, _, _) =>
            {
                BBlueprintPart window = BContract.BContractBlueprintFind(new ABootstrap(), "AWindow");
                BBlueprint.BBlueprintPlace(window, 0, 0, 80, 24);
                BBlueprintPart index = BContract.BContractFind(window, "AIndex");
                const string notice = "No entries yet";

                BCanvasText text = Assert.Single(
                    library.BLibraryDraw(
                        new Dictionary<string, IReadOnlyList<string>>(), window, true, new BCanvasWidth(false)),
                    static text => text.BCanvasTextValue == notice);

                int inner = index.BBlueprintPartWidth - 2;
                Assert.Equal(index.BBlueprintPartColumn + 1 + ((inner - notice.Length) / 2), text.BCanvasTextColumn);
                Assert.Equal(
                    index.BBlueprintPartRow + 1 + ((index.BBlueprintPartHeight - 5) / 2), text.BCanvasTextRow);
            });
    }

    [Fact]
    public void WindowKeyObserve_OtherTabShown_KeepsKeysFromLibrary()
    {
        TLibraryWindowRun(
            ["fire", "stone"],
            (window, atelier, _) =>
            {
                List<CNavigationState> states = [];
                Assert.True(atelier.CAtelierNavigation.CNavigationTabSelect("Input"));
                atelier.CAtelierNavigation.CNavigationChanged += states.Add;

                window.BWindowKeyObserve(TLibrary.TLibraryDown);
                window.BWindowKeyObserve(TLibrary.TLibraryEnter);

                Assert.Empty(states);

                Assert.True(atelier.CAtelierNavigation.CNavigationTabSelect("Library"));
                window.BWindowKeyObserve(TLibrary.TLibraryDown);
                window.BWindowKeyObserve(TLibrary.TLibraryEnter);

                Assert.Equal(2, states.Count);
                Assert.Equal("Library", states[0].CNavigationStateTab);
            });
    }

    [Fact]
    public void WindowKeyObserve_DimRow_SwallowsTheKey()
    {
        TLibraryWindowRun(
            ["fire"],
            (window, atelier, console) =>
            {
                Assert.True(atelier.CAtelierNavigation.CNavigationTabSelect("Library"));
                window.BWindowBatchObserve();
                int frames = console.TConsoleFakeShots.Count;

                window.BWindowKeyObserve(new ConsoleKeyInfo('\0', ConsoleKey.F8, false, false, false));
                window.BWindowBatchObserve();

                Assert.Equal(frames, console.TConsoleFakeShots.Count);

                window.BWindowKeyObserve(new ConsoleKeyInfo('\0', ConsoleKey.F7, false, false, false));
                window.BWindowBatchObserve();

                Assert.Equal(frames + 1, console.TConsoleFakeShots.Count);
            });
    }

    private static void TLibraryWindowRun(
        IReadOnlyList<string> headwords, Action<BWindow, CAtelier, TConsoleFake> check)
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        foreach (string headword in headwords)
        {
            engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
                headword, "English", string.Empty, string.Empty, [TInterface.TCardCreate("a meaning", 1)], []));
        }

        SynchronizationContext? context = SynchronizationContext.Current;
        BLoop loop = new(static _ => { });
        SynchronizationContext.SetSynchronizationContext(loop);
        try
        {
            TConsoleFake console = new() { TConsoleFakeTerminal = true };
            BLocalizationCatalog catalog = new();
            catalog.BLocalizationCatalogApply(TInterface.TLocalizationLoad("en"));
            ABootstrap bootstrap = new();
            using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine, false);
            BTheme spare = new(BTheme.BThemeSpareRead);
            BWindow window = new(
                console,
                loop,
                catalog,
                atelier,
                new BEnvoy(console, catalog, bootstrap),
                bootstrap,
                (BTheme.BThemeDefault, spare),
                _ => spare,
                new LCapsuleConsole());
            window.BWindowOpen();
            window.BWindowAtelierOpen();
            check(window, atelier, console);
        }
        finally
        {
            SynchronizationContext.SetSynchronizationContext(context);
        }
    }
}
