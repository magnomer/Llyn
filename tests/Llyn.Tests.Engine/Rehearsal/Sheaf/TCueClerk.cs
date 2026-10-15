using System.Collections.Generic;
using System.Linq;
using Llyn.Application;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TCueClerk
{
    [Fact]
    public void CueClerkScan_ThreeMeanings_ReturnsThreeRows()
    {
        LEntryDraft draft = TCueDraftCreate([TCueCardCreate(11), TCueCardCreate(12), TCueCardCreate(13)], []);

        IReadOnlyList<LCue> rows = TInterface.TCueClerkScan(draft, LGrain.LGrainMeaning);

        Assert.Equal([11L, 12L, 13L], rows.Select(static cue => cue.LCueMeaningId));
    }

    [Fact]
    public void CueClerkScan_NestedMeaning_ReturnsChild()
    {
        LCardDraft parent = TCueCardCreate(21) with { LCardDraftChild = [TCueCardCreate(22)] };

        IReadOnlyList<LCue> rows = TInterface.TCueClerkScan(TCueDraftCreate([parent], []), LGrain.LGrainMeaning);

        Assert.Equal([21L, 22L], rows.Select(static cue => cue.LCueMeaningId));
    }

    [Fact]
    public void CueClerkScan_PairGrain_ReturnsRowPerTarget()
    {
        LEntryDraft draft = TCueDraftCreate([TCueCardCreate(31, [5, 6])], []);

        IReadOnlyList<LCue> rows = TInterface.TCueClerkScan(draft, LGrain.LGrainPair);

        Assert.Equal([5L, 6L], rows.Select(static cue => cue.LCueTargetId));
        Assert.All(rows, static cue => Assert.Equal(31, cue.LCueMeaningId));
    }

    [Fact]
    public void CueClerkScan_CollocationPair_ReturnsRowPerTarget()
    {
        LEntryDraft draft = TCueDraftCreate([], [TCueCardCreate(41, [7, 8])]);

        IReadOnlyList<LCue> rows = TInterface.TCueClerkScan(draft, LGrain.LGrainPair);

        Assert.Equal([7L, 8L], rows.Select(static cue => cue.LCueTargetId));
        Assert.All(rows, static cue => Assert.Equal(41, cue.LCueCollocationId));
    }

    [Fact]
    public void CueClerkAdd_AlreadyBound_SkipsRow()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        LRig rig = workspace.TWorkspaceRigCreate();
        LEntry bank = TCueEntrySave(rig);
        LDeck deck = TInterface.TDeckClerkCreate(rig).TDeckClerkCreate("Banks", null, LGrain.LGrainMeaning);
        LCueClerk clerk = TInterface.TCueClerkCreate(rig);
        Assert.Equal(2, clerk.TCueClerkAdd(deck.LDeckId, [bank.LEntryId]).Count);

        Assert.Empty(clerk.TCueClerkAdd(deck.LDeckId, [bank.LEntryId]));

        Assert.Equal(2, rig.LRigRehearsal.LRigRehearsalCues.TCueRead(deck.LDeckId).Count);
    }

    [Fact]
    public void AccessionFind_BoundEntry_MarksBound()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        LRig rig = workspace.TWorkspaceRigCreate();
        LEntry bank = TCueEntrySave(rig);
        LEntry river = TInterface.TEntryClerkCreate(rig).TEntryClerkSave(
            TInterface.TEntryDraftCreate("river", "English", string.Empty, string.Empty, [], []));
        LDeck deck = TInterface.TDeckClerkCreate(rig).TDeckClerkCreate("Banks", null, LGrain.LGrainEntry);
        TInterface.TCueClerkCreate(rig).TCueClerkAdd(deck.LDeckId, [bank.LEntryId]);
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LVista vista = engine.TEngineVistaStart("accession", LCatalogOrder.LCatalogOrderHeadword);

        var found = TInterface.TDeckOutletCreate(engine).TEngineAccessionFind(vista, deck.LDeckId);

        Assert.Equal([bank.LEntryId, river.LEntryId], found.Select(static row => row.LAccessionRow.LVistaRowId));
        Assert.Equal([true, false], found.Select(static row => row.LAccessionBound));
    }

    [Fact]
    public void CueClerkRead_TwoDecks_ReturnsDeckCues()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        LRig rig = workspace.TWorkspaceRigCreate();
        LEntry bank = TCueEntrySave(rig);
        LDeckClerk decks = TInterface.TDeckClerkCreate(rig);
        LDeck banks = decks.TDeckClerkCreate("Banks", null, LGrain.LGrainMeaning);
        LDeck notes = decks.TDeckClerkCreate("Notes", null, LGrain.LGrainMeaning);
        LCueClerk clerk = TInterface.TCueClerkCreate(rig);
        clerk.TCueClerkAdd(banks.LDeckId, [bank.LEntryId]);
        clerk.TCueClerkCreate(notes.LDeckId, []);

        IReadOnlyList<LCue> cues = clerk.TCueClerkRead(banks.LDeckId);

        Assert.Equal(2, cues.Count);
        Assert.All(cues, cue => Assert.Equal(banks.LDeckId, cue.LCueDeckId));
    }

    [Fact]
    public void DeckClerkCreate_NewDeck_OwnsTwoBearings()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        LRig rig = workspace.TWorkspaceRigCreate();
        LDeckClerk clerk = TInterface.TDeckClerkCreate(rig);

        LDeck deck = clerk.TDeckClerkCreate("Verbs", null, LGrain.LGrainMeaning);

        Assert.Equal(
            [TInterface.TBearingFixtureCreate(1, 1, 2, 1), TInterface.TBearingFixtureCreate(2, 2, 1, 2)],
            clerk.TDeckClerkRead(deck.LDeckId)!.Value.LDeckBearings);
        Assert.False(rig.LRigRehearsal.LRigRehearsalStencils.TStencilRead(deck.LDeckStencilId)!.LStencilSaved);
    }

    [Fact]
    public void StencilClerkRead_NewDeck_ReturnsGrainDefault()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        LRig rig = workspace.TWorkspaceRigCreate();
        LDeck deck = TInterface.TDeckClerkCreate(rig).TDeckClerkCreate("Phrases", null, LGrain.LGrainCollocation);

        LStencil stencil = TInterface.TStencilClerkCreate(rig).TStencilClerkRead(deck.LDeckId);

        Assert.Equal(TInterface.TStencilDefaultRead(LGrain.LGrainCollocation), stencil.LStencilSlots);
    }

    [Fact]
    public void StencilSlotSet_TakenSlot_ReplacesBody()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        LRig rig = workspace.TWorkspaceRigCreate();
        LDeck deck = TInterface.TDeckClerkCreate(rig).TDeckClerkCreate("Verbs", null, LGrain.LGrainMeaning);
        LStencilClerk clerk = TInterface.TStencilClerkCreate(rig);
        LStencilSlot slot = new(LCueColumn.LCueColumnText, 2, "{definition:all}");

        clerk.TStencilSlotSet(deck.LDeckId, slot);

        IReadOnlyList<LStencilSlot> slots = clerk.TStencilClerkRead(deck.LDeckId).LStencilSlots;
        Assert.Equal(TInterface.TStencilDefaultRead(LGrain.LGrainMeaning).Count, slots.Count);
        Assert.Contains(slot, slots);
    }

    [Fact]
    public void StencilSlotSet_NewSlot_AppendsSlot()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        LRig rig = workspace.TWorkspaceRigCreate();
        LDeck deck = TInterface.TDeckClerkCreate(rig).TDeckClerkCreate("Verbs", null, LGrain.LGrainMeaning);
        LStencilClerk clerk = TInterface.TStencilClerkCreate(rig);
        LStencilSlot slot = new(LCueColumn.LCueColumnNotes, 1, "{tag}");

        clerk.TStencilSlotSet(deck.LDeckId, slot);

        IReadOnlyList<LStencilSlot> slots = clerk.TStencilClerkRead(deck.LDeckId).LStencilSlots;
        Assert.Equal(TInterface.TStencilDefaultRead(LGrain.LGrainMeaning).Count + 1, slots.Count);
        Assert.Contains(slot, slots);
    }

    [Fact]
    public void StencilClerkApply_SavedStencil_CopiesIntoDeck()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        LRig rig = workspace.TWorkspaceRigCreate();
        LStencilVault stencils = rig.LRigRehearsal.LRigRehearsalStencils;
        LDeck deck = TInterface.TDeckClerkCreate(rig).TDeckClerkCreate("Verbs", null, LGrain.LGrainMeaning);
        LStencil saved = stencils.TStencilCreate(TInterface.TStencilFixtureCreate("Headwords", saved: true));

        TInterface.TStencilClerkCreate(rig).TStencilClerkApply(deck.LDeckId, saved.LStencilId);

        Assert.Equal(saved.LStencilSlots, stencils.TStencilRead(deck.LDeckStencilId)!.LStencilSlots);
    }

    [Fact]
    public void StencilClerkSave_EditAfterSave_LeavesSavedAsIs()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        LRig rig = workspace.TWorkspaceRigCreate();
        LStencilVault stencils = rig.LRigRehearsal.LRigRehearsalStencils;
        LStencilClerk clerk = TInterface.TStencilClerkCreate(rig);
        LDeck deck = TInterface.TDeckClerkCreate(rig).TDeckClerkCreate("Verbs", null, LGrain.LGrainMeaning);
        LStencil first = stencils.TStencilCreate(TInterface.TStencilFixtureCreate("Headwords", saved: true));
        LStencil second = stencils.TStencilCreate(
            TInterface.TStencilSlotCreate(LCueColumn.LCueColumnText, "{Definition}") with { LStencilSaved = true });
        clerk.TStencilClerkApply(deck.LDeckId, first.LStencilId);
        clerk.TStencilClerkSave(deck.LDeckId, "Mine");

        clerk.TStencilClerkApply(deck.LDeckId, second.LStencilId);

        LStencil mine = stencils.TStencilRead().Single(static stencil => stencil.LStencilName == "Mine");
        Assert.Equal(first.LStencilSlots, mine.LStencilSlots);
    }

    [Fact]
    public void CueCellSave_FreeCue_ReadsBackCells()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        LRig rig = workspace.TWorkspaceRigCreate();
        LDeck deck = TInterface.TDeckClerkCreate(rig).TDeckClerkCreate("Notes", null, LGrain.LGrainMeaning);
        LCueClerk clerk = TInterface.TCueClerkCreate(rig);
        LCue cue = clerk.TCueClerkCreate(deck.LDeckId, []);
        IReadOnlyList<LCueCell> cells = TInterface.TCueFixtureCreate(LCueColumn.LCueColumnText, 2);

        clerk.TCueCellSave(cue.LCueId, cells);

        Assert.Equal(cells, clerk.TCueCellRead(cue.LCueId));
    }

    [Fact]
    public void CueCellSave_BoundCue_Refuses()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        LRig rig = workspace.TWorkspaceRigCreate();
        LEntry bank = TCueEntrySave(rig);
        LDeck deck = TInterface.TDeckClerkCreate(rig).TDeckClerkCreate("Banks", null, LGrain.LGrainMeaning);
        LCueClerk clerk = TInterface.TCueClerkCreate(rig);
        LCue cue = clerk.TCueClerkAdd(deck.LDeckId, [bank.LEntryId])[0];

        LRefusal refusal = Assert.Throws<LRefusal>(() => clerk.TCueCellSave(
            cue.LCueId, TInterface.TCueFixtureCreate(LCueColumn.LCueColumnText, 1)));

        Assert.Equal(LRefusal.LRefusalBound, refusal.LRefusalReason);
    }

    [Fact]
    public void CueLineRead_MeaningCue_ReadsDefinition()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        LRig rig = workspace.TWorkspaceRigCreate();
        LEntry bank = TCueEntrySave(rig);
        LDeck deck = TInterface.TDeckClerkCreate(rig).TDeckClerkCreate("Banks", null, LGrain.LGrainMeaning);
        LCueClerk clerk = TInterface.TCueClerkCreate(rig);
        clerk.TCueClerkAdd(deck.LDeckId, [bank.LEntryId]);

        IReadOnlyList<(LCue LCueStored, string LCueLine)> lines =
            clerk.TCueLineRead(deck.LDeckId, TInterface.TPortraitLabelRead());

        Assert.Equal(["edge of a river", "place for money"], lines.Select(static line => line.LCueLine));
    }

    [Fact]
    public void CueLineRead_FreeCue_ReadsTextCell()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        LRig rig = workspace.TWorkspaceRigCreate();
        LDeck deck = TInterface.TDeckClerkCreate(rig).TDeckClerkCreate("Notes", null, LGrain.LGrainMeaning);
        LCueClerk clerk = TInterface.TCueClerkCreate(rig);
        clerk.TCueClerkCreate(
            deck.LDeckId,
            [
                TInterface.TCueCellCreate(LCueColumn.LCueColumnNotes, 1, "note"),
                TInterface.TCueCellCreate(LCueColumn.LCueColumnText, 2, "back"),
                TInterface.TCueCellCreate(LCueColumn.LCueColumnText, 1, "front"),
            ]);

        IReadOnlyList<(LCue LCueStored, string LCueLine)> lines =
            clerk.TCueLineRead(deck.LDeckId, TInterface.TPortraitLabelRead());

        Assert.Equal("front", Assert.Single(lines).LCueLine);
    }

    [Fact]
    public void CueLineRead_OrphanedCue_ReadsEmptyLine()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        LRig rig = workspace.TWorkspaceRigCreate();
        LDeck deck = TInterface.TDeckClerkCreate(rig).TDeckClerkCreate("Banks", null, LGrain.LGrainMeaning);
        LCue orphan = TInterface.TCueCreate(LGrain.LGrainMeaning, null, null, null, null, null)
            with { LCueDeckId = deck.LDeckId };
        rig.LRigRehearsal.LRigRehearsalCues.TCueCreate(deck.LDeckId, [orphan]);

        IReadOnlyList<(LCue LCueStored, string LCueLine)> lines =
            TInterface.TCueClerkCreate(rig).TCueLineRead(deck.LDeckId, TInterface.TPortraitLabelRead());

        (LCue LCueStored, string LCueLine) line = Assert.Single(lines);
        Assert.True(line.LCueStored.LCueOrphaned);
        Assert.Empty(line.LCueLine);
    }

    private static LCardDraft TCueCardCreate(long id, IReadOnlyList<long>? translation = null) =>
        TInterface.TCardDraftCreate(
            string.Empty, string.Empty, $"meaning {id}", [], [], translation ?? [], [], [], (int)id, id);

    private static LEntryDraft TCueDraftCreate(
        IReadOnlyList<LCardDraft> meanings, IReadOnlyList<LCardDraft> collocations) =>
        TInterface.TEntryDraftCreate("bank", "English", string.Empty, string.Empty, meanings, collocations);

    private static LEntry TCueEntrySave(LRig rig) =>
        TInterface.TEntryClerkCreate(rig).TEntryClerkSave(TInterface.TEntryDraftCreate(
            "bank",
            "English",
            string.Empty,
            string.Empty,
            [
                TInterface.TCardDraftCreate(string.Empty, string.Empty, "edge of a river", [], [], [], [], [], 1),
                TInterface.TCardDraftCreate(string.Empty, string.Empty, "place for money", [], [], [], [], [], 2),
            ],
            []));
}
