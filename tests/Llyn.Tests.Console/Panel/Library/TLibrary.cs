using Llyn.Conduct;
using Llyn.ShellEngine;
using Llyn.UIDemeanor;
using Llyn.UITerminal;
using Xunit;

namespace Llyn.Tests;

public sealed class TLibrary
{
    internal static readonly ConsoleKeyInfo TLibraryDown = new('\0', ConsoleKey.DownArrow, false, false, false);
    internal static readonly ConsoleKeyInfo TLibraryEnter = new('\r', ConsoleKey.Enter, false, false, false);

    [Fact]
    public void LibraryObserve_ArrowsThenEnter_SelectsOnceWithOneStation()
    {
        TLibraryRun(
            ["fire", "stone", "water"],
            true,
            (library, atelier, _) =>
            {
                List<CNavigationState> states = [];
                atelier.CAtelierNavigation.CNavigationChanged += states.Add;

                library.BLibraryObserve(TLibraryDown);
                library.BLibraryObserve(TLibraryDown);
                Assert.Empty(states);
                Assert.Contains("Select an entry to read it", TLibraryDraw(library));

                library.BLibraryObserve(TLibraryEnter);

                Assert.Single(states);
                Assert.DoesNotContain("Select an entry to read it", TLibraryDraw(library));
            });
    }

    [Fact]
    public void LibraryObserve_EnterOnLitRow_SelectsWithoutRest()
    {
        TLibraryRun(
            ["fire", "stone"],
            true,
            (library, atelier, _) =>
            {
                List<CNavigationState> states = [];
                atelier.CAtelierNavigation.CNavigationChanged += states.Add;

                library.BLibraryObserve(TLibraryDown);
                library.BLibraryObserve(TLibraryEnter);

                Assert.Single(states);
            });
    }

    [Fact]
    public void LibraryKeymap_ControlF4_OrdersHeadwordReverse()
    {
        TLibraryRun(
            ["fire"],
            true,
            (library, _, _) =>
            {
                ConsoleKeyInfo key = new('\0', ConsoleKey.F4, false, false, true);
                BKeymapRow row = Assert.IsType<BKeymapRow>(
                    library.BLibraryKeymap.BKeymapFind(BKeymapFocus.BKeymapFocusWindow, key));

                row.BKeymapRowAction(key);

                Assert.Contains("Headword Z → A", TLibraryDraw(library));
            });
    }

    [Fact]
    public void LibraryKeymap_ControlF7AndF8_ShowTheChoosers()
    {
        TLibraryRun(
            ["fire"],
            true,
            (library, _, shown) =>
            {
                foreach (ConsoleKey pressed in (ConsoleKey[])[ConsoleKey.F7, ConsoleKey.F8])
                {
                    ConsoleKeyInfo key = new('\0', pressed, false, false, true);
                    library.BLibraryKeymap.BKeymapFind(BKeymapFocus.BKeymapFocusWindow, key)!.BKeymapRowAction(key);
                }

                Assert.Equal([true, false], shown.Select(static until => until is not null));
            });
    }

    [Fact]
    public void LibraryMenuBuild_Shown_BuildsEntryAndView()
    {
        TLibraryRun(
            ["fire"],
            true,
            (library, _, _) =>
            {
                IReadOnlyList<BRoofMenu> menus = library.BLibraryMenuBuild();

                Assert.Equal(["Entry", "View"], menus.Select(static menu => menu.BRoofMenuTitle));
                Assert.Equal([ConsoleKey.E, ConsoleKey.V], menus.Select(static menu => menu.BRoofMenuHotkey));
                Assert.Equal(
                    ["Import markup…", "Export…"], menus[0].BRoofMenuItems.Select(static item => item!.BRoofItemLabel));
                Assert.Equal(2, menus[1].BRoofMenuItems.Count);
            });
    }

    [Fact]
    public void LibraryObserve_OtherTabShown_IgnoresKeysAndDrawsNothing()
    {
        TLibraryRun(
            ["fire", "stone"],
            false,
            (library, atelier, _) =>
            {
                List<CNavigationState> states = [];
                atelier.CAtelierNavigation.CNavigationChanged += states.Add;

                library.BLibraryObserve(TLibraryDown);

                Assert.Empty(states);
                Assert.Empty(TLibraryDraw(library));
                Assert.Empty(library.BLibraryMenuBuild());
            });
    }

    [Fact]
    public void LibraryObserve_EveryEnter_RecordsOneStation()
    {
        TLibraryRun(
            ["fire", "stone", "water"],
            true,
            (library, atelier, _) =>
            {
                List<CNavigationState> states = [];
                atelier.CAtelierNavigation.CNavigationChanged += states.Add;

                library.BLibraryObserve(TLibraryEnter);
                library.BLibraryObserve(TLibraryDown);
                library.BLibraryObserve(TLibraryDown);
                library.BLibraryObserve(TLibraryEnter);
                library.BLibraryObserve(TLibraryEnter);
                int entered = states.Count;
                library.BLibraryObserve(TLibraryDown);
                library.BLibraryObserve(new ConsoleKeyInfo('\0', ConsoleKey.UpArrow, false, false, false));

                Assert.Equal(3, entered);
                Assert.Equal(3, states.Count);
            });
    }

    [Fact]
    public void LibraryObserve_SelectFails_KeepsTheLitRow()
    {
        List<string> asked = [];
        TLibraryRun(
            ["fire", "stone", "water"],
            true,
            (library, _, _) =>
            {
                library.BLibraryObserve(TLibraryDown);
                library.BLibraryObserve(TLibraryDown);

                library.BLibraryObserve(TLibraryEnter);

                Assert.Equal(["List.LoadFailed"], asked);
                Assert.Contains("3 of 3", TLibraryDraw(library));
                Assert.Contains("Select an entry to read it", TLibraryDraw(library));
            },
            asked,
            "ALTER TABLE entry RENAME TO entry_gone;");
    }

    [Fact]
    public void LibraryDraw_ChosenEntryFailsToLoad_ShowsThePrompt()
    {
        TLibraryRun(
            ["fire", "stone"],
            true,
            (library, atelier, _) =>
            {
                Assert.Contains("Select an entry to read it", TLibraryDraw(library));

                Assert.True(atelier.CAtelierNavigation.CNavigationEntryOpen(0));

                BKeymapRow bin = library.BLibraryKeymap.BKeymapFind(
                    BKeymapFocus.BKeymapFocusWindow, new ConsoleKeyInfo('\0', ConsoleKey.F8, false, false, false))!;
                Assert.Equal(BKeymapLive.BKeymapLiveShown, bin.BKeymapRowLive());
                Assert.Contains("Select an entry to read it", TLibraryDraw(library));
            });
    }

    internal static void TLibraryRun(
        IReadOnlyList<string> headwords,
        bool visible,
        Action<BLibrary, CAtelier, List<Task?>> check,
        List<string>? asked = null,
        string? script = null,
        CEnvoy? envoy = null,
        Action<LEngine>? seed = null)
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        foreach (string headword in headwords)
        {
            engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
                headword, "English", string.Empty, string.Empty, [TInterface.TCardCreate("a meaning", 1)], []));
        }

        seed?.Invoke(engine);

        SynchronizationContext? context = SynchronizationContext.Current;
        BLoop loop = new(static _ => { });
        SynchronizationContext.SetSynchronizationContext(loop);
        try
        {
            using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
            BLocalizationCatalog catalog = new();
            catalog.BLocalizationCatalogApply(TInterface.TLocalizationLoad("en"));
            ABootstrap bootstrap = new();
            List<Task?> shown = [];
            CEnvoy answer = envoy ?? TEnvoyFake.TEnvoyCreate(false, asked ?? []);
            BLibrary library = new(
                catalog,
                atelier,
                answer,
                BContract.BContractBlueprintFind(bootstrap, "ASParley"),
                (_, until) =>
                {
                    shown.Add(until);
                    return null;
                },
                () => visible,
                static () => { },
                static () => new BCanvasWidth(false));
            atelier.CAtelierOpen(answer, BObserver.BObserverCreate<Action>(static work => work()));
            if (script is not null)
            {
                workspace.TWorkspaceScriptRun(script);
            }

            check(library, atelier, shown);
        }
        finally
        {
            SynchronizationContext.SetSynchronizationContext(context);
        }
    }

    internal static string TLibraryDraw(BLibrary library)
    {
        BBlueprintPart window = BContract.BContractBlueprintFind(new ABootstrap(), "AWindow");
        BBlueprint.BBlueprintPlace(window, 0, 0, 80, 24);
        return string.Join(
            "\n",
            library.BLibraryDraw(new Dictionary<string, IReadOnlyList<string>>(), window, true, new BCanvasWidth(false))
                .Select(static text => text.BCanvasTextValue));
    }
}
