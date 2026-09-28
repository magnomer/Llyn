using System.Collections.Generic;
using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TDisplayCard
{
    [Fact]
    public void DisplayCardRead_ShownEntry_AnswersOrderLinksAndSections()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LEntry eau = engine.TEngineEntrySave(TInterface.TEntryDraftCreate("eau", "French", "", "", [], []));
        LEntry water = engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "water",
            "English",
            string.Empty,
            string.Empty,
            [TInterface.TCardCreate("a liquid", 1) with { LCardDraftTranslation = [eau.LEntryId] }],
            []));
        CWing wing = TDisplayWingPrepare(atelier, []);
        wing.CWingEntryOpen(water.LEntryId);

        CLecternCard card = wing.CWingDisplay.CDisplayArea.CDisplayCardRead();

        Assert.Equal(atelier.CAtelierCatalog.CCatalogOrderRead("English"), card.CLecternCardOrder);
        CTranslationTarget target = Assert.Single(card.CLecternCardTargets);
        Assert.Equal((eau.LEntryId, "eau", "French"), (
            target.CTranslationTargetId, target.CTranslationTargetHeadword, target.CTranslationTargetLanguage));
        Assert.True(card.CLecternCardDefined);
        Assert.False(card.CLecternCardCollocated);
    }

    [Fact]
    public void DisplayCardRead_NothingShown_AnswersNoLinksAndNoSections()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CWing wing = TDisplayWingPrepare(atelier, []);

        CLecternCard card = wing.CWingDisplay.CDisplayArea.CDisplayCardRead();

        Assert.Empty(card.CLecternCardTargets);
        Assert.Empty(card.CLecternCardCitations);
        Assert.False(card.CLecternCardDefined);
        Assert.False(card.CLecternCardCollocated);
    }

    [Fact]
    public void DisplayIncomingRead_NothingChosen_AnswersNone()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CWing wing = TDisplayWingPrepare(atelier, []);

        Assert.Empty(wing.CWingDisplay.CDisplayArea.CDisplayIncomingRead());
    }

    [Fact]
    public void DisplayEtymologyRead_SourceLink_ShowsTheFieldWithItsNamedLink()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LEntry rinnan = TDisplayEntrySave(engine, "rinnan");
        LEntry run = TDisplayEntrySave(engine, "run");
        LDraft held = engine.TEngineDraftStart("Input", run.LEntryId);
        engine.TEngineRequestApply(TInterface.TEtymonAdditionCreate(held.LDraftId, rinnan.LEntryId, 0));
        engine.TEngineDraftCommit(held.LDraftId);
        CWing wing = TDisplayWingPrepare(atelier, []);
        wing.CWingEntryOpen(run.LEntryId);

        CLecternEtymology etymology = wing.CWingDisplay.CDisplayArea.CDisplayEtymologyRead();

        Assert.Equal("English", etymology.CLecternEtymologyLanguage);
        Assert.Equal(rinnan.LEntryId, Assert.Single(etymology.CLecternEtymologyTargets).CTranslationTargetId);
        Assert.True(etymology.CLecternEtymologyShown);
        Assert.True(etymology.CLecternEtymologyDerived);
    }

    [Fact]
    public void DisplayEtymologyRead_Narrative_ShowsTheFieldWithItsText()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LEntry run = TDisplayEntrySave(engine, "run");
        LDraft held = engine.TEngineDraftStart("Input", run.LEntryId);
        engine.TEngineRequestApply(TInterface.TEtymologyTextCreate(held.LDraftId, "From rinnan."));
        engine.TEngineDraftCommit(held.LDraftId);
        CWing wing = TDisplayWingPrepare(atelier, []);
        wing.CWingEntryOpen(run.LEntryId);

        CLecternEtymology etymology = wing.CWingDisplay.CDisplayArea.CDisplayEtymologyRead();

        Assert.Equal("From rinnan.", etymology.CLecternEtymologyText);
        Assert.Empty(etymology.CLecternEtymologyTargets);
        Assert.True(etymology.CLecternEtymologyShown);
        Assert.True(etymology.CLecternEtymologyDerived);
    }

    [Fact]
    public void DisplayEtymologyRead_NoEtymology_HidesTheFieldAndTheSection()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LEntry run = TDisplayEntrySave(engine, "run");
        CWing wing = TDisplayWingPrepare(atelier, []);
        wing.CWingEntryOpen(run.LEntryId);

        CLecternEtymology etymology = wing.CWingDisplay.CDisplayArea.CDisplayEtymologyRead();

        Assert.False(etymology.CLecternEtymologyShown);
        Assert.False(etymology.CLecternEtymologyDerived);
    }

    [Fact]
    public void DisplayChipOpen_StoredRecord_OpensThePanelItsKindPicks()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CDisplay area = TDisplayWingPrepare(atelier, []).CWingDisplay.CDisplayArea;
        List<string> opened = [];

        bool situation = TDisplayChipOpen(
            area, TInterface.TSituationDraftCreate("a market") with { LSituationDraftId = 7 }, null, opened);
        bool register = TDisplayChipOpen(
            area, TInterface.TRegisterDraftCreate("formal") with { LRegisterDraftId = 8 }, null, opened);
        bool tag = TDisplayChipOpen(
            area, TInterface.TTagDraftCreate("literal")[0] with { LTagDraftId = 9 }, null, opened);
        bool link = TDisplayChipOpen(area, "a link chip", 12, opened);

        Assert.Equal((true, true, true, true), (situation, register, tag, link));
        Assert.Equal(["situation 7", "register 8", "tag 9", "entry 12"], opened);
    }

    [Fact]
    public void DisplayChipOpen_UnsavedRecord_OpensNothing()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CDisplay area = TDisplayWingPrepare(atelier, []).CWingDisplay.CDisplayArea;
        List<string> opened = [];

        bool situation = TDisplayChipOpen(area, TInterface.TSituationDraftCreate("a market"), null, opened);
        bool link = TDisplayChipOpen(area, null, 0, opened);
        bool none = TDisplayChipOpen(area, null, null, opened);

        Assert.Equal((false, false, false), (situation, link, none));
        Assert.Empty(opened);
    }

    [Fact]
    public void DisplayMentionFind_TextWithoutLanguage_ReadsItInTheShownLanguage()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LEntry water = TDisplayEntrySave(engine, "water");
        LEntry run = TDisplayEntrySave(engine, "run");
        List<string> asked = [];
        CWing wing = TDisplayWingPrepare(atelier, asked);
        wing.CWingEntryOpen(run.LEntryId);

        CMentionResult? found = wing.CWingDisplay.CDisplayArea.CDisplayMentionFind("water", string.Empty, 1, null);
        CMentionResult? foreign = wing.CWingDisplay.CDisplayArea.CDisplayMentionFind("water", "French", 1, null);

        Assert.Equal(water.LEntryId, found?.CMentionResultFirst);
        Assert.Empty(foreign!.CMentionResultEntry);
        Assert.Empty(asked);
    }

    [Fact]
    public void DisplayCardFind_ShownCards_AnswersTheListAndThePlace()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LEntry water = engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "water",
            "English",
            string.Empty,
            string.Empty,
            [TInterface.TCardCreate("a liquid", 1), TInterface.TCardCreate("a sea", 2)],
            [TInterface.TCardCreate("still water", 1)]));
        CWing wing = TDisplayWingPrepare(atelier, []);
        CDisplay area = wing.CWingDisplay.CDisplayArea;
        Assert.Null(area.CDisplayCardFind(1));
        wing.CWingEntryOpen(water.LEntryId);
        LEntryDraft shown = wing.CWingDisplay.LDisplaySound.LDisplayShown!;

        (CCompassPart, int)? sea = area.CDisplayCardFind(shown.LEntryDraftMeanings[1].LCardDraftId);
        (CCompassPart, int)? still = area.CDisplayCardFind(shown.LEntryDraftCollocations[0].LCardDraftId);

        Assert.Equal((CCompassPart.CCompassPartMeaning, 1), sea);
        Assert.Equal((CCompassPart.CCompassPartCollocation, 0), still);
        Assert.Null(area.CDisplayCardFind(987654));
    }

    private static bool TDisplayChipOpen(CDisplay area, object? chip, long? link, List<string> opened) =>
        area.CDisplayChipOpen(
            chip,
            link,
            id => TDisplayOpenAdd(opened, "entry", id),
            id => TDisplayOpenAdd(opened, "situation", id),
            id => TDisplayOpenAdd(opened, "register", id),
            id => TDisplayOpenAdd(opened, "tag", id));

    private static bool TDisplayOpenAdd(List<string> opened, string panel, long id)
    {
        opened.Add(panel + " " + id);
        return true;
    }

    private static CWing TDisplayWingPrepare(CAtelier atelier, List<string> asked)
    {
        CWing wing = CWing.CWingCreate(atelier, TInterfaceConduct.TEnvoyCreate(false, asked));
        wing.CWingVistaRestore("left");
        return wing;
    }

    private static LEntry TDisplayEntrySave(LEngine engine, string headword) =>
        engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            headword, "English", string.Empty, string.Empty, [TInterface.TCardCreate("a thing", 1)], []));
}
