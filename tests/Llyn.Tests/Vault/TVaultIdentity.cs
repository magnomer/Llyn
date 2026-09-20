using Llyn.Application;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TVaultIdentity
{
    [Fact]
    public void IdentityCreate_TwiceInOneSession_Descends()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        LIdentity identity = TInterface.TIdentityCreate(TInterface.TWorkspaceVaultCreate(workspace.TWorkspaceDatabase));

        long first = identity.TIdentityCreate();
        long second = identity.TIdentityCreate();

        Assert.True(second < first);
    }

    [Fact]
    public void IdentityCreate_AcrossSessions_NeverRepeats()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        LWorkspaceVault workspaces = TInterface.TWorkspaceVaultCreate(workspace.TWorkspaceDatabase);

        long earlier = TInterface.TIdentityCreate(workspaces).TIdentityCreate();
        long later = TInterface.TIdentityCreate(workspaces).TIdentityCreate();

        Assert.True(later < earlier);
        Assert.Equal(later, TInterface.TIdentityCreate(workspaces).LIdentityFloor);
    }
}
