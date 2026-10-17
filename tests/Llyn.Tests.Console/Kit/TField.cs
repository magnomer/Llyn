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
            new Dictionary<string, IReadOnlyList<string>>(), 3, 4, 6, true, "dialog"));

        Assert.Equal(new BCanvasText(3, 4, 6, "나다  ", "field"), drawn);
        Assert.Equal("가나다", field.BFieldText);
        Assert.Equal("가나다", edited);
    }
}
