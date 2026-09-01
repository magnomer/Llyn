using Llyn.Core;
using Llyn.Infrastructure;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Database.Tests;

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
                new LCardDraft(string.Empty, string.Empty, "the first meaning", [], [], string.Empty, []),
                new LCardDraft(string.Empty, string.Empty, "the second meaning", [], [], string.Empty, []),
            ],
            [
                new LCardDraft(
                    string.Empty, "in a word", "briefly", [], [], string.Empty, []),
            ]));

        Assert.NotEmpty(entry.LEntryId);

        LEntryArchive entries = new(workspace.TWorkspaceDatabase);
        Assert.Equal("word", entries.LEntryRead(entry.LEntryId)?.LEntryHeadword);
        Assert.Equal("English", entries.LEntryRead(entry.LEntryId)?.LEntryLanguage);

        IReadOnlyList<LSense> senses = new LSenseArchive(workspace.TWorkspaceDatabase).LSenseRead(entry.LEntryId);
        Assert.Equal(
            ["the first meaning", "the second meaning"],
            senses.Select(sense => sense.LSenseDefinition));
        Assert.Equal([0, 1], senses.Select(sense => sense.LSensePosition));

        LCollocation collocation = Assert.Single(
            new LCollocationArchive(workspace.TWorkspaceDatabase).LCollocationRead(entry.LEntryId));
        Assert.Equal("in a word", collocation.LCollocationExpression);
        Assert.Equal("briefly", collocation.LCollocationMeaning);

        Assert.Equal("a note", new LNoteArchive(workspace.TWorkspaceDatabase).LNoteRead(entry.LEntryId)?.LNoteText);
        Assert.Equal(
            "wɜːd",
            new LPronunciationArchive(workspace.TWorkspaceDatabase)
                .LPronunciationRead(entry.LEntryId)?.LPronunciationIpa);

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
            [new LCardDraft(
                string.Empty, string.Empty, "a meaning", ["he said a word"], ["conversation"], "term",
                ["spoken"])],
            [new LCardDraft(
                string.Empty, "in a word", "briefly", ["in a word, no"], ["summary"], string.Empty,
                ["written"])]);

        LEntry entry = engine.LEngineEntrySave(draft);

        LSense sense = Assert.Single(new LSenseArchive(workspace.TWorkspaceDatabase).LSenseRead(entry.LEntryId));
        LCollocation collocation = Assert.Single(
            new LCollocationArchive(workspace.TWorkspaceDatabase).LCollocationRead(entry.LEntryId));

        LExampleLink examples = new(workspace.TWorkspaceDatabase);
        LSituationArchive situations = new(workspace.TWorkspaceDatabase);
        LTagArchive tags = new(workspace.TWorkspaceDatabase);

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

        Assert.Equal(0, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM relation;"));
        Assert.Equal(0, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM collocation_synonym;"));

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
    public void ADraftOfNothingButAHeadwordWritesNoSenseAndNoCollocation()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = new(workspace.TWorkspaceFolder);

        LCardDraft blank = new(
            string.Empty, string.Empty, string.Empty, [], [], string.Empty, []);

        LEntry entry = engine.LEngineEntrySave(new LEntryDraft(
            "word", "English", string.Empty, string.Empty, [blank], [blank]));

        Assert.Equal(0, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM sense;"));
        Assert.Equal(0, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM collocation;"));

        LEntryDraft? loaded = engine.LEngineEntryLoad(entry.LEntryId);
        Assert.NotNull(loaded);
        Assert.Equal("word", loaded.LEntryDraftHeadword);
        Assert.Empty(loaded.LEntryDraftSenses);
        Assert.Empty(loaded.LEntryDraftCollocations);
    }

    [Fact]
    public void ABlankCardBetweenTwoFilledOnesLeavesTheRestAtContiguousPositions()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = new(workspace.TWorkspaceFolder);

        LEntry entry = engine.LEngineEntrySave(new LEntryDraft(
            "word",
            "English",
            string.Empty,
            string.Empty,
            [
                new LCardDraft(string.Empty, string.Empty, "the first meaning", [], [], string.Empty, []),
                new LCardDraft(string.Empty, string.Empty, "   ", [], [], string.Empty, ["  "]),
                new LCardDraft(string.Empty, string.Empty, "the second meaning", [], [], string.Empty, []),
            ],
            [
                new LCardDraft(string.Empty, "in a word", "briefly", [], [], string.Empty, []),
                new LCardDraft(string.Empty, string.Empty, string.Empty, [], [], string.Empty, []),
                new LCardDraft(string.Empty, "word for word", "exactly", [], [], string.Empty, []),
            ]));

        IReadOnlyList<LSense> senses = new LSenseArchive(workspace.TWorkspaceDatabase).LSenseRead(entry.LEntryId);
        Assert.Equal(
            ["the first meaning", "the second meaning"],
            senses.Select(sense => sense.LSenseDefinition));
        Assert.Equal([0, 1], senses.Select(sense => sense.LSensePosition));

        IReadOnlyList<LCollocation> collocations =
            new LCollocationArchive(workspace.TWorkspaceDatabase).LCollocationRead(entry.LEntryId);
        Assert.Equal(
            ["in a word", "word for word"],
            collocations.Select(collocation => collocation.LCollocationExpression));
        Assert.Equal([0, 1], collocations.Select(collocation => collocation.LCollocationPosition));
    }

    [Fact]
    public void ADraftWithNoHeadwordIsRefusedBeforeAnythingIsWritten()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = new(workspace.TWorkspaceFolder);

        LRefusal refusal = Assert.Throws<LRefusal>(() => engine.LEngineEntrySave(
            new LEntryDraft("   ", "English", string.Empty, string.Empty, [], [])));

        Assert.Equal(LRefusal.LRefusalHeadword, refusal.LRefusalReason);

        Assert.Equal(0, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM entry;"));
    }

    [Fact]
    public void AFailurePartWayThroughASaveLeavesNoEntryBehind()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = new(workspace.TWorkspaceFolder);

        workspace.TWorkspaceScriptRun("DROP TABLE revision_change; DROP TABLE revision;");

        Assert.ThrowsAny<Exception>(() => engine.LEngineEntrySave(new LEntryDraft(
            "word",
            "English",
            string.Empty,
            "a note",
            [new LCardDraft(string.Empty, string.Empty, "a meaning", [], [], string.Empty, [])],
            [])));

        Assert.Equal(0, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM entry;"));
        Assert.Equal(0, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM sense;"));
        Assert.Equal(0, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM note;"));
    }
}
