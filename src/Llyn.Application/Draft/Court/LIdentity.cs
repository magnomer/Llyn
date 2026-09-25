using System;
using System.Collections.Generic;
using Llyn.Core;

namespace Llyn.Application;

public sealed class LIdentity
{
    private readonly LWorkspaceVault _lIdentityWorkspaces;
    private readonly IReadOnlySet<long> _lIdentityRetired;

    public LIdentity(LWorkspaceVault workspaces, IReadOnlySet<long>? retired = null)
    {
        ArgumentNullException.ThrowIfNull(workspaces);
        _lIdentityWorkspaces = workspaces;
        _lIdentityRetired = retired ?? new HashSet<long>();
    }

    public long LIdentityCreate()
    {
        long id = _lIdentityWorkspaces.LWorkspaceFloorAdjust();
        while (_lIdentityRetired.Contains(id))
        {
            id = _lIdentityWorkspaces.LWorkspaceFloorAdjust();
        }

        return id;
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
