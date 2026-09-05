using Llyn.Core;
using Llyn.Infrastructure;
using Xunit;

namespace Llyn.Tests;

public sealed class TEntryStore
{
    [Fact]
    public void AnEntryReadsBackWithItsFormsAndPartsOfSpeech()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        LEntryArchive entries = TInterface.TEntryArchiveCreate(workspace.TWorkspaceDatabase);

        LEntry stored = entries.TEntryCreate(
            TInterface.TEntryCreate(string.Empty, "word", "en", null, null, null, null),
            [TInterface.TFormCreate(string.Empty, 0, "word", null, "headword"),
             TInterface.TFormCreate(string.Empty, 0, "words", null, "plural")],
            [TInterface.TSpeechCreate(string.Empty, 0, "noun")]);

        Assert.NotEmpty(stored.LEntryId);
        Assert.Equal("word", entries.TEntryRead(stored.LEntryId)?.LEntryHeadword);
        Assert.Equal([0, 1], entries.TEntryFormRead(stored.LEntryId).Select(form => form.LFormPosition));
        Assert.Single(entries.TEntrySpeechRead(stored.LEntryId));
    }

    [Fact]
    public void UpdatingAnEntryThatDoesNotExistThrows()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        LEntryArchive entries = TInterface.TEntryArchiveCreate(workspace.TWorkspaceDatabase);

        Assert.Throws<InvalidOperationException>(
            () => entries.TEntryUpdate(TInterface.TEntryCreate("missing", "word", "en", null, null, null, null)));
    }

    [Fact]
    public void DeletingAnEntryTakesEverythingItOwnsAndNothingItOnlyReferences()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        LEntryArchive entries = TInterface.TEntryArchiveCreate(workspace.TWorkspaceDatabase);
        LSenseArchive senses = TInterface.TSenseArchiveCreate(workspace.TWorkspaceDatabase);
        LExampleArchive examples = TInterface.TExampleArchiveCreate(workspace.TWorkspaceDatabase);
        LExampleLink links = TInterface.TExampleLinkCreate(workspace.TWorkspaceDatabase);

        LEntry entry = entries.TEntryCreate(
            TInterface.TEntryCreate(string.Empty, "word", "en", null, null, null, null),
            [TInterface.TFormCreate(string.Empty, 0, "word", null, "headword")],
            []);
        LSense sense = senses.TSenseCreate(
            TInterface.TSenseCreate(string.Empty, entry.LEntryId, null, 0, null, "a meaning", null, null, string.Empty));
        LExample example = examples.TExampleCreate(
            TInterface.TExampleCreate(string.Empty, "en", "a sentence", null, null));
        links.TExampleSenseAttach(sense.LSenseId, example.LExampleId, 0);

        entries.TEntryDelete(entry.LEntryId);

        Assert.Null(entries.TEntryRead(entry.LEntryId));
        Assert.Equal(0, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM form;"));
        Assert.Equal(0, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM sense;"));
        Assert.Equal(0, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM sense_example;"));

        Assert.NotNull(examples.TExampleRead(example.LExampleId));
    }

    [Fact]
    public void DeletingAnEntryAnotherEntryLinksToIsRefused()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        LEntryArchive entries = TInterface.TEntryArchiveCreate(workspace.TWorkspaceDatabase);
        LSenseArchive senses = TInterface.TSenseArchiveCreate(workspace.TWorkspaceDatabase);
        LRelationArchive relations = TInterface.TRelationArchiveCreate(workspace.TWorkspaceDatabase);

        LEntry target = entries.TEntryCreate(
            TInterface.TEntryCreate(string.Empty, "target", "en", null, null, null, null), [], []);
        LEntry origin = entries.TEntryCreate(
            TInterface.TEntryCreate(string.Empty, "origin", "en", null, null, null, null), [], []);
        LSense sense = senses.TSenseCreate(
            TInterface.TSenseCreate(string.Empty, origin.LEntryId, null, 0, null, null, null, null, string.Empty));
        relations.TRelationCreate(
            TInterface.TRelationCreate(string.Empty, sense.LSenseId, 0, "synonym", null, null, target.LEntryId, null));

        InvalidOperationException error =
            Assert.Throws<InvalidOperationException>(() => entries.TEntryDelete(target.LEntryId));

        Assert.Contains("lexical link", error.Message, StringComparison.Ordinal);
        Assert.NotNull(entries.TEntryRead(target.LEntryId));
    }

    [Fact]
    public void DeletingAnEntryClearsTheLinksItOwnsIntoItself()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        LEntryArchive entries = TInterface.TEntryArchiveCreate(workspace.TWorkspaceDatabase);
        LSenseArchive senses = TInterface.TSenseArchiveCreate(workspace.TWorkspaceDatabase);
        LRelationArchive relations = TInterface.TRelationArchiveCreate(workspace.TWorkspaceDatabase);

        LEntry entry = entries.TEntryCreate(
            TInterface.TEntryCreate(string.Empty, "word", "en", null, null, null, null), [], []);
        LSense first = senses.TSenseCreate(
            TInterface.TSenseCreate(string.Empty, entry.LEntryId, null, 0, null, "one", null, null, string.Empty));
        LSense second = senses.TSenseCreate(
            TInterface.TSenseCreate(string.Empty, entry.LEntryId, null, 0, null, "two", null, null, string.Empty));
        relations.TRelationCreate(
            TInterface.TRelationCreate(string.Empty, first.LSenseId, 0, "related", null, null, null, second.LSenseId));

        entries.TEntryDelete(entry.LEntryId);

        Assert.Equal(0, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM relation;"));
        Assert.Equal(0, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM relation_sense;"));
    }

    [Fact]
    public void AnAccentedHeadwordIsFoundTypedInTheOtherCase()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        LEntryArchive entries = TInterface.TEntryArchiveCreate(workspace.TWorkspaceDatabase);

        entries.TEntryCreate(TInterface.TEntryCreate(string.Empty, "Äpfel", "de", null, null, null, null), [], []);
        entries.TEntryCreate(TInterface.TEntryCreate(string.Empty, "straße", "de", null, null, null, null), [], []);

        Assert.Equal("Äpfel", Assert.Single(entries.TEntryFind("äpfel")).LEntryHeadword);
        Assert.Equal("Äpfel", Assert.Single(entries.TEntryFind("ÄPF")).LEntryHeadword);
        Assert.Equal("straße", Assert.Single(entries.TEntryFind("STRAßE")).LEntryHeadword);

        Assert.Equal(2, entries.TEntryFind("   ").Count);
        Assert.Equal(2, entries.TEntryFind(string.Empty).Count);
        Assert.Single(entries.TEntryFind("  Äpfel "));
    }
}
