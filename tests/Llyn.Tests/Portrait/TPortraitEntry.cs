using System.Linq;
using Llyn.Application;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TPortraitEntry
{
    [Fact]
    public void PortraitRead_EntryWithCardAndSentence_CarriesTheTreeTheDisplayShows()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LCardDraft meaning = TInterface.TCardDraftCreate(
            TInterface.TStateValueCreate("set alight"),
            LStateValue.LStateValueUnspecified,
            TInterface.TStateValueCreate("to set something burning"),
            [TInterface.TSentenceDraftCreate("she knelt to kindle the damp logs")],
            [TInterface.TSituationDraftCreate("around a hearth")],
            [],
            ["literal"],
            [],
            1);
        LEntry entry = engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "kindle", "English", "ˈkɪnd(ə)l", "Chiefly *literary*.", [meaning], [], speeches: ["verb"]));

        LPortraitPage page = engine.TEnginePortraitRead(entry.LEntryId, TInterface.TPortraitLabelRead());

        Assert.Equal("kindle", page.LPortraitPageTitle);
        Assert.Equal("English", page.LPortraitPageLanguage);
        Assert.False(page.LPortraitPageFavorite);
        Assert.Equal("ˈkɪnd(ə)l", page.LPortraitPageLine[0].LPortraitLineText);
        Assert.Equal("[", page.LPortraitPageLine[0].LPortraitLineOpener);
        Assert.Equal("]", page.LPortraitPageLine[0].LPortraitLineCloser);
        Assert.Equal(["Verb"], page.LPortraitPageChip);
        Assert.Equal(
            ["Meanings", "Note"],
            page.LPortraitPageSection.Select(row => row.LPortraitSectionHeading));

        LPortraitSection card = Assert.Single(page.LPortraitPageSection[0].LPortraitSectionChild);
        Assert.Equal(1, card.LPortraitSectionPosition);
        Assert.Equal("set alight", card.LPortraitSectionHeading);
        Assert.Equal("to set something burning", Assert.Single(card.LPortraitSectionLine).LPortraitLineText);
        Assert.Equal(
            ["Situations", "Example", "Tags"],
            card.LPortraitSectionChild.Select(row => row.LPortraitSectionHeading));
        Assert.Equal(["around a hearth"], card.LPortraitSectionChild[0].LPortraitSectionChip);
        Assert.Equal(["literal"], card.LPortraitSectionChild[2].LPortraitSectionChip);

        LPortraitSection quote = card.LPortraitSectionChild[1];
        Assert.Equal(0, quote.LPortraitSectionPosition);
        Assert.Equal("she knelt to kindle the damp logs", quote.LPortraitSectionLine[0].LPortraitLineText);

        Assert.Equal("Chiefly *literary*.", page.LPortraitPageSection[1].LPortraitSectionNote);
    }

    [Fact]
    public void PortraitRead_RespelledLanguage_PrintsOneReadingPerRow()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LEntryDraft draft = TInterface.TEntryDraftCreate("kindle", "English", string.Empty, string.Empty, [], [])
            with
            {
                LEntryDraftPronunciations =
                    [TInterface.TPronunciationDraftCreate("ˈkɪndəl", respelling: "KIN-dəl")],
            };
        LEntry entry = engine.TEngineEntrySave(draft);

        LPortraitPage plain = engine.TEnginePortraitRead(entry.LEntryId, TInterface.TPortraitLabelRead());
        engine.TEngineRespellingSave(true);
        LPortraitPage respelled = engine.TEnginePortraitRead(entry.LEntryId, TInterface.TPortraitLabelRead());

        Assert.Equal("ˈkɪndəl", Assert.Single(plain.LPortraitPageLine).LPortraitLineText);
        Assert.Equal("KIN-dəl", Assert.Single(respelled.LPortraitPageLine).LPortraitLineText);
    }

    [Fact]
    public void PortraitRead_LinkedEtymology_CarriesOneBandOfSourceLinks()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        long source = TPortraitEntryCreate(engine, "rinnan");
        long entryId = TPortraitEntryCreate(engine, "run");

        LDraft held = engine.TEngineDraftStart("Input", entryId);
        engine.TEngineRequestApply(TInterface.TEtymonAdditionCreate(held.LDraftId, source, 0));
        engine.TEngineDraftCommit(held.LDraftId);

        LPortraitPage page = engine.TEnginePortraitRead(entryId, TInterface.TPortraitLabelRead());

        LPortraitSection band = Assert.Single(
            page.LPortraitPageSection,
            static row => row.LPortraitSectionHeading == "Etymology");
        Assert.Empty(band.LPortraitSectionLine);
        Assert.Equal("rinnan", Assert.Single(band.LPortraitSectionLink).LPortraitLinkHeadword);
    }

    [Fact]
    public void PortraitRead_NarratedEtymology_CarriesTheProseAndItsSpans()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        long source = TPortraitEntryCreate(engine, "rinnan");
        long entryId = TPortraitEntryCreate(engine, "run");

        LDraft held = engine.TEngineDraftStart("Input", entryId);
        engine.TEngineRequestApply(TInterface.TEtymologyTextCreate(held.LDraftId, "From rinnan."));
        engine.TEngineRequestApply(TInterface.TEtymologyMentionCreate(held.LDraftId, 5, 6, source));
        engine.TEngineDraftCommit(held.LDraftId);

        LPortraitPage page = engine.TEnginePortraitRead(entryId, TInterface.TPortraitLabelRead());

        LPortraitSection band = Assert.Single(
            page.LPortraitPageSection,
            static row => row.LPortraitSectionHeading == "Etymology");
        Assert.Equal("From rinnan.", Assert.Single(band.LPortraitSectionLine).LPortraitLineText);
        Assert.Equal("rinnan", Assert.Single(band.LPortraitSectionLink).LPortraitLinkHeadword);
    }

    [Fact]
    public void PortraitRead_NoEtymology_AddsNoBand()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        long entryId = TPortraitEntryCreate(engine, "run");

        LPortraitPage page = engine.TEnginePortraitRead(entryId, TInterface.TPortraitLabelRead());

        Assert.DoesNotContain(
            page.LPortraitPageSection, static row => row.LPortraitSectionHeading == "Etymology");
    }

    private static long TPortraitEntryCreate(LEngine engine, string headword)
    {
        return engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            headword, "English", string.Empty, string.Empty, [], [])).LEntryId;
    }

    [Fact]
    public void PortraitRead_MissingEntry_Throws()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        Assert.Throws<InvalidOperationException>(() =>
            engine.TEnginePortraitRead(99, TInterface.TPortraitLabelRead()));
    }
}
