using System.ComponentModel;
using Llyn.Core;
using Llyn.UIDeportment;
using Xunit;

namespace Llyn.Tests;

public sealed class TIndexItem
{
    [Fact]
    public void IndexItemBuild_ChosenRow_CarriesMark()
    {
        IReadOnlyList<LIndexItem> items = TInterfaceDeportment.TIndexItemBuild(
            [TIndexRowCreate(1, "Latin", false), TIndexRowCreate(2, "Latin", true)]);

        Assert.Equal([1L, 2L], items.Select(item => item.LIndexItemId));
        Assert.False(items[0].LIndexItemChosen);
        Assert.True(items[1].LIndexItemChosen);
        Assert.Null(items[1].LIndexItemFlag);
    }

    [Fact]
    public void IndexItemMatch_DifferentMark_Matches()
    {
        LIndexItem held = TIndexItemCreate(TIndexRowCreate(1, "Latin", false));
        LIndexItem fresh = TIndexItemCreate(TIndexRowCreate(1, "Latin", true));

        Assert.True(TInterfaceDeportment.TIndexItemMatch(held, fresh));
    }

    [Fact]
    public void IndexItemMatch_DifferentLanguage_Refuses()
    {
        LIndexItem held = TIndexItemCreate(TIndexRowCreate(1, "Latin", false));
        LIndexItem fresh = TIndexItemCreate(TIndexRowCreate(1, "Greek", false));

        Assert.False(TInterfaceDeportment.TIndexItemMatch(held, fresh));
    }

    [Fact]
    public void IndexItemSync_MovedMark_RaisesChange()
    {
        LIndexItem held = TIndexItemCreate(TIndexRowCreate(1, "Latin", false));
        LIndexItem fresh = TIndexItemCreate(TIndexRowCreate(1, "Latin", true));
        List<string?> raised = [];
        held.PropertyChanged += (_, change) => raised.Add(change.PropertyName);

        TInterfaceDeportment.TIndexItemSync(held, fresh);

        Assert.True(held.LIndexItemChosen);
        Assert.Equal([nameof(LIndexItem.LIndexItemChosen)], raised);
    }

    [Fact]
    public void IndexItemSync_SameMark_RaisesNothing()
    {
        LIndexItem held = TIndexItemCreate(TIndexRowCreate(1, "Latin", true));
        LIndexItem fresh = TIndexItemCreate(TIndexRowCreate(1, "Latin", true));
        List<PropertyChangedEventArgs> raised = [];
        held.PropertyChanged += (_, change) => raised.Add(change);

        TInterfaceDeportment.TIndexItemSync(held, fresh);

        Assert.True(held.LIndexItemChosen);
        Assert.Empty(raised);
    }

    [Fact]
    public void IndexNeighbourFind_NoChosenDown_PicksFirst()
    {
        IReadOnlyList<LIndexItem> items = TIndexListCreate(-1);

        Assert.Equal(1L, TInterfaceDeportment.TIndexNeighbourFind(items, true));
    }

    [Fact]
    public void IndexNeighbourFind_NoChosenUp_PicksFirst()
    {
        IReadOnlyList<LIndexItem> items = TIndexListCreate(-1);

        Assert.Equal(1L, TInterfaceDeportment.TIndexNeighbourFind(items, false));
    }

    [Fact]
    public void IndexNeighbourFind_MiddleChosen_MovesOneRow()
    {
        IReadOnlyList<LIndexItem> items = TIndexListCreate(1);

        Assert.Equal(3L, TInterfaceDeportment.TIndexNeighbourFind(items, true));
        Assert.Equal(1L, TInterfaceDeportment.TIndexNeighbourFind(items, false));
    }

    [Fact]
    public void IndexNeighbourFind_EndChosen_StaysAtEnd()
    {
        Assert.Equal(3L, TInterfaceDeportment.TIndexNeighbourFind(TIndexListCreate(2), true));
        Assert.Equal(1L, TInterfaceDeportment.TIndexNeighbourFind(TIndexListCreate(0), false));
    }

    [Fact]
    public void IndexNeighbourFind_EmptyList_AnswersNull()
    {
        Assert.Null(TInterfaceDeportment.TIndexNeighbourFind([], true));
    }

    private static IReadOnlyList<LIndexItem> TIndexListCreate(int chosen) =>
        TInterfaceDeportment.TIndexItemBuild(
            [TIndexRowCreate(1, "Latin", chosen == 0), TIndexRowCreate(2, "Latin", chosen == 1),
                TIndexRowCreate(3, "Latin", chosen == 2)]);

    private static LVistaRow TIndexRowCreate(long id, string language, bool chosen) =>
        new(id, "aqua", language, null, "aqua", chosen);

    private static LIndexItem TIndexItemCreate(LVistaRow row) => TInterfaceDeportment.TIndexItemBuild([row])[0];
}
