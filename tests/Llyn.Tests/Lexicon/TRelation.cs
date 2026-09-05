using Llyn.Core;
using Llyn.Infrastructure;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TRelation
{
    [Fact]
    public void ARelationWrittenAgainstAResolvedTargetReadsBack()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LEntry origin = TRelationEntryCreate(engine, "word", "the first meaning");
        LEntry target = TRelationEntryCreate(engine, "term", "the other meaning");

        LEntry resolved = Assert.Single(engine.TEngineEntryFind("term"));
        Assert.Equal(target.LEntryId, resolved.LEntryId);

        string senseId = TRelationSenseRead(workspace, origin.LEntryId);
        LRelation stored = engine.TEngineRelationCreate(TInterface.TRelationCreate(
            string.Empty, senseId, 0, "synonym", null, null, resolved.LEntryId, null));

        Assert.NotEmpty(stored.LRelationId);

        LRelation read = Assert.Single(engine.TEngineRelationRead(senseId));
        Assert.Equal(stored.LRelationId, read.LRelationId);
        Assert.Equal("synonym", read.LRelationType);
        Assert.Equal(target.LEntryId, read.LRelationTargetEntry);
        Assert.Null(read.LRelationTargetSense);
    }

    [Fact]
    public void ARelationMayPointAtAMeaningTheMeaningLookupFound()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LEntry origin = TRelationEntryCreate(engine, "word", "the first meaning");
        LEntry target = TRelationEntryCreate(engine, "term", "the other meaning");

        LSense chosen = Assert.Single(engine.TEngineSenseFind("term"));
        Assert.Equal(target.LEntryId, chosen.LSenseEntryId);
        Assert.Equal("the other meaning", chosen.LSenseDefinition);

        string senseId = TRelationSenseRead(workspace, origin.LEntryId);
        engine.TEngineRelationCreate(TInterface.TRelationCreate(
            string.Empty, senseId, 0, "synonym", null, null, null, chosen.LSenseId));

        LRelation read = Assert.Single(engine.TEngineRelationRead(senseId));
        Assert.Equal(chosen.LSenseId, read.LRelationTargetSense);
        Assert.Null(read.LRelationTargetEntry);
    }

    [Fact]
    public void ATargetThatResolvesToNothingIsRefusedRatherThanWritten()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LEntry origin = TRelationEntryCreate(engine, "word", "the first meaning");
        string senseId = TRelationSenseRead(workspace, origin.LEntryId);

        Assert.Empty(engine.TEngineEntryFind("nothingatall"));
        Assert.Empty(engine.TEngineSenseFind("nothingatall"));

        LRelation unresolved = TInterface.TRelationCreate(
            string.Empty, senseId, 0, "synonym", null, null, "no-such-entry", null);
        LRefusal refusal = Assert.Throws<LRefusal>(() => engine.TEngineRelationCreate(unresolved));
        Assert.Equal(LRefusal.LRefusalTarget, refusal.LRefusalReason);

        Assert.Equal(
            LRefusal.LRefusalTarget,
            Assert.Throws<LRefusal>(() => engine.TEngineRelationCreate(
                unresolved with { LRelationTargetEntry = origin.LEntryId, LRelationTargetSense = senseId }))
                .LRefusalReason);
        Assert.Equal(
            LRefusal.LRefusalTarget,
            Assert.Throws<LRefusal>(() => engine.TEngineRelationCreate(
                unresolved with { LRelationTargetEntry = null })).LRefusalReason);

        Assert.Empty(engine.TEngineRelationRead(senseId));
        Assert.Equal(0, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM relation;"));
    }

    [Fact]
    public void RelationsAreUpdatedMovedAndDeletedThroughTheEngine()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LEntry origin = TRelationEntryCreate(engine, "word", "the first meaning");
        LEntry target = TRelationEntryCreate(engine, "term", "the other meaning");
        string senseId = TRelationSenseRead(workspace, origin.LEntryId);

        LRelation first = engine.TEngineRelationCreate(TInterface.TRelationCreate(
            string.Empty, senseId, 0, "synonym", null, null, target.LEntryId, null));
        LRelation second = engine.TEngineRelationCreate(TInterface.TRelationCreate(
            string.Empty, senseId, 0, "antonym", null, null, target.LEntryId, null));

        Assert.Equal([0, 1], engine.TEngineRelationRead(senseId).Select(row => row.LRelationPosition));

        engine.TEngineRelationUpdate(first with { LRelationType = "hypernym", LRelationLabel = "formal" });
        LRelation updated = engine.TEngineRelationRead(senseId)[0];
        Assert.Equal("hypernym", updated.LRelationType);
        Assert.Equal("formal", updated.LRelationLabel);

        engine.TEngineRelationMove(second.LRelationId, 0);
        Assert.Equal(
            [second.LRelationId, first.LRelationId],
            engine.TEngineRelationRead(senseId).Select(row => row.LRelationId));

        engine.TEngineRelationDelete(second.LRelationId);
        Assert.Equal(first.LRelationId, Assert.Single(engine.TEngineRelationRead(senseId)).LRelationId);

        Assert.NotNull(engine.TEngineEntryRead(target.LEntryId));
    }

    [Fact]
    public void ACollocationSynonymIsCreatedUpdatedMovedAndDeletedAgainstResolvedTargets()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LEntry origin = TRelationEntryCreate(engine, "word", "the first meaning");
        LEntry target = TRelationEntryCreate(engine, "term", "the other meaning");
        string collocationId = Assert
            .Single(TInterface.TCollocationArchiveCreate(workspace.TWorkspaceDatabase).TCollocationRead(origin.LEntryId))
            .LCollocationId;

        LSynonym first = engine.TEngineSynonymCreate(
            TInterface.TSynonymCreate(string.Empty, collocationId, 0, target.LEntryId, null));
        LSynonym second = engine.TEngineSynonymCreate(
            TInterface.TSynonymCreate(string.Empty, collocationId, 0, target.LEntryId, null));

        Assert.Equal([0, 1], engine.TEngineSynonymRead(collocationId).Select(row => row.LSynonymPosition));

        LSense chosen = Assert.Single(engine.TEngineSenseFind("term"));
        engine.TEngineSynonymUpdate(first with
        {
            LSynonymTargetEntry = null,
            LSynonymTargetSense = chosen.LSenseId,
        });
        LSynonym repointed = engine.TEngineSynonymRead(collocationId)[0];
        Assert.Equal(chosen.LSenseId, repointed.LSynonymTargetSense);
        Assert.Null(repointed.LSynonymTargetEntry);

        Assert.Equal(
            LRefusal.LRefusalTarget,
            Assert.Throws<LRefusal>(() => engine.TEngineSynonymUpdate(
                first with { LSynonymTargetEntry = "no-such-entry", LSynonymTargetSense = null }))
                .LRefusalReason);
        Assert.Equal(
            LRefusal.LRefusalTarget,
            Assert.Throws<LRefusal>(() => engine.TEngineSynonymCreate(
                TInterface.TSynonymCreate(string.Empty, collocationId, 0, null, null))).LRefusalReason);

        engine.TEngineSynonymMove(second.LSynonymId, 0);
        Assert.Equal(
            [second.LSynonymId, first.LSynonymId],
            engine.TEngineSynonymRead(collocationId).Select(row => row.LSynonymId));

        engine.TEngineSynonymDelete(second.LSynonymId);
        Assert.Equal(first.LSynonymId, Assert.Single(engine.TEngineSynonymRead(collocationId)).LSynonymId);
        Assert.NotNull(engine.TEngineEntryRead(target.LEntryId));
    }

    [Fact]
    public void TheMeaningLookupAnswersTheSameWayTheEntryLookupDoes()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        TRelationEntryCreate(engine, "Résumé", "a summary");
        TRelationEntryCreate(engine, "term", "the other meaning");

        Assert.Equal("a summary", Assert.Single(engine.TEngineSenseFind("résumé")).LSenseDefinition);
        Assert.Equal("a summary", Assert.Single(engine.TEngineSenseFind("RÉSUMÉ")).LSenseDefinition);

        Assert.Equal(
            ["a summary", "the other meaning"],
            engine.TEngineSenseFind(string.Empty).Select(sense => sense.LSenseDefinition));
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

    private static string TRelationSenseRead(TWorkspace workspace, string entryId)
    {
        return Assert.Single(TInterface.TSenseArchiveCreate(workspace.TWorkspaceDatabase).TSenseRead(entryId)).LSenseId;
    }
}
