using Llyn.Core;
using Llyn.ShellEngine;
using Llyn.UIDemeanor;
using Xunit;
using static Llyn.Tests.TEditorFrame;

namespace Llyn.Tests;

public sealed class TEditorLink
{
    private const int TEditorLinkSide = 40;

    private const int TEditorLinkPart = 4;

    private static readonly ConsoleKeyInfo TEditorLinkEscape = new('\u001b', ConsoleKey.Escape, false, false, false);

    private static readonly ConsoleKeyInfo TEditorLinkBack = new('\b', ConsoleKey.Backspace, false, false, false);

    [Fact]
    public void EditorDraw_EmptyTranslationField_ShowsLitHint()
    {
        TEditorLinkRun((library, find) =>
        {
            Assert.Equal("Editor.Open", find(ConsoleKey.Enter)?.BKeymapRowLabel);
            Assert.Contains(TEditorFrameDraw(library), static text => text.BCanvasTextRole == "lit"
                && text.BCanvasTextValue.Contains("Add translations"));
        });
    }

    [Fact]
    public void EditorKeymap_CommaAfterStoredWord_MakesAngledChip()
    {
        TEditorLinkRun((library, find) =>
        {
            TEditorLinkSend(library, "chat,");

            IReadOnlyList<BCanvasText> texts = TEditorFrameDraw(library);
            BCanvasText chip = Assert.Single(texts, static text => text.BCanvasTextValue == "<chat French>");
            Assert.Equal("link", chip.BCanvasTextRole);
            BCanvasText entry = Assert.Single(texts, TEditorLinkMatch);
            Assert.Equal(chip.BCanvasTextRow, entry.BCanvasTextRow);
            Assert.True(entry.BCanvasTextColumn > chip.BCanvasTextColumn);
            Assert.Equal("Card.ChipAdd", find(ConsoleKey.Enter)?.BKeymapRowLabel);
        });
    }

    [Fact]
    public void EditorKeymap_EnterOnWholeMatch_LightsThenLinks()
    {
        TEditorLinkRun((library, find) =>
        {
            TEditorLinkSend(library, "chat");

            Assert.Contains(TEditorFrameDraw(library), static text => text.BCanvasTextRole == "menu"
                && text.BCanvasTextValue.StartsWith(" chat  French"));
            Assert.Equal("Card.ChipAdd", find(ConsoleKey.Enter)?.BKeymapRowLabel);

            TEditorFrameSend(library, TLibrary.TLibraryEnter);

            Assert.Equal("Editor.Pick", find(ConsoleKey.Enter)?.BKeymapRowLabel);
            Assert.Contains(TEditorFrameDraw(library), static text => text.BCanvasTextRole == "lit"
                && text.BCanvasTextValue.StartsWith(" chat  French"));

            TEditorFrameSend(library, TLibrary.TLibraryEnter);

            IReadOnlyList<BCanvasText> texts = TEditorFrameDraw(library);
            Assert.Contains(texts, static text => text.BCanvasTextValue == "<chat French>");
            Assert.DoesNotContain(texts, static text => text.BCanvasTextRole == "shadow");
            Assert.StartsWith(" ", Assert.Single(texts, TEditorLinkMatch).BCanvasTextValue);
        });
    }

    [Fact]
    public void EditorKeymap_LitFreshRow_LinksTentativeEntry()
    {
        TEditorLinkRun((library, find) =>
        {
            TEditorLinkSend(library, "zzyzx");

            Assert.Contains(TEditorFrameDraw(library), static text => text.BCanvasTextRole == "menu"
                && text.BCanvasTextValue.StartsWith(" + zzyzx  "));

            TEditorFrameSend(library, TLibrary.TLibraryDown);

            Assert.Equal("Editor.Pick", find(ConsoleKey.Enter)?.BKeymapRowLabel);

            TEditorFrameSend(library, TLibrary.TLibraryEnter);

            BCanvasText chip = Assert.Single(TEditorFrameDraw(library), static text => text.BCanvasTextRole == "link"
                && text.BCanvasTextValue.StartsWith("<zzyzx "));
            Assert.EndsWith(">", chip.BCanvasTextValue);
        });
    }

    [Fact]
    public void EditorDraw_RepeatedHeadwordInList_ReadsNumber()
    {
        TEditorLinkRun(
            (library, _) =>
            {
                TEditorLinkSend(library, "eau");

                IReadOnlyList<BCanvasText> texts = TEditorFrameDraw(library);
                Assert.Contains(texts, static text => text.BCanvasTextRole == "menu"
                    && text.BCanvasTextValue.StartsWith(" eau (1)  French"));
                Assert.Contains(texts, static text => text.BCanvasTextRole == "menu"
                    && text.BCanvasTextValue.StartsWith(" eau (2)  French"));
            },
            engine =>
            {
                engine.TEngineEntrySave(TInterface.TEntryDraftCreate("eau", "French", "", "", [], []));
                engine.TEngineEntrySave(TInterface.TEntryDraftCreate("eau", "French", "", "", [], []));
            });
    }

    [Fact]
    public void EditorKeymap_ClosingField_LinksOneMatchKeepsRest()
    {
        TEditorLinkRun((library, find) =>
        {
            TEditorLinkSend(library, "chat");
            TEditorFrameSend(library, TEditorLinkEscape);
            Assert.Equal("Editor.Close", find(ConsoleKey.Escape)?.BKeymapRowLabel);

            TEditorFrameSend(library, TEditorLinkEscape);

            IReadOnlyList<BCanvasText> texts = TEditorFrameDraw(library);
            Assert.DoesNotContain(texts, TEditorLinkMatch);
            Assert.Contains(texts, static text => text.BCanvasTextRole == "lit"
                && text.BCanvasTextValue.Contains("<chat French>"));

            TEditorFrameSend(library, TLibrary.TLibraryEnter);
            TEditorLinkSend(library, "zzyzx");
            TEditorFrameSend(library, TEditorLinkEscape);
            TEditorFrameSend(library, TEditorLinkEscape);
            TEditorFrameSend(library, TLibrary.TLibraryEnter);

            Assert.StartsWith("zzyzx", Assert.Single(TEditorFrameDraw(library), TEditorLinkMatch).BCanvasTextValue);
        });
    }

    [Fact]
    public void EditorKeymap_FailedResolveOnClose_ShowsFailureKeepsText()
    {
        bool failing = false;
        List<string> asked = [];
        TEditorLinkRun(
            (library, find) =>
            {
                TEditorLinkSend(library, "chat");
                TEditorFrameSend(library, TEditorLinkEscape);
                failing = true;

                TEditorFrameSend(library, TEditorLinkEscape);
                failing = false;

                Assert.Contains("Input.TranslationFailed", asked);
                Assert.Equal("Editor.Open", find(ConsoleKey.Enter)?.BKeymapRowLabel);

                TEditorFrameSend(library, TLibrary.TLibraryEnter);

                Assert.StartsWith("chat", Assert.Single(TEditorFrameDraw(library), TEditorLinkMatch).BCanvasTextValue);
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

    [Fact]
    public void EditorKeymap_BackspaceOnEmptyEntry_RemovesLinkChip()
    {
        TEditorLinkRun((library, _) =>
        {
            TEditorLinkSend(library, "chat,");
            Assert.Contains(TEditorFrameDraw(library), static text => text.BCanvasTextValue == "<chat French>");

            TEditorFrameSend(library, TEditorLinkBack);

            Assert.DoesNotContain(TEditorFrameDraw(library), static text => text.BCanvasTextRole == "link");
        });
    }

    private static void TEditorLinkRun(
        Action<BLibrary, Func<ConsoleKey, BKeymapRow?>> check,
        Action<LEngine>? hold = null,
        List<string>? asked = null)
    {
        TEditorFrameRun(
            [],
            (library, find) =>
            {
                for (int step = 0; step < 6 + TEditorLinkPart; step++)
                {
                    TEditorFrameSend(library, TLibrary.TLibraryDown);
                }

                check(library, key => find(key, 0));
            },
            asked,
            engine =>
            {
                engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
                    "air", "English", string.Empty, string.Empty, [TInterface.TCardCreate("a meaning", 1)], []));
                _ = engine.TEngineTranslationCreate("chat", "French");
                hold?.Invoke(engine);
            });
    }

    private static bool TEditorLinkMatch(BCanvasText text)
    {
        return text.BCanvasTextRole == "field" && text.BCanvasTextColumn >= TEditorLinkSide;
    }

    private static void TEditorLinkSend(BLibrary library, string typed)
    {
        foreach (char letter in typed)
        {
            TEditorFrameSend(library, new ConsoleKeyInfo(
                letter, letter == ',' ? ConsoleKey.OemComma : ConsoleKey.A, false, false, false));
        }
    }
}
