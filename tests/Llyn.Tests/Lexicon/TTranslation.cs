using Llyn.Core;
using Llyn.Infrastructure;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TTranslation
{
    [Fact]
    public void TranslationMeaningSave_CardWithLinks_ReadsBackInOrder()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        LEntryArchive entries = TInterface.TEntryArchiveCreate(workspace.TWorkspaceDatabase);
        LTranslationArchive translations = TInterface.TTranslationArchiveCreate(workspace.TWorkspaceDatabase);

        LEntry source = TTranslationEntryCreate(entries, "break", "English");
        long meaningId = TTranslationMeaningCreate(workspace, source.LEntryId);
        LEntry first = TTranslationEntryCreate(entries, "부수다", "Korean");
        LEntry second = TTranslationEntryCreate(entries, "깨다", "Korean");
        LEntry third = TTranslationEntryCreate(entries, "부러뜨리다", "Korean");

        translations.TTranslationMeaningSave(
            meaningId,
            [
                TInterface.TTranslationCreate(first.LEntryId, 0),
                TInterface.TTranslationCreate(second.LEntryId, 0),
                TInterface.TTranslationCreate(third.LEntryId, 0),
            ]);

        Assert.Equal(
            [first.LEntryId, second.LEntryId, third.LEntryId],
            translations.TTranslationMeaningRead(meaningId)
                .Select(translation => translation.LTranslationEntryId));
        Assert.Equal(
            [0, 1, 2],
            translations.TTranslationMeaningRead(meaningId)
                .Select(translation => translation.LTranslationPosition));
    }

    [Fact]
    public void TranslationMeaningSave_SameEntryTwice_KeepsOneLink()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        LEntryArchive entries = TInterface.TEntryArchiveCreate(workspace.TWorkspaceDatabase);
        LTranslationArchive translations = TInterface.TTranslationArchiveCreate(workspace.TWorkspaceDatabase);

        LEntry source = TTranslationEntryCreate(entries, "break", "English");
        long meaningId = TTranslationMeaningCreate(workspace, source.LEntryId);
        LEntry first = TTranslationEntryCreate(entries, "부수다", "Korean");
        LEntry second = TTranslationEntryCreate(entries, "깨다", "Korean");

        translations.TTranslationMeaningSave(
            meaningId,
            [
                TInterface.TTranslationCreate(first.LEntryId, 0),
                TInterface.TTranslationCreate(first.LEntryId, 0),
                TInterface.TTranslationCreate(0, 0),
                TInterface.TTranslationCreate(second.LEntryId, 0),
            ]);

        Assert.Equal(
            [first.LEntryId, second.LEntryId],
            translations.TTranslationMeaningRead(meaningId)
                .Select(translation => translation.LTranslationEntryId));
        Assert.Equal(
            [0, 1],
            TTranslationPositionRead(workspace, "sense_translation", "sense_parent", meaningId));
    }

    [Fact]
    public void TranslationCollocationSave_ShorterList_Renumbers()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        LEntryArchive entries = TInterface.TEntryArchiveCreate(workspace.TWorkspaceDatabase);
        LTranslationArchive translations = TInterface.TTranslationArchiveCreate(workspace.TWorkspaceDatabase);

        LEntry source = TTranslationEntryCreate(entries, "break", "English");
        long collocationId = TTranslationCollocationCreate(workspace, source.LEntryId);
        LEntry first = TTranslationEntryCreate(entries, "부수다", "Korean");
        LEntry second = TTranslationEntryCreate(entries, "깨다", "Korean");
        LEntry third = TTranslationEntryCreate(entries, "부러뜨리다", "Korean");

        translations.TTranslationCollocationSave(
            collocationId,
            [
                TInterface.TTranslationCreate(first.LEntryId, 0),
                TInterface.TTranslationCreate(second.LEntryId, 0),
                TInterface.TTranslationCreate(third.LEntryId, 0),
            ]);
        translations.TTranslationCollocationSave(
            collocationId,
            [TInterface.TTranslationCreate(third.LEntryId, 0), TInterface.TTranslationCreate(first.LEntryId, 0)]);

        Assert.Equal(
            [third.LEntryId, first.LEntryId],
            translations.TTranslationCollocationRead(collocationId)
                .Select(translation => translation.LTranslationEntryId));
        Assert.Equal(
            [0, 1],
            TTranslationPositionRead(
                workspace, "collocation_translation", "collocation_parent", collocationId));
    }

    [Fact]
    public void EntryDelete_LinkTarget_RemovesLinksKeepsCard()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        LEntryArchive entries = TInterface.TEntryArchiveCreate(workspace.TWorkspaceDatabase);
        LTranslationArchive translations = TInterface.TTranslationArchiveCreate(workspace.TWorkspaceDatabase);

        LEntry source = TTranslationEntryCreate(entries, "break", "English");
        long meaningId = TTranslationMeaningCreate(workspace, source.LEntryId);
        LEntry first = TTranslationEntryCreate(entries, "부수다", "Korean");
        LEntry second = TTranslationEntryCreate(entries, "깨다", "Korean");

        translations.TTranslationMeaningSave(
            meaningId,
            [TInterface.TTranslationCreate(first.LEntryId, 0), TInterface.TTranslationCreate(second.LEntryId, 0)]);

        entries.TEntryDelete(first.LEntryId);

        Assert.Equal(
            [second.LEntryId],
            translations.TTranslationMeaningRead(meaningId)
                .Select(translation => translation.LTranslationEntryId));
        Assert.Equal(1, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM sense;"));
    }

    [Fact]
    public void TranslationTargetRead_UnknownId_PassesOverIt()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        LEntryArchive entries = TInterface.TEntryArchiveCreate(workspace.TWorkspaceDatabase);
        LTranslationArchive translations = TInterface.TTranslationArchiveCreate(workspace.TWorkspaceDatabase);

        LEntry first = TTranslationEntryCreate(entries, "부수다", "Korean");
        LEntry second = TTranslationEntryCreate(entries, "깨다", "Korean");

        IReadOnlyList<LTranslationTarget> targets =
            translations.TTranslationTargetRead([second.LEntryId, 9999, first.LEntryId]);

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
    public void TranslationIncomingRead_EntryPointedAt_FindsBothCards()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        LEntryArchive entries = TInterface.TEntryArchiveCreate(workspace.TWorkspaceDatabase);
        LTranslationArchive translations = TInterface.TTranslationArchiveCreate(workspace.TWorkspaceDatabase);

        LEntry source = TTranslationEntryCreate(entries, "break", "English");
        long meaningId = TTranslationMeaningCreate(workspace, source.LEntryId);
        long collocationId = TTranslationCollocationCreate(workspace, source.LEntryId);
        LEntry target = TTranslationEntryCreate(entries, "부수다", "Korean");

        translations.TTranslationMeaningSave(meaningId, [TInterface.TTranslationCreate(target.LEntryId, 0)]);
        translations.TTranslationCollocationSave(
            collocationId, [TInterface.TTranslationCreate(target.LEntryId, 0)]);

        IReadOnlyList<LUsage> incoming = translations.TTranslationIncomingRead(target.LEntryId);

        Assert.Equal([meaningId, collocationId], incoming.Select(usage => usage.LUsageId));
        Assert.Equal(
            [LOwner.LOwnerMeaning, LOwner.LOwnerCollocation],
            incoming.Select(usage => usage.LUsageOwner));
        Assert.Equal(["break", "break"], incoming.Select(usage => usage.LUsageHeadword));
        Assert.Equal(
            [source.LEntryId, source.LEntryId], incoming.Select(usage => usage.LUsageEntry));
        Assert.Empty(translations.TTranslationIncomingRead(source.LEntryId));
    }

    [Fact]
    public void EntrySave_CardWithLinks_ReadsBackWithLinks()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LEntry first = engine.TEngineTranslationCreate("부수다", "Korean");
        LEntry second = engine.TEngineTranslationCreate("깨다", "Korean");

        LEntry stored = engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "break",
            "English",
            string.Empty,
            string.Empty,
            [TTranslationCardCreate("to come apart", [first.LEntryId, second.LEntryId])],
            []));

        LEntryDraft? loaded = engine.TEngineEntryLoad(stored.LEntryId);

        Assert.NotNull(loaded);
        Assert.Equal(
            [first.LEntryId, second.LEntryId],
            Assert.Single(loaded.LEntryDraftMeanings).LCardDraftTranslation);
        Assert.Equal(
            ["break"],
            engine.TEngineIncomingRead(first.LEntryId).Select(usage => usage.LUsageHeadword));
    }

    [Fact]
    public void EntryUpdate_OneLinkRemoved_DropsRowKeepsOther()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LEntry first = engine.TEngineTranslationCreate("부수다", "Korean");
        LEntry second = engine.TEngineTranslationCreate("깨다", "Korean");

        LEntry stored = engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "break",
            "English",
            string.Empty,
            string.Empty,
            [TTranslationCardCreate("to come apart", [first.LEntryId, second.LEntryId])],
            []));

        LEntryDraft? loaded = engine.TEngineEntryLoad(stored.LEntryId);

        Assert.NotNull(loaded);
        Assert.Equal(2, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM sense_translation;"));

        LCardDraft card = loaded.LEntryDraftMeanings[0];
        engine.TEngineEntryUpdate(stored.LEntryId, loaded with
        {
            LEntryDraftMeanings = [card with { LCardDraftTranslation = [second.LEntryId] }],
        });

        Assert.Equal(1, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM sense_translation;"));

        LEntryDraft? reloaded = engine.TEngineEntryLoad(stored.LEntryId);

        Assert.NotNull(reloaded);
        Assert.Equal(
            second.LEntryId,
            Assert.Single(Assert.Single(reloaded.LEntryDraftMeanings).LCardDraftTranslation));
    }

    [Fact]
    public void EntrySave_CardWithOnlyALink_StoresCard()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LEntry target = engine.TEngineTranslationCreate("부수다", "Korean");

        LEntry stored = engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "break",
            "English",
            string.Empty,
            string.Empty,
            [TTranslationCardCreate(string.Empty, [target.LEntryId])],
            []));

        LEntryDraft? loaded = engine.TEngineEntryLoad(stored.LEntryId);

        Assert.NotNull(loaded);
        Assert.Equal(
            target.LEntryId,
            Assert.Single(Assert.Single(loaded.LEntryDraftMeanings).LCardDraftTranslation));
    }

    private static LCardDraft TTranslationCardCreate(string meaning, IReadOnlyList<long> ids)
    {
        return TInterface.TCardDraftCreate(
            string.Empty,
            string.Empty,
            meaning,
            [],
            [],
            ids,
            [],
            [],
            1);
    }

    private static IReadOnlyList<long> TTranslationPositionRead(
        TWorkspace workspace, string table, string column, long ownerId)
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
    public void TranslationResolve_TypedWord_MatchesWholeHeadword()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LEntryArchive entries = TInterface.TEntryArchiveCreate(workspace.TWorkspaceDatabase);

        LEntry breakfast = TTranslationEntryCreate(entries, "breakfast", "English");

        Assert.Null(engine.TEngineTranslationResolve("break", null));
        Assert.Equal("breakfast", Assert.Single(engine.TEngineTranslationFind("break", null)).LEntryHeadword);

        LEntry broken = TTranslationEntryCreate(entries, "break", "English");

        Assert.Equal(broken.LEntryId, engine.TEngineTranslationResolve("break", null)?.LEntryId);
        Assert.Equal(broken.LEntryId, engine.TEngineTranslationResolve("  BREAK  ", null)?.LEntryId);
        Assert.Equal(2, engine.TEngineTranslationFind("break", null).Count);
        Assert.NotEqual(breakfast.LEntryId, engine.TEngineTranslationResolve("break", null)?.LEntryId);
    }

    [Fact]
    public void TranslationResolve_SharedHeadword_SettlesNothing()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LEntryArchive entries = TInterface.TEntryArchiveCreate(workspace.TWorkspaceDatabase);

        TTranslationEntryCreate(entries, "부수다", "Korean");
        TTranslationEntryCreate(entries, "부수다", "Korean");

        Assert.Null(engine.TEngineTranslationResolve("부수다", null));
        Assert.Equal(2, engine.TEngineTranslationFind("부수다", null).Count);
    }

    [Fact]
    public void TranslationResolve_EntryBeingEdited_NeverOffersItself()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LEntryArchive entries = TInterface.TEntryArchiveCreate(workspace.TWorkspaceDatabase);

        LEntry source = TTranslationEntryCreate(entries, "break", "English");

        Assert.Null(engine.TEngineTranslationResolve("break", source.LEntryId));
        Assert.Empty(engine.TEngineTranslationFind("break", source.LEntryId));
        Assert.NotNull(engine.TEngineTranslationResolve("break", null));
    }

    private static LEntry TTranslationEntryCreate(
        LEntryArchive entries, string headword, string language)
    {
        return entries.TEntryCreate(
            TInterface.TEntryCreate(0, headword, language, null, null, null, null), [], []);
    }

    private static long TTranslationMeaningCreate(TWorkspace workspace, long entryId)
    {
        LMeaningArchive meanings = TInterface.TMeaningArchiveCreate(workspace.TWorkspaceDatabase);
        return meanings.TMeaningCreate(TInterface.TMeaningCreate(
            0, entryId, null, 0, null, "a meaning")).LMeaningId;
    }

    private static long TTranslationCollocationCreate(TWorkspace workspace, long entryId)
    {
        LCollocationArchive collocations = TInterface.TCollocationArchiveCreate(workspace.TWorkspaceDatabase);
        return collocations.TCollocationCreate(TInterface.TCollocationCreate(
            0, entryId, 0, null, "in a word", "briefly")).LCollocationId;
    }
}
