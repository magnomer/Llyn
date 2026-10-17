using Llyn.Conduct;
using Llyn.UIDemeanor;
using Xunit;

namespace Llyn.Tests;

public sealed class TCustoms
{
    [Fact]
    public void EnvoyCustomsRead_ModePicked_AnswersImport()
    {
        TConsoleFake console = TCustomsRun(
            ConsoleKey.Enter, ConsoleKey.DownArrow, ConsoleKey.Enter, ConsoleKey.RightArrow, ConsoleKey.Enter,
            ConsoleKey.Enter, ConsoleKey.F2, ConsoleKey.F10);

        object? answer = TEnvoy.TEnvoyRun(console, static envoy => envoy.CEnvoyCustomsRead(TCustomsPrepare([7, 8])));

        Assert.Equal(true, answer);
    }

    [Fact]
    public void EnvoyCustomsRead_NotReady_IgnoresImport()
    {
        TConsoleFake console = TCustomsRun(
            ConsoleKey.Enter, ConsoleKey.DownArrow, ConsoleKey.Enter, ConsoleKey.F2, ConsoleKey.Escape, ConsoleKey.F10);

        object? answer = TEnvoy.TEnvoyRun(console, static envoy => envoy.CEnvoyCustomsRead(TCustomsPrepare([7, 8])));

        Assert.Equal(false, answer);
    }

    [Fact]
    public void EnvoyCustomsRead_Escape_Declines()
    {
        TConsoleFake console = TCustomsRun(ConsoleKey.Escape, ConsoleKey.F10);

        object? answer = TEnvoy.TEnvoyRun(console, static envoy => envoy.CEnvoyCustomsRead(TCustomsPrepare([7])));

        Assert.Equal(false, answer);
        string shot = string.Join('\n', console.TConsoleFakeShots[0]);
        Assert.Contains(" Import entries ", shot);
        Assert.Contains("═ 1 of 1 ═", shot);
    }

    [Fact]
    public void CustomsDraw_Replace_DrawsLoss()
    {
        BLocalizationCatalog catalog = new();
        catalog.BLocalizationCatalogApply(TInterface.TLocalizationLoad("en"));
        BSCustoms customs = new(TCustomsPrepare([7]), catalog, new BSParleyButton("Import"));
        foreach (ConsoleKeyInfo key in TCustomsRun(
            ConsoleKey.Enter, ConsoleKey.DownArrow, ConsoleKey.Enter).TConsoleFakeKeys)
        {
            customs.BSParleyBodyObserve(key);
        }

        List<BCanvasText> texts =
            [.. customs.BSParleyBodyDraw(new Dictionary<string, IReadOnlyList<string>>(), 0, 0, 72, true, "dialog")];

        Assert.Contains(new BCanvasText(10, 0, 72, "17 meanings, 27 collocations dropped", "dialog"), texts);
        Assert.Contains(texts, static text => text is { BCanvasTextValue: "[Replace ]", BCanvasTextRole: "lit" });
        Assert.Contains(texts, static text => text.BCanvasTextValue.StartsWith("[ember #7", StringComparison.Ordinal));
    }

    [Fact]
    public void CustomsDraw_DropOnLastRow_StaysInBody()
    {
        List<BCanvasText> texts = TCustomsDropDraw(0);

        Assert.All(texts, static text => Assert.InRange(text.BCanvasTextRow, 0, BSCustoms.BSCustomsHeight - 1));
        Assert.Equal(
            5,
            texts.Count(static text => text.BCanvasTextValue.StartsWith(" ember #", StringComparison.Ordinal)));
    }

    [Fact]
    public void CustomsDraw_LitPastPage_ScrollsDrop()
    {
        List<BCanvasText> texts = TCustomsDropDraw(7);

        Assert.Contains(texts, static text => text.BCanvasTextRole == "lit"
            && text.BCanvasTextValue.StartsWith(" ember #8 ", StringComparison.Ordinal));
        Assert.DoesNotContain(
            texts,
            static text => text.BCanvasTextValue.StartsWith(" ember #1 ", StringComparison.Ordinal));
    }

    [Fact]
    public void EnvoyOmissionShow_Rows_DrawsLines()
    {
        TConsoleFake console = TCustomsRun(ConsoleKey.Enter, ConsoleKey.F10);

        object? answer = TEnvoy.TEnvoyRun(console, static envoy =>
        {
            envoy.CEnvoyOmissionShow([new("12", "<etymology> holds no text"), new("0", "unknown tag <pinyin2>")]);
            return null;
        });

        Assert.Null(answer);
        string shot = string.Join('\n', console.TConsoleFakeShots[0]);
        Assert.Contains("Some parts of the file could not be placed:", shot);
        Assert.Contains("    12  <etymology> holds no text", shot);
        Assert.Contains("     0  unknown tag <pinyin2>", shot);
        Assert.Contains("[ Close ]", shot);
    }

    private static TConsoleFake TCustomsRun(params ConsoleKey[] keys)
    {
        TConsoleFake console = new() { TConsoleFakeTerminal = true };
        foreach (ConsoleKey key in keys)
        {
            char letter = key switch
            {
                ConsoleKey.Enter => '\r',
                ConsoleKey.Escape => '\e',
                _ => '\0',
            };
            console.TConsoleFakeKeys.Add(new ConsoleKeyInfo(letter, key, false, false, false));
        }

        return console;
    }

    private static List<BCanvasText> TCustomsDropDraw(int steps)
    {
        BLocalizationCatalog catalog = new();
        catalog.BLocalizationCatalogApply(TInterface.TLocalizationLoad("en"));
        long[] ids = [1, 2, 3, 4, 5, 6, 7, 8, 9];
        CSCustoms prepared = TInterfaceConductDialog.TCustomsCreate(
            [
                .. Enumerable.Repeat(new CMarkupEntry("English", "ash", [1]), 6),
                new CMarkupEntry("English", "ember", ids),
            ],
            [.. ids.Select(static id => new CMarkupTarget(id, (int)id + 10, (int)id + 20))]);
        BSCustoms customs = new(prepared, catalog, new BSParleyButton("Import"));
        ConsoleKey[] keys =
        [
            .. Enumerable.Repeat(ConsoleKey.DownArrow, 6),
            ConsoleKey.Enter, ConsoleKey.DownArrow, ConsoleKey.Enter, ConsoleKey.RightArrow, ConsoleKey.Enter,
            .. Enumerable.Repeat(ConsoleKey.DownArrow, steps),
        ];
        foreach (ConsoleKeyInfo key in TCustomsRun(keys).TConsoleFakeKeys)
        {
            customs.BSParleyBodyObserve(key);
        }

        return [.. customs.BSParleyBodyDraw(new Dictionary<string, IReadOnlyList<string>>(), 0, 0, 72, true, "dialog")];
    }

    private static CSCustoms TCustomsPrepare(long[] ids) =>
        TInterfaceConductDialog.TCustomsCreate(
            [new CMarkupEntry("English", "ember", ids)],
            [.. ids.Select(static id => new CMarkupTarget(id, (int)id + 10, (int)id + 20))]);
}
