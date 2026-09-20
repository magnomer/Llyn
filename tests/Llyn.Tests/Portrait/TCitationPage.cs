using Llyn.Core;
using Xunit;

namespace Llyn.Tests;

public sealed class TCitationPage
{
    [Fact]
    public void ExamplePageRead_GlossAndSource_CarriesWhatTheExcerptShows()
    {
        LReference source = TInterface.TReferenceCreate(
            7, "Field Notes", null, LReferenceKind.LReferenceKindUnspecified, null, null,
            LStateMark.LStateMarkUnspecified);
        LExample example = TInterface.TExampleCreate(
            3, "English", "The fire kindled.", "불이 붙었다.", TInterface.TStateAnchorRead(source.LReferenceId));

        LPortraitPage page = TInterface.TExamplePageRead(example, source, 2, TInterface.TPortraitLegendRead());

        Assert.Equal("The fire kindled.", page.LPortraitPageTitle);
        Assert.Equal("English", page.LPortraitPageLanguage);
        Assert.Equal(["2 places use it"], page.LPortraitPageChip);
        Assert.Equal(["Translation", "Source"], page.LPortraitPageSection.Select(row => row.LPortraitSectionHeading));
        Assert.Equal("불이 붙었다.", page.LPortraitPageSection[0].LPortraitSectionLine[0].LPortraitLineText);
        Assert.Equal("Field Notes", page.LPortraitPageSection[1].LPortraitSectionLine[0].LPortraitLineText);
    }

    [Fact]
    public void ExamplePageRead_UnwrittenAndUncited_FallsBackToTheUnwrittenWord()
    {
        LExample example = TInterface.TExampleCreate(3, "English", null, null, TInterface.TStateAnchorRead(null));

        LPortraitPage page = TInterface.TExamplePageRead(example, null, 0, TInterface.TPortraitLegendRead());

        Assert.Equal("Unwritten", page.LPortraitPageTitle);
        Assert.Equal(["Not used yet"], page.LPortraitPageChip);
        Assert.Empty(page.LPortraitPageSection);
    }

    [Fact]
    public void ReferencePageRead_TwoCreditsAndAYear_CarriesWhatTheColophonShows()
    {
        LReference source = TInterface.TReferenceCreate(
            7, "Field Notes", "1999", LReferenceKind.LReferenceKindBook, null, "https://notes.example",
            LStateMark.LStateMarkSpecified);
        IReadOnlyList<LAuthor> credits = [TInterface.TAuthorCreate(1, "Ada"), TInterface.TAuthorCreate(2, "Grace")];

        LPortraitPage page = TInterface.TReferencePageRead(source, credits, 1, TInterface.TPortraitLegendRead());

        Assert.Equal("Field Notes", page.LPortraitPageTitle);
        Assert.Equal(["book", "Used in one place"], page.LPortraitPageChip);
        Assert.Equal(["Authors", "Year", "URL"], page.LPortraitPageSection.Select(row => row.LPortraitSectionHeading));
        Assert.Equal("Ada, Grace", page.LPortraitPageSection[0].LPortraitSectionLine[0].LPortraitLineText);
        Assert.Equal("1999", page.LPortraitPageSection[1].LPortraitSectionLine[0].LPortraitLineText);
    }

    [Fact]
    public void ReferencePageRead_AuthorsUnknown_ShowsTheMarkAlone()
    {
        LReference source = TInterface.TReferenceCreate(
            7, "Field Notes", null, LReferenceKind.LReferenceKindUnspecified, null, null,
            LStateMark.LStateMarkUnknown);

        LPortraitPage page = TInterface.TReferencePageRead(source, [], 0, TInterface.TPortraitLegendRead());

        Assert.Equal(["Not used yet"], page.LPortraitPageChip);
        LPortraitSection authors = Assert.Single(page.LPortraitPageSection);
        Assert.Equal("Authors", authors.LPortraitSectionHeading);
        Assert.Equal("Unknown", authors.LPortraitSectionLine[0].LPortraitLineText);
    }

    [Fact]
    public void SituationPageRead_KindAndDescription_CarriesTheDescriptionAsANote()
    {
        LSituation situation = TInterface.TSituationCreate(
            5, "at the hearth", "Sitting **close** to the fire.", "domestic");

        LPortraitPage page = TInterface.TSituationPageRead(situation, 0, TInterface.TPortraitLegendRead());

        Assert.Equal("at the hearth", page.LPortraitPageTitle);
        Assert.Equal(["domestic", "Not used yet"], page.LPortraitPageChip);
        LPortraitSection description = Assert.Single(page.LPortraitPageSection);
        Assert.Equal("Description", description.LPortraitSectionHeading);
        Assert.Equal("Sitting **close** to the fire.", description.LPortraitSectionNote);
    }
}
