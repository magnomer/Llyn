using Llyn.UIDemeanor;
using Xunit;

namespace Llyn.Tests;

public sealed class TCanvasWidth
{
    [Fact]
    public void CanvasWidthResolve_Han_ReturnsTwo()
    {
        Assert.Equal(2, new BCanvasWidth(false).BCanvasWidthResolve("水"));
    }

    [Fact]
    public void CanvasWidthResolve_Latin_ReturnsOne()
    {
        Assert.Equal(1, new BCanvasWidth(false).BCanvasWidthResolve("a"));
    }

    [Fact]
    public void CanvasWidthResolve_CombiningMark_ReturnsZero()
    {
        Assert.Equal(0, new BCanvasWidth(false).BCanvasWidthResolve("́"));
    }

    [Fact]
    public void CanvasWidthResolve_AmbiguousNarrow_ReturnsOne()
    {
        Assert.Equal(1, new BCanvasWidth(false).BCanvasWidthResolve("ǐ"));
    }

    [Fact]
    public void CanvasWidthResolve_AmbiguousWide_ReturnsTwo()
    {
        Assert.Equal(2, new BCanvasWidth(true).BCanvasWidthResolve("ǐ"));
    }

    [Fact]
    public void CanvasWidthResolve_Emoji_ReturnsTwo()
    {
        Assert.Equal(2, new BCanvasWidth(false).BCanvasWidthResolve("\U0001F600"));
    }

    [Fact]
    public void CanvasWidthDivide_HangulWords_BreaksAtSpaces()
    {
        Assert.Equal(["이", "콘솔은", "가로"], new BCanvasWidth(false).BCanvasWidthDivide("이 콘솔은 가로", 6));
    }

    [Fact]
    public void CanvasWidthDivide_HanKana_BreaksBetweenCharacters()
    {
        Assert.Equal(["漢字か", "な"], new BCanvasWidth(false).BCanvasWidthDivide("漢字かな", 6));
    }

    [Fact]
    public void CanvasWidthDivide_LongHangul_BreaksBetweenClusters()
    {
        Assert.Equal(["콘솔", "콘솔"], new BCanvasWidth(false).BCanvasWidthDivide("콘솔콘솔", 5));
    }

    [Fact]
    public void CanvasWidthResolve_MixedText_SumsClusters()
    {
        Assert.Equal(6, new BCanvasWidth(false).BCanvasWidthResolve("水áǐ❤️"));
    }
}
