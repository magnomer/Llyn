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
    public void DisplayMentionFind_TextWithoutLanguage_ReadsItInTheShownLanguageAndOpensTheSoleEntry()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LEntry water = TDisplayEntrySave(engine, "water", "English");
        LEntry run = TDisplayEntrySave(engine, "run", "English");
        List<string> asked = [];
        List<long> arrived = TDisplayLibraryAdd(atelier, asked, true);
        CDisplay area = TDisplayMentionPrepare(atelier, asked, run.LEntryId);

        CMentionOffer? found = area.CDisplayMentionFind("water", string.Empty, 1, null);
        CMentionOffer? foreign = area.CDisplayMentionFind("water", "French", 1, null);

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
        LEntry run = TDisplayEntrySave(engine, "run", "English");
        List<string> asked = [];
        List<long> arrived = TDisplayLibraryAdd(atelier, asked, false);
        CDisplay area = TDisplayMentionPrepare(atelier, asked, run.LEntryId);

        CMentionOffer? offer = area.CDisplayMentionFind("water", string.Empty, 1, null);

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
        LEntry run = TDisplayEntrySave(engine, "run", "English");
        List<string> asked = [];
        List<long> arrived = TDisplayLibraryAdd(atelier, asked, true);
        CDisplay area = TDisplayMentionPrepare(atelier, asked, run.LEntryId);

        CMentionOffer? offer = area.CDisplayMentionFind("the water", string.Empty, 5, null);

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
        LEntry run = TDisplayEntrySave(engine, "run", "English");
        List<string> asked = [];
        List<long> arrived = TDisplayLibraryAdd(atelier, asked, true);
        CDisplay area = TDisplayMentionPrepare(atelier, asked, run.LEntryId);

        CMentionOffer? offer = area.CDisplayMentionFind("quartz", string.Empty, 1, null);

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
        LEntry run = TDisplayEntrySave(engine, "run", "English");
        List<string> asked = [];
        List<long> arrived = TDisplayLibraryAdd(atelier, asked, true);
        List<long> senses = [];
        atelier.CAtelierMention.CMentionSenseChosen += senses.Add;
        CDisplay area = TDisplayMentionPrepare(atelier, asked, run.LEntryId);

        CMentionOffer? offer =
            area.CDisplayMentionFind("the water", string.Empty, 5, [new CMentionMark(1, 4, 5, 40, 7)]);

        Assert.Empty(offer!.CMentionOfferEntry);
        Assert.Equal([40L], arrived);
        Assert.Equal([7L], senses);
    }

    [Fact]
    public void DisplayMentionFind_EngineFails_ShowsTheFindFailureAndOffersNothing()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        List<string> asked = [];
        List<long> arrived = TDisplayLibraryAdd(atelier, asked, true);

        CMentionOffer? offer = TInterfaceMention.TDisplayMentionFind(
            engine, atelier, TEnvoyFake.TEnvoyCreate(false, asked));

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

    private static LEntry TDisplayEntrySave(LEngine engine, string headword, string language) =>
        engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            headword, language, string.Empty, string.Empty, [TInterface.TCardCreate("a thing", 1)], []));
}
