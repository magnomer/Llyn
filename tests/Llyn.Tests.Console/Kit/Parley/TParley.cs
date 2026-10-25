using Llyn.Conduct;
using Llyn.UIDemeanor;
using Llyn.UITerminal;
using Xunit;

namespace Llyn.Tests;

public sealed class TParley
{
    [Fact]
    public void ParleyDraw_Centred_DrawsShadowBelowAndRight()
    {
        ABootstrap bootstrap = new();
        BSParley parley = new(
            bootstrap.BBlueprintParts["ASParley"], "Llyn", "dialog", [new("Text", false)], [new("OK")], 0);

        IReadOnlyList<BCanvasText> texts =
            parley.BSParleyDraw(bootstrap.BBlueprintGlyphs, 80, 24, new BCanvasWidth(false));

        BCanvasText[] shadow =
        [
            .. Enumerable.Range(9, 6).Select(static row => new BCanvasText(row, 68, 1, " ", "shadow")),
            new BCanvasText(15, 11, 58, new string(' ', 58), "shadow"),
        ];
        Assert.Equal(shadow, texts.Where(static text => text.BCanvasTextRole == "shadow"));
    }

    [Fact]
    public void ParleyDraw_WiderThanConsole_ShrinksLessTwoCells()
    {
        ABootstrap bootstrap = new();
        BSParley parley = new(
            bootstrap.BBlueprintParts["ASParley"], "Llyn", "dialog", [new("Text", false)], [new("OK")], 0);

        IReadOnlyList<BCanvasText> texts =
            parley.BSParleyDraw(bootstrap.BBlueprintGlyphs, 40, 24, new BCanvasWidth(false));

        Assert.Equal((8, 0, 38), (texts[0].BCanvasTextRow, texts[0].BCanvasTextColumn, texts[0].BCanvasTextSpan));
    }

    [Fact]
    public void ParleyDraw_Anchored_PlacesAtAnchor()
    {
        ABootstrap bootstrap = new();
        BSParley parley = new(
            bootstrap.BBlueprintParts["ASParley"], "Llyn", "dialog", [new("Text", false)], [new("OK")], 0)
        {
            BSParleyAnchor = (2, 5),
        };

        IReadOnlyList<BCanvasText> texts =
            parley.BSParleyDraw(bootstrap.BBlueprintGlyphs, 80, 24, new BCanvasWidth(false));

        Assert.Equal((2, 5), (texts[0].BCanvasTextRow, texts[0].BCanvasTextColumn));
    }

    [Fact]
    public void ParleyDraw_PathPastText_DrawsAllRows()
    {
        ABootstrap bootstrap = new();
        string[] rows = [.. Enumerable.Range(0, 16).Select(static at => new string((char)('a' + at), 50) + "\\")];
        BSParley parley = new(
            bootstrap.BBlueprintParts["ASParley"],
            "Llyn",
            "alert",
            [new(string.Concat(rows), true)],
            [new("OK")],
            0);

        IReadOnlyList<BCanvasText> texts =
            parley.BSParleyDraw(bootstrap.BBlueprintGlyphs, 80, 24, new BCanvasWidth(false));

        Assert.Equal(rows, texts.Select(static text => text.BCanvasTextValue).Where(rows.Contains));
    }

    [Fact]
    public void ParleyDraw_DisabledButton_DrawsDisabledRole()
    {
        ABootstrap bootstrap = new();
        BSParley parley = new(
            bootstrap.BBlueprintParts["ASParley"],
            "Llyn",
            "dialog",
            [new("Text", false)],
            [new("Create") { BSParleyButtonEnabled = false }, new("Cancel")],
            1);

        IReadOnlyList<BCanvasText> texts =
            parley.BSParleyDraw(bootstrap.BBlueprintGlyphs, 80, 24, new BCanvasWidth(false));

        Assert.Equal(["disabled", "dialog"], texts.TakeLast(2).Select(static text => text.BCanvasTextRole));
    }

    [Fact]
    public void ParleyDraw_LitButton_DrawsLitRole()
    {
        ABootstrap bootstrap = new();
        BSParley parley = new(
            bootstrap.BBlueprintParts["ASParley"],
            "Llyn",
            "alert",
            [new("Text", false)],
            [new("Yes"), new("No")],
            1);

        IReadOnlyList<BCanvasText> texts =
            parley.BSParleyDraw(bootstrap.BBlueprintGlyphs, 80, 24, new BCanvasWidth(false));

        Assert.Equal(["lit", "alert"], texts.TakeLast(2).Select(static text => text.BCanvasTextRole));
    }

    [Fact]
    public async Task ParleyKeyObserve_RightPastDisabled_SkipsIt()
    {
        BSParley parley = new(
            new ABootstrap().BBlueprintParts["ASParley"],
            "Llyn",
            "dialog",
            [new("Text", false)],
            [new("One"), new("Two") { BSParleyButtonEnabled = false }, new("Three")],
            2);

        parley.BSParleyKeyObserve(new ConsoleKeyInfo('\0', ConsoleKey.RightArrow, false, false, false));
        parley.BSParleyKeyObserve(new ConsoleKeyInfo('\r', ConsoleKey.Enter, false, false, false));

        Assert.Equal(2, await parley.BSParleyAnswer);
    }

    [Fact]
    public void ParleyCreate_NoButtonsNoBody_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new BSParley(
            new ABootstrap().BBlueprintParts["ASParley"], "Llyn", "dialog", [new("Text", false)], [], 0));
    }

    [Fact]
    public void ParleyDraw_NoButtonsWithBody_DrawsTheBodyOverTheButtonRow()
    {
        ABootstrap bootstrap = new();
        BSParley parley = new(
            bootstrap.BBlueprintParts["ASParley"], "Order", "dialog", [], [], 0, TParleyOrderCreate([]))
        {
            BSParleyAnchor = (3, 4),
        };

        IReadOnlyList<BCanvasText> texts =
            parley.BSParleyDraw(bootstrap.BBlueprintGlyphs, 80, 24, new BCanvasWidth(false));

        Assert.Equal(
            [(5, "(•) Headword A → Z"), (6, "( ) Headword Z → A")],
            texts.Where(static text => text.BCanvasTextValue.Contains("Headword", StringComparison.Ordinal))
                .Select(static text => (text.BCanvasTextRow, text.BCanvasTextValue.TrimEnd())));
        Assert.Equal(
            9, texts.Where(static text => text.BCanvasTextRole == "shadow").Max(static text => text.BCanvasTextRow));
        Assert.DoesNotContain(texts, static text => text.BCanvasTextValue.StartsWith("[ ", StringComparison.Ordinal));
    }

    [Fact]
    public async Task ParleyKeyObserve_NoButtons_BodyTakesEnterAndEscapeAnswersSafe()
    {
        List<CCatalogOrder> chosen = [];
        BSParley parley = new(
            new ABootstrap().BBlueprintParts["ASParley"], "Order", "dialog", [], [], 0, TParleyOrderCreate(chosen));

        parley.BSParleyKeyObserve(new ConsoleKeyInfo('\t', ConsoleKey.Tab, false, false, false));
        parley.BSParleyKeyObserve(new ConsoleKeyInfo('\0', ConsoleKey.RightArrow, false, false, false));
        parley.BSParleyKeyObserve(new ConsoleKeyInfo('\0', ConsoleKey.DownArrow, false, false, false));
        parley.BSParleyKeyObserve(new ConsoleKeyInfo('\r', ConsoleKey.Enter, false, false, false));
        parley.BSParleyKeyObserve(new ConsoleKeyInfo('\e', ConsoleKey.Escape, false, false, false));

        Assert.Equal([CCatalogOrder.CCatalogOrderReverse], chosen);
        Assert.Equal(0, await parley.BSParleyAnswer);
        Assert.DoesNotContain(
            parley.BSParleyKeymapRead(),
            static row => row.BKeymapRowKey is ConsoleKey.Tab or ConsoleKey.LeftArrow
                && row.BKeymapRowLive() != BKeymapLive.BKeymapLiveHidden);
    }

    [Fact]
    public async Task ParleyKeyObserve_BodyWithButton_TabHandsTheKeysToTheButton()
    {
        List<CCatalogOrder> chosen = [];
        BSParley parley = new(
            new ABootstrap().BBlueprintParts["ASParley"],
            "Order",
            "dialog",
            [],
            [new("Close")],
            0,
            TParleyOrderCreate(chosen));

        parley.BSParleyKeyObserve(new ConsoleKeyInfo('\t', ConsoleKey.Tab, false, false, false));
        parley.BSParleyKeyObserve(new ConsoleKeyInfo('\r', ConsoleKey.Enter, false, false, false));

        Assert.Equal(0, await parley.BSParleyAnswer);
        Assert.Empty(chosen);
    }

    private static BSChoiceOrder TParleyOrderCreate(List<CCatalogOrder> chosen)
    {
        BLocalizationCatalog catalog = new();
        catalog.BLocalizationCatalogApply(TInterface.TLocalizationLoad("en"));
        return new BSChoiceOrder(
            catalog,
            "Order",
            [CCatalogOrder.CCatalogOrderHeadword, CCatalogOrder.CCatalogOrderReverse],
            static () => CCatalog.CCatalogOrderFormat(CCatalogOrder.CCatalogOrderHeadword),
            chosen.Add,
            new BCanvasWidth(false));
    }

    [Fact]
    public async Task ParleyKeyObserve_Escape_AnswersSafe()
    {
        BSParley parley = new(
            new ABootstrap().BBlueprintParts["ASParley"],
            "Llyn",
            "dialog",
            [new("Text", false)],
            [new("Yes"), new("No")],
            1);

        parley.BSParleyKeyObserve(new ConsoleKeyInfo('\e', ConsoleKey.Escape, false, false, false));

        Assert.Equal(1, await parley.BSParleyAnswer);
    }
}
