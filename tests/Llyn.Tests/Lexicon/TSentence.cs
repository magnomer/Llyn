using Llyn.Core;
using Llyn.Infrastructure;
using Xunit;

namespace Llyn.Tests;

public sealed class TSentence
{
    [Fact]
    public void SentenceSave_FrameAndRevision_ReadsThemBack()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        LSentenceArchive sentences = TInterface.TSentenceArchiveCreate(workspace.TWorkspaceDatabase);
        LExampleArchive examples = TInterface.TExampleArchiveCreate(workspace.TWorkspaceDatabase);

        string meaningId = TSentenceMeaningCreate(workspace);
        LExample stated = examples.TExampleCreate(
            TInterface.TExampleCreate(string.Empty, "en", "he waited for her", null, null));
        LExample rewritten = examples.TExampleCreate(
            TInterface.TExampleCreate(string.Empty, "en", "he waited for the letter", null, null));

        sentences.TSentenceMeaningSave(
            meaningId,
            [TInterface.TSentenceCreate(string.Empty, meaningId, 0, stated, rewritten, "for", "Patient")]);

        LSentence read = Assert.Single(sentences.TSentenceMeaningRead(meaningId));
        Assert.Equal(stated.LExampleId, read.LSentenceExample.LExampleId);
        Assert.Equal(rewritten.LExampleId, read.LSentenceRevision?.LExampleId);
        Assert.Equal("he waited for the letter", read.LSentenceRevision?.LExampleText.TStateValueShow());
        Assert.Equal("for", read.LSentenceParticle.TStateValueShow());
        Assert.Equal("Patient", read.LSentenceDependence.TStateValueShow());
        Assert.NotEmpty(read.LSentenceId);
    }

    [Fact]
    public void SentenceSave_SharedExample_KeepsEachMeaningsFrame()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        LSentenceArchive sentences = TInterface.TSentenceArchiveCreate(workspace.TWorkspaceDatabase);
        LExampleArchive examples = TInterface.TExampleArchiveCreate(workspace.TWorkspaceDatabase);

        string first = TSentenceMeaningCreate(workspace);
        string second = TSentenceMeaningCreate(workspace, "another meaning");
        LExample shared = examples.TExampleCreate(
            TInterface.TExampleCreate(string.Empty, "en", "she ran", null, null));

        sentences.TSentenceMeaningSave(
            first, [TInterface.TSentenceCreate(string.Empty, first, 0, shared, null, "to", "Goal")]);
        sentences.TSentenceMeaningSave(
            second, [TInterface.TSentenceCreate(string.Empty, second, 0, shared, null, "from", "Source")]);

        Assert.Equal("to", Assert.Single(sentences.TSentenceMeaningRead(first)).LSentenceParticle.TStateValueShow());
        Assert.Equal(
            "from", Assert.Single(sentences.TSentenceMeaningRead(second)).LSentenceParticle.TStateValueShow());
    }

    [Fact]
    public void SentenceSave_ManyRows_KeepsTheOrderGiven()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        LSentenceArchive sentences = TInterface.TSentenceArchiveCreate(workspace.TWorkspaceDatabase);
        LExampleArchive examples = TInterface.TExampleArchiveCreate(workspace.TWorkspaceDatabase);

        string meaningId = TSentenceMeaningCreate(workspace);
        LExample first = examples.TExampleCreate(
            TInterface.TExampleCreate(string.Empty, "en", "first", null, null));
        LExample second = examples.TExampleCreate(
            TInterface.TExampleCreate(string.Empty, "en", "second", null, null));

        sentences.TSentenceMeaningSave(
            meaningId,
            [TInterface.TSentenceCreate(string.Empty, meaningId, 0, second, null, null, null),
             TInterface.TSentenceCreate(string.Empty, meaningId, 1, first, null, null, null)]);

        Assert.Equal(
            ["second", "first"],
            sentences.TSentenceMeaningRead(meaningId)
                .Select(sentence => sentence.LSentenceExample.LExampleText.TStateValueShow()));
        Assert.Equal(
            [0, 1],
            sentences.TSentenceMeaningRead(meaningId).Select(sentence => sentence.LSentencePosition));
    }

    [Fact]
    public void SentenceDetach_MiddleRow_ClosesTheOrder()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        LSentenceArchive sentences = TInterface.TSentenceArchiveCreate(workspace.TWorkspaceDatabase);
        LExampleArchive examples = TInterface.TExampleArchiveCreate(workspace.TWorkspaceDatabase);

        string meaningId = TSentenceMeaningCreate(workspace);
        for (int position = 0; position < 3; position++)
        {
            LExample example = examples.TExampleCreate(
                TInterface.TExampleCreate(string.Empty, "en", $"sentence {position}", null, null));
            sentences.TSentenceMeaningAttach(meaningId, example.LExampleId, position);
        }

        string middle = sentences.TSentenceMeaningRead(meaningId)[1].LSentenceExample.LExampleId;
        sentences.TSentenceMeaningDetach(meaningId, middle);

        Assert.Equal(
            [0, 1],
            sentences.TSentenceMeaningRead(meaningId).Select(sentence => sentence.LSentencePosition));
    }

    [Fact]
    public void MeaningDelete_HeldSentences_LeavesNoRows()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        LSentenceArchive sentences = TInterface.TSentenceArchiveCreate(workspace.TWorkspaceDatabase);
        LExampleArchive examples = TInterface.TExampleArchiveCreate(workspace.TWorkspaceDatabase);
        LMeaningArchive meanings = TInterface.TMeaningArchiveCreate(workspace.TWorkspaceDatabase);

        string meaningId = TSentenceMeaningCreate(workspace);
        LExample example = examples.TExampleCreate(
            TInterface.TExampleCreate(string.Empty, "en", "a sentence", null, null));
        sentences.TSentenceMeaningAttach(meaningId, example.LExampleId, 0);

        meanings.TMeaningDelete(meaningId);

        Assert.Equal(0, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM sense_example;"));
        Assert.Equal(1, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM example;"));
    }

    [Fact]
    public void ExampleDelete_CitedAsRevision_ClearsTheRevision()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        LSentenceArchive sentences = TInterface.TSentenceArchiveCreate(workspace.TWorkspaceDatabase);
        LExampleArchive examples = TInterface.TExampleArchiveCreate(workspace.TWorkspaceDatabase);

        string meaningId = TSentenceMeaningCreate(workspace);
        LExample stated = examples.TExampleCreate(
            TInterface.TExampleCreate(string.Empty, "en", "he waited", null, null));
        LExample rewritten = examples.TExampleCreate(
            TInterface.TExampleCreate(string.Empty, "en", "he waited long", null, null));

        sentences.TSentenceMeaningSave(
            meaningId,
            [TInterface.TSentenceCreate(string.Empty, meaningId, 0, stated, rewritten, null, null)]);

        examples.TExampleDelete(rewritten.LExampleId, true);

        LSentence read = Assert.Single(sentences.TSentenceMeaningRead(meaningId));
        Assert.Null(read.LSentenceRevision);
        Assert.Equal(stated.LExampleId, read.LSentenceExample.LExampleId);
    }

    [Fact]
    public void SentenceSave_CollocationFrame_ReadsItBack()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        LSentenceArchive sentences = TInterface.TSentenceArchiveCreate(workspace.TWorkspaceDatabase);
        LExampleArchive examples = TInterface.TExampleArchiveCreate(workspace.TWorkspaceDatabase);

        string collocationId = TSentenceCollocationCreate(workspace);
        LExample stated = examples.TExampleCreate(
            TInterface.TExampleCreate(string.Empty, "en", "in a word, no", null, null));
        LExample rewritten = examples.TExampleCreate(
            TInterface.TExampleCreate(string.Empty, "en", "in short, no", null, null));

        sentences.TSentenceCollocationSave(
            collocationId,
            [TInterface.TSentenceCreate(string.Empty, collocationId, 0, stated, rewritten, "in", "Manner")]);

        LSentence read = Assert.Single(sentences.TSentenceCollocationRead(collocationId));
        Assert.Equal(stated.LExampleId, read.LSentenceExample.LExampleId);
        Assert.Equal("in short, no", read.LSentenceRevision?.LExampleText.TStateValueShow());
        Assert.Equal("in", read.LSentenceParticle.TStateValueShow());
        Assert.Equal("Manner", read.LSentenceDependence.TStateValueShow());
        Assert.NotEmpty(read.LSentenceId);
    }

    [Fact]
    public void SentenceSave_SharedExample_KeepsBothOwnersFrames()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        LSentenceArchive sentences = TInterface.TSentenceArchiveCreate(workspace.TWorkspaceDatabase);
        LExampleArchive examples = TInterface.TExampleArchiveCreate(workspace.TWorkspaceDatabase);

        string meaningId = TSentenceMeaningCreate(workspace);
        string collocationId = TSentenceCollocationCreate(workspace);
        LExample shared = examples.TExampleCreate(
            TInterface.TExampleCreate(string.Empty, "en", "she ran", null, null));

        sentences.TSentenceMeaningSave(
            meaningId, [TInterface.TSentenceCreate(string.Empty, meaningId, 0, shared, null, "to", "Goal")]);
        sentences.TSentenceCollocationSave(
            collocationId,
            [TInterface.TSentenceCreate(string.Empty, collocationId, 0, shared, null, "from", "Source")]);

        Assert.Equal(
            "to", Assert.Single(sentences.TSentenceMeaningRead(meaningId)).LSentenceParticle.TStateValueShow());
        Assert.Equal(
            "from",
            Assert.Single(sentences.TSentenceCollocationRead(collocationId)).LSentenceParticle.TStateValueShow());
    }

    [Fact]
    public void CollocationDelete_HeldSentences_LeavesNoRows()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        LSentenceArchive sentences = TInterface.TSentenceArchiveCreate(workspace.TWorkspaceDatabase);
        LExampleArchive examples = TInterface.TExampleArchiveCreate(workspace.TWorkspaceDatabase);
        LCollocationArchive collocations = TInterface.TCollocationArchiveCreate(workspace.TWorkspaceDatabase);

        string collocationId = TSentenceCollocationCreate(workspace);
        LExample example = examples.TExampleCreate(
            TInterface.TExampleCreate(string.Empty, "en", "a sentence", null, null));
        sentences.TSentenceCollocationAttach(collocationId, example.LExampleId, 0);

        collocations.TCollocationDelete(collocationId);

        Assert.Equal(0, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM collocation_example;"));
        Assert.Equal(1, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM example;"));
    }

    [Fact]
    public void SentenceDetach_CollocationMiddleRow_ClosesTheOrder()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        LSentenceArchive sentences = TInterface.TSentenceArchiveCreate(workspace.TWorkspaceDatabase);
        LExampleArchive examples = TInterface.TExampleArchiveCreate(workspace.TWorkspaceDatabase);

        string collocationId = TSentenceCollocationCreate(workspace);
        for (int position = 0; position < 3; position++)
        {
            LExample example = examples.TExampleCreate(
                TInterface.TExampleCreate(string.Empty, "en", $"sentence {position}", null, null));
            sentences.TSentenceCollocationAttach(collocationId, example.LExampleId, position);
        }

        string middle = sentences.TSentenceCollocationRead(collocationId)[1].LSentenceExample.LExampleId;
        sentences.TSentenceCollocationDetach(collocationId, middle);

        Assert.Equal(
            [0, 1],
            sentences.TSentenceCollocationRead(collocationId).Select(sentence => sentence.LSentencePosition));
    }

    private static string TSentenceCollocationCreate(TWorkspace workspace, string expression = "in a word")
    {
        LEntryArchive entries = TInterface.TEntryArchiveCreate(workspace.TWorkspaceDatabase);
        LCollocationArchive collocations = TInterface.TCollocationArchiveCreate(workspace.TWorkspaceDatabase);

        LEntry entry = entries.TEntryCreate(
            TInterface.TEntryCreate(string.Empty, "word", "en", null, null, null, null),
            [TInterface.TFormCreate(string.Empty, 0, "word", null, "headword")],
            []);

        return collocations.TCollocationCreate(
            TInterface.TCollocationCreate(
                string.Empty,
                entry.LEntryId,
                0,
                LStateValue.LStateValueUnspecified,
                TInterface.TStateValueCreate(expression),
                LStateValue.LStateValueUnspecified)).LCollocationId;
    }

    private static string TSentenceMeaningCreate(TWorkspace workspace, string definition = "a meaning")
    {
        LEntryArchive entries = TInterface.TEntryArchiveCreate(workspace.TWorkspaceDatabase);
        LMeaningArchive meanings = TInterface.TMeaningArchiveCreate(workspace.TWorkspaceDatabase);

        LEntry entry = entries.TEntryCreate(
            TInterface.TEntryCreate(string.Empty, "word", "en", null, null, null, null),
            [TInterface.TFormCreate(string.Empty, 0, "word", null, "headword")],
            []);

        return meanings.TMeaningCreate(
            TInterface.TMeaningCreate(
                string.Empty, entry.LEntryId, null, 0, null, definition, null, null, string.Empty)).LMeaningId;
    }
}
