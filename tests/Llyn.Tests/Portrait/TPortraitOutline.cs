using Xunit;

namespace Llyn.Tests;

public sealed class TPortraitOutline
{
    [Fact]
    public void OutlineFormat_FullPortrait_KeepsTheDisplayReadingOrder()
    {
        string page = TInterface.TOutlineFormat(TPortraitSample.TPortraitSampleCreate());

        int headword = page.IndexOf("# kindle", System.StringComparison.Ordinal);
        int meanings = page.IndexOf("## Meanings", System.StringComparison.Ordinal);
        int collocations = page.IndexOf("## Collocations", System.StringComparison.Ordinal);
        int incoming = page.IndexOf("## Links here", System.StringComparison.Ordinal);
        int note = page.IndexOf("## Note", System.StringComparison.Ordinal);

        Assert.True(headword >= 0);
        Assert.True(headword < meanings);
        Assert.True(meanings < collocations);
        Assert.True(collocations < incoming);
        Assert.True(incoming < note);
    }

    [Fact]
    public void OutlineFormat_MarkdownInFieldText_EscapesEveryControlCharacter()
    {
        string page = TInterface.TOutlineFormat(TPortraitSample.TPortraitSampleCreate());

        Assert.Contains(@"around a hearth \<cold\>", page);
        Assert.DoesNotContain("\n# Chiefly", page);
    }

    [Fact]
    public void OutlineFormat_MarkdownNote_KeepsTheNoteUnescaped()
    {
        string page = TInterface.TOutlineFormat(TPortraitSample.TPortraitSampleCreate());

        Assert.Contains("## Note\n\nChiefly *literary*.\n\n- poetic\n- archaic\n", page);
    }
}
