using System;
using System.Linq;
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

    [Fact]
    public void PanelMediumRead_EveryMirrorFormat_ReturnsTheSameNamedEngineFormat()
    {
        CPortraitMedium[] media = Enum.GetValues<CPortraitMedium>();

        Assert.Equal(media.Length, Enum.GetValues<LPortraitMedium>().Length);
        foreach (CPortraitMedium medium in media)
        {
            Assert.Equal(medium.ToString()[1..], TInterfaceDeportment.TPanelMediumRead(medium).ToString()[1..]);
        }
    }

    [Fact]
    public void PanelTicketRead_EverySideAndInk_CastsToTheSameNamedEngineMember()
    {
        Assert.Equal(Enum.GetValues<CPressSide>().Length, Enum.GetValues<LPressSide>().Length);
        Assert.Equal(Enum.GetValues<CPressInk>().Length, Enum.GetValues<LPressInk>().Length);
        foreach (CPressSide side in Enum.GetValues<CPressSide>())
        {
            foreach (CPressInk ink in Enum.GetValues<CPressInk>())
            {
                LPressTicket ticket = TInterfaceDeportment.TPanelTicketRead(
                    new CPressTicket("Office", 8.27, 11.69, true, 2, false, side, ink));

                Assert.Equal(side.ToString()[1..], ticket.LPressTicketSide.ToString()[1..]);
                Assert.Equal(ink.ToString()[1..], ticket.LPressTicketInk.ToString()[1..]);
            }
        }
    }

    [Fact]
    public void PanelTicketRead_NamedSheet_CarriesTheDialogAnswer()
    {
        LPressTicket ticket = TInterfaceDeportment.TPanelTicketRead(new CPressTicket(
            "Office", 8.5, 11.0, true, 2, false, CPressSide.CPressSideLong, CPressInk.CPressInkGray));

        Assert.Equal("Office", ticket.LPressTicketPrinter);
        Assert.Equal(8.5, ticket.LPressTicketPaper.LPressPaperWidth);
        Assert.Equal(11.0, ticket.LPressTicketPaper.LPressPaperHeight);
        Assert.True(ticket.LPressTicketLandscape);
        Assert.Equal(2, ticket.LPressTicketCopies);
        Assert.False(ticket.LPressTicketCollated);
    }

    [Fact]
    public void PanelTicketRead_NoSheetNamed_TakesTheLocalSheet()
    {
        LPressTicket ticket = TInterfaceDeportment.TPanelTicketRead(new CPressTicket(
            "Office", null, 11.0, false, 1, true, CPressSide.CPressSideDefault, CPressInk.CPressInkDefault));

        Assert.Equal(LPressPaper.LPressPaperLocal, ticket.LPressTicketPaper);
    }

    [Fact]
    public void PanelLabelRead_EveryWord_CopiesItToTheSameNamedEngineWord()
    {
        string[] words = [.. Enumerable.Range(0, 22).Select(static index => $"word{index}")];
        CPortraitLabel label = (CPortraitLabel)Activator.CreateInstance(typeof(CPortraitLabel), words)!;

        LPortraitLabel engine = TInterfaceDeportment.TPanelLabelRead(label);

        foreach (System.Reflection.PropertyInfo mirror in typeof(CPortraitLabel).GetProperties())
        {
            Assert.Equal(
                mirror.GetValue(label),
                typeof(LPortraitLabel).GetProperty("L" + mirror.Name[1..])!.GetValue(engine));
        }
    }
}
