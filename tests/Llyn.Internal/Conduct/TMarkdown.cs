using System;
using Llyn.Conduct;
using Xunit;

namespace Llyn.Tests;

public sealed class TMarkdown
{
    [Fact]
    public void MarkdownParse_BoldItem_ReadsListedBoldSpan()
    {
        CMarkdownBlock block = Assert.Single(TInterfaceMarkdown.TMarkdownBlockRead("- **bold** item"));

        Assert.True(block.CMarkdownBlockListed);
        Assert.False(block.CMarkdownBlockHeaded);
        Assert.Equal("bold", block.CMarkdownBlockSpan[0].CMarkdownSpanText);
        Assert.True(block.CMarkdownBlockSpan[0].CMarkdownSpanBold);
    }

    [Fact]
    public void MarkdownParse_LinkSpan_KeepsAddress()
    {
        CMarkdownBlock block = Assert.Single(TInterfaceMarkdown.TMarkdownBlockRead("[site](https://example.com)"));

        Assert.Equal(new Uri("https://example.com"), block.CMarkdownBlockSpan[0].CMarkdownSpanAddress);
    }
}
