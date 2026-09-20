using System;
using System.Collections.Generic;
using Llyn.Core;

namespace Llyn.Application;

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

    public static void LIdentityRecord(Dictionary<long, long> identity, long draftId, long rowId)
    {
        ArgumentNullException.ThrowIfNull(identity);

        if (draftId < 0)
        {
            identity[draftId] = rowId;
        }
    }
}
