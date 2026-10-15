using System.Collections.ObjectModel;
using System.Linq;
using Llyn.Conduct;
using Llyn.UIDeportment;
using Xunit;

namespace Llyn.Tests;

public sealed class TEtymologyItem
{
    [Fact]
    public void EtymologyItemShow_CoSources_NestsUnderTheirChild()
    {
        ObservableCollection<QEtymologyItem> items = [];

        TInterfaceDeportment.TEtymologyItemShow(
            items, [TAncestorCreate(1, "1", 0, "a"), TAncestorCreate(2, "1/2", 1, "b"),
                TAncestorCreate(3, "1/3", 1, "c"), TAncestorCreate(4, "4", 0, "d")]);

        Assert.Equal([1L, 4L], items.Select(static item => item.QEtymologyItemAncestor.CAncestorId));
        Assert.Equal(
            [2L, 3L], items[0].QEtymologyItemSources.Select(static item => item.QEtymologyItemAncestor.CAncestorId));
        Assert.Empty(items[1].QEtymologyItemSources);
    }

    [Fact]
    public void EtymologyItemShow_SecondRead_KeepsPairedItems()
    {
        ObservableCollection<QEtymologyItem> items = [];
        TInterfaceDeportment.TEtymologyItemShow(
            items, [TAncestorCreate(1, "1", 0, "a"), TAncestorCreate(2, "1/2", 1, "b")]);
        QEtymologyItem source = items[0].QEtymologyItemSources[0];

        TInterfaceDeportment.TEtymologyItemShow(
            items, [TAncestorCreate(1, "1", 0, "a"), TAncestorCreate(2, "1/2", 1, "bc")]);

        Assert.Same(source, items[0].QEtymologyItemSources[0]);
        Assert.Equal("bc", source.QEtymologyItemAncestor.CAncestorText);
    }

    [Fact]
    public void EtymologyItemShow_RemovedBox_DropsItsItem()
    {
        ObservableCollection<QEtymologyItem> items = [];
        TInterfaceDeportment.TEtymologyItemShow(
            items, [TAncestorCreate(1, "1", 0, "a"), TAncestorCreate(2, "1/2", 1, "b")]);

        TInterfaceDeportment.TEtymologyItemShow(items, [TAncestorCreate(1, "1", 0, "a")]);

        Assert.Single(items);
        Assert.Empty(items[0].QEtymologyItemSources);
    }

    [Fact]
    public void EtymologyItemShow_SameIdUnderNewKey_PairsByKey()
    {
        ObservableCollection<QEtymologyItem> items = [];
        TInterfaceDeportment.TEtymologyItemShow(items, [TAncestorCreate(1, "1", 0, "a")]);
        QEtymologyItem first = items[0];

        TInterfaceDeportment.TEtymologyItemShow(items, [TAncestorCreate(1, "2/1", 0, "a")]);

        Assert.NotSame(first, Assert.Single(items));
        Assert.Equal("2/1", items[0].QEtymologyItemAncestor.CAncestorKey);
    }

    private static CAncestor TAncestorCreate(long id, string key, int depth, string text) =>
        new(id, key, depth, text, string.Empty, null, false, string.Empty, null, false);
}
