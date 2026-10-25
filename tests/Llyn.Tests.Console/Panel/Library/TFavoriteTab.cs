using Llyn.Conduct;
using Llyn.ShellEngine;
using Llyn.UIDemeanor;
using Llyn.UITerminal;
using Xunit;

namespace Llyn.Tests;

public sealed class TFavoriteTab
{
    private static readonly ConsoleKey[] TFavoriteTabGrasp =
        [.. Enumerable.Repeat(ConsoleKey.DownArrow, 4), ConsoleKey.Enter];

    [Fact]
    public void FavoriteKeymap_ControlFunctionKeys_PickTheFiveOrderings()
    {
        TFavoriteTabRun(
            [("apple", "English", 0), ("fire", "French", 0)],
            (library, send, parleys) =>
            {
                string[] words = ["Headword A → Z", "Headword Z → A", "Language A → Z", "Recently marked"];
                foreach ((ConsoleKey pressed, string word) in words.Select(
                    static (word, index) => (ConsoleKey.F3 + index, word)))
                {
                    send(new ConsoleKeyInfo('\0', pressed, false, false, true));
                    Assert.Contains(word, TLibrary.TLibraryDraw(library));
                }

                send(new ConsoleKeyInfo('\0', ConsoleKey.F7, false, false, true));
                BSParley chooser = Assert.Single(parleys);
                foreach (ConsoleKey pressed in TFavoriteTabGrasp)
                {
                    chooser.BSParleyKeyObserve(new ConsoleKeyInfo('\0', pressed, false, false, false));
                }

                Assert.Contains("Grasp high → low", TLibrary.TLibraryDraw(library));
            });
    }

    [Fact]
    public void FavoriteKeymap_NoNew_LeavesFunctionSevenBlankAndOffersExportAlone()
    {
        TFavoriteTabRun(
            [("apple", "English", 0)],
            (library, _, _) =>
            {
                Assert.Null(library.BLibraryKeymap.BKeymapFind(
                    BKeymapFocus.BKeymapFocusWindow, new ConsoleKeyInfo('\0', ConsoleKey.F7, false, false, false)));
                IReadOnlyList<BRoofMenu> menus = library.BLibraryMenuBuild();
                Assert.Equal(["Export…"], menus[0].BRoofMenuItems.Select(static item => item!.BRoofItemLabel));
                Assert.Contains("Search favorites", TLibrary.TLibraryDraw(library));
            });
    }

    [Fact]
    public void FavoriteKeymap_FunctionSixOnTheChosenRow_UnmarksItAndLightsTheRowInItsPlace()
    {
        TFavoriteTabRun(
            [("apple", "English", 0), ("fire", "English", 0), ("stone", "English", 0)],
            (library, send, _) =>
            {
                ConsoleKeyInfo six = new('\0', ConsoleKey.F6, false, false, false);
                send(TLibrary.TLibraryDown);
                send(TLibrary.TLibraryEnter);

                library.BLibraryKeymap.BKeymapFind(BKeymapFocus.BKeymapFocusWindow, six)!.BKeymapRowAction(six);

                string drawn = TLibrary.TLibraryDraw(library);
                Assert.Contains("2 of 2", drawn);
                Assert.Contains("fire", drawn);
                Assert.Equal(1, drawn.Split('\n').Count(static line => line == "fire"));

                library.BLibraryKeymap.BKeymapFind(BKeymapFocus.BKeymapFocusWindow, six)!.BKeymapRowAction(six);

                Assert.Contains("3 of 3", TLibrary.TLibraryDraw(library));
            });
    }

    [Fact]
    public void FavoriteKeymap_FunctionSixOnTheLastRow_LightsTheRowAbove()
    {
        TFavoriteTabRun(
            [("apple", "English", 0), ("fire", "English", 0), ("stone", "English", 0)],
            (library, send, _) =>
            {
                ConsoleKeyInfo six = new('\0', ConsoleKey.F6, false, false, false);
                send(new ConsoleKeyInfo('\0', ConsoleKey.End, false, false, false));
                send(TLibrary.TLibraryEnter);

                library.BLibraryKeymap.BKeymapFind(BKeymapFocus.BKeymapFocusWindow, six)!.BKeymapRowAction(six);

                Assert.Contains("2 of 2", TLibrary.TLibraryDraw(library));
            });
    }

    [Fact]
    public void FavoriteLectern_GraspRaisedUnderTheGraspOrder_ResortsAndTheLitRowFollows()
    {
        TFavoriteTabRun(
            [("apple", "English", 2), ("fire", "English", 1), ("stone", "English", 0)],
            (library, send, parleys) =>
            {
                send(new ConsoleKeyInfo('\0', ConsoleKey.F7, false, false, true));
                foreach (ConsoleKey pressed in TFavoriteTabGrasp)
                {
                    parleys[0].BSParleyKeyObserve(new ConsoleKeyInfo('\0', pressed, false, false, false));
                }

                send(new ConsoleKeyInfo('\0', ConsoleKey.End, false, false, false));
                send(TLibrary.TLibraryEnter);
                Assert.Contains("3 of 3", TLibrary.TLibraryDraw(library));

                foreach (ConsoleKey pressed in (ConsoleKey[])[ConsoleKey.DownArrow, ConsoleKey.DownArrow,
                    ConsoleKey.RightArrow, ConsoleKey.RightArrow, ConsoleKey.RightArrow, ConsoleKey.RightArrow])
                {
                    send(new ConsoleKeyInfo('\0', pressed, false, false, false));
                }

                Assert.Contains("1 of 3", TLibrary.TLibraryDraw(library));
            });
    }

    [Fact]
    public void FavoriteDraw_NoFavorites_ShowsTheEmptyNotice()
    {
        TFavoriteTabRun(
            [],
            (library, _, _) =>
            {
                string drawn = TLibrary.TLibraryDraw(library);
                Assert.Contains("No favorites yet", drawn);
                Assert.Contains("Favorite", drawn);
            });
    }

    private static void TFavoriteTabRun(
        IReadOnlyList<(string, string, int)> favorites,
        Action<BLibrary, Action<ConsoleKeyInfo>, List<BSParley>> check)
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        foreach ((string headword, string language, int grasp) in favorites)
        {
            long id = engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
                headword, language, string.Empty, string.Empty, [TInterface.TCardCreate("a meaning", 1)], [])).LEntryId;
            engine.TEngineFavoriteSave(id);
            engine.TEngineGraspSave(id, grasp);
        }

        SynchronizationContext? context = SynchronizationContext.Current;
        BLoop loop = new(static _ => { });
        SynchronizationContext.SetSynchronizationContext(loop);
        try
        {
            using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
            BLocalizationCatalog catalog = new();
            catalog.BLocalizationCatalogApply(TInterface.TLocalizationLoad("en"));
            List<BSParley> parleys = [];
            CEnvoy answer = TEnvoyFake.TEnvoyCreate(false, []);
            BLibrary library = new(
                catalog,
                atelier,
                answer,
                BContract.BContractBlueprintFind(new ABootstrap(), "ASParley"),
                (parley, _) =>
                {
                    parleys.Add(parley);
                    return null;
                },
                static () => true,
                static () => { },
                static () => new BCanvasWidth(false),
                CFavorite.CFavoriteCreate);
            atelier.CAtelierOpen(answer, BObserver.BObserverCreate<Action>(static work => work()));
            Action<ConsoleKeyInfo> send = key =>
            {
                BKeymapRow? row = library.BLibraryKeymap.BKeymapFind(library.BLibraryKeymapFocus, key);
                if (row is null)
                {
                    library.BLibraryObserve(key);
                }
                else
                {
                    row.BKeymapRowAction(key);
                }
            };
            check(library, send, parleys);
        }
        finally
        {
            SynchronizationContext.SetSynchronizationContext(context);
        }
    }
}
