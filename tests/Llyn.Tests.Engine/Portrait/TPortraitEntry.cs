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
        engine.TEngineMorphologySave(false);

        LCardDraft meaning = TInterface.TCardDraftCreate(
            TInterfaceState.TStateValueCreate("set alight"),
            LStateValue.LStateValueUnspecified,
            TInterfaceState.TStateValueCreate("to set something burning"),
            [TInterfaceExample.TSentenceDraftCreate("she knelt to kindle the damp logs")],
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
    public void PortraitRead_ChosenUnitAndFreedom_LeadTheSpeechChips()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        engine.TEngineMorphologySave(false);
        LEntry entry = engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "un", "English", string.Empty, string.Empty, [TInterface.TCardCreate("not", 1)], [], speeches: ["Adverb"])
            with { LEntryDraftUnit = LUnit.LUnitMorpheme, LEntryDraftFreedom = LFreedom.LFreedomDependent });

        LPortraitPage page = engine.TEnginePortraitRead(entry.LEntryId, TInterface.TPortraitLabelRead());

        Assert.Equal(["Morpheme", "Dependent", "Adverb"], page.LPortraitPageChip);
    }

    [Fact]
    public void PortraitRead_LinkedCollocation_ShowsUnderTheLinkedMeaning()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        engine.TEngineMorphologySave(false);
        LCardDraft first = TInterface.TCardCreate("to place", 1) with { LCardDraftId = TInterface.TIdentityCreate() };
        LCardDraft second = TInterface.TCardCreate("to harden", 2) with { LCardDraftId = TInterface.TIdentityCreate() };
        LCardDraft collocation = TInterface.TCardDraftCreate(
            LStateValue.LStateValueUnspecified,
            TInterfaceState.TStateValueCreate("set off"),
            LStateValue.LStateValueUnspecified,
            [], [], [], [], [], 1) with
        {
            LCardDraftSense = [second.LCardDraftId, first.LCardDraftId],
        };
        LEntry entry = engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "set", "English", string.Empty, string.Empty, [first, second], [collocation]));

        LPortraitPage page = engine.TEnginePortraitRead(entry.LEntryId, TInterface.TPortraitLabelRead());

        Assert.DoesNotContain(
            "Other collocations", page.LPortraitPageSection.Select(row => row.LPortraitSectionHeading));
        IReadOnlyList<LPortraitSection> cards = page.LPortraitPageSection[0].LPortraitSectionChild;
        LPortraitSection rows = Assert.Single(
            cards[1].LPortraitSectionChild, row => row.LPortraitSectionRole == LPortraitRole.LPortraitRoleList);
        Assert.Equal("Collocations · 1", rows.LPortraitSectionHeading);
        LPortraitSection home = Assert.Single(rows.LPortraitSectionChild);
        Assert.Equal(LPortraitRole.LPortraitRoleRow, home.LPortraitSectionRole);
        Assert.Equal("set off", home.LPortraitSectionHeading);
        Assert.False(home.LPortraitSectionFold);
        Assert.Equal("→ set off see 2", cards[0].LPortraitSectionChild[^1].LPortraitSectionLine[0].LPortraitLineText);
    }

    [Fact]
    public void PortraitRead_UnlinkedCollocation_ShowsAsOtherRow()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        engine.TEngineMorphologySave(false);
        LCardDraft collocation = TInterface.TCardDraftCreate(
            LStateValue.LStateValueUnspecified,
            TInterfaceState.TStateValueCreate("set off"),
            TInterfaceState.TStateValueCreate("to start a journey"),
            [], [], [], [], [], 1);
        LEntry entry = engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "set", "English", string.Empty, string.Empty, [TInterface.TCardCreate("to place", 1)], [collocation]));
        LEntryDraft saved = engine.TEngineEntryLoad(entry.LEntryId)!;
        engine.TEngineFoldSave(entry.LEntryId, saved.LEntryDraftCollocations[0].LCardDraftId);

        LPortraitPage page = engine.TEnginePortraitRead(entry.LEntryId, TInterface.TPortraitLabelRead());

        LPortraitSection other = Assert.Single(
            page.LPortraitPageSection, section => section.LPortraitSectionHeading == "Other collocations");
        LPortraitSection row = Assert.Single(other.LPortraitSectionChild);
        Assert.Equal(LPortraitRole.LPortraitRoleRow, row.LPortraitSectionRole);
        Assert.Equal("set off", row.LPortraitSectionHeading);
        Assert.Equal("to start a journey", Assert.Single(row.LPortraitSectionLine).LPortraitLineText);
        Assert.True(row.LPortraitSectionFold);
    }

    [Fact]
    public void PortraitRead_FoldedCollocation_CarriesTheFoldOnItsRow()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        engine.TEngineMorphologySave(false);
        LCardDraft meaning = TInterface.TCardCreate("to place", 1) with { LCardDraftId = TInterface.TIdentityCreate() };
        LCardDraft collocation = TInterface.TCardDraftCreate(
            LStateValue.LStateValueUnspecified,
            TInterfaceState.TStateValueCreate("set off"),
            TInterfaceState.TStateValueCreate("to start a journey"),
            [], [], [], [], [], 1) with
        {
            LCardDraftSense = [meaning.LCardDraftId],
        };
        LEntry entry = engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "set", "English", string.Empty, string.Empty, [meaning], [collocation]));
        LEntryDraft saved = engine.TEngineEntryLoad(entry.LEntryId)!;
        engine.TEngineFoldSave(entry.LEntryId, saved.LEntryDraftCollocations[0].LCardDraftId);

        LPortraitPage page = engine.TEnginePortraitRead(entry.LEntryId, TInterface.TPortraitLabelRead());

        LPortraitSection rows = Assert.Single(
            page.LPortraitPageSection[0].LPortraitSectionChild[0].LPortraitSectionChild,
            row => row.LPortraitSectionRole == LPortraitRole.LPortraitRoleList);
        LPortraitSection row = Assert.Single(rows.LPortraitSectionChild);
        Assert.True(row.LPortraitSectionFold);
        Assert.Equal("to start a journey", Assert.Single(row.LPortraitSectionLine).LPortraitLineText);
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
