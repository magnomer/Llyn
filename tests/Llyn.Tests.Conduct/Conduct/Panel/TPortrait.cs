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
    public void PortraitMediumRead_AllMirrors_ReturnsSameNamedFormat()
    {
        CPortraitMedium[] media = Enum.GetValues<CPortraitMedium>();

        Assert.Equal(media.Length, Enum.GetValues<LPortraitMedium>().Length);
        foreach (CPortraitMedium medium in media)
        {
            Assert.Equal(medium.ToString()[1..], TInterfaceConductPortrait.TPortraitMediumRead(medium).ToString()[1..]);
        }
    }

    [Fact]
    public void PortraitMediumRead_UnknownFormat_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            static () => TInterfaceConductPortrait.TPortraitMediumRead((CPortraitMedium)99));
    }

    [Fact]
    public void PortraitLabelRead_EngineWording_WordsAllHeadingsByKey()
    {
        LPortraitLabel label = TInterfaceConductPortrait.TPortraitLabelRead(TPortraitSettingsCreate());

        Assert.Equal("text:Display.Unknown", label.LPortraitLabelUnknown);
        Assert.Equal("text:Display.MeaningPlural", label.LPortraitLabelMeanings);
        Assert.Equal("text:Frequency.Title", label.LPortraitLabelFrequency);
        Assert.Equal("text:Reference.Title", label.LPortraitLabelSource);
        Assert.Equal("text:Portrait.Tag", label.LPortraitLabelTag);
        Assert.Equal("text:Unit.Morpheme", label.LPortraitLabelUnit[LUnit.LUnitMorpheme]);
        Assert.Equal("text:Freedom.Standalone", label.LPortraitLabelFreedom[LFreedom.LFreedomStandalone]);
        Assert.All(
            typeof(LPortraitLabel).GetProperties()
                .Where(static property => property.PropertyType == typeof(string))
                .Select(property => (string)property.GetValue(label)!)
                .Concat(label.LPortraitLabelUnit.Values)
                .Concat(label.LPortraitLabelFreedom.Values),
            static word => Assert.StartsWith("text:", word, StringComparison.Ordinal));
    }

    [Theory]
    [InlineData("Example", "text:Example.Unwritten")]
    [InlineData("Source", "text:Source.Untitled")]
    [InlineData("Situation", "text:Situation.Untitled")]
    public void PortraitLegendRead_Realm_WordsFallbackTallyAndKinds(string realm, string untitled)
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
    public void PortraitChoiceRead_EngineFormats_OffersAllChoosesHtml()
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
    public void PortraitChoiceRead_NoChosenRow_ChoosesFirst()
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
    public void PortraitChoiceRead_ManyChosenRows_KeepsFirstChosen()
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
    public async Task PortraitFileExport_DeclinedOrFailed_ExportsOnlyAccepted()
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
    public async Task PortraitFileExport_NameReadFails_ShowsNameFailure()
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
    public async Task PortraitPrint_SucceededOrFailed_PrintsOrShowsFailure()
    {
        List<string> asked = [];
        int printed = 0;

        await TInterfaceConductPortrait.TPortraitPrint(TEnvoyFake.TEnvoyCreate(null, asked), () =>
        {
            printed++;
            return Task.FromResult<LGalleyRelay?>(null);
        });
        await TInterfaceConductPortrait.TPortraitPrint(
            TEnvoyFake.TEnvoyCreate(null, asked),
            static () => throw new InvalidOperationException("The printer is gone."));

        Assert.Equal(1, printed);
        Assert.Equal(["Print.Failed"], asked);
    }

    [Fact]
    public async Task PortraitPrint_ClosedWindow_DisposesBoard()
    {
        TPressBoard board = new();
        List<string> asked = [];
        int windows = 0;
        CEnvoy envoy = TEnvoyFake.TEnvoyPressCreate(
            _ =>
            {
                windows++;
                Assert.False(board.TPressBoardDisposed);
                return true;
            },
            asked);

        await TInterfaceConductPortrait.TPortraitPrint(envoy, board);

        Assert.Equal(1, windows);
        Assert.True(board.TPressBoardDisposed);
        Assert.Empty(asked);
    }

    [Fact]
    public async Task PortraitPrint_WindowThrows_StillDisposesBoard()
    {
        TPressBoard board = new();
        CEnvoy envoy = TEnvoyFake.TEnvoyPressCreate(
            static _ => throw new InvalidOperationException("The window broke."), []);

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => TInterfaceConductPortrait.TPortraitPrint(envoy, board));

        Assert.True(board.TPressBoardDisposed);
    }

    [Fact]
    public async Task PortraitPrint_NullGalley_OpensNoWindow()
    {
        List<string> asked = [];
        int windows = 0;
        CEnvoy envoy = TEnvoyFake.TEnvoyPressCreate(
            _ =>
            {
                windows++;
                return true;
            },
            asked);

        await TInterfaceConductPortrait.TPortraitPrint(envoy, static () => Task.FromResult<LGalleyRelay?>(null));

        Assert.Equal(0, windows);
        Assert.Empty(asked);
    }

    private static LSettingsPort TPortraitSettingsCreate()
    {
        return TEngineFake.TEngineCreate<LSettingsPort>(new Dictionary<string, Func<object?[]?, object?>>
        {
            ["LEngineTextRead"] = static args => "text:" + (string)args![0]!,
        });
    }
}
