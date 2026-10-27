using Llyn.Conduct;
using Llyn.ShellEngine;
using Llyn.UIDemeanor;
using Llyn.UITerminal;
using Xunit;
using static Llyn.Tests.TEditorFrame;

namespace Llyn.Tests;

public sealed class TEditorMedia
{
    private const int TEditorMediaPart = 6;

    private static readonly ConsoleKeyInfo TEditorMediaNext = new('\0', ConsoleKey.PageDown, false, false, true);

    private static readonly ConsoleKeyInfo TEditorMediaBrowse = new('\u000f', ConsoleKey.O, false, false, true);

    private static readonly ConsoleKeyInfo TEditorMediaInsertion = new('\0', ConsoleKey.Insert, false, false, false);

    private static readonly ConsoleKeyInfo TEditorMediaDeletion = new('\0', ConsoleKey.Delete, false, false, false);

    private static readonly ConsoleKeyInfo TEditorMediaRight = new('\0', ConsoleKey.RightArrow, false, false, false);

    private static readonly ConsoleKeyInfo TEditorMediaEnter = new('\r', ConsoleKey.Enter, false, false, false);

    private static readonly ConsoleKeyInfo TEditorMediaDown = new('\0', ConsoleKey.DownArrow, false, false, false);

    [Fact]
    public void EditorDraw_UnreachedImage_DimsLocation()
    {
        TEditorMediaRun(null, [], 0, static (draw, _, _) =>
        {
            BCanvasText row = Assert.Single(
                draw(), static text => text.BCanvasTextValue.Contains("cat.png", StringComparison.Ordinal));
            Assert.Equal("muted", row.BCanvasTextRole);
            Assert.EndsWith("(missing)", row.BCanvasTextValue, StringComparison.Ordinal);
            Assert.Contains(draw(), static text => text.BCanvasTextValue.Trim() == "Image");
        });
    }

    [Fact]
    public void EditorDraw_UnfoldedCard_ShowsBothAdders()
    {
        TEditorMediaRun(null, [], 0, static (draw, _, _) =>
        {
            IReadOnlyList<BCanvasText> texts = draw();
            BCanvasText image = Assert.Single(texts, static text => text.BCanvasTextValue == "+ Add image");
            BCanvasText video = Assert.Single(texts, static text => text.BCanvasTextValue == "+ Add video");
            Assert.Equal(image.BCanvasTextRow, video.BCanvasTextRow);
            Assert.True(video.BCanvasTextColumn > image.BCanvasTextColumn);
        });
    }

    [Fact]
    public void EditorObserve_TypingOnImageRow_StoresLocation()
    {
        TEditorMediaRun(null, [], TEditorMediaPart, static (draw, send, find) =>
        {
            Assert.Equal("Card.MenuOpen", find(TEditorMediaEnter)?.BKeymapRowLabel);

            send(new ConsoleKeyInfo('x', ConsoleKey.X, false, false, false));

            Assert.Contains(draw(), static text => text.BCanvasTextRole == "field");
            Assert.Equal("Editor.Close", find(TEditorMediaEnter)?.BKeymapRowLabel);

            send(TEditorMediaEnter);

            IReadOnlyList<BCanvasText> texts = draw();
            Assert.DoesNotContain(texts, static text => text.BCanvasTextRole == "field");
            Assert.Contains(texts, static text => text.BCanvasTextValue.Contains("cat.pngx", StringComparison.Ordinal));
        });
    }

    [Fact]
    public void EditorKeymap_InsertOnImageRow_AddsImage()
    {
        TEditorMediaRun(null, [], TEditorMediaPart, static (draw, send, find) =>
        {
            Assert.Equal("Card.ImageAdd", find(TEditorMediaInsertion)?.BKeymapRowLabel);

            send(TEditorMediaInsertion);

            Assert.Equal(2, draw().Count(static text =>
                text.BCanvasTextValue.TrimStart().StartsWith("Image", StringComparison.Ordinal)));
        });
    }

    [Fact]
    public void EditorKeymap_DeleteOnImageRow_RemovesImage()
    {
        TEditorMediaRun(null, [], TEditorMediaPart, static (draw, send, find) =>
        {
            Assert.Equal("Card.ImageRemove", find(TEditorMediaDeletion)?.BKeymapRowLabel);

            send(TEditorMediaDeletion);

            Assert.DoesNotContain(
                draw(), static text => text.BCanvasTextValue.Contains("cat.png", StringComparison.Ordinal));
        });
    }

    [Fact]
    public void EditorKeymap_BrowseOnImageRow_SetsChosenFile()
    {
        List<string> asked = [];
        TEditorMediaRun(@"D:\Pictures\pool.jpg", asked, TEditorMediaPart, (draw, send, find) =>
        {
            Assert.Equal("Card.Browse", find(TEditorMediaBrowse)?.BKeymapRowLabel);

            send(TEditorMediaBrowse);

            Assert.Equal(["Media:Card.ImageChoose"], asked);
            Assert.Contains(
                draw(), static text => text.BCanvasTextValue.Contains("pool.jpg", StringComparison.Ordinal));
        });
    }

    [Fact]
    public void EditorKeymap_BrowseCancelled_KeepsLocation()
    {
        List<string> asked = [];
        TEditorMediaRun(null, asked, TEditorMediaPart, (draw, send, _) =>
        {
            send(TEditorMediaBrowse);

            Assert.Equal(["Media:Card.ImageChoose"], asked);
            Assert.Contains(draw(), static text => text.BCanvasTextValue.Contains("cat.png", StringComparison.Ordinal));
        });
    }

    [Fact]
    public void EditorKeymap_MenuBrowseOnVideoRow_AsksForVideoFile()
    {
        List<string> asked = [];
        TEditorMediaRun(@"D:\Films\pour.mp4", asked, TEditorMediaPart + 1, (draw, send, find) =>
        {
            send(TEditorMediaEnter);

            Assert.Contains(draw(), static text => text.BCanvasTextRole is "menu" or "lit"
                && text.BCanvasTextValue.Contains("Browse", StringComparison.OrdinalIgnoreCase));
            Assert.Equal("Editor.Pick", find(TEditorMediaEnter)?.BKeymapRowLabel);

            send(TEditorMediaEnter);

            Assert.Equal(["Media:Card.VideoChoose"], asked);
            Assert.DoesNotContain(
                draw(), static text => text.BCanvasTextValue.Contains("https", StringComparison.Ordinal));
        });
    }

    [Fact]
    public void EditorObserve_TypingOnSpanSlot_StoresSpan()
    {
        TEditorMediaRun(null, [], TEditorMediaPart + 1, static (draw, send, find) =>
        {
            Assert.Equal("Card.VideoSlot", find(TEditorMediaRight)?.BKeymapRowLabel);

            send(TEditorMediaRight);
            send(new ConsoleKeyInfo('5', ConsoleKey.D5, false, false, false));
            send(TEditorMediaEnter);

            Assert.Contains(draw(), static text => text.BCanvasTextValue.Contains("00:315", StringComparison.Ordinal));
        });
    }

    [Fact]
    public void EditorKeymap_AdderLitRight_AddsVideo()
    {
        TEditorMediaRun(null, [], TEditorMediaPart + 2, static (draw, send, find) =>
        {
            Assert.Equal("Card.ImageAdd", find(TEditorMediaEnter)?.BKeymapRowLabel);

            send(TEditorMediaRight);

            Assert.Equal("Card.VideoAdd", find(TEditorMediaEnter)?.BKeymapRowLabel);

            send(TEditorMediaEnter);

            Assert.Equal(2, draw().Count(static text => text.BCanvasTextValue.Trim() == "Video"));
        });
    }

    [Fact]
    public void EditorKeymap_MenuRemoveOnImageRow_RemovesImage()
    {
        TEditorMediaRun(null, [], TEditorMediaPart, static (draw, send, _) =>
        {
            send(TEditorMediaEnter);
            send(TEditorMediaDown);
            send(TEditorMediaEnter);

            Assert.DoesNotContain(
                draw(), static text => text.BCanvasTextValue.Contains("cat.png", StringComparison.Ordinal));
        });
    }

    [Fact]
    public void EditorKeymap_DeleteOnVideoRow_RemovesVideo()
    {
        TEditorMediaRun(null, [], TEditorMediaPart + 1, static (draw, send, find) =>
        {
            Assert.Equal("Card.VideoRemove", find(TEditorMediaDeletion)?.BKeymapRowLabel);

            send(TEditorMediaDeletion);

            Assert.DoesNotContain(
                draw(), static text => text.BCanvasTextValue.Contains("swim.mp4", StringComparison.Ordinal));
        });
    }

    [Fact]
    public void EditorKeymap_EscapeOnImageMenu_ClosesMenu()
    {
        TEditorMediaRun(null, [], TEditorMediaPart, static (draw, send, find) =>
        {
            ConsoleKeyInfo escape = new('\u001b', ConsoleKey.Escape, false, false, false);
            send(TEditorMediaEnter);

            Assert.Equal("Editor.Shut", find(escape)?.BKeymapRowLabel);

            send(escape);

            Assert.Equal("Card.MenuOpen", find(TEditorMediaEnter)?.BKeymapRowLabel);
            Assert.Contains(draw(), static text => text.BCanvasTextValue.Contains("cat.png", StringComparison.Ordinal));
        });
    }

    [Fact]
    public void EditorKeymap_BrowseOnVideoRow_AsksEditorEnvoy()
    {
        List<string> asked = [];
        TEditorMediaRun(@"D:\Films\pour.mp4", asked, TEditorMediaPart + 1, (draw, send, find) =>
        {
            Assert.Equal("Card.Browse", find(TEditorMediaBrowse)?.BKeymapRowLabel);

            send(TEditorMediaBrowse);
            send(TEditorMediaDown);

            Assert.Equal(["Media:Card.VideoChoose"], asked);
            BCanvasText row = Assert.Single(
                draw(), static text => text.BCanvasTextValue.Contains("pour.mp4", StringComparison.Ordinal));
            Assert.Equal("muted", row.BCanvasTextRole);
        });
    }

    [Fact]
    public void EditorDraw_WideUnreachedVideo_ShowsMissingMark()
    {
        List<string> asked = [];
        TEditorMediaRun(@"D:\Films\pour.mp4", asked, TEditorMediaPart + 1, static (draw, send, _) =>
        {
            send(TEditorMediaBrowse);
            send(TEditorMediaDown);

            BCanvasText row = Assert.Single(
                draw(), static text => text.BCanvasTextValue.Contains("pour.mp4", StringComparison.Ordinal));
            Assert.Equal("muted", row.BCanvasTextRole);
            Assert.Contains("pour.mp4 (missing)", row.BCanvasTextValue, StringComparison.Ordinal);
        }, width: 120);
    }

    [Fact]
    public void EditorDraw_ReachableImage_KeepsPlainLocation()
    {
        TEditorMediaRun(null, [], 0, static (draw, _, _) =>
        {
            BCanvasText row = Assert.Single(
                draw(), static text => text.BCanvasTextValue.Contains("cat.png", StringComparison.Ordinal));
            Assert.Equal("text", row.BCanvasTextRole);
            Assert.DoesNotContain("(missing)", row.BCanvasTextValue, StringComparison.Ordinal);
        }, "https://example.org/cat.png");
    }

    [Fact]
    public void EditorKeymap_PageDownWhileTyping_StoresLocation()
    {
        TEditorMediaRun(null, [], TEditorMediaPart, static (draw, send, _) =>
        {
            send(new ConsoleKeyInfo('x', ConsoleKey.X, false, false, false));
            send(new ConsoleKeyInfo('\0', ConsoleKey.PageDown, false, false, false));

            IReadOnlyList<BCanvasText> texts = draw();
            Assert.DoesNotContain(texts, static text => text.BCanvasTextRole == "field");
            Assert.Contains(texts, static text => text.BCanvasTextValue.Contains("cat.pngx", StringComparison.Ordinal));
        });
    }

    private static void TEditorMediaRun(
        string? path,
        List<string> asked,
        int down,
        Action<Func<IReadOnlyList<BCanvasText>>, Action<ConsoleKeyInfo>, Func<ConsoleKeyInfo, BKeymapRow?>> check,
        string image = "C:/no/cat.png",
        int width = 80)
    {
        TEditorWindowRun((_, atelier, _) =>
        {
            CEditor editor = atelier.CAtelierInputCreate(TEnvoyFake.TEnvoyMediaCreate(path, asked));
            BLocalizationCatalog catalog = new();
            catalog.BLocalizationCatalogApply(TInterface.TLocalizationLoad("en"));
            BEditor face = new(
                catalog,
                editor,
                "Navigation.Input",
                static () => true,
                static () => true,
                static () => { });
            editor.CEditorEntryOpen(null);
            editor.CEditorList.CCardMeaningAdd();
            long card = editor.CEditorWalk[0][2].CStepCard!.CCardDraftId;
            editor.CEditorImage.CImageAdd(false, card);
            editor.CEditorVideo.CVideoAdd(false, card);
            CCardDraft held = editor.CEditorWalk[0][2].CStepCard!;
            editor.CEditorImage.CImageLocationSet(held.CCardDraftImage[0].CImageDraftId, image);
            long video = held.CCardDraftVideo[0].CVideoDraftId;
            editor.CEditorVideo.CVideoLocationSet(video, "https://example.org/swim.mp4");
            editor.CEditorVideo.CVideoSpanSet(video, "00:12 - 00:31");
            editor.CEditorDesk.CDeskDraft.CDeskDraftPersist();
            Func<ConsoleKeyInfo, BKeymapRow?> find = key =>
                face.BEditorKeymap.BKeymapFind(BKeymapFocus.BKeymapFocusEditor, key);
            Action<ConsoleKeyInfo> send = key =>
            {
                if (find(key) is BKeymapRow row)
                {
                    Assert.Equal(BKeymapLive.BKeymapLiveShown, row.BKeymapRowLive());
                    row.BKeymapRowAction(key);
                    return;
                }

                _ = face.BEditorObserve(key);
            };
            send(TEditorMediaNext);
            for (int step = 0; step < down; step++)
            {
                send(TEditorMediaDown);
            }

            BBlueprintPart window = BContract.BContractBlueprintFind(new ABootstrap(), "AWindow");
            BBlueprint.BBlueprintPlace(window, 0, 0, width, 24);
            check(
                () => face.BEditorDraw(
                    new Dictionary<string, IReadOnlyList<string>>(),
                    BContract.BContractFind(window, "ADisplay"),
                    true,
                    new BCanvasWidth(false)),
                send,
                find);
        }, static engine => TInterfaceConduct.TAtelierCreate(engine, false, new LMediaOutlet(engine)));
    }
}
