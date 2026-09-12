using Llyn.Core;
using Llyn.Infrastructure;
using Xunit;

namespace Llyn.Tests;

public sealed class TSentence
{
    [Fact]
    public void SentenceSave_Frame_ReadsItBack()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        LSentenceArchive sentences = TInterface.TSentenceArchiveCreate(workspace.TWorkspaceDatabase);
        LExampleArchive examples = TInterface.TExampleArchiveCreate(workspace.TWorkspaceDatabase);

        long meaningId = TSentenceMeaningCreate(workspace);
        LExample stated = examples.TExampleCreate(
            TInterface.TExampleCreate(
            0, "en", "he waited for her", null, null));

        sentences.TSentenceMeaningSave(
            meaningId,
            [TInterface.TSentenceCreate(0, meaningId, 0, stated, "for", "Patient")]);

        LSentence read = Assert.Single(sentences.TSentenceMeaningRead(meaningId));
        Assert.Equal(stated.LExampleId, read.LSentenceExample.LExampleId);
        Assert.Equal("for", read.LSentenceParticle.TStateValueShow());
        Assert.Equal("Patient", read.LSentenceDependence.TStateValueShow());
        Assert.NotEqual(0, read.LSentenceId);
    }

    [Fact]
    public void SentenceSave_FrameWithNoExample_ReadsItBack()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        LSentenceArchive sentences = TInterface.TSentenceArchiveCreate(workspace.TWorkspaceDatabase);

        long meaningId = TSentenceMeaningCreate(workspace);

        sentences.TSentenceMeaningSave(
            meaningId,
            [TInterface.TSentenceCreate(0, meaningId, 0, null, "for", "Patient")]);

        LSentence read = Assert.Single(sentences.TSentenceMeaningRead(meaningId));
        Assert.Null(read.LSentenceExample);
        Assert.Equal("for", read.LSentenceParticle.TStateValueShow());
        Assert.Equal("Patient", read.LSentenceDependence.TStateValueShow());
        Assert.NotEqual(0, read.LSentenceId);
    }

    [Fact]
    public void SentenceFrameRead_SavedRows_OffersWhatTheLanguageHolds()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        LSentenceArchive sentences = TInterface.TSentenceArchiveCreate(workspace.TWorkspaceDatabase);

        Assert.Empty(sentences.TSentenceParticleRead("en"));
        Assert.Empty(sentences.TSentenceDependenceRead("en"));

        long english = TSentenceMeaningCreate(workspace);
        long japanese = TSentenceMeaningCreate(workspace, "another meaning", "ja");

        sentences.TSentenceMeaningSave(
            english,
            [TInterface.TSentenceCreate(0, english, 0, null, "for", "Patient")]);
        sentences.TSentenceMeaningSave(
            japanese,
            [TInterface.TSentenceCreate(0, japanese, 0, null, "を", "Object")]);

        Assert.Equal(["for"], sentences.TSentenceParticleRead("en"));
        Assert.Equal(["Patient"], sentences.TSentenceDependenceRead("en"));
        Assert.Equal(["を"], sentences.TSentenceParticleRead("ja"));
        Assert.Equal(["Object"], sentences.TSentenceDependenceRead("ja"));
    }

    [Fact]
    public void SentenceSave_SharedExample_KeepsEachMeaningsFrame()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        LSentenceArchive sentences = TInterface.TSentenceArchiveCreate(workspace.TWorkspaceDatabase);
        LExampleArchive examples = TInterface.TExampleArchiveCreate(workspace.TWorkspaceDatabase);

        long first = TSentenceMeaningCreate(workspace);
        long second = TSentenceMeaningCreate(workspace, "another meaning");
        LExample shared = examples.TExampleCreate(
            TInterface.TExampleCreate(
            0, "en", "she ran", null, null));

        sentences.TSentenceMeaningSave(
            first, [TInterface.TSentenceCreate(0, first, 0, shared, "to", "Goal")]);
        sentences.TSentenceMeaningSave(
            second, [TInterface.TSentenceCreate(0, second, 0, shared, "from", "Source")]);

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

        long meaningId = TSentenceMeaningCreate(workspace);
        LExample first = examples.TExampleCreate(
            TInterface.TExampleCreate(
            0, "en", "first", null, null));
        LExample second = examples.TExampleCreate(
            TInterface.TExampleCreate(
            0, "en", "second", null, null));

        sentences.TSentenceMeaningSave(
            meaningId,
            [TInterface.TSentenceCreate(0, meaningId, 0, second, null, null),
             TInterface.TSentenceCreate(0, meaningId, 1, first, null, null)]);

        Assert.Equal(
            ["second", "first"],
            sentences.TSentenceMeaningRead(meaningId)
                .Select(sentence => sentence.LSentenceExample.LExampleText.TStateValueShow()));
        Assert.Equal(
            [0, 1],
            sentences.TSentenceMeaningRead(meaningId).Select(sentence => sentence.LSentencePosition));
    }

    [Fact]
    public void SentenceSave_RowNamedAgain_KeepsItsId()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        LSentenceArchive sentences = TInterface.TSentenceArchiveCreate(workspace.TWorkspaceDatabase);
        LExampleArchive examples = TInterface.TExampleArchiveCreate(workspace.TWorkspaceDatabase);

        long meaningId = TSentenceMeaningCreate(workspace);
        LExample first = examples.TExampleCreate(
            TInterface.TExampleCreate(
            0, "en", "first", null, null));
        LExample second = examples.TExampleCreate(
            TInterface.TExampleCreate(
            0, "en", "second", null, null));

        IReadOnlyList<long> written = sentences.TSentenceMeaningSave(
            meaningId,
            [TInterface.TSentenceCreate(0, meaningId, 0, first, null, null),
             TInterface.TSentenceCreate(0, meaningId, 1, second, null, null)]);

        IReadOnlyList<long> rewritten = sentences.TSentenceMeaningSave(
            meaningId,
            [TInterface.TSentenceCreate(written[1], meaningId, 0, second, "for", null),
             TInterface.TSentenceCreate(0, meaningId, 1, first, null, null)]);

        Assert.Equal(written[1], rewritten[0]);
        Assert.NotEqual(written[0], rewritten[1]);
        IReadOnlyList<LSentence> read = sentences.TSentenceMeaningRead(meaningId);
        Assert.Equal([written[1], rewritten[1]], read.Select(sentence => sentence.LSentenceId));
        Assert.Equal("for", read[0].LSentenceParticle.TStateValueShow());
        Assert.Equal(2, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM sense_example;"));
    }

    [Fact]
    public void SentenceDetach_MiddleRow_ClosesTheOrder()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        LSentenceArchive sentences = TInterface.TSentenceArchiveCreate(workspace.TWorkspaceDatabase);
        LExampleArchive examples = TInterface.TExampleArchiveCreate(workspace.TWorkspaceDatabase);

        long meaningId = TSentenceMeaningCreate(workspace);
        for (int position = 0; position < 3; position++)
        {
            LExample example = examples.TExampleCreate(
                TInterface.TExampleCreate(
            0, "en", $"sentence {position}", null, null));
            sentences.TSentenceMeaningAttach(meaningId, example.LExampleId, position);
        }

        long middle = sentences.TSentenceMeaningRead(meaningId)[1].LSentenceExample.LExampleId;
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

        long meaningId = TSentenceMeaningCreate(workspace);
        LExample example = examples.TExampleCreate(
            TInterface.TExampleCreate(
            0, "en", "a sentence", null, null));
        sentences.TSentenceMeaningAttach(meaningId, example.LExampleId, 0);

        meanings.TMeaningDelete(meaningId);

        Assert.Equal(0, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM sense_example;"));
        Assert.Equal(1, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM example;"));
    }

    [Fact]
    public void SentenceSave_CollocationFrame_ReadsItBack()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        LSentenceArchive sentences = TInterface.TSentenceArchiveCreate(workspace.TWorkspaceDatabase);
        LExampleArchive examples = TInterface.TExampleArchiveCreate(workspace.TWorkspaceDatabase);

        long collocationId = TSentenceCollocationCreate(workspace);
        LExample stated = examples.TExampleCreate(
            TInterface.TExampleCreate(
            0, "en", "in a word, no", null, null));

        sentences.TSentenceCollocationSave(
            collocationId,
            [TInterface.TSentenceCreate(0, collocationId, 0, stated, "in", "Manner")]);

        LSentence read = Assert.Single(sentences.TSentenceCollocationRead(collocationId));
        Assert.Equal(stated.LExampleId, read.LSentenceExample.LExampleId);
        Assert.Equal("in", read.LSentenceParticle.TStateValueShow());
        Assert.Equal("Manner", read.LSentenceDependence.TStateValueShow());
        Assert.NotEqual(0, read.LSentenceId);
    }

    [Fact]
    public void SentenceSave_SharedExample_KeepsBothOwnersFrames()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        LSentenceArchive sentences = TInterface.TSentenceArchiveCreate(workspace.TWorkspaceDatabase);
        LExampleArchive examples = TInterface.TExampleArchiveCreate(workspace.TWorkspaceDatabase);

        long meaningId = TSentenceMeaningCreate(workspace);
        long collocationId = TSentenceCollocationCreate(workspace);
        LExample shared = examples.TExampleCreate(
            TInterface.TExampleCreate(
            0, "en", "she ran", null, null));

        sentences.TSentenceMeaningSave(
            meaningId, [TInterface.TSentenceCreate(0, meaningId, 0, shared, "to", "Goal")]);
        sentences.TSentenceCollocationSave(
            collocationId,
            [TInterface.TSentenceCreate(0, collocationId, 0, shared, "from", "Source")]);

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

        long collocationId = TSentenceCollocationCreate(workspace);
        LExample example = examples.TExampleCreate(
            TInterface.TExampleCreate(
            0, "en", "a sentence", null, null));
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

        long collocationId = TSentenceCollocationCreate(workspace);
        for (int position = 0; position < 3; position++)
        {
            LExample example = examples.TExampleCreate(
                TInterface.TExampleCreate(
            0, "en", $"sentence {position}", null, null));
            sentences.TSentenceCollocationAttach(collocationId, example.LExampleId, position);
        }

        long middle = sentences.TSentenceCollocationRead(collocationId)[1].LSentenceExample.LExampleId;
        sentences.TSentenceCollocationDetach(collocationId, middle);

        Assert.Equal(
            [0, 1],
            sentences.TSentenceCollocationRead(collocationId).Select(sentence => sentence.LSentencePosition));
    }

    private static long TSentenceCollocationCreate(TWorkspace workspace, string expression = "in a word")
    {
        LEntryArchive entries = TInterface.TEntryArchiveCreate(workspace.TWorkspaceDatabase);
        LCollocationArchive collocations = TInterface.TCollocationArchiveCreate(workspace.TWorkspaceDatabase);

        LEntry entry = entries.TEntryCreate(
            TInterface.TEntryCreate(0, "word", "en", null, null, null, null),
            [TInterface.TFormCreate(0, 0, "word", null, "headword")],
            []);

        return collocations.TCollocationCreate(
            TInterface.TCollocationCreate(
                0,
                entry.LEntryId,
                0,
                LStateValue.LStateValueUnspecified,
                TInterface.TStateValueCreate(expression),
                LStateValue.LStateValueUnspecified)).LCollocationId;
    }

    private static long TSentenceMeaningCreate(
        TWorkspace workspace, string definition = "a meaning", string language = "en")
    {
        LEntryArchive entries = TInterface.TEntryArchiveCreate(workspace.TWorkspaceDatabase);
        LMeaningArchive meanings = TInterface.TMeaningArchiveCreate(workspace.TWorkspaceDatabase);

        LEntry entry = entries.TEntryCreate(
            TInterface.TEntryCreate(0, "word", language, null, null, null, null),
            [TInterface.TFormCreate(0, 0, "word", null, "headword")],
            []);

        return meanings.TMeaningCreate(
            TInterface.TMeaningCreate(
                0, entry.LEntryId, null, 0, null, definition)).LMeaningId;
    }
}
