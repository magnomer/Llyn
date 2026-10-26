using System.Collections.Generic;
using System.Linq;
using Llyn.Application;
using Llyn.Core;
using Xunit;

namespace Llyn.Tests;

public sealed class TStencilClerk
{
    [Fact]
    public void StencilClerkRead_SavedAndOwned_ReadsSavedOnly()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        LRig rig = workspace.TWorkspaceRigCreate();
        LStencilVault stencils = rig.LRigRehearsal.LRigRehearsalStencils;
        TInterface.TDeckClerkCreate(rig).TDeckClerkCreate("Verbs", null, LGrain.LGrainMeaning);
        LStencil saved = stencils.TStencilCreate(TInterface.TStencilFixtureCreate("Headwords", saved: true));

        IReadOnlyList<LStencil> listed = TInterface.TStencilClerkCreate(rig).TStencilClerkRead();

        Assert.Equal(saved.LStencilId, Assert.Single(listed).LStencilId);
    }

    [Fact]
    public void StencilFieldAppend_TextSide_SavesSlotBody()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        LRig rig = workspace.TWorkspaceRigCreate();
        LDeck deck = TInterface.TDeckClerkCreate(rig).TDeckClerkCreate("Verbs", null, LGrain.LGrainMeaning);
        LStencilClerk clerk = TInterface.TStencilClerkCreate(rig);

        clerk.TStencilFieldAppend(deck.LDeckId, 2, LStencilField.LStencilFieldPronunciation);

        Assert.Contains(
            TInterface.TStencilSlotCreate(LCueColumn.LCueColumnText, 2, "{definition}\n{pronunciation}"),
            clerk.TStencilClerkRead(deck.LDeckId).LStencilSlots);
    }

    [Fact]
    public void StencilFieldRemove_OnlyField_ClearsSlot()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        LRig rig = workspace.TWorkspaceRigCreate();
        LDeck deck = TInterface.TDeckClerkCreate(rig).TDeckClerkCreate("Verbs", null, LGrain.LGrainMeaning);
        LStencilClerk clerk = TInterface.TStencilClerkCreate(rig);

        clerk.TStencilFieldRemove(deck.LDeckId, 1, 0);

        Assert.Contains(
            TInterface.TStencilSlotCreate(LCueColumn.LCueColumnText, 1, string.Empty),
            clerk.TStencilClerkRead(deck.LDeckId).LStencilSlots);
    }

    [Fact]
    public void StencilJoinSet_AllJoin_SavesSlotBody()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        LRig rig = workspace.TWorkspaceRigCreate();
        LDeck deck = TInterface.TDeckClerkCreate(rig).TDeckClerkCreate("Verbs", null, LGrain.LGrainMeaning);
        LStencilClerk clerk = TInterface.TStencilClerkCreate(rig);

        clerk.TStencilJoinSet(deck.LDeckId, 2, 0, all: true);

        Assert.Contains(
            TInterface.TStencilSlotCreate(LCueColumn.LCueColumnText, 2, "{definition:all}"),
            clerk.TStencilClerkRead(deck.LDeckId).LStencilSlots);
    }

    [Fact]
    public void StencilMediaSet_PictureTwo_MovesImageField()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        LRig rig = workspace.TWorkspaceRigCreate();
        LDeck deck = TInterface.TDeckClerkCreate(rig).TDeckClerkCreate("Verbs", null, LGrain.LGrainMeaning);
        LStencilClerk clerk = TInterface.TStencilClerkCreate(rig);
        LStencil saved = rig.LRigRehearsal.LRigRehearsalStencils.TStencilCreate(
            TInterface.TStencilSlotCreate(LCueColumn.LCueColumnPicture, "{image:all}") with { LStencilSaved = true });
        clerk.TStencilClerkApply(deck.LDeckId, saved.LStencilId);

        clerk.TStencilMediaSet(deck.LDeckId, LCueColumn.LCueColumnPicture, 2);

        Assert.Equal(
            TInterface.TStencilSlotCreate(LCueColumn.LCueColumnPicture, 2, "{image:all}"),
            Assert.Single(clerk.TStencilClerkRead(deck.LDeckId).LStencilSlots));
    }

    [Theory]
    [InlineData("{definition:all}", "edge of a river|place for money")]
    [InlineData("Sense: {definition:2}", "Sense: place for money")]
    public void StencilClerkResolve_DefinitionToken_FillsCell(string body, string expected)
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        LRig rig = workspace.TWorkspaceRigCreate();
        LEntry bank = TInterface.TEntryClerkCreate(rig).TEntryClerkSave(TInterface.TEntryDraftCreate(
            "bank",
            "English",
            string.Empty,
            string.Empty,
            [
                TInterface.TCardDraftCreate(string.Empty, string.Empty, "edge of a river", [], [], [], [], [], 1),
                TInterface.TCardDraftCreate(string.Empty, string.Empty, "place for money", [], [], [], [], [], 2),
            ],
            []));
        LCue cue = TInterface.TCueCreate(LGrain.LGrainEntry, bank.LEntryId, null, null, null, null);

        LCueFace face = TInterface.TStencilClerkCreate(rig).TStencilClerkResolve(
            cue,
            TInterface.TStencilSlotCreate(LCueColumn.LCueColumnText, body),
            TInterface.TPortraitLabelRead());

        Assert.Equal(expected, Assert.Single(face.LCueFaceCells).LCueCellValue);
    }

    [Fact]
    public void StencilClerkResolve_ImageOnMeaning_FillsPicture()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        LRig rig = workspace.TWorkspaceRigCreate();
        LImageDraft image = TInterface.TImageDraftCreate("cat.png");
        LEntry cat = TInterface.TEntryClerkCreate(rig).TEntryClerkSave(TInterface.TEntryDraftCreate(
            "cat",
            "English",
            string.Empty,
            string.Empty,
            [TInterface.TCardDraftCreate(string.Empty, string.Empty, "small pet", [], [], [], [], [image], 1)],
            []));
        long meaningId = rig.LRigEntries.TEntryLoad(cat.LEntryId)!.LEntryDraftMeanings[0].LCardDraftId;
        LCue cue = TInterface.TCueCreate(LGrain.LGrainMeaning, null, meaningId, null, null, null);

        LCueFace face = TInterface.TStencilClerkCreate(rig).TStencilClerkResolve(
            cue,
            TInterface.TStencilSlotCreate(LCueColumn.LCueColumnPicture, "{image}"),
            TInterface.TPortraitLabelRead());

        LCueCell picture = Assert.Single(face.LCueFaceCells);
        Assert.Equal(LCueColumn.LCueColumnPicture, picture.LCueCellColumn);
        Assert.EndsWith("cat.png", picture.LCueCellValue);
        Assert.True(Path.IsPathRooted(picture.LCueCellValue));
    }

    [Fact]
    public void StencilClerkResolve_RecordedEntry_FillsSound()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        LRig rig = workspace.TWorkspaceRigCreate();
        LEntry water = TInterface.TEntryClerkCreate(rig).TEntryClerkSave(TInterface.TEntryDraftCreate(
            "water", "English", "ˈwɔːtər", string.Empty, [], [], audio: "water.mp3"));
        LCue cue = TInterface.TCueCreate(LGrain.LGrainEntry, water.LEntryId, null, null, null, null);

        LCueFace face = TInterface.TStencilClerkCreate(rig).TStencilClerkResolve(
            cue,
            TInterface.TStencilSlotCreate(LCueColumn.LCueColumnSound, "{recording}"),
            TInterface.TPortraitLabelRead());

        LCueCell sound = Assert.Single(face.LCueFaceCells);
        Assert.EndsWith("water.mp3", sound.LCueCellValue);
        Assert.True(Path.IsPathRooted(sound.LCueCellValue));
    }

    [Fact]
    public void StencilClerkResolve_PairGrain_WritesTarget()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        LRig rig = workspace.TWorkspaceRigCreate();
        LEntryClerk entries = TInterface.TEntryClerkCreate(rig);
        LEntry wasser = entries.TEntryClerkSave(
            TInterface.TEntryDraftCreate("Wasser", "German", string.Empty, string.Empty, [], []));
        long target = wasser.LEntryId;
        LEntry water = entries.TEntryClerkSave(TInterface.TEntryDraftCreate(
            "water",
            "English",
            string.Empty,
            string.Empty,
            [TInterface.TCardDraftCreate(string.Empty, string.Empty, "clear liquid", [], [], [target], [], [], 1)],
            []));
        long meaningId = rig.LRigEntries.TEntryLoad(water.LEntryId)!.LEntryDraftMeanings[0].LCardDraftId;
        LCue cue = TInterface.TCueCreate(LGrain.LGrainPair, null, meaningId, null, null, wasser.LEntryId);

        LCueFace face = TInterface.TStencilClerkCreate(rig).TStencilClerkResolve(
            cue,
            TInterface.TStencilSlotCreate(LCueColumn.LCueColumnText, "{target}"),
            TInterface.TPortraitLabelRead());

        Assert.Equal("Wasser", Assert.Single(face.LCueFaceCells).LCueCellValue);
    }

    [Fact]
    public void StencilClerkResolve_LinkRemoved_LeavesTargetEmpty()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        LRig rig = workspace.TWorkspaceRigCreate();
        LEntryClerk entries = TInterface.TEntryClerkCreate(rig);
        LEntry wasser = entries.TEntryClerkSave(
            TInterface.TEntryDraftCreate("Wasser", "German", string.Empty, string.Empty, [], []));
        LEntry water = entries.TEntryClerkSave(TInterface.TEntryDraftCreate(
            "water",
            "English",
            string.Empty,
            string.Empty,
            [TInterface.TCardDraftCreate(string.Empty, string.Empty, "clear liquid", [], [], [], [], [], 1)],
            []));
        long meaningId = rig.LRigEntries.TEntryLoad(water.LEntryId)!.LEntryDraftMeanings[0].LCardDraftId;
        LCue cue = TInterface.TCueCreate(LGrain.LGrainPair, null, meaningId, null, null, wasser.LEntryId);

        LCueFace face = TInterface.TStencilClerkCreate(rig).TStencilClerkResolve(
            cue,
            TInterface.TStencilSlotCreate(LCueColumn.LCueColumnText, "{target}"),
            TInterface.TPortraitLabelRead());

        Assert.Equal(string.Empty, Assert.Single(face.LCueFaceCells).LCueCellValue);
        Assert.False(cue.LCueOrphaned);
    }

    [Fact]
    public void StencilClerkResolve_VideoOnly_WritesNoPicture()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        LRig rig = workspace.TWorkspaceRigCreate();
        LEntry run = TInterface.TEntryClerkCreate(rig).TEntryClerkSave(TInterface.TEntryDraftCreate(
            "run",
            "English",
            string.Empty,
            string.Empty,
            [
                TInterface.TCardDraftCreate(
                    string.Empty,
                    string.Empty,
                    "move fast",
                    [],
                    [],
                    [],
                    [],
                    [],
                    1,
                    video: [TInterface.TVideoDraftCreate("run.mp4")]),
            ],
            []));
        long meaningId = rig.LRigEntries.TEntryLoad(run.LEntryId)!.LEntryDraftMeanings[0].LCardDraftId;
        LCue cue = TInterface.TCueCreate(LGrain.LGrainMeaning, null, meaningId, null, null, null);

        LCueFace face = TInterface.TStencilClerkCreate(rig).TStencilClerkResolve(
            cue,
            TInterface.TStencilSlotCreate(LCueColumn.LCueColumnPicture, "{image:all}"),
            TInterface.TPortraitLabelRead());

        Assert.Equal(string.Empty, Assert.Single(face.LCueFaceCells).LCueCellValue);
    }

    [Fact]
    public void StencilClerkResolve_MeaningCue_FillsDefinition()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        LRig rig = workspace.TWorkspaceRigCreate();
        LEntry fright = TInterface.TEntryClerkCreate(rig).TEntryClerkSave(TInterface.TEntryDraftCreate(
            "fright",
            "English",
            string.Empty,
            string.Empty,
            [TInterface.TCardDraftCreate(string.Empty, string.Empty, "sudden fear", [], [], [], [], [], 1)],
            []));

        IReadOnlyList<string> sides = TStencilFaceRead(rig, fright.LEntryId, LGrain.LGrainMeaning);

        Assert.Equal(["fright", "sudden fear"], sides);
    }

    [Fact]
    public void StencilClerkResolve_CollocationCue_FillsParaphrase()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        LRig rig = workspace.TWorkspaceRigCreate();
        LEntry decision = TInterface.TEntryClerkCreate(rig).TEntryClerkSave(TInterface.TEntryDraftCreate(
            "decision",
            "English",
            string.Empty,
            string.Empty,
            [],
            [TInterface.TCardDraftCreate(string.Empty, "make a decision", "decide", [], [], [], [], [], 1)]));

        IReadOnlyList<string> sides = TStencilFaceRead(rig, decision.LEntryId, LGrain.LGrainCollocation);

        Assert.Equal(["make a decision", "decide"], sides);
    }

    [Fact]
    public void StencilClerkResolve_ExampleCue_FillsGloss()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        LRig rig = workspace.TWorkspaceRigCreate();
        LGlossDraft gloss = TInterface.TGlossDraftCreate(0, "German", TInterfaceState.TStateValueCreate("Sie sprang."));
        LExampleDraft example = TInterfaceExample.TExampleDraftCreate("She jumped.") with
        {
            LExampleDraftGloss = [gloss],
        };
        LEntry jump = TInterface.TEntryClerkCreate(rig).TEntryClerkSave(TInterface.TEntryDraftCreate(
            "jump",
            "English",
            string.Empty,
            string.Empty,
            [
                TInterface.TCardDraftCreate(
                    string.Empty, string.Empty, "leap", [TInterface.TSentenceDraftCreate(example)], [], [], [], [], 1),
            ],
            []));

        IReadOnlyList<string> sides = TStencilFaceRead(rig, jump.LEntryId, LGrain.LGrainExample);

        Assert.Equal(["She jumped.", "Sie sprang."], sides);
    }

    [Fact]
    public void StencilClerkResolve_PairCue_FillsTarget()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        LRig rig = workspace.TWorkspaceRigCreate();
        LEntryClerk entries = TInterface.TEntryClerkCreate(rig);
        LEntry wasser = entries.TEntryClerkSave(
            TInterface.TEntryDraftCreate("Wasser", "German", string.Empty, string.Empty, [], []));
        LEntry water = entries.TEntryClerkSave(TInterface.TEntryDraftCreate(
            "water",
            "English",
            string.Empty,
            string.Empty,
            [
                TInterface.TCardDraftCreate(
                    string.Empty, string.Empty, "clear liquid", [], [], [wasser.LEntryId], [], [], 1),
            ],
            []));

        IReadOnlyList<string> sides = TStencilFaceRead(rig, water.LEntryId, LGrain.LGrainPair);

        Assert.Equal(["water", "Wasser"], sides);
    }

    private static IReadOnlyList<string> TStencilFaceRead(LRig rig, long entryId, LGrain grain)
    {
        LDeck deck = TInterface.TDeckClerkCreate(rig).TDeckClerkCreate("Deck", null, grain);
        TInterface.TCueClerkCreate(rig).TCueClerkAdd(deck.LDeckId, [entryId]);
        LCue cue = Assert.Single(rig.LRigRehearsal.LRigRehearsalCues.TCueRead(deck.LDeckId));
        LStencil stencil = rig.LRigRehearsal.LRigRehearsalStencils.TStencilRead(deck.LDeckStencilId)!;

        LCueFace face = TInterface.TStencilClerkCreate(rig).TStencilClerkResolve(
            cue, stencil, TInterface.TPortraitLabelRead());

        return face.LCueFaceCells
            .Where(static cell => cell.LCueCellColumn == LCueColumn.LCueColumnText)
            .OrderBy(static cell => cell.LCueCellIndex)
            .Select(static cell => cell.LCueCellValue)
            .ToList();
    }
}
