using Llyn.Conduct;
using Llyn.ShellEngine;
using Llyn.UIDemeanor;
using Llyn.UITerminal;
using Xunit;

namespace Llyn.Tests;

public sealed class TLecternNote
{
    [Fact]
    public void LecternNoteDraw_EveryBlockKind_DrawsItsConsoleLook()
    {
        const string note = "# Title\n\nPlain **bold** and *soft* with `tick`.\n\n## Smaller\n\n- one\n- two\n\n"
            + "> quoted words\n\n```\ncode  line\n    indented\n```\n\n---\n\nLast words.";
        TLecternCard.TLecternCardRun(
            static (engine, _) => TLecternNoteSave(engine, note),
            static (_, rows, _, _, console) =>
            {
                List<string> shown = [.. rows()];
                int heading = shown.IndexOf("Note");

                Assert.True(heading >= 0);
                Assert.Equal(
                    [
                        "Title",
                        new string('─', shown[heading + 2].Length),
                        string.Empty,
                        "Plain bold and soft with tick.",
                        string.Empty,
                        "Smaller",
                        string.Empty,
                        "• one",
                        "• two",
                        string.Empty,
                        "│ quoted words",
                        string.Empty,
                        "code  line",
                        "indented",
                        string.Empty,
                        new string('─', shown[heading + 2].Length),
                        string.Empty,
                        "Last words.",
                    ],
                    shown.Skip(heading + 1).Take(18));
                Assert.True(shown[heading + 2].Length > 10);
                Assert.Equal("heading", TLectern.TLecternRoleRead(console, "Title"));
                Assert.Equal("emphasis", TLectern.TLecternRoleRead(console, "bold"));
                Assert.Equal("emphasis", TLectern.TLecternRoleRead(console, "soft"));
                Assert.Equal("code", TLectern.TLecternRoleRead(console, "tick"));
                Assert.Equal("muted", TLectern.TLecternRoleRead(console, "• one"));
                Assert.Equal("muted", TLectern.TLecternRoleRead(console, "quoted words"));
                Assert.Equal("code", TLectern.TLecternRoleRead(console, "code  line"));
                Assert.Equal("code", TLectern.TLecternRoleRead(console, "indented"));
            });
    }

    [Fact]
    public void LecternNoteDraw_LongCodeLine_CutsInsteadOfWrapping()
    {
        string code = string.Concat(Enumerable.Repeat("abcdefghij", 30));
        TLecternCard.TLecternCardRun(
            (engine, _) => TLecternNoteSave(engine, "```\n" + code + "\n```"),
            static (_, rows, _, _, _) =>
            {
                List<string> shown = [.. rows()];
                int heading = shown.IndexOf("Note");

                Assert.StartsWith("abcdefghij", shown[heading + 1], StringComparison.Ordinal);
                Assert.True(shown[heading + 1].Length < 300);
                Assert.True(shown.Count <= heading + 2 || !shown[heading + 2].StartsWith('a'));
            });
    }

    [Fact]
    public void LecternNoteOpen_EnterOnTheLink_OpensTheAddress()
    {
        List<string> opened = [];
        TLecternCard.TLecternCardRun(
            static (engine, _) => TLecternNoteSave(engine, "See [the site](https://example.com/page) now."),
            (send, rows, lit, _, console) =>
            {
                for (int step = 0; step < 30 && lit()?.Item1 != "the site"; step++)
                {
                    send(ConsoleKey.DownArrow);
                }

                Assert.Equal(("the site", "lit"), lit());
                Assert.Contains("See the site now.", rows());
                Assert.Contains(console.TConsoleFakeGrid, static row =>
                    row.Contains("https://example.com/page", StringComparison.Ordinal));

                send(ConsoleKey.Enter);

                Assert.Equal(["https://example.com/page"], opened);
                Assert.Equal(("the site", "lit"), lit());
            },
            build: engine => TInterfaceConduct.TAtelierCreate(engine, opened.Add));
    }

    [Fact]
    public void LecternNoteOpen_RefusedScheme_ShowsPlainTextAndOpensNothing()
    {
        List<string> opened = [];
        TLecternCard.TLecternCardRun(
            static (engine, _) => TLecternNoteSave(
                engine, "Run [it](file:///C:/evil.exe) or [mail](mailto:a@b.c) or [code](javascript:alert)."),
            (send, rows, lit, _, console) =>
            {
                Assert.Contains("Run it or mail or code.", rows());
                Assert.Equal("text", TLectern.TLecternRoleRead(console, "it or mail or code."));

                for (int step = 0; step < 30; step++)
                {
                    send(ConsoleKey.DownArrow);
                    Assert.NotEqual("it", lit()?.Item1);
                }

                send(ConsoleKey.Enter);

                Assert.Empty(opened);
            },
            build: engine => TInterfaceConduct.TAtelierCreate(engine, opened.Add));
    }

    [Fact]
    public void LecternNoteOpen_DrawnItem_OpensTheAddressReadAtDraw()
    {
        List<string> opened = [];
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine, opened.Add);
        BLocalizationCatalog catalog = new();
        catalog.BLocalizationCatalogApply(TInterface.TLocalizationLoad("en"));
        BLecternNote note = new(catalog, atelier);
        CLectern drawn = TLecternNoteShow("first", "https://example.com/first");

        string item = note.BLecternNoteDraw(
                drawn, new ABootstrap().BBlueprintGlyphs["single"], 80, new BCanvasWidth(false))
            .SelectMany(static line => line.BLecternLinePieces)
            .Select(static piece => piece.BLecternLineItem)
            .OfType<string>()
            .Single();

        Assert.Equal("https://example.com/first", note.BLecternNoteRead(item));
        Assert.True(note.BLecternNoteCheck(item));
        _ = note.BLecternNoteOpen(item);
        Assert.Equal(["https://example.com/first"], opened);
    }

    [Fact]
    public void LecternNoteDraw_NoNote_DrawsNoHeading()
    {
        TLecternCard.TLecternCardRun(
            static (_, save) => save("brook", [TInterface.TCardCreate("a small stream", 1)]),
            static (_, rows, _, _, _) => Assert.DoesNotContain("Note", rows()));
    }

    private static void TLecternNoteSave(LEngine engine, string note)
    {
        _ = engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "brook", "English", string.Empty, note, [TInterface.TCardCreate("a small stream", 1)], []));
    }

    private static CLectern TLecternNoteShow(string text, string address)
    {
        return new CLectern(
            "brook",
            "English",
            [],
            false,
            [
                new CMarkdownBlock(
                    false, false, false, false, false, 0, string.Empty, text,
                    [new CMarkdownSpan(text, false, false, false, new Uri(address))]),
            ],
            true,
            string.Empty,
            string.Empty,
            false,
            string.Empty,
            string.Empty);
    }
}
