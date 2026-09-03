using Llyn.Core;
using Llyn.Infrastructure;
using Xunit;

namespace Llyn.Database.Tests;

public sealed class TEntryArchive
{
    [Fact]
    public void AnEntryReadsBackWithItsFormsAndPartsOfSpeech()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        LEntryArchive entries = new(workspace.TWorkspaceDatabase);

        LEntry stored = entries.LEntryCreate(
            new LEntry(string.Empty, "word", "en", null, null, null, null),
            [new LForm(string.Empty, 0, "word", null, "headword"),
             new LForm(string.Empty, 0, "words", null, "plural")],
            [new LSpeech(string.Empty, 0, "noun")]);

        Assert.NotEmpty(stored.LEntryId);
        Assert.Equal("word", entries.LEntryRead(stored.LEntryId)?.LEntryHeadword);
        Assert.Equal([0, 1], entries.LEntryFormRead(stored.LEntryId).Select(form => form.LFormPosition));
        Assert.Single(entries.LEntrySpeechRead(stored.LEntryId));
    }

    [Fact]
    public void UpdatingAnEntryThatDoesNotExistThrows()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        LEntryArchive entries = new(workspace.TWorkspaceDatabase);

        Assert.Throws<InvalidOperationException>(
            () => entries.LEntryUpdate(new LEntry("missing", "word", "en", null, null, null, null)));
    }

    [Fact]
    public void DeletingAnEntryTakesEverythingItOwnsAndNothingItOnlyReferences()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        LEntryArchive entries = new(workspace.TWorkspaceDatabase);
        LSenseArchive senses = new(workspace.TWorkspaceDatabase);
        LExampleArchive examples = new(workspace.TWorkspaceDatabase);
        LExampleLink links = new(workspace.TWorkspaceDatabase);

        LEntry entry = entries.LEntryCreate(
            new LEntry(string.Empty, "word", "en", null, null, null, null),
            [new LForm(string.Empty, 0, "word", null, "headword")],
            []);
        LSense sense = senses.LSenseCreate(
            new LSense(string.Empty, entry.LEntryId, null, 0, null, "a meaning", null, null, string.Empty));
        LExample example = examples.LExampleCreate(
            new LExample(string.Empty, "en", "a sentence", null, null));
        links.LExampleSenseAttach(sense.LSenseId, example.LExampleId, 0);

        entries.LEntryDelete(entry.LEntryId);

        Assert.Null(entries.LEntryRead(entry.LEntryId));
        Assert.Equal(0, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM form;"));
        Assert.Equal(0, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM sense;"));
        Assert.Equal(0, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM sense_example;"));

        Assert.NotNull(examples.LExampleRead(example.LExampleId));
    }

    [Fact]
    public void DeletingAnEntryAnotherEntryLinksToIsRefused()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        LEntryArchive entries = new(workspace.TWorkspaceDatabase);
        LSenseArchive senses = new(workspace.TWorkspaceDatabase);
        LRelationArchive relations = new(workspace.TWorkspaceDatabase);

        LEntry target = entries.LEntryCreate(
            new LEntry(string.Empty, "target", "en", null, null, null, null), [], []);
        LEntry origin = entries.LEntryCreate(
            new LEntry(string.Empty, "origin", "en", null, null, null, null), [], []);
        LSense sense = senses.LSenseCreate(
            new LSense(string.Empty, origin.LEntryId, null, 0, null, null, null, null, string.Empty));
        relations.LRelationCreate(
            new LRelation(string.Empty, sense.LSenseId, 0, "synonym", null, null, target.LEntryId, null));

        InvalidOperationException error =
            Assert.Throws<InvalidOperationException>(() => entries.LEntryDelete(target.LEntryId));

        Assert.Contains("lexical link", error.Message, StringComparison.Ordinal);
        Assert.NotNull(entries.LEntryRead(target.LEntryId));
    }

    [Fact]
    public void DeletingAnEntryClearsTheLinksItOwnsIntoItself()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        LEntryArchive entries = new(workspace.TWorkspaceDatabase);
        LSenseArchive senses = new(workspace.TWorkspaceDatabase);
        LRelationArchive relations = new(workspace.TWorkspaceDatabase);

        LEntry entry = entries.LEntryCreate(
            new LEntry(string.Empty, "word", "en", null, null, null, null), [], []);
        LSense first = senses.LSenseCreate(
            new LSense(string.Empty, entry.LEntryId, null, 0, null, "one", null, null, string.Empty));
        LSense second = senses.LSenseCreate(
            new LSense(string.Empty, entry.LEntryId, null, 0, null, "two", null, null, string.Empty));
        relations.LRelationCreate(
            new LRelation(string.Empty, first.LSenseId, 0, "related", null, null, null, second.LSenseId));

        entries.LEntryDelete(entry.LEntryId);

        Assert.Equal(0, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM relation;"));
        Assert.Equal(0, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM relation_sense;"));
    }

    [Fact]
    public void AnAccentedHeadwordIsFoundTypedInTheOtherCase()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        LEntryArchive entries = new(workspace.TWorkspaceDatabase);

        entries.LEntryCreate(new LEntry(string.Empty, "Äpfel", "de", null, null, null, null), [], []);
        entries.LEntryCreate(new LEntry(string.Empty, "straße", "de", null, null, null, null), [], []);

        Assert.Equal("Äpfel", Assert.Single(entries.LEntryFind("äpfel")).LEntryHeadword);
        Assert.Equal("Äpfel", Assert.Single(entries.LEntryFind("ÄPF")).LEntryHeadword);
        Assert.Equal("straße", Assert.Single(entries.LEntryFind("STRAßE")).LEntryHeadword);

        Assert.Equal(2, entries.LEntryFind("   ").Count);
        Assert.Equal(2, entries.LEntryFind(string.Empty).Count);
        Assert.Single(entries.LEntryFind("  Äpfel "));
    }
}
