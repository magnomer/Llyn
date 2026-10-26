using Llyn.UIDemeanor;
using Xunit;

namespace Llyn.Tests;

public sealed class TEditorDrop
{
    private static readonly string[] TEditorDropLanguage = ["English", "French", "Finnish", "German"];

    [Fact]
    public void DropSeek_LettersInAnOpenList_LightTheNextRowStartingWithThem()
    {
        BDrop drop = new(TEditorDropLanguage, 0, true, 8);

        Assert.True(drop.BDropSeek('F'));
        Assert.Equal(1, drop.BDropLit);

        Assert.True(drop.BDropSeek('i'));
        Assert.Equal(2, drop.BDropLit);

        Assert.False(drop.BDropSeek('z'));
        Assert.Equal(2, drop.BDropLit);
    }

    [Fact]
    public void DropSeek_TheSameLetterAgain_StepsToTheNextMatchAndWraps()
    {
        BDrop drop = new(TEditorDropLanguage, 0, true, 8);

        _ = drop.BDropSeek('f');
        _ = drop.BDropSeek('f');
        Assert.Equal(2, drop.BDropLit);

        _ = drop.BDropSeek('f');
        Assert.Equal(1, drop.BDropLit);
    }

    [Fact]
    public void DropSeek_FirstLetterOfTheLitRow_LightsTheNextMatchAfterIt()
    {
        BDrop drop = new(TEditorDropLanguage, 1, true, 8);

        Assert.True(drop.BDropSeek('f'));

        Assert.Equal(2, drop.BDropLit);
    }

    [Fact]
    public void DropDraw_NeitherSideFits_ClampsTheBoxAndItsShadowToTheBounds()
    {
        BDrop drop = new(["a", "b", "c", "d", "e", "f", "g", "h"], 0, true, 8);
        BBlueprintPart bounds = TEditorDropCreate();

        BCanvasText[] texts = [.. drop.BDropDraw(
            new Dictionary<string, IReadOnlyList<string>>(),
            new BCanvasWidth(false),
            bounds,
            (10, 10),
            ([], true))];

        int bottom = bounds.BBlueprintPartRow + bounds.BBlueprintPartHeight;
        Assert.All(texts, text => Assert.InRange(
            text.BCanvasTextRow, bounds.BBlueprintPartRow, bottom - 1));
    }

    [Fact]
    public void DropSeek_ControlKeyOrEmptyList_LightsNothing()
    {
        BDrop drop = new(TEditorDropLanguage, 3, true, 8);

        Assert.False(drop.BDropSeek('\r'));
        Assert.Equal(3, drop.BDropLit);
        Assert.False(new BDrop([], 0, true, 8).BDropSeek('e'));
    }

    [Fact]
    public void DropDraw_RoomBelowTheField_HangsAFramedBoxUnderItWithAShadow()
    {
        BDrop drop = new(TEditorDropLanguage, 1, true, 8);

        BCanvasText[] texts = [.. drop.BDropDraw(
            new Dictionary<string, IReadOnlyList<string>>(),
            new BCanvasWidth(false),
            TEditorDropCreate(),
            (3, 10),
            (["", "x", "", ""], true))];

        BCanvasText roof = texts[0];
        Assert.Equal(4, roof.BCanvasTextRow);
        Assert.Equal(10, roof.BCanvasTextColumn);
        Assert.StartsWith("+-", roof.BCanvasTextValue);
        BCanvasText lit = Assert.Single(texts, static text => text.BCanvasTextRole == "lit");
        Assert.Equal(6, lit.BCanvasTextRow);
        Assert.StartsWith(" French", lit.BCanvasTextValue);
        Assert.EndsWith("x ", lit.BCanvasTextValue);
        Assert.Contains(
            texts, static text => text.BCanvasTextRole == "menu" && text.BCanvasTextValue.StartsWith(" German"));
        Assert.Contains(texts, text => text.BCanvasTextRole == "shadow" && text.BCanvasTextRow == 4 + 6);
        Assert.Contains(texts, text => text.BCanvasTextRole == "shadow"
            && text.BCanvasTextColumn == 10 + roof.BCanvasTextSpan);
    }

    [Fact]
    public void DropDraw_WindowEndsBeforeTheBox_FlipsItAboveTheField()
    {
        BDrop drop = new(TEditorDropLanguage, 0, true, 8);

        BCanvasText[] texts = [.. drop.BDropDraw(
            new Dictionary<string, IReadOnlyList<string>>(),
            new BCanvasWidth(false),
            TEditorDropCreate(),
            (18, 10),
            ([], false))];

        Assert.All(texts, static text => Assert.True(text.BCanvasTextRow < 18));
        Assert.Equal(18 - 7, texts[0].BCanvasTextRow);
        Assert.Contains(texts, static text => text.BCanvasTextRole == "shadow" && text.BCanvasTextRow == 17);
        Assert.Equal("lit-other", Assert.Single(
            texts, static text => text.BCanvasTextValue.StartsWith(" English")).BCanvasTextRole);
    }

    private static BBlueprintPart TEditorDropCreate()
    {
        return new BBlueprintPart
        {
            BBlueprintPartElement = "Text",
            BBlueprintPartTheme = "side",
            BBlueprintPartMargin = [],
            BBlueprintPartTracks = [],
            BBlueprintPartChildren = [],
            BBlueprintPartRow = 2,
            BBlueprintPartColumn = 1,
            BBlueprintPartWidth = 60,
            BBlueprintPartHeight = 18,
        };
    }
}
