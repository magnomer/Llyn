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
    public void DisplayMentionFind_UnlabeledSentence_OpensSoleEntry()
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

        CMentionOffer? found = area.CDisplayRoute.CDisplayMentionFind(lines[0], "water", 1);
        CMentionOffer? foreign = area.CDisplayRoute.CDisplayMentionFind(lines[1], "water", 1);

        Assert.Equal(0, found?.CMentionOfferUnit);
        Assert.Empty(found!.CMentionOfferEntry);
        Assert.Empty(foreign!.CMentionOfferEntry);
        Assert.Equal([water.LEntryId], arrived);
        Assert.Equal(["Library"], asked);
    }

    [Fact]
    public void DisplayMentionFind_LeaveDeclined_OffersNothing()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        TDisplayEntrySave(engine, "water", "English");
        LEntry run = TDisplayEntrySave(engine, "run", "English", TDisplaySentenceCreate("water", string.Empty, []));
        List<string> asked = [];
        List<long> arrived = TDisplayLibraryAdd(atelier, asked, false);
        CDisplay area = TDisplayMentionPrepare(atelier, asked, run.LEntryId);

        CMentionOffer? offer =
            area.CDisplayRoute.CDisplayMentionFind(Assert.Single(TDisplayLineRead(area)), "water", 1);

        Assert.Empty(offer!.CMentionOfferEntry);
        Assert.Empty(arrived);
        Assert.Equal(["Library"], asked);
    }

    [Fact]
    public void DisplayMentionFind_ManyEntries_OffersAllUnderFoundWord()
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

        CMentionOffer? offer =
            area.CDisplayRoute.CDisplayMentionFind(Assert.Single(TDisplayLineRead(area)), "the water", 5);

        Assert.Equal(4, offer?.CMentionOfferUnit);
        Assert.Equal(
            [first.LEntryId, second.LEntryId],
            offer!.CMentionOfferEntry.Select(entry => entry.CTranslationTargetId).Order());
        Assert.Empty(arrived);
        Assert.Empty(asked);
    }

    [Fact]
    public void DisplayMentionFind_AstralPrefix_OffersWholeTextUnit()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LEntry water = TDisplayEntrySave(engine, "water", "English");
        TDisplayEntrySave(engine, "waterfall", "English");
        TDisplayEntrySave(engine, "waterfall", "English");
        string text = "\U0001F600 waterfall";
        LEntry run = TDisplayEntrySave(
            engine,
            "run",
            "English",
            TDisplaySentenceCreate(
                text, string.Empty, [TInterfaceMentionSpan.TMentionDraftCreate(0, 2, 5, water.LEntryId, 0)]));
        List<string> asked = [];
        List<long> arrived = TDisplayLibraryAdd(atelier, asked, true);
        CDisplay area = TDisplayMentionPrepare(atelier, asked, run.LEntryId);
        CLeafLine line = Assert.Single(
            Assert.Single(area.CDisplayCard.CDisplayCardRead().CLecternCardMeanings).CLeafSentence);

        CMentionOffer? offer = area.CDisplayRoute.CDisplayMentionFind(line.CLeafLineSentence, text, 8);

        Assert.Equal("fall", line.CLeafLinePiece[^1].CMentionPieceText);
        Assert.Equal(text, string.Concat(line.CLeafLinePiece.Select(static piece => piece.CMentionPieceText)));
        Assert.Equal(3, offer?.CMentionOfferUnit);
        Assert.Equal(2, offer!.CMentionOfferEntry.Count);
        Assert.Empty(arrived);
    }

    [Fact]
    public void DisplayMentionFind_NoEntry_OffersEmptyMenu()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LEntry run = TDisplayEntrySave(engine, "run", "English", TDisplaySentenceCreate("quartz", string.Empty, []));
        List<string> asked = [];
        List<long> arrived = TDisplayLibraryAdd(atelier, asked, true);
        CDisplay area = TDisplayMentionPrepare(atelier, asked, run.LEntryId);

        CMentionOffer? offer =
            area.CDisplayRoute.CDisplayMentionFind(Assert.Single(TDisplayLineRead(area)), "quartz", 1);

        Assert.NotNull(offer);
        Assert.Empty(offer!.CMentionOfferEntry);
        Assert.Empty(arrived);
        Assert.Empty(asked);
    }

    [Fact]
    public void DisplayMentionFind_StoredMention_OpensEntryRaisesSense()
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

        CMentionOffer? offer =
            area.CDisplayRoute.CDisplayMentionFind(Assert.Single(TDisplayLineRead(area)), "the water", 5);

        Assert.Empty(offer!.CMentionOfferEntry);
        Assert.Equal([water.LEntryId], arrived);
        Assert.Equal([sense], senses);
    }

    [Fact]
    public void DisplayMentionFind_RowNotShown_OffersEmptyMenu()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        TDisplayEntrySave(engine, "water", "English");
        LEntry run = TDisplayEntrySave(engine, "run", "English", TDisplaySentenceCreate("water", string.Empty, []));
        List<string> asked = [];
        List<long> arrived = TDisplayLibraryAdd(atelier, asked, true);
        CDisplay area = TDisplayMentionPrepare(atelier, asked, run.LEntryId);

        CMentionOffer? offer =
            area.CDisplayRoute.CDisplayMentionFind(Assert.Single(TDisplayLineRead(area)) + 1000, "water", 1);

        Assert.Empty(offer!.CMentionOfferEntry);
        Assert.Empty(arrived);
        Assert.Empty(asked);
    }

    [Fact]
    public void DisplayMentionFind_EngineFails_ShowsFindFailure()
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

    [Fact]
    public void DisplayWordMove_StepsAndSeeks_StopsOnWordsAndPieces()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LEntry water = TDisplayEntrySave(engine, "water", "English");
        LEntry run = TDisplayEntrySave(
            engine,
            "run",
            "English",
            TDisplaySentenceCreate(
                "\U0001F600 the water",
                string.Empty,
                [TInterfaceMentionSpan.TMentionDraftCreate(0, 2, 3, water.LEntryId)]));
        CDisplay area = TDisplayMentionPrepare(atelier, [], run.LEntryId);
        CDisplayRoute route = area.CDisplayRoute;
        CLeafLine line = Assert.Single(
            Assert.Single(area.CDisplayCard.CDisplayCardRead().CLecternCardMeanings).CLeafSentence);
        List<(int, int, string)> seen = [];
        Action<Action> step = move =>
        {
            move();
            seen.Add((route.CDisplayWordUnit, route.CDisplayWordLength, route.CDisplayWordLead));
        };

        step(() => route.CDisplayWordStart(line.CLeafLineSentence));
        step(() => route.CDisplayWordMove(1));
        step(() => route.CDisplayWordMove(1));
        step(() => route.CDisplayWordSeek(1));
        step(() => route.CDisplayWordSeek(1));
        step(() => route.CDisplayWordMove(1));
        step(() => route.CDisplayWordSeek(1));
        step(() => route.CDisplayWordSeek(-1));

        Assert.Equal(
            [
                (0, 2, string.Empty), (2, 1, "\U0001F600"), (3, 1, string.Empty), (6, 1, string.Empty),
                (7, 1, string.Empty), (8, 1, "w"), (8, 1, string.Empty), (7, 1, string.Empty),
            ],
            seen);
    }

    [Fact]
    public void DisplayWordFind_ManyEntries_OffersAllAndLeadsFoundWord()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        TDisplayEntrySave(engine, "water", "English");
        TDisplayEntrySave(engine, "water", "English");
        LEntry run = TDisplayEntrySave(engine, "run", "English", TDisplaySentenceCreate("the water", string.Empty, []));
        List<string> asked = [];
        List<long> arrived = TDisplayLibraryAdd(atelier, asked, true);
        CDisplay area = TDisplayMentionPrepare(atelier, asked, run.LEntryId);
        CDisplayRoute route = area.CDisplayRoute;
        Assert.Null(route.CDisplayWordFind());

        CLeafLine line = Assert.Single(
            Assert.Single(area.CDisplayCard.CDisplayCardRead().CLecternCardMeanings).CLeafSentence);
        route.CDisplayWordStart(line.CLeafLineSentence + 1000);
        Assert.Null(route.CDisplayWordFind());
        route.CDisplayWordStart(line.CLeafLineSentence);
        route.CDisplayWordSeek(1);
        route.CDisplayWordMove(1);
        route.CDisplayWordMove(1);
        CMentionOffer? offer = route.CDisplayWordFind();

        Assert.Equal(2, offer?.CMentionOfferEntry.Count);
        Assert.Equal((6, "wa"), (route.CDisplayWordUnit, route.CDisplayWordLead));
        Assert.Empty(arrived);
    }

    [Fact]
    public void DisplayWordStart_SharedLinkId_StartsOnCollocationLine()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        TDisplayEntrySave(engine, "water", "English");
        TDisplayEntrySave(engine, "water", "English");
        LEntry run = TDisplayPairSave(engine);
        List<string> asked = [];
        List<long> arrived = TDisplayLibraryAdd(atelier, asked, true);
        CDisplay area = TDisplayMentionPrepare(atelier, asked, run.LEntryId);
        CLecternCard card = area.CDisplayCard.CDisplayCardRead();
        long first = Assert.Single(Assert.Single(card.CLecternCardMeanings).CLeafSentence).CLeafLineSentence;
        long second = Assert.Single(Assert.Single(card.CLecternCardOther).CLeafSentence).CLeafLineSentence;

        area.CDisplayRoute.CDisplayWordStart(second);
        area.CDisplayRoute.CDisplayWordMove(1);
        CMentionOffer? offer = area.CDisplayRoute.CDisplayMentionFind(second, "water", 1);

        Assert.NotEqual(first, second);
        Assert.Equal("w", area.CDisplayRoute.CDisplayWordLead);
        Assert.Equal(2, offer?.CMentionOfferEntry.Count);
        Assert.Empty(arrived);
    }

    [Fact]
    public void DisplayCardRead_RedrawnCard_KeepsLineKeys()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LEntry run = TDisplayPairSave(engine);
        CWing wing = CWing.CWingCreate(atelier, TEnvoyFake.TEnvoyCreate(false, []), true);
        wing.CWingEntryOpen(run.LEntryId);

        IReadOnlyList<long> drawn = TDisplayKeyRead(wing.CWingDisplay);
        IReadOnlyList<long> again = TDisplayKeyRead(wing.CWingDisplay);
        wing.CWingEntryOpen(run.LEntryId);

        Assert.Equal(2, drawn.Distinct().Count());
        Assert.Equal(drawn, again);
        Assert.Equal(drawn, TDisplayKeyRead(wing.CWingDisplay));
    }

    private static IReadOnlyList<long> TDisplayKeyRead(CDisplay area)
    {
        CLecternCard card = area.CDisplayCard.CDisplayCardRead();
        return card.CLecternCardMeanings.Concat(card.CLecternCardOther)
            .SelectMany(static leaf => leaf.CLeafSentence)
            .Select(static line => line.CLeafLineSentence)
            .ToList();
    }

    private static LEntry TDisplayPairSave(LEngine engine) =>
        engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "run",
            "English",
            string.Empty,
            string.Empty,
            [
                TInterface.TCardCreate("a thing", 1) with
                {
                    LCardDraftSentence = [TDisplaySentenceCreate("alpha", string.Empty, [])],
                },
            ],
            [
                TInterface.TCardCreate("a turn", 1) with
                {
                    LCardDraftSentence = [TDisplaySentenceCreate("water", string.Empty, [])],
                },
            ]));

    private static CDisplay TDisplayMentionPrepare(CAtelier atelier, List<string> asked, long shown)
    {
        CWing wing = CWing.CWingCreate(atelier, TEnvoyFake.TEnvoyCreate(false, asked), true);
        wing.CWingEntryOpen(shown);
        return wing.CWingDisplay;
    }

    private static IReadOnlyList<long> TDisplayLineRead(CDisplay area) =>
        Assert.Single(area.CDisplayCard.CDisplayCardRead().CLecternCardMeanings).CLeafSentence
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
