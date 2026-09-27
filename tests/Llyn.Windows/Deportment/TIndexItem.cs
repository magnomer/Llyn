using System.ComponentModel;
using Llyn.Conduct;
using Llyn.UIDeportment;
using Xunit;

namespace Llyn.Tests;

public sealed class TIndexItem
{
    [Fact]
    public void IndexItemBuild_ChosenRow_CarriesMark()
    {
        IReadOnlyList<QIndexItem> items = TInterfaceDeportment.TIndexItemBuild(
            [TIndexRowCreate(1, "Latin", false), TIndexRowCreate(2, "Latin", true)]);

        Assert.Equal([1L, 2L], items.Select(item => item.QIndexItemId));
        Assert.False(items[0].QIndexItemChosen);
        Assert.True(items[1].QIndexItemChosen);
        Assert.Null(items[1].QIndexItemFlag);
    }

    [Fact]
    public void IndexItemMatch_DifferentMark_Matches()
    {
        QIndexItem held = TIndexItemCreate(TIndexRowCreate(1, "Latin", false));
        QIndexItem fresh = TIndexItemCreate(TIndexRowCreate(1, "Latin", true));

        Assert.True(TInterfaceDeportment.TIndexItemMatch(held, fresh));
    }

    [Fact]
    public void IndexItemMatch_DifferentLanguage_Refuses()
    {
        QIndexItem held = TIndexItemCreate(TIndexRowCreate(1, "Latin", false));
        QIndexItem fresh = TIndexItemCreate(TIndexRowCreate(1, "Greek", false));

        Assert.False(TInterfaceDeportment.TIndexItemMatch(held, fresh));
    }

    [Fact]
    public void IndexItemSync_MovedMark_RaisesChange()
    {
        QIndexItem held = TIndexItemCreate(TIndexRowCreate(1, "Latin", false));
        QIndexItem fresh = TIndexItemCreate(TIndexRowCreate(1, "Latin", true));
        List<string?> raised = [];
        held.PropertyChanged += (_, change) => raised.Add(change.PropertyName);

        TInterfaceDeportment.TIndexItemSync(held, fresh);

        Assert.True(held.QIndexItemChosen);
        Assert.Equal([nameof(QIndexItem.QIndexItemChosen)], raised);
    }

    [Fact]
    public void IndexItemSync_SameMark_RaisesNothing()
    {
        QIndexItem held = TIndexItemCreate(TIndexRowCreate(1, "Latin", true));
        QIndexItem fresh = TIndexItemCreate(TIndexRowCreate(1, "Latin", true));
        List<PropertyChangedEventArgs> raised = [];
        held.PropertyChanged += (_, change) => raised.Add(change);

        TInterfaceDeportment.TIndexItemSync(held, fresh);

        Assert.True(held.QIndexItemChosen);
        Assert.Empty(raised);
    }

    [Fact]
    public void IndexNeighbourFind_NoChosenDown_PicksFirst()
    {
        IReadOnlyList<QIndexItem> items = TIndexListCreate(-1);

        Assert.Equal(1L, TInterfaceDeportment.TIndexNeighbourFind(items, true));
    }

    [Fact]
    public void IndexNeighbourFind_NoChosenUp_PicksFirst()
    {
        IReadOnlyList<QIndexItem> items = TIndexListCreate(-1);

        Assert.Equal(1L, TInterfaceDeportment.TIndexNeighbourFind(items, false));
    }

    [Fact]
    public void IndexNeighbourFind_MiddleChosen_MovesOneRow()
    {
        IReadOnlyList<QIndexItem> items = TIndexListCreate(1);

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

    private static IReadOnlyList<QIndexItem> TIndexListCreate(int chosen) =>
        TInterfaceDeportment.TIndexItemBuild(
            [TIndexRowCreate(1, "Latin", chosen == 0), TIndexRowCreate(2, "Latin", chosen == 1),
                TIndexRowCreate(3, "Latin", chosen == 2)]);

    private static CVistaRow TIndexRowCreate(long id, string language, bool chosen) =>
        new(id, "aqua", language, string.Empty, "aqua", chosen);

    private static QIndexItem TIndexItemCreate(CVistaRow row) => TInterfaceDeportment.TIndexItemBuild([row])[0];
}
