using System;
using Llyn.Infrastructure;

namespace Llyn.ShellEngine;

public sealed class LIdentity
{
    private readonly LWorkspaceArchive _lIdentityArchive;

    public LIdentity(LDatabase database)
    {
        ArgumentNullException.ThrowIfNull(database);
        _lIdentityArchive = new LWorkspaceArchive(database);
    }

    public long LIdentityFloor => _lIdentityArchive.LWorkspaceStateRead().LWorkspaceStateIdentityFloor;

    public long LIdentityCreate()
    {
        return _lIdentityArchive.LWorkspaceFloorLower();
    }
}
