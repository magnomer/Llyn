using System.Collections.Generic;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TDraftSituation
{
    [Fact]
    public void SituationSave_HeldSituation_SurvivesScan()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();

        long held;

        using (LEngine engine = workspace.TWorkspaceEngineStart())
        {
            LDraft started = engine.TEngineSituationStart("Repertoire", null);
            held = started.LDraftId;

            engine.TEngineSituationSave(started with
            {
                LDraftSituation = TDraftSituationCreate(started, "asking the way at a station"),
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
        Assert.NotNull(found.LDraftSituation);
        Assert.Equal(
            "asking the way at a station",
            found.LDraftSituation.LSituationTitle.TStateValueShow());
        Assert.Contains(launched.TEngineLeftoverRead(), draft => draft.LDraftId == held);
    }

    [Fact]
    public void SituationCommit_HeldSituation_LeavesNothingBehind()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LDraft started = engine.TEngineSituationStart("Repertoire", null);

        engine.TEngineSituationSave(started with
        {
            LDraftSituation = TDraftSituationCreate(started, "asking the way at a station"),
        });

        LSituation stored = engine.TEngineSituationCommit(started.LDraftId);

        Assert.Equal("asking the way at a station", stored.LSituationTitle.TStateValueShow());
        Assert.Null(engine.TEngineDraftRead(started.LDraftId));
        Assert.Empty(engine.TEngineDraftScan());
        Assert.Empty(engine.TEngineLeftoverRead());

        LSituation? written = engine.TEngineSituationRead(stored.LSituationId);

        Assert.NotNull(written);
        Assert.Equal("asking the way at a station", written.LSituationTitle.TStateValueShow());
    }

    [Fact]
    public void LeftoverSweep_SituationMatchingStoredSituation_SweepsIt()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();

        long swept;
        long kept;

        using (LEngine engine = workspace.TWorkspaceEngineStart())
        {
            LDraft committed = engine.TEngineSituationStart("Repertoire", null);
            LDraft edited = engine.TEngineSituationStart("Repertoire", null);

            swept = committed.LDraftId;
            kept = edited.LDraftId;

            LSituation situation = TDraftSituationCreate(committed, "asking the way at a station");
            engine.TEngineSituationSave(committed with { LDraftSituation = situation });

            LSituation stored = engine.TEngineSituationCreate(situation);

            engine.TEngineSituationSave(committed with
            {
                LDraftEntryId = stored.LSituationId,
                LDraftSituation = stored,
            });
            engine.TEngineSituationSave(edited with
            {
                LDraftSituation = TDraftSituationCreate(edited, "ordering at a counter"),
            });
        }

        using LEngine launched = workspace.TWorkspaceEngineStart();

        launched.TEngineLeftoverSweep();

        Assert.Null(launched.TEngineDraftRead(swept));
        Assert.NotNull(launched.TEngineDraftRead(kept));
        Assert.Contains(launched.TEngineLeftoverRead(), draft => draft.LDraftId == kept);
    }

    private static LSituation TDraftSituationCreate(LDraft draft, string title)
    {
        Assert.NotNull(draft.LDraftSituation);

        return draft.LDraftSituation with
        {
            LSituationTitle = TInterface.TStateValueCreate(title),
        };
    }
}
