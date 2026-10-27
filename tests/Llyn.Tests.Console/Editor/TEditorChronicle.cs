using Llyn.Conduct;
using Llyn.ShellEngine;
using Llyn.UIDemeanor;
using Llyn.UITerminal;
using Xunit;
using static Llyn.Tests.TEditorFrame;

namespace Llyn.Tests;

public sealed class TEditorChronicle
{
    [Fact]
    public void EditorKeymap_ControlZWithStep_UndoesAndKeepsFieldOpen()
    {
        TEditorFrameRun(
            ["fire"],
            (library, find) =>
            {
                ConsoleKeyInfo undo = new('\u001a', ConsoleKey.Z, false, false, true);
                TEditorFrameSend(library, TLibrary.TLibraryEnter);
                TEditorFrameSend(library, new ConsoleKeyInfo('s', ConsoleKey.S, false, false, false));
                TEditorFrameSend(library, new ConsoleKeyInfo('\u001b', ConsoleKey.Escape, false, false, false));
                TEditorFrameSend(library, TLibrary.TLibraryEnter);
                Assert.Equal(
                    BKeymapLive.BKeymapLiveShown, find(ConsoleKey.Z, ConsoleModifiers.Control)?.BKeymapRowLive());

                TEditorFrameSend(library, undo);

                IReadOnlyList<BCanvasText> texts = TEditorFrameDraw(library);
                string title = texts.First(static text => text.BCanvasTextValue.StartsWith(" Editing "))
                    .BCanvasTextValue;
                string headword = title[" Editing ".Length..^1];
                BCanvasText field = texts.Last(static text => text.BCanvasTextRole == "field");
                BCanvasText caret = Assert.Single(texts, static text => text.BCanvasTextRole == "caret");
                Assert.Equal("fire", headword);
                Assert.Equal(headword, field.BCanvasTextValue.TrimEnd());
                Assert.Equal(field.BCanvasTextColumn + headword.Length, caret.BCanvasTextColumn);
                Assert.Equal("Editor.Close", find(ConsoleKey.Escape, 0)?.BKeymapRowLabel);
            });
    }

    [Fact]
    public void EditorKeymap_ControlZOnDimRow_KeepsCaretWhereTyped()
    {
        TEditorFrameRun(
            ["fire"],
            (library, find) =>
            {
                ConsoleKeyInfo undo = new('\u001a', ConsoleKey.Z, false, false, true);
                TEditorFrameSend(library, TLibrary.TLibraryEnter);
                TEditorFrameSend(library, new ConsoleKeyInfo('\0', ConsoleKey.Home, false, false, false));
                BKeymapRow row = find(ConsoleKey.Z, ConsoleModifiers.Control)!;
                Assert.Equal(BKeymapLive.BKeymapLiveDim, row.BKeymapRowLive());

                row.BKeymapRowAction(undo);
                TEditorFrameSend(library, new ConsoleKeyInfo('s', ConsoleKey.S, false, false, false));

                IReadOnlyList<BCanvasText> texts = TEditorFrameDraw(library);
                BCanvasText field = texts.Last(static text => text.BCanvasTextRole == "field");
                BCanvasText caret = Assert.Single(texts, static text => text.BCanvasTextRole == "caret");
                Assert.Equal("sfire", field.BCanvasTextValue.TrimEnd());
                Assert.Equal(field.BCanvasTextColumn + 1, caret.BCanvasTextColumn);
            },
            seed: static engine => engine.TEngineDelaySet(0));
    }

    [Fact]
    public void EditorKeymap_ControlYAndControlShiftZ_RedoesStep()
    {
        TEditorFrameRun(
            ["fire"],
            (library, find) =>
            {
                ConsoleKeyInfo undo = new('\u001a', ConsoleKey.Z, false, false, true);
                ConsoleKeyInfo shifted = new('\u001a', ConsoleKey.Z, true, false, true);
                Func<BKeymapRow?> again = () =>
                    library.BLibraryKeymap.BKeymapFind(library.BLibraryKeymapFocus, shifted);
                TEditorFrameSend(library, new ConsoleKeyInfo('s', ConsoleKey.S, false, false, false));
                TEditorFrameSend(library, new ConsoleKeyInfo('\u001b', ConsoleKey.Escape, false, false, false));
                Assert.Equal(
                    BKeymapLive.BKeymapLiveDim, find(ConsoleKey.Y, ConsoleModifiers.Control)?.BKeymapRowLive());
                Assert.Equal(BKeymapLive.BKeymapLiveDim, again()?.BKeymapRowLive());

                TEditorFrameSend(library, undo);
                Assert.Contains(TEditorFrameDraw(library), static text => text.BCanvasTextValue == " Editing fire ");

                TEditorFrameSend(library, new ConsoleKeyInfo('\u0019', ConsoleKey.Y, false, false, true));
                Assert.Contains(TEditorFrameDraw(library), static text => text.BCanvasTextValue == " Editing fires ");

                TEditorFrameSend(library, undo);
                Assert.Equal("Chronicle.Redo", again()?.BKeymapRowLabel);
                TEditorFrameSend(library, shifted);
                Assert.Contains(TEditorFrameDraw(library), static text => text.BCanvasTextValue == " Editing fires ");
            });
    }

    [Fact]
    public void NavigationDraw_EditorOnRightSide_ShowsUndoRedoDimmed()
    {
        TEditorFrameRun(
            ["fire"],
            (library, _) =>
            {
                TEditorFrameSend(library, new ConsoleKeyInfo('s', ConsoleKey.S, false, false, false));
                TEditorFrameSend(library, new ConsoleKeyInfo('\u001b', ConsoleKey.Escape, false, false, false));
                BLocalizationCatalog catalog = new();
                catalog.BLocalizationCatalogApply(TInterface.TLocalizationLoad("en"));
                ABootstrap bootstrap = new();
                BBlueprintPart window = BContract.BContractBlueprintFind(bootstrap, "AWindow");
                BBlueprint.BBlueprintPlace(window, 0, 0, 80, 24);
                BBlueprintPart roof = BContract.BContractFind(window, "ARoof");
                BBlueprintPart foot = BContract.BContractFind(window, "AVoyage");
                BNavigation tabs = new(bootstrap, null);
                CNavigationState state = new("Library", [], new CVoyageState(true, true));
                Assert.NotNull(library.BLibraryChronicle);
                (bool, bool) chronicle = library.BLibraryChronicle.Value;

                BCanvasText[] texts =
                [
                    .. tabs.BNavigationDraw(
                        catalog, state, new Dictionary<string, IReadOnlyList<string>>(), roof, foot,
                        new BCanvasWidth(false), chronicle),
                ];

                BCanvasText undo = Assert.Single(texts, static text => text.BCanvasTextValue == "Undo");
                BCanvasText redo = Assert.Single(texts, static text => text.BCanvasTextValue == "Redo");
                Assert.Equal(foot.BBlueprintPartTheme, undo.BCanvasTextRole);
                Assert.Equal("disabled", redo.BCanvasTextRole);
                Assert.DoesNotContain(texts, static text => text.BCanvasTextValue.Contains("Back"));

                ConsoleKeyInfo f4 = new('\0', ConsoleKey.F4, false, false, false);
                library.BLibraryKeymap.BKeymapFind(BKeymapFocus.BKeymapFocusWindow, f4)!.BKeymapRowAction(f4);

                Assert.Null(library.BLibraryChronicle);
            });
    }

    [Fact]
    public void EditorDraw_HaltedChronicle_RefusesFieldNamesFailure()
    {
        LEngine? engine = null;
        List<string> asked = [];
        TLibrary.TLibraryRun(
            [],
            true,
            (_, atelier, _) =>
            {
                CEditor editor = atelier.CAtelierInputCreate(TEnvoyFake.TEnvoyCreate(false, asked));
                BLocalizationCatalog catalog = new();
                catalog.BLocalizationCatalogApply(TInterface.TLocalizationLoad("en"));
                BEditor drawn = new(
                    catalog, editor, "Navigation.Input", static () => true, static () => true, static () => { });
                BBlueprintPart window = BContract.BContractBlueprintFind(new ABootstrap(), "AWindow");
                BBlueprint.BBlueprintPlace(window, 0, 0, 80, 24);
                BBlueprintPart display = BContract.BContractFind(window, "ADisplay");
                ConsoleKeyInfo enter = TLibrary.TLibraryEnter;
                Action<char> type = letter =>
                {
                    _ = drawn.BEditorObserve(new ConsoleKeyInfo(letter, ConsoleKey.A, false, false, false));
                    BLoop loop = Assert.IsType<BLoop>(SynchronizationContext.Current);
                    TaskCompletionSource settled = new();
                    loop.BLoopDefer(settled.SetResult);
                    loop.BLoopRun(settled.Task.WaitAsync(TimeSpan.FromSeconds(10)));
                };
                Func<IReadOnlyList<BCanvasText>> draw = () => drawn.BEditorDraw(
                    new Dictionary<string, IReadOnlyList<string>>(), display, true, new BCanvasWidth(false));

                editor.CEditorEntryOpen(null);
                type('w');
                Assert.False(editor.CEditorDesk.CDeskChronicle.CDeskChronicleHalted);
                engine!.TEngineDraftCancel(editor.CEditorDesk.CDeskId);

                type('a');

                Assert.True(editor.CEditorDesk.CDeskChronicle.CDeskChronicleHalted);
                Assert.Equal(["Input.HoldFailed"], asked);
                IReadOnlyList<BCanvasText> texts = draw();
                Assert.DoesNotContain(texts, static text => text.BCanvasTextRole is "field" or "lit" or "heading");
                Assert.Contains(
                    texts,
                    static text => text.BCanvasTextValue.Trim() == "The unsaved entry could not be held."
                        && text.BCanvasTextRole == "text");
                Assert.DoesNotContain(texts, static text => text.BCanvasTextValue.Trim() == "Undo");
                Assert.Equal(
                    BKeymapLive.BKeymapLiveDim,
                    drawn.BEditorKeymap.BKeymapFind(BKeymapFocus.BKeymapFocusEditor, enter)?.BKeymapRowLive());

                drawn.BEditorKeymap.BKeymapFind(BKeymapFocus.BKeymapFocusEditor, enter)!.BKeymapRowAction(enter);
                Assert.DoesNotContain(draw(), static text => text.BCanvasTextRole == "field");
                type('b');

                Assert.DoesNotContain(draw(),static text => text.BCanvasTextRole == "field");
            },
            seed: held =>
            {
                engine = held;
                held.TEngineDelaySet(0);
            });
    }
}
