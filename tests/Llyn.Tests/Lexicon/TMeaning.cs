using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TMeaning
{
    [Fact]
    public void MeaningCreate_OneRowAtATime_ReadsBackEachStep()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LEntry entry = TMeaningEntryCreate(engine);
        LMeaning first = Assert.Single(engine.TEngineMeaningRead(entry.LEntryId, LOwner.LOwnerEntry));

        LMeaning second = engine.TEngineMeaningCreate(TInterface.TMeaningCreate(
            0, entry.LEntryId, null, 0, null, "a second meaning"));
        Assert.Equal(1, second.LMeaningPosition);
        Assert.Equal("a second meaning", engine.TEngineMeaningRead(second.LMeaningId)?.LMeaningDefinition);

        engine.TEngineMeaningUpdate(second with { LMeaningTitle = "the other one" });
        Assert.Equal("the other one", engine.TEngineMeaningRead(second.LMeaningId)?.LMeaningTitle);

        engine.TEngineMeaningMove(second.LMeaningId, 0);
        Assert.Equal(
            [second.LMeaningId, first.LMeaningId],
            engine.TEngineMeaningRead(entry.LEntryId, LOwner.LOwnerEntry).Select(row => row.LMeaningId));

        engine.TEngineMeaningDelete(second.LMeaningId);
        LMeaning left = Assert.Single(engine.TEngineMeaningRead(entry.LEntryId, LOwner.LOwnerEntry));
        Assert.Equal(first.LMeaningId, left.LMeaningId);
        Assert.Equal(0, left.LMeaningPosition);
    }

    [Fact]
    public void CollocationCreate_OneRowAtATime_ReadsBackEachStep()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LEntry entry = TMeaningEntryCreate(engine);
        LCollocation first =
            Assert.Single(engine.TEngineCollocationRead(entry.LEntryId, LOwner.LOwnerEntry));

        LCollocation second = engine.TEngineCollocationCreate(TInterface.TCollocationCreate(
            0, entry.LEntryId, 0, null, "at a word", "quickly"));
        Assert.Equal(1, second.LCollocationPosition);

        engine.TEngineCollocationUpdate(second with { LCollocationMeaning = "in short" });
        engine.TEngineCollocationMove(second.LCollocationId, 0);

        IReadOnlyList<LCollocation> stored =
            engine.TEngineCollocationRead(entry.LEntryId, LOwner.LOwnerEntry);
        Assert.Equal([second.LCollocationId, first.LCollocationId], stored.Select(row => row.LCollocationId));
        Assert.Equal("in short", stored[0].LCollocationMeaning);

        engine.TEngineCollocationDelete(second.LCollocationId);
        Assert.Equal(
            first.LCollocationId,
            Assert.Single(engine.TEngineCollocationRead(entry.LEntryId, LOwner.LOwnerEntry))
                .LCollocationId);
    }

    [Fact]
    public void MeaningRead_UnknownSide_Throws()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LEntry entry = TMeaningEntryCreate(engine);
        long meaningId = engine.TEngineMeaningRead(entry.LEntryId, LOwner.LOwnerEntry)[0].LMeaningId;

        Assert.Throws<ArgumentOutOfRangeException>(() =>
            engine.TEngineMeaningRead(meaningId, LOwner.LOwnerMeaning));
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            engine.TEngineCollocationRead(meaningId, LOwner.LOwnerCollocation));
    }

    private static LEntry TMeaningEntryCreate(LEngine engine)
    {
        return engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "word",
            "English",
            string.Empty,
            string.Empty,
            [TInterface.TCardDraftCreate(string.Empty, string.Empty, "a meaning", [], [], [], [], [], 1)],
            [TInterface.TCardDraftCreate(string.Empty, "in a word", "briefly", [], [], [], [], [], 1)]));
    }
}
