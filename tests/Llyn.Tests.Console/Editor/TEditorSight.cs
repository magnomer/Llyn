using Llyn.Conduct;
using Llyn.Core;
using Llyn.UIDemeanor;
using Llyn.UITerminal;
using Xunit;

namespace Llyn.Tests;

public sealed class TEditorSight
{
    [Fact]
    public void EditorDraw_LitHeadwordClosed_ParksTheCursorAtTheEndOfTheText()
    {
        TEditorFrame.TEditorFrameRun(
            ["fire"],
            (library, _) =>
            {
                IReadOnlyList<BCanvasText> texts = TEditorFrame.TEditorFrameDraw(library);
                BCanvasText shining = Assert.Single(
                    texts, static text => text.BCanvasTextRole == "lit" && text.BCanvasTextValue.StartsWith("fire"));
                BCanvasText caret = Assert.Single(texts, static text => text.BCanvasTextRole == "caret");

                Assert.Equal(shining.BCanvasTextRow, caret.BCanvasTextRow);
                Assert.Equal(shining.BCanvasTextColumn + 4, caret.BCanvasTextColumn);

                TEditorFrame.TEditorFrameSend(library, new ConsoleKeyInfo('s', ConsoleKey.S, false, false, false));

                Assert.Contains(
                    TEditorFrame.TEditorFrameDraw(library), static text => text.BCanvasTextValue.StartsWith("fires"));
                Assert.Equal(
                    "Editor.Close",
                    library.BLibraryKeymap.BKeymapFind(
                        library.BLibraryKeymapFocus,
                        new ConsoleKeyInfo('\u001b', ConsoleKey.Escape, false, false, false))?.BKeymapRowLabel);
            });
    }

    [Fact]
    public void EditorDraw_SideTallerThanItsRows_ShowsAPositionMarkAndTheHiddenCount()
    {
        TEditorFrame.TEditorFrameRun(
            [],
            (library, _) =>
            {
                IReadOnlyList<BCanvasText> texts = TEditorFrame.TEditorFrameDraw(library);
                BCanvasText shining = texts.Where(static text => text.BCanvasTextRole == "lit")
                    .MaxBy(static text => text.BCanvasTextColumn)!;
                int border = shining.BCanvasTextColumn + shining.BCanvasTextSpan;
                BCanvasText mark = Assert.Single(
                    texts, text => text.BCanvasTextColumn == border && text.BCanvasTextValue == "#");
                BCanvasText more = Assert.Single(texts, static text => text.BCanvasTextValue.EndsWith(" more"));

                Assert.Equal("#", mark.BCanvasTextValue);
                Assert.Equal(shining.BCanvasTextRow, mark.BCanvasTextRow);
                Assert.Matches(@"^v \d+ more$", more.BCanvasTextValue);

                TEditorFrame.TEditorFrameSend(
                    library, new ConsoleKeyInfo('\0', ConsoleKey.End, false, false, true));
                texts = TEditorFrame.TEditorFrameDraw(library);
                BCanvasText moved = Assert.Single(
                    texts, text => text.BCanvasTextColumn == border && text.BCanvasTextValue == "#");

                Assert.True(moved.BCanvasTextRow > mark.BCanvasTextRow);
                Assert.DoesNotContain(texts, static text => text.BCanvasTextValue.EndsWith(" more"));
            },
            seed: static engine => engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
                "fire",
                "English",
                string.Empty,
                string.Empty,
                [.. Enumerable.Range(1, 40).Select(static rank => TInterface.TCardCreate("a meaning", rank))],
                [])));
    }

    [Theory]
    [InlineData("x", 200)]
    [InlineData("火", 100)]
    public void EditorDraw_LitHeadwordClosedLongerThanTheSide_ParksTheCursorInsideTheSide(string glyph, int repeats)
    {
        string headword = string.Concat(Enumerable.Repeat(glyph, repeats));
        TEditorFrame.TEditorFrameRun(
            [headword],
            (library, _) =>
            {
                IReadOnlyList<BCanvasText> texts = TEditorFrame.TEditorFrameDraw(library);
                BCanvasText shining = Assert.Single(
                    texts, static text => text.BCanvasTextRole == "lit" && text.BCanvasTextRow > 1);
                BCanvasText caret = Assert.Single(texts, static text => text.BCanvasTextRole == "caret");

                Assert.Equal(shining.BCanvasTextRow, caret.BCanvasTextRow);
                Assert.Equal(shining.BCanvasTextColumn + shining.BCanvasTextSpan - 1, caret.BCanvasTextColumn);
            });
    }

    [Theory]
    [InlineData(1, false)]
    [InlineData(2, true)]
    public void EditorDraw_RowsAgainstTheSide_ShowThumbAndHiddenCountOnlyPastTheFit(int cards, bool overflows)
    {
        TEditorFrame.TEditorFrameRun(
            [],
            (library, _) =>
            {
                IReadOnlyList<BCanvasText> texts = TEditorFrame.TEditorFrameDraw(library);

                Assert.Equal(overflows, texts.Any(static text => text.BCanvasTextValue == "#"));
                Assert.Equal(overflows, texts.Any(static text => text.BCanvasTextValue.EndsWith(" more")));
            },
            seed: engine => engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
                "fire",
                "English",
                string.Empty,
                string.Empty,
                [.. Enumerable.Range(1, cards).Select(static rank => TInterface.TCardCreate("a meaning", rank))],
                [])));
    }

    [Fact]
    public void EditorDraw_UncertainCardTitle_ShowsTheUnknownMark()
    {
        TLibrary.TLibraryRun(
            ["fire"],
            true,
            (library, _, _) =>
            {
                ConsoleKeyInfo f4 = new('\0', ConsoleKey.F4, false, false, false);
                library.BLibraryObserve(library.BLibraryKeymapFocus, TLibrary.TLibraryEnter);
                library.BLibraryKeymap.BKeymapFind(BKeymapFocus.BKeymapFocusWindow, f4)!.BKeymapRowAction(f4);
                string drawn = string.Join(
                    "\n", TEditorFrame.TEditorFrameDraw(library).Select(static text => text.BCanvasTextValue));

                Assert.Contains("-- 1 - Unknown ", drawn);
                Assert.DoesNotContain("broken", drawn);
                Assert.DoesNotContain("Display.Unknown", drawn);
            },
            script: "UPDATE sense SET title_state = 'broken', title = NULL;");
    }

    [Fact]
    public void EditorKeymap_SaveWithAnUnreadableValue_RefusesWithValueUnreadable()
    {
        List<string> asked = [];
        TLibrary.TLibraryRun(
            ["fire"],
            true,
            (library, atelier, _) =>
            {
                ConsoleKeyInfo f4 = new('\0', ConsoleKey.F4, false, false, false);
                library.BLibraryObserve(library.BLibraryKeymapFocus, TLibrary.TLibraryEnter);
                library.BLibraryKeymap.BKeymapFind(BKeymapFocus.BKeymapFocusWindow, f4)!.BKeymapRowAction(f4);
                TEditorFrame.TEditorFrameSend(library, new ConsoleKeyInfo('s', ConsoleKey.S, false, false, false));
                TEditorFrame.TEditorFrameSend(library, TLibrary.TLibraryEnter);
                asked.Clear();
                ConsoleKeyInfo f2 = new('\0', ConsoleKey.F2, false, false, false);

                library.BLibraryKeymap.BKeymapFind(BKeymapFocus.BKeymapFocusWindow, f2)!.BKeymapRowAction(f2);
                TEditorFrame.TEditorFrameSend(library, new ConsoleKeyInfo('\0', ConsoleKey.F24, false, false, false));

                Assert.Equal(["Notice.UnreadableDrop", "Input.SaveFailed"], asked);
                string drawn = string.Join(
                    "\n", TEditorFrame.TEditorFrameDraw(library).Select(static text => text.BCanvasTextValue));
                Assert.Contains(" Editing fires ", drawn);
                Assert.Contains("Unsaved", drawn);
            },
            asked,
            "UPDATE sense SET title_state = 'broken', title = NULL;");
    }

    [Fact]
    public void EditorObserve_RefusedCardAddAfterTypingTheHeadword_ShowsTheUpdateFailureEachTimeAndKeepsTheText()
    {
        TLibrary.TLibraryRun(
            [],
            true,
            (_, atelier, _) =>
            {
                List<string> asked = [];
                CEditor editor = atelier.CAtelierInputCreate(TEnvoyFake.TEnvoyCreate(false, asked));
                BLocalizationCatalog catalog = new();
                catalog.BLocalizationCatalogApply(TInterface.TLocalizationLoad("en"));
                BEditor drawn = new(
                    catalog, editor, "Navigation.Input", static () => true, static () => true, static () => { });
                editor.CEditorEntryOpen(null);
                BLoop loop = Assert.IsType<BLoop>(SynchronizationContext.Current);
                Action settle = () =>
                {
                    TaskCompletionSource settled = new();
                    loop.BLoopDefer(settled.SetResult);
                    loop.BLoopRun(settled.Task.WaitAsync(TimeSpan.FromSeconds(10)));
                };
                settle();
                foreach (char letter in "fire")
                {
                    _ = drawn.BEditorObserve(new ConsoleKeyInfo(letter, ConsoleKey.A, false, false, false));
                    settle();
                }

                for (int tried = 0; tried < 2; tried++)
                {
                    editor.CEditorDesk.TDeskDefer(TInterface.TRequestAdditionCreate(
                        editor.CEditorDesk.CDeskId, LCardKind.LCardKindMeaning, long.MaxValue, 0));
                    settle();
                }

                BBlueprintPart window = BContract.BContractBlueprintFind(new ABootstrap(), "AWindow");
                BBlueprint.BBlueprintPlace(window, 0, 0, 80, 24);
                IReadOnlyList<BCanvasText> texts = drawn.BEditorDraw(
                    new Dictionary<string, IReadOnlyList<string>>(),
                    BContract.BContractFind(window, "ADisplay"),
                    true,
                    new BCanvasWidth(false));

                Assert.Equal(["Input.UpdateFailed", "Input.UpdateFailed"], asked);
                BCanvasText field = Assert.Single(texts, static text => text.BCanvasTextRole == "field");
                Assert.StartsWith("fire", field.BCanvasTextValue);
            },
            seed: static engine => engine.TEngineDelaySet(0));
    }

    [Fact]
    public void EditorDraw_PasteFailingAfterTheFirstPart_KeepsTheUntriedPart()
    {
        bool failing = false;
        List<string> asked = [];
        TEditorInlet.TEditorInletRun(
            1,
            (library, _) =>
            {
                TEditorInlet.TEditorInletSend(library, "d");
                failing = true;

                TEditorSightSend(library, "ry,rain,");
                failing = false;

                IReadOnlyList<BCanvasText> texts = TEditorFrame.TEditorFrameDraw(library);
                Assert.Contains("Input.UpdateFailed", asked);
                Assert.Contains(texts, static text => text.BCanvasTextValue == "[dry]");
                Assert.DoesNotContain(texts, static text => text.BCanvasTextValue == "[rain]");
                Assert.Contains(texts, static text => TEditorInlet.TEditorInletMatch(text)
                    && text.BCanvasTextValue.Contains("rain", StringComparison.Ordinal));
            },
            engine => engine.TEngineObserverAttach(bulletin =>
            {
                if (failing && bulletin.LBulletinSubject == LSubject.LSubjectTenure)
                {
                    throw new InvalidOperationException("The tenure observer failed.");
                }
            }),
            asked);
    }

    private static void TEditorSightSend(BLibrary library, string pasted)
    {
        foreach (char letter in pasted)
        {
            ConsoleKeyInfo key = new(letter, letter == ',' ? ConsoleKey.OemComma : ConsoleKey.A, false, false, false);
            if (library.BLibraryKeymap.BKeymapFind(library.BLibraryKeymapFocus, key) is BKeymapRow row)
            {
                row.BKeymapRowAction(key);
            }
            else
            {
                library.BLibraryObserve(library.BLibraryKeymapFocus, key);
            }
        }

        BLoop loop = Assert.IsType<BLoop>(SynchronizationContext.Current);
        TaskCompletionSource settled = new();
        loop.BLoopDefer(settled.SetResult);
        loop.BLoopRun(settled.Task.WaitAsync(TimeSpan.FromSeconds(10)));
    }
}
