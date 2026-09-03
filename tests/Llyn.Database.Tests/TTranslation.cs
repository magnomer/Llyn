using Llyn.Core;
using Llyn.Infrastructure;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Database.Tests;

public sealed class TTranslation
{
    [Fact]
    public void ACardHoldsItsLinksInTheOrderItGaveThem()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        LEntryArchive entries = new(workspace.TWorkspaceDatabase);
        LTranslationArchive translations = new(workspace.TWorkspaceDatabase);

        LEntry source = TTranslationEntryCreate(entries, "break", "English");
        string senseId = TTranslationSenseCreate(workspace, source.LEntryId);
        LEntry first = TTranslationEntryCreate(entries, "부수다", "Korean");
        LEntry second = TTranslationEntryCreate(entries, "깨다", "Korean");
        LEntry third = TTranslationEntryCreate(entries, "부러뜨리다", "Korean");

        translations.LTranslationSenseSave(
            senseId,
            [
                new LTranslation(first.LEntryId, 0),
                new LTranslation(second.LEntryId, 0),
                new LTranslation(third.LEntryId, 0),
            ]);

        Assert.Equal(
            [first.LEntryId, second.LEntryId, third.LEntryId],
            translations.LTranslationSenseRead(senseId)
                .Select(translation => translation.LTranslationEntryId));
        Assert.Equal(
            [0, 1, 2],
            translations.LTranslationSenseRead(senseId)
                .Select(translation => translation.LTranslationPosition));
    }

    [Fact]
    public void WritingTheSameEntryTwiceUnderOneCardKeepsOneLink()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        LEntryArchive entries = new(workspace.TWorkspaceDatabase);
        LTranslationArchive translations = new(workspace.TWorkspaceDatabase);

        LEntry source = TTranslationEntryCreate(entries, "break", "English");
        string senseId = TTranslationSenseCreate(workspace, source.LEntryId);
        LEntry first = TTranslationEntryCreate(entries, "부수다", "Korean");
        LEntry second = TTranslationEntryCreate(entries, "깨다", "Korean");

        translations.LTranslationSenseSave(
            senseId,
            [
                new LTranslation(first.LEntryId, 0),
                new LTranslation(first.LEntryId, 0),
                new LTranslation("   ", 0),
                new LTranslation(second.LEntryId, 0),
            ]);

        Assert.Equal(
            [first.LEntryId, second.LEntryId],
            translations.LTranslationSenseRead(senseId)
                .Select(translation => translation.LTranslationEntryId));
        Assert.Equal(
            [0, 1],
            TTranslationPositionRead(workspace, "sense_translation", "sense_id", senseId));
    }

    [Fact]
    public void AShorterListDropsTheRemovedLinksAndRenumbersWhatIsLeft()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        LEntryArchive entries = new(workspace.TWorkspaceDatabase);
        LTranslationArchive translations = new(workspace.TWorkspaceDatabase);

        LEntry source = TTranslationEntryCreate(entries, "break", "English");
        string collocationId = TTranslationCollocationCreate(workspace, source.LEntryId);
        LEntry first = TTranslationEntryCreate(entries, "부수다", "Korean");
        LEntry second = TTranslationEntryCreate(entries, "깨다", "Korean");
        LEntry third = TTranslationEntryCreate(entries, "부러뜨리다", "Korean");

        translations.LTranslationCollocationSave(
            collocationId,
            [
                new LTranslation(first.LEntryId, 0),
                new LTranslation(second.LEntryId, 0),
                new LTranslation(third.LEntryId, 0),
            ]);
        translations.LTranslationCollocationSave(
            collocationId,
            [new LTranslation(third.LEntryId, 0), new LTranslation(first.LEntryId, 0)]);

        Assert.Equal(
            [third.LEntryId, first.LEntryId],
            translations.LTranslationCollocationRead(collocationId)
                .Select(translation => translation.LTranslationEntryId));
        Assert.Equal(
            [0, 1],
            TTranslationPositionRead(
                workspace, "collocation_translation", "collocation_id", collocationId));
    }

    [Fact]
    public void DeletingATargetTakesTheLinksPointingAtItAndLeavesTheCardStanding()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        LEntryArchive entries = new(workspace.TWorkspaceDatabase);
        LTranslationArchive translations = new(workspace.TWorkspaceDatabase);

        LEntry source = TTranslationEntryCreate(entries, "break", "English");
        string senseId = TTranslationSenseCreate(workspace, source.LEntryId);
        LEntry first = TTranslationEntryCreate(entries, "부수다", "Korean");
        LEntry second = TTranslationEntryCreate(entries, "깨다", "Korean");

        translations.LTranslationSenseSave(
            senseId,
            [new LTranslation(first.LEntryId, 0), new LTranslation(second.LEntryId, 0)]);

        entries.LEntryDelete(first.LEntryId);

        Assert.Equal(
            [second.LEntryId],
            translations.LTranslationSenseRead(senseId)
                .Select(translation => translation.LTranslationEntryId));
        Assert.Equal(1, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM sense;"));
    }

    [Fact]
    public void ATargetReadNamesEveryStoredEntryAndPassesOverOneNothingAnswers()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        LEntryArchive entries = new(workspace.TWorkspaceDatabase);
        LTranslationArchive translations = new(workspace.TWorkspaceDatabase);

        LEntry first = TTranslationEntryCreate(entries, "부수다", "Korean");
        LEntry second = TTranslationEntryCreate(entries, "깨다", "Korean");

        IReadOnlyList<LTranslationTarget> targets =
            translations.LTranslationTargetRead([second.LEntryId, "missing", first.LEntryId]);

        Assert.Equal(
            [second.LEntryId, first.LEntryId],
            targets.Select(target => target.LTranslationTargetId));
        Assert.Equal(
            ["깨다", "부수다"],
            targets.Select(target => target.LTranslationTargetHeadword));
        Assert.Equal(
            ["Korean", "Korean"],
            targets.Select(target => target.LTranslationTargetLanguage));
    }

    [Fact]
    public void AnEntryFindsBothKindsOfCardThatPointsAtIt()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        LEntryArchive entries = new(workspace.TWorkspaceDatabase);
        LTranslationArchive translations = new(workspace.TWorkspaceDatabase);

        LEntry source = TTranslationEntryCreate(entries, "break", "English");
        string senseId = TTranslationSenseCreate(workspace, source.LEntryId);
        string collocationId = TTranslationCollocationCreate(workspace, source.LEntryId);
        LEntry target = TTranslationEntryCreate(entries, "부수다", "Korean");

        translations.LTranslationSenseSave(senseId, [new LTranslation(target.LEntryId, 0)]);
        translations.LTranslationCollocationSave(
            collocationId, [new LTranslation(target.LEntryId, 0)]);

        IReadOnlyList<LUsage> incoming = translations.LTranslationIncomingRead(target.LEntryId);

        Assert.Equal([senseId, collocationId], incoming.Select(usage => usage.LUsageId));
        Assert.Equal(
            [LOwner.LOwnerSense, LOwner.LOwnerCollocation],
            incoming.Select(usage => usage.LUsageOwner));
        Assert.Equal(["break", "break"], incoming.Select(usage => usage.LUsageHeadword));
        Assert.Equal(
            [source.LEntryId, source.LEntryId], incoming.Select(usage => usage.LUsageEntry));
        Assert.Empty(translations.LTranslationIncomingRead(source.LEntryId));
    }

    [Fact]
    public void ACardCarriesItsLinksThroughASaveAndComesBackHoldingThem()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = new(workspace.TWorkspaceFolder);

        LEntry first = engine.LEngineTranslationCreate("부수다", "Korean");
        LEntry second = engine.LEngineTranslationCreate("깨다", "Korean");

        LEntry stored = engine.LEngineEntrySave(new LEntryDraft(
            "break",
            "English",
            string.Empty,
            string.Empty,
            [TTranslationCardCreate("to come apart", [first.LEntryId, second.LEntryId])],
            []));

        LEntryDraft? loaded = engine.LEngineEntryLoad(stored.LEntryId);

        Assert.NotNull(loaded);
        Assert.Equal(
            [first.LEntryId, second.LEntryId],
            Assert.Single(loaded.LEntryDraftSenses).LCardDraftTranslation);
        Assert.Equal(
            ["break"],
            engine.LEngineIncomingRead(first.LEntryId).Select(usage => usage.LUsageHeadword));
    }

    [Fact]
    public void TakingOneLinkOffACardDropsThatRowAndLeavesTheOther()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = new(workspace.TWorkspaceFolder);

        LEntry first = engine.LEngineTranslationCreate("부수다", "Korean");
        LEntry second = engine.LEngineTranslationCreate("깨다", "Korean");

        LEntry stored = engine.LEngineEntrySave(new LEntryDraft(
            "break",
            "English",
            string.Empty,
            string.Empty,
            [TTranslationCardCreate("to come apart", [first.LEntryId, second.LEntryId])],
            []));

        LEntryDraft? loaded = engine.LEngineEntryLoad(stored.LEntryId);

        Assert.NotNull(loaded);
        Assert.Equal(2, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM sense_translation;"));

        LCardDraft card = loaded.LEntryDraftSenses[0];
        engine.LEngineEntryUpdate(stored.LEntryId, loaded with
        {
            LEntryDraftSenses = [card with { LCardDraftTranslation = [second.LEntryId] }],
        });

        Assert.Equal(1, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM sense_translation;"));

        LEntryDraft? reloaded = engine.LEngineEntryLoad(stored.LEntryId);

        Assert.NotNull(reloaded);
        Assert.Equal(
            second.LEntryId,
            Assert.Single(Assert.Single(reloaded.LEntryDraftSenses).LCardDraftTranslation));
    }

    [Fact]
    public void ACardCarryingNothingButALinkIsStillStored()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = new(workspace.TWorkspaceFolder);

        LEntry target = engine.LEngineTranslationCreate("부수다", "Korean");

        LEntry stored = engine.LEngineEntrySave(new LEntryDraft(
            "break",
            "English",
            string.Empty,
            string.Empty,
            [TTranslationCardCreate(string.Empty, [target.LEntryId])],
            []));

        LEntryDraft? loaded = engine.LEngineEntryLoad(stored.LEntryId);

        Assert.NotNull(loaded);
        Assert.Equal(
            target.LEntryId,
            Assert.Single(Assert.Single(loaded.LEntryDraftSenses).LCardDraftTranslation));
    }

    private static LCardDraft TTranslationCardCreate(string meaning, IReadOnlyList<string> ids)
    {
        return new LCardDraft(
            string.Empty,
            string.Empty,
            meaning,
            [],
            [],
            ids,
            string.Empty,
            [],
            []);
    }

    private static IReadOnlyList<long> TTranslationPositionRead(
        TWorkspace workspace, string table, string column, string ownerId)
    {
        using Microsoft.Data.Sqlite.SqliteConnection connection = workspace.TWorkspaceConnectionRead();
        using Microsoft.Data.Sqlite.SqliteCommand command = connection.CreateCommand();
        command.CommandText =
            $"SELECT position FROM {table} WHERE {column} = $owner ORDER BY position;";
        command.Parameters.AddWithValue("$owner", ownerId);

        List<long> positions = [];
        using Microsoft.Data.Sqlite.SqliteDataReader reader = command.ExecuteReader();
        while (reader.Read())
        {
            positions.Add(reader.GetInt64(0));
        }

        return positions;
    }

    [Fact]
    public void ATypedWordResolvesOnlyToAHeadwordThatIsThatWholeWord()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = new(workspace.TWorkspaceFolder);
        LEntryArchive entries = new(workspace.TWorkspaceDatabase);

        LEntry breakfast = TTranslationEntryCreate(entries, "breakfast", "English");

        Assert.Null(engine.LEngineTranslationResolve("break", null));
        Assert.Equal("breakfast", Assert.Single(engine.LEngineTranslationFind("break", null)).LEntryHeadword);

        LEntry broken = TTranslationEntryCreate(entries, "break", "English");

        Assert.Equal(broken.LEntryId, engine.LEngineTranslationResolve("break", null)?.LEntryId);
        Assert.Equal(broken.LEntryId, engine.LEngineTranslationResolve("  BREAK  ", null)?.LEntryId);
        Assert.Equal(2, engine.LEngineTranslationFind("break", null).Count);
        Assert.NotEqual(breakfast.LEntryId, engine.LEngineTranslationResolve("break", null)?.LEntryId);
    }

    [Fact]
    public void TwoEntriesSharingAHeadwordSettleNothingOnTheirOwn()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = new(workspace.TWorkspaceFolder);
        LEntryArchive entries = new(workspace.TWorkspaceDatabase);

        TTranslationEntryCreate(entries, "부수다", "Korean");
        TTranslationEntryCreate(entries, "부수다", "Korean");

        Assert.Null(engine.LEngineTranslationResolve("부수다", null));
        Assert.Equal(2, engine.LEngineTranslationFind("부수다", null).Count);
    }

    [Fact]
    public void TheEntryBeingEditedIsNeverOfferedAsItsOwnTranslation()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = new(workspace.TWorkspaceFolder);
        LEntryArchive entries = new(workspace.TWorkspaceDatabase);

        LEntry source = TTranslationEntryCreate(entries, "break", "English");

        Assert.Null(engine.LEngineTranslationResolve("break", source.LEntryId));
        Assert.Empty(engine.LEngineTranslationFind("break", source.LEntryId));
        Assert.NotNull(engine.LEngineTranslationResolve("break", null));
    }

    [Fact]
    public void AStubMadeAndThenThrownAwayLeavesNothingBehind()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = new(workspace.TWorkspaceFolder);

        LEntry stub = engine.LEngineTranslationCreate("부수다", "Korean");

        Assert.NotNull(engine.LEngineEntryRead(stub.LEntryId));
        Assert.Contains(
            engine.LEngineChangeRead(engine.LEngineRevisionRead()!.LRevisionId),
            change => change.LRevisionChangeTarget == stub.LEntryId);

        engine.LEngineTranslationDelete(stub.LEntryId);

        Assert.Null(engine.LEngineEntryRead(stub.LEntryId));
    }

    [Fact]
    public void AStubSomethingAlreadyLinksToSurvivesTheDiscard()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = new(workspace.TWorkspaceFolder);
        LEntryArchive entries = new(workspace.TWorkspaceDatabase);
        LTranslationArchive translations = new(workspace.TWorkspaceDatabase);

        LEntry source = TTranslationEntryCreate(entries, "break", "English");
        string senseId = TTranslationSenseCreate(workspace, source.LEntryId);
        LEntry stub = engine.LEngineTranslationCreate("부수다", "Korean");
        translations.LTranslationSenseSave(senseId, [new LTranslation(stub.LEntryId, 0)]);

        engine.LEngineTranslationDelete(stub.LEntryId);

        Assert.NotNull(engine.LEngineEntryRead(stub.LEntryId));
        Assert.Single(translations.LTranslationSenseRead(senseId));
    }

    private static LEntry TTranslationEntryCreate(
        LEntryArchive entries, string headword, string language)
    {
        return entries.LEntryCreate(
            new LEntry(string.Empty, headword, language, null, null, null, null), [], []);
    }

    private static string TTranslationSenseCreate(TWorkspace workspace, string entryId)
    {
        LSenseArchive senses = new(workspace.TWorkspaceDatabase);
        return senses.LSenseCreate(new LSense(
            string.Empty, entryId, null, 0, null, "a meaning", null, null, string.Empty)).LSenseId;
    }

    private static string TTranslationCollocationCreate(TWorkspace workspace, string entryId)
    {
        LCollocationArchive collocations = new(workspace.TWorkspaceDatabase);
        return collocations.LCollocationCreate(new LCollocation(
            string.Empty, entryId, 0, null, "in a word", "briefly")).LCollocationId;
    }
}
