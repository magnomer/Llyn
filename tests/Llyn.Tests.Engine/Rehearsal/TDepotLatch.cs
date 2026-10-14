using Llyn.Application;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TDepotLatch
{
    [Fact]
    public void WorkspaceStaffBuild_TwoWorkspaces_HoldsTwoLatches()
    {
        using TWorkspace first = TWorkspace.TWorkspacePrepare();
        using TWorkspace second = TWorkspace.TWorkspacePrepare();
        using LEngine firstEngine = first.TWorkspaceEngineStart();
        using LEngine secondEngine = second.TWorkspaceEngineStart();
        LDepotLatch busy = firstEngine.TDepotLatchRead();
        LDepotLatch idle = secondEngine.TDepotLatchRead();

        Assert.True(busy.TDepotLatchStart());

        Assert.NotSame(busy, idle);
        Assert.False(idle.TDepotLatchCheck());
        Assert.True(idle.TDepotLatchStart());
    }

    [Fact]
    public void DepotLatchStart_LatchStarted_ReturnsFalse()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LDepotLatch latch = engine.TDepotLatchRead();

        Assert.True(latch.TDepotLatchStart());
        Assert.False(latch.TDepotLatchStart());
        Assert.True(latch.TDepotLatchCheck());

        latch.TDepotLatchFinish();

        Assert.False(latch.TDepotLatchCheck());
        Assert.True(latch.TDepotLatchStart());
    }
}
