using Llyn.Core;
using Llyn.Infrastructure;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Database.Tests;

public sealed class TEntryUpdate
{
    [Fact]
    public void AnUpdateKeepsTheCardsTheDraftStillNamesAndRecordsEveryChange()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = new(workspace.TWorkspaceFolder);

        LEntry entry = engine.LEngineEntrySave(new LEntryDraft(
            "word",
            "English",
            string.Empty,
            string.Empty,
            [
                new LCardDraft(string.Empty, string.Empty, "first", [], [], string.Empty, [], []),
                new LCardDraft(string.Empty, string.Empty, "second", [], [], string.Empty, [], []),
                new LCardDraft(string.Empty, string.Empty, "third", [], [], string.Empty, [], []),
            ],
            []));

        LSenseArchive senses = new(workspace.TWorkspaceDatabase);
        IReadOnlyList<LSense> saved = senses.LSenseRead(entry.LEntryId);
        string firstId = saved[0].LSenseId;
        string thirdId = saved[2].LSenseId;

        LEntryDraft? loaded = engine.LEngineEntryLoad(entry.LEntryId);
        Assert.NotNull(loaded);
        LEntryDraft edited = loaded with
        {
            LEntryDraftSenses =
            [
                loaded.LEntryDraftSenses[0] with { LCardDraftMeaning = "first, reworded" },
                loaded.LEntryDraftSenses[2],
                new LCardDraft(string.Empty, string.Empty, "fourth", [], [], string.Empty, [], []),
            ],
        };

        LEntry updated = engine.LEngineEntryUpdate(entry.LEntryId, edited);

        Assert.Equal(entry.LEntryId, updated.LEntryId);
        Assert.Equal(entry.LEntryAddedUtc, updated.LEntryAddedUtc);
        Assert.NotEqual(entry.LEntryUpdatedUtc, updated.LEntryUpdatedUtc);

        IReadOnlyList<LSense> stored = senses.LSenseRead(entry.LEntryId);
        Assert.Equal(["first, reworded", "third", "fourth"], stored.Select(sense => sense.LSenseDefinition));
        Assert.Equal([0, 1, 2], stored.Select(sense => sense.LSensePosition));

        Assert.Equal(firstId, stored[0].LSenseId);
        Assert.Equal(thirdId, stored[1].LSenseId);
        Assert.DoesNotContain(stored[2].LSenseId, new[] { firstId, thirdId });

        LRevision revision = Assert.IsType<LRevision>(engine.LEngineRevisionRead());
        IReadOnlyList<LRevisionChange> changes = engine.LEngineChangeRead(revision.LRevisionId);
        Assert.Contains(changes, change =>
            change.LRevisionChangeKind == "delete" && change.LRevisionChangeSummary == "second");
        Assert.Contains(changes, change =>
            change.LRevisionChangeKind == "update" && change.LRevisionChangeTarget == firstId);
        Assert.Contains(changes, change =>
            change.LRevisionChangeKind == "update" && change.LRevisionChangeTarget == thirdId);
        Assert.Contains(changes, change =>
            change.LRevisionChangeKind == "create" && change.LRevisionChangeSummary == "fourth");

        Assert.Equal(revision.LRevisionId, engine.LEngineStateRead().LWorkspaceStateRevision);
    }

    [Fact]
    public void ACardsIndependentsAreReAttachedAndTheLastDetachLeavesTheRowStanding()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = new(workspace.TWorkspaceFolder);

        LEntry entry = engine.LEngineEntrySave(new LEntryDraft(
            "word",
            "English",
            string.Empty,
            string.Empty,
            [
                new LCardDraft(
                    string.Empty, string.Empty, "meaning", [LExampleDraft.LExampleDraftCreate("one"), LExampleDraft.LExampleDraftCreate("two")], [],
                    string.Empty, ["kept", "dropped"], []),
            ],
            []));

        LEntryDraft loaded = Assert.IsType<LEntryDraft>(engine.LEngineEntryLoad(entry.LEntryId));
        LCardDraft card = loaded.LEntryDraftSenses[0];

        engine.LEngineEntryUpdate(entry.LEntryId, loaded with
        {
            LEntryDraftSenses =
            [
                card with
                {
                    LCardDraftExample = [card.LCardDraftExample[1], card.LCardDraftExample[0]],
                    LCardDraftTag = ["kept"],
                },
            ],
        });

        LSense sense = Assert.Single(new LSenseArchive(workspace.TWorkspaceDatabase).LSenseRead(entry.LEntryId));
        LExampleLink examples = new(workspace.TWorkspaceDatabase);
        Assert.Equal(
            ["two", "one"],
            examples.LExampleSenseRead(sense.LSenseId).Select(example => example.LExampleText));

        Assert.Equal(2, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM example;"));

        LTagArchive tags = new(workspace.TWorkspaceDatabase);
        Assert.Equal(["kept"], tags.LTagSenseRead(sense.LSenseId).Select(tag => tag.LTagText));

        Assert.Equal(1, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM sense_tag;"));
        Assert.Equal(2, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM tag;"));
    }

    [Fact]
    public void AnUpdateThatFailsPartWayLeavesTheStoredEntryExactlyAsItWas()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = new(workspace.TWorkspaceFolder);

        LEntry entry = engine.LEngineEntrySave(new LEntryDraft(
            "word",
            "English",
            string.Empty,
            "a note",
            [
                new LCardDraft(string.Empty, string.Empty, "first", [], [], string.Empty, [], []),
                new LCardDraft(string.Empty, string.Empty, "second", [], [], string.Empty, [], []),
            ],
            []));

        LSenseArchive senses = new(workspace.TWorkspaceDatabase);
        IReadOnlyList<LSense> saved = senses.LSenseRead(entry.LEntryId);

        LEntryArchive entries = new(workspace.TWorkspaceDatabase);
        LEntry origin = entries.LEntryCreate(
            new LEntry(string.Empty, "origin", "English", null, null, null, null), [], []);
        LSense source = senses.LSenseCreate(
            new LSense(string.Empty, origin.LEntryId, null, 0, null, null, null, "links", string.Empty));
        new LRelationArchive(workspace.TWorkspaceDatabase).LRelationCreate(
            new LRelation(string.Empty, source.LSenseId, 0, "synonym", null, null, null, saved[1].LSenseId));

        LEntryDraft loaded = Assert.IsType<LEntryDraft>(engine.LEngineEntryLoad(entry.LEntryId));
        LRevision? before = engine.LEngineRevisionRead();

        Assert.Throws<InvalidOperationException>(() => engine.LEngineEntryUpdate(
            entry.LEntryId,
            loaded with
            {
                LEntryDraftHeadword = "rewritten",
                LEntryDraftNote = "a different note",
                LEntryDraftSenses = [loaded.LEntryDraftSenses[0] with { LCardDraftMeaning = "changed" }],
            }));

        LEntry? stored = entries.LEntryRead(entry.LEntryId);
        Assert.Equal("word", stored?.LEntryHeadword);
        Assert.Equal(entry.LEntryUpdatedUtc, stored?.LEntryUpdatedUtc);
        Assert.Equal(
            ["first", "second"],
            senses.LSenseRead(entry.LEntryId).Select(sense => sense.LSenseDefinition));
        Assert.Equal(
            "a note",
            new LNoteArchive(workspace.TWorkspaceDatabase).LNoteRead(entry.LEntryId)?.LNoteText);

        Assert.Equal(before?.LRevisionId, engine.LEngineRevisionRead()?.LRevisionId);
    }

    [Fact]
    public void AnUpdateOfAnEntryThatIsNoLongerStoredIsRefused()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = new(workspace.TWorkspaceFolder);

        LRefusal refusal = Assert.Throws<LRefusal>(() => engine.LEngineEntryUpdate(
            "no-such-entry",
            new LEntryDraft("word", "English", string.Empty, string.Empty, [], [])));

        Assert.Equal(LRefusal.LRefusalEntry, refusal.LRefusalReason);
        Assert.Equal(0, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM revision;"));
    }
}
