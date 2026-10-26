using Llyn.Conduct;
using Llyn.UIDemeanor;
using Llyn.UITerminal;
using Xunit;
using static Llyn.Tests.TEditorFrame;

namespace Llyn.Tests;

public sealed class TEditorMarker
{
    private static readonly ConsoleKeyInfo TEditorMarkerRight = new('\0', ConsoleKey.RightArrow, false, false, false);

    private static readonly ConsoleKeyInfo TEditorMarkerEscape = new('\u001b', ConsoleKey.Escape, false, false, false);

    private static readonly ConsoleKeyInfo TEditorMarkerBack = new('\b', ConsoleKey.Backspace, false, false, false);

    [Fact]
    public void EditorKeymap_EnterOnTheUnitChooser_OpensItsListAndAPickSetsTheUnit()
    {
        TEditorMarkerRun((library, find) =>
        {
            Assert.Contains(TEditorFrameDraw(library), static text => text.BCanvasTextValue.StartsWith("Unit v"));
            Assert.Equal("Editor.Unit", find(ConsoleKey.Enter)?.BKeymapRowLabel);

            TEditorFrameSend(library, TLibrary.TLibraryEnter);

            Assert.Equal("Editor.Pick", find(ConsoleKey.Enter)?.BKeymapRowLabel);
            IReadOnlyList<BCanvasText> texts = TEditorFrameDraw(library);
            Assert.Contains(texts, static text => text.BCanvasTextRole == "shadow");
            Assert.Contains(texts, static text => text.BCanvasTextValue.StartsWith(" Expression"));
            TEditorMarkerSeek(library, " Word");

            TEditorFrameSend(library, TLibrary.TLibraryEnter);

            Assert.Equal("Editor.Unit", find(ConsoleKey.Enter)?.BKeymapRowLabel);
            texts = TEditorFrameDraw(library);
            Assert.DoesNotContain(texts, static text => text.BCanvasTextRole == "shadow");
            Assert.Contains(texts, static text => text.BCanvasTextValue.StartsWith("Word v"));
            Assert.Contains(texts, static text => text.BCanvasTextValue.Contains("Unsaved"));
        });
    }

    [Fact]
    public void EditorKeymap_PickingTheHeldFreedom_ChecksItThenClearsIt()
    {
        TEditorMarkerRun((library, find) =>
        {
            TEditorFrameSend(library, TEditorMarkerRight);
            Assert.Equal("Editor.Freedom", find(ConsoleKey.Enter)?.BKeymapRowLabel);
            TEditorFrameSend(library, TLibrary.TLibraryEnter);
            TEditorMarkerSeek(library, " Standalone");
            TEditorFrameSend(library, TLibrary.TLibraryEnter);
            Assert.Contains(TEditorFrameDraw(library), static text => text.BCanvasTextValue.StartsWith("Standalone v"));

            TEditorFrameSend(library, TLibrary.TLibraryEnter);

            BCanvasText held = Assert.Single(TEditorFrameDraw(library), static text => text.BCanvasTextRole == "lit"
                && text.BCanvasTextValue.StartsWith(' '));
            Assert.StartsWith(" Standalone", held.BCanvasTextValue);
            Assert.EndsWith("[x] ", held.BCanvasTextValue);

            TEditorFrameSend(library, TLibrary.TLibraryEnter);

            Assert.Contains(TEditorFrameDraw(library), static text => text.BCanvasTextValue.Contains("Freedom v"));
        });
    }

    [Fact]
    public void EditorKeymap_TypingOnTheSpeechField_OffersTheMenuAndALitRowBecomesAChip()
    {
        TEditorMarkerRun((library, find) =>
        {
            TEditorFrameSend(library, TEditorMarkerRight);
            TEditorFrameSend(library, TEditorMarkerRight);
            TEditorMarkerSend(library, "nou");

            Assert.Equal("Speech.Add", find(ConsoleKey.Enter)?.BKeymapRowLabel);
            IReadOnlyList<BCanvasText> texts = TEditorFrameDraw(library);
            Assert.Contains(texts, static text => text.BCanvasTextRole == "shadow");
            Assert.Contains(texts, static text => text.BCanvasTextRole == "menu"
                && text.BCanvasTextValue.StartsWith(" Noun"));
            Assert.DoesNotContain(texts, static text => text.BCanvasTextRole is "lit" or "lit-other"
                && text.BCanvasTextValue.StartsWith(" Noun"));

            TEditorFrameSend(library, TLibrary.TLibraryDown);
            Assert.Equal("Editor.Pick", find(ConsoleKey.Enter)?.BKeymapRowLabel);
            TEditorMarkerSeek(library, " Noun");
            TEditorFrameSend(library, TLibrary.TLibraryEnter);

            texts = TEditorFrameDraw(library);
            Assert.DoesNotContain(texts, static text => text.BCanvasTextRole == "shadow");
            Assert.Contains(texts, static text => text.BCanvasTextValue.Contains("(Noun)"));
            Assert.Equal("Speech.Remove", find(ConsoleKey.Backspace)?.BKeymapRowLabel);

            TEditorFrameSend(library, TEditorMarkerBack);

            Assert.DoesNotContain(TEditorFrameDraw(library), static text => text.BCanvasTextValue.Contains("(Noun)"));
        });
    }

    [Fact]
    public void EditorKeymap_EscapeOnTheSpeechMenu_ShutsItKeepingTheTypingThenClosesTheField()
    {
        TEditorMarkerRun((library, find) =>
        {
            TEditorFrameSend(library, TEditorMarkerRight);
            TEditorFrameSend(library, TEditorMarkerRight);
            TEditorMarkerSend(library, "ver");
            Assert.Equal("Editor.Shut", find(ConsoleKey.Escape)?.BKeymapRowLabel);

            TEditorFrameSend(library, TEditorMarkerEscape);

            Assert.DoesNotContain(TEditorFrameDraw(library), static text => text.BCanvasTextRole == "shadow");
            Assert.Contains(TEditorFrameDraw(library), static text => text.BCanvasTextRole == "field"
                && text.BCanvasTextValue.StartsWith("ver"));
            Assert.Equal("Editor.Close", find(ConsoleKey.Escape)?.BKeymapRowLabel);

            TEditorFrameSend(library, TEditorMarkerEscape);

            Assert.DoesNotContain(TEditorFrameDraw(library), static text => text.BCanvasTextRole == "field"
                && text.BCanvasTextValue.StartsWith("ver"));
            Assert.Contains(TEditorFrameDraw(library), static text => text.BCanvasTextValue.Contains("ver"));
            Assert.Equal("Editor.Open", find(ConsoleKey.Enter)?.BKeymapRowLabel);
        });
    }

    [Fact]
    public void EditorKeymap_ArrowOnAnUnmatchedSpeechField_ShowsTheHintAsOneDimLine()
    {
        TEditorMarkerRun((library, find) =>
        {
            TEditorFrameSend(library, TEditorMarkerRight);
            TEditorFrameSend(library, TEditorMarkerRight);
            TEditorMarkerSend(library, "zzqq");
            Assert.DoesNotContain(TEditorFrameDraw(library), static text => text.BCanvasTextRole == "shadow");
            Assert.Equal("Speech.Offer", find(ConsoleKey.DownArrow)?.BKeymapRowLabel);

            TEditorFrameSend(library, TLibrary.TLibraryDown);

            Assert.Equal("muted", Assert.Single(TEditorFrameDraw(library), static text =>
                text.BCanvasTextValue.Contains("No part of speech matches")).BCanvasTextRole);
            Assert.Equal("Speech.Add", find(ConsoleKey.Enter)?.BKeymapRowLabel);
        });
    }

    [Fact]
    public void EditorKeymap_LightLeavingTheMarkerRow_ClosesItsOpenList()
    {
        TEditorMarkerRun((library, find) =>
        {
            TEditorFrameSend(library, TLibrary.TLibraryEnter);
            Assert.Contains(TEditorFrameDraw(library), static text => text.BCanvasTextRole == "shadow");

            TEditorFrameSend(library, new ConsoleKeyInfo('\0', ConsoleKey.PageDown, false, false, false));

            Assert.DoesNotContain(TEditorFrameDraw(library), static text => text.BCanvasTextRole == "shadow");
            Assert.NotEqual("Editor.Pick", find(ConsoleKey.Enter)?.BKeymapRowLabel);
        });
    }

    [Fact]
    public void EditorKeymap_FailedLanguageLoad_OpensAnEmptyList()
    {
        TEditorWindowRun(
            (_, atelier, _) =>
            {
                CEditor editor = atelier.CAtelierInputCreate(TEnvoyFake.TEnvoyCreate(false, []));
                BLocalizationCatalog catalog = new();
                catalog.BLocalizationCatalogApply(TInterface.TLocalizationLoad("en"));
                BEditor drawn = new(
                    catalog,
                    editor,
                    "Navigation.Input",
                    static () => true,
                    static () => true,
                    static () => { },
                    languages: static () => Task.FromResult<IReadOnlyList<string>>([]));
                editor.CEditorEntryOpen(null);
                TEditorMarkerSettle();
                Func<ConsoleKey, BKeymapRow?> find = key => drawn.BEditorKeymap.BKeymapFind(
                    BKeymapFocus.BKeymapFocusEditor, new ConsoleKeyInfo('\0', key, false, false, false));
                find(ConsoleKey.DownArrow)!.BKeymapRowAction(TLibrary.TLibraryDown);
                Assert.Equal("Editor.Language", find(ConsoleKey.Enter)?.BKeymapRowLabel);

                find(ConsoleKey.Enter)!.BKeymapRowAction(TLibrary.TLibraryEnter);
                TEditorMarkerSettle();

                Assert.Equal("Editor.Shut", find(ConsoleKey.Escape)?.BKeymapRowLabel);
                Assert.NotEqual("Editor.Pick", find(ConsoleKey.Enter)?.BKeymapRowLabel);
                BBlueprintPart window = BContract.BContractBlueprintFind(new ABootstrap(), "AWindow");
                BBlueprint.BBlueprintPlace(window, 0, 0, 80, 24);
                IReadOnlyList<BCanvasText> texts = drawn.BEditorDraw(
                    new Dictionary<string, IReadOnlyList<string>>(),
                    BContract.BContractFind(window, "ADisplay"),
                    true,
                    new BCanvasWidth(false));
                Assert.Contains(texts, static text => text.BCanvasTextRole == "shadow");
                Assert.DoesNotContain(texts, static text => text.BCanvasTextRole is "menu" or "lit"
                    && text.BCanvasTextValue.StartsWith(' ') && text.BCanvasTextValue.Trim().Length > 0);
            });
    }

    private static void TEditorMarkerRun(Action<BLibrary, Func<ConsoleKey, BKeymapRow?>> check)
    {
        TLibrary.TLibraryRun(
            ["fire"],
            true,
            (library, _, _) =>
            {
                ConsoleKeyInfo f4 = new('\0', ConsoleKey.F4, false, false, false);
                library.BLibraryObserve(library.BLibraryKeymapFocus, TLibrary.TLibraryEnter);
                library.BLibraryKeymap.BKeymapFind(BKeymapFocus.BKeymapFocusWindow, f4)!.BKeymapRowAction(f4);
                TEditorFrameSend(library, TLibrary.TLibraryDown);
                TEditorFrameSend(library, TLibrary.TLibraryDown);
                TEditorFrameSend(library, TLibrary.TLibraryDown);
                check(library, key => library.BLibraryKeymap.BKeymapFind(
                    library.BLibraryKeymapFocus, new ConsoleKeyInfo('\0', key, false, false, false)));
            });
    }

    private static void TEditorMarkerSend(BLibrary library, string typed)
    {
        foreach (char letter in typed)
        {
            TEditorFrameSend(library, new ConsoleKeyInfo(letter, ConsoleKey.A, false, false, false));
        }
    }

    private static void TEditorMarkerSeek(BLibrary library, string row)
    {
        Func<string> lit = () => Assert.Single(TEditorFrameDraw(library), static text => text.BCanvasTextRole == "lit"
            && text.BCanvasTextValue.StartsWith(' ')).BCanvasTextValue;
        for (int step = 0; !lit().StartsWith(row, StringComparison.Ordinal); step++)
        {
            Assert.True(step < 8, $"No row starts with {row}.");
            TEditorFrameSend(library, TLibrary.TLibraryDown);
        }
    }

    private static void TEditorMarkerSettle()
    {
        BLoop loop = Assert.IsType<BLoop>(SynchronizationContext.Current);
        TaskCompletionSource settled = new();
        loop.BLoopDefer(settled.SetResult);
        loop.BLoopRun(settled.Task.WaitAsync(TimeSpan.FromSeconds(10)));
    }
}
