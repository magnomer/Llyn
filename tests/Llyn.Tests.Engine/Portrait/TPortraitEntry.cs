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
        Assert.Equal("Collocations", rows.LPortraitSectionHeading);
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
    public void PortraitRead_LatinHeadword_DropsGlyphSection()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LEntry latin = engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "ma", "Mandarin", string.Empty, string.Empty, [TInterface.TCardCreate("mother", 1)], []));
        LEntry han = engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "妈", "Mandarin", string.Empty, string.Empty, [TInterface.TCardCreate("mother", 1)], []));
        LPortraitLabel label = TInterface.TPortraitLabelRead();

        LPortraitPage page = engine.TEnginePortraitRead(latin.LEntryId, label);

        Assert.DoesNotContain(
            label.LPortraitLabelGlyph, page.LPortraitPageSection.Select(row => row.LPortraitSectionHeading));
        LPortraitSection glyph = Assert.Single(
            engine.TEnginePortraitRead(han.LEntryId, label).LPortraitPageSection,
            row => row.LPortraitSectionHeading == label.LPortraitLabelGlyph);
        Assert.Equal(["妈"], glyph.LPortraitSectionChip);
    }

    [Fact]
    public void PortraitRead_MissingEntry_Throws()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        Assert.Throws<InvalidOperationException>(() =>
            engine.TEnginePortraitRead(99, TInterface.TPortraitLabelRead()));
    }

    [Fact]
    public void PortraitRead_LinkedAncestor_DrawsTheBorrowedMapAboveIt()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LEntry source = engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "renn", "English", string.Empty, string.Empty, [TInterface.TCardCreate("a course", 1)], []) with
        {
            LEntryDraftAncestors = [TInterface.TAncestorDraftCreate(-1, null, null, "rinnan", "Latin", null, "flow")],
        });
        LEntry entry = engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "run", "English", string.Empty, string.Empty, [TInterface.TCardCreate("to move fast", 1)], []) with
        {
            LEntryDraftAncestors = [TInterface.TAncestorDraftCreate(-1, null, source.LEntryId, string.Empty)],
        });

        string page = TInterface.TOutlineFormat(
            engine.TEnginePortraitRead(entry.LEntryId, TInterface.TPortraitLabelRead()));

        Assert.Contains(
            "- run\n- ↑ English renn\n- · ⇡ Latin rinnan ‘flow’\n\n→ renn (English)\n\n",
            page,
            System.StringComparison.Ordinal);
    }

    [Fact]
    public void SheetEntryFormat_LinkedAncestor_DrawsTheTreeAboveTheHeadword()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LEntry source = engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "renn", "English", string.Empty, string.Empty, [TInterface.TCardCreate("a course", 1)], []) with
        {
            LEntryDraftAncestors = [TInterface.TAncestorDraftCreate(-1, null, null, "rinnan", "Latin", null, "flow")],
        });
        LEntry entry = engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "run", "English", string.Empty, string.Empty, [TInterface.TCardCreate("to move fast", 1)], []) with
        {
            LEntryDraftAncestors = [TInterface.TAncestorDraftCreate(-1, null, source.LEntryId, string.Empty)],
        });

        string sheet = TInterface.TSheetEntryFormat(
            Assert.IsType<LLiveryPage>(engine.TLiveryRead(entry.LEntryId)),
            engine.TEnginePortraitRead(entry.LEntryId, TInterface.TPortraitLabelRead()),
            static key => key);

        Assert.Contains(
            "<div class=\"lineage\"><div class=\"tree\">\n<div class=\"stock\">\n"
                + "<div class=\"etymon\"><div class=\"stock\">\n"
                + "<div class=\"etymon\"><span class=\"tongue\">Latin</span>"
                + "<span class=\"word borrowed\">rinnan</span><span class=\"gloss\">flow</span>"
                + "<i class=\"stem\"></i><i class=\"stem\"></i></div>\n</div>\n"
                + "<i class=\"stem\"></i><span class=\"tongue\">English</span>"
                + "<span class=\"word linked\">renn<b>↗</b></span>"
                + "<i class=\"stem\"></i><i class=\"stem\"></i></div>\n</div>\n"
                + "<i class=\"stem\"></i><div class=\"root\">run</div>\n</div></div>",
            sheet,
            System.StringComparison.Ordinal);
        Assert.DoesNotContain("⇡", sheet, System.StringComparison.Ordinal);
    }
}
