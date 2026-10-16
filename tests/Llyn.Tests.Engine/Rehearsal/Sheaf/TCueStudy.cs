using System.Collections.Generic;
using System.Linq;
using Llyn.Application;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TCueStudy
{
    [Fact]
    public void StencilFaceRead_RestingCue_DropsCue()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        LRig rig = workspace.TWorkspaceRigCreate();
        LDeck deck = TInterface.TDeckClerkCreate(rig).TDeckClerkCreate("Banks", null, LGrain.LGrainMeaning);
        LCue kept = TInterface.TCueClerkCreate(rig).TCueClerkCreate(
            deck.LDeckId, [TInterface.TCueCellCreate(LCueColumn.LCueColumnText, 1, "bank")]);
        TCueOrphanCreate(rig, deck.LDeckId);

        var faces = TInterface.TStencilClerkCreate(rig)
            .TStencilFaceRead(deck.LDeckId, TInterface.TPortraitLabelRead());

        Assert.Equal(kept.LCueId, Assert.Single(faces).LCueStored.LCueId);
    }

    [Fact]
    public void StencilFaceRead_FreeCue_ReadsStoredCells()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        LRig rig = workspace.TWorkspaceRigCreate();
        LDeck deck = TInterface.TDeckClerkCreate(rig).TDeckClerkCreate("Notes", null, LGrain.LGrainMeaning);
        TInterface.TCueClerkCreate(rig).TCueClerkCreate(
            deck.LDeckId,
            [
                TInterface.TCueCellCreate(LCueColumn.LCueColumnText, 1, "front"),
                TInterface.TCueCellCreate(LCueColumn.LCueColumnText, 2, "back"),
            ]);

        var faces = TInterface.TStencilClerkCreate(rig)
            .TStencilFaceRead(deck.LDeckId, TInterface.TPortraitLabelRead());

        Assert.Equal(
            ["front", "back"],
            Assert.Single(faces).LCueShown.LCueFaceCells
                .OrderBy(static cell => cell.LCueCellIndex)
                .Select(static cell => cell.LCueCellValue));
    }

    [Fact]
    public void CueActiveRead_OrphanedCue_CountsOthers()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        LRig rig = workspace.TWorkspaceRigCreate();
        LDeckClerk decks = TInterface.TDeckClerkCreate(rig);
        LDeck banks = decks.TDeckClerkCreate("Banks", null, LGrain.LGrainMeaning);
        LDeck notes = decks.TDeckClerkCreate("Notes", null, LGrain.LGrainMeaning);
        LCueClerk clerk = TInterface.TCueClerkCreate(rig);
        clerk.TCueClerkCreate(banks.LDeckId, []);
        clerk.TCueClerkCreate(notes.LDeckId, []);
        clerk.TCueClerkCreate(notes.LDeckId, []);
        TCueOrphanCreate(rig, banks.LDeckId);

        IReadOnlyDictionary<long, int> active = clerk.TCueActiveRead();

        Assert.Equal(1, active[banks.LDeckId]);
        Assert.Equal(2, active[notes.LDeckId]);
    }

    [Fact]
    public void FaceRead_FreeCue_ReturnsFrontAndBack()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LDeckOutlet port = TInterface.TDeckOutletCreate(engine);
        long deck = port.TEngineDeckCreate("Notes", null, LGrain.LGrainMeaning);
        port.TEngineCueCreate(
            deck,
            [
                TInterface.TCueCellCreate(LCueColumn.LCueColumnText, 1, "bank"),
                TInterface.TCueCellCreate(LCueColumn.LCueColumnText, 2, "edge of a river|place for money"),
            ]);

        var face = Assert.Single(port.TEngineFaceRead(deck, TInterface.TPortraitLabelRead()));

        Assert.Equal("bank", face.LCueFront.LCueSideText);
        Assert.Equal("edge of a river\nplace for money", face.LCueBack.LCueSideText);
    }

    [Fact]
    public void ActiveRead_TwoDecks_ReturnsCounts()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LDeckOutlet port = TInterface.TDeckOutletCreate(engine);
        long folder = port.TEngineFolderCreate("Words");
        long banks = port.TEngineDeckCreate("Banks", folder, LGrain.LGrainMeaning);
        long notes = port.TEngineDeckCreate("Notes", null, LGrain.LGrainMeaning);
        port.TEngineCueCreate(banks, []);
        port.TEngineCueCreate(banks, []);

        var active = port.TEngineActiveRead();

        Assert.Equal(folder, Assert.Single(active.LDeckFolders).LDeckFolderId);
        Assert.Equal([banks, notes], active.LDeckListed.Select(static deck => deck.LDeckId).Order());
        Assert.Equal(2, active.LDeckActive[banks]);
        Assert.False(active.LDeckActive.ContainsKey(notes));
    }

    [Fact]
    public void CueCensusRead_DeckChosen_CountsOneDeck()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        LRig rig = workspace.TWorkspaceRigCreate();
        LDeckClerk decks = TInterface.TDeckClerkCreate(rig);
        LDeck banks = decks.TDeckClerkCreate("Banks", null, LGrain.LGrainMeaning);
        LDeck notes = decks.TDeckClerkCreate("Notes", null, LGrain.LGrainMeaning);
        LCueClerk clerk = TInterface.TCueClerkCreate(rig);
        clerk.TCueClerkCreate(banks.LDeckId, []);
        clerk.TCueClerkCreate(notes.LDeckId, []);
        TCueOrphanCreate(rig, banks.LDeckId);
        rig.LRigRehearsal.LRigRehearsalDecks.TDeckUpdate(banks with { LDeckSyncedUtc = "2026-10-03 07:00:00" });

        LDeckCensus census = clerk.TCueCensusRead(banks.LDeckId, TimeZoneInfo.Utc);

        Assert.Equal(2, census.LDeckCensusTotal);
        Assert.Equal(1, census.LDeckCensusActive);
        Assert.Equal(1, census.LDeckCensusOrphaned);
        Assert.Equal(new DateTimeOffset(2026, 10, 3, 7, 0, 0, TimeSpan.Zero), census.LDeckCensusSynced);
        Assert.Equal(2, census.LDeckCensusWeeks.Sum(static week => week.LDeckWeekAdded));
    }

    [Fact]
    public void CensusRead_NoDeckChosen_ReturnsLatestSync()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LDeckOutlet port = TInterface.TDeckOutletCreate(engine);
        long banks = port.TEngineDeckCreate("Banks", null, LGrain.LGrainMeaning);
        long notes = port.TEngineDeckCreate("Notes", null, LGrain.LGrainMeaning);
        port.TEngineCueCreate(banks, []);
        port.TEngineCueCreate(notes, []);
        port.TEngineCueCreate(notes, []);
        LRig rig = workspace.TWorkspaceRigCreate();
        LDeckVault vault = rig.LRigRehearsal.LRigRehearsalDecks;
        vault.TDeckUpdate(vault.TDeckRead(banks)! with { LDeckSyncedUtc = "2026-10-01 08:00:00" });
        vault.TDeckUpdate(vault.TDeckRead(notes)! with { LDeckSyncedUtc = "2026-10-03 07:00:00" });

        LDeckCensus census = port.TEngineCensusRead(null);

        Assert.Equal(3, census.LDeckCensusTotal);
        Assert.Equal(new DateTimeOffset(2026, 10, 3, 7, 0, 0, TimeSpan.Zero), census.LDeckCensusSynced);
        Assert.Equal(3, census.LDeckCensusWeeks.Sum(static week => week.LDeckWeekAdded));
    }

    private static void TCueOrphanCreate(LRig rig, long deckId)
    {
        LCue orphan = TInterface.TCueCreate(LGrain.LGrainMeaning, null, null, null, null, null)
            with { LCueDeckId = deckId };
        rig.LRigRehearsal.LRigRehearsalCues.TCueCreate(deckId, [orphan]);
    }
}
