using Llyn.Application;
using Llyn.Core;
using Xunit;

namespace Llyn.Tests;

public sealed class TTrailClerk
{
    [Fact]
    public void TrailPathValidate_OutsideWorkspace_Refuses()
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

    [Fact]
    public void TrailPathValidate_InsideWorkspace_ReturnsRelativePath()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        LTrailClerk clerk = TInterface.TTrailClerkCreate(workspace.TWorkspaceRigCreate());

        Assert.Equal(
            Path.Combine("sync", "phone"),
            clerk.TTrailPathValidate(Path.Combine(workspace.TWorkspaceFolder, "sync", "phone")));
    }

    [Fact]
    public void TrailFileResolve_RelativePath_AnswersAbsoluteFileUri()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        LTrailClerk clerk = TInterface.TTrailClerkCreate(workspace.TWorkspaceRigCreate());

        Uri? made = clerk.TTrailFileResolve(Path.Combine("sync", "phone.txt"));

        Assert.NotNull(made);
        Assert.True(made.IsAbsoluteUri);
    }

    [Fact]
    public void TrailPathValidate_RelativeEscape_Refuses()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        LTrailClerk clerk = TInterface.TTrailClerkCreate(workspace.TWorkspaceRigCreate());

        LRefusal refusal = Assert.Throws<LRefusal>(() => clerk.TTrailPathValidate(Path.Combine("..", "phone")));

        Assert.Equal(LRefusal.LRefusalPathRooted, refusal.LRefusalReason);
    }
}
