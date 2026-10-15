using Llyn.Core;
using Xunit;

namespace Llyn.Tests;

public sealed class TStencilBody
{
    [Fact]
    public void StencilBodyAppend_EmptyBody_WritesToken()
    {
        string body = TInterface.TStencilBodyAppend(string.Empty, LStencilField.LStencilFieldHeadword);

        Assert.Equal("{headword}", body);
    }

    [Fact]
    public void StencilBodyAppend_OneToken_AddsLineBreak()
    {
        string body = TInterface.TStencilBodyAppend("{headword}", LStencilField.LStencilFieldPronunciation);

        Assert.Equal("{headword}\n{pronunciation}", body);
    }

    [Fact]
    public void StencilBodyRemove_MiddleToken_KeepsOthers()
    {
        string body = TInterface.TStencilBodyRemove("{headword}\n{definition:all}\n{pronunciation}", 1);

        Assert.Equal("{headword}\n{pronunciation}", body);
    }

    [Fact]
    public void StencilBodyRemove_LastToken_ReturnsEmpty()
    {
        string body = TInterface.TStencilBodyRemove("{headword}", 0);

        Assert.Empty(body);
    }

    [Fact]
    public void StencilBodySet_AllJoin_WritesAllSuffix()
    {
        string body = TInterface.TStencilBodySet("{headword}\n{definition}", 1, all: true);

        Assert.Equal("{headword}\n{definition:all}", body);
    }

    [Fact]
    public void StencilBodySet_FirstJoin_DropsSuffix()
    {
        string body = TInterface.TStencilBodySet("{definition:all}", 0, all: false);

        Assert.Equal("{definition}", body);
    }

    [Fact]
    public void StencilBodyCheck_FieldsAndBreaks_ReturnsTrue()
    {
        Assert.True(TInterface.TStencilBodyCheck("{headword}\n{definition:all}\r\n{image}"));
    }

    [Fact]
    public void StencilBodyCheck_LiteralText_ReturnsFalse()
    {
        Assert.False(TInterface.TStencilBodyCheck("Word: {headword}"));
    }

    [Fact]
    public void StencilBodyCheck_NthIndex_ReturnsFalse()
    {
        Assert.False(TInterface.TStencilBodyCheck("{headword}\n{definition:2}"));
    }
}
