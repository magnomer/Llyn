using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TSense
{
    [Fact]
    public void AMeaningIsCreatedMovedAndDeletedOneRowAtATime()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LEntry entry = TSenseEntryCreate(engine);
        LSense first = Assert.Single(engine.TEngineSenseRead(entry.LEntryId, LOwner.LOwnerEntry));

        LSense second = engine.TEngineSenseCreate(TInterface.TSenseCreate(
            string.Empty, entry.LEntryId, null, 0, null, null, null, "a second meaning", string.Empty));
        Assert.Equal(1, second.LSensePosition);
        Assert.Equal("a second meaning", engine.TEngineSenseRead(second.LSenseId)?.LSenseDefinition);

        engine.TEngineSenseUpdate(second with { LSenseTitle = "the other one" });
        Assert.Equal("the other one", engine.TEngineSenseRead(second.LSenseId)?.LSenseTitle);

        engine.TEngineSenseMove(second.LSenseId, 0);
        Assert.Equal(
            [second.LSenseId, first.LSenseId],
            engine.TEngineSenseRead(entry.LEntryId, LOwner.LOwnerEntry).Select(row => row.LSenseId));

        engine.TEngineSenseDelete(second.LSenseId);
        LSense left = Assert.Single(engine.TEngineSenseRead(entry.LEntryId, LOwner.LOwnerEntry));
        Assert.Equal(first.LSenseId, left.LSenseId);
        Assert.Equal(0, left.LSensePosition);
    }

    [Fact]
    public void ACollocationIsCreatedMovedAndDeletedOneRowAtATime()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LEntry entry = TSenseEntryCreate(engine);
        LCollocation first =
            Assert.Single(engine.TEngineCollocationRead(entry.LEntryId, LOwner.LOwnerEntry));

        LCollocation second = engine.TEngineCollocationCreate(TInterface.TCollocationCreate(
            string.Empty, entry.LEntryId, 0, null, "at a word", "quickly"));
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
    public void ASideNeitherCardKindHangsFromIsRefused()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LEntry entry = TSenseEntryCreate(engine);
        string senseId = engine.TEngineSenseRead(entry.LEntryId, LOwner.LOwnerEntry)[0].LSenseId;

        Assert.Throws<ArgumentOutOfRangeException>(() =>
            engine.TEngineSenseRead(senseId, LOwner.LOwnerSense));
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            engine.TEngineCollocationRead(senseId, LOwner.LOwnerCollocation));
    }

    private static LEntry TSenseEntryCreate(LEngine engine)
    {
        return engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "word",
            "English",
            string.Empty,
            string.Empty,
            [TInterface.TCardDraftCreate(string.Empty, string.Empty, "a meaning", [], [], [], string.Empty, [], [], 1)],
            [TInterface.TCardDraftCreate(string.Empty, "in a word", "briefly", [], [], [], string.Empty, [], [], 1)]));
    }
}
