using System;
using System.Linq;
using Llyn.Core;
using Llyn.Infrastructure;
using Microsoft.Data.Sqlite;
using Xunit;

namespace Llyn.Tests;

public sealed class TMention
{
    [Fact]
    public void ExampleCreate_TwoMentions_ReadsBackSortedByStart()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        LExampleArchive examples = TInterface.TExampleArchiveCreate(workspace.TWorkspaceDatabase);
        LMentionArchive mentions = TInterface.TMentionArchiveCreate(workspace.TWorkspaceDatabase);

        (long entryId, long meaningId) = TMentionEntryCreate(workspace);
        LExample stored = examples.TExampleCreate(TInterface.TExampleCreate(
            0, "en", "he said the word", null, null) with
        {
            LExampleMention =
            [
                TInterface.TMentionCreate(0, 12, 4, entryId, meaningId),
                TInterface.TMentionCreate(0, 3, 4, entryId),
            ],
        });

        Assert.Equal([3, 12], stored.LExampleMention.Select(mention => mention.LMentionOffset));
        Assert.All(stored.LExampleMention, mention => Assert.NotEqual(0, mention.LMentionId));

        LExample read = examples.TExampleRead(stored.LExampleId)!;
        Assert.Equal([3, 12], read.LExampleMention.Select(mention => mention.LMentionOffset));
        Assert.Equal([4, 4], read.LExampleMention.Select(mention => mention.LMentionLength));
        Assert.Equal(entryId, read.LExampleMention[1].LMentionEntryId);
        Assert.Equal(meaningId, read.LExampleMention[1].LMentionSenseId);
        Assert.Equal(0, read.LExampleMention[0].LMentionSenseId);
        Assert.Equal(read.LExampleMention, mentions.TMentionRead(stored.LExampleId));
        Assert.Equal(read.LExampleMention, Assert.Single(examples.TExampleRead()).LExampleMention);
    }

    [Fact]
    public void MentionSave_OverlappingPair_Refuses()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        LExampleArchive examples = TInterface.TExampleArchiveCreate(workspace.TWorkspaceDatabase);
        LMentionArchive mentions = TInterface.TMentionArchiveCreate(workspace.TWorkspaceDatabase);

        (long entryId, _) = TMentionEntryCreate(workspace);
        LExample stored = examples.TExampleCreate(
            TInterface.TExampleCreate(0, "en", "he said the word", null, null));

        Assert.Throws<InvalidOperationException>(() => mentions.TMentionSave(
            stored.LExampleId,
            [TInterface.TMentionCreate(0, 3, 4, entryId), TInterface.TMentionCreate(0, 5, 3, entryId)]));
        Assert.Empty(mentions.TMentionRead(stored.LExampleId));
    }

    [Fact]
    public void MentionSave_SpanPastText_Refuses()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        LExampleArchive examples = TInterface.TExampleArchiveCreate(workspace.TWorkspaceDatabase);
        LMentionArchive mentions = TInterface.TMentionArchiveCreate(workspace.TWorkspaceDatabase);

        (long entryId, _) = TMentionEntryCreate(workspace);
        LExample stored = examples.TExampleCreate(
            TInterface.TExampleCreate(0, "en", "he said the word", null, null));

        Assert.Throws<InvalidOperationException>(() => mentions.TMentionSave(
            stored.LExampleId, [TInterface.TMentionCreate(0, 12, 5, entryId)]));
        mentions.TMentionSave(stored.LExampleId, [TInterface.TMentionCreate(0, 12, 4, entryId)]);
        Assert.Single(mentions.TMentionRead(stored.LExampleId));
    }

    [Fact]
    public void MentionSave_SenseWithoutEntry_RefusesAtCheck()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        LExampleArchive examples = TInterface.TExampleArchiveCreate(workspace.TWorkspaceDatabase);
        LMentionArchive mentions = TInterface.TMentionArchiveCreate(workspace.TWorkspaceDatabase);

        (_, long meaningId) = TMentionEntryCreate(workspace);
        LExample stored = examples.TExampleCreate(
            TInterface.TExampleCreate(0, "en", "he said the word", null, null));

        Assert.Throws<SqliteException>(() => mentions.TMentionSave(
            stored.LExampleId, [TInterface.TMentionCreate(0, 3, 4, 0, meaningId)]));
        mentions.TMentionSave(stored.LExampleId, [TInterface.TMentionCreate(0, 3, 4, 0)]);
        Assert.Equal(0, Assert.Single(mentions.TMentionRead(stored.LExampleId)).LMentionEntryId);
    }

    [Fact]
    public void EntryDelete_MentionedEntry_DropsTheMention()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        LExampleArchive examples = TInterface.TExampleArchiveCreate(workspace.TWorkspaceDatabase);
        LMentionArchive mentions = TInterface.TMentionArchiveCreate(workspace.TWorkspaceDatabase);
        LEntryArchive entries = TInterface.TEntryArchiveCreate(workspace.TWorkspaceDatabase);

        (long entryId, long meaningId) = TMentionEntryCreate(workspace);
        LExample stored = examples.TExampleCreate(
            TInterface.TExampleCreate(0, "en", "he said the word", null, null));
        mentions.TMentionSave(stored.LExampleId, [TInterface.TMentionCreate(0, 12, 4, entryId, meaningId)]);

        entries.TEntryDelete(entryId);

        Assert.Empty(mentions.TMentionRead(stored.LExampleId));
        Assert.NotNull(examples.TExampleRead(stored.LExampleId));
    }

    [Fact]
    public void MeaningDelete_MentionedSense_KeepsTheEntry()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        LExampleArchive examples = TInterface.TExampleArchiveCreate(workspace.TWorkspaceDatabase);
        LMentionArchive mentions = TInterface.TMentionArchiveCreate(workspace.TWorkspaceDatabase);
        LMeaningArchive meanings = TInterface.TMeaningArchiveCreate(workspace.TWorkspaceDatabase);

        (long entryId, long meaningId) = TMentionEntryCreate(workspace);
        LExample stored = examples.TExampleCreate(
            TInterface.TExampleCreate(0, "en", "he said the word", null, null));
        long mentionId = Assert.Single(mentions.TMentionSave(
            stored.LExampleId, [TInterface.TMentionCreate(0, 12, 4, entryId, meaningId)]));

        meanings.TMeaningDelete(meaningId);

        LMention kept = Assert.Single(mentions.TMentionRead(stored.LExampleId));
        Assert.Equal(mentionId, kept.LMentionId);
        Assert.Equal(entryId, kept.LMentionEntryId);
        Assert.Equal(0, kept.LMentionSenseId);
    }

    [Fact]
    public void ExampleTextUpdate_ShorterText_DropsOnlyTheSpanPastTheEnd()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        LExampleArchive examples = TInterface.TExampleArchiveCreate(workspace.TWorkspaceDatabase);
        LMentionArchive mentions = TInterface.TMentionArchiveCreate(workspace.TWorkspaceDatabase);

        (long entryId, _) = TMentionEntryCreate(workspace);
        LExample stored = examples.TExampleCreate(
            TInterface.TExampleCreate(0, "en", "he said the word", null, null));
        mentions.TMentionSave(
            stored.LExampleId,
            [TInterface.TMentionCreate(0, 3, 4, entryId), TInterface.TMentionCreate(0, 12, 4, entryId)]);

        examples.TExampleTextUpdate(stored.LExampleId, TInterface.TStateValueCreate("he said the"));

        LMention kept = Assert.Single(mentions.TMentionRead(stored.LExampleId));
        Assert.Equal(3, kept.LMentionOffset);
    }

    [Fact]
    public void SentenceMeaningRead_MentionedExamples_FillsEveryRow()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        LExampleArchive examples = TInterface.TExampleArchiveCreate(workspace.TWorkspaceDatabase);
        LMentionArchive mentions = TInterface.TMentionArchiveCreate(workspace.TWorkspaceDatabase);
        LSentenceArchive sentences = TInterface.TSentenceArchiveCreate(workspace.TWorkspaceDatabase);

        (long entryId, long meaningId) = TMentionEntryCreate(workspace);
        LExample first = examples.TExampleCreate(
            TInterface.TExampleCreate(0, "en", "he said the word", null, null));
        LExample second = examples.TExampleCreate(
            TInterface.TExampleCreate(0, "en", "a word of advice", null, null));
        mentions.TMentionSave(first.LExampleId, [TInterface.TMentionCreate(0, 12, 4, entryId)]);
        mentions.TMentionSave(second.LExampleId, [TInterface.TMentionCreate(0, 2, 4, entryId, meaningId)]);

        sentences.TSentenceMeaningSave(
            meaningId,
            [
                TInterface.TSentenceCreate(0, first, null, null),
                TInterface.TSentenceCreate(0, null, "for", null),
                TInterface.TSentenceCreate(0, second, null, null),
            ]);

        IReadOnlyList<LSentence> read = sentences.TSentenceMeaningRead(meaningId);
        Assert.Equal(3, read.Count);
        Assert.Equal(12, Assert.Single(read[0].LSentenceExample!.LExampleMention).LMentionOffset);
        Assert.Null(read[1].LSentenceExample);
        Assert.Equal(meaningId, Assert.Single(read[2].LSentenceExample!.LExampleMention).LMentionSenseId);
    }

    private static (long TMentionEntry, long TMentionMeaning) TMentionEntryCreate(TWorkspace workspace)
    {
        LEntryArchive entries = TInterface.TEntryArchiveCreate(workspace.TWorkspaceDatabase);
        LMeaningArchive meanings = TInterface.TMeaningArchiveCreate(workspace.TWorkspaceDatabase);

        LEntry entry = entries.TEntryCreate(
            TInterface.TEntryCreate(0, "word", "en", 0, null, null),
            [TInterface.TFormCreate(0, 0, "word", null, "headword")],
            []);
        LMeaning meaning = meanings.TMeaningCreate(
            TInterface.TMeaningCreate(0, entry.LEntryId, null, 0, null, "a unit of language"));

        return (entry.LEntryId, meaning.LMeaningId);
    }
    [Fact]
    public void MentionDraftResolve_NegativeId_KeepsTheSpan()
    {
        LMention resolved = TInterface.TMentionDraftResolve(TInterface.TMentionDraftCreate(-3, 12, 4, 7, 9));

        Assert.Equal(-3, resolved.LMentionId);
        Assert.Equal((12, 4), (resolved.LMentionOffset, resolved.LMentionLength));
        Assert.Equal((7L, 9L), (resolved.LMentionEntryId, resolved.LMentionSenseId));
    }
}
