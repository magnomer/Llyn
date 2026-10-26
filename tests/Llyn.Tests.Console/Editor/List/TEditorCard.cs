using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;
using Llyn.UIDemeanor;
using Llyn.UITerminal;
using Xunit;
using static Llyn.Tests.TEditorFrame;
using static Llyn.Tests.TEditorInlet;

namespace Llyn.Tests;

public sealed class TEditorCard
{
    private static readonly ConsoleKeyInfo TEditorCardNext = new('\0', ConsoleKey.PageDown, false, false, true);

    private static readonly ConsoleKeyInfo TEditorCardEscape = new('\u001b', ConsoleKey.Escape, false, false, false);

    private static readonly ConsoleKeyInfo TEditorCardBadge = new('#', ConsoleKey.D3, true, false, false);

    [Fact]
    public void EditorKeymap_ListOfOneCard_OffersNoMoveNorBadgeAndKeepsTheCard()
    {
        TEditorCardRun(["a meaning"], (library, find) =>
        {
            Assert.Null(find(ConsoleKey.UpArrow, ConsoleModifiers.Control));
            Assert.Null(find(ConsoleKey.DownArrow, ConsoleModifiers.Control));
            Assert.Equal("Card.Remove", find(ConsoleKey.Delete, 0)?.BKeymapRowLabel);

            TEditorFrameSend(library, new ConsoleKeyInfo('\0', ConsoleKey.Delete, false, false, false));

            IReadOnlyList<BCanvasText> texts = TEditorFrameDraw(library);
            Assert.DoesNotContain(texts, TEditorInletMatch);
            Assert.DoesNotContain(texts, static text => text.BCanvasTextValue == "[");
            Assert.Single(texts, TEditorCardMatch);
        });
    }

    [Fact]
    public void EditorObserve_HashOnAListOfOneCard_StartsTheTitleWithIt()
    {
        TEditorCardRun(["a meaning"], (library, _) =>
        {
            TEditorFrameSend(library, TEditorCardBadge);

            IReadOnlyList<BCanvasText> texts = TEditorFrameDraw(library);
            Assert.DoesNotContain(texts, static text => text.BCanvasTextValue == "[");
            Assert.Contains(
                texts, static text => text.BCanvasTextRole == "field" && text.BCanvasTextValue.Contains('#'));
        });
    }

    [Fact]
    public void EditorKeymap_HashTypedOnAnotherLayout_OpensTheBadgeByItsSign()
    {
        TEditorCardRun(["a meaning", "b meaning"], (library, _) =>
        {
            TEditorFrameSend(library, new ConsoleKeyInfo('#', ConsoleKey.D3, false, true, true));

            Assert.Contains(TEditorFrameDraw(library), static text => text.BCanvasTextValue == "[");
        });
    }

    [Fact]
    public void EditorKeymap_ShiftThreeTypingAnotherSign_OpensNoBadge()
    {
        TEditorCardRun(["a meaning", "b meaning"], (library, _) =>
        {
            TEditorFrameSend(library, new ConsoleKeyInfo('3', ConsoleKey.D3, true, false, false));

            Assert.DoesNotContain(TEditorFrameDraw(library), static text => text.BCanvasTextValue == "[");
        });
    }

    [Fact]
    public void EditorKeymap_RefusedMoveThenUndo_KeepsTheLitBarInPlace()
    {
        TEditorCardRun(["a meaning", "b meaning"], (library, _) =>
        {
            ConsoleKeyInfo down = new('\0', ConsoleKey.DownArrow, false, false, true);
            TEditorCardSend(library, "x");
            TEditorFrameSend(library, down);
            TEditorFrameSend(library, down);
            TEditorFrameSend(library, new ConsoleKeyInfo('\u001a', ConsoleKey.Z, false, false, true));

            BCanvasText lit = Assert.Single(TEditorFrameDraw(library),
                static text => text.BCanvasTextRole == "lit" && TEditorCardMatch(text));
            Assert.StartsWith("-- 2 ", lit.BCanvasTextValue);
        });
    }

    [Fact]
    public void EditorReset_BadgeOpen_ClosesThePositionInConduct()
    {
        TEditorCardStart((editor, send) =>
        {
            long card = editor.CEditorWalk[0][editor.CEditorWalkCard[0][0]].CStepCard!.CCardDraftId;
            send(TEditorCardBadge);

            editor.CEditorEntryOpen(null);

            Assert.False(editor.CEditorList.CCardPositionSettle(false, card, "2"));
        });
    }

    [Fact]
    public void EditorKeymap_EscapeInTheBadge_ClosesThePositionInConduct()
    {
        TEditorCardStart((editor, send) =>
        {
            long card = editor.CEditorWalk[0][editor.CEditorWalkCard[0][0]].CStepCard!.CCardDraftId;
            send(TEditorCardBadge);
            send(TEditorCardEscape);

            Assert.False(editor.CEditorList.CCardPositionSettle(false, card, "2"));
        });
    }

    [Fact]
    public void EditorKeymap_TooLargeNumberInTheBadge_SettlesTheCardLast()
    {
        TEditorCardRun(["a meaning", "b meaning"], (library, _) =>
        {
            TEditorCardSend(library, "x");
            TEditorFrameSend(library, TEditorCardBadge);
            TEditorFrameSend(library, new ConsoleKeyInfo('9', ConsoleKey.D9, false, false, false));
            TEditorFrameSend(library, TLibrary.TLibraryEnter);

            BCanvasText header = Assert.Single(
                TEditorFrameDraw(library), static text => text.BCanvasTextValue.StartsWith("-- 2 - x"));
            Assert.Equal("lit", header.BCanvasTextRole);
        });
    }

    [Fact]
    public void EditorKeymap_ControlPageOffAHeader_LightsTheNeighbourCards()
    {
        TEditorCardRun(["a meaning", "b meaning"], (library, find) =>
        {
            TEditorFrameSend(library, TLibrary.TLibraryDown);
            Assert.Equal("Editor.Card", find(ConsoleKey.PageUp, ConsoleModifiers.Control)?.BKeymapRowLabel);

            TEditorFrameSend(library, TEditorCardNext);

            Assert.StartsWith("-- 2 ", TEditorCardFind(library).BCanvasTextValue);

            TEditorFrameSend(library, TLibrary.TLibraryDown);
            TEditorFrameSend(library, new ConsoleKeyInfo('\0', ConsoleKey.PageUp, false, false, true));

            Assert.StartsWith("-- 2 ", TEditorCardFind(library).BCanvasTextValue);
        });
    }

    [Fact]
    public void EditorKeymap_FoldOnAStoredCard_LeavesTheEntryUnaltered()
    {
        TEditorCardRun(["a meaning"], (library, _) =>
        {
            TEditorFrameSend(library, TLibrary.TLibraryEnter);

            Assert.DoesNotContain(TEditorFrameDraw(library), static text => text.BCanvasTextValue == "Unsaved");
        });
    }

    [Fact]
    public void EditorDraw_StoredCardWithTheSingleGlyphSet_DrawsTheFoldMarkFromTheSet()
    {
        TEditorCardRun(["a meaning"], (library, _) =>
        {
            IReadOnlyDictionary<string, IReadOnlyList<string>> glyphs = new ABootstrap().BBlueprintGlyphs["single"];
            IReadOnlyList<string> marks = glyphs["Fold"];
            string rule = glyphs["Rule"][1];
            Predicate<BCanvasText> header = text => text.BCanvasTextValue.StartsWith($"{rule}{rule} ");
            BCanvasText open = Assert.Single(TEditorCardDraw(library), header);
            Assert.EndsWith(" " + marks[0], open.BCanvasTextValue.TrimEnd());

            TEditorFrameSend(library, TLibrary.TLibraryEnter);

            BCanvasText shut = Assert.Single(TEditorCardDraw(library), header);
            Assert.EndsWith(" " + marks[1], shut.BCanvasTextValue.TrimEnd());
        });
    }

    [Fact]
    public void EditorKeymap_InsertOnAHeader_AddsAnUnstoredCardAndOpensItsTitle()
    {
        TEditorCardRun(["a meaning"], (library, _) =>
        {
            TEditorFrameSend(library, new ConsoleKeyInfo('\0', ConsoleKey.Insert, false, false, false));

            Assert.Contains(TEditorFrameDraw(library), static text => text.BCanvasTextRole == "field");

            TEditorInletSend(library, "blaze");
            TEditorFrameSend(library, TLibrary.TLibraryEnter);

            IReadOnlyList<BCanvasText> texts = TEditorFrameDraw(library);
            BCanvasText header = Assert.Single(texts, static text => text.BCanvasTextValue.StartsWith("-- 2 - blaze"));
            Assert.Equal("lit", header.BCanvasTextRole);
            Assert.False(header.BCanvasTextValue.TrimEnd().EndsWith(" v"));
            Assert.False(header.BCanvasTextValue.TrimEnd().EndsWith(" >"));
        });
    }

    [Fact]
    public void EditorKeymap_ControlDownOnAHeader_MovesTheCardAndTheLitBarFollows()
    {
        TEditorCardRun(["a meaning", "b meaning"], (library, find) =>
        {
            TEditorCardSend(library, "x");
            Assert.Equal("Card.Move", find(ConsoleKey.DownArrow, ConsoleModifiers.Control)?.BKeymapRowLabel);

            TEditorFrameSend(library, new ConsoleKeyInfo('\0', ConsoleKey.DownArrow, false, false, true));

            BCanvasText header = Assert.Single(
                TEditorFrameDraw(library), static text => text.BCanvasTextValue.StartsWith("-- 2 - x"));
            Assert.Equal("lit", header.BCanvasTextRole);
        });
    }

    [Fact]
    public void EditorKeymap_TypedNumberInTheBadge_SettlesTheCardThere()
    {
        TEditorCardRun(["a meaning", "b meaning"], (library, _) =>
        {
            TEditorCardSend(library, "x");
            TEditorFrameSend(library, TEditorCardBadge);

            Assert.Contains(TEditorFrameDraw(library), static text => text.BCanvasTextValue == "[");

            TEditorFrameSend(library, new ConsoleKeyInfo('2', ConsoleKey.D2, false, false, false));
            TEditorFrameSend(library, TLibrary.TLibraryEnter);

            BCanvasText header = Assert.Single(
                TEditorFrameDraw(library), static text => text.BCanvasTextValue.StartsWith("-- 2 - x"));
            Assert.Equal("lit", header.BCanvasTextRole);
        });
    }

    [Fact]
    public void EditorKeymap_EscapeInTheBadge_KeepsTheOldNumber()
    {
        TEditorCardRun(["a meaning", "b meaning"], (library, _) =>
        {
            TEditorCardSend(library, "x");
            TEditorFrameSend(library, TEditorCardBadge);
            TEditorFrameSend(library, new ConsoleKeyInfo('2', ConsoleKey.D2, false, false, false));
            TEditorFrameSend(library, TEditorCardEscape);

            IReadOnlyList<BCanvasText> texts = TEditorFrameDraw(library);
            Assert.DoesNotContain(texts, static text => text.BCanvasTextValue == "[");
            Assert.Single(texts, static text => text.BCanvasTextValue.StartsWith("-- 1 - x"));
        });
    }

    [Fact]
    public void EditorKeymap_DeleteOnAHeaderOfTwo_RemovesTheCard()
    {
        TEditorCardRun(["a meaning", "b meaning"], (library, _) =>
        {
            Assert.Equal(2, TEditorFrameDraw(library).Count(TEditorCardMatch));

            TEditorFrameSend(library, new ConsoleKeyInfo('\0', ConsoleKey.Delete, false, false, false));

            Assert.Single(TEditorFrameDraw(library), TEditorCardMatch);
        });
    }

    [Fact]
    public void EditorDraw_FoldedUntitledMeaning_PeeksItsDefinitionInTheHeader()
    {
        TEditorCardRun(["a meaning"], (library, _) =>
        {
            TEditorFrameSend(library, TLibrary.TLibraryEnter);

            BCanvasText header = Assert.Single(TEditorFrameDraw(library), TEditorCardMatch);
            Assert.StartsWith("-- 1 - a meaning ", header.BCanvasTextValue);
            Assert.EndsWith(" >", header.BCanvasTextValue.TrimEnd());
        });
    }

    [Fact]
    public void EditorKeymap_EnterOnTheAddRow_AddsACollocationOnTheCollocationTab()
    {
        TEditorCardRun(["a meaning"], (library, find) =>
        {
            TEditorFrameSend(library, new ConsoleKeyInfo('2', ConsoleKey.D2, false, true, false));
            TEditorFrameSend(library, new ConsoleKeyInfo('\0', ConsoleKey.End, false, false, true));
            Assert.Equal("Input.CollocationAdd", find(ConsoleKey.Enter, 0)?.BKeymapRowLabel);
            Assert.Contains(TEditorFrameDraw(library), static text => text.BCanvasTextValue.StartsWith("+ "));

            TEditorFrameSend(library, TLibrary.TLibraryEnter);

            Assert.Contains(TEditorFrameDraw(library), static text => text.BCanvasTextRole == "field");
            Assert.Equal("Editor.Close", find(ConsoleKey.Enter, 0)?.BKeymapRowLabel);
        });
    }

    private static void TEditorCardSend(BLibrary library, string title)
    {
        TEditorInletSend(library, title);
        TEditorFrameSend(library, TLibrary.TLibraryEnter);
    }

    private static bool TEditorCardMatch(BCanvasText text)
    {
        return text.BCanvasTextValue.StartsWith("-- ");
    }

    private static BCanvasText TEditorCardFind(BLibrary library)
    {
        return Assert.Single(
            TEditorFrameDraw(library), static text => text.BCanvasTextRole == "lit" && TEditorCardMatch(text));
    }

    private static IReadOnlyList<BCanvasText> TEditorCardDraw(BLibrary library)
    {
        BBlueprintPart window = BContract.BContractBlueprintFind(new ABootstrap(), "AWindow");
        BBlueprint.BBlueprintPlace(window, 0, 0, 80, 24);
        return [.. library.BLibraryDraw(
            new ABootstrap().BBlueprintGlyphs["single"], window, true, new BCanvasWidth(false))];
    }

    private static void TEditorCardStart(Action<CEditor, Action<ConsoleKeyInfo>> check)
    {
        TEditorWindowRun((_, atelier, _) =>
        {
            CEditor editor = atelier.CAtelierInputCreate(TEnvoyFake.TEnvoyCreate(false, []));
            BLocalizationCatalog catalog = new();
            catalog.BLocalizationCatalogApply(TInterface.TLocalizationLoad("en"));
            BEditor face = new(
                catalog, editor, "Navigation.Input", static () => true, static () => true, static () => { });
            editor.CEditorEntryOpen(null);
            editor.CEditorList.CCardMeaningAdd();
            editor.CEditorList.CCardMeaningAdd();
            Action<ConsoleKeyInfo> send = key =>
            {
                if (face.BEditorKeymap.BKeymapFind(BKeymapFocus.BKeymapFocusEditor, key) is BKeymapRow row)
                {
                    row.BKeymapRowAction(key);
                    return;
                }

                _ = face.BEditorObserve(key);
            };
            send(TEditorCardNext);
            check(editor, send);
        });
    }

    private static void TEditorCardRun(
        IReadOnlyList<string> definitions, Action<BLibrary, Func<ConsoleKey, ConsoleModifiers, BKeymapRow?>> check)
    {
        TEditorFrameRun(
            [],
            (library, find) =>
            {
                TEditorFrameSend(library, TEditorCardNext);
                check(library, find);
            },
            null,
            engine => engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
                "fire",
                "English",
                string.Empty,
                string.Empty,
                [.. definitions.Select(static (text, at) => TInterface.TCardCreate(text, at + 1))],
                [])));
    }
}
