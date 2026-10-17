using Llyn.UIDemeanor;
using Xunit;

namespace Llyn.Tests;

public sealed class TPager
{
    [Fact]
    public void PagerObserve_Tab_LightsNextLink()
    {
        BPager pager = new(() => [[null, 1, 2], [null], [null]], 0, 2);

        bool taken = pager.BPagerObserve(new ConsoleKeyInfo('\t', ConsoleKey.Tab, false, false, false));

        Assert.True(taken);
        Assert.Equal(1, pager.BPagerLit);
        Assert.Equal(1, pager.BPagerTop);
    }

    [Fact]
    public void PagerObserve_EnterThenBackspace_RestoresPlace()
    {
        BPager pager = new(() => [[null, null, null, 1], [null]], 0, 2);
        pager.BPagerObserve(new ConsoleKeyInfo('\0', ConsoleKey.DownArrow, false, false, false));

        pager.BPagerObserve(new ConsoleKeyInfo('\r', ConsoleKey.Enter, false, false, false));
        Assert.Equal((1, 0), (pager.BPagerPage, pager.BPagerTop));
        pager.BPagerObserve(new ConsoleKeyInfo('\b', ConsoleKey.Backspace, false, false, false));

        Assert.Equal((0, 1, 0), (pager.BPagerPage, pager.BPagerTop, pager.BPagerLit));
    }

    [Fact]
    public void PagerObserve_End_ClampsTop()
    {
        BPager pager = new(() => [[null, null, null, null, null, null, null, null, null, null]], 0, 4);

        pager.BPagerObserve(new ConsoleKeyInfo('\0', ConsoleKey.End, false, false, false));
        Assert.Equal(6, pager.BPagerTop);
        pager.BPagerObserve(new ConsoleKeyInfo('\0', ConsoleKey.PageDown, false, false, false));
        Assert.Equal(6, pager.BPagerTop);
        pager.BPagerObserve(new ConsoleKeyInfo('\0', ConsoleKey.PageUp, false, false, false));
        Assert.Equal(2, pager.BPagerTop);
        pager.BPagerObserve(new ConsoleKeyInfo('\0', ConsoleKey.Home, false, false, false));
        pager.BPagerObserve(new ConsoleKeyInfo('\0', ConsoleKey.UpArrow, false, false, false));

        Assert.Equal(0, pager.BPagerTop);
        Assert.False(pager.BPagerObserve(new ConsoleKeyInfo('\e', ConsoleKey.Escape, false, false, false)));
    }

    [Fact]
    public void PagerObserve_ShiftDown_KeepsTop()
    {
        BPager pager = new(() => [[null, null, null, null]], 0, 2);

        bool taken = pager.BPagerObserve(new ConsoleKeyInfo('\0', ConsoleKey.DownArrow, true, false, false));

        Assert.False(taken);
        Assert.Equal(0, pager.BPagerTop);
    }

    [Fact]
    public void PagerShow_SamePage_KeepsTrail()
    {
        BPager pager = new(() => [[null], [null]], 0, 3);

        pager.BPagerShow(1);
        pager.BPagerShow(1);
        pager.BPagerObserve(new ConsoleKeyInfo('\b', ConsoleKey.Backspace, false, false, false));
        Assert.Equal(0, pager.BPagerPage);
        pager.BPagerObserve(new ConsoleKeyInfo('\b', ConsoleKey.Backspace, false, false, false));

        Assert.Equal(0, pager.BPagerPage);
    }
}
