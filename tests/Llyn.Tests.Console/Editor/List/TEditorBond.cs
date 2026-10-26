using Llyn.Core;
using Llyn.ShellEngine;
using Llyn.UIDemeanor;
using Xunit;
using static Llyn.Tests.TEditorFrame;
using static Llyn.Tests.TEditorInlet;

namespace Llyn.Tests;

public sealed class TEditorBond
{
    private static readonly ConsoleKeyInfo TEditorBondNext = new('\0', ConsoleKey.PageDown, false, false, true);

    private static readonly ConsoleKeyInfo TEditorBondStack = new('2', ConsoleKey.D2, false, true, false);

    private static readonly ConsoleKeyInfo TEditorBondLeft = new('\0', ConsoleKey.LeftArrow, false, false, false);

    private static readonly ConsoleKeyInfo TEditorBondUnlink = new('\0', ConsoleKey.Delete, false, false, false);

    private static readonly ConsoleKeyInfo TEditorBondEscape = new('\u001b', ConsoleKey.Escape, false, false, false);

    [Fact]
    public void EditorDraw_CollocationCard_ShowsTheExpressionAndTheLinkedMeaningsRow()
    {
        TEditorBondRun((library, _) =>
        {
            IReadOnlyList<BCanvasText> texts = TEditorFrameDraw(library);

            Assert.Contains(texts, static text => text.BCanvasTextValue.Contains("set off", StringComparison.Ordinal));
            BCanvasText caption = Assert.Single(texts, static text => text.BCanvasTextValue == "Meanings");
            BCanvasText chip = Assert.Single(texts, TEditorBondMatch);
            Assert.Equal("link", chip.BCanvasTextRole);
            Assert.Contains("to place", chip.BCanvasTextValue, StringComparison.Ordinal);
            BCanvasText adder = Assert.Single(texts, static text => text.BCanvasTextValue == "+ link");
            Assert.Equal("lit", adder.BCanvasTextRole);
            Assert.Equal(caption.BCanvasTextRow, adder.BCanvasTextRow);
            Assert.True(adder.BCanvasTextColumn > chip.BCanvasTextColumn);
        });
    }

    [Fact]
    public void EditorKeymap_EnterOnTheLinkMark_OpensEveryMeaningIndentedByDepth()
    {
        TEditorBondRun((library, find) =>
        {
            Assert.Equal("Card.SenseLink", find(ConsoleKey.Enter)?.BKeymapRowLabel);

            TEditorFrameSend(library, TLibrary.TLibraryEnter);

            IReadOnlyList<BCanvasText> rows = TEditorBondRead(library);
            BCanvasText root = Assert.Single(rows, static text =>
                text.BCanvasTextValue.Contains("to place", StringComparison.Ordinal)
                && !text.BCanvasTextValue.Contains("firmly", StringComparison.Ordinal));
            BCanvasText child = Assert.Single(
                rows, static text => text.BCanvasTextValue.Contains("to place firmly", StringComparison.Ordinal));
            Assert.Contains(rows, static text => text.BCanvasTextValue.Contains("to harden", StringComparison.Ordinal));
            Assert.True(child.BCanvasTextValue.Length - child.BCanvasTextValue.TrimStart().Length
                > root.BCanvasTextValue.Length - root.BCanvasTextValue.TrimStart().Length);
            Assert.Equal("Editor.Pick", find(ConsoleKey.Enter)?.BKeymapRowLabel);
            Assert.Equal("Window.Help.Item", find(ConsoleKey.DownArrow)?.BKeymapRowLabel);
            Assert.Equal("Editor.Shut", find(ConsoleKey.Escape)?.BKeymapRowLabel);
        });
    }

    [Fact]
    public void EditorKeymap_PickInTheMeaningList_AttachesTheMeaningAsAChip()
    {
        TEditorBondRun((library, _) =>
        {
            TEditorFrameSend(library, TLibrary.TLibraryEnter);
            Func<bool> reached = () => TEditorBondRead(library).Any(static text =>
                text.BCanvasTextRole == "lit" && text.BCanvasTextValue.Contains("to harden", StringComparison.Ordinal));
            for (int step = 0; step < 4 && !reached(); step++)
            {
                TEditorFrameSend(library, TLibrary.TLibraryDown);
            }

            TEditorFrameSend(library, TLibrary.TLibraryEnter);

            IReadOnlyList<BCanvasText> texts = TEditorFrameDraw(library);
            Assert.Empty(TEditorBondRead(library));
            BCanvasText[] chips = [.. texts.Where(TEditorBondMatch)];
            Assert.Equal(2, chips.Length);
            Assert.Contains(chips, static chip => chip.BCanvasTextValue.Contains("to place", StringComparison.Ordinal));
            Assert.Contains(
                chips, static chip => chip.BCanvasTextValue.Contains("to harden", StringComparison.Ordinal));
        });
    }

    [Fact]
    public void BondDraw_ChipsPastWidth_FlowsBesideCaption()
    {
        TEditorBondRun((library, _) =>
        {
            TEditorBondAttach(library);

            IReadOnlyList<BCanvasText> texts = TEditorFrameDraw(library);
            BCanvasText caption = Assert.Single(texts, static text => text.BCanvasTextValue == "Meanings");
            BCanvasText first = Assert.Single(texts, static text =>
                TEditorBondMatch(text) && text.BCanvasTextValue.Contains("to place", StringComparison.Ordinal));
            BCanvasText second = Assert.Single(texts, static text =>
                TEditorBondMatch(text) && text.BCanvasTextValue.Contains("to harden", StringComparison.Ordinal));
            BCanvasText adder = Assert.Single(texts, static text => text.BCanvasTextValue == "+ link");
            Assert.Equal(caption.BCanvasTextRow, first.BCanvasTextRow);
            Assert.Equal(first.BCanvasTextRow + 1, second.BCanvasTextRow);
            Assert.Equal(first.BCanvasTextColumn, second.BCanvasTextColumn);
            Assert.Equal(second.BCanvasTextRow, adder.BCanvasTextRow);
            Assert.True(adder.BCanvasTextColumn > second.BCanvasTextColumn);
        });
    }

    [Fact]
    public void EditorKeymap_UndoShrinkingTheChipsBeneathTheLight_KeepsAChipOrTheLinkMarkLit()
    {
        TEditorBondRun((library, find) =>
        {
            TEditorBondAttach(library);
            TEditorFrameSend(library, TEditorBondLeft);
            TEditorFrameSend(library, TEditorBondLeft);
            TEditorFrameSend(library, new ConsoleKeyInfo('\u001a', ConsoleKey.Z, false, false, true));

            IReadOnlyList<BCanvasText> texts = TEditorFrameDraw(library);
            Assert.Single(texts, TEditorBondMatch);
            Assert.Contains(texts, static text => text.BCanvasTextRole == "lit"
                && (TEditorBondMatch(text) || text.BCanvasTextValue == "+ link"));
            Assert.True(find(ConsoleKey.Enter) is not null || find(ConsoleKey.Delete) is not null);
        });
    }

    [Fact]
    public void EditorKeymap_CardSeekAwayAndBack_LightsTheLinkMarkAgain()
    {
        TEditorBondRun((library, find) =>
        {
            TEditorFrameSend(library, TEditorBondLeft);
            TEditorFrameSend(library, new ConsoleKeyInfo('\0', ConsoleKey.PageUp, false, false, true));
            TEditorFrameSend(library, TLibrary.TLibraryDown);
            TEditorFrameSend(library, TLibrary.TLibraryDown);

            IReadOnlyList<BCanvasText> texts = TEditorFrameDraw(library);
            Assert.Equal("link", Assert.Single(texts, TEditorBondMatch).BCanvasTextRole);
            Assert.Equal("lit", Assert.Single(texts, static text => text.BCanvasTextValue == "+ link").BCanvasTextRole);
            Assert.Equal("Card.SenseLink", find(ConsoleKey.Enter)?.BKeymapRowLabel);
        });
    }

    private static void TEditorBondAttach(BLibrary library)
    {
        TEditorFrameSend(library, TLibrary.TLibraryEnter);
        Func<bool> reached = () => TEditorBondRead(library).Any(static text =>
            text.BCanvasTextRole == "lit" && text.BCanvasTextValue.Contains("to harden", StringComparison.Ordinal));
        for (int step = 0; step < 4 && !reached(); step++)
        {
            TEditorFrameSend(library, TLibrary.TLibraryDown);
        }

        TEditorFrameSend(library, TLibrary.TLibraryEnter);
    }

    [Fact]
    public void EditorKeymap_EscapeInTheMeaningList_ClosesItAndLinksNothing()
    {
        TEditorBondRun((library, find) =>
        {
            TEditorFrameSend(library, TLibrary.TLibraryEnter);
            TEditorFrameSend(library, TEditorBondEscape);

            Assert.Empty(TEditorBondRead(library));
            Assert.Single(TEditorFrameDraw(library), TEditorBondMatch);
            Assert.Equal("Card.SenseLink", find(ConsoleKey.Enter)?.BKeymapRowLabel);
        });
    }

    [Fact]
    public void EditorKeymap_LeftThenDeleteOnTheLinkedRow_DetachesTheLitChip()
    {
        TEditorBondRun((library, find) =>
        {
            Assert.Null(find(ConsoleKey.Delete));
            Assert.Equal("Card.SenseLight", find(ConsoleKey.LeftArrow)?.BKeymapRowLabel);

            TEditorFrameSend(library, TEditorBondLeft);

            BCanvasText chip = Assert.Single(TEditorFrameDraw(library), TEditorBondMatch);
            Assert.Equal("lit", chip.BCanvasTextRole);
            Assert.Null(find(ConsoleKey.Enter));
            Assert.Equal("Card.SenseDetach", find(ConsoleKey.Delete)?.BKeymapRowLabel);

            TEditorFrameSend(library, TEditorBondUnlink);

            IReadOnlyList<BCanvasText> texts = TEditorFrameDraw(library);
            Assert.DoesNotContain(texts, TEditorBondMatch);
            Assert.Equal("lit", Assert.Single(texts, static text => text.BCanvasTextValue == "+ link").BCanvasTextRole);
            Assert.Null(find(ConsoleKey.LeftArrow));
        });
    }

    [Fact]
    public void EditorKeymap_TypingOnTheExpressionRow_OpensItsFieldAndKeepsTheText()
    {
        TEditorBondRun((library, find) =>
        {
            TEditorFrameSend(library, new ConsoleKeyInfo('\0', ConsoleKey.UpArrow, false, false, false));
            Assert.Equal("Editor.Open", find(ConsoleKey.Enter)?.BKeymapRowLabel);

            TEditorInletSend(library, "x");

            Assert.Contains(TEditorFrameDraw(library), TEditorInletMatch);
            Assert.Equal("Editor.Close", find(ConsoleKey.Enter)?.BKeymapRowLabel);

            TEditorFrameSend(library, TLibrary.TLibraryEnter);

            IReadOnlyList<BCanvasText> texts = TEditorFrameDraw(library);
            Assert.DoesNotContain(texts, TEditorInletMatch);
            Assert.Contains(texts, static text => text.BCanvasTextValue.Contains("set offx", StringComparison.Ordinal));
        });
    }

    [Fact]
    public void EditorKeymap_MeaningCard_EditsTheDefinitionAndShowsNoLinkedMeaningsRow()
    {
        TEditorFrameRun(
            [],
            (library, find) =>
            {
                TEditorFrameSend(library, TEditorBondNext);
                TEditorFrameSend(library, TLibrary.TLibraryDown);

                Assert.DoesNotContain(TEditorFrameDraw(library), static text => text.BCanvasTextValue == "Meanings");
                Assert.Equal("Editor.Open", find(ConsoleKey.Enter, 0)?.BKeymapRowLabel);
                Assert.Null(find(ConsoleKey.LeftArrow, 0));

                TEditorFrameSend(library, TLibrary.TLibraryEnter);
                TEditorInletSend(library, "s");
                TEditorFrameSend(library, TLibrary.TLibraryEnter);

                BCanvasText row = Assert.Single(TEditorFrameDraw(library),
                    static text => text.BCanvasTextValue.Contains("a meanings", StringComparison.Ordinal));
                Assert.StartsWith("   a meanings", row.BCanvasTextValue);
            },
            null,
            static engine => engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
                "fire", "English", string.Empty, string.Empty, [TInterface.TCardCreate("a meaning", 1)], [])));
    }

    private static bool TEditorBondMatch(BCanvasText text)
    {
        return text.BCanvasTextValue.StartsWith('[') && text.BCanvasTextValue.EndsWith(']')
            && text.BCanvasTextValue.Contains("to ", StringComparison.Ordinal);
    }

    private static IReadOnlyList<BCanvasText> TEditorBondRead(BLibrary library)
    {
        return [.. TEditorFrameDraw(library).Where(static text => text.BCanvasTextRole is "menu" or "lit"
            && text.BCanvasTextValue.Contains("to ", StringComparison.Ordinal)
            && !text.BCanvasTextValue.StartsWith('['))];
    }

    private static void TEditorBondRun(Action<BLibrary, Func<ConsoleKey, BKeymapRow?>> check)
    {
        TEditorFrameRun(
            [],
            (library, find) =>
            {
                TEditorFrameSend(library, TEditorBondStack);
                TEditorFrameSend(library, TEditorBondNext);
                TEditorFrameSend(library, TLibrary.TLibraryDown);
                TEditorFrameSend(library, TLibrary.TLibraryDown);
                check(library, key => find(key, 0));
            },
            null,
            static engine =>
            {
                LCardDraft firmly = TInterface.TCardCreate("to place firmly", 1) with
                {
                    LCardDraftId = TInterface.TIdentityCreate(),
                };
                LCardDraft root = TInterface.TCardCreate("to place", 1) with
                {
                    LCardDraftId = TInterface.TIdentityCreate(),
                    LCardDraftChild = [firmly],
                };
                LCardDraft other = TInterface.TCardCreate("to harden", 2) with
                {
                    LCardDraftId = TInterface.TIdentityCreate(),
                };
                LCardDraft linked = TInterface.TCardDraftCreate(
                    LStateValue.LStateValueUnspecified,
                    TInterfaceState.TStateValueCreate("set off"),
                    LStateValue.LStateValueUnspecified,
                    [], [], [], [], [], 1) with
                {
                    LCardDraftSense = [root.LCardDraftId],
                };
                _ = engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
                    "set", "English", string.Empty, string.Empty, [root, other], [linked]));
            });
    }
}
