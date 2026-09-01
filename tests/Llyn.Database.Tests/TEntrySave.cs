using Llyn.Core;
using Llyn.Infrastructure;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Database.Tests;

/// <summary>
/// Covers the one seam that writes an entry: the whole input form goes in as a single engine call and
/// every row it produces reads back, a blank headword is refused before anything is opened, and a
/// failure part-way through leaves no entry row behind — the save is one unit of work or none.
/// </summary>
public sealed class TEntrySave
{
    [Fact]
    public void AWholeDraftIsSavedAsOneEntryAndReadsBackRowForRow()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = new(workspace.TWorkspaceFolder);

        LEntry entry = engine.LEngineEntrySave(new LEntryDraft(
            "word",
            "English",
            "wɜːd",
            "a note",
            [
                new LSenseDraft(1, "the first meaning", string.Empty, string.Empty, string.Empty, string.Empty),
                new LSenseDraft(2, "the second meaning", string.Empty, string.Empty, string.Empty, string.Empty),
            ],
            [
                new LCollocationDraft(
                    1, "in a word", "briefly", string.Empty, string.Empty, string.Empty, string.Empty),
            ]));

        Assert.NotEmpty(entry.LEntryId);

        LEntryArchive entries = new(workspace.TWorkspaceDatabase);
        Assert.Equal("word", entries.LEntryRead(entry.LEntryId)?.LEntryHeadword);
        Assert.Equal("English", entries.LEntryRead(entry.LEntryId)?.LEntryLanguage);

        // The senses land in card order, each at the position its card held.
        IReadOnlyList<LSense> senses = new LSenseArchive(workspace.TWorkspaceDatabase).LSenseRead(entry.LEntryId);
        Assert.Equal(
            ["the first meaning", "the second meaning"],
            senses.Select(sense => sense.LSenseDefinition));
        Assert.Equal([0, 1], senses.Select(sense => sense.LSensePosition));

        // The collocation carries both of its fields: the expression and the meaning that explains it.
        LCollocation collocation = Assert.Single(
            new LCollocationArchive(workspace.TWorkspaceDatabase).LCollocationRead(entry.LEntryId));
        Assert.Equal("in a word", collocation.LCollocationExpression);
        Assert.Equal("briefly", collocation.LCollocationMeaning);

        Assert.Equal("a note", new LNoteArchive(workspace.TWorkspaceDatabase).LNoteRead(entry.LEntryId)?.LNoteText);
        Assert.Equal(
            "wɜːd",
            new LPronunciationArchive(workspace.TWorkspaceDatabase)
                .LPronunciationRead(entry.LEntryId)?.LPronunciationIpa);

        // The save is recorded as history and the workspace row is moved onto it.
        LRevision? revision = new LRevisionArchive(workspace.TWorkspaceDatabase).LRevisionLatestRead();
        Assert.NotNull(revision);
        LRevisionChange change = Assert.Single(
            new LRevisionArchive(workspace.TWorkspaceDatabase).LRevisionChangeRead(revision.LRevisionId));
        Assert.Equal("create", change.LRevisionChangeKind);
        Assert.Equal(entry.LEntryId, change.LRevisionChangeTarget);

        LWorkspaceState state = new LWorkspaceArchive(workspace.TWorkspaceDatabase).LWorkspaceStateRead();
        Assert.Equal(entry.LEntryId, state.LWorkspaceStateLeft);
        Assert.Equal(revision.LRevisionId, state.LWorkspaceStateRevision);
    }

    [Fact]
    public void ACardsExampleSituationAndTagAreWrittenAsIndependentRowsTheCardReferences()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = new(workspace.TWorkspaceFolder);

        LEntryDraft draft = new(
            "word",
            "English",
            string.Empty,
            string.Empty,
            [new LSenseDraft(1, "a meaning", "he said a word", "conversation", "term", "spoken")],
            [new LCollocationDraft(
                1, "in a word", "briefly", "in a word, no", "summary", string.Empty, "written")]);

        LEntry entry = engine.LEngineEntrySave(draft);

        LSense sense = Assert.Single(new LSenseArchive(workspace.TWorkspaceDatabase).LSenseRead(entry.LEntryId));
        LCollocation collocation = Assert.Single(
            new LCollocationArchive(workspace.TWorkspaceDatabase).LCollocationRead(entry.LEntryId));

        LExampleLink examples = new(workspace.TWorkspaceDatabase);
        LSituationArchive situations = new(workspace.TWorkspaceDatabase);
        LTagArchive tags = new(workspace.TWorkspaceDatabase);

        // Each field became a row of its own that the card now references, not a column on the card.
        Assert.Equal(
            "he said a word",
            Assert.Single(examples.LExampleSenseRead(sense.LSenseId)).LExampleText);
        Assert.Equal(
            "conversation",
            Assert.Single(situations.LSituationSenseRead(sense.LSenseId)).LSituationTitle);
        Assert.Equal("spoken", Assert.Single(tags.LTagSenseRead(sense.LSenseId)).LTagText);

        Assert.Equal(
            "in a word, no",
            Assert.Single(examples.LExampleCollocationRead(collocation.LCollocationId)).LExampleText);
        Assert.Equal(
            "summary",
            Assert.Single(situations.LSituationCollocationRead(collocation.LCollocationId)).LSituationTitle);
        Assert.Equal(
            "written",
            Assert.Single(tags.LTagCollocationRead(collocation.LCollocationId)).LTagText);

        // The sense card's Synonym text is deliberately dropped: the relation it would become targets
        // an Entry or a Meaning by id and the field holds free text, so nothing can resolve it yet.
        Assert.Equal(0, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM relation;"));
        Assert.Equal(0, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM collocation_synonym;"));

        // Saving the same text again matches nothing: every non-empty field creates a new row, so the
        // second entry references rows of its own rather than the first entry's.
        LEntry second = engine.LEngineEntrySave(draft);
        LSense other = Assert.Single(new LSenseArchive(workspace.TWorkspaceDatabase).LSenseRead(second.LEntryId));

        Assert.NotEqual(
            Assert.Single(examples.LExampleSenseRead(sense.LSenseId)).LExampleId,
            Assert.Single(examples.LExampleSenseRead(other.LSenseId)).LExampleId);
        Assert.NotEqual(
            Assert.Single(situations.LSituationSenseRead(sense.LSenseId)).LSituationId,
            Assert.Single(situations.LSituationSenseRead(other.LSenseId)).LSituationId);
        Assert.NotEqual(
            Assert.Single(tags.LTagSenseRead(sense.LSenseId)).LTagId,
            Assert.Single(tags.LTagSenseRead(other.LSenseId)).LTagId);

        Assert.Equal(4, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM example;"));
        Assert.Equal(4, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM situation;"));
        Assert.Equal(4, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM tag;"));
    }

    [Fact]
    public void ADraftWithNoHeadwordIsRefusedBeforeAnythingIsWritten()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = new(workspace.TWorkspaceFolder);

        Assert.Throws<InvalidOperationException>(() => engine.LEngineEntrySave(
            new LEntryDraft("   ", "English", string.Empty, string.Empty, [], [])));

        Assert.Equal(0, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM entry;"));
    }

    [Fact]
    public void AFailurePartWayThroughASaveLeavesNoEntryBehind()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = new(workspace.TWorkspaceFolder);

        // The revision is written after the entry and everything under it, so removing the table it
        // needs fails the save at its last step — with the entry, its sense, and its note already
        // written inside the session. The engine is built first, or creating it would restore the table.
        workspace.TWorkspaceScriptRun("DROP TABLE revision_change; DROP TABLE revision;");

        Assert.ThrowsAny<Exception>(() => engine.LEngineEntrySave(new LEntryDraft(
            "word",
            "English",
            string.Empty,
            "a note",
            [new LSenseDraft(1, "a meaning", string.Empty, string.Empty, string.Empty, string.Empty)],
            [])));

        Assert.Equal(0, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM entry;"));
        Assert.Equal(0, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM sense;"));
        Assert.Equal(0, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM note;"));
    }
}
