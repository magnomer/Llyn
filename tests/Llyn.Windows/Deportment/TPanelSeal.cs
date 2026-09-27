using System;
using Llyn.Conduct;
using Llyn.Core;
using Xunit;

namespace Llyn.Tests;

public sealed class TPanelSeal
{
    [Fact]
    public void PanelOrderRead_EveryEngineOrder_ReturnsTheSameNamedMirror()
    {
        LCatalogOrder[] orders = Enum.GetValues<LCatalogOrder>();

        Assert.Equal(orders.Length, Enum.GetValues<CCatalogOrder>().Length);
        foreach (LCatalogOrder order in orders)
        {
            CCatalogOrder mirror = TInterfaceDeportment.TPanelOrderRead(order);

            Assert.Equal(order.ToString()[1..], mirror.ToString()[1..]);
            Assert.Equal(order, TInterfaceDeportment.TPanelOrderRead(mirror));
        }
    }

    [Fact]
    public void PanelSubjectRead_EverySubject_ReturnsTheSameNamedEngineSubject()
    {
        CSubject[] subjects = Enum.GetValues<CSubject>();

        Assert.Equal(subjects.Length, Enum.GetValues<LSubject>().Length);
        foreach (CSubject subject in subjects)
        {
            Assert.Equal(subject.ToString()[1..], TInterfaceDeportment.TPanelSubjectRead(subject).ToString()[1..]);
        }
    }

    [Fact]
    public void PanelOrderRead_NoOrder_ReturnsNone()
    {
        Assert.Null(TInterfaceDeportment.TPanelOrderRead(null));
    }

    [Fact]
    public void PanelFilterRead_NothingHidden_ReturnsEmptyEngineFilter()
    {
        Assert.Same(LCatalogFilter.LCatalogFilterEmpty, TInterfaceDeportment.TPanelFilterRead(new CCatalogFilter([])));
    }

    [Fact]
    public void PanelFilterRead_HiddenLanguages_CarriesThemBothWays()
    {
        LCatalogFilter hidden = TInterfaceDeportment.TPanelFilterRead(new CCatalogFilter(["Latin", "Greek"]));

        Assert.Equal(["Latin", "Greek"], hidden.LCatalogFilterHidden);
        Assert.Equal(["Latin", "Greek"], TInterfaceDeportment.TPanelFilterRead(hidden).CCatalogFilterHidden);
    }
}
