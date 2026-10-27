using System;
using System.Collections.Generic;
using System.Linq;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TAncestorSaving
{
    [Fact]
    public void AncestorUpdate_EditedMap_StoresTree()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        long source = TAncestor.TAncestorEntryCreate(engine, "rinnan");
        long entryId = TAncestor.TAncestorEntryCreate(engine, "run");
        LDraft held = engine.TEngineDraftStart("Input", entryId);
        (long root, long first, long second) = TAncestor.TAncestorTreeCreate(engine, held.LDraftId);
        engine.TEngineRequestApply(TInterface.TAncestorTextCreate(held.LDraftId, root, " rennen "));
        engine.TEngineRequestApply(TInterface.TAncestorTargetCreate(held.LDraftId, first, source));
        engine.TEngineRequestApply(TInterface.TAncestorTextCreate(held.LDraftId, second, "iernan"));

        engine.TEngineDraftCommit(held.LDraftId);

        IReadOnlyList<LAncestorDraft> map = TAncestorStoredRead(engine, entryId);
        Assert.Equal(["rennen", "rinnan", "iernan"], map.Select(static box => box.LAncestorDraftText));
        Assert.Equal(source, map[1].LAncestorDraftTarget);
        Assert.All(map.Skip(1), box => Assert.Equal(map[0].LAncestorDraftId, box.LAncestorDraftChild));
    }

    [Fact]
    public void AncestorUpdate_NewEntry_StoresMap()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LAncestorDraft box = TInterface.TAncestorDraftCreate(
            -1, null, null, "rennen", "Middle English", null, "to run");
        LEntryDraft draft = TInterface.TEntryDraftCreate("run", "English", string.Empty, string.Empty, [], []) with
        {
            LEntryDraftAncestors = [box],
        };

        long entryId = engine.TEngineEntrySave(draft).LEntryId;

        LAncestorDraft stored = Assert.Single(TAncestorStoredRead(engine, entryId));
        Assert.Equal(("rennen", "Middle English"), (stored.LAncestorDraftText, stored.LAncestorDraftLanguage));
    }

    [Fact]
    public void AncestorUpdate_FirstMap_RecordsCreate()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        long entryId = TAncestor.TAncestorEntryCreate(engine, "run");
        LDraft held = engine.TEngineDraftStart("Input", entryId);
        TAncestor.TAncestorTreeCreate(engine, held.LDraftId);
        engine.TEngineRequestApply(TInterface.TAncestorTextCreate(
            held.LDraftId, TAncestor.TAncestorHeldRead(engine, held.LDraftId)[0].LAncestorDraftId, "rennen"));

        engine.TEngineDraftCommit(held.LDraftId);

        LRevisionDelta change = TAncestorChangeRead(workspace, engine);
        Assert.Equal(("create", "rennen"), (change.LRevisionDeltaKind, change.LRevisionDeltaSummary));
    }

    [Fact]
    public void AncestorUpdate_ChangedMap_RecordsUpdate()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        long entryId = TAncestor.TAncestorEntryCreate(engine, "run");
        LDraft held = engine.TEngineDraftStart("Input", entryId);
        TAncestor.TAncestorTreeCreate(engine, held.LDraftId);
        engine.TEngineDraftCommit(held.LDraftId);
        LDraft again = engine.TEngineDraftStart("Input", entryId);

        engine.TEngineRequestApply(TInterface.TAncestorGlossCreate(
            again.LDraftId, TAncestor.TAncestorHeldRead(engine, again.LDraftId)[0].LAncestorDraftId, "to run"));
        engine.TEngineDraftCommit(again.LDraftId);

        Assert.Equal("update", TAncestorChangeRead(workspace, engine).LRevisionDeltaKind);
    }

    [Fact]
    public void AncestorUpdate_ClearedMap_RecordsDelete()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        long entryId = TAncestor.TAncestorEntryCreate(engine, "run");
        LDraft held = engine.TEngineDraftStart("Input", entryId);
        engine.TEngineRequestApply(TInterface.TAncestorInsertionCreate(held.LDraftId, 0));
        engine.TEngineDraftCommit(held.LDraftId);
        LDraft again = engine.TEngineDraftStart("Input", entryId);

        engine.TEngineRequestApply(TInterface.TAncestorRemovalCreate(
            again.LDraftId, TAncestor.TAncestorHeldRead(engine, again.LDraftId)[0].LAncestorDraftId));
        engine.TEngineDraftCommit(again.LDraftId);

        Assert.Equal("delete", TAncestorChangeRead(workspace, engine).LRevisionDeltaKind);
        Assert.Empty(TAncestorStoredRead(engine, entryId));
    }

    [Fact]
    public void AncestorUpdate_SameShapeNewKeys_RecordsNoRevision()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        long entryId = TAncestor.TAncestorEntryCreate(engine, "run");
        LDraft held = engine.TEngineDraftStart("Input", entryId);
        (long root, _, _) = TAncestor.TAncestorTreeCreate(engine, held.LDraftId);
        engine.TEngineRequestApply(TInterface.TAncestorTextCreate(held.LDraftId, root, "rennen"));
        engine.TEngineDraftCommit(held.LDraftId);
        long? before = engine.TEngineRevisionRead();
        LDraft again = engine.TEngineDraftStart("Input", entryId);
        long stored = TAncestor.TAncestorHeldRead(engine, again.LDraftId)[0].LAncestorDraftId;

        engine.TEngineRequestApply(TInterface.TAncestorInsertionCreate(again.LDraftId, 0));
        engine.TEngineRequestApply(TInterface.TAncestorRemovalCreate(again.LDraftId, stored));
        engine.TEngineRequestApply(TInterface.TAncestorTextCreate(
            again.LDraftId, TAncestor.TAncestorHeldRead(engine, again.LDraftId)[0].LAncestorDraftId, "rennen"));
        engine.TEngineDraftCommit(again.LDraftId);

        Assert.Equal(before, engine.TEngineRevisionRead());
    }

    private static LRevisionDelta TAncestorChangeRead(TWorkspace workspace, LEngine engine)
    {
        long revision = Assert.IsType<long>(engine.TEngineRevisionRead());
        return Assert.Single(
            workspace.TRevisionChangeRead(revision),
            static row => string.Equals(row.LRevisionDeltaSubject, "ancestor", StringComparison.Ordinal));
    }

    private static IReadOnlyList<LAncestorDraft> TAncestorStoredRead(LEngine engine, long entryId)
    {
        return Assert.IsType<LEntryDraft>(engine.TEngineEntryLoad(entryId)).LEntryDraftAncestors;
    }
}
