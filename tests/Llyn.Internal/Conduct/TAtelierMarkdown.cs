using System;
using System.Collections.Generic;
using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TAtelierMarkdown
{
    [Fact]
    public void MarkdownParse_BoldItem_ReadsListedBoldSpan()
    {
        using LEngine engine = TRigFake.TRigFakeStart(TRigFake.TRigFakeBuild());
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(
            engine,
            new Dictionary<string, Func<object?[]?, object?>>
            {
                ["LEngineMarkdownParse"] = args => TInterface.TMarkdownParse((string?)args![0]),
            });

        CMarkdownBlock block = Assert.Single(atelier.CAtelierMarkdown.CMarkdownParse("- **bold** item"));

        Assert.True(block.CMarkdownBlockListed);
        Assert.False(block.CMarkdownBlockHeaded);
        Assert.Equal("bold", block.CMarkdownBlockSpan[0].CMarkdownSpanText);
        Assert.True(block.CMarkdownBlockSpan[0].CMarkdownSpanBold);
    }

    [Fact]
    public void MarkdownRead_LinkSpan_KeepsAddress()
    {
        IReadOnlyList<LMarkdownBlock> blocks = TInterface.TMarkdownParse("[site](https://example.com)");

        CMarkdownBlock block = CMarkdown.CMarkdownRead(Assert.Single(blocks));

        Assert.Equal(new Uri("https://example.com"), block.CMarkdownBlockSpan[0].CMarkdownSpanAddress);
    }
}
