using Llyn.Conduct;
using Llyn.UIDemeanor;
using Llyn.UITerminal;
using Xunit;

namespace Llyn.Tests;

public sealed class TChoice
{
    [Fact]
    public void ParleyBodyDraw_AppliedOrder_MarksAndLightsOrder()
    {
        BLocalizationCatalog catalog = new();
        catalog.BLocalizationCatalogApply(TInterface.TLocalizationLoad("en"));
        BSChoiceOrder chooser = new(
            catalog,
            "Order",
            [
                CCatalogOrder.CCatalogOrderHeadword, CCatalogOrder.CCatalogOrderReverse,
                CCatalogOrder.CCatalogOrderRecent,
            ],
            static () => CCatalog.CCatalogOrderFormat(CCatalogOrder.CCatalogOrderReverse),
            static _ => { },
            new BCanvasWidth(false));

        BCanvasText[] texts =
        [
            .. chooser.BSParleyBodyDraw(
                new ABootstrap().BBlueprintGlyphs["double"], 0, 0, 30, 10, true, "dialog", new(false)),
        ];

        Assert.Equal(
            ["( ) Headword A → Z", "(•) Headword Z → A", "( ) Newest first"],
            texts.Select(static text => text.BCanvasTextValue.TrimEnd()));
        Assert.Equal(["dialog", "lit", "dialog"], texts.Select(static text => text.BCanvasTextRole));
    }

    [Fact]
    public void OrderObserve_DownEnter_ChoosesNextAndKeepsAppliedMark()
    {
        BLocalizationCatalog catalog = new();
        catalog.BLocalizationCatalogApply(TInterface.TLocalizationLoad("en"));
        List<CCatalogOrder> chosen = [];
        BSChoiceOrder chooser = new(
            catalog,
            "Series",
            [CCatalogOrder.CCatalogOrderHeadword, CCatalogOrder.CCatalogOrderReverse, CCatalogOrder.CCatalogOrderGrasp],
            static () => CCatalog.CCatalogOrderFormat(CCatalogOrder.CCatalogOrderReverse),
            chosen.Add,
            new BCanvasWidth(false));

        chooser.BSParleyBodyObserve(new ConsoleKeyInfo('\0', ConsoleKey.DownArrow, false, false, false));
        chooser.BSParleyBodyObserve(new ConsoleKeyInfo('\r', ConsoleKey.Enter, false, false, false));

        Assert.Equal([CCatalogOrder.CCatalogOrderGrasp], chosen);
        Assert.Equal(
            ["( ) Headword A → Z", "(•) Headword Z → A", "( ) Grasp high → low"],
            chooser.BSParleyBodyDraw(
                    new ABootstrap().BBlueprintGlyphs["double"], 0, 0, 30, 10, true, "dialog", new(false))
                .Select(static text => text.BCanvasTextValue.TrimEnd()));
    }

    [Fact]
    public void OrderObserve_Escape_LeavesToSublevel()
    {
        List<CCatalogOrder> chosen = [];
        BSChoiceOrder chooser = new(
            new BLocalizationCatalog(),
            "Order",
            [CCatalogOrder.CCatalogOrderHeadword],
            static () => CCatalog.CCatalogOrderFormat(CCatalogOrder.CCatalogOrderHeadword),
            chosen.Add,
            new BCanvasWidth(false));

        Assert.False(chooser.BSParleyBodyObserve(new ConsoleKeyInfo('\e', ConsoleKey.Escape, false, false, false)));
        Assert.Empty(chosen);
    }

    [Fact]
    public async Task OrderObserve_EmptyList_LeavesEnterToSublevel()
    {
        List<CCatalogOrder> chosen = [];
        BSChoiceOrder chooser = new(
            new BLocalizationCatalog(), "Order", [], static () => string.Empty, chosen.Add, new BCanvasWidth(false));
        BSParley parley = new(
            new ABootstrap().BBlueprintParts["ASParley"], "Order", "dialog", [], [new("Close")], 0, chooser);
        ConsoleKeyInfo enter = new('\r', ConsoleKey.Enter, false, false, false);

        Assert.False(chooser.BSParleyBodyObserve(enter));
        parley.BSParleyKeyObserve(enter);

        Assert.Equal(0, await parley.BSParleyAnswer.WaitAsync(TimeSpan.FromSeconds(1)));
        Assert.Empty(chosen);
    }

    [Fact]
    public void FilterObserve_EmptyList_LeavesSpaceToSublevel()
    {
        List<IReadOnlyList<string>> hides = [];
        BSChoiceFilter chooser = new([], new CCatalogFilter([]), hides.Add, new BCanvasWidth(false));

        Assert.False(chooser.BSParleyBodyObserve(new ConsoleKeyInfo(' ', ConsoleKey.Spacebar, false, false, false)));
        Assert.DoesNotContain(
            chooser.BSParleyBodyKeymap.BKeymapRead(BKeymapFocus.BKeymapFocusParley),
            static row => row.BKeymapRowLive() != BKeymapLive.BKeymapLiveHidden);
        Assert.Empty(hides);
    }

    [Fact]
    public void FilterObserve_Space_TicksAndUnticksAtOnce()
    {
        CCatalogFilter filter = new([]);
        BSChoiceFilter chooser = new(
            ["English", "Japanese", "Korean"],
            filter,
            hidden => filter = new CCatalogFilter(hidden),
            new BCanvasWidth(false));
        ConsoleKeyInfo space = new(' ', ConsoleKey.Spacebar, false, false, false);

        chooser.BSParleyBodyObserve(space);
        Assert.Equal(["English"], filter.CCatalogFilterHidden);

        chooser.BSParleyBodyObserve(new ConsoleKeyInfo('\0', ConsoleKey.DownArrow, false, false, false));
        chooser.BSParleyBodyObserve(space);
        Assert.Equal(["English", "Japanese"], filter.CCatalogFilterHidden);

        chooser.BSParleyBodyObserve(space);
        Assert.Equal(["English"], filter.CCatalogFilterHidden);
        Assert.Equal(
            ["[ ] English", "[x] Japanese", "[x] Korean"],
            chooser.BSParleyBodyDraw(
                    new ABootstrap().BBlueprintGlyphs["double"], 0, 0, 30, 10, true, "dialog", new(false))
                .Select(static text => text.BCanvasTextValue.TrimEnd()));
    }

    [Fact]
    public void FilterObserve_Space_HidesOnlyListedUntickedLanguages()
    {
        CCatalogFilter filter = new(["Klingon", "korean"]);
        BSChoiceFilter chooser = new(
            ["English", "Korean"],
            filter,
            hidden => filter = new CCatalogFilter(hidden),
            new BCanvasWidth(false));

        chooser.BSParleyBodyObserve(new ConsoleKeyInfo(' ', ConsoleKey.Spacebar, false, false, false));

        Assert.Equal(["English", "Korean"], filter.CCatalogFilterHidden);
    }

    [Theory]
    [InlineData(ConsoleKey.Enter, '\r')]
    [InlineData(ConsoleKey.Escape, '\e')]
    public async Task ParleyKeyObserve_EnterOrEscape_Closes(ConsoleKey key, char letter)
    {
        List<IReadOnlyList<string>> hides = [];
        BSChoiceFilter chooser = new(["English"], new CCatalogFilter([]), hides.Add, new BCanvasWidth(false));
        BSParley parley = new(
            new ABootstrap().BBlueprintParts["ASParley"], "Filter", "dialog", [], [new("Close")], 0, chooser);

        parley.BSParleyKeyObserve(new ConsoleKeyInfo(letter, key, false, false, false));

        Assert.Equal(0, await parley.BSParleyAnswer.WaitAsync(TimeSpan.FromSeconds(1)));
        Assert.Empty(hides);
    }

    [Fact]
    public void ParleyDraw_SmallConsole_KeepsLitRowInsideFrame()
    {
        ABootstrap bootstrap = new();
        string[] languages = [.. Enumerable.Range(1, 28).Select(static at => $"Language {at:D2}")];
        BSChoiceFilter chooser = new(languages, new CCatalogFilter([]), static _ => { }, new BCanvasWidth(false));
        BSParley parley = new(
            bootstrap.BBlueprintParts["ASParley"], "Filter", "dialog", [], [new("Close")], 0, chooser);
        for (int step = 0; step < 20; step++)
        {
            parley.BSParleyKeyObserve(new ConsoleKeyInfo('\0', ConsoleKey.DownArrow, false, false, false));
        }

        IReadOnlyList<BCanvasText> texts = parley.BSParleyDraw(bootstrap.BBlueprintGlyphs, 80, 14, new(false));

        BCanvasText[] rows =
        [
            .. texts.Where(static text => text.BCanvasTextValue.Contains("Language", StringComparison.Ordinal)),
        ];
        int bar = texts.Single(static text => text.BCanvasTextValue == "[ Close ]").BCanvasTextRow;
        Assert.All(rows, row => Assert.True(row.BCanvasTextRow < bar - 1));
        Assert.Contains(
            rows,
            static row => row is { BCanvasTextRole: "lit" }
                && row.BCanvasTextValue.StartsWith("[x] Language 21", StringComparison.Ordinal));
        Assert.InRange(rows.Length, 1, languages.Length - 1);
    }
}
