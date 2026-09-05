using Llyn.Core;
using Llyn.Infrastructure;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TEntryUpdate
{
    [Fact]
    public void AnUpdateKeepsTheCardsTheDraftStillNamesAndRecordsEveryChange()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LEntry entry = engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "word",
            "English",
            string.Empty,
            string.Empty,
            [
                TInterface.TCardDraftCreate(string.Empty, string.Empty, "first", [], [], [], string.Empty, [], [], 1),
                TInterface.TCardDraftCreate(string.Empty, string.Empty, "second", [], [], [], string.Empty, [], [], 2),
                TInterface.TCardDraftCreate(string.Empty, string.Empty, "third", [], [], [], string.Empty, [], [], 3),
            ],
            []));

        LSenseArchive senses = TInterface.TSenseArchiveCreate(workspace.TWorkspaceDatabase);
        IReadOnlyList<LSense> saved = senses.TSenseRead(entry.LEntryId);
        string firstId = saved[0].LSenseId;
        string thirdId = saved[2].LSenseId;

        LEntryDraft? loaded = engine.TEngineEntryLoad(entry.LEntryId);
        Assert.NotNull(loaded);
        LEntryDraft edited = loaded with
        {
            LEntryDraftSenses =
            [
                loaded.LEntryDraftSenses[0] with { LCardDraftMeaning = "first, reworded" },
                loaded.LEntryDraftSenses[2],
                TInterface.TCardDraftCreate(string.Empty, string.Empty, "fourth", [], [], [], string.Empty, [], [], 3),
            ],
        };

        LEntry updated = engine.TEngineEntryUpdate(entry.LEntryId, edited);

        Assert.Equal(entry.LEntryId, updated.LEntryId);
        Assert.Equal(entry.LEntryAddedUtc, updated.LEntryAddedUtc);
        Assert.NotEqual(entry.LEntryUpdatedUtc, updated.LEntryUpdatedUtc);

        IReadOnlyList<LSense> stored = senses.TSenseRead(entry.LEntryId);
        Assert.Equal(["first, reworded", "third", "fourth"], stored.Select(sense => sense.LSenseDefinition));
        Assert.Equal([0, 1, 2], stored.Select(sense => sense.LSensePosition));

        Assert.Equal(firstId, stored[0].LSenseId);
        Assert.Equal(thirdId, stored[1].LSenseId);
        Assert.DoesNotContain(stored[2].LSenseId, new[] { firstId, thirdId });

        LRevision revision = Assert.IsType<LRevision>(engine.TEngineRevisionRead());
        IReadOnlyList<LRevisionChange> changes = engine.TEngineChangeRead(revision.LRevisionId);
        Assert.Contains(changes, change =>
            change.LRevisionChangeKind == "delete" && change.LRevisionChangeSummary == "second");
        Assert.Contains(changes, change =>
            change.LRevisionChangeKind == "update" && change.LRevisionChangeTarget == firstId);
        Assert.Contains(changes, change =>
            change.LRevisionChangeKind == "update" && change.LRevisionChangeTarget == thirdId);
        Assert.Contains(changes, change =>
            change.LRevisionChangeKind == "create" && change.LRevisionChangeSummary == "fourth");

        Assert.Equal(revision.LRevisionId, engine.TEngineStateRead().LWorkspaceStateRevision);
    }

    [Fact]
    public void ACardsIndependentsAreReAttachedAndTheLastDetachLeavesTheRowStanding()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LEntry entry = engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "word",
            "English",
            string.Empty,
            string.Empty,
            [
                TInterface.TCardDraftCreate(
                    string.Empty, string.Empty, "meaning", [TInterface.TExampleDraftCreate("one"), TInterface.TExampleDraftCreate("two")], [],
                    [],
                    string.Empty, ["kept", "dropped"], [], 1),
            ],
            []));

        LEntryDraft loaded = Assert.IsType<LEntryDraft>(engine.TEngineEntryLoad(entry.LEntryId));
        LCardDraft card = loaded.LEntryDraftSenses[0];

        engine.TEngineEntryUpdate(entry.LEntryId, loaded with
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

        LSense sense = Assert.Single(TInterface.TSenseArchiveCreate(workspace.TWorkspaceDatabase).TSenseRead(entry.LEntryId));
        LExampleLink examples = TInterface.TExampleLinkCreate(workspace.TWorkspaceDatabase);
        Assert.Equal(
            ["two", "one"],
            examples.TExampleSenseRead(sense.LSenseId).Select(example => example.LExampleText));

        Assert.Equal(2, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM example;"));

        LTagArchive tags = TInterface.TTagArchiveCreate(workspace.TWorkspaceDatabase);
        Assert.Equal(["kept"], tags.TTagSenseRead(sense.LSenseId).Select(tag => tag.LTagText));

        Assert.Equal(1, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM sense_tag;"));
    }

    [Fact]
    public void AnUpdateThatFailsPartWayLeavesTheStoredEntryExactlyAsItWas()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LEntry entry = engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "word",
            "English",
            string.Empty,
            "a note",
            [
                TInterface.TCardDraftCreate(string.Empty, string.Empty, "first", [], [], [], string.Empty, [], [], 1),
                TInterface.TCardDraftCreate(string.Empty, string.Empty, "second", [], [], [], string.Empty, [], [], 2),
            ],
            []));

        LSenseArchive senses = TInterface.TSenseArchiveCreate(workspace.TWorkspaceDatabase);
        IReadOnlyList<LSense> saved = senses.TSenseRead(entry.LEntryId);

        LEntryArchive entries = TInterface.TEntryArchiveCreate(workspace.TWorkspaceDatabase);
        LEntry origin = entries.TEntryCreate(
            TInterface.TEntryCreate(string.Empty, "origin", "English", null, null, null, null), [], []);
        LSense source = senses.TSenseCreate(
            TInterface.TSenseCreate(string.Empty, origin.LEntryId, null, 0, null, null, null, "links", string.Empty));
        TInterface.TRelationArchiveCreate(workspace.TWorkspaceDatabase).TRelationCreate(
            TInterface.TRelationCreate(string.Empty, source.LSenseId, 0, "synonym", null, null, null, saved[1].LSenseId));

        LEntryDraft loaded = Assert.IsType<LEntryDraft>(engine.TEngineEntryLoad(entry.LEntryId));
        LRevision? before = engine.TEngineRevisionRead();

        Assert.Throws<InvalidOperationException>(() => engine.TEngineEntryUpdate(
            entry.LEntryId,
            loaded with
            {
                LEntryDraftHeadword = "rewritten",
                LEntryDraftNote = "a different note",
                LEntryDraftSenses = [loaded.LEntryDraftSenses[0] with { LCardDraftMeaning = "changed" }],
            }));

        LEntry? stored = entries.TEntryRead(entry.LEntryId);
        Assert.Equal("word", stored?.LEntryHeadword);
        Assert.Equal(entry.LEntryUpdatedUtc, stored?.LEntryUpdatedUtc);
        Assert.Equal(
            ["first", "second"],
            senses.TSenseRead(entry.LEntryId).Select(sense => sense.LSenseDefinition));
        Assert.Equal(
            "a note",
            TInterface.TNoteArchiveCreate(workspace.TWorkspaceDatabase).TNoteRead(entry.LEntryId)?.LNoteText);

        Assert.Equal(before?.LRevisionId, engine.TEngineRevisionRead()?.LRevisionId);
    }

    [Fact]
    public void AnUpdateOfAnEntryThatIsNoLongerStoredIsRefused()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LRefusal refusal = Assert.Throws<LRefusal>(() => engine.TEngineEntryUpdate(
            "no-such-entry",
            TInterface.TEntryDraftCreate("word", "English", string.Empty, string.Empty, [], [])));

        Assert.Equal(LRefusal.LRefusalEntry, refusal.LRefusalReason);
        Assert.Equal(0, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM revision;"));
    }
}
