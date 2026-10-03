using System;
using System.Collections.Generic;
using System.Linq;
using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TDisplayMention
{
    [Fact]
    public void DisplayMentionFind_SentenceWithoutLanguage_ReadsItInTheShownLanguageAndOpensTheSoleEntry()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LEntry water = TDisplayEntrySave(engine, "water", "English");
        LEntry run = TDisplayEntrySave(
            engine,
            "run",
            "English",
            TDisplaySentenceCreate("water", string.Empty, []),
            TDisplaySentenceCreate("water", "French", []));
        List<string> asked = [];
        List<long> arrived = TDisplayLibraryAdd(atelier, asked, true);
        CDisplay area = TDisplayMentionPrepare(atelier, asked, run.LEntryId);
        IReadOnlyList<long> lines = TDisplayLineRead(area);

        CMentionOffer? found = area.CDisplayMentionFind(lines[0], 1);
        CMentionOffer? foreign = area.CDisplayMentionFind(lines[1], 1);

        Assert.Equal(0, found?.CMentionOfferOffset);
        Assert.Empty(found!.CMentionOfferEntry);
        Assert.Empty(foreign!.CMentionOfferEntry);
        Assert.Equal([water.LEntryId], arrived);
        Assert.Equal(["Library"], asked);
    }

    [Fact]
    public void DisplayMentionFind_SoleEntryLeaveDeclined_OpensNothingAndOffersNothing()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        TDisplayEntrySave(engine, "water", "English");
        LEntry run = TDisplayEntrySave(engine, "run", "English", TDisplaySentenceCreate("water", string.Empty, []));
        List<string> asked = [];
        List<long> arrived = TDisplayLibraryAdd(atelier, asked, false);
        CDisplay area = TDisplayMentionPrepare(atelier, asked, run.LEntryId);

        CMentionOffer? offer = area.CDisplayMentionFind(Assert.Single(TDisplayLineRead(area)), 1);

        Assert.Empty(offer!.CMentionOfferEntry);
        Assert.Empty(arrived);
        Assert.Equal(["Library"], asked);
    }

    [Fact]
    public void DisplayMentionFind_ManyEntries_OffersThemUnderTheFoundWordAndOpensNothing()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LEntry first = TDisplayEntrySave(engine, "water", "English");
        LEntry second = TDisplayEntrySave(engine, "water", "English");
        LEntry run = TDisplayEntrySave(
            engine, "run", "English", TDisplaySentenceCreate("the water", string.Empty, []));
        List<string> asked = [];
        List<long> arrived = TDisplayLibraryAdd(atelier, asked, true);
        CDisplay area = TDisplayMentionPrepare(atelier, asked, run.LEntryId);

        CMentionOffer? offer = area.CDisplayMentionFind(Assert.Single(TDisplayLineRead(area)), 5);

        Assert.Equal(4, offer?.CMentionOfferOffset);
        Assert.Equal(
            [first.LEntryId, second.LEntryId],
            offer!.CMentionOfferEntry.Select(entry => entry.CTranslationTargetId).Order());
        Assert.Empty(arrived);
        Assert.Empty(asked);
    }

    [Fact]
    public void DisplayMentionFind_NoEntry_OffersAnEmptyMenuAndOpensNothing()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LEntry run = TDisplayEntrySave(engine, "run", "English", TDisplaySentenceCreate("quartz", string.Empty, []));
        List<string> asked = [];
        List<long> arrived = TDisplayLibraryAdd(atelier, asked, true);
        CDisplay area = TDisplayMentionPrepare(atelier, asked, run.LEntryId);

        CMentionOffer? offer = area.CDisplayMentionFind(Assert.Single(TDisplayLineRead(area)), 1);

        Assert.NotNull(offer);
        Assert.Empty(offer!.CMentionOfferEntry);
        Assert.Empty(arrived);
        Assert.Empty(asked);
    }

    [Fact]
    public void DisplayMentionFind_StoredMention_OpensItsEntryAndRaisesItsSense()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LEntry water = TDisplayEntrySave(engine, "water", "English");
        long sense = Assert.Single(
            Assert.IsType<LEntryDraft>(engine.TEngineEntryLoad(water.LEntryId)).LEntryDraftMeanings).LCardDraftId;
        LEntry run = TDisplayEntrySave(
            engine,
            "run",
            "English",
            TDisplaySentenceCreate(
                "the water",
                string.Empty,
                [TInterfaceMentionSpan.TMentionDraftCreate(0, 4, 5, water.LEntryId, sense)]));
        List<string> asked = [];
        List<long> arrived = TDisplayLibraryAdd(atelier, asked, true);
        List<long> senses = [];
        atelier.CAtelierMention.CMentionSenseChosen += senses.Add;
        CDisplay area = TDisplayMentionPrepare(atelier, asked, run.LEntryId);

        CMentionOffer? offer = area.CDisplayMentionFind(Assert.Single(TDisplayLineRead(area)), 5);

        Assert.Empty(offer!.CMentionOfferEntry);
        Assert.Equal([water.LEntryId], arrived);
        Assert.Equal([sense], senses);
    }

    [Fact]
    public void DisplayMentionFind_RowNotShown_OffersAnEmptyMenuAndOpensNothing()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        TDisplayEntrySave(engine, "water", "English");
        LEntry run = TDisplayEntrySave(engine, "run", "English", TDisplaySentenceCreate("water", string.Empty, []));
        List<string> asked = [];
        List<long> arrived = TDisplayLibraryAdd(atelier, asked, true);
        CDisplay area = TDisplayMentionPrepare(atelier, asked, run.LEntryId);

        CMentionOffer? offer = area.CDisplayMentionFind(Assert.Single(TDisplayLineRead(area)) + 1000, 1);

        Assert.Empty(offer!.CMentionOfferEntry);
        Assert.Empty(arrived);
        Assert.Empty(asked);
    }

    [Fact]
    public void DisplayEtymologyFind_ProseWord_ReadsItInTheShownLanguageAndOpensTheSoleEntry()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LEntry water = TDisplayEntrySave(engine, "water", "English");
        LEntry run = TDisplayEntrySave(engine, "run", "English");
        LDraft held = engine.TEngineDraftStart("Input", run.LEntryId);
        engine.TEngineRequestApply(TInterface.TEtymologyTextCreate(held.LDraftId, "from water"));
        engine.TEngineDraftCommit(held.LDraftId);
        List<string> asked = [];
        List<long> arrived = TDisplayLibraryAdd(atelier, asked, true);
        CDisplay area = TDisplayMentionPrepare(atelier, asked, run.LEntryId);

        CMentionOffer? offer = area.CDisplayEtymologyFind(6);

        Assert.Equal(5, offer?.CMentionOfferOffset);
        Assert.Empty(offer!.CMentionOfferEntry);
        Assert.Equal([water.LEntryId], arrived);
        Assert.Equal(["Library"], asked);
    }

    [Fact]
    public void DisplayEtymologyFind_NothingShown_AnswersNothing()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        List<string> asked = [];
        CWing wing = CWing.CWingCreate(atelier, TEnvoyFake.TEnvoyCreate(false, asked), true);

        Assert.Null(wing.CWingDisplay.CDisplayArea.CDisplayEtymologyFind(1));
        Assert.Empty(asked);
    }

    [Fact]
    public void DisplayMentionFind_EngineFails_ShowsTheFindFailureAndOffersNothing()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LEntry run = TDisplayEntrySave(engine, "run", "English");
        List<string> asked = [];
        List<long> arrived = TDisplayLibraryAdd(atelier, asked, true);

        CMentionOffer? offer = TInterfaceMention.TDisplayMentionFind(
            engine, atelier, TEnvoyFake.TEnvoyCreate(false, asked), run.LEntryId);

        Assert.Null(offer);
        Assert.Equal(["Mention.FindFailed"], asked);
        Assert.Empty(arrived);
    }

    private static CDisplay TDisplayMentionPrepare(CAtelier atelier, List<string> asked, long shown)
    {
        CWing wing = CWing.CWingCreate(atelier, TEnvoyFake.TEnvoyCreate(false, asked), true);
        wing.CWingEntryOpen(shown);
        return wing.CWingDisplay.CDisplayArea;
    }

    private static IReadOnlyList<long> TDisplayLineRead(CDisplay area) =>
        Assert.Single(area.CDisplayCardRead().CLecternCardMeanings).CLeafSentence
            .Select(static line => line.CLeafLineSentence)
            .ToList();

    private static List<long> TDisplayLibraryAdd(CAtelier atelier, List<string> asked, bool leave)
    {
        List<long> arrived = [];
        atelier.CAtelierNavigation.TNavigationTabAdd(
            "Library",
            () =>
            {
                asked.Add("Library");
                return leave;
            },
            static () => 0,
            static _ => { },
            arrived.Add);
        return arrived;
    }

    private static LSentenceDraft TDisplaySentenceCreate(
        string text, string language, IReadOnlyList<LMentionDraft> mentions) =>
        TInterfaceExample.TSentenceDraftCreate(text) with
        {
            LSentenceDraftExample = TInterfaceExample.TExampleDraftCreate(text) with
            {
                LExampleDraftLanguage = language,
                LExampleDraftMention = mentions,
            },
        };

    private static LEntry TDisplayEntrySave(
        LEngine engine, string headword, string language, params LSentenceDraft[] sentences) =>
        engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            headword,
            language,
            string.Empty,
            string.Empty,
            [TInterface.TCardCreate("a thing", 1) with { LCardDraftSentence = sentences }],
            []));
}
