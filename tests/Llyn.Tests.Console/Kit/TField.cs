using Llyn.UIDemeanor;
using Xunit;

namespace Llyn.Tests;

public sealed class TField
{
    [Fact]
    public void FieldDraw_LongText_ShowsTail()
    {
        string edited = string.Empty;
        BField field = new(text => edited = text);
        foreach (char letter in "가나다")
        {
            field.BSParleyBodyObserve(new ConsoleKeyInfo(letter, ConsoleKey.A, false, false, false));
        }

        BCanvasText drawn = Assert.Single(field.BSParleyBodyDraw(
            new Dictionary<string, IReadOnlyList<string>>(), 3, 4, 6, 1, false, "dialog", new BCanvasWidth(false)));

        Assert.Equal(new BCanvasText(3, 4, 6, "<나다 ", "field"), drawn);
        Assert.Equal("가나다", field.BFieldText);
        Assert.Equal("가나다", edited);
    }

    [Fact]
    public void FieldObserve_Arrows_EditAtCaret()
    {
        List<string> edits = [];
        BField field = new(edits.Add, "abc");
        Func<ConsoleKey, char, bool> press = (key, letter) =>
            field.BSParleyBodyObserve(new ConsoleKeyInfo(letter, key, false, false, false));

        press(ConsoleKey.LeftArrow, '\0');
        press(ConsoleKey.LeftArrow, '\0');
        press(ConsoleKey.X, 'x');
        Assert.Equal("axbc", field.BFieldText);
        press(ConsoleKey.Home, '\0');
        press(ConsoleKey.Y, 'y');
        Assert.Equal("yaxbc", field.BFieldText);
        press(ConsoleKey.Delete, '\0');
        Assert.Equal("yxbc", field.BFieldText);
        press(ConsoleKey.End, '\0');
        press(ConsoleKey.Backspace, '\b');
        Assert.Equal("yxb", field.BFieldText);
        Assert.Equal(new[] { "axbc", "yaxbc", "yxbc", "yxb" }, edits);

        field.BFieldClear();
        press(ConsoleKey.Z, 'z');
        Assert.Equal("z", field.BFieldText);
    }

    [Fact]
    public void FieldObserve_ControlArrows_StepWords()
    {
        BField field = new(_ => { }, "ab cd éf");
        Func<ConsoleKey, bool, bool> press = (key, control) =>
            field.BSParleyBodyObserve(new ConsoleKeyInfo('\0', key, false, false, control));

        Func<string> mark = () =>
        {
            field.BSParleyBodyObserve(new ConsoleKeyInfo('|', ConsoleKey.Oem5, false, false, false));
            string marked = field.BFieldText;
            field.BSParleyBodyObserve(new ConsoleKeyInfo('\b', ConsoleKey.Backspace, false, false, false));
            return marked;
        };

        Assert.True(press(ConsoleKey.LeftArrow, true));
        Assert.Equal("ab cd |éf", mark());
        press(ConsoleKey.LeftArrow, false);
        Assert.Equal("ab cd| éf", mark());
        press(ConsoleKey.LeftArrow, true);
        Assert.Equal("ab |cd éf", mark());
        press(ConsoleKey.Home, false);
        press(ConsoleKey.RightArrow, true);
        Assert.Equal("ab |cd éf", mark());
        press(ConsoleKey.RightArrow, true);
        press(ConsoleKey.RightArrow, false);
        Assert.Equal("ab cd é|f", mark());
    }

    [Fact]
    public void FieldObserve_ShiftArrows_ReplaceSelection()
    {
        BField field = new(_ => { }, "abcd");
        field.BSParleyBodyObserve(new ConsoleKeyInfo('\0', ConsoleKey.LeftArrow, true, false, false));
        field.BSParleyBodyObserve(new ConsoleKeyInfo('\0', ConsoleKey.LeftArrow, true, false, false));

        BCanvasText[] drawn = [.. field.BSParleyBodyDraw(
            new Dictionary<string, IReadOnlyList<string>>(), 0, 0, 8, 1, false, "dialog", new BCanvasWidth(false))];
        Assert.Equal(new BCanvasText(0, 2, 2, "cd", "chosen"), drawn[^1]);

        field.BSParleyBodyObserve(new ConsoleKeyInfo('x', ConsoleKey.X, false, false, false));
        Assert.Equal("abx", field.BFieldText);
        field.BSParleyBodyObserve(new ConsoleKeyInfo('\0', ConsoleKey.Home, true, false, false));
        field.BSParleyBodyObserve(new ConsoleKeyInfo('\0', ConsoleKey.Delete, false, false, false));
        Assert.Equal(string.Empty, field.BFieldText);
    }

    [Fact]
    public void FieldObserve_EdgeKeys_ReachOwner()
    {
        List<string> reports = [];
        BField field = new(_ => { }, "ab", remove: side => reports.Add($"remove {side}"),
            step: side => reports.Add($"step {side}"));
        Func<ConsoleKey, bool> press = key =>
            field.BSParleyBodyObserve(new ConsoleKeyInfo('\0', key, false, false, false));

        Assert.True(press(ConsoleKey.Delete));
        press(ConsoleKey.Home);
        Assert.True(press(ConsoleKey.Backspace));
        Assert.Equal("ab", field.BFieldText);
        field.BFieldClear();
        Assert.True(press(ConsoleKey.LeftArrow));
        Assert.True(press(ConsoleKey.RightArrow));
        Assert.Equal(string.Empty, field.BFieldText);
        Assert.Equal(new[] { "remove 1", "remove -1", "step -1", "step 1" }, reports);

        BField plain = new(_ => { }, "ab");
        Assert.False(plain.BSParleyBodyObserve(new ConsoleKeyInfo('\0', ConsoleKey.Delete, false, false, false)));
        ConsoleKey[] passed = [ConsoleKey.Enter, ConsoleKey.Escape, ConsoleKey.UpArrow, ConsoleKey.DownArrow];
        foreach (ConsoleKey key in passed)
        {
            Assert.False(plain.BSParleyBodyObserve(new ConsoleKeyInfo(
                key == ConsoleKey.Enter ? '\r' : key == ConsoleKey.Escape ? '\e' : '\0', key, false, false, false)));
        }

        plain.BFieldClear();
        Assert.False(plain.BSParleyBodyObserve(new ConsoleKeyInfo('\0', ConsoleKey.LeftArrow, false, false, false)));
    }

    [Fact]
    public void FieldDraw_CaretInside_ScrollsWithMarks()
    {
        BField field = new(_ => { }, "abcdefghij");
        BCanvasWidth measure = new(false);
        Func<string> draw = () => Assert.Single(field.BSParleyBodyDraw(
            new Dictionary<string, IReadOnlyList<string>>(), 0, 0, 6, 1, false, "dialog", measure)).BCanvasTextValue;

        Assert.Equal("<ghij ", draw());
        field.BSParleyBodyObserve(new ConsoleKeyInfo('\0', ConsoleKey.Home, false, false, false));
        Assert.Equal("abcde>", draw());
        for (int count = 0; count < 5; count++)
        {
            field.BSParleyBodyObserve(new ConsoleKeyInfo('\0', ConsoleKey.RightArrow, false, false, false));
        }

        Assert.Equal("<cdef>", draw());
    }

    [Fact]
    public void FieldDraw_WideCut_ShowsBlank()
    {
        BField field = new(_ => { }, "a가나");

        BCanvasText drawn = Assert.Single(field.BSParleyBodyDraw(
            new Dictionary<string, IReadOnlyList<string>>(), 0, 0, 5, 1, false, "dialog", new BCanvasWidth(false)));

        Assert.Equal("< 나 ", drawn.BCanvasTextValue);
        field.BSParleyBodyObserve(new ConsoleKeyInfo('\0', ConsoleKey.LeftArrow, false, false, false));
        Assert.Equal("<가나", Assert.Single(field.BSParleyBodyDraw(
            new Dictionary<string, IReadOnlyList<string>>(), 0, 0, 5, 1, false, "dialog", new BCanvasWidth(false)))
            .BCanvasTextValue);
    }

    [Fact]
    public void FieldDraw_EmptyText_ShowsHint()
    {
        BLocalizationCatalog catalog = new();
        catalog.BLocalizationCatalogApply(TInterface.TLocalizationLoad("en"));
        BField field = new(_ => { }, catalog: catalog, hint: "Pathfinder.Path");

        BCanvasText[] drawn = [.. field.BSParleyBodyDraw(
            new Dictionary<string, IReadOnlyList<string>>(), 0, 0, 20, 1, true, "dialog", new BCanvasWidth(false))];

        Assert.Equal(new BCanvasText(0, 0, 20, new string(' ', 20), "field"), drawn[0]);
        Assert.Equal(new BCanvasText(0, 0, 20, "Path" + new string(' ', 16), "muted"), drawn[1]);
    }

    [Fact]
    public void FieldObserve_Edit_OwnerSeesNewText()
    {
        List<string> seen = [];
        BField? field = null;
        field = new(_ => seen.Add(field!.BFieldText), "ab");

        field.BSParleyBodyObserve(new ConsoleKeyInfo('c', ConsoleKey.C, false, false, false));
        field.BSParleyBodyObserve(new ConsoleKeyInfo('\b', ConsoleKey.Backspace, false, false, false));

        Assert.Equal(new[] { "abc", "ab" }, seen);
    }

    [Fact]
    public void FieldObserve_BackspaceAtStart_KeepsText()
    {
        int edits = 0;
        BField field = new(_ => edits++, "ab");
        field.BSParleyBodyObserve(new ConsoleKeyInfo('\0', ConsoleKey.Home, false, false, false));

        Assert.False(field.BSParleyBodyObserve(new ConsoleKeyInfo('\b', ConsoleKey.Backspace, false, false, false)));
        Assert.Equal("ab", field.BFieldText);
        Assert.Equal(0, edits);
    }

    [Fact]
    public void FieldClear_Text_EmptiesWithoutEdit()
    {
        int edits = 0;
        BField field = new(_ => edits++, "abc");

        field.BFieldClear();

        Assert.Equal(string.Empty, field.BFieldText);
        Assert.Equal(0, edits);
    }

    [Fact]
    public void FieldDraw_Focused_PlacesCaretMark()
    {
        BField field = new(_ => { }, "가나다");
        BCanvasWidth measure = new(false);
        Func<bool, BCanvasText[]> marks = focused => [.. field.BSParleyBodyDraw(
                new Dictionary<string, IReadOnlyList<string>>(), 3, 4, 6, 1, focused, "dialog", measure)
            .Where(static text => text.BCanvasTextRole == "caret")];

        Assert.Equal([new BCanvasText(3, 9, 0, string.Empty, "caret")], marks(true));
        field.BSParleyBodyObserve(new ConsoleKeyInfo('\0', ConsoleKey.LeftArrow, false, false, false));
        Assert.Equal([new BCanvasText(3, 8, 0, string.Empty, "caret")], marks(true));
        field.BSParleyBodyObserve(new ConsoleKeyInfo('\0', ConsoleKey.Home, false, false, false));
        Assert.Equal([new BCanvasText(3, 4, 0, string.Empty, "caret")], marks(true));
        Assert.Empty(marks(false));

        BLocalizationCatalog catalog = new();
        catalog.BLocalizationCatalogApply(TInterface.TLocalizationLoad("en"));
        BField hinted = new(_ => { }, catalog: catalog, hint: "Pathfinder.Path");
        Assert.Equal(
            new BCanvasText(2, 7, 0, string.Empty, "caret"),
            hinted.BSParleyBodyDraw(
                new Dictionary<string, IReadOnlyList<string>>(), 2, 7, 20, 1, true, "dialog", measure).Last());
    }

    [Fact]
    public void FieldObserve_SurrogateHalves_InsertOneCharacter()
    {
        List<string> edits = [];
        BField field = new(edits.Add, "a");
        Func<char, bool> type = letter =>
            field.BSParleyBodyObserve(new ConsoleKeyInfo(letter, ConsoleKey.NoName, false, false, false));

        Assert.True(type('\uD83D'));
        Assert.Equal("a", field.BFieldText);
        Assert.True(type('\uDE00'));
        Assert.Equal("a\U0001F600", field.BFieldText);
        type('\uDE00');
        field.BSParleyBodyObserve(new ConsoleKeyInfo('\0', ConsoleKey.LeftArrow, false, false, false));
        BCanvasText drawn = field.BSParleyBodyDraw(
            new Dictionary<string, IReadOnlyList<string>>(), 0, 0, 10, 1, true, "dialog", new BCanvasWidth(false))
            .Last();

        Assert.Equal(new BCanvasText(0, 1, 0, string.Empty, "caret"), drawn);
        Assert.Equal("a\U0001F600", field.BFieldText);
        Assert.Equal(new[] { "a\U0001F600" }, edits);
    }
}
