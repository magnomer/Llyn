using System.Collections.Generic;
using System.Linq;
using Llyn.Core;
using Llyn.Infrastructure;
using Xunit;

namespace Llyn.Tests;

public sealed class TDeckArchive
{
    [Fact]
    public void DeckCreate_NewDeck_ReadsBackSchedule()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        LDeckArchive decks = TInterface.TDeckArchiveCreate(workspace.TWorkspaceDatabase);
        LDeck draft = TDeckPrepare(workspace, "Verbs", saved: false);
        LDeckSchedule schedule = draft.LDeckSchedule with
        {
            LDeckScheduleOrder = LAlmanacOrder.LAlmanacOrderGoal,
            LDeckScheduleGoal = 15,
            LDeckScheduleSpaced = 25,
            LDeckSchedulePool = 5,
            LDeckScheduleStreak = 7,
            LDeckScheduleTarget = 100,
            LDeckScheduleQuorum = 3,
        };

        LDeck created = decks.TDeckCreate(draft with { LDeckSchedule = schedule });

        LDeck stored = decks.TDeckRead(created.LDeckId)!;
        Assert.Equal(schedule, stored.LDeckSchedule);
        Assert.Equal("Verbs", stored.LDeckCode);
        Assert.Equal(created.LDeckAddedUtc, stored.LDeckAddedUtc);
    }

    [Fact]
    public void DeckUpdate_QuorumCleared_ReadsNull()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        LDeckArchive decks = TInterface.TDeckArchiveCreate(workspace.TWorkspaceDatabase);
        LDeck draft = TDeckPrepare(workspace, "Verbs", saved: false);
        LDeck created = decks.TDeckCreate(
            draft with { LDeckSchedule = draft.LDeckSchedule with { LDeckScheduleQuorum = 3 } });

        decks.TDeckUpdate(created with { LDeckSchedule = created.LDeckSchedule with { LDeckScheduleQuorum = null } });

        Assert.Null(decks.TDeckRead(created.LDeckId)!.LDeckSchedule.LDeckScheduleQuorum);
    }

    [Fact]
    public void DeckDelete_DeckWithCues_RemovesCues()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        LDeckArchive decks = TInterface.TDeckArchiveCreate(workspace.TWorkspaceDatabase);
        LCueArchive cues = TInterface.TCueArchiveCreate(workspace.TWorkspaceDatabase);
        LDeck created = decks.TDeckCreate(TDeckPrepare(workspace, "Verbs", saved: false));
        LCue free = TInterface.TCueCreate(LGrain.LGrainFree, null, null, null, null, null);
        cues.TCueCreate(created.LDeckId, [free, free]);

        decks.TDeckDelete(created.LDeckId);

        Assert.Equal(0, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM cue;"));
    }

    [Fact]
    public void DeckDelete_OwnedStencil_RemovesStencil()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        LDeckArchive decks = TInterface.TDeckArchiveCreate(workspace.TWorkspaceDatabase);
        LDeck created = decks.TDeckCreate(TDeckPrepare(workspace, "Verbs", saved: false));

        decks.TDeckDelete(created.LDeckId);

        Assert.Equal(0, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM stencil;"));
        Assert.Equal(0, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM stencil_slot;"));
    }

    [Fact]
    public void DeckDelete_SavedStencil_KeepsStencil()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        LDeckArchive decks = TInterface.TDeckArchiveCreate(workspace.TWorkspaceDatabase);
        LStencilArchive stencils = TInterface.TStencilArchiveCreate(workspace.TWorkspaceDatabase);
        LDeck created = decks.TDeckCreate(TDeckPrepare(workspace, "Verbs", saved: true));

        decks.TDeckDelete(created.LDeckId);

        Assert.Null(decks.TDeckRead(created.LDeckId));
        Assert.NotNull(stencils.TStencilRead(created.LDeckStencilId));
    }

    [Fact]
    public void DeckOrderSet_Reversed_ReadsNewOrder()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        LDeckArchive decks = TInterface.TDeckArchiveCreate(workspace.TWorkspaceDatabase);
        string[] names = ["Nouns", "Verbs", "Idioms"];
        IReadOnlyList<long> created = names
            .Select(name => decks.TDeckCreate(TDeckPrepare(workspace, name, saved: false)).LDeckId)
            .ToList();

        decks.TDeckOrderSet(null, created.Reverse().ToList());

        Assert.Equal(["Idioms", "Verbs", "Nouns"], decks.TDeckRead().Select(static deck => deck.LDeckName));
        Assert.Equal([0, 1, 2], decks.TDeckRead().Select(static deck => deck.LDeckPosition));
    }

    [Fact]
    public void BearingSave_TwoDirections_ReadsBothSets()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        LDeckArchive decks = TInterface.TDeckArchiveCreate(workspace.TWorkspaceDatabase);
        LDeck created = decks.TDeckCreate(TDeckPrepare(workspace, "Verbs", saved: false));
        IReadOnlyList<LBearing> bearings =
        [
            TInterface.TBearingFixtureCreate(1, 1, 2, 1),
            TInterface.TBearingFixtureCreate(2, 2, 5, 2),
        ];

        decks.TBearingSave(created.LDeckId, bearings);

        Assert.Equal(bearings, decks.TBearingRead(created.LDeckId));
    }

    [Fact]
    public void CueCreate_MeaningGrain_BindsMeaning()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        LCueArchive cues = TInterface.TCueArchiveCreate(workspace.TWorkspaceDatabase);
        LDeck deck = TInterface.TDeckArchiveCreate(workspace.TWorkspaceDatabase)
            .TDeckCreate(TDeckPrepare(workspace, "Verbs", saved: false));
        long meaningId = TMeaningPrepare(workspace).LMeaningId;

        LCue created = Assert.Single(cues.TCueCreate(
            deck.LDeckId, [TInterface.TCueCreate(LGrain.LGrainMeaning, null, meaningId, null, null, null)]));

        LCue stored = Assert.Single(cues.TCueRead(deck.LDeckId));
        Assert.Equal(created, stored);
        Assert.Equal(meaningId, stored.LCueMeaningId);
        Assert.False(stored.LCueOrphaned);
    }

    [Fact]
    public void MeaningDelete_BoundCue_KeepsCueOrphaned()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        LCueArchive cues = TInterface.TCueArchiveCreate(workspace.TWorkspaceDatabase);
        LDeck deck = TInterface.TDeckArchiveCreate(workspace.TWorkspaceDatabase)
            .TDeckCreate(TDeckPrepare(workspace, "Verbs", saved: false));
        long meaningId = TMeaningPrepare(workspace).LMeaningId;
        cues.TCueCreate(deck.LDeckId, [TInterface.TCueCreate(LGrain.LGrainMeaning, null, meaningId, null, null, null)]);

        TInterface.TMeaningArchiveCreate(workspace.TWorkspaceDatabase).TMeaningDelete(meaningId);

        LCue kept = Assert.Single(cues.TCueRead(deck.LDeckId));
        Assert.Null(kept.LCueMeaningId);
        Assert.True(kept.LCueOrphaned);
    }

    [Fact]
    public void CueCellSave_TextNine_ReadsBackAllCells()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        LCueArchive cues = TInterface.TCueArchiveCreate(workspace.TWorkspaceDatabase);
        LDeck deck = TInterface.TDeckArchiveCreate(workspace.TWorkspaceDatabase)
            .TDeckCreate(TDeckPrepare(workspace, "Verbs", saved: false));
        LCue cue = Assert.Single(cues.TCueCreate(
            deck.LDeckId, [TInterface.TCueCreate(LGrain.LGrainFree, null, null, null, null, null)]));
        IReadOnlyList<LCueCell> cells = TInterface.TCueFixtureCreate(LCueColumn.LCueColumnText, 9);

        cues.TCueCellSave(cue.LCueId, cells.Reverse().ToList());

        Assert.Equal(cells, cues.TCueCellRead(cue.LCueId));
    }

    [Fact]
    public void StencilRead_SavedAndOwned_ReadsSavedOnly()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        LStencilArchive stencils = TInterface.TStencilArchiveCreate(workspace.TWorkspaceDatabase);
        LStencil saved = stencils.TStencilCreate(TInterface.TStencilFixtureCreate("Library", saved: true));
        stencils.TStencilCreate(TInterface.TStencilFixtureCreate("Owned", saved: false));

        IReadOnlyList<LStencil> listed = stencils.TStencilRead();

        LStencil only = Assert.Single(listed);
        Assert.Equal(saved.LStencilId, only.LStencilId);
        Assert.Equal(saved.LStencilSlots, only.LStencilSlots);
    }

    private static LDeck TDeckPrepare(TWorkspace workspace, string name, bool saved)
    {
        LStencil stencil = TInterface.TStencilArchiveCreate(workspace.TWorkspaceDatabase)
            .TStencilCreate(TInterface.TStencilFixtureCreate(name, saved));
        return TInterface.TDeckFixtureCreate(name, stencil.LStencilId);
    }

    private static LMeaning TMeaningPrepare(TWorkspace workspace)
    {
        LEntry entry = TInterface.TEntryArchiveCreate(workspace.TWorkspaceDatabase)
            .TEntryCreate(TInterface.TEntryCreate(0, "run", "en", 0, null, null), [], []);
        return TInterface.TMeaningArchiveCreate(workspace.TWorkspaceDatabase).TMeaningCreate(
            TInterface.TMeaningCreate(
                0, entry.LEntryId, null, 0, LStateValue.LStateValueUnspecified, LStateValue.LStateValueUnspecified));
    }
}
