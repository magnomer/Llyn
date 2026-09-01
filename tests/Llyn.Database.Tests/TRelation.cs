using Llyn.Core;
using Llyn.Infrastructure;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Database.Tests;

/// <summary>
/// Covers the engine's relation seam: a Meaning's relations and a Collocation's synonyms created,
/// read, updated, moved and deleted through <see cref="LEngine"/>, the lookup a caller resolves typed
/// text with before it writes, and the refusal that meets a target which resolved to nothing.
/// </summary>
public sealed class TRelation
{
    [Fact]
    public void ARelationWrittenAgainstAResolvedTargetReadsBack()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = new(workspace.TWorkspaceFolder);

        LEntry origin = TRelationEntryCreate(engine, "word", "the first meaning");
        LEntry target = TRelationEntryCreate(engine, "term", "the other meaning");

        // The caller resolves first: typed text becomes the entry the user meant, and its id is what
        // reaches the engine. Nothing about the text is handed over.
        LEntry resolved = Assert.Single(engine.LEngineEntryFind("term"));
        Assert.Equal(target.LEntryId, resolved.LEntryId);

        string senseId = TRelationSenseRead(workspace, origin.LEntryId);
        LRelation stored = engine.LEngineRelationCreate(new LRelation(
            string.Empty, senseId, 0, "synonym", null, null, resolved.LEntryId, null));

        Assert.NotEmpty(stored.LRelationId);

        LRelation read = Assert.Single(engine.LEngineRelationRead(senseId));
        Assert.Equal(stored.LRelationId, read.LRelationId);
        Assert.Equal("synonym", read.LRelationType);
        Assert.Equal(target.LEntryId, read.LRelationTargetEntry);
        Assert.Null(read.LRelationTargetSense);
    }

    [Fact]
    public void ARelationMayPointAtAMeaningTheMeaningLookupFound()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = new(workspace.TWorkspaceFolder);

        LEntry origin = TRelationEntryCreate(engine, "word", "the first meaning");
        LEntry target = TRelationEntryCreate(engine, "term", "the other meaning");

        // The meaning-level twin of the entry lookup: text names the Meanings it could mean, and the
        // caller picks one of them.
        LSense chosen = Assert.Single(engine.LEngineSenseFind("term"));
        Assert.Equal(target.LEntryId, chosen.LSenseEntryId);
        Assert.Equal("the other meaning", chosen.LSenseDefinition);

        string senseId = TRelationSenseRead(workspace, origin.LEntryId);
        engine.LEngineRelationCreate(new LRelation(
            string.Empty, senseId, 0, "synonym", null, null, null, chosen.LSenseId));

        LRelation read = Assert.Single(engine.LEngineRelationRead(senseId));
        Assert.Equal(chosen.LSenseId, read.LRelationTargetSense);
        Assert.Null(read.LRelationTargetEntry);
    }

    [Fact]
    public void ATargetThatResolvesToNothingIsRefusedRatherThanWritten()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = new(workspace.TWorkspaceFolder);

        LEntry origin = TRelationEntryCreate(engine, "word", "the first meaning");
        string senseId = TRelationSenseRead(workspace, origin.LEntryId);

        // Nothing in the workspace is spelled this way, so the lookup finds nothing to resolve to.
        Assert.Empty(engine.LEngineEntryFind("nothingatall"));
        Assert.Empty(engine.LEngineSenseFind("nothingatall"));

        // An id nobody carries, both targets at once, and neither target are one failure: the caller
        // did not hand over a resolved target, and all three are refused with the same stated reason.
        LRelation unresolved = new(
            string.Empty, senseId, 0, "synonym", null, null, "no-such-entry", null);
        LRefusal refusal = Assert.Throws<LRefusal>(() => engine.LEngineRelationCreate(unresolved));
        Assert.Equal(LRefusal.LRefusalTarget, refusal.LRefusalReason);

        Assert.Equal(
            LRefusal.LRefusalTarget,
            Assert.Throws<LRefusal>(() => engine.LEngineRelationCreate(
                unresolved with { LRelationTargetEntry = origin.LEntryId, LRelationTargetSense = senseId }))
                .LRefusalReason);
        Assert.Equal(
            LRefusal.LRefusalTarget,
            Assert.Throws<LRefusal>(() => engine.LEngineRelationCreate(
                unresolved with { LRelationTargetEntry = null })).LRefusalReason);

        // A refused write leaves no row behind: the relation is not written half-resolved.
        Assert.Empty(engine.LEngineRelationRead(senseId));
        Assert.Equal(0, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM relation;"));
    }

    [Fact]
    public void RelationsAreUpdatedMovedAndDeletedThroughTheEngine()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = new(workspace.TWorkspaceFolder);

        LEntry origin = TRelationEntryCreate(engine, "word", "the first meaning");
        LEntry target = TRelationEntryCreate(engine, "term", "the other meaning");
        string senseId = TRelationSenseRead(workspace, origin.LEntryId);

        LRelation first = engine.LEngineRelationCreate(new LRelation(
            string.Empty, senseId, 0, "synonym", null, null, target.LEntryId, null));
        LRelation second = engine.LEngineRelationCreate(new LRelation(
            string.Empty, senseId, 0, "antonym", null, null, target.LEntryId, null));

        Assert.Equal([0, 1], engine.LEngineRelationRead(senseId).Select(row => row.LRelationPosition));

        engine.LEngineRelationUpdate(first with { LRelationType = "hypernym", LRelationLabel = "formal" });
        LRelation updated = engine.LEngineRelationRead(senseId)[0];
        Assert.Equal("hypernym", updated.LRelationType);
        Assert.Equal("formal", updated.LRelationLabel);

        engine.LEngineRelationMove(second.LRelationId, 0);
        Assert.Equal(
            [second.LRelationId, first.LRelationId],
            engine.LEngineRelationRead(senseId).Select(row => row.LRelationId));

        engine.LEngineRelationDelete(second.LRelationId);
        Assert.Equal(first.LRelationId, Assert.Single(engine.LEngineRelationRead(senseId)).LRelationId);

        // The link went; what it linked to did not.
        Assert.NotNull(engine.LEngineEntryRead(target.LEntryId));
    }

    [Fact]
    public void ACollocationSynonymIsCreatedUpdatedMovedAndDeletedAgainstResolvedTargets()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = new(workspace.TWorkspaceFolder);

        LEntry origin = TRelationEntryCreate(engine, "word", "the first meaning");
        LEntry target = TRelationEntryCreate(engine, "term", "the other meaning");
        string collocationId = Assert
            .Single(new LCollocationArchive(workspace.TWorkspaceDatabase).LCollocationRead(origin.LEntryId))
            .LCollocationId;

        LSynonym first = engine.LEngineSynonymCreate(
            new LSynonym(string.Empty, collocationId, 0, target.LEntryId, null));
        LSynonym second = engine.LEngineSynonymCreate(
            new LSynonym(string.Empty, collocationId, 0, target.LEntryId, null));

        Assert.Equal([0, 1], engine.LEngineSynonymRead(collocationId).Select(row => row.LSynonymPosition));

        // Re-pointing resolves the new target on the same terms the create did.
        LSense chosen = Assert.Single(engine.LEngineSenseFind("term"));
        engine.LEngineSynonymUpdate(first with
        {
            LSynonymTargetEntry = null,
            LSynonymTargetSense = chosen.LSenseId,
        });
        LSynonym repointed = engine.LEngineSynonymRead(collocationId)[0];
        Assert.Equal(chosen.LSenseId, repointed.LSynonymTargetSense);
        Assert.Null(repointed.LSynonymTargetEntry);

        Assert.Equal(
            LRefusal.LRefusalTarget,
            Assert.Throws<LRefusal>(() => engine.LEngineSynonymUpdate(
                first with { LSynonymTargetEntry = "no-such-entry", LSynonymTargetSense = null }))
                .LRefusalReason);
        Assert.Equal(
            LRefusal.LRefusalTarget,
            Assert.Throws<LRefusal>(() => engine.LEngineSynonymCreate(
                new LSynonym(string.Empty, collocationId, 0, null, null))).LRefusalReason);

        engine.LEngineSynonymMove(second.LSynonymId, 0);
        Assert.Equal(
            [second.LSynonymId, first.LSynonymId],
            engine.LEngineSynonymRead(collocationId).Select(row => row.LSynonymId));

        engine.LEngineSynonymDelete(second.LSynonymId);
        Assert.Equal(first.LSynonymId, Assert.Single(engine.LEngineSynonymRead(collocationId)).LSynonymId);
        Assert.NotNull(engine.LEngineEntryRead(target.LEntryId));
    }

    [Fact]
    public void TheMeaningLookupAnswersTheSameWayTheEntryLookupDoes()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = new(workspace.TWorkspaceFolder);

        TRelationEntryCreate(engine, "Résumé", "a summary");
        TRelationEntryCreate(engine, "term", "the other meaning");

        // Folded over the whole of Unicode, the same as the entry lookup, so an accented headword is
        // found typed in either case.
        Assert.Equal("a summary", Assert.Single(engine.LEngineSenseFind("résumé")).LSenseDefinition);
        Assert.Equal("a summary", Assert.Single(engine.LEngineSenseFind("RÉSUMÉ")).LSenseDefinition);

        // An empty query is every Meaning, ordered by the headword each one hangs from.
        Assert.Equal(
            ["a summary", "the other meaning"],
            engine.LEngineSenseFind(string.Empty).Select(sense => sense.LSenseDefinition));
    }

    // One saved entry with a single Meaning card and a single Collocation card, which is the shape
    // every test here needs an origin and a target to have.
    private static LEntry TRelationEntryCreate(LEngine engine, string headword, string definition)
    {
        return engine.LEngineEntrySave(new LEntryDraft(
            headword,
            "English",
            string.Empty,
            string.Empty,
            [new LCardDraft(string.Empty, string.Empty, definition, [], [], string.Empty, [])],
            [new LCardDraft(string.Empty, "in a word", "briefly", [], [], string.Empty, [])]));
    }

    // The id of the entry's only Meaning.
    private static string TRelationSenseRead(TWorkspace workspace, string entryId)
    {
        return Assert.Single(new LSenseArchive(workspace.TWorkspaceDatabase).LSenseRead(entryId)).LSenseId;
    }
}
