using System.Collections.Generic;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TDraftReference
{
    [Fact]
    public void ReferenceSave_HeldReference_SurvivesScan()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();

        long held;

        using (LEngine engine = workspace.TWorkspaceEngineStart())
        {
            LDraft started = engine.TEngineReferenceStart("Reference", null);
            held = started.LDraftId;

            engine.TEngineReferenceSave(started with
            {
                LDraftReference = TDraftReferenceCreate(started, "the evening news"),
            });

            Assert.True(engine.TEngineDraftCheck(held));
        }

        using LEngine launched = workspace.TWorkspaceEngineStart();

        IReadOnlyList<LDraft> drafts = launched.TEngineDraftScan();
        LDraft? found = null;
        foreach (LDraft draft in drafts)
        {
            if (draft.LDraftId == held)
            {
                found = draft;
            }
        }

        Assert.NotNull(found);
        Assert.NotNull(found.LDraftReference);
        Assert.Equal("the evening news", found.LDraftReference.LReferenceTitle.TStateValueShow());
        Assert.Contains(launched.TEngineLeftoverRead(), draft => draft.LDraftId == held);
    }

    [Fact]
    public void ReferenceCommit_HeldReference_LeavesNothingBehind()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LDraft started = engine.TEngineReferenceStart("Reference", null);

        engine.TEngineReferenceSave(started with
        {
            LDraftReference = TDraftReferenceCreate(started, "the evening news"),
        });

        LReference stored = engine.TEngineReferenceCommit(started.LDraftId);

        Assert.Equal("the evening news", stored.LReferenceTitle.TStateValueShow());
        Assert.Null(engine.TEngineDraftRead(started.LDraftId));
        Assert.Empty(engine.TEngineDraftScan());
        Assert.Empty(engine.TEngineLeftoverRead());

        LReference? written = engine.TEngineReferenceRead(stored.LReferenceId);

        Assert.NotNull(written);
        Assert.Equal("the evening news", written.LReferenceTitle.TStateValueShow());
    }

    [Fact]
    public void LeftoverSweep_ReferenceMatchingStoredReference_SweepsIt()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();

        long swept;
        long kept;

        using (LEngine engine = workspace.TWorkspaceEngineStart())
        {
            LDraft committed = engine.TEngineReferenceStart("Reference", null);
            LDraft edited = engine.TEngineReferenceStart("Reference", null);

            swept = committed.LDraftId;
            kept = edited.LDraftId;

            LReference reference = TDraftReferenceCreate(committed, "the evening news");
            engine.TEngineReferenceSave(committed with { LDraftReference = reference });

            LReference stored = engine.TEngineReferenceCreate(reference);

            engine.TEngineReferenceSave(committed with
            {
                LDraftEntry = stored.LReferenceId,
                LDraftReference = stored,
            });
            engine.TEngineReferenceSave(edited with
            {
                LDraftReference = TDraftReferenceCreate(edited, "the morning paper"),
            });
        }

        using LEngine launched = workspace.TWorkspaceEngineStart();

        launched.TEngineLeftoverSweep();

        Assert.Null(launched.TEngineDraftRead(swept));
        Assert.NotNull(launched.TEngineDraftRead(kept));
        Assert.Contains(launched.TEngineLeftoverRead(), draft => draft.LDraftId == kept);
    }

    private static LReference TDraftReferenceCreate(LDraft draft, string title)
    {
        Assert.NotNull(draft.LDraftReference);

        return draft.LDraftReference with
        {
            LReferenceTitle = TInterface.TStateValueCreate(title),
        };
    }
}
