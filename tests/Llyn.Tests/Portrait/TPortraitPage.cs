using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TPortraitPage
{
    [Fact]
    public void PortraitRead_ExampleWithGlossAndSource_CarriesWhatTheExcerptShows()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LReference source = TPortraitSourceCreate(engine, "Field Notes");
        LExample example = engine.TEngineExampleCreate(TInterface.TExampleCreate(
            0, "English", "The fire kindled.", "불이 붙었다.", TInterface.TStateAnchorRead(source.LReferenceId)));

        LPortraitPage page = engine.TEnginePortraitRead(
            example.LExampleId, LOwner.LOwnerExample, TInterface.TPortraitLegendRead());

        Assert.Equal("The fire kindled.", page.LPortraitPageTitle);
        Assert.Equal("English", page.LPortraitPageLanguage);
        Assert.Equal(["Not used yet"], page.LPortraitPageChip);
        Assert.Equal(["Translation", "Source"], page.LPortraitPageSection.Select(row => row.LPortraitSectionHeading));
        Assert.Equal("불이 붙었다.", page.LPortraitPageSection[0].LPortraitSectionLine[0].LPortraitLineText);
        Assert.Equal("Field Notes", page.LPortraitPageSection[1].LPortraitSectionLine[0].LPortraitLineText);
    }

    [Fact]
    public void PortraitRead_UnwrittenExample_FallsBackToTheUnwrittenWord()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LExample example = engine.TEngineExampleCreate(TInterface.TExampleCreate(
            0, "English", null, null, TInterface.TStateAnchorRead(null)));

        LPortraitPage page = engine.TEnginePortraitRead(
            example.LExampleId, LOwner.LOwnerExample, TInterface.TPortraitLegendRead());

        Assert.Equal("Unwritten", page.LPortraitPageTitle);
        Assert.Empty(page.LPortraitPageSection);
    }

    [Fact]
    public void PortraitRead_SourceWithCredits_CarriesWhatTheColophonShows()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LReference source = engine.TEngineReferenceCreate(TInterface.TReferenceCreate(
            0, "Field Notes", "1999", LReferenceKind.LReferenceKindBook, null, "https://notes.example",
            LStateMark.LStateMarkUnspecified));
        LAuthor author = engine.TEngineAuthorCreate(TInterface.TAuthorCreate(0, "Ada"));
        engine.TEngineAuthorAttach(source.LReferenceId, author.LAuthorId, 0);

        LPortraitPage page = engine.TEnginePortraitRead(
            source.LReferenceId, LOwner.LOwnerReference, TInterface.TPortraitLegendRead());

        Assert.Equal("Field Notes", page.LPortraitPageTitle);
        Assert.Equal(["book", "Not used yet"], page.LPortraitPageChip);
        Assert.Equal(["Authors", "Year", "URL"], page.LPortraitPageSection.Select(row => row.LPortraitSectionHeading));
        Assert.Equal("Ada", page.LPortraitPageSection[0].LPortraitSectionLine[0].LPortraitLineText);
    }

    [Fact]
    public void PortraitRead_SituationWithDescription_CarriesItAsANote()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LSituation situation = engine.TEngineSituationCreate(
            TInterface.TSituationCreate(0, "at the hearth", "Sitting **close** to the fire.", "domestic"));

        LPortraitPage page = engine.TEnginePortraitRead(
            situation.LSituationId, LOwner.LOwnerSituation, TInterface.TPortraitLegendRead());

        Assert.Equal("at the hearth", page.LPortraitPageTitle);
        Assert.Equal(["domestic", "Not used yet"], page.LPortraitPageChip);
        LPortraitSection section = Assert.Single(page.LPortraitPageSection);
        Assert.Equal("Description", section.LPortraitSectionHeading);
        Assert.Equal("Sitting **close** to the fire.", section.LPortraitSectionNote);
    }

    [Fact]
    public void PortraitRead_MissingRow_Throws()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        Assert.Throws<InvalidOperationException>(() =>
            engine.TEnginePortraitRead(99, LOwner.LOwnerSituation, TInterface.TPortraitLegendRead()));
    }

    [Fact]
    public void SheetPageFormat_FullPage_CarriesTitleChipsAndSections()
    {
        LPortraitPage page = TInterface.TPortraitPageCreate(
            "The fire <kindled>.",
            "English",
            ["Quoted in one place"],
            [
                TInterface.TPortraitSectionCreate(
                    "Translation", [TInterface.TPortraitLineCreate("Korean", "불이 붙었다.")], string.Empty, [], []),
                TInterface.TPortraitSectionCreate(
                    "Description", [], "Sitting **close** to the fire.", [], []),
            ]);

        string sheet = TInterface.TSheetPageFormat(page, TInterface.TThemeLoad());

        Assert.Contains("<h1 class=\"headword\">The fire &lt;kindled&gt;.</h1>", sheet);
        Assert.Contains("<span>English</span><span>Quoted in one place</span>", sheet);
        Assert.Contains("<h2>Translation</h2>", sheet);
        Assert.Contains("<span class=\"tag\">Korean</span><span>불이 붙었다.</span>", sheet);
        Assert.Contains("<p>Sitting <strong>close</strong> to the fire.</p>", sheet);
    }

    [Fact]
    public async Task PortraitPrint_ExampleOnFakePress_HandsThePageAndTheTicketToThePress()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        TPress press = new();
        engine.TEnginePressApply(press);

        LExample example = engine.TEngineExampleCreate(TInterface.TExampleCreate(
            0, "English", "The fire kindled.", null, TInterface.TStateAnchorRead(null)));

        await engine.TEnginePortraitPrint(
            example.LExampleId,
            LOwner.LOwnerExample,
            TInterface.TPortraitLegendRead(),
            TInterface.TPressTicketCreate("Paper Printer", true, 2));

        Assert.Contains("The fire kindled.", press.TPressHtml);
        Assert.Equal("Paper Printer", press.TPressTicket?.LPressTicketPrinter);
        Assert.True(press.TPressTicket?.LPressTicketLandscape);
        Assert.Equal(2, press.TPressTicket?.LPressTicketCopies);
        Assert.Equal(LPressPaper.LPressPaperMetric, press.TPressTicket?.LPressTicketPaper);
        Assert.Equal(LPressSide.LPressSideLong, press.TPressTicket?.LPressTicketSide);
        Assert.Equal(LPressInk.LPressInkGray, press.TPressTicket?.LPressTicketInk);
    }

    [Fact]
    public async Task PortraitPrint_NoPress_Throws()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LSituation situation = engine.TEngineSituationCreate(
            TInterface.TSituationCreate(0, "at the hearth", null, null));

        await Assert.ThrowsAsync<InvalidOperationException>(() => engine.TEnginePortraitPrint(
            situation.LSituationId,
            LOwner.LOwnerSituation,
            TInterface.TPortraitLegendRead(),
            TInterface.TPressTicketCreate("Paper Printer", false, 1)));
    }

    private static LReference TPortraitSourceCreate(LEngine engine, string title)
    {
        return engine.TEngineReferenceCreate(TInterface.TReferenceCreate(
            0, title, null, LReferenceKind.LReferenceKindUnspecified, null, null, LStateMark.LStateMarkUnspecified));
    }
}
