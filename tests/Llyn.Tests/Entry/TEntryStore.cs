using Llyn.Core;
using Llyn.Infrastructure;
using Xunit;

namespace Llyn.Tests;

public sealed class TEntryStore
{
    [Fact]
    public void EntryRead_StoredEntry_ReturnsFormsAndSpeeches()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        LEntryArchive entries = TInterface.TEntryArchiveCreate(workspace.TWorkspaceDatabase);

        LEntry stored = entries.TEntryCreate(
            TInterface.TEntryCreate(0, "word", "en", null, null, null, null),
            [TInterface.TFormCreate(0, 0, "word", null, "headword"),
             TInterface.TFormCreate(0, 0, "words", null, "plural")],
            [TInterface.TSpeechCreate(0, 0, "noun")]);

        Assert.NotEqual(0, stored.LEntryId);
        Assert.Equal("word", entries.TEntryRead(stored.LEntryId)?.LEntryHeadword);
        Assert.Equal([0, 1], entries.TEntryFormRead(stored.LEntryId).Select(form => form.LFormPosition));
        Assert.Single(entries.TEntrySpeechRead(stored.LEntryId));
    }

    [Fact]
    public void EntryUpdate_UnknownEntry_Throws()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        LEntryArchive entries = TInterface.TEntryArchiveCreate(workspace.TWorkspaceDatabase);

        Assert.Throws<InvalidOperationException>(
            () => entries.TEntryUpdate(TInterface.TEntryCreate(9999, "word", "en", null, null, null, null)));
    }

    [Fact]
    public void EntryDelete_OwnedAndReferencedRows_RemovesOnlyOwned()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        LEntryArchive entries = TInterface.TEntryArchiveCreate(workspace.TWorkspaceDatabase);
        LMeaningArchive meanings = TInterface.TMeaningArchiveCreate(workspace.TWorkspaceDatabase);
        LExampleArchive examples = TInterface.TExampleArchiveCreate(workspace.TWorkspaceDatabase);
        LSentenceArchive links = TInterface.TSentenceArchiveCreate(workspace.TWorkspaceDatabase);

        LEntry entry = entries.TEntryCreate(
            TInterface.TEntryCreate(0, "word", "en", null, null, null, null),
            [TInterface.TFormCreate(0, 0, "word", null, "headword")],
            []);
        LMeaning meaning = meanings.TMeaningCreate(
            TInterface.TMeaningCreate(0, entry.LEntryId, null, 0, null, "a meaning", null, null, string.Empty));
        LExample example = examples.TExampleCreate(
            TInterface.TExampleCreate(
            0, "en", "a sentence", null, null));
        links.TSentenceMeaningAttach(meaning.LMeaningId, example.LExampleId, 0);

        entries.TEntryDelete(entry.LEntryId);

        Assert.Null(entries.TEntryRead(entry.LEntryId));
        Assert.Equal(0, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM form;"));
        Assert.Equal(0, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM sense;"));
        Assert.Equal(0, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM sense_example;"));

        Assert.NotNull(examples.TExampleRead(example.LExampleId));
    }

    [Fact]
    public void EntryDelete_LinkedFromAnotherEntry_Throws()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        LEntryArchive entries = TInterface.TEntryArchiveCreate(workspace.TWorkspaceDatabase);
        LMeaningArchive meanings = TInterface.TMeaningArchiveCreate(workspace.TWorkspaceDatabase);
        LRelationArchive relations = TInterface.TRelationArchiveCreate(workspace.TWorkspaceDatabase);

        LEntry target = entries.TEntryCreate(
            TInterface.TEntryCreate(0, "target", "en", null, null, null, null), [], []);
        LEntry origin = entries.TEntryCreate(
            TInterface.TEntryCreate(0, "origin", "en", null, null, null, null), [], []);
        LMeaning meaning = meanings.TMeaningCreate(
            TInterface.TMeaningCreate(0, origin.LEntryId, null, 0, null, null, null, null, string.Empty));
        relations.TRelationCreate(
            TInterface.TRelationCreate(0, meaning.LMeaningId, 0, "synonym", null, null, target.LEntryId, null));

        InvalidOperationException error =
            Assert.Throws<InvalidOperationException>(() => entries.TEntryDelete(target.LEntryId));

        Assert.Contains("lexical link", error.Message, StringComparison.Ordinal);
        Assert.NotNull(entries.TEntryRead(target.LEntryId));
    }

    [Fact]
    public void EntryDelete_LinksIntoItself_ClearsThem()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        LEntryArchive entries = TInterface.TEntryArchiveCreate(workspace.TWorkspaceDatabase);
        LMeaningArchive meanings = TInterface.TMeaningArchiveCreate(workspace.TWorkspaceDatabase);
        LRelationArchive relations = TInterface.TRelationArchiveCreate(workspace.TWorkspaceDatabase);

        LEntry entry = entries.TEntryCreate(
            TInterface.TEntryCreate(0, "word", "en", null, null, null, null), [], []);
        LMeaning first = meanings.TMeaningCreate(
            TInterface.TMeaningCreate(0, entry.LEntryId, null, 0, null, "one", null, null, string.Empty));
        LMeaning second = meanings.TMeaningCreate(
            TInterface.TMeaningCreate(0, entry.LEntryId, null, 0, null, "two", null, null, string.Empty));
        relations.TRelationCreate(
            TInterface.TRelationCreate(0, first.LMeaningId, 0, "related", null, null, null, second.LMeaningId));

        entries.TEntryDelete(entry.LEntryId);

        Assert.Equal(0, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM relation;"));
        Assert.Equal(0, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM relation_sense;"));
    }

    [Fact]
    public void EntryFind_AccentedOtherCase_FindsEntry()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        LEntryArchive entries = TInterface.TEntryArchiveCreate(workspace.TWorkspaceDatabase);

        entries.TEntryCreate(TInterface.TEntryCreate(0, "Äpfel", "de", null, null, null, null), [], []);
        entries.TEntryCreate(TInterface.TEntryCreate(0, "straße", "de", null, null, null, null), [], []);

        Assert.Equal("Äpfel", Assert.Single(entries.TEntryFind("äpfel")).LEntryHeadword);
        Assert.Equal("Äpfel", Assert.Single(entries.TEntryFind("ÄPF")).LEntryHeadword);
        Assert.Equal("straße", Assert.Single(entries.TEntryFind("STRAßE")).LEntryHeadword);

        Assert.Equal(2, entries.TEntryFind("   ").Count);
        Assert.Equal(2, entries.TEntryFind(string.Empty).Count);
        Assert.Single(entries.TEntryFind("  Äpfel "));
    }
}
