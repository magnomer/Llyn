using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Database.Tests;

public sealed class TSense
{
    [Fact]
    public void AMeaningIsCreatedMovedAndDeletedOneRowAtATime()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = new(workspace.TWorkspaceFolder);

        LEntry entry = TSenseEntryCreate(engine);
        LSense first = Assert.Single(engine.LEngineSenseRead(entry.LEntryId, LOwner.LOwnerEntry));

        LSense second = engine.LEngineSenseCreate(new LSense(
            string.Empty, entry.LEntryId, null, 0, null, null, null, "a second meaning", string.Empty));
        Assert.Equal(1, second.LSensePosition);
        Assert.Equal("a second meaning", engine.LEngineSenseRead(second.LSenseId)?.LSenseDefinition);

        engine.LEngineSenseUpdate(second with { LSenseTitle = "the other one" });
        Assert.Equal("the other one", engine.LEngineSenseRead(second.LSenseId)?.LSenseTitle);

        engine.LEngineSenseMove(second.LSenseId, 0);
        Assert.Equal(
            [second.LSenseId, first.LSenseId],
            engine.LEngineSenseRead(entry.LEntryId, LOwner.LOwnerEntry).Select(row => row.LSenseId));

        engine.LEngineSenseDelete(second.LSenseId);
        LSense left = Assert.Single(engine.LEngineSenseRead(entry.LEntryId, LOwner.LOwnerEntry));
        Assert.Equal(first.LSenseId, left.LSenseId);
        Assert.Equal(0, left.LSensePosition);
    }

    [Fact]
    public void ACollocationIsCreatedMovedAndDeletedOneRowAtATime()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = new(workspace.TWorkspaceFolder);

        LEntry entry = TSenseEntryCreate(engine);
        LCollocation first =
            Assert.Single(engine.LEngineCollocationRead(entry.LEntryId, LOwner.LOwnerEntry));

        LCollocation second = engine.LEngineCollocationCreate(new LCollocation(
            string.Empty, entry.LEntryId, 0, null, "at a word", "quickly"));
        Assert.Equal(1, second.LCollocationPosition);

        engine.LEngineCollocationUpdate(second with { LCollocationMeaning = "in short" });
        engine.LEngineCollocationMove(second.LCollocationId, 0);

        IReadOnlyList<LCollocation> stored =
            engine.LEngineCollocationRead(entry.LEntryId, LOwner.LOwnerEntry);
        Assert.Equal([second.LCollocationId, first.LCollocationId], stored.Select(row => row.LCollocationId));
        Assert.Equal("in short", stored[0].LCollocationMeaning);

        engine.LEngineCollocationDelete(second.LCollocationId);
        Assert.Equal(
            first.LCollocationId,
            Assert.Single(engine.LEngineCollocationRead(entry.LEntryId, LOwner.LOwnerEntry))
                .LCollocationId);
    }

    [Fact]
    public void ASideNeitherCardKindHangsFromIsRefused()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = new(workspace.TWorkspaceFolder);

        LEntry entry = TSenseEntryCreate(engine);
        string senseId = engine.LEngineSenseRead(entry.LEntryId, LOwner.LOwnerEntry)[0].LSenseId;

        Assert.Throws<ArgumentOutOfRangeException>(() =>
            engine.LEngineSenseRead(senseId, LOwner.LOwnerSense));
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            engine.LEngineCollocationRead(senseId, LOwner.LOwnerCollocation));
    }

    private static LEntry TSenseEntryCreate(LEngine engine)
    {
        return engine.LEngineEntrySave(new LEntryDraft(
            "word",
            "English",
            string.Empty,
            string.Empty,
            [new LCardDraft(string.Empty, string.Empty, "a meaning", [], [], [], string.Empty, [], [])],
            [new LCardDraft(string.Empty, "in a word", "briefly", [], [], [], string.Empty, [], [])]));
    }
}
