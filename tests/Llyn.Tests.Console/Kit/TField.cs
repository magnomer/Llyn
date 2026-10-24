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
            new Dictionary<string, IReadOnlyList<string>>(), 3, 4, 6, 1, true, "dialog", new BCanvasWidth(false)));

        Assert.Equal(new BCanvasText(3, 4, 6, "나다  ", "field"), drawn);
        Assert.Equal("가나다", field.BFieldText);
        Assert.Equal("가나다", edited);
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
}
