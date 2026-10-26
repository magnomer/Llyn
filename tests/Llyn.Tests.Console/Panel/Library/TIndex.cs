using Llyn.Conduct;
using Llyn.UIDemeanor;
using Llyn.UITerminal;
using Xunit;

namespace Llyn.Tests;

public sealed class TIndex
{
    private static readonly ConsoleKeyInfo TIndexDown = new('\0', ConsoleKey.DownArrow, false, false, false);
    private static readonly ConsoleKeyInfo TIndexEnter = new('\r', ConsoleKey.Enter, false, false, false);
    private static readonly ConsoleKeyInfo TIndexEscape = new('\e', ConsoleKey.Escape, false, false, false);
    private static readonly ConsoleKeyInfo TIndexBackspace = new('\b', ConsoleKey.Backspace, false, false, false);
    private static readonly ConsoleKeyInfo TIndexMarkKey = new('\0', ConsoleKey.Insert, false, false, false);
    private static readonly ConsoleKeyInfo TIndexShift = new('\0', ConsoleKey.DownArrow, false, false, true);
    private static readonly ConsoleKeyInfo TIndexLetter = new('s', ConsoleKey.S, false, false, false);

    private static readonly IReadOnlyList<CVistaRow> TIndexRows =
    [
        new(1, "apple", "English", string.Empty, "apple", false),
        new(2, "stone", "English", "stoun", "stone", true),
        new(3, "水", "Mandarin", "shuǐ", "水 (2)", false),
    ];

    [Fact]
    public void IndexDraw_ChosenElsewhere_MarksFirstCell()
    {
        BIndex<CVistaRow> index = TIndexBuild(TIndexRows);
        index.BIndexObserve(TIndexDown);

        string[] grid = TIndexShow(index, false);

        Assert.StartsWith("|o stone  stoun", grid[2], StringComparison.Ordinal);
        Assert.StartsWith("|  水 (2)  shuǐ", grid[3], StringComparison.Ordinal);
        Assert.EndsWith("Mandarin|", grid[3], StringComparison.Ordinal);
    }

    [Fact]
    public void IndexDraw_LitOnChosen_WearsNoMark()
    {
        string[] grid = TIndexShow(TIndexBuild(TIndexRows), false);

        Assert.StartsWith("|  stone", grid[2], StringComparison.Ordinal);
        Assert.Contains(" 2 of 3  Headword A to Z ", grid[^1], StringComparison.Ordinal);
    }

    [Fact]
    public void IndexDraw_LanguageHidden_ShowsFilterMark()
    {
        BIndex<CVistaRow> index = TIndexBuild(TIndexRows, filtered: true);

        Assert.Contains("  Filter* ", TIndexShow(index, false)[^1], StringComparison.Ordinal);
        Assert.DoesNotContain("Filter*", TIndexShow(TIndexBuild(TIndexRows), false)[^1], StringComparison.Ordinal);
    }

    [Fact]
    public void IndexDraw_NarrowSide_CutsLanguageByCells()
    {
        string[] grid = TIndexShow(TIndexBuild(TIndexRows), false, 26);

        Assert.EndsWith(" Mandar|", grid[3], StringComparison.Ordinal);
    }

    [Fact]
    public void IndexDraw_Focused_LightsTitleAndRow()
    {
        IReadOnlyList<BCanvasText> texts = TIndexDraw(TIndexBuild(TIndexRows), true, 40);

        Assert.Contains(texts, static text => text.BCanvasTextValue == " Library " && text.BCanvasTextRole == "title");
        Assert.Contains(texts, static text => text.BCanvasTextValue.StartsWith("stone", StringComparison.Ordinal)
            && text.BCanvasTextRole == "lit");
    }

    [Fact]
    public void IndexDraw_NoRows_ShowsApertureNoticeOnly()
    {
        string[] grid = TIndexShow(TIndexBuild([], empty: true), false);

        Assert.Equal("|" + new string(' ', 12) + "No entries yet" + new string(' ', 12) + "|", grid[2]);
        Assert.Contains(" 0 of 0 ", grid[^1], StringComparison.Ordinal);
    }

    [Fact]
    public void IndexDraw_FailedRead_WordsNothing()
    {
        string[] grid = TIndexShow(TIndexBuild([], empty: false), false);

        Assert.All(grid[1..^2], static line => Assert.Equal("|" + new string(' ', 38) + "|", line));
    }

    [Fact]
    public void IndexRefine_RowsReordered_KeepsLitEntry()
    {
        BIndex<CVistaRow> index = TIndexBuild(TIndexRows);
        index.BIndexObserve(TIndexDown);

        index.BIndexRefine([.. TIndexRows.Reverse()], "Order.Headword", false, string.Empty);

        string[] grid = TIndexShow(index, true);
        Assert.Contains(" 1 of 3 ", grid[^1], StringComparison.Ordinal);
        Assert.StartsWith("|  水 (2)", grid[1], StringComparison.Ordinal);
    }

    [Fact]
    public void IndexRefine_LitEntryGone_FallsBackToChosen()
    {
        BIndex<CVistaRow> index = TIndexBuild(TIndexRows);
        index.BIndexObserve(TIndexDown);

        index.BIndexRefine([TIndexRows[0], TIndexRows[1]], "Order.Headword", false, string.Empty);

        Assert.Contains(" 2 of 2 ", TIndexShow(index, false)[^1], StringComparison.Ordinal);
    }

    [Fact]
    public void IndexLitSettle_SettledRowGone_TakesTheRowInItsPlace()
    {
        BIndex<CVistaRow> index = TIndexBuild(TIndexRows);

        index.BIndexLitSettle(
            () => index.BIndexRefine([TIndexRows[0], TIndexRows[2]], "Order.Headword", false, string.Empty));

        Assert.StartsWith("|  水 (2)", TIndexShow(index, true)[2], StringComparison.Ordinal);
        Assert.Contains(" 2 of 2 ", TIndexShow(index, true)[^1], StringComparison.Ordinal);
        index.BIndexLitSettle(() => index.BIndexRefine([TIndexRows[0]], "Order.Headword", false, string.Empty));
        Assert.Contains(" 1 of 1 ", TIndexShow(index, true)[^1], StringComparison.Ordinal);
        index.BIndexLitSettle(() => index.BIndexRefine([.. TIndexRows], "Order.Headword", false, string.Empty));
        Assert.Contains(" 1 of 3 ", TIndexShow(index, true)[^1], StringComparison.Ordinal);
    }

    [Fact]
    public void IndexLitSettle_SettledRowStays_LeavesLitFollowingChosen()
    {
        BIndex<CVistaRow> index = TIndexBuild(TIndexRows);
        CVistaRow[] moved = [new(1, "apple", "English", string.Empty, "apple", true), TIndexRows[1] with
        {
            CVistaRowChosen = false,
        }, TIndexRows[2]];

        index.BIndexLitSettle(() => index.BIndexRefine(TIndexRows, "Order.Headword", false, string.Empty));
        index.BIndexRefine(moved, "Order.Headword", false, string.Empty);

        Assert.Contains(" 1 of 3 ", TIndexShow(index, true)[^1], StringComparison.Ordinal);
    }

    [Fact]
    public void IndexObserve_Typing_SendsQueryToConduct()
    {
        List<string> queries = [];
        BIndex<CVistaRow> index = TIndexBuild(TIndexRows, query: queries.Add);

        Assert.True(index.BIndexObserve(TIndexLetter));
        Assert.True(index.BIndexObserve(TIndexBackspace));

        Assert.Equal(["s", string.Empty], queries);
        Assert.Contains("|Search entries ", TIndexShow(index, false)[^2], StringComparison.Ordinal);
    }

    [Fact]
    public void IndexObserve_BackspaceOnEmptyQuery_ClimbsOut()
    {
        int climbs = 0;
        BIndex<CVistaRow> index = TIndexBuild(TIndexRows, climb: () => climbs++);

        Assert.True(index.BIndexObserve(TIndexBackspace));
        Assert.False(TIndexBuild(TIndexRows).BIndexObserve(TIndexBackspace));
        Assert.Equal(1, climbs);
    }

    [Fact]
    public void IndexObserve_Escape_EmptiesQueryThenDoesNothing()
    {
        List<string> queries = [];
        BIndex<CVistaRow> index = TIndexBuild(TIndexRows, query: queries.Add);
        index.BIndexObserve(TIndexLetter);

        Assert.True(index.BIndexObserve(TIndexEscape));
        Assert.False(index.BIndexObserve(TIndexEscape));
        Assert.Equal(["s", string.Empty], queries);
    }

    [Fact]
    public void IndexObserve_Enter_RunsLitRow()
    {
        List<long> chosen = [];
        BIndex<CVistaRow> index = TIndexBuild(TIndexRows, choose: row => chosen.Add(row.CVistaRowId));
        index.BIndexObserve(TIndexDown);

        Assert.True(index.BIndexObserve(TIndexEnter));
        Assert.False(TIndexBuild([]).BIndexObserve(TIndexEnter));
        Assert.Equal([3L], chosen);
    }

    [Fact]
    public void IndexObserve_Insert_MarksAndMovesDown()
    {
        List<long> marked = [];
        BIndex<CVistaRow> index = TIndexBuild(TIndexRows, mark: row => marked.Add(row.CVistaRowId));

        Assert.True(index.BIndexObserve(TIndexMarkKey));
        Assert.True(index.BIndexObserve(TIndexMarkKey));
        Assert.False(TIndexBuild(TIndexRows).BIndexObserve(TIndexMarkKey));
        Assert.Equal([2L, 3L], marked);
    }

    [Fact]
    public void IndexObserve_ControlDown_MovesRowOnePlace()
    {
        List<(long, int)> shifted = [];
        BIndex<CVistaRow> index = TIndexBuild(TIndexRows, shift: (row, step) => shifted.Add((row.CVistaRowId, step)));

        Assert.True(index.BIndexObserve(TIndexShift));
        Assert.False(TIndexBuild(TIndexRows).BIndexObserve(TIndexShift));
        Assert.Equal([(2L, 1)], shifted);
    }

    [Fact]
    public void IndexObserve_LetterWithoutSearch_JumpsToName()
    {
        BIndex<CVistaRow> index = TIndexBuild(TIndexRows, search: false);
        index.BIndexObserve(new ConsoleKeyInfo('a', ConsoleKey.A, false, false, false));

        Assert.Contains(" 1 of 3 ", TIndexShow(index, false)[^1], StringComparison.Ordinal);
    }

    [Fact]
    public void BlueprintGlyphs_EverySet_CarriesTheListMarks()
    {
        IReadOnlyDictionary<string, IReadOnlyDictionary<string, IReadOnlyList<string>>> sets =
            new ABootstrap().BBlueprintGlyphs.ToDictionary(static set => set.Key, static set => set.Value);

        Assert.Equal("* -", string.Join(' ', sets["ascii"]["Favorite"]));
        Assert.Equal("# + .", string.Join(' ', sets["ascii"]["Grasp"]));
        Assert.Equal("v >", string.Join(' ', sets["ascii"]["Fold"]));
        Assert.Equal("P V", string.Join(' ', sets["ascii"]["Media"]));
        Assert.Equal("~ ~~", string.Join(' ', sets["ascii"]["Clip"]));
        Assert.Equal("-> <-", string.Join(' ', sets["ascii"]["Arrow"]));
        Assert.Equal("o", string.Join(' ', sets["ascii"]["Pending"]));
        Assert.Equal("● ◐ ○", string.Join(' ', sets["double"]["Grasp"]));
        Assert.Equal("|- `- |", string.Join(' ', sets["ascii"]["Tree"]));
        Assert.Equal("├─ └─ │", string.Join(' ', sets["single"]["Tree"]));
        foreach (string mark in new[] { "Favorite", "Grasp", "Fold", "Media", "Clip", "Arrow", "Pending", "Tree" })
        {
            Assert.Equal(sets["ascii"][mark].Count, sets["double"][mark].Count);
            Assert.Equal(sets["ascii"][mark].Count, sets["single"][mark].Count);
        }
    }

    private static BIndex<CVistaRow> TIndexBuild(
        IReadOnlyList<CVistaRow> rows,
        bool filtered = false,
        bool empty = false,
        bool search = true,
        Action<string>? query = null,
        Action<CVistaRow>? choose = null,
        Action? climb = null,
        Action<CVistaRow>? mark = null,
        Action<CVistaRow, int>? shift = null)
    {
        BLocalizationCatalog catalog = new();
        catalog.BLocalizationCatalogApply(new Dictionary<string, string>
        {
            ["Navigation.Library"] = "Library",
            ["List.Search"] = "Search entries",
            ["List.Empty"] = "No entries yet",
            ["Order.Headword"] = "Headword A to Z",
            ["Index.Position"] = "{0} of {1}",
            ["Index.Filtered"] = "Filter*",
        });
        BIndex<CVistaRow> index = new(
            catalog,
            "Navigation.Library",
            "List.Search",
            static row => (row.CVistaRowName, row.CVistaRowEpithet, row.CVistaRowLanguage, row.CVistaRowChosen),
            static row => row.CVistaRowId,
            search ? query ?? (static _ => { }) : null,
            choose ?? (static _ => { }),
            climb,
            mark,
            shift);
        index.BIndexRefine(
            rows,
            "Order." + CCatalog.CCatalogOrderFormat(CCatalogOrder.CCatalogOrderHeadword),
            filtered,
            empty ? "List.Empty" : string.Empty);
        return index;
    }

    private static IReadOnlyList<BCanvasText> TIndexDraw(BIndex<CVistaRow> index, bool focused, int width)
    {
        return [.. index.BIndexDraw(
            new ABootstrap().BBlueprintGlyphs["ascii"], 0, 0, width, 8, focused, new BCanvasWidth(false))];
    }

    private static string[] TIndexShow(BIndex<CVistaRow> index, bool focused, int width = 40)
    {
        BCanvasWidth measure = new(false);
        string[] grid = [.. Enumerable.Range(0, 8).Select(_ => "|" + new string(' ', width - 2) + "|")];
        foreach (BCanvasText text in TIndexDraw(index, focused, width))
        {
            string value = measure.BCanvasWidthClamp(text.BCanvasTextValue, text.BCanvasTextSpan);
            string line = grid[text.BCanvasTextRow];
            int start = TIndexOffsetResolve(line, text.BCanvasTextColumn, measure);
            int end = TIndexOffsetResolve(line, text.BCanvasTextColumn + text.BCanvasTextSpan, measure);
            grid[text.BCanvasTextRow] = line[..start] + value + line[end..];
        }

        return grid;
    }

    private static int TIndexOffsetResolve(string line, int cells, BCanvasWidth measure)
    {
        int offset = 0;
        for (int taken = 0; taken < cells && offset < line.Length; offset++)
        {
            taken += measure.BCanvasWidthResolve(line.AsSpan(offset, 1));
        }

        return offset;
    }
}
