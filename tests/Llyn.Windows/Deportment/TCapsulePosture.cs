using Llyn.UIDeportment;
using Llyn.UIDeportment.Capsule;
using Xunit;

namespace Llyn.Tests;

public sealed class TCapsulePosture
{
    [Fact]
    public void PostureLayoutSave_TwoTabs_ReadsBackAfterReopen()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using (QPosture posture = TInterfaceDeportment.TPostureCreate(workspace.TWorkspaceFolder))
        {
            posture.QPostureLayoutSave([
                TInterfaceCapsule.TCapsuleColumnCreate("library", 400, null),
                TInterfaceCapsule.TCapsuleColumnCreate("corpus", 390, null)]);
        }

        using QPosture reopened = TInterfaceDeportment.TPostureCreate(workspace.TWorkspaceFolder);

        Assert.Equal(400, reopened.QPostureColumnRead("library")?.LCapsuleColumnLeft);
        Assert.Equal(390, reopened.QPostureColumnRead("corpus")?.LCapsuleColumnLeft);
        Assert.Null(reopened.QPostureColumnRead("tenor"));
    }

    [Fact]
    public void PostureLayoutReset_AfterSave_DropsWidthsKeepsTabs()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using QPosture posture = TInterfaceDeportment.TPostureCreate(workspace.TWorkspaceFolder);
        posture.QPostureLayoutSave([TInterfaceCapsule.TCapsuleColumnCreate("tenor", 320, 300)]);

        posture.QPostureLayoutReset();

        LCapsuleColumn tenor = Assert.IsType<LCapsuleColumn>(posture.QPostureColumnRead("tenor"));
        Assert.Null(tenor.LCapsuleColumnLeft);
        Assert.Null(tenor.LCapsuleColumnMiddle);
    }

    [Fact]
    public void PostureLinkedSave_False_ReportsChangeOnce()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using QPosture posture = TInterfaceDeportment.TPostureCreate(workspace.TWorkspaceFolder);

        Assert.True(posture.QPostureLinkedSave(false));
        Assert.False(posture.QPostureLinkedSave(false));
        Assert.False(posture.QPostureRead().LCapsuleContentLinked);
    }

    [Fact]
    public void PostureWindowDefer_AtOnceAndMinimized_KeepsOnlyTheShownWindow()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using QPosture posture = TInterfaceDeportment.TPostureCreate(workspace.TWorkspaceFolder);
        LCapsuleWindow shown = TInterfaceCapsule.TCapsuleWindowCreate(10, 20, 800, 600, true);

        posture.QPostureWindowDefer(shown, false, 0);
        posture.QPostureWindowDefer(TInterfaceCapsule.TCapsuleWindowCreate(0, 0, 10, 10, false), true, 0);

        Assert.Equal(shown, TInterfaceCapsule.TCapsuleRead(workspace.TWorkspaceFolder).LCapsuleContentWindow);
    }
}
