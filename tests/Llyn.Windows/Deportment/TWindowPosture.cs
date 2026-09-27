using Llyn.Conduct;
using Llyn.ShellEngine;
using Llyn.UIDeportment;
using Xunit;

namespace Llyn.Tests;

public sealed class TWindowPosture
{
    [Fact]
    public void WindowLayoutSave_TwoTabs_ReadsBackThroughPosture()
    {
        using LEngine engine = new(TRigFake.TRigFakeBuild());
        LWindow window = TInterfaceDeportment.TWindowCreate(engine);

        window.TWindowLayoutSave(new CLayout("library", 400, null), new CLayout("corpus", 390, null));

        Assert.Equal(400, window.TWindowLayoutRead("library")?.CLayoutLeft);
        Assert.Equal(390, window.TWindowLayoutRead("corpus")?.CLayoutLeft);
        Assert.Null(window.TWindowLayoutRead("tenor"));
    }

    [Fact]
    public void WindowModeSave_Name_Matches()
    {
        using LEngine engine = new(TRigFake.TRigFakeBuild());
        LWindow window = TInterfaceDeportment.TWindowCreate(engine);

        window.TWindowModeSave("Corpus");

        Assert.True(window.TWindowModeMatch("Corpus"));
        Assert.False(window.TWindowModeMatch("Library"));
    }

    [Fact]
    public void WindowLayoutReset_AfterSave_DropsWidthsKeepsTabs()
    {
        using LEngine engine = new(TRigFake.TRigFakeBuild());
        LWindow window = TInterfaceDeportment.TWindowCreate(engine);
        window.TWindowLayoutSave(new CLayout("tenor", 320, 300));

        window.TWindowLayoutReset();

        CLayout tenor = Assert.IsType<CLayout>(window.TWindowLayoutRead("tenor"));
        Assert.Equal("tenor", tenor.CLayoutTab);
        Assert.Null(tenor.CLayoutLeft);
        Assert.Null(tenor.CLayoutMiddle);
    }

    [Fact]
    public void WindowPostureRead_FreshEngine_CopiesTheDefaultPosture()
    {
        using LEngine engine = new(TRigFake.TRigFakeBuild());
        LWindow window = TInterfaceDeportment.TWindowCreate(engine);

        CPostureState posture = window.TWindowPostureRead();

        Assert.Null(posture.CPostureStateWindow);
        Assert.True(posture.CPostureStateLinked);
        Assert.False(posture.CPostureStateSplit);
        Assert.Equal(1, posture.CPostureStateVolume);
    }
}
