using Llyn.Core;
using Llyn.Infrastructure;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TSituationMedia
{
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

        Assert.Equal(1, workspace.TWorkspaceCountRead(
            $"SELECT COUNT(*) FROM situation_image WHERE image_ref = {stored.LSituationImage[0].LImageDraftId};"));
        Assert.Equal(1, workspace.TWorkspaceCountRead(
            $"SELECT COUNT(*) FROM situation_video WHERE video_ref = {stored.LSituationVideo[0].LVideoDraftId};"));
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
        Assert.Equal(0, workspace.TWorkspaceCountRead(
            $"SELECT COUNT(*) FROM situation_image WHERE image_ref = {droppedId};"));
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

        IReadOnlyList<LSituation> read =
        [
            .. engine.TEngineSituationFind(string.Empty, LCatalogOrder.LCatalogOrderEarliest)
                .Select(row => row.LCatalogSituationStored),
        ];


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

        engine.TEngineSituationDelete(stored.LSituationId, false);

        Assert.Null(engine.TEngineSituationRead(stored.LSituationId));
        Assert.Equal(0, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM situation_image;"));
        Assert.Equal(0, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM situation_video;"));
        Assert.Equal(1, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM image;"));
        Assert.Equal(1, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM video;"));
    }
}
