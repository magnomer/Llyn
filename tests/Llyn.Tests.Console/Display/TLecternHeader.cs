using Llyn.UIDemeanor;
using Xunit;

namespace Llyn.Tests;

public sealed class TLecternHeader
{
    [Fact]
    public void LecternObserve_EnterOnHeartAndF6_TogglesFavorite()
    {
        TLecternCard.TLecternCardRun(
            static (_, save) => save("fire", [TInterface.TCardCreate("a meaning", 1)]),
            static (send, rows, lit, _, _) =>
            {
                send(ConsoleKey.F6);
                Assert.Contains("Select an entry to read it", rows());

                send(ConsoleKey.Enter);
                Assert.EndsWith("☆", rows()[0], StringComparison.Ordinal);
                send(ConsoleKey.DownArrow);
                send(ConsoleKey.Enter);

                Assert.Equal(("★", "lit"), lit());
                send(ConsoleKey.Tab);
                Assert.Equal(("★", "lit-other"), lit());

                send(ConsoleKey.F6);

                Assert.EndsWith("☆", rows()[0], StringComparison.Ordinal);
                send(ConsoleKey.Tab);
                send(ConsoleKey.UpArrow);
                Assert.Equal(("☆", "lit"), lit());
            },
            height: 24,
            open: false);
    }

    [Fact]
    public void LecternObserve_ArrowsOnStars_MovesGraspAndLeftClears()
    {
        TLecternCard.TLecternCardRun(
            static (_, save) => save("fire", [TInterface.TCardCreate("a meaning", 1)]),
            static (send, _, lit, _, _) =>
            {
                send(ConsoleKey.DownArrow);
                send(ConsoleKey.DownArrow);

                send(ConsoleKey.RightArrow);
                Assert.Equal(("◐○○○○  Never seen it", "lit"), lit());

                send(ConsoleKey.RightArrow);
                Assert.Equal(("●○○○○  Seen it before", "lit"), lit());

                send(ConsoleKey.LeftArrow);
                Assert.Equal(("◐○○○○  Never seen it", "lit"), lit());

                send(ConsoleKey.Enter);
                Assert.Equal(("◐○○○○  Never seen it", "lit"), lit());

                send(ConsoleKey.LeftArrow);
                Assert.Equal(("○○○○○  Not rated", "lit"), lit());

                send(ConsoleKey.LeftArrow);
                Assert.Equal(("○○○○○  Not rated", "lit"), lit());
            },
            height: 24);
    }

    [Fact]
    public void LecternDraw_Frequency_ShowsRungAndSourceWhileLit()
    {
        TLecternCard.TLecternCardRun(
            static (_, save) => save("fire", [TInterface.TCardCreate("a meaning", 1)]),
            static (send, rows, lit, _, console) =>
            {
                IReadOnlyList<string> shown = rows();
                Assert.StartsWith("English", shown[1], StringComparison.Ordinal);
                Assert.Contains("Core ●●●●", shown[1], StringComparison.Ordinal);
                Assert.Equal("frequency-4", TLectern.TLecternRoleRead(console, "Core ●●●●"));
                Assert.DoesNotContain(
                    console.TConsoleFakeGrid, static row => row.Contains("Corpus", StringComparison.Ordinal));

                send(ConsoleKey.DownArrow);
                send(ConsoleKey.DownArrow);

                Assert.Equal(("Core ●●●●", "lit"), lit());
                Assert.Contains(
                    console.TConsoleFakeGrid, static row => row.Contains("Corpus: ", StringComparison.Ordinal));
            },
            """
            INSERT INTO frequency (entry_parent, source, raw, band, fetched_utc)
            SELECT entry_id, 'Corpus', '120', 'Core', '2026-01-01T00:00:00Z' FROM entry;
            """,
            24);
    }

    [Fact]
    public void LecternDraw_StoredEntry_DrawsStampsBelowHeader()
    {
        TLecternCard.TLecternCardRun(
            static (_, save) => save("fire", [TInterface.TCardCreate("a meaning", 1)]),
            static (_, rows, _, _, console) =>
            {
                IReadOnlyList<string> shown = rows();

                Assert.Equal(["", "Meanings", "1 ▾ Meaning", "a meaning", ""], shown.Skip(3).Take(5));
                Assert.StartsWith("Created  ", shown[8], StringComparison.Ordinal);
                Assert.StartsWith("Updated  ", shown[9], StringComparison.Ordinal);
                Assert.Equal("muted", TLectern.TLecternRoleRead(console, "Created"));
                Assert.Equal("heading", TLectern.TLecternRoleRead(console, "fire"));
            },
            height: 24);
    }

    [Fact]
    public void LecternCardDraw_GlyphSection_ShowsChip()
    {
        TLecternCard.TLecternCardRun(
            static (engine, _) => engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
                "한자",
                "Korean",
                string.Empty,
                string.Empty,
                [TInterface.TCardCreate("a character", 1)],
                [],
                transcriptions: [TInterface.TTranscriptionDraftCreate("Hanja", "漢字")])),
            static (send, rows, lit, _, console) =>
            {
                Assert.Contains("Hanja  漢  字", rows());
                Assert.Equal("cjk", TLectern.TLecternRoleRead(console, "漢"));
                for (int step = 0; step < 10 && lit()?.Item1 != "漢"; step++)
                {
                    send(ConsoleKey.DownArrow);
                }

                Assert.Equal(("漢", "lit"), lit());

                send(ConsoleKey.Enter);

                Assert.StartsWith("漢", rows()[0], StringComparison.Ordinal);
            },
            height: 24);
    }

    [Fact]
    public void LibraryKeymap_TabWithScribeShown_MovesFocusToEditor()
    {
        TLecternCard.TLecternCardRun(
            static (_, save) => save("fire", [TInterface.TCardCreate("a meaning", 1)]),
            static (send, rows, lit, _, _) =>
            {
                send(ConsoleKey.Tab);
                send(ConsoleKey.F4);
                send(ConsoleKey.Tab);
                send(ConsoleKey.F4);
                send(ConsoleKey.DownArrow);

                Assert.StartsWith("fire", rows()[0], StringComparison.Ordinal);
                Assert.NotNull(lit());
            },
            height: 24);
    }

    [Fact]
    public void LecternLineDivide_LongText_WrapsAtWordWithIndent()
    {
        BCanvasWidth measure = new(false);
        BLecternLine line = new([("one two three", "text", null)], 0);

        IReadOnlyList<BLecternLine> rows = line.BLecternLineDivide(8, measure);

        Assert.Equal(["one two", "three"], rows.Select(static row => row.BLecternLinePieces.Single().BLecternLineText));
        Assert.Equal([0, 2], rows.Select(static row => row.BLecternLineIndent));
    }

    [Fact]
    public void LecternLineDivide_OwnedPieces_TrimsTrailingSpace()
    {
        BCanvasWidth measure = new(false);
        BLecternLine line = new([("The water ", "text", "Item"), ("is cold.", "text", "Item")], 0);

        IReadOnlyList<BLecternLine> rows = line.BLecternLineDivide(10, measure);

        Assert.Equal(
            ["The water", "is cold."],
            rows.Select(static row => Assert.Single(row.BLecternLinePieces).BLecternLineText));
    }

    [Fact]
    public void LecternLineDivide_ItemAndLongWord_KeepsItemCutsWord()
    {
        BCanvasWidth measure = new(false);
        BLecternLine item = new([("ab ", "text", null), ("ITEM", "lit", "Item")], 0);
        BLecternLine word = new([("abcdefghij", "text", null)], 0);

        IReadOnlyList<BLecternLine> held = item.BLecternLineDivide(6, measure);
        IReadOnlyList<BLecternLine> cut = word.BLecternLineDivide(4, measure);

        Assert.Equal(("ITEM", "lit", "Item"), Assert.Single(held[1].BLecternLinePieces));
        Assert.Equal("ab", Assert.Single(held[0].BLecternLinePieces).BLecternLineText);
        Assert.Equal(
            ["abcd", "ef", "gh", "ij"], cut.Select(static row => row.BLecternLinePieces.Single().BLecternLineText));
        Assert.Empty(item.BLecternLineDivide(0, measure));
    }
}
