using Llyn.Core;
using Llyn.Infrastructure;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TRelation
{
    [Fact]
    public void RelationCreate_ResolvedTarget_ReadsBack()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LEntry origin = TRelationEntryCreate(engine, "word", "the first meaning");
        LEntry target = TRelationEntryCreate(engine, "term", "the other meaning");

        LEntry resolved = Assert.Single(engine.TEngineEntryFind("term"));
        Assert.Equal(target.LEntryId, resolved.LEntryId);

        long meaningId = TRelationMeaningRead(workspace, origin.LEntryId);
        LRelation stored = engine.TEngineRelationCreate(TInterface.TRelationCreate(
            0, meaningId, 0, "synonym", null, null, resolved.LEntryId, null));

        Assert.NotEqual(0, stored.LRelationId);

        LRelation read = Assert.Single(engine.TEngineRelationRead(meaningId));
        Assert.Equal(stored.LRelationId, read.LRelationId);
        Assert.Equal("synonym", read.LRelationType);
        Assert.Equal(target.LEntryId, read.LRelationTargetEntry.TStateAnchorShow());
        Assert.True(read.LRelationTargetMeaning.LStateAnchorEmpty);
    }

    [Fact]
    public void RelationCreate_MeaningTarget_ReadsBack()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LEntry origin = TRelationEntryCreate(engine, "word", "the first meaning");
        LEntry target = TRelationEntryCreate(engine, "term", "the other meaning");

        LMeaning chosen = Assert.Single(engine.TEngineMeaningFind("term"));
        Assert.Equal(target.LEntryId, chosen.LMeaningEntryId);
        Assert.Equal("the other meaning", chosen.LMeaningDefinition);

        long meaningId = TRelationMeaningRead(workspace, origin.LEntryId);
        engine.TEngineRelationCreate(TInterface.TRelationCreate(
            0, meaningId, 0, "synonym", null, null, null, chosen.LMeaningId));

        LRelation read = Assert.Single(engine.TEngineRelationRead(meaningId));
        Assert.Equal(chosen.LMeaningId, read.LRelationTargetMeaning.TStateAnchorShow());
        Assert.True(read.LRelationTargetEntry.LStateAnchorEmpty);
    }

    [Fact]
    public void RelationCreate_UnresolvedTarget_RefusesWriting()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LEntry origin = TRelationEntryCreate(engine, "word", "the first meaning");
        long meaningId = TRelationMeaningRead(workspace, origin.LEntryId);

        Assert.Empty(engine.TEngineEntryFind("nothingatall"));
        Assert.Empty(engine.TEngineMeaningFind("nothingatall"));

        LRelation unresolved = TInterface.TRelationCreate(
            0, meaningId, 0, "synonym", null, null, 9999, null);
        LRefusal refusal = Assert.Throws<LRefusal>(() => engine.TEngineRelationCreate(unresolved));
        Assert.Equal(LRefusal.LRefusalTarget, refusal.LRefusalReason);

        Assert.Equal(
            LRefusal.LRefusalTarget,
            Assert.Throws<LRefusal>(() => engine.TEngineRelationCreate(
                unresolved with
                {
                    LRelationTargetEntry = TInterface.TStateAnchorRead(origin.LEntryId),
                    LRelationTargetMeaning = TInterface.TStateAnchorRead(meaningId),
                }))
                .LRefusalReason);
        Assert.Equal(
            LRefusal.LRefusalTarget,
            Assert.Throws<LRefusal>(() => engine.TEngineRelationCreate(
                unresolved with { LRelationTargetEntry = TInterface.TStateAnchorRead(null) })).LRefusalReason);

        Assert.Empty(engine.TEngineRelationRead(meaningId));
        Assert.Equal(0, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM relation;"));
    }

    [Fact]
    public void RelationUpdate_MovedAndDeleted_ReadsBackEachStep()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LEntry origin = TRelationEntryCreate(engine, "word", "the first meaning");
        LEntry target = TRelationEntryCreate(engine, "term", "the other meaning");
        long meaningId = TRelationMeaningRead(workspace, origin.LEntryId);

        LRelation first = engine.TEngineRelationCreate(TInterface.TRelationCreate(
            0, meaningId, 0, "synonym", null, null, target.LEntryId, null));
        LRelation second = engine.TEngineRelationCreate(TInterface.TRelationCreate(
            0, meaningId, 0, "antonym", null, null, target.LEntryId, null));

        Assert.Equal([0, 1], engine.TEngineRelationRead(meaningId).Select(row => row.LRelationPosition));

        engine.TEngineRelationUpdate(first with { LRelationType = "hypernym", LRelationLabel = "formal" });
        LRelation updated = engine.TEngineRelationRead(meaningId)[0];
        Assert.Equal("hypernym", updated.LRelationType);
        Assert.Equal("formal", updated.LRelationLabel);

        engine.TEngineRelationMove(second.LRelationId, 0);
        Assert.Equal(
            [second.LRelationId, first.LRelationId],
            engine.TEngineRelationRead(meaningId).Select(row => row.LRelationId));

        engine.TEngineRelationDelete(second.LRelationId);
        Assert.Equal(first.LRelationId, Assert.Single(engine.TEngineRelationRead(meaningId)).LRelationId);

        Assert.NotNull(engine.TEngineEntryRead(target.LEntryId));
    }

    [Fact]
    public void SynonymCreate_ResolvedTargets_ReadsBackEachStep()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LEntry origin = TRelationEntryCreate(engine, "word", "the first meaning");
        LEntry target = TRelationEntryCreate(engine, "term", "the other meaning");
        long collocationId = Assert
            .Single(TInterface.TCollocationArchiveCreate(workspace.TWorkspaceDatabase).TCollocationRead(origin.LEntryId))
            .LCollocationId;

        LSynonym first = engine.TEngineSynonymCreate(
            TInterface.TSynonymCreate(0, collocationId, 0, target.LEntryId, null));
        LSynonym second = engine.TEngineSynonymCreate(
            TInterface.TSynonymCreate(0, collocationId, 0, target.LEntryId, null));

        Assert.Equal([0, 1], engine.TEngineSynonymRead(collocationId).Select(row => row.LSynonymPosition));

        LMeaning chosen = Assert.Single(engine.TEngineMeaningFind("term"));
        engine.TEngineSynonymUpdate(first with
        {
            LSynonymTargetEntry = TInterface.TStateAnchorRead(null),
            LSynonymTargetMeaning = TInterface.TStateAnchorRead(chosen.LMeaningId),
        });
        LSynonym repointed = engine.TEngineSynonymRead(collocationId)[0];
        Assert.Equal(chosen.LMeaningId, repointed.LSynonymTargetMeaning.TStateAnchorShow());
        Assert.True(repointed.LSynonymTargetEntry.LStateAnchorEmpty);

        Assert.Equal(
            LRefusal.LRefusalTarget,
            Assert.Throws<LRefusal>(() => engine.TEngineSynonymUpdate(
                first with
                {
                    LSynonymTargetEntry = TInterface.TStateAnchorRead(9999),
                    LSynonymTargetMeaning = TInterface.TStateAnchorRead(null),
                }))
                .LRefusalReason);
        Assert.Equal(
            LRefusal.LRefusalTarget,
            Assert.Throws<LRefusal>(() => engine.TEngineSynonymCreate(
                TInterface.TSynonymCreate(0, collocationId, 0, null, null))).LRefusalReason);

        engine.TEngineSynonymMove(second.LSynonymId, 0);
        Assert.Equal(
            [second.LSynonymId, first.LSynonymId],
            engine.TEngineSynonymRead(collocationId).Select(row => row.LSynonymId));

        engine.TEngineSynonymDelete(second.LSynonymId);
        Assert.Equal(first.LSynonymId, Assert.Single(engine.TEngineSynonymRead(collocationId)).LSynonymId);
        Assert.NotNull(engine.TEngineEntryRead(target.LEntryId));
    }

    [Fact]
    public void MeaningFind_SameQueryAsEntryLookup_AnswersAlike()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        TRelationEntryCreate(engine, "Résumé", "a summary");
        TRelationEntryCreate(engine, "term", "the other meaning");

        Assert.Equal("a summary", Assert.Single(engine.TEngineMeaningFind("résumé")).LMeaningDefinition);
        Assert.Equal("a summary", Assert.Single(engine.TEngineMeaningFind("RÉSUMÉ")).LMeaningDefinition);

        Assert.Equal(
            ["a summary", "the other meaning"],
            engine.TEngineMeaningFind(string.Empty).Select(meaning => meaning.LMeaningDefinition));
    }

    private static LEntry TRelationEntryCreate(LEngine engine, string headword, string definition)
    {
        return engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            headword,
            "English",
            string.Empty,
            string.Empty,
            [TInterface.TCardDraftCreate(string.Empty, string.Empty, definition, [], [], [], string.Empty, [], [], 1)],
            [TInterface.TCardDraftCreate(string.Empty, "in a word", "briefly", [], [], [], string.Empty, [], [], 1)]));
    }

    private static long TRelationMeaningRead(TWorkspace workspace, long entryId)
    {
        return Assert.Single(TInterface.TMeaningArchiveCreate(workspace.TWorkspaceDatabase).TMeaningRead(entryId)).LMeaningId;
    }
}
