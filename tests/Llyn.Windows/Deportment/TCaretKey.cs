using System.Collections.Generic;
using Xunit;

namespace Llyn.Tests;

public sealed class TCaretKey
{
    [Fact]
    public void CaretKeyApply_BackAtStart_RemovesChipBefore()
    {
        List<int> removed = [];

        bool taken = TInterfaceDeportment.TCaretKeyApply(
            "Back", 0, 3, 0, removed.Add, static _ => true, static () => { });

        Assert.True(taken);
        Assert.Equal([-1], removed);
    }

    [Fact]
    public void CaretKeyApply_RightOnEmptyEntry_MovesAndPlaces()
    {
        List<int> moved = [];
        int placed = 0;

        bool taken = TInterfaceDeportment.TCaretKeyApply(
            "Right", 0, 0, 0, static _ => { }, step => { moved.Add(step); return true; }, () => placed++);

        Assert.True(taken);
        Assert.Equal([1], moved);
        Assert.Equal(1, placed);
    }

    [Fact]
    public void CaretKeyApply_Selection_LeavesKeyToTextBox()
    {
        bool taken = TInterfaceDeportment.TCaretKeyApply(
            "Back", 0, 3, 2, static _ => Assert.Fail("removed"), static _ => true, static () => { });

        Assert.False(taken);
    }
}
