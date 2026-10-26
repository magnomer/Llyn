using Llyn.Conduct;
using Llyn.ShellEngine;
using Llyn.UIDemeanor;
using Llyn.UIDemeanor.Capsule;
using Llyn.UITerminal;
using Xunit;

namespace Llyn.Tests;

public sealed class TEditorFrame
{
    [Fact]
    public void LibraryDraw_EditingAnEntry_TitlesTheRightSideWithEditingAndTheHeadword()
    {
        TEditorFrameRun(
            ["fire"],
            (library, find) =>
            {
                string drawn = string.Join(
                    "\n", TEditorFrameDraw(library).Select(static text => text.BCanvasTextValue));

                Assert.Equal(BKeymapFocus.BKeymapFocusEditor, library.BLibraryKeymapFocus);
                Assert.Contains(" Editing fire ", drawn);
                Assert.Contains("fire", drawn);
                Assert.Contains("[Meaning 1]", drawn);
                Assert.Contains(" Collocation 1 ", drawn);
                Assert.Contains("-- 1 - Meaning ", drawn);
                Assert.DoesNotContain("Select an entry to read it", drawn);
            });
    }

    [Fact]
    public void EditorKeymap_EnterOnTheHeadword_OpensTheFieldAndTypingReachesTheDraft()
    {
        TEditorFrameRun(
            ["fire"],
            (library, find) =>
            {
                TEditorFrameSend(library, TLibrary.TLibraryEnter);
                Assert.Equal("Editor.Close", find(ConsoleKey.Escape, 0)?.BKeymapRowLabel);

                TEditorFrameSend(library, new ConsoleKeyInfo('s', ConsoleKey.S, false, false, false));

                Assert.Contains(TEditorFrameDraw(library), static text => text.BCanvasTextValue.StartsWith("fires"));

                TEditorFrameSend(library, new ConsoleKeyInfo('\u001b', ConsoleKey.Escape, false, false, false));

                string drawn = string.Join(
                    "\n", TEditorFrameDraw(library).Select(static text => text.BCanvasTextValue));
                Assert.Contains(" Editing fires ", drawn);
                Assert.Contains("Unsaved", drawn);
                Assert.Equal("Editor.Open", find(ConsoleKey.Enter, 0)?.BKeymapRowLabel);
                Assert.Null(find(ConsoleKey.Escape, 0));
            });
    }

    [Fact]
    public void EditorObserve_PrintableKeyOnTheLitHeadword_OpensTheFieldAndTypesTheKey()
    {
        TEditorFrameRun(
            ["fire"],
            (library, find) =>
            {
                TEditorFrameSend(library, new ConsoleKeyInfo('x', ConsoleKey.X, false, false, false));

                Assert.Contains(TEditorFrameDraw(library), static text => text.BCanvasTextValue.StartsWith("firex"));
                Assert.Equal("Editor.Close", find(ConsoleKey.Enter, 0)?.BKeymapRowLabel);
            });
    }

    [Fact]
    public void EditorKeymap_ArrowsOnTheTabLine_ChooseTheStackTab()
    {
        TEditorFrameRun(
            ["fire"],
            (library, find) =>
            {
                Assert.Null(find(ConsoleKey.RightArrow, 0));

                TEditorFrameSend(library, TLibrary.TLibraryDown);
                TEditorFrameSend(library, new ConsoleKeyInfo('\0', ConsoleKey.RightArrow, false, false, false));

                string drawn = string.Join(
                    "\n", TEditorFrameDraw(library).Select(static text => text.BCanvasTextValue));
                Assert.Contains("[Collocation 1]", drawn);
                Assert.DoesNotContain("-- 1 - Meaning ", drawn);
            });
    }

    [Fact]
    public void EditorKeymap_AltDigit_ChoosesThatStackTab()
    {
        TEditorFrameRun(
            ["fire"],
            (library, find) =>
            {
                TEditorFrameSend(library, new ConsoleKeyInfo('3', ConsoleKey.D3, false, true, false));

                Assert.Contains(
                    TEditorFrameDraw(library), static text => text.BCanvasTextValue.Contains("[Etymology]"));

                TEditorFrameSend(library, new ConsoleKeyInfo('1', ConsoleKey.D1, false, true, false));

                Assert.Contains(
                    TEditorFrameDraw(library), static text => text.BCanvasTextValue.Contains("[Meaning 1]"));
            });
    }

    [Fact]
    public void EditorKeymap_EnterOnAStoredCardHeader_FoldsItThroughConduct()
    {
        TEditorFrameRun(
            ["fire"],
            (library, find) =>
            {
                ConsoleKeyInfo next = new('\0', ConsoleKey.PageDown, false, false, true);
                TEditorFrameSend(library, next);
                Assert.Equal("Card.Fold", find(ConsoleKey.Enter, 0)?.BKeymapRowLabel);
                Assert.Contains(TEditorFrameDraw(library), static text => text.BCanvasTextValue.EndsWith(" v"));

                TEditorFrameSend(library, TLibrary.TLibraryEnter);

                Assert.Contains(TEditorFrameDraw(library), static text => text.BCanvasTextValue.EndsWith(" >"));
            });
    }

    [Fact]
    public void EditorKeymap_ControlPageKeys_LightOnlyCardHeaders()
    {
        TEditorFrameRun(
            ["fire"],
            (library, find) =>
            {
                Assert.Null(find(ConsoleKey.PageUp, ConsoleModifiers.Control));
                Assert.NotNull(find(ConsoleKey.PageDown, ConsoleModifiers.Control));

                TEditorFrameSend(library, new ConsoleKeyInfo('\0', ConsoleKey.PageDown, false, false, true));

                Assert.Null(find(ConsoleKey.PageDown, ConsoleModifiers.Control));
                Assert.Null(find(ConsoleKey.PageUp, ConsoleModifiers.Control));
                BCanvasText header = Assert.Single(
                    TEditorFrameDraw(library), static text => text.BCanvasTextValue.StartsWith("-- 1"));
                Assert.Equal("lit", header.BCanvasTextRole);
            });
    }

    [Fact]
    public void EditorKeymap_ControlHomeAndEnd_LightTheFirstAndLastRow()
    {
        TEditorFrameRun(
            ["fire"],
            (library, find) =>
            {
                TEditorFrameSend(library, new ConsoleKeyInfo('\0', ConsoleKey.End, false, false, true));
                Assert.Equal("Card.Fold", find(ConsoleKey.Enter, 0)?.BKeymapRowLabel);

                TEditorFrameSend(library, new ConsoleKeyInfo('\0', ConsoleKey.Home, false, false, true));
                Assert.Equal("Editor.Open", find(ConsoleKey.Enter, 0)?.BKeymapRowLabel);
            });
    }

    [Fact]
    public void LibraryKeymap_F3AndEscapeWhileEditing_WidenAndGiveTheListBack()
    {
        TEditorFrameRun(
            ["fire"],
            (library, find) =>
            {
                TEditorFrameSend(library, new ConsoleKeyInfo('\0', ConsoleKey.F3, false, false, false));
                Assert.True(library.BLibraryWhole);

                TEditorFrameSend(library, TLibrary.TLibraryEnter);
                TEditorFrameSend(library, new ConsoleKeyInfo('\u001b', ConsoleKey.Escape, false, false, false));
                Assert.True(library.BLibraryWhole);

                TEditorFrameSend(library, new ConsoleKeyInfo('\u001b', ConsoleKey.Escape, false, false, false));
                Assert.False(library.BLibraryWhole);
            });
    }

    [Fact]
    public void LibraryKeymap_TabWhileEditing_GoesBackToTheList()
    {
        TEditorFrameRun(
            ["fire"],
            (library, find) =>
            {
                TEditorFrameSend(library, new ConsoleKeyInfo('\t', ConsoleKey.Tab, false, false, false));

                Assert.Equal(BKeymapFocus.BKeymapFocusWindow, library.BLibraryKeymapFocus);
            });
    }

    [Fact]
    public void WindowKeyObserve_InputTab_EditsOverTheWholeWindowAndF7StartsBlank()
    {
        TEditorWindowRun(
            (window, atelier, console) =>
            {
                Assert.True(atelier.CAtelierNavigation.CNavigationTabSelect("Input"));
                window.BWindowBatchObserve();
                string[] grid = console.TConsoleFakeGrid;
                Assert.Contains(grid, static line => line.Contains("Headword"));
                Assert.DoesNotContain(grid, static line => line.Contains("Library"));
                Assert.Contains(grid, static line => line.Contains("Undo"));

                window.BWindowKeyObserve(new ConsoleKeyInfo('w', ConsoleKey.W, false, false, false));
                BLoop loop = Assert.IsType<BLoop>(SynchronizationContext.Current);
                TaskCompletionSource settled = new();
                loop.BLoopDefer(settled.SetResult);
                loop.BLoopRun(settled.Task.WaitAsync(TimeSpan.FromSeconds(10)));
                window.BWindowKeyObserve(TLibrary.TLibraryEnter);
                window.BWindowBatchObserve();
                Assert.Contains(console.TConsoleFakeGrid, static line => line.Contains("Unsaved"));

                window.BWindowKeyObserve(new ConsoleKeyInfo('\0', ConsoleKey.F7, false, false, false));
                window.BWindowBatchObserve();
                Assert.DoesNotContain(console.TConsoleFakeGrid, static line => line.Contains("Unsaved"));
            });
    }

    [Fact]
    public void EditorDraw_EntryWithSteps_LightsTheUndoMarkAtFirstDraw()
    {
        TEditorWindowRun(
            (_, atelier, _) =>
            {
                CEditor editor = atelier.CAtelierInputCreate(TEnvoyFake.TEnvoyCreate(false, []));
                BLocalizationCatalog catalog = new();
                catalog.BLocalizationCatalogApply(TInterface.TLocalizationLoad("en"));
                BEditor drawn = new(
                    catalog, editor, "Navigation.Input", static () => true, static () => true, static () => { });
                editor.CEditorEntryOpen(null);
                editor.CEditorEntry.CEntryHeadwordSet("water");
                editor.CEditorDesk.CDeskDraft.CDeskDraftPersist();
                BLoop loop = Assert.IsType<BLoop>(SynchronizationContext.Current);
                TaskCompletionSource settled = new();
                loop.BLoopDefer(settled.SetResult);
                loop.BLoopRun(settled.Task.WaitAsync(TimeSpan.FromSeconds(10)));
                BBlueprintPart window = BContract.BContractBlueprintFind(new ABootstrap(), "AWindow");
                BBlueprint.BBlueprintPlace(window, 0, 0, 80, 24);

                IReadOnlyList<BCanvasText> texts = drawn.BEditorDraw(
                    new Dictionary<string, IReadOnlyList<string>>(),
                    BContract.BContractFind(window, "ADisplay"),
                    true,
                    new BCanvasWidth(false));

                Assert.True(editor.CEditorDesk.CDeskChronicle.CDeskChronicleRead().CDeskBackward);
                BCanvasText undo = Assert.Single(texts, static text => text.BCanvasTextValue.Trim() == "Undo");
                Assert.Equal("text", undo.BCanvasTextRole);
            });
    }

    [Fact]
    public void EditorDraw_CaretMovedWithoutADraftChange_RedrawsTheFace()
    {
        TEditorWindowRun(
            (_, atelier, _) =>
            {
                CEditor editor = atelier.CAtelierInputCreate(TEnvoyFake.TEnvoyCreate(false, []));
                BLocalizationCatalog catalog = new();
                catalog.BLocalizationCatalogApply(TInterface.TLocalizationLoad("en"));
                int redraws = 0;
                _ = new BEditor(
                    catalog, editor, "Navigation.Input", static () => true, static () => true, () => redraws++);
                editor.CEditorEntryOpen(null);
                editor.CEditorList.CCardMeaningAdd();
                long card = editor.CEditorWalk[0][2].CStepCard!.CCardDraftId;
                editor.CEditorCaret.CCaretRegisterSettle(false, card, "slang, wild");
                int before = redraws;

                (bool moved, _) = editor.CEditorCaret.CCaretRegister.CCaretTrailMove(false, card, -1);

                Assert.True(moved);
                Assert.True(redraws > before);
            });
    }

    internal static void TEditorFrameRun(
        IReadOnlyList<string> headwords,
        Action<BLibrary, Func<ConsoleKey, ConsoleModifiers, BKeymapRow?>> check,
        List<string>? asked = null,
        Action<LEngine>? seed = null)
    {
        TLibrary.TLibraryRun(
            headwords,
            true,
            (library, _, _) =>
            {
                library.BLibraryObserve(library.BLibraryKeymapFocus, TLibrary.TLibraryEnter);
                ConsoleKeyInfo f4 = new('\0', ConsoleKey.F4, false, false, false);
                library.BLibraryKeymap.BKeymapFind(BKeymapFocus.BKeymapFocusWindow, f4)!.BKeymapRowAction(f4);
                check(library, (key, modifiers) => library.BLibraryKeymap.BKeymapFind(
                    library.BLibraryKeymapFocus,
                    new ConsoleKeyInfo(
                        '\0',
                        key,
                        false,
                        modifiers.HasFlag(ConsoleModifiers.Alt),
                        modifiers.HasFlag(ConsoleModifiers.Control))));
            },
            asked,
            seed: seed);
    }

    internal static IReadOnlyList<BCanvasText> TEditorFrameDraw(BLibrary library)
    {
        BBlueprintPart window = BContract.BContractBlueprintFind(new ABootstrap(), "AWindow");
        BBlueprint.BBlueprintPlace(window, 0, 0, 80, 24);
        return [.. library.BLibraryDraw(
            new Dictionary<string, IReadOnlyList<string>>(), window, true, new BCanvasWidth(false))];
    }

    internal static void TEditorFrameSend(BLibrary library, ConsoleKeyInfo key)
    {
        if ((library.BLibraryKeymap.BKeymapFind(library.BLibraryKeymapFocus, key)
                ?? library.BLibraryKeymap.BKeymapFind(BKeymapFocus.BKeymapFocusWindow, key)) is BKeymapRow row)
        {
            Assert.Equal(BKeymapLive.BKeymapLiveShown, row.BKeymapRowLive());
            row.BKeymapRowAction(key);
        }
        else
        {
            library.BLibraryObserve(library.BLibraryKeymapFocus, key);
        }

        BLoop loop = Assert.IsType<BLoop>(SynchronizationContext.Current);
        TaskCompletionSource settled = new();
        loop.BLoopDefer(settled.SetResult);
        loop.BLoopRun(settled.Task.WaitAsync(TimeSpan.FromSeconds(10)));
    }

    internal static void TEditorWindowRun(Action<BWindow, CAtelier, TConsoleFake> check)
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
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
