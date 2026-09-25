using System.Threading;
using Llyn.Core;
using Llyn.Infrastructure;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TEntryUpdate
{
    [Fact]
    public void EntryUpdate_DraftNamesItsCards_KeepsThemRecordsChanges()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LEntry entry = engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "word",
            "English",
            string.Empty,
            string.Empty,
            [
                TInterface.TCardDraftCreate(string.Empty, string.Empty, "first", [], [], [], [], [], 1),
                TInterface.TCardDraftCreate(string.Empty, string.Empty, "second", [], [], [], [], [], 2),
                TInterface.TCardDraftCreate(string.Empty, string.Empty, "third", [], [], [], [], [], 3),
            ],
            []));

        LMeaningArchive meanings = TInterface.TMeaningArchiveCreate(workspace.TWorkspaceDatabase);
        IReadOnlyList<LMeaning> saved = meanings.TMeaningRead(entry.LEntryId);
        long firstId = saved[0].LMeaningId;
        long thirdId = saved[2].LMeaningId;

        LEntryDraft? loaded = engine.TEngineEntryLoad(entry.LEntryId);
        Assert.NotNull(loaded);
        LEntryDraft edited = loaded with
        {
            LEntryDraftMeanings =
            [
                loaded.LEntryDraftMeanings[0] with { LCardDraftMeaning = "first, reworded" },
                loaded.LEntryDraftMeanings[2],
                TInterface.TCardDraftCreate(string.Empty, string.Empty, "fourth", [], [], [], [], [], 3),
            ],
        };

        LEntry updated = engine.TEngineEntryUpdate(entry.LEntryId, edited);

        Assert.Equal(entry.LEntryId, updated.LEntryId);
        Assert.Equal(entry.LEntryAddedUtc, updated.LEntryAddedUtc);
        Assert.NotEqual(entry.LEntryUpdatedUtc, updated.LEntryUpdatedUtc);

        IReadOnlyList<LMeaning> stored = meanings.TMeaningRead(entry.LEntryId);
        Assert.Equal(["first, reworded", "third", "fourth"], stored.Select(meaning => meaning.LMeaningDefinition));
        Assert.Equal([0, 1, 2], stored.Select(meaning => meaning.LMeaningPosition));

        Assert.Equal(firstId, stored[0].LMeaningId);
        Assert.Equal(thirdId, stored[1].LMeaningId);
        Assert.DoesNotContain(stored[2].LMeaningId, new[] { firstId, thirdId });

        long revision = Assert.IsType<long>(engine.TEngineRevisionRead());
        IReadOnlyList<LRevisionChange> changes = workspace.TRevisionChangeRead(revision);
        Assert.Contains(changes, change =>
            change.LRevisionChangeKind == "delete" && change.LRevisionChangeSummary == "second");
        Assert.Contains(changes, change =>
            change.LRevisionChangeKind == "update" && change.LRevisionChangeTarget == firstId);
        Assert.Contains(changes, change =>
            change.LRevisionChangeKind == "update" && change.LRevisionChangeTarget == thirdId);
        Assert.Contains(changes, change =>
            change.LRevisionChangeKind == "create" && change.LRevisionChangeSummary == "fourth");

        Assert.Equal(revision, engine.TEngineStateRead().LWorkspaceStateRevision);
    }

    [Fact]
    public void EntryUpdate_SameDraft_KeepsUpdatedStamp()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LEntry entry = engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "word",
            "English",
            string.Empty,
            string.Empty,
            [TInterface.TCardDraftCreate(string.Empty, string.Empty, "first", [], [], [], [], [], 1)],
            []));

        LEntryDraft? loaded = engine.TEngineEntryLoad(entry.LEntryId);
        Assert.NotNull(loaded);

        Thread.Sleep(20);
        LEntry updated = engine.TEngineEntryUpdate(entry.LEntryId, loaded);

        Assert.Equal(entry.LEntryUpdatedUtc, updated.LEntryUpdatedUtc);
    }

    [Fact]
    public void EntryUpdate_MeaningChangedAlone_StampsEntry()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LEntry entry = engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "word",
            "English",
            string.Empty,
            string.Empty,
            [
                TInterface.TCardDraftCreate(string.Empty, string.Empty, "first", [], [], [], [], [], 1),
                TInterface.TCardDraftCreate(string.Empty, string.Empty, "second", [], [], [], [], [], 2),
            ],
            []));

        LEntryDraft? loaded = engine.TEngineEntryLoad(entry.LEntryId);
        Assert.NotNull(loaded);
        LEntryDraft edited = loaded with
        {
            LEntryDraftMeanings =
            [
                loaded.LEntryDraftMeanings[0] with { LCardDraftMeaning = "first, reworded" },
                loaded.LEntryDraftMeanings[1],
            ],
        };

        Thread.Sleep(20);
        LEntry updated = engine.TEngineEntryUpdate(entry.LEntryId, edited);

        Assert.Equal("word", updated.LEntryHeadword);
        Assert.NotEqual(entry.LEntryUpdatedUtc, updated.LEntryUpdatedUtc);
    }

    [Fact]
    public void EntryUpdate_IndependentReAttached_KeepsRowOnLastDetach()
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
                    string.Empty, string.Empty, "meaning",
                    [TInterface.TSentenceDraftCreate("one"), TInterface.TSentenceDraftCreate("two")],
                    [], [], ["kept", "dropped"], [], 1),
            ],
            []));

        LEntryDraft loaded = Assert.IsType<LEntryDraft>(engine.TEngineEntryLoad(entry.LEntryId));
        LCardDraft card = loaded.LEntryDraftMeanings[0];

        engine.TEngineEntryUpdate(entry.LEntryId, loaded with
        {
            LEntryDraftMeanings =
            [
                card with
                {
                    LCardDraftSentence = [card.LCardDraftSentence[1], card.LCardDraftSentence[0]],
                    LCardDraftTag = TInterface.TTagDraftCreate("kept"),
                },
            ],
        });

        LMeaning meaning = Assert.Single(
            TInterface.TMeaningArchiveCreate(workspace.TWorkspaceDatabase).TMeaningRead(entry.LEntryId));
        LSentenceArchive examples = TInterface.TSentenceArchiveCreate(workspace.TWorkspaceDatabase);
        Assert.Equal(
            ["two", "one"],
            examples.TSentenceMeaningRead(meaning.LMeaningId)
                .Select(sentence => sentence.LSentenceExample!.LExampleText));

        Assert.Equal(2, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM example;"));

        LTagArchive tags = TInterface.TTagArchiveCreate(workspace.TWorkspaceDatabase);
        Assert.Equal(["kept"], tags.TTagMeaningRead(meaning.LMeaningId).Select(tag => tag.LTagText));

        Assert.Equal(1, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM sense_tag;"));
    }

    [Fact]
    public void EntryUpdate_FailurePartWay_LeavesEntryUnchanged()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LEntry entry = engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "word",
            "English",
            string.Empty,
            "a note",
            [
                TInterface.TCardDraftCreate(string.Empty, string.Empty, "first", [], [], [], [], [], 1),
                TInterface.TCardDraftCreate(string.Empty, string.Empty, "second", [], [], [], [], [], 2),
            ],
            []));

        LMeaningArchive meanings = TInterface.TMeaningArchiveCreate(workspace.TWorkspaceDatabase);
        LEntryArchive entries = TInterface.TEntryArchiveCreate(workspace.TWorkspaceDatabase);
        LEntryDraft loaded = Assert.IsType<LEntryDraft>(engine.TEngineEntryLoad(entry.LEntryId));
        long? before = engine.TEngineRevisionRead();

        LRefusal refusal = Assert.Throws<LRefusal>(() => engine.TEngineEntryUpdate(
            entry.LEntryId,
            loaded with
            {
                LEntryDraftHeadword = "rewritten",
                LEntryDraftNote = "a different note",
                LEntryDraftMeanings =
                [
                    loaded.LEntryDraftMeanings[0] with
                    {
                        LCardDraftMeaning = "changed",
                        LCardDraftTag = [TInterface.TTagDraftCreate("stale")[0] with { LTagDraftId = 9999 }],
                    },
                ],
            }));
        Assert.Equal(LRefusal.LRefusalLink, refusal.LRefusalReason);

        LEntry? stored = entries.TEntryRead(entry.LEntryId);
        Assert.Equal("word", stored?.LEntryHeadword);
        Assert.Equal(entry.LEntryUpdatedUtc, stored?.LEntryUpdatedUtc);
        Assert.Equal(
            ["first", "second"],
            meanings.TMeaningRead(entry.LEntryId).Select(meaning => meaning.LMeaningDefinition));
        Assert.Equal(
            "a note",
            TInterface.TNoteArchiveCreate(workspace.TWorkspaceDatabase).TNoteRead(entry.LEntryId)?.LNoteText);

        Assert.Equal(before, engine.TEngineRevisionRead());
    }

    [Fact]
    public void EntryUpdate_EntryNoLongerStored_Refuses()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LRefusal refusal = Assert.Throws<LRefusal>(() => engine.TEngineEntryUpdate(
            9999,
            TInterface.TEntryDraftCreate("word", "English", string.Empty, string.Empty, [], [])));

        Assert.Equal(LRefusal.LRefusalEntry, refusal.LRefusalReason);
        Assert.Equal(0, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM revision;"));
    }
}
