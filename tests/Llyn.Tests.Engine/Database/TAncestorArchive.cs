using System;
using System.Collections.Generic;
using System.Linq;
using Llyn.Core;
using Llyn.Infrastructure;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TAncestorArchive
{
    [Fact]
    public void AncestorSave_UnsavedKeys_ReadsTreeBackLinked()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        long source = TAncestorEntryCreate(engine, "rinnan");
        long entryId = TAncestorEntryCreate(engine, "run");
        LAncestorArchive archive = TInterface.TAncestorArchiveCreate(workspace.TWorkspaceDatabase);

        archive.TAncestorSave(entryId, TInterface.TAncestorDraftResolve(TAncestorMapCreate(source)));

        IReadOnlyList<LAncestor> stored = archive.TAncestorRead(entryId);
        Assert.Equal(3, stored.Count);
        LAncestor root = stored[0];
        Assert.Null(root.LAncestorChildId);
        Assert.Equal("Middle English", root.LAncestorLanguage);
        Assert.Equal("to run", root.LAncestorGloss);
        Assert.Equal([root.LAncestorId, root.LAncestorId], stored.Skip(1).Select(static box => box.LAncestorChildId));
        Assert.Equal([0, 1], stored.Skip(1).Select(static box => box.LAncestorPosition));
        Assert.Equal(source, stored[1].LAncestorTargetId);
        Assert.Equal("iernan", stored[2].LAncestorText);
    }

    [Fact]
    public void AncestorSave_EmptyList_ClearsMap()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        long source = TAncestorEntryCreate(engine, "rinnan");
        long entryId = TAncestorEntryCreate(engine, "run");
        LAncestorArchive archive = TInterface.TAncestorArchiveCreate(workspace.TWorkspaceDatabase);
        archive.TAncestorSave(entryId, TInterface.TAncestorDraftResolve(TAncestorMapCreate(source)));

        Assert.Empty(archive.TAncestorSave(entryId, []));

        Assert.Empty(archive.TAncestorRead(entryId));
    }

    [Fact]
    public void AncestorSave_MissingChildOrRing_Throws()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        long entryId = TAncestorEntryCreate(engine, "run");
        LAncestorArchive archive = TInterface.TAncestorArchiveCreate(workspace.TWorkspaceDatabase);
        LAncestorDraft orphan = TInterface.TAncestorDraftCreate(-1, -9, null, "rinnan");
        LAncestorDraft ringStart = TInterface.TAncestorDraftCreate(-1, -2, null, "rinnan");
        LAncestorDraft ringEnd = TInterface.TAncestorDraftCreate(-2, -1, null, "iernan");

        Assert.Throws<InvalidOperationException>(
            () => archive.TAncestorSave(entryId, TInterface.TAncestorDraftResolve([orphan])));
        Assert.Throws<InvalidOperationException>(
            () => archive.TAncestorSave(entryId, TInterface.TAncestorDraftResolve([ringStart, ringEnd])));
    }

    [Fact]
    public void EntryDelete_LinkedEntry_KeepsBoxAsText()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        long source = TAncestorEntryCreate(engine, "rinnan");
        long entryId = TAncestorEntryCreate(engine, "run");
        LAncestorArchive archive = TInterface.TAncestorArchiveCreate(workspace.TWorkspaceDatabase);
        archive.TAncestorSave(entryId, TInterface.TAncestorDraftResolve(TAncestorMapCreate(source)));

        engine.TEngineEntryDelete(source);

        LAncestor linked = archive.TAncestorRead(entryId)[1];
        Assert.Null(linked.LAncestorTargetId);
        Assert.Equal("rinnan", linked.LAncestorText);
    }

    [Fact]
    public void EntryDelete_Owner_DropsAncestorRows()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        long source = TAncestorEntryCreate(engine, "rinnan");
        long entryId = TAncestorEntryCreate(engine, "run");
        LAncestorArchive archive = TInterface.TAncestorArchiveCreate(workspace.TWorkspaceDatabase);
        archive.TAncestorSave(entryId, TInterface.TAncestorDraftResolve(TAncestorMapCreate(source)));

        engine.TEngineEntryDelete(entryId);

        Assert.Equal(0, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM ancestor;"));
    }

    [Fact]
    public void AncestorRead_RootBoxDeleted_ReturnsNothing()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        long source = TAncestorEntryCreate(engine, "rinnan");
        long entryId = TAncestorEntryCreate(engine, "run");
        LAncestorArchive archive = TInterface.TAncestorArchiveCreate(workspace.TWorkspaceDatabase);
        archive.TAncestorSave(entryId, TInterface.TAncestorDraftResolve(TAncestorMapCreate(source)));

        workspace.TWorkspaceScriptRun("DELETE FROM ancestor WHERE child_ref IS NULL;");

        Assert.Empty(archive.TAncestorRead(entryId));
    }

    [Fact]
    public void RelationRead_FreshWorkspace_ReturnsUndeletableBuiltins()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        LAncestorArchive archive = TInterface.TAncestorArchiveCreate(workspace.TWorkspaceDatabase);

        IReadOnlyList<LAncestorRelation> relations = archive.TAncestorRelationRead();

        Assert.Equal(
            ["borrowed", "compound", "derived", "calque", "blend"],
            relations.Select(static relation => relation.LAncestorRelationName));
        Assert.All(relations, static relation => Assert.True(relation.LAncestorRelationBuiltin));
        Assert.False(archive.TAncestorRelationDelete(relations[0].LAncestorRelationId));
        Assert.Equal(5, archive.TAncestorRelationRead().Count);
    }

    [Fact]
    public void RelationDelete_CustomInUse_CountsAndClearsEveryBox()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        long source = TAncestorEntryCreate(engine, "rinnan");
        long entryId = TAncestorEntryCreate(engine, "run");
        LAncestorArchive archive = TInterface.TAncestorArchiveCreate(workspace.TWorkspaceDatabase);
        LAncestorRelation inherited = archive.TAncestorRelationCreate(" inherited ");
        Assert.Equal(inherited, archive.TAncestorRelationCreate("inherited"));
        IReadOnlyList<LAncestorDraft> map = TAncestorMapCreate(source);
        archive.TAncestorSave(entryId, TInterface.TAncestorDraftResolve(
            [map[0] with { LAncestorDraftRelation = inherited.LAncestorRelationId }, .. map.Skip(1)]));

        LAncestorRelation counted = archive.TAncestorRelationRead()[^1];
        Assert.Equal("inherited", counted.LAncestorRelationName);
        Assert.False(counted.LAncestorRelationBuiltin);
        Assert.Equal(1, counted.LAncestorRelationUsage);

        Assert.True(archive.TAncestorRelationDelete(inherited.LAncestorRelationId));

        Assert.Null(archive.TAncestorRead(entryId)[0].LAncestorRelationId);
        Assert.Equal(3, archive.TAncestorRead(entryId).Count);
    }

    [Fact]
    public void TongueDelete_CustomInUse_CountsAndClearsEveryBox()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        long source = TAncestorEntryCreate(engine, "rinnan");
        long entryId = TAncestorEntryCreate(engine, "run");
        LAncestorArchive archive = TInterface.TAncestorArchiveCreate(workspace.TWorkspaceDatabase);
        LAncestorTongue gothic = archive.TAncestorTongueCreate("Gothic");
        IReadOnlyList<LAncestorDraft> map = TAncestorMapCreate(source);
        archive.TAncestorSave(entryId, TInterface.TAncestorDraftResolve(
            [.. map.Take(2), map[2] with { LAncestorDraftTongue = gothic.LAncestorTongueId }]));

        Assert.Equal(1, Assert.Single(archive.TAncestorTongueRead()).LAncestorTongueUsage);
        Assert.True(archive.TAncestorTongueDelete(gothic.LAncestorTongueId));

        Assert.Empty(archive.TAncestorTongueRead());
        Assert.Null(archive.TAncestorRead(entryId)[2].LAncestorTongueId);
    }

    [Fact]
    public void AncestorSave_DeletedRelationAndTongue_SavesBothCleared()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        long source = TAncestorEntryCreate(engine, "rinnan");
        long entryId = TAncestorEntryCreate(engine, "run");
        LAncestorArchive archive = TInterface.TAncestorArchiveCreate(workspace.TWorkspaceDatabase);
        LAncestorRelation inherited = archive.TAncestorRelationCreate("inherited");
        LAncestorTongue gothic = archive.TAncestorTongueCreate("Gothic");
        Assert.True(archive.TAncestorRelationDelete(inherited.LAncestorRelationId));
        Assert.True(archive.TAncestorTongueDelete(gothic.LAncestorTongueId));
        IReadOnlyList<LAncestorDraft> map = TAncestorMapCreate(source);
        LAncestorDraft stale = map[0] with
        {
            LAncestorDraftRelation = inherited.LAncestorRelationId,
            LAncestorDraftTongue = gothic.LAncestorTongueId,
        };

        archive.TAncestorSave(entryId, TInterface.TAncestorDraftResolve([stale, .. map.Skip(1)]));

        LAncestor root = archive.TAncestorRead(entryId)[0];
        Assert.Null(root.LAncestorRelationId);
        Assert.Null(root.LAncestorTongueId);
        Assert.Equal(3, archive.TAncestorRead(entryId).Count);
    }

    [Fact]
    public void AncestorSave_DeletedLinkedEntry_SavesBoxAsText()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        long source = TAncestorEntryCreate(engine, "rinnan");
        long entryId = TAncestorEntryCreate(engine, "run");
        LAncestorArchive archive = TInterface.TAncestorArchiveCreate(workspace.TWorkspaceDatabase);
        IReadOnlyList<LAncestorDraft> map = TAncestorMapCreate(source);

        engine.TEngineEntryDelete(source);
        archive.TAncestorSave(entryId, TInterface.TAncestorDraftResolve(map));

        LAncestor linked = archive.TAncestorRead(entryId)[1];
        Assert.Null(linked.LAncestorTargetId);
        Assert.Equal("rinnan", linked.LAncestorText);
    }

    [Fact]
    public void EntryLoad_SavedMap_FillsDraft()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        long source = TAncestorEntryCreate(engine, "rinnan");
        long entryId = TAncestorEntryCreate(engine, "run");
        LAncestorArchive archive = TInterface.TAncestorArchiveCreate(workspace.TWorkspaceDatabase);
        IReadOnlyList<LAncestor> stored =
            archive.TAncestorSave(entryId, TInterface.TAncestorDraftResolve(TAncestorMapCreate(source)));

        LEntryDraft loaded = TInterface.TEntryVaultCreate(workspace.TWorkspaceDatabase).TEntryLoad(entryId)!;

        Assert.Equal(TInterface.TAncestorDraftCreate(stored), loaded.LEntryDraftAncestors);
    }

    [Fact]
    public void DraftArchiveSave_UnsavedMap_ReadsBackAsWritten()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        LDraft draft = TInterface.TDraftNestedCreate("editor", "kindle");
        IReadOnlyList<LAncestorDraft> map = TAncestorMapCreate(7);
        draft = draft with { LDraftContent = draft.LDraftContent with { LEntryDraftAncestors = map } };

        TInterface.TDraftArchiveSave(workspace.TWorkspaceFolder, draft);
        LDraft loaded = TInterface.TDraftArchiveRead(workspace.TWorkspaceFolder, draft.LDraftId)!;

        Assert.Equal(map, loaded.LDraftContent.LEntryDraftAncestors);
    }

    private static IReadOnlyList<LAncestorDraft> TAncestorMapCreate(long source) =>
    [
        TInterface.TAncestorDraftCreate(-1, null, null, "rennen", "Middle English", null, "to run"),
        TInterface.TAncestorDraftCreate(-2, -1, source, "rinnan"),
        TInterface.TAncestorDraftCreate(-3, -1, null, "iernan"),
    ];

    private static long TAncestorEntryCreate(LEngine engine, string headword)
    {
        return engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            headword, "English", string.Empty, string.Empty, [], [])).LEntryId;
    }
}
