using Llyn.Core;
using Xunit;

namespace Llyn.Tests;

public sealed class TStencilToken
{
    [Fact]
    public void StencilTokenParse_LiteralAndField_SplitsBoth()
    {
        IReadOnlyList<LStencilToken> tokens = TInterface.TStencilTokenParse("Word: {headword}");

        Assert.Equal(2, tokens.Count);
        Assert.Equal("Word: ", tokens[0].LStencilTokenLiteral);
        Assert.Null(tokens[0].LStencilTokenField);
        Assert.Null(tokens[1].LStencilTokenLiteral);
        Assert.Equal(LStencilField.LStencilFieldHeadword, tokens[1].LStencilTokenField);
        Assert.Null(tokens[1].LStencilTokenIndex);
        Assert.False(tokens[1].LStencilTokenAll);
    }

    [Fact]
    public void StencilTokenParse_NthField_ReadsIndex()
    {
        LStencilToken token = Assert.Single(TInterface.TStencilTokenParse("{definition:2}"));

        Assert.Equal(LStencilField.LStencilFieldDefinition, token.LStencilTokenField);
        Assert.Equal(2, token.LStencilTokenIndex);
        Assert.False(token.LStencilTokenAll);
    }

    [Fact]
    public void StencilTokenParse_AllField_SetsAll()
    {
        LStencilToken token = Assert.Single(TInterface.TStencilTokenParse("{Definition:all}"));

        Assert.Equal(LStencilField.LStencilFieldDefinition, token.LStencilTokenField);
        Assert.Null(token.LStencilTokenIndex);
        Assert.True(token.LStencilTokenAll);
    }

    [Theory]
    [InlineData("a {colour} b")]
    [InlineData("a {definition:0} b")]
    [InlineData("a {definition:2:3} b")]
    [InlineData("a {{headword b")]
    public void StencilTokenParse_UnknownField_KeepsLiteral(string body)
    {
        LStencilToken token = Assert.Single(TInterface.TStencilTokenParse(body));

        Assert.Equal(body, token.LStencilTokenLiteral);
        Assert.Null(token.LStencilTokenField);
    }
}
