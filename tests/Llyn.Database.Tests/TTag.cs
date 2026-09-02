using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Database.Tests;

public sealed class TTag
{
    [Fact]
    public void ATagIsWrittenOnceAndReferencedFromBothCardKinds()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = new(workspace.TWorkspaceFolder);

        LEntry entry = TTagEntryCreate(engine);
        string senseId = engine.LEngineSenseRead(entry.LEntryId, LOwner.LOwnerEntry)[0].LSenseId;
        string collocationId =
            engine.LEngineCollocationRead(entry.LEntryId, LOwner.LOwnerEntry)[0].LCollocationId;

        LTag formal = engine.LEngineTagCreate(new LTag(string.Empty, "formal"));
        LTag rare = engine.LEngineTagCreate(new LTag(string.Empty, "rare"));
        Assert.NotEmpty(formal.LTagId);
        Assert.Equal("formal", engine.LEngineTagRead(formal.LTagId)?.LTagText);

        engine.LEngineTagAttach(senseId, rare.LTagId, 0, LOwner.LOwnerSense);
        engine.LEngineTagAttach(senseId, formal.LTagId, 0, LOwner.LOwnerSense);
        engine.LEngineTagAttach(collocationId, formal.LTagId, 0, LOwner.LOwnerCollocation);

        Assert.Equal(
            ["formal", "rare"],
            engine.LEngineTagRead(senseId, LOwner.LOwnerSense).Select(tag => tag.LTagText));
        Assert.Equal(
            formal.LTagId,
            Assert.Single(engine.LEngineTagRead(collocationId, LOwner.LOwnerCollocation)).LTagId);

        engine.LEngineTagUpdate(formal with { LTagText = "formal register" });
        Assert.Equal(
            "formal register",
            Assert.Single(engine.LEngineTagRead(collocationId, LOwner.LOwnerCollocation)).LTagText);
    }

    [Fact]
    public void ADetachLeavesTheTagStandingAndARemovedLastReferenceTakesItWithIt()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = new(workspace.TWorkspaceFolder);

        LEntry entry = TTagEntryCreate(engine);
        string senseId = engine.LEngineSenseRead(entry.LEntryId, LOwner.LOwnerEntry)[0].LSenseId;
        string collocationId =
            engine.LEngineCollocationRead(entry.LEntryId, LOwner.LOwnerEntry)[0].LCollocationId;

        LTag tag = engine.LEngineTagCreate(new LTag(string.Empty, "formal"));
        engine.LEngineTagAttach(senseId, tag.LTagId, 0, LOwner.LOwnerSense);
        engine.LEngineTagAttach(collocationId, tag.LTagId, 0, LOwner.LOwnerCollocation);

        engine.LEngineTagDetach(senseId, tag.LTagId, LOwner.LOwnerSense);
        Assert.Empty(engine.LEngineTagRead(senseId, LOwner.LOwnerSense));
        Assert.NotNull(engine.LEngineTagRead(tag.LTagId));

        engine.LEngineTagAttach(senseId, tag.LTagId, 0, LOwner.LOwnerSense);
        engine.LEngineTagRemove(senseId, tag.LTagId, LOwner.LOwnerSense);
        Assert.NotNull(engine.LEngineTagRead(tag.LTagId));

        engine.LEngineTagRemove(collocationId, tag.LTagId, LOwner.LOwnerCollocation);
        Assert.Null(engine.LEngineTagRead(tag.LTagId));
        Assert.Equal(0, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM tag;"));
    }

    [Fact]
    public void ADeleteIsRefusedWhileACardStillReferencesTheTag()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = new(workspace.TWorkspaceFolder);

        LEntry entry = TTagEntryCreate(engine);
        string senseId = engine.LEngineSenseRead(entry.LEntryId, LOwner.LOwnerEntry)[0].LSenseId;

        LTag tag = engine.LEngineTagCreate(new LTag(string.Empty, "formal"));
        engine.LEngineTagAttach(senseId, tag.LTagId, 0, LOwner.LOwnerSense);

        Assert.Throws<InvalidOperationException>(() => engine.LEngineTagDelete(tag.LTagId));
        Assert.NotNull(engine.LEngineTagRead(tag.LTagId));

        engine.LEngineTagDetach(senseId, tag.LTagId, LOwner.LOwnerSense);
        engine.LEngineTagDelete(tag.LTagId);
        Assert.Null(engine.LEngineTagRead(tag.LTagId));
    }

    [Fact]
    public void ASideATagNeverHangsFromIsRefusedRatherThanGuessedAt()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = new(workspace.TWorkspaceFolder);

        LEntry entry = TTagEntryCreate(engine);
        LTag tag = engine.LEngineTagCreate(new LTag(string.Empty, "formal"));

        Assert.Throws<ArgumentOutOfRangeException>(() =>
            engine.LEngineTagAttach(entry.LEntryId, tag.LTagId, 0, LOwner.LOwnerEntry));
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            engine.LEngineTagRead(entry.LEntryId, LOwner.LOwnerEntry));
    }

    private static LEntry TTagEntryCreate(LEngine engine)
    {
        return engine.LEngineEntrySave(new LEntryDraft(
            "word",
            "English",
            string.Empty,
            string.Empty,
            [new LCardDraft(string.Empty, string.Empty, "a meaning", [], [], string.Empty, [], [])],
            [new LCardDraft(string.Empty, "in a word", "briefly", [], [], string.Empty, [], [])]));
    }
}
