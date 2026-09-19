using System;
using Llyn.Core;

namespace Llyn.ShellEngine;

public sealed class LIdentity
{
    private readonly LWorkspaceVault _lIdentityWorkspaces;

    public LIdentity(LWorkspaceVault workspaces)
    {
        ArgumentNullException.ThrowIfNull(workspaces);
        _lIdentityWorkspaces = workspaces;
    }

    public long LIdentityFloor => _lIdentityWorkspaces.LWorkspaceStateRead().LWorkspaceStateFloor;

    public long LIdentityCreate()
    {
        return _lIdentityWorkspaces.LWorkspaceFloorAdjust();
    }
}
