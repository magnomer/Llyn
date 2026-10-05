using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;
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
            Assert.Equal(medium.ToString()[1..], TInterfaceConductPortrait.TPortraitMediumRead(medium).ToString()[1..]);
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
                LPressTicket ticket = TInterfaceConductPortrait.TPortraitTicketRead(
                    new CPressTicket("Office", 8.27, 11.69, true, 2, false, side, ink));

                Assert.Equal(side.ToString()[1..], ticket.LPressTicketSide.ToString()[1..]);
                Assert.Equal(ink.ToString()[1..], ticket.LPressTicketInk.ToString()[1..]);
            }
        }
    }

    [Fact]
    public void PortraitMediumRead_UnknownFormat_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            static () => TInterfaceConductPortrait.TPortraitMediumRead((CPortraitMedium)99));
    }

    [Fact]
    public void PortraitTicketRead_UnknownSideOrInk_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(static () =>
            TInterfaceConductPortrait.TPortraitTicketRead(new CPressTicket(
                "Office", null, null, false, 1, true, (CPressSide)99, CPressInk.CPressInkDefault)));
        Assert.Throws<ArgumentOutOfRangeException>(static () =>
            TInterfaceConductPortrait.TPortraitTicketRead(new CPressTicket(
                "Office", null, null, false, 1, true, CPressSide.CPressSideDefault, (CPressInk)99)));
    }

    [Fact]
    public void PortraitTicketRead_NamedSheet_CarriesTheDialogAnswer()
    {
        LPressTicket ticket = TInterfaceConductPortrait.TPortraitTicketRead(new CPressTicket(
            "Office", 816.0, 1056.0, true, 2, false, CPressSide.CPressSideLong, CPressInk.CPressInkGray));

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
        LPressTicket ticket = TInterfaceConductPortrait.TPortraitTicketRead(new CPressTicket(
            "Office", null, 11.0, false, 1, true, CPressSide.CPressSideDefault, CPressInk.CPressInkDefault));

        Assert.Equal(LPressPaper.LPressPaperLocal, ticket.LPressTicketPaper);
    }

    [Fact]
    public void PortraitTicketRead_EmptySide_TakesTheLocalSheet()
    {
        LPressTicket ticket = TInterfaceConductPortrait.TPortraitTicketRead(new CPressTicket(
            "Office", 0.0, 1056.0, false, 1, true, CPressSide.CPressSideDefault, CPressInk.CPressInkDefault));

        Assert.Equal(LPressPaper.LPressPaperLocal, ticket.LPressTicketPaper);
    }

    [Fact]
    public void PortraitLabelRead_EngineWording_WordsEveryHeadingByItsKey()
    {
        LPortraitLabel label = TInterfaceConductPortrait.TPortraitLabelRead(TPortraitSettingsCreate());

        Assert.Equal("text:Display.Unknown", label.LPortraitLabelUnknown);
        Assert.Equal("text:Display.MeaningPlural", label.LPortraitLabelMeanings);
        Assert.Equal("text:Frequency.Title", label.LPortraitLabelFrequency);
        Assert.Equal("text:Reference.Title", label.LPortraitLabelSource);
        Assert.Equal("text:Portrait.Tag", label.LPortraitLabelTag);
        Assert.All(
            typeof(LPortraitLabel).GetProperties().Select(property => (string)property.GetValue(label)!),
            static word => Assert.StartsWith("text:", word, StringComparison.Ordinal));
    }

    [Theory]
    [InlineData("Example", "text:Example.Unwritten")]
    [InlineData("Source", "text:Source.Untitled")]
    [InlineData("Situation", "text:Situation.Untitled")]
    public void PortraitLegendRead_Realm_WordsTheFallbackTallyAndEveryKind(string realm, string untitled)
    {
        LPortraitLegend legend = TInterfaceConductPortrait.TPortraitLegendRead(TPortraitSettingsCreate(), realm);

        Assert.Equal(untitled, legend.LPortraitLegendUntitled);
        Assert.Equal("text:Example.Unwritten", legend.LPortraitLegendUnwritten);
        Assert.Equal($"text:{realm}.UsageNone", legend.LPortraitLegendUnused);
        Assert.Equal($"text:{realm}.UsageOne", legend.LPortraitLegendOnce);
        Assert.Equal($"text:{realm}.UsageMany", legend.LPortraitLegendUses);
        Assert.Equal("text:Situation.Description", legend.LPortraitLegendDescription);
        Assert.Equal(Enum.GetValues<LReferenceKind>().Length, legend.LPortraitLegendKind.Count);
        Assert.All(
            legend.LPortraitLegendKind.Values,
            static named => Assert.StartsWith("text:", named, StringComparison.Ordinal));
    }

    [Fact]
    public void PortraitChoiceRead_EngineFormats_OffersEveryFormatWithHtmlFirstChosen()
    {
        IReadOnlyList<CPortraitChoice> choices = TInterfaceConductPortrait.TPortraitChoiceRead();

        Assert.Equal(
            Enum.GetValues<CPortraitMedium>().Order(),
            choices.Select(static choice => choice.CPortraitChoiceMedium).Order());
        Assert.All(
            choices,
            static choice => Assert.StartsWith("Export.", choice.CPortraitChoiceKey, StringComparison.Ordinal));
        Assert.All(
            choices,
            static choice => Assert.StartsWith(".", choice.CPortraitChoiceSuffix, StringComparison.Ordinal));
        CPortraitChoice chosen = Assert.Single(choices, static choice => choice.CPortraitChoiceChosen);
        Assert.Equal(CPortraitMedium.CPortraitMediumHtml, chosen.CPortraitChoiceMedium);
        Assert.Equal(".html", chosen.CPortraitChoiceSuffix);
        Assert.Equal("Export.Html", chosen.CPortraitChoiceKey);
    }

    [Fact]
    public void PortraitChoiceRead_NoRows_OffersMarkupChosen()
    {
        CPortraitChoice choice = Assert.Single(TInterfaceConductPortrait.TPortraitChoiceRead([]));

        Assert.Equal(
            new CPortraitChoice("Export.Markup", string.Empty, true, CPortraitMedium.CPortraitMediumMarkup),
            choice);
    }

    [Fact]
    public void PortraitChoiceRead_PipeInSuffix_StripsEveryPipe()
    {
        IReadOnlyList<CPortraitChoice> choices = TInterfaceConductPortrait.TPortraitChoiceRead(
        [
            (LPortraitMedium.LPortraitMediumHtml, "|.h|tml|", true),
            (LPortraitMedium.LPortraitMediumPdf, "||", false),
        ]);

        Assert.Equal([".html", string.Empty], choices.Select(static choice => choice.CPortraitChoiceSuffix));
        Assert.Single(choices, static choice => choice.CPortraitChoiceChosen);
    }

    [Fact]
    public void PortraitChoiceRead_NoChosenRow_ChoosesTheFirst()
    {
        IReadOnlyList<CPortraitChoice> choices = TInterfaceConductPortrait.TPortraitChoiceRead(
        [
            (LPortraitMedium.LPortraitMediumDocx, ".docx", false),
            (LPortraitMedium.LPortraitMediumPdf, ".pdf", false),
        ]);

        Assert.Equal([true, false], choices.Select(static choice => choice.CPortraitChoiceChosen));
        Assert.Equal(CPortraitMedium.CPortraitMediumDocx, choices[0].CPortraitChoiceMedium);
    }

    [Fact]
    public void PortraitChoiceRead_ManyChosenRows_KeepsTheFirstChosen()
    {
        IReadOnlyList<CPortraitChoice> choices = TInterfaceConductPortrait.TPortraitChoiceRead(
        [
            (LPortraitMedium.LPortraitMediumMarkdown, ".md", false),
            (LPortraitMedium.LPortraitMediumHtml, ".html", true),
            (LPortraitMedium.LPortraitMediumHtml, ".htm", true),
            (LPortraitMedium.LPortraitMediumPdf, ".pdf", true),
        ]);

        Assert.Equal(4, choices.Count);
        Assert.Equal([false, true, false, false], choices.Select(static choice => choice.CPortraitChoiceChosen));
        Assert.Equal(".html", choices[1].CPortraitChoiceSuffix);
    }

    [Fact]
    public void PortraitChoiceRead_UnknownMedium_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(static () => TInterfaceConductPortrait.TPortraitChoiceRead(
        [
            ((LPortraitMedium)99, ".odd", true),
            (LPortraitMedium.LPortraitMediumPdf, ".pdf", false),
        ]));
    }

    [Fact]
    public async Task PortraitFileExport_DeclinedOrFailed_AsksWithTheNameThenExportsNothingOrShowsTheFailure()
    {
        List<string> asked = [];
        List<(string, LPortraitMedium)> exported = [];

        await TInterfaceConductPortrait.TPortraitFileExport(
            TEnvoyFake.TEnvoyFileCreate(null, CPortraitMedium.CPortraitMediumPdf, asked),
            "water",
            (path, medium) =>
            {
                exported.Add((path, medium));
                return Task.CompletedTask;
            });
        await TInterfaceConductPortrait.TPortraitFileExport(
            TEnvoyFake.TEnvoyFileCreate("water.pdf", CPortraitMedium.CPortraitMediumPdf, asked),
            "water",
            (path, medium) =>
            {
                exported.Add((path, medium));
                return Task.CompletedTask;
            });
        await TInterfaceConductPortrait.TPortraitFileExport(
            TEnvoyFake.TEnvoyFileCreate("water.md", CPortraitMedium.CPortraitMediumMarkdown, asked),
            "water",
            static (_, _) => throw new InvalidOperationException("The disk is full."));

        Assert.Equal([("water.pdf", LPortraitMedium.LPortraitMediumPdf)], exported);
        Assert.Equal(["File:water", "File:water", "File:water", "Export.Failed"], asked);
    }

    [Fact]
    public async Task PortraitFileExport_NameReadFails_ShowsTheNameFailureAndExportsNothing()
    {
        List<string> asked = [];
        List<string> exported = [];

        await TInterfaceConductPortrait.TPortraitFileExport(
            TEnvoyFake.TEnvoyFileCreate("water.pdf", CPortraitMedium.CPortraitMediumPdf, asked),
            TInterfaceConduct.TSettingsCreate(),
            static () => throw new InvalidOperationException("The name is unreadable."),
            (path, _) =>
            {
                exported.Add(path);
                return Task.CompletedTask;
            });

        Assert.Equal(["Export.NameFailed"], asked);
        Assert.Empty(exported);
    }

    [Fact]
    public async Task PortraitFileExport_NoSettings_Throws()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(() => TInterfaceConductPortrait.TPortraitFileExport(
            TEngineFake.TEngineStubCreate<CEnvoy>(),
            null!,
            static () => "water",
            static (_, _) => Task.CompletedTask));
    }

    [Fact]
    public async Task PortraitTicketPrint_DeclinedOrFailed_AsksThenPrintsNothingOrShowsTheFailure()
    {
        List<string> asked = [];
        List<string> printed = [];
        CPressTicket ticket = new(
            "Office", null, null, false, 1, true, CPressSide.CPressSideDefault, CPressInk.CPressInkDefault);

        await TInterfaceConductPortrait.TPortraitTicketPrint(
            TEnvoyFake.TEnvoyTicketCreate(() => null, asked), chosen =>
        {
            printed.Add(chosen.LPressTicketPrinter);
            return Task.CompletedTask;
        });
        await TInterfaceConductPortrait.TPortraitTicketPrint(
            TEnvoyFake.TEnvoyTicketCreate(() => ticket, asked), chosen =>
        {
            printed.Add(chosen.LPressTicketPrinter);
            return Task.CompletedTask;
        });
        await TInterfaceConductPortrait.TPortraitTicketPrint(
            TEnvoyFake.TEnvoyTicketCreate(() => ticket, asked),
            static _ => throw new InvalidOperationException("The printer is gone."));
        await TInterfaceConductPortrait.TPortraitTicketPrint(
            TEnvoyFake.TEnvoyTicketCreate(
                static () => throw new InvalidOperationException("The spooler is down."), asked),
            chosen =>
            {
                printed.Add(chosen.LPressTicketPrinter);
                return Task.CompletedTask;
            });

        Assert.Equal(["Office"], printed);
        Assert.Equal(["Ticket", "Ticket", "Ticket", "Print.Failed", "Ticket", "Print.Failed"], asked);
    }

    private static LSettingsPort TPortraitSettingsCreate()
    {
        return TEngineFake.TEngineCreate<LSettingsPort>(new Dictionary<string, Func<object?[]?, object?>>
        {
            ["LEngineTextRead"] = static args => "text:" + (string)args![0]!,
        });
    }
}
