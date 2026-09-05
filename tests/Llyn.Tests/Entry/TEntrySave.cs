using Llyn.Core;
using Llyn.Infrastructure;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TEntrySave
{
    [Fact]
    public void AWholeDraftIsSavedAsOneEntryAndReadsBackRowForRow()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LEntry entry = engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "word",
            "English",
            "wɜːd",
            "a note",
            [
                TInterface.TCardDraftCreate(string.Empty, string.Empty, "the first meaning", [], [], [], string.Empty, [], [], 1),
                TInterface.TCardDraftCreate(string.Empty, string.Empty, "the second meaning", [], [], [], string.Empty, [], [], 2),
            ],
            [
                TInterface.TCardDraftCreate(
                    string.Empty, "in a word", "briefly", [], [], [], string.Empty, [], [], 1),
            ]));

        Assert.NotEmpty(entry.LEntryId);

        LEntryArchive entries = TInterface.TEntryArchiveCreate(workspace.TWorkspaceDatabase);
        Assert.Equal("word", entries.TEntryRead(entry.LEntryId)?.LEntryHeadword);
        Assert.Equal("English", entries.TEntryRead(entry.LEntryId)?.LEntryLanguage);

        IReadOnlyList<LSense> senses = TInterface.TSenseArchiveCreate(workspace.TWorkspaceDatabase).TSenseRead(entry.LEntryId);
        Assert.Equal(
            ["the first meaning", "the second meaning"],
            senses.Select(sense => sense.LSenseDefinition));
        Assert.Equal([0, 1], senses.Select(sense => sense.LSensePosition));

        LCollocation collocation = Assert.Single(
            TInterface.TCollocationArchiveCreate(workspace.TWorkspaceDatabase).TCollocationRead(entry.LEntryId));
        Assert.Equal("in a word", collocation.LCollocationExpression);
        Assert.Equal("briefly", collocation.LCollocationMeaning);

        Assert.Equal("a note", TInterface.TNoteArchiveCreate(workspace.TWorkspaceDatabase).TNoteRead(entry.LEntryId)?.LNoteText);
        Assert.Equal(
            "wɜːd",
            TInterface.TPronunciationArchiveCreate(workspace.TWorkspaceDatabase)
                .TPronunciationRead(entry.LEntryId)?.LPronunciationIpa);

        LRevision? revision = TInterface.TRevisionArchiveCreate(workspace.TWorkspaceDatabase).TRevisionLatestRead();
        Assert.NotNull(revision);
        LRevisionChange change = Assert.Single(
            TInterface.TRevisionArchiveCreate(workspace.TWorkspaceDatabase).TRevisionChangeRead(revision.LRevisionId));
        Assert.Equal("create", change.LRevisionChangeKind);
        Assert.Equal(entry.LEntryId, change.LRevisionChangeTarget);

        LWorkspaceState state = TInterface.TWorkspaceArchiveCreate(workspace.TWorkspaceDatabase).TWorkspaceStateRead();
        Assert.Equal(entry.LEntryId, state.LWorkspaceStateLeft);
        Assert.Equal(revision.LRevisionId, state.LWorkspaceStateRevision);
    }

    [Fact]
    public void ACardsExampleSituationAndTagAreWrittenAsIndependentRowsTheCardReferences()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LEntryDraft draft = TInterface.TEntryDraftCreate(
            "word",
            "English",
            string.Empty,
            string.Empty,
            [TInterface.TCardDraftCreate(
                string.Empty, string.Empty, "a meaning", [TInterface.TExampleDraftCreate("he said a word")], [TInterface.TSituationDraftCreate("conversation")], [], "term",
                ["spoken"], [], 1)],
            [TInterface.TCardDraftCreate(
                string.Empty, "in a word", "briefly", [TInterface.TExampleDraftCreate("in a word, no")], [TInterface.TSituationDraftCreate("summary")], [], string.Empty,
                ["written"], [], 1)]);

        LEntry entry = engine.TEngineEntrySave(draft);

        LSense sense = Assert.Single(TInterface.TSenseArchiveCreate(workspace.TWorkspaceDatabase).TSenseRead(entry.LEntryId));
        LCollocation collocation = Assert.Single(
            TInterface.TCollocationArchiveCreate(workspace.TWorkspaceDatabase).TCollocationRead(entry.LEntryId));

        LExampleLink examples = TInterface.TExampleLinkCreate(workspace.TWorkspaceDatabase);
        LSituationArchive situations = TInterface.TSituationArchiveCreate(workspace.TWorkspaceDatabase);
        LTagArchive tags = TInterface.TTagArchiveCreate(workspace.TWorkspaceDatabase);

        Assert.Equal(
            "he said a word",
            Assert.Single(examples.TExampleSenseRead(sense.LSenseId)).LExampleText);
        Assert.Equal(
            "conversation",
            Assert.Single(situations.TSituationSenseRead(sense.LSenseId)).LSituationTitle);
        Assert.Equal("spoken", Assert.Single(tags.TTagSenseRead(sense.LSenseId)).LTagText);

        Assert.Equal(
            "in a word, no",
            Assert.Single(examples.TExampleCollocationRead(collocation.LCollocationId)).LExampleText);
        Assert.Equal(
            "summary",
            Assert.Single(situations.TSituationCollocationRead(collocation.LCollocationId)).LSituationTitle);
        Assert.Equal(
            "written",
            Assert.Single(tags.TTagCollocationRead(collocation.LCollocationId)).LTagText);

        Assert.Equal(0, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM relation;"));
        Assert.Equal(0, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM collocation_synonym;"));

        LEntry second = engine.TEngineEntrySave(draft);
        LSense other = Assert.Single(TInterface.TSenseArchiveCreate(workspace.TWorkspaceDatabase).TSenseRead(second.LEntryId));

        Assert.NotEqual(
            Assert.Single(examples.TExampleSenseRead(sense.LSenseId)).LExampleId,
            Assert.Single(examples.TExampleSenseRead(other.LSenseId)).LExampleId);
        Assert.NotEqual(
            Assert.Single(situations.TSituationSenseRead(sense.LSenseId)).LSituationId,
            Assert.Single(situations.TSituationSenseRead(other.LSenseId)).LSituationId);
        Assert.Equal(
            Assert.Single(tags.TTagSenseRead(sense.LSenseId)).LTagText,
            Assert.Single(tags.TTagSenseRead(other.LSenseId)).LTagText);

        Assert.Equal(4, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM example;"));
        Assert.Equal(4, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM situation;"));
        Assert.Equal(2, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM sense_tag;"));
        Assert.Equal(2, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM collocation_tag;"));
    }

    [Fact]
    public void ADraftOfNothingButAHeadwordWritesNoSenseAndNoCollocation()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LCardDraft blank = TInterface.TCardDraftCreate(
            string.Empty, string.Empty, string.Empty, [], [], [], string.Empty, [], [], 1);

        LEntry entry = engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "word", "English", string.Empty, string.Empty, [blank], [blank]));

        Assert.Equal(0, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM sense;"));
        Assert.Equal(0, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM collocation;"));

        LEntryDraft? loaded = engine.TEngineEntryLoad(entry.LEntryId);
        Assert.NotNull(loaded);
        Assert.Equal("word", loaded.LEntryDraftHeadword);
        Assert.Empty(loaded.LEntryDraftSenses);
        Assert.Empty(loaded.LEntryDraftCollocations);
    }

    [Fact]
    public void ABlankCardBetweenTwoFilledOnesLeavesTheRestAtContiguousPositions()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LEntry entry = engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "word",
            "English",
            string.Empty,
            string.Empty,
            [
                TInterface.TCardDraftCreate(string.Empty, string.Empty, "the first meaning", [], [], [], string.Empty, [], [], 1),
                TInterface.TCardDraftCreate(string.Empty, string.Empty, "   ", [], [], [], string.Empty, ["  "], [], 2),
                TInterface.TCardDraftCreate(string.Empty, string.Empty, "the second meaning", [], [], [], string.Empty, [], [], 3),
            ],
            [
                TInterface.TCardDraftCreate(string.Empty, "in a word", "briefly", [], [], [], string.Empty, [], [], 1),
                TInterface.TCardDraftCreate(string.Empty, string.Empty, string.Empty, [], [], [], string.Empty, [], [], 2),
                TInterface.TCardDraftCreate(string.Empty, "word for word", "exactly", [], [], [], string.Empty, [], [], 3),
            ]));

        IReadOnlyList<LSense> senses = TInterface.TSenseArchiveCreate(workspace.TWorkspaceDatabase).TSenseRead(entry.LEntryId);
        Assert.Equal(
            ["the first meaning", "the second meaning"],
            senses.Select(sense => sense.LSenseDefinition));
        Assert.Equal([0, 1], senses.Select(sense => sense.LSensePosition));

        IReadOnlyList<LCollocation> collocations =
            TInterface.TCollocationArchiveCreate(workspace.TWorkspaceDatabase).TCollocationRead(entry.LEntryId);
        Assert.Equal(
            ["in a word", "word for word"],
            collocations.Select(collocation => collocation.LCollocationExpression));
        Assert.Equal([0, 1], collocations.Select(collocation => collocation.LCollocationPosition));
    }

    [Fact]
    public void ADraftWithNoHeadwordIsRefusedBeforeAnythingIsWritten()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LRefusal refusal = Assert.Throws<LRefusal>(() => engine.TEngineEntrySave(
            TInterface.TEntryDraftCreate("   ", "English", string.Empty, string.Empty, [], [])));

        Assert.Equal(LRefusal.LRefusalHeadword, refusal.LRefusalReason);

        Assert.Equal(0, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM entry;"));
    }

    [Fact]
    public void AFailurePartWayThroughASaveLeavesNoEntryBehind()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        workspace.TWorkspaceScriptRun("DROP TABLE revision_change; DROP TABLE revision;");

        Assert.ThrowsAny<Exception>(() => engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "word",
            "English",
            string.Empty,
            "a note",
            [TInterface.TCardDraftCreate(string.Empty, string.Empty, "a meaning", [], [], [], string.Empty, [], [], 1)],
            [])));

        Assert.Equal(0, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM entry;"));
        Assert.Equal(0, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM sense;"));
        Assert.Equal(0, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM note;"));
    }
}
