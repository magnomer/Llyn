using Llyn.Conduct;
using Llyn.UIDemeanor;
using Llyn.UITerminal;
using Xunit;
using static Llyn.Tests.TEditorFrame;

namespace Llyn.Tests;

public sealed class TEditorHead
{
    private static readonly ConsoleKeyInfo TEditorHeadRight = new('\0', ConsoleKey.RightArrow, false, false, false);

    private static readonly ConsoleKeyInfo TEditorHeadOne = new('1', ConsoleKey.D1, false, false, false);

    private static readonly ConsoleKeyInfo TEditorHeadSix = new('\0', ConsoleKey.F6, false, false, false);

    [Fact]
    public void EditorKeymap_EnterOnTheLanguagePill_OpensTheListAndALetterLightsTheNextMatch()
    {
        TEditorHeadRun((library, atelier, find) =>
        {
            IReadOnlyList<string> languages = atelier.CAtelierCatalog.CCatalogLanguageRead();
            Assert.Contains("English", languages);
            TEditorFrameSend(library, TLibrary.TLibraryDown);
            Assert.Contains(TEditorFrameDraw(library), static text => text.BCanvasTextValue.StartsWith("English v"));
            Assert.Equal("Editor.Language", find(ConsoleKey.Enter)?.BKeymapRowLabel);

            TEditorFrameSend(library, TLibrary.TLibraryEnter);

            Assert.Equal("Editor.Pick", find(ConsoleKey.Enter)?.BKeymapRowLabel);
            Assert.Equal("Editor.Shut", find(ConsoleKey.Escape)?.BKeymapRowLabel);
            IReadOnlyList<BCanvasText> texts = TEditorFrameDraw(library);
            Assert.Contains(texts, static text => text.BCanvasTextRole == "shadow");
            Assert.Equal(Math.Min(languages.Count, 8), texts.Count(text => text.BCanvasTextRole is "menu" or "lit"
                && languages.Any(name => text.BCanvasTextValue.Trim() == name)));
            Assert.Equal(" English", Assert.Single(texts, static text => text.BCanvasTextRole == "lit"
                && text.BCanvasTextValue.StartsWith(' ')).BCanvasTextValue.TrimEnd());

            string other = languages.First(static name => name != "English");
            TEditorFrameSend(library, new ConsoleKeyInfo(other[0], ConsoleKey.A, false, false, false));

            Assert.StartsWith(other[..1], Assert.Single(TEditorFrameDraw(library), static text =>
                text.BCanvasTextRole == "lit" && text.BCanvasTextValue.StartsWith(' ')).BCanvasTextValue.Trim());
        });
    }

    [Fact]
    public void EditorKeymap_LightLeavingTheLanguageRow_ClosesItsOpenList()
    {
        TEditorHeadRun((library, _, find) =>
        {
            TEditorFrameSend(library, TLibrary.TLibraryDown);
            TEditorFrameSend(library, TLibrary.TLibraryEnter);
            Assert.Contains(TEditorFrameDraw(library), static text => text.BCanvasTextRole == "shadow");

            TEditorFrameSend(library, new ConsoleKeyInfo('\0', ConsoleKey.End, false, false, true));

            Assert.DoesNotContain(TEditorFrameDraw(library), static text => text.BCanvasTextRole == "shadow");
            Assert.NotEqual("Editor.Pick", find(ConsoleKey.Enter)?.BKeymapRowLabel);
            Assert.Null(find(ConsoleKey.Escape));
        });
    }

    [Fact]
    public void SpeakerOpen_SecondOpenDuringTheLoad_LoadsOnce()
    {
        TEditorWindowRun((_, atelier, _) =>
        {
            int loads = 0;
            TaskCompletionSource<IReadOnlyList<string>> pending = new();
            BSpeaker speaker = new(
                atelier.CAtelierInputCreate(TEnvoyFake.TEnvoyCreate(false, [])).CEditorEntry,
                () =>
                {
                    loads++;
                    return pending.Task;
                },
                static () => { });

            _ = speaker.BSpeakerOpen();
            _ = speaker.BSpeakerOpen();
            pending.SetResult(["English", "French"]);
            TEditorHeadSettle();

            Assert.Equal(1, loads);
            Assert.NotNull(speaker.BSpeakerDrop);
        });
    }

    [Fact]
    public void SpeakerOpen_CloseDuringTheLoad_LeavesTheListShut()
    {
        TEditorWindowRun((_, atelier, _) =>
        {
            TaskCompletionSource<IReadOnlyList<string>> pending = new();
            BSpeaker speaker = new(
                atelier.CAtelierInputCreate(TEnvoyFake.TEnvoyCreate(false, [])).CEditorEntry,
                () => pending.Task,
                static () => { });

            _ = speaker.BSpeakerOpen();
            speaker.BSpeakerClose();
            pending.SetResult(["English", "French"]);
            TEditorHeadSettle();

            Assert.Null(speaker.BSpeakerDrop);
        });
    }

    [Fact]
    public void EditorKeymap_EnterOnALitLanguage_SetsTheDraftLanguageAndClosesTheList()
    {
        TEditorHeadRun((library, atelier, find) =>
        {
            string other = atelier.CAtelierCatalog.CCatalogLanguageRead().First(static name => name != "English");
            TEditorFrameSend(library, TLibrary.TLibraryDown);
            TEditorFrameSend(library, TLibrary.TLibraryEnter);
            while (Assert.Single(TEditorFrameDraw(library), static text => text.BCanvasTextRole == "lit"
                && text.BCanvasTextValue.StartsWith(' ')).BCanvasTextValue.Trim() != other)
            {
                TEditorFrameSend(library, TLibrary.TLibraryDown);
            }

            TEditorFrameSend(library, TLibrary.TLibraryEnter);

            Assert.Equal("Editor.Language", find(ConsoleKey.Enter)?.BKeymapRowLabel);
            IReadOnlyList<BCanvasText> texts = TEditorFrameDraw(library);
            Assert.DoesNotContain(texts, static text => text.BCanvasTextRole == "shadow");
            Assert.Contains(texts, text => text.BCanvasTextValue.StartsWith($"{other} v"));
            Assert.Contains(texts, static text => text.BCanvasTextValue.Contains("Unsaved"));
        });
    }

    [Fact]
    public void EditorKeymap_EscapeOnAnOpenList_ShutsOnlyTheList()
    {
        TEditorHeadRun((library, _, find) =>
        {
            TEditorFrameSend(library, TLibrary.TLibraryDown);
            TEditorFrameSend(library, TLibrary.TLibraryEnter);

            TEditorFrameSend(library, new ConsoleKeyInfo('\u001b', ConsoleKey.Escape, false, false, false));

            Assert.DoesNotContain(TEditorFrameDraw(library), static text => text.BCanvasTextRole == "shadow");
            Assert.Equal("Editor.Language", find(ConsoleKey.Enter)?.BKeymapRowLabel);
            Assert.DoesNotContain(TEditorFrameDraw(library), static text => text.BCanvasTextValue.Contains("Unsaved"));
        });
    }

    [Fact]
    public void EditorKeymap_KeysOnTheEsteemRow_MoveSetAndClearTheGraspThroughConduct()
    {
        TEditorHeadRun((library, _, find) =>
        {
            TEditorFrameSend(library, TLibrary.TLibraryDown);
            TEditorFrameSend(library, TLibrary.TLibraryDown);
            Assert.Equal("Grasp.Mark", find(ConsoleKey.RightArrow)?.BKeymapRowLabel);
            Assert.Contains(TEditorFrameDraw(library), static text => text.BCanvasTextValue.Contains("Not rated"));

            TEditorFrameSend(library, TEditorHeadRight);
            Assert.Contains(TEditorFrameDraw(library), static text => text.BCanvasTextValue.Contains("Never seen it"));

            TEditorFrameSend(library, TEditorHeadOne);
            Assert.Contains(TEditorFrameDraw(library), static text => text.BCanvasTextValue.Contains("Seen it before"));

            TEditorFrameSend(library, TEditorHeadOne);
            Assert.Contains(TEditorFrameDraw(library), static text => text.BCanvasTextValue.Contains("Not rated"));
        });
    }

    [Fact]
    public void EditorKeymap_F6WhileEditingAStoredEntry_FlipsTheFavoriteMark()
    {
        TEditorHeadRun((library, _, _) =>
        {
            Assert.Contains(TEditorFrameDraw(library), static text =>
                text.BCanvasTextValue == "  Favorite" && text.BCanvasTextRole == "muted");

            TEditorFrameSend(library, TEditorHeadSix);

            Assert.Contains(TEditorFrameDraw(library), static text =>
                text.BCanvasTextValue == "* Favorite" && text.BCanvasTextRole == "favorite");

            TEditorFrameSend(library, TEditorHeadSix);

            Assert.Contains(TEditorFrameDraw(library), static text => text.BCanvasTextValue == "  Favorite");
        });
    }

    [Fact]
    public void EditorKeymap_FreshDraft_DimsFavoriteAndGrasp()
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

                Assert.Equal("disabled", Assert.Single(
                    texts, static text => text.BCanvasTextValue == "  Favorite").BCanvasTextRole);
                Assert.Equal(
                    BKeymapLive.BKeymapLiveDim,
                    drawn.BEditorKeymap.BKeymapFind(BKeymapFocus.BKeymapFocusWindow, TEditorHeadSix)!
                        .BKeymapRowLive());
            });
    }

    private static void TEditorHeadSettle()
    {
        BLoop loop = Assert.IsType<BLoop>(SynchronizationContext.Current);
        TaskCompletionSource settled = new();
        loop.BLoopDefer(settled.SetResult);
        loop.BLoopRun(settled.Task.WaitAsync(TimeSpan.FromSeconds(10)));
    }

    private static void TEditorHeadRun(Action<BLibrary, CAtelier, Func<ConsoleKey, BKeymapRow?>> check)
    {
        TLibrary.TLibraryRun(
            ["fire"],
            true,
            (library, atelier, _) =>
            {
                ConsoleKeyInfo f4 = new('\0', ConsoleKey.F4, false, false, false);
                library.BLibraryObserve(library.BLibraryKeymapFocus, TLibrary.TLibraryEnter);
                library.BLibraryKeymap.BKeymapFind(BKeymapFocus.BKeymapFocusWindow, f4)!.BKeymapRowAction(f4);
                check(library, atelier, key => library.BLibraryKeymap.BKeymapFind(
                    library.BLibraryKeymapFocus, new ConsoleKeyInfo('\0', key, false, false, false)));
            });
    }
}
