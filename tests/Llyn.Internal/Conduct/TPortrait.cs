using System;
using System.Collections.Generic;
using System.Linq;
using Llyn.Conduct;
using Llyn.Core;
using Xunit;

namespace Llyn.Tests;

public sealed class TPortrait
{
    [Fact]
    public void PortraitMediumRead_EveryMirrorFormat_ReturnsTheSameNamedEngineFormat()
    {
        CPortraitMedium[] media = Enum.GetValues<CPortraitMedium>();

        Assert.Equal(media.Length, Enum.GetValues<LPortraitMedium>().Length);
        foreach (CPortraitMedium medium in media)
        {
            Assert.Equal(medium.ToString()[1..], CPortrait.CPortraitMediumRead(medium).ToString()[1..]);
        }
    }

    [Fact]
    public void PortraitTicketRead_EverySideAndInk_MapsToTheSameNamedEngineMember()
    {
        Assert.Equal(Enum.GetValues<CPressSide>().Length, Enum.GetValues<LPressSide>().Length);
        Assert.Equal(Enum.GetValues<CPressInk>().Length, Enum.GetValues<LPressInk>().Length);
        foreach (CPressSide side in Enum.GetValues<CPressSide>())
        {
            foreach (CPressInk ink in Enum.GetValues<CPressInk>())
            {
                LPressTicket ticket = CPortrait.CPortraitTicketRead(
                    new CPressTicket("Office", 8.27, 11.69, true, 2, false, side, ink));

                Assert.Equal(side.ToString()[1..], ticket.LPressTicketSide.ToString()[1..]);
                Assert.Equal(ink.ToString()[1..], ticket.LPressTicketInk.ToString()[1..]);
            }
        }
    }

    [Fact]
    public void PortraitMediumRead_UnknownFormat_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(static () => CPortrait.CPortraitMediumRead((CPortraitMedium)99));
    }

    [Fact]
    public void PortraitTicketRead_UnknownSideOrInk_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(static () => CPortrait.CPortraitTicketRead(new CPressTicket(
            "Office", null, null, false, 1, true, (CPressSide)99, CPressInk.CPressInkDefault)));
        Assert.Throws<ArgumentOutOfRangeException>(static () => CPortrait.CPortraitTicketRead(new CPressTicket(
            "Office", null, null, false, 1, true, CPressSide.CPressSideDefault, (CPressInk)99)));
    }

    [Fact]
    public void PortraitTicketRead_NamedSheet_CarriesTheDialogAnswer()
    {
        LPressTicket ticket = CPortrait.CPortraitTicketRead(new CPressTicket(
            "Office", 8.5, 11.0, true, 2, false, CPressSide.CPressSideLong, CPressInk.CPressInkGray));

        Assert.Equal("Office", ticket.LPressTicketPrinter);
        Assert.Equal(8.5, ticket.LPressTicketPaper.LPressPaperWidth);
        Assert.Equal(11.0, ticket.LPressTicketPaper.LPressPaperHeight);
        Assert.True(ticket.LPressTicketLandscape);
        Assert.Equal(2, ticket.LPressTicketCopies);
        Assert.False(ticket.LPressTicketCollated);
    }

    [Fact]
    public void PortraitTicketRead_NoSheetNamed_TakesTheLocalSheet()
    {
        LPressTicket ticket = CPortrait.CPortraitTicketRead(new CPressTicket(
            "Office", null, 11.0, false, 1, true, CPressSide.CPressSideDefault, CPressInk.CPressInkDefault));

        Assert.Equal(LPressPaper.LPressPaperLocal, ticket.LPressTicketPaper);
    }

    [Fact]
    public void PortraitLabelRead_EveryWord_CopiesItToTheSameNamedEngineWord()
    {
        string[] words = [.. Enumerable.Range(0, 22).Select(static index => $"word{index}")];
        CPortraitLabel label = (CPortraitLabel)Activator.CreateInstance(typeof(CPortraitLabel), words)!;

        LPortraitLabel engine = CPortrait.CPortraitLabelRead(label);

        foreach (System.Reflection.PropertyInfo mirror in typeof(CPortraitLabel).GetProperties())
        {
            Assert.Equal(
                mirror.GetValue(label),
                typeof(LPortraitLabel).GetProperty("L" + mirror.Name[1..])!.GetValue(engine));
        }
    }

    [Fact]
    public void PortraitLegendRead_EveryWord_CopiesItToTheSameNamedEngineWord()
    {
        string[] words = [.. Enumerable.Range(0, 13).Select(static index => $"word{index}")];
        CPortraitLegend legend = (CPortraitLegend)Activator.CreateInstance(
            typeof(CPortraitLegend), [.. words, new Dictionary<string, string>()])!;

        LPortraitLegend engine = CPortrait.CPortraitLegendRead(legend);

        foreach (System.Reflection.PropertyInfo mirror in typeof(CPortraitLegend).GetProperties()
                     .Where(static property => property.PropertyType == typeof(string)))
        {
            Assert.Equal(
                mirror.GetValue(legend),
                typeof(LPortraitLegend).GetProperty("L" + mirror.Name[1..])!.GetValue(engine));
        }
    }

    [Fact]
    public void PortraitLegendRead_KindWordMissing_KeepsTheKindName()
    {
        CPortraitLegend legend = new(
            "?", "Untitled", "Unwritten", "Unused", "Once", "uses", "Translation", "Source",
            "Author", "Year", "Url", "Note", "Description", new Dictionary<string, string>());

        LPortraitLegend engine = CPortrait.CPortraitLegendRead(legend);

        Assert.Equal(Enum.GetValues<LReferenceKind>().Length, engine.LPortraitLegendKind.Count);
        Assert.All(engine.LPortraitLegendKind.Values, static named => Assert.NotEqual(string.Empty, named));
    }
}
