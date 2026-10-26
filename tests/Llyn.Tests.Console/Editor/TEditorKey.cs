using Llyn.Conduct;
using Llyn.ShellEngine;
using Llyn.UIDemeanor;
using Llyn.UITerminal;
using Xunit;

namespace Llyn.Tests;

public sealed class TEditorKey
{
    private static readonly ConsoleKeyInfo TEditorKeyEscape = new('\u001b', ConsoleKey.Escape, false, false, false);

    private static readonly ConsoleKeyInfo TEditorKeyEight = new('\0', ConsoleKey.F8, false, false, false);

    [Fact]
    public void EditorKeymap_ControlPageUpOnALaterCard_LightsThePreviousCardHeader()
    {
        TEditorFrame.TEditorFrameRun(
            [],
            (library, find) =>
            {
                Func<string, string> role = rank => Assert.Single(
                    TEditorFrame.TEditorFrameDraw(library), text => text.BCanvasTextValue.StartsWith(rank))
                    .BCanvasTextRole;
                TEditorFrame.TEditorFrameSend(library, new ConsoleKeyInfo('\0', ConsoleKey.End, false, false, true));
                Assert.Equal("lit", role("-- 2"));
                Assert.NotNull(find(ConsoleKey.PageUp, ConsoleModifiers.Control));

                TEditorFrame.TEditorFrameSend(library, new ConsoleKeyInfo('\0', ConsoleKey.PageUp, false, false, true));

                Assert.Equal("lit", role("-- 1"));
                Assert.NotEqual("lit", role("-- 2"));
                Assert.Null(find(ConsoleKey.PageUp, ConsoleModifiers.Control));
                Assert.NotNull(find(ConsoleKey.PageDown, ConsoleModifiers.Control));
            },
            seed: static engine => engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
                "fire",
                "English",
                string.Empty,
                string.Empty,
                [TInterface.TCardCreate("a flame", 1), TInterface.TCardCreate("a blaze", 2)],
                [])));
    }

    [Fact]
    public void LibraryKeymap_F8WhileEditing_AsksBeforeDeletingTheEditedEntry()
    {
        List<string> asked = [];
        TEditorFrame.TEditorFrameRun(
            ["fire"],
            (library, _) =>
            {
                Assert.Equal(BKeymapFocus.BKeymapFocusEditor, library.BLibraryKeymapFocus);

                TEditorFrame.TEditorFrameSend(library, TEditorKeyEight);

                Assert.Equal(["Scribe.DeleteConfirm"], asked);
                Assert.Contains("fire", TLibrary.TLibraryDraw(library));
            },
            asked);
    }

    [Fact]
    public void FavoriteKeymap_F8WhileEditing_IsLiveAndKeepsTheEntryWhenDeclined()
    {
        TFavoriteTab.TFavoriteTabRun(
            [("apple", "English", 0)],
            (library, send, _) =>
            {
                send(TLibrary.TLibraryEnter);
                ConsoleKeyInfo four = new('\0', ConsoleKey.F4, false, false, false);
                library.BLibraryKeymap.BKeymapFind(BKeymapFocus.BKeymapFocusWindow, four)!.BKeymapRowAction(four);
                Assert.Equal(BKeymapFocus.BKeymapFocusEditor, library.BLibraryKeymapFocus);
                BKeymapRow row = library.BLibraryKeymap.BKeymapFind(BKeymapFocus.BKeymapFocusWindow, TEditorKeyEight)!;

                Assert.Equal("Scribe.Delete", row.BKeymapRowLabel);
                Assert.Equal(BKeymapLive.BKeymapLiveShown, row.BKeymapRowLive());
                row.BKeymapRowAction(TEditorKeyEight);

                Assert.Contains("apple", TLibrary.TLibraryDraw(library));
            });
    }

    [Fact]
    public void InputKeymap_F8WhileEditing_LeavesTheDraftAlone()
    {
        TEditorFrame.TEditorWindowRun(
            (window, atelier, console) =>
            {
                Assert.True(atelier.CAtelierNavigation.CNavigationTabSelect("Input"));
                window.BWindowKeyObserve(new ConsoleKeyInfo('w', ConsoleKey.W, false, false, false));
                BLoop loop = Assert.IsType<BLoop>(SynchronizationContext.Current);
                TaskCompletionSource settled = new();
                loop.BLoopDefer(settled.SetResult);
                loop.BLoopRun(settled.Task.WaitAsync(TimeSpan.FromSeconds(10)));
                window.BWindowKeyObserve(TEditorKeyEscape);
                window.BWindowBatchObserve();
                Assert.Contains(console.TConsoleFakeGrid, static line => line.Contains("Unsaved"));

                window.BWindowKeyObserve(TEditorKeyEight);
                window.BWindowBatchObserve();

                Assert.Contains(console.TConsoleFakeGrid, static line => line.Contains("Unsaved"));
            });
    }

    [Fact]
    public void LibraryKeymap_TabFromTheList_GoesBackToTheEditor()
    {
        TEditorFrame.TEditorFrameRun(
            ["fire"],
            (library, find) =>
            {
                ConsoleKeyInfo tab = new('\t', ConsoleKey.Tab, false, false, false);
                TEditorFrame.TEditorFrameSend(library, tab);
                Assert.Equal(BKeymapFocus.BKeymapFocusWindow, library.BLibraryKeymapFocus);

                TEditorFrame.TEditorFrameSend(library, tab);

                Assert.Equal(BKeymapFocus.BKeymapFocusEditor, library.BLibraryKeymapFocus);
                Assert.Equal("Editor.Open", find(ConsoleKey.Enter, 0)?.BKeymapRowLabel);
            });
    }

    [Fact]
    public void EditorKeymap_EscapeOnAnOpenField_ClosesItAndKeepsTheHeadwordLit()
    {
        TEditorFrame.TEditorFrameRun(
            ["fire"],
            (library, find) =>
            {
                TEditorFrame.TEditorFrameSend(library, TLibrary.TLibraryEnter);
                TEditorFrame.TEditorFrameSend(library, new ConsoleKeyInfo('s', ConsoleKey.S, false, false, false));

                TEditorFrame.TEditorFrameSend(library, TEditorKeyEscape);

                Assert.Equal("Editor.Open", find(ConsoleKey.Enter, 0)?.BKeymapRowLabel);
                Assert.Null(find(ConsoleKey.Escape, 0));
                BCanvasText headword = Assert.Single(
                    TEditorFrame.TEditorFrameDraw(library), static text => text.BCanvasTextValue.StartsWith("fires"));
                Assert.Equal("lit", headword.BCanvasTextRole);
            });
    }

    [Fact]
    public void EditorKeymap_UpAndDownOnAnOpenField_CloseItAndMoveTheLight()
    {
        TEditorFrame.TEditorFrameRun(
            ["fire"],
            (library, find) =>
            {
                TEditorFrame.TEditorFrameSend(library, TLibrary.TLibraryEnter);
                Assert.Equal("Editor.Close", find(ConsoleKey.UpArrow, 0)?.BKeymapRowLabel);

                ConsoleKeyInfo up = new('\0', ConsoleKey.UpArrow, false, false, false);
                TEditorFrame.TEditorFrameSend(library, up);

                Assert.Equal("Editor.Open", find(ConsoleKey.Enter, 0)?.BKeymapRowLabel);

                TEditorFrame.TEditorFrameSend(library, TLibrary.TLibraryEnter);
                Assert.Equal("Editor.Close", find(ConsoleKey.DownArrow, 0)?.BKeymapRowLabel);

                TEditorFrame.TEditorFrameSend(library, TLibrary.TLibraryDown);

                Assert.Equal("Window.Help.Item", find(ConsoleKey.DownArrow, 0)?.BKeymapRowLabel);
                Assert.Equal("Editor.Language", find(ConsoleKey.Enter, 0)?.BKeymapRowLabel);
                IReadOnlyList<BCanvasText> texts = TEditorFrame.TEditorFrameDraw(library);
                BCanvasText pill = Assert.Single(texts, static text => text.BCanvasTextValue.StartsWith("English v"));
                Assert.Equal("lit", pill.BCanvasTextRole);
            });
    }

    [Fact]
    public void EditorKeymap_AltOneToFour_ChooseEachStackTab()
    {
        TEditorFrame.TEditorFrameRun(
            ["fire"],
            (library, _) =>
            {
                string[] tabs = ["[Meaning 1]", "[Collocation 1]", "[Etymology]", "[Note]"];
                foreach ((string tab, int index) in tabs.Select(static (tab, index) => (tab, index)))
                {
                    TEditorFrame.TEditorFrameSend(
                        library, new ConsoleKeyInfo((char)('1' + index), ConsoleKey.D1 + index, false, true, false));

                    Assert.True(
                        TEditorFrame.TEditorFrameDraw(library).Any(text => text.BCanvasTextValue.Contains(tab)), tab);
                }
            });
    }

    [Fact]
    public void EditorDraw_AltThreeAtANarrowWidth_KeepsTheChosenTabWhole()
    {
        TEditorFrame.TEditorFrameRun(
            ["fire"],
            (library, _) =>
            {
                TEditorFrame.TEditorFrameSend(library, new ConsoleKeyInfo('3', ConsoleKey.D3, false, true, false));
                BBlueprintPart window = BContract.BContractBlueprintFind(new ABootstrap(), "AWindow");
                BBlueprint.BBlueprintPlace(window, 0, 0, 60, 24);

                IReadOnlyList<BCanvasText> texts = [.. library.BLibraryDraw(
                    new Dictionary<string, IReadOnlyList<string>>(), window, true, new BCanvasWidth(false))];

                Assert.Contains(texts, static text => text.BCanvasTextValue.Contains("[Etymology]"));
                Assert.DoesNotContain(texts, static text => text.BCanvasTextValue.Contains("Meaning 1"));
            });
    }
}
