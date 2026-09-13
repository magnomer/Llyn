using Llyn.Core;
using Llyn.Infrastructure;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TSituation
{
    [Fact]
    public void SituationUpdate_OnManyCards_ShowsNewWordingOnEach()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LEntry entry = TSituationEntryCreate(engine);
        long meaningId = engine.TEngineMeaningRead(entry.LEntryId, LOwner.LOwnerEntry)[0].LMeaningId;
        long collocationId =
            engine.TEngineCollocationRead(entry.LEntryId, LOwner.LOwnerEntry)[0].LCollocationId;

        LSituation situation = engine.TEngineSituationCreate(
            TInterface.TSituationCreate(0, "at a funeral", null, null));
        engine.TEngineSituationAttach(meaningId, situation.LSituationId, 0, LOwner.LOwnerMeaning);
        engine.TEngineSituationAttach(
            collocationId, situation.LSituationId, 0, LOwner.LOwnerCollocation);

        engine.TEngineSituationUpdate(situation with
        {
            LSituationTitle = "at a memorial",
            LSituationDescription = "spoken to the bereaved",
            LSituationKind = "register",
        });

        LSituation read = Assert.Single(engine.TEngineSituationRead(meaningId, LOwner.LOwnerMeaning));
        Assert.Equal("at a memorial", read.LSituationTitle);
        Assert.Equal(
            "at a memorial",
            Assert.Single(engine.TEngineSituationRead(collocationId, LOwner.LOwnerCollocation))
                .LSituationTitle);
        Assert.Equal("spoken to the bereaved", engine.TEngineSituationRead(
            situation.LSituationId)?.LSituationDescription);
    }

    [Fact]
    public void SituationRead_CardHoldingSeveral_ReturnsCardOrder()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LEntry entry = TSituationEntryCreate(engine);
        long meaningId = engine.TEngineMeaningRead(entry.LEntryId, LOwner.LOwnerEntry)[0].LMeaningId;

        LSituation first = engine.TEngineSituationCreate(
            TInterface.TSituationCreate(0, "in court", null, null));
        LSituation second = engine.TEngineSituationCreate(
            TInterface.TSituationCreate(0, "at home", null, null));

        engine.TEngineSituationAttach(meaningId, first.LSituationId, 0, LOwner.LOwnerMeaning);
        engine.TEngineSituationAttach(meaningId, second.LSituationId, 0, LOwner.LOwnerMeaning);

        Assert.Equal(
            ["at home", "in court"],
            engine.TEngineSituationRead(meaningId, LOwner.LOwnerMeaning)
                .Select(row => row.LSituationTitle));
    }

    [Fact]
    public void SituationRemove_LastReferenceGone_DeletesSituation()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LEntry entry = TSituationEntryCreate(engine);
        long meaningId = engine.TEngineMeaningRead(entry.LEntryId, LOwner.LOwnerEntry)[0].LMeaningId;

        LSituation situation = engine.TEngineSituationCreate(
            TInterface.TSituationCreate(0, "in court", null, null));
        engine.TEngineSituationAttach(meaningId, situation.LSituationId, 0, LOwner.LOwnerMeaning);

        Assert.Throws<InvalidOperationException>(() =>
            engine.TEngineSituationDelete(situation.LSituationId));

        engine.TEngineSituationDetach(meaningId, situation.LSituationId, LOwner.LOwnerMeaning);
        Assert.NotNull(engine.TEngineSituationRead(situation.LSituationId));

        engine.TEngineSituationAttach(meaningId, situation.LSituationId, 0, LOwner.LOwnerMeaning);
        engine.TEngineSituationRemove(meaningId, situation.LSituationId, LOwner.LOwnerMeaning);
        Assert.Null(engine.TEngineSituationRead(situation.LSituationId));
    }

    [Fact]
    public void EntryUpdate_SituationEdited_KeepsId()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LEntry entry = engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "word",
            "English",
            string.Empty,
            string.Empty,
            [TInterface.TCardDraftCreate(
                string.Empty,
                string.Empty,
                "a unit of language",
                [],
                [
                    TInterface.TSituationDraftCreate("in conversation"),
                    TInterface.TSituationDraftCreate("in court"),
                    TInterface.TSituationDraftCreate("at home"),
                ],
                [],
                [], [], 1)],
            []));

        long meaningId = engine.TEngineMeaningRead(entry.LEntryId, LOwner.LOwnerEntry)[0].LMeaningId;
        Assert.Equal(
            ["in conversation", "in court", "at home"],
            engine.TEngineSituationRead(meaningId, LOwner.LOwnerMeaning).Select(row => row.LSituationTitle));

        LEntryDraft loaded = Assert.IsType<LEntryDraft>(engine.TEngineEntryLoad(entry.LEntryId));
        LCardDraft card = loaded.LEntryDraftMeanings[0];
        Assert.Equal(3, card.LCardDraftSituation.Count);
        foreach (LSituationDraft draft in card.LCardDraftSituation)
        {
            Assert.NotEqual(0, draft.LSituationDraftId);
        }

        LReference reference = engine.TEngineReferenceCreate(TInterface.TReferenceCreate(
            0,
            TInterface.TStateValueCreate("A Dictionary"),
            LStateValue.LStateValueUnspecified,
            LReferenceKind.LReferenceKindUnspecified,
            LStateValue.LStateValueUnspecified,
            LStateValue.LStateValueUnspecified,
            LStateMark.LStateMarkUnspecified));

        engine.TEngineEntryUpdate(entry.LEntryId, loaded with
        {
            LEntryDraftMeanings =
            [
                card with
                {
                    LCardDraftSituation =
                    [
                        card.LCardDraftSituation[1] with
                        {
                            LSituationDraftTitle = "in a courtroom",
                        },
                        card.LCardDraftSituation[0],
                        TInterface.TSituationDraftCreate("in a letter"),
                    ],
                },
            ],
        });

        long citedId = card.LCardDraftSituation[1].LSituationDraftId;
        IReadOnlyList<LSituation> attached = engine.TEngineSituationRead(meaningId, LOwner.LOwnerMeaning);
        Assert.Equal(
            ["in a courtroom", "in conversation", "in a letter"],
            attached.Select(row => row.LSituationTitle));
        Assert.Equal(citedId, attached[0].LSituationId);
        Assert.Equal(card.LCardDraftSituation[0].LSituationDraftId, attached[1].LSituationId);

        Assert.NotNull(engine.TEngineSituationRead(card.LCardDraftSituation[2].LSituationDraftId));
        Assert.Equal(4, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM situation;"));
        Assert.Equal(3, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM sense_situation;"));
    }

    [Fact]
    public void SituationRead_WorkspaceShelf_ReturnsReferenceCounts()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LEntry entry = TSituationEntryCreate(engine);
        long meaningId = engine.TEngineMeaningRead(entry.LEntryId, LOwner.LOwnerEntry)[0].LMeaningId;
        long collocationId =
            engine.TEngineCollocationRead(entry.LEntryId, LOwner.LOwnerEntry)[0].LCollocationId;

        LSituation shared = engine.TEngineSituationCreate(
            TInterface.TSituationCreate(0, "in court", null, null));
        LSituation lonely = engine.TEngineSituationCreate(
            TInterface.TSituationCreate(0, "at home", null, null));

        engine.TEngineSituationAttach(meaningId, shared.LSituationId, 0, LOwner.LOwnerMeaning);
        engine.TEngineSituationAttach(collocationId, shared.LSituationId, 0, LOwner.LOwnerCollocation);

        Assert.Equal(
            ["in court", "at home"],
            engine.TEngineSituationRead().Select(row => row.LSituationTitle.TStateValueShow()));

        IReadOnlyDictionary<long, int> counts = engine.TEngineUsageRead(LOwner.LOwnerSituation);
        Assert.Equal(2, counts[shared.LSituationId]);
        Assert.DoesNotContain(lonely.LSituationId, counts);
    }

    [Fact]
    public void UsageRead_SituationOnCard_NamesSideAndEntry()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LEntry entry = TSituationEntryCreate(engine);
        long meaningId = engine.TEngineMeaningRead(entry.LEntryId, LOwner.LOwnerEntry)[0].LMeaningId;
        long collocationId =
            engine.TEngineCollocationRead(entry.LEntryId, LOwner.LOwnerEntry)[0].LCollocationId;

        LSituation situation = engine.TEngineSituationCreate(
            TInterface.TSituationCreate(0, "in court", null, null));
        engine.TEngineSituationAttach(meaningId, situation.LSituationId, 0, LOwner.LOwnerMeaning);
        engine.TEngineSituationAttach(collocationId, situation.LSituationId, 0, LOwner.LOwnerCollocation);

        IReadOnlyList<LUsage> usage = engine.TEngineUsageRead(situation.LSituationId, LOwner.LOwnerSituation);
        Assert.Equal(2, usage.Count);

        LUsage meaning = usage.Single(row => row.LUsageOwner == LOwner.LOwnerMeaning);
        Assert.Equal(meaningId, meaning.LUsageId);
        Assert.Equal(entry.LEntryId, meaning.LUsageEntry);
        Assert.Equal("word", meaning.LUsageHeadword);
        Assert.Equal("English", meaning.LUsageLanguage);
        Assert.Equal("a meaning", meaning.LUsageTitle.TStateValueShow());

        LUsage collocation = usage.Single(row => row.LUsageOwner == LOwner.LOwnerCollocation);
        Assert.Equal(collocationId, collocation.LUsageId);
        Assert.Equal("in a word", collocation.LUsageTitle.TStateValueShow());

        Assert.Empty(engine.TEngineUsageRead(
            engine.TEngineSituationCreate(TInterface.TSituationCreate(0, "at home", null, null))
                .LSituationId,
            LOwner.LOwnerSituation));
    }

    [Fact]
    public void SituationDelete_DetachingDelete_DropsAndRenumbers()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LEntry entry = TSituationEntryCreate(engine);
        long meaningId = engine.TEngineMeaningRead(entry.LEntryId, LOwner.LOwnerEntry)[0].LMeaningId;
        long collocationId =
            engine.TEngineCollocationRead(entry.LEntryId, LOwner.LOwnerEntry)[0].LCollocationId;

        LSituation first = engine.TEngineSituationCreate(
            TInterface.TSituationCreate(0, "in court", null, null));
        LSituation second = engine.TEngineSituationCreate(
            TInterface.TSituationCreate(0, "at home", null, null));

        engine.TEngineSituationAttach(meaningId, first.LSituationId, 0, LOwner.LOwnerMeaning);
        engine.TEngineSituationAttach(meaningId, second.LSituationId, 1, LOwner.LOwnerMeaning);
        engine.TEngineSituationAttach(collocationId, first.LSituationId, 0, LOwner.LOwnerCollocation);

        engine.TEngineSituationDelete(first.LSituationId, true);

        Assert.Null(engine.TEngineSituationRead(first.LSituationId));
        Assert.Equal(
            ["at home"],
            engine.TEngineSituationRead(meaningId, LOwner.LOwnerMeaning)
                .Select(row => row.LSituationTitle.TStateValueShow()));
        Assert.Empty(engine.TEngineSituationRead(collocationId, LOwner.LOwnerCollocation));
        Assert.Equal(0, workspace.TWorkspaceCountRead(
            "SELECT COUNT(*) FROM sense_situation WHERE position <> 0;"));

        LSituation third = engine.TEngineSituationCreate(
            TInterface.TSituationCreate(0, "in a letter", null, null));
        engine.TEngineSituationAttach(meaningId, third.LSituationId, 1, LOwner.LOwnerMeaning);
        Assert.Equal(
            ["at home", "in a letter"],
            engine.TEngineSituationRead(meaningId, LOwner.LOwnerMeaning)
                .Select(row => row.LSituationTitle.TStateValueShow()));
    }

    [Fact]
    public void SituationCreate_WithMedia_ReadsBackInOrder()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LSituation stored = engine.TEngineSituationCreate(
            TInterface.TSituationCreate(0, "in court", null, null) with
            {
                LSituationImage =
                [
                    TInterface.TImageDraftCreate("court-a.png"),
                    TInterface.TImageDraftCreate("court-b.png"),
                ],
                LSituationVideo = [TInterface.TVideoDraftCreate("court.mp4", "00:10-00:40")],
            });

        Assert.All(stored.LSituationImage, row => Assert.NotEqual(0, row.LImageDraftId));
        Assert.All(stored.LSituationVideo, row => Assert.NotEqual(0, row.LVideoDraftId));

        LSituation read = Assert.IsType<LSituation>(engine.TEngineSituationRead(stored.LSituationId));
        Assert.Equal(
            ["court-a.png", "court-b.png"],
            read.LSituationImage.Select(row => row.LImageDraftLocation.TStateValueShow()));
        Assert.Equal(
            ["court.mp4"],
            read.LSituationVideo.Select(row => row.LVideoDraftLocation.TStateValueShow()));
        Assert.Equal("00:10-00:40", read.LSituationVideo[0].LVideoDraftSpan.TStateValueShow());
        Assert.Equal(stored, read);

        LImageArchive images = TInterface.TImageArchiveCreate(workspace.TWorkspaceDatabase);
        Assert.Equal(1, images.TImageReferenceRead(stored.LSituationImage[0].LImageDraftId));
        Assert.Equal(
            1,
            TInterface.TVideoArchiveCreate(workspace.TWorkspaceDatabase)
                .TVideoReferenceRead(stored.LSituationVideo[0].LVideoDraftId));
    }

    [Fact]
    public void SituationUpdate_MediaDropped_DetachesAndKeepsRecord()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LSituation stored = engine.TEngineSituationCreate(
            TInterface.TSituationCreate(0, "in court", null, null) with
            {
                LSituationImage =
                [
                    TInterface.TImageDraftCreate("court-a.png"),
                    TInterface.TImageDraftCreate("court-b.png"),
                ],
                LSituationVideo = [TInterface.TVideoDraftCreate("court.mp4")],
            });
        long droppedId = stored.LSituationImage[0].LImageDraftId;

        engine.TEngineSituationUpdate(stored with
        {
            LSituationImage =
            [
                stored.LSituationImage[1],
                TInterface.TImageDraftCreate("court-c.png"),
            ],
            LSituationVideo = [],
        });

        LSituation read = Assert.IsType<LSituation>(engine.TEngineSituationRead(stored.LSituationId));
        Assert.Equal(
            ["court-b.png", "court-c.png"],
            read.LSituationImage.Select(row => row.LImageDraftLocation.TStateValueShow()));
        Assert.Equal(stored.LSituationImage[1].LImageDraftId, read.LSituationImage[0].LImageDraftId);
        Assert.Empty(read.LSituationVideo);

        LImageArchive images = TInterface.TImageArchiveCreate(workspace.TWorkspaceDatabase);
        Assert.NotNull(images.TImageRead(droppedId));
        Assert.Equal(0, images.TImageReferenceRead(droppedId));
        Assert.NotNull(TInterface.TVideoArchiveCreate(workspace.TWorkspaceDatabase)
            .TVideoRead(stored.LSituationVideo[0].LVideoDraftId));
        Assert.Equal(0, workspace.TWorkspaceCountRead(
            "SELECT COUNT(*) FROM situation_image WHERE position > 1;"));
    }

    [Fact]
    public void SituationRead_ListWithMedia_GroupsEveryRowUnderItsOwner()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LSituation first = engine.TEngineSituationCreate(
            TInterface.TSituationCreate(0, "in court", null, null) with
            {
                LSituationImage =
                [
                    TInterface.TImageDraftCreate("court-a.png"),
                    TInterface.TImageDraftCreate("court-b.png"),
                ],
            });
        LSituation second = engine.TEngineSituationCreate(
            TInterface.TSituationCreate(0, "at sea", null, null) with
            {
                LSituationVideo = [TInterface.TVideoDraftCreate("sea.mp4", "00:01-00:02")],
            });
        LSituation third = engine.TEngineSituationCreate(TInterface.TSituationCreate(0, "bare", null, null));

        IReadOnlyList<LSituation> read = engine.TEngineSituationRead();

        LSituation court = Assert.Single(read, row => row.LSituationId == first.LSituationId);
        Assert.Equal(
            ["court-a.png", "court-b.png"],
            court.LSituationImage.Select(row => row.LImageDraftLocation.TStateValueShow()));
        Assert.Empty(court.LSituationVideo);

        LSituation sea = Assert.Single(read, row => row.LSituationId == second.LSituationId);
        Assert.Empty(sea.LSituationImage);
        Assert.Equal("00:01-00:02", Assert.Single(sea.LSituationVideo).LVideoDraftSpan.TStateValueShow());

        LSituation bare = Assert.Single(read, row => row.LSituationId == third.LSituationId);
        Assert.Empty(bare.LSituationImage);
        Assert.Empty(bare.LSituationVideo);
    }

    [Fact]
    public void SituationDelete_WithMedia_DropsLinksAndKeepsRecords()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LSituation stored = engine.TEngineSituationCreate(
            TInterface.TSituationCreate(0, "in court", null, null) with
            {
                LSituationImage = [TInterface.TImageDraftCreate("court.png")],
                LSituationVideo = [TInterface.TVideoDraftCreate("court.mp4")],
            });

        engine.TEngineSituationDelete(stored.LSituationId);

        Assert.Null(engine.TEngineSituationRead(stored.LSituationId));
        Assert.Equal(0, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM situation_image;"));
        Assert.Equal(0, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM situation_video;"));
        Assert.Equal(1, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM image;"));
        Assert.Equal(1, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM video;"));
    }

    private static LEntry TSituationEntryCreate(LEngine engine)
    {
        return engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "word",
            "English",
            string.Empty,
            string.Empty,
            [TInterface.TCardDraftCreate(string.Empty, string.Empty, "a meaning", [], [], [], [], [], 1)],
            [TInterface.TCardDraftCreate(string.Empty, "in a word", "briefly", [], [], [], [], [], 1)]));
    }
}
