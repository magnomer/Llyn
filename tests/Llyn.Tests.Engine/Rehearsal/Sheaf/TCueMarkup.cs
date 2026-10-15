using Xunit;

namespace Llyn.Tests;

public sealed class TCueMarkup
{
    [Fact]
    public void CueMarkupFormat_BarInText_EscapesBar()
    {
        Assert.Equal("a\u2223b", TInterface.TCueMarkupFormat("a|b"));
    }
}
