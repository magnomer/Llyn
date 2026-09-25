using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TChronicle
{
    [Fact]
    public void RequestApply_ThenUndo_RestoresHeldDraft()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LDraft started = engine.TEngineDraftStart("editor", null);
        engine.TEngineRequestApply(TInterface.TRequestHeadwordCreate(started.LDraftId, "ember"));

        LDraft? restored = engine.TEngineChronicleUndo(started.LDraftId);

        Assert.NotNull(restored);
        Assert.Equal(string.Empty, restored.LDraftContent.LEntryDraftHeadword);
        Assert.Equal(string.Empty, engine.TEngineDraftRead(started.LDraftId)!.LDraftContent.LEntryDraftHeadword);
        Assert.False(engine.TEngineUndoCheck(started.LDraftId));
        Assert.True(engine.TEngineRedoCheck(started.LDraftId));
    }

    [Fact]
    public void Undo_ThenRedo_ReturnsToLatest()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LDraft started = engine.TEngineDraftStart("editor", null);
        LDraft latest = engine.TEngineRequestApply(TInterface.TRequestHeadwordCreate(started.LDraftId, "ember"));

        engine.TEngineChronicleUndo(started.LDraftId);
        LDraft? redone = engine.TEngineChronicleRedo(started.LDraftId);

        Assert.NotNull(redone);
        Assert.Equal(latest.LDraftContent.LEntryDraftHeadword, redone.LDraftContent.LEntryDraftHeadword);
        Assert.Equal("ember", engine.TEngineDraftRead(started.LDraftId)!.LDraftContent.LEntryDraftHeadword);
        Assert.True(engine.TEngineUndoCheck(started.LDraftId));
        Assert.False(engine.TEngineRedoCheck(started.LDraftId));
    }

    [Fact]
    public void RequestApply_AfterUndo_ClearsFuture()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LDraft started = engine.TEngineDraftStart("editor", null);
        engine.TEngineRequestApply(TInterface.TRequestHeadwordCreate(started.LDraftId, "ember"));
        engine.TEngineChronicleUndo(started.LDraftId);

        engine.TEngineRequestApply(TInterface.TRequestHeadwordCreate(started.LDraftId, "hearth"));

        Assert.False(engine.TEngineRedoCheck(started.LDraftId));
        Assert.Null(engine.TEngineChronicleRedo(started.LDraftId));
        Assert.Equal("hearth", engine.TEngineDraftRead(started.LDraftId)!.LDraftContent.LEntryDraftHeadword);
    }

    [Fact]
    public void RequestApply_NoChange_RecordsNothing()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LDraft started = engine.TEngineDraftStart("editor", null);
        engine.TEngineRequestApply(TInterface.TRequestHeadwordCreate(started.LDraftId, "ember"));
        engine.TEngineRequestApply(TInterface.TRequestHeadwordCreate(started.LDraftId, "ember"));

        LDraft? restored = engine.TEngineChronicleUndo(started.LDraftId);

        Assert.NotNull(restored);
        Assert.Equal(string.Empty, restored.LDraftContent.LEntryDraftHeadword);
        Assert.False(engine.TEngineUndoCheck(started.LDraftId));
    }

    [Fact]
    public void RequestApply_Scaffolding_RecordsNothing()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LDraft started = engine.TEngineDraftStart("editor", null);
        engine.TEngineRequestApply(TInterface.TRequestLanguageCreate(started.LDraftId, "English"));
        LDraft carded = engine.TEngineRequestApply(
            TInterface.TRequestAdditionCreate(started.LDraftId, LCardKind.LCardKindMeaning, 0, 0));
        engine.TEngineRequestApply(
            TInterface.TRequestAdditionCreate(started.LDraftId, LCardKind.LCardKindCollocation, 0, 0));
        engine.TEngineRequestApply(TInterface.TSentenceAdditionCreate(
            started.LDraftId, carded.LDraftContent.LEntryDraftMeanings[0].LCardDraftId, 0));

        Assert.False(engine.TEngineUndoCheck(started.LDraftId));

        engine.TEngineRequestApply(TInterface.TRequestHeadwordCreate(started.LDraftId, "ember"));
        engine.TEngineRequestApply(
            TInterface.TRequestAdditionCreate(started.LDraftId, LCardKind.LCardKindMeaning, 0, 1));

        LDraft? restored = engine.TEngineChronicleUndo(started.LDraftId);

        Assert.NotNull(restored);
        Assert.Equal(string.Empty, restored.LDraftContent.LEntryDraftHeadword);
        Assert.False(engine.TEngineUndoCheck(started.LDraftId));
    }

    [Fact]
    public void RequestApply_SameTypeWithinWindow_MergesStep()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        DateTimeOffset moment = new(2026, 1, 1, 0, 0, 0, TimeSpan.Zero);
        workspace.TWorkspaceClockSet(() => moment);

        LDraft started = engine.TEngineDraftStart("editor", null);
        engine.TEngineRequestApply(TInterface.TRequestHeadwordCreate(started.LDraftId, "em"));
        moment += TimeSpan.FromMilliseconds(1000);
        engine.TEngineRequestApply(TInterface.TRequestHeadwordCreate(started.LDraftId, "ember"));
        moment += TimeSpan.FromMilliseconds(2000);
        engine.TEngineRequestApply(TInterface.TRequestHeadwordCreate(started.LDraftId, "embers"));

        LDraft? restored = engine.TEngineChronicleUndo(started.LDraftId);

        Assert.NotNull(restored);
        Assert.Equal("ember", restored.LDraftContent.LEntryDraftHeadword);
        restored = engine.TEngineChronicleUndo(started.LDraftId);
        Assert.NotNull(restored);
        Assert.Equal(string.Empty, restored.LDraftContent.LEntryDraftHeadword);
        Assert.False(engine.TEngineUndoCheck(started.LDraftId));
    }

    [Fact]
    public void RequestApply_OtherType_StartsStep()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        DateTimeOffset moment = new(2026, 1, 1, 0, 0, 0, TimeSpan.Zero);
        workspace.TWorkspaceClockSet(() => moment);

        LDraft started = engine.TEngineDraftStart("editor", null);
        engine.TEngineRequestApply(TInterface.TRequestHeadwordCreate(started.LDraftId, "ember"));
        engine.TEngineRequestApply(TInterface.TRequestNoteCreate(started.LDraftId, "glowing"));

        LDraft? restored = engine.TEngineChronicleUndo(started.LDraftId);

        Assert.NotNull(restored);
        Assert.Equal("ember", restored.LDraftContent.LEntryDraftHeadword);
        Assert.True(engine.TEngineUndoCheck(started.LDraftId));
    }

    [Fact]
    public void RequestApply_AfterUndo_StartsStep()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        DateTimeOffset moment = new(2026, 1, 1, 0, 0, 0, TimeSpan.Zero);
        workspace.TWorkspaceClockSet(() => moment);

        LDraft started = engine.TEngineDraftStart("editor", null);
        engine.TEngineRequestApply(TInterface.TRequestHeadwordCreate(started.LDraftId, "ember"));
        engine.TEngineChronicleUndo(started.LDraftId);
        engine.TEngineChronicleRedo(started.LDraftId);
        engine.TEngineRequestApply(TInterface.TRequestHeadwordCreate(started.LDraftId, "hearth"));

        LDraft? restored = engine.TEngineChronicleUndo(started.LDraftId);

        Assert.NotNull(restored);
        Assert.Equal("ember", restored.LDraftContent.LEntryDraftHeadword);
    }

    [Fact]
    public void CardShift_ThenUndo_RestoresOrder()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LDraft started = engine.TEngineDraftStart("editor", null);
        LDraft written = engine.TRequestContentApply(
            started.LDraftId,
            TInterface.TDraftPlainCreate("kindle") with
            {
                LEntryDraftMeanings =
                [
                    TInterface.TDraftCardCreate("first"),
                    TInterface.TDraftCardCreate("second"),
                    TInterface.TDraftCardCreate("third"),
                ],
            });

        engine.TEngineRequestApply(TInterface.TRequestShiftCreate(
            started.LDraftId, written.LDraftContent.LEntryDraftMeanings[0].LCardDraftId, 0, 2));
        LDraft? restored = engine.TEngineChronicleUndo(started.LDraftId);

        Assert.NotNull(restored);
        Assert.Equal(
            ["first", "second", "third"],
            restored.LDraftContent.LEntryDraftMeanings.Select(card => card.LCardDraftTitle.TStateValueShow()));
        LDraft? held = engine.TEngineDraftRead(started.LDraftId);

        Assert.NotNull(held);
        Assert.Equal(
            written.LDraftContent.LEntryDraftMeanings.Select(card => card.LCardDraftId),
            held.LDraftContent.LEntryDraftMeanings.Select(card => card.LCardDraftId));
    }

    [Fact]
    public void DraftCommit_ClearsChronicle()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LDraft started = engine.TEngineDraftStart("editor", null);
        engine.TRequestContentApply(started.LDraftId, TInterface.TDraftPlainCreate("kindle"));

        Assert.True(engine.TEngineUndoCheck(started.LDraftId));

        engine.TEngineDraftCommit(started.LDraftId);

        Assert.False(engine.TEngineUndoCheck(started.LDraftId));
        Assert.False(engine.TEngineRedoCheck(started.LDraftId));
    }

    [Fact]
    public void Undo_EmptyPast_ReturnsNull()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LDraft started = engine.TEngineDraftStart("editor", null);

        Assert.Null(engine.TEngineChronicleUndo(started.LDraftId));
        Assert.Null(engine.TEngineChronicleRedo(started.LDraftId));
        Assert.NotNull(engine.TEngineDraftRead(started.LDraftId));
    }

    [Fact]
    public void Record_PastCap_DropsOldest()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        DateTimeOffset moment = new(2026, 1, 1, 0, 0, 0, TimeSpan.Zero);
        workspace.TWorkspaceClockSet(() => moment);

        LDraft started = engine.TEngineDraftStart("editor", null);
        for (int step = 1; step <= 101; step++)
        {
            moment += TimeSpan.FromSeconds(2);
            engine.TEngineRequestApply(TInterface.TRequestHeadwordCreate(started.LDraftId, $"word{step}"));
        }

        LDraft? oldest = null;
        int walked = 0;
        while (engine.TEngineChronicleUndo(started.LDraftId) is LDraft restored)
        {
            oldest = restored;
            walked++;
        }

        Assert.Equal(100, walked);
        Assert.NotNull(oldest);
        Assert.Equal("word1", oldest.LDraftContent.LEntryDraftHeadword);
    }
}
