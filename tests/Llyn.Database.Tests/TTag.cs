using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Database.Tests;

/// <summary>
/// Covers the engine's Tag seam: a Tag created, read, renamed and referenced from both card kinds
/// through <see cref="LEngine"/>, the difference between detaching a reference and removing one, and
/// the refusals that meet a delete the references forbid and a side a Tag never hangs from.
/// </summary>
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

        // One row, referenced from both sides, each side holding its own order over what it references.
        engine.LEngineTagAttach(senseId, rare.LTagId, 0, LOwner.LOwnerSense);
        engine.LEngineTagAttach(senseId, formal.LTagId, 0, LOwner.LOwnerSense);
        engine.LEngineTagAttach(collocationId, formal.LTagId, 0, LOwner.LOwnerCollocation);

        Assert.Equal(
            ["formal", "rare"],
            engine.LEngineTagRead(senseId, LOwner.LOwnerSense).Select(tag => tag.LTagText));
        Assert.Equal(
            formal.LTagId,
            Assert.Single(engine.LEngineTagRead(collocationId, LOwner.LOwnerCollocation)).LTagId);

        // A rename reaches every reference at once: a reference points at the row, never at its words.
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

        // Detaching is what editing a card does: the reference goes, the row other cards use stays.
        engine.LEngineTagDetach(senseId, tag.LTagId, LOwner.LOwnerSense);
        Assert.Empty(engine.LEngineTagRead(senseId, LOwner.LOwnerSense));
        Assert.NotNull(engine.LEngineTagRead(tag.LTagId));

        // Removing is the other intention, and it still leaves a Tag another card references.
        engine.LEngineTagAttach(senseId, tag.LTagId, 0, LOwner.LOwnerSense);
        engine.LEngineTagRemove(senseId, tag.LTagId, LOwner.LOwnerSense);
        Assert.NotNull(engine.LEngineTagRead(tag.LTagId));

        // The last reference going takes the row with it, so no Tag is left that nothing can reach.
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

        // An Entry references Examples but never Tags, so there is no table to attach this to.
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            engine.LEngineTagAttach(entry.LEntryId, tag.LTagId, 0, LOwner.LOwnerEntry));
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            engine.LEngineTagRead(entry.LEntryId, LOwner.LOwnerEntry));
    }

    // One saved entry with a Meaning card and a Collocation card, neither carrying tags of its own, so
    // every Tag in these tests is one the seam wrote.
    private static LEntry TTagEntryCreate(LEngine engine)
    {
        return engine.LEngineEntrySave(new LEntryDraft(
            "word",
            "English",
            string.Empty,
            string.Empty,
            [new LCardDraft(string.Empty, string.Empty, "a meaning", [], [], string.Empty, [])],
            [new LCardDraft(string.Empty, "in a word", "briefly", [], [], string.Empty, [])]));
    }
}
