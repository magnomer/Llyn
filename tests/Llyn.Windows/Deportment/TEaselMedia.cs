using Llyn.Core;
using Llyn.ShellEngine;
using Llyn.UIDeportment;
using Xunit;

namespace Llyn.Tests;

public sealed class TEaselMedia
{
    [Fact]
    public void ImageSet_AddedRow_WritesLocationAndRemoveDropsIt()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LDesk desk = TInterfaceDeportment.TDeskCreate(engine, "Situation", static () => false);
        desk.TDeskSituationStart(null);

        desk.TEaselImageAdd(0);
        long image = desk.TDeskRead()!.LDraftSituation!.LSituationImage[0].LImageDraftId;
        desk.TEaselImageSet(image, "https://example.test/a.png", true);

        Assert.Equal(
            "https://example.test/a.png",
            desk.TDeskRead()!.LDraftSituation!.LSituationImage[0].LImageDraftLocation.TStateValueShow());

        desk.TEaselImageRemove(image);

        Assert.Empty(desk.TDeskRead()!.LDraftSituation!.LSituationImage);
    }

    [Fact]
    public void VideoSet_ChosenFileAndSpan_WritesBothAndRemoveDropsRow()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LDesk desk = TInterfaceDeportment.TDeskCreate(engine, "Situation", static () => false);
        desk.TDeskSituationStart(null);

        desk.TEaselVideoAdd(0);
        long video = desk.TDeskRead()!.LDraftSituation!.LSituationVideo[0].LVideoDraftId;
        desk.TEaselVideoSet(video, "https://example.test/a.mp4", false);
        desk.TEaselSpanSet(video, "1:05");

        LVideoDraft held = desk.TDeskRead()!.LDraftSituation!.LSituationVideo[0];
        Assert.Equal("https://example.test/a.mp4", held.LVideoDraftLocation.TStateValueShow());
        Assert.Equal("1:05", held.LVideoDraftSpan.TStateValueShow());

        desk.TEaselVideoRemove(video);

        Assert.Empty(desk.TDeskRead()!.LDraftSituation!.LSituationVideo);
    }
}
