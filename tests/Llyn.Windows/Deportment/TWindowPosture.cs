using System.Collections.Generic;
using Llyn.Core;
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

        window.TWindowLayoutSave(TInterface.TLayoutCreate("library", 400), TInterface.TLayoutCreate("corpus", 390));

        IReadOnlyList<LLayout> layout = window.TWindowPostureRead().LPostureStateLayout ?? [];
        Assert.Equal(2, layout.Count);
        Assert.Contains(layout, tab => tab.LLayoutTab == "library" && tab.LLayoutLeft == 400);
        Assert.Contains(layout, tab => tab.LLayoutTab == "corpus" && tab.LLayoutLeft == 390);
    }

    [Fact]
    public void WindowModeSave_Name_ReadsBackAndMatches()
    {
        using LEngine engine = new(TRigFake.TRigFakeBuild());
        LWindow window = TInterfaceDeportment.TWindowCreate(engine);

        window.TWindowModeSave("Corpus");

        Assert.Equal("Corpus", window.TWindowPostureRead().LPostureStateMode);
        Assert.True(window.TWindowModeMatch("Corpus"));
        Assert.False(window.TWindowModeMatch("Library"));
    }

    [Fact]
    public void WindowLayoutReset_AfterSave_DropsWidthsKeepsTabs()
    {
        using LEngine engine = new(TRigFake.TRigFakeBuild());
        LWindow window = TInterfaceDeportment.TWindowCreate(engine);
        window.TWindowLayoutSave(TInterface.TLayoutCreate("tenor", 320, 300));

        window.TWindowLayoutReset();

        IReadOnlyList<LLayout> layout = window.TWindowPostureRead().LPostureStateLayout ?? [];
        LLayout tenor = Assert.Single(layout);
        Assert.Equal("tenor", tenor.LLayoutTab);
        Assert.Null(tenor.LLayoutLeft);
        Assert.Null(tenor.LLayoutMiddle);
    }
}
