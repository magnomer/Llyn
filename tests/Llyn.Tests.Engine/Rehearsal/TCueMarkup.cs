using Xunit;

namespace Llyn.Tests;

public sealed class TCueMarkup
{
    [Fact]
    public void CueMarkupResolve_BoldItalic_WritesPhoneTags()
    {
        Assert.Equal("<b>big</b> and <i>small</i>", TInterface.TCueMarkupResolve("**big** and *small*"));
    }

    [Fact]
    public void CueMarkupResolve_LinkAndCode_KeepsPlainText()
    {
        Assert.Equal("site and code", TInterface.TCueMarkupResolve("[site](https://example.test) and `code`"));
    }

    [Fact]
    public void CueMarkupResolve_TwoParagraphs_JoinsWithBar()
    {
        Assert.Equal("one|two|three", TInterface.TCueMarkupResolve("one\n\ntwo\nthree\n\n---"));
    }

    [Fact]
    public void CueMarkupFormat_BarInText_EscapesBar()
    {
        Assert.Equal("a\u2223b", TInterface.TCueMarkupFormat("a|b"));
        Assert.Equal("a\u2223b", TInterface.TCueMarkupResolve("a|b"));
    }
}
