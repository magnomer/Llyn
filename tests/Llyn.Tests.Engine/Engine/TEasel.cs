using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TEasel
{
    private const int TEaselHold = 600000;

    [Fact]
    public void ImageAdd_FreshSituation_AddsEmptyRow()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LTenure tenure = TEaselStart(engine, 0);

        tenure.TEaselCreate().TEaselImageAdd(0);

        LImageDraft added = Assert.Single(tenure.TTenureRead()!.LDraftSituation!.LSituationImage);
        Assert.True(added.LImageDraftLocation.LStateValueEmpty);
    }

    [Fact]
    public void ImageRemove_AddedRow_DropsIt()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LTenure tenure = TEaselStart(engine, 0);
        LEasel easel = tenure.TEaselCreate();

        easel.TEaselImageAdd(0);
        easel.TEaselImageRemove(tenure.TTenureRead()!.LDraftSituation!.LSituationImage[0].LImageDraftId);

        Assert.Empty(tenure.TTenureRead()!.LDraftSituation!.LSituationImage);
    }

    [Fact]
    public void ImageSet_Deferred_WritesOnlyAfterFlush()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LTenure tenure = TEaselStart(engine, TEaselHold);
        LEasel easel = tenure.TEaselCreate();

        easel.TEaselImageAdd(0);
        long image = tenure.TTenureRead()!.LDraftSituation!.LSituationImage[0].LImageDraftId;
        easel.TEaselImageSet(image, "https://example.test/a.png", true);

        Assert.True(tenure.TTenureRead()!.LDraftSituation!.LSituationImage[0].LImageDraftLocation.LStateValueEmpty);

        tenure.TTenurePersist();

        Assert.Equal(
            "https://example.test/a.png",
            tenure.TTenureRead()!.LDraftSituation!.LSituationImage[0].LImageDraftLocation.TStateValueShow());
    }

    [Fact]
    public void VideoAdd_FreshSituation_AddsEmptyRow()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LTenure tenure = TEaselStart(engine, 0);

        tenure.TEaselCreate().TEaselVideoAdd(0);

        LVideoDraft added = Assert.Single(tenure.TTenureRead()!.LDraftSituation!.LSituationVideo);
        Assert.True(added.LVideoDraftLocation.LStateValueEmpty);
    }

    [Fact]
    public void VideoRemove_AddedRow_DropsIt()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LTenure tenure = TEaselStart(engine, 0);
        LEasel easel = tenure.TEaselCreate();

        easel.TEaselVideoAdd(0);
        easel.TEaselVideoRemove(tenure.TTenureRead()!.LDraftSituation!.LSituationVideo[0].LVideoDraftId);

        Assert.Empty(tenure.TTenureRead()!.LDraftSituation!.LSituationVideo);
    }

    [Fact]
    public void VideoSet_ChosenFile_WritesAtOnce()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LTenure tenure = TEaselStart(engine, TEaselHold);
        LEasel easel = tenure.TEaselCreate();

        easel.TEaselVideoAdd(0);
        long video = tenure.TTenureRead()!.LDraftSituation!.LSituationVideo[0].LVideoDraftId;
        easel.TEaselVideoSet(video, "https://example.test/a.mp4", false);

        Assert.Equal(
            "https://example.test/a.mp4",
            tenure.TTenureRead()!.LDraftSituation!.LSituationVideo[0].LVideoDraftLocation.TStateValueShow());
    }

    [Fact]
    public void SpanSet_Typed_WritesSpan()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LTenure tenure = TEaselStart(engine, 0);
        LEasel easel = tenure.TEaselCreate();

        easel.TEaselVideoAdd(0);
        long video = tenure.TTenureRead()!.LDraftSituation!.LSituationVideo[0].LVideoDraftId;
        easel.TEaselSpanSet(video, "1:05");

        Assert.Equal(
            "1:05", tenure.TTenureRead()!.LDraftSituation!.LSituationVideo[0].LVideoDraftSpan.TStateValueShow());
    }

    private static LTenure TEaselStart(LEngine engine, int delay)
    {
        engine.TEngineDelaySet(delay);
        return engine.TEngineTenureStart("test", LSubject.LSubjectSituation, null);
    }
}
