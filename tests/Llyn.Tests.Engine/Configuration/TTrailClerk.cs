using Llyn.Application;
using Llyn.Core;
using Xunit;

namespace Llyn.Tests;

public sealed class TTrailClerk
{
    [Fact]
    public void TrailPathValidate_AbsolutePath_Refuses()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        LTrailClerk clerk = TInterface.TTrailClerkCreate(workspace.TWorkspaceRigCreate());

        LRefusal refusal = Assert.Throws<LRefusal>(
            () => clerk.TTrailPathValidate(Path.Combine(Path.GetTempPath(), "phone")));

        Assert.Equal(LRefusal.LRefusalPathRooted, refusal.LRefusalReason);
    }

    [Fact]
    public void TrailPathValidate_RelativePath_ReturnsPath()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        LTrailClerk clerk = TInterface.TTrailClerkCreate(workspace.TWorkspaceRigCreate());
        string folder = Path.Combine("sync", "phone");

        Assert.Equal(folder, clerk.TTrailPathValidate(folder));
    }
}
