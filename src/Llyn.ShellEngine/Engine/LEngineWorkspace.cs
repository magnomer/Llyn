using System;
using Llyn.Core;

namespace Llyn.ShellEngine;

public sealed partial class LEngine
{
    public LWorkspaceState LEngineStateRead()
    {
        lock (LEngineGate)
        {
            return _lEngineStaff.LEngineStaffWorkspace.LWorkspaceStateRead();
        }
    }

    public void LEngineLeftSave(long? id)
    {
        LEngineStateChange(state => state with { LWorkspaceStateLeft = id });
    }

    public void LEngineRightSave(long? id)
    {
        LEngineStateChange(state => state with { LWorkspaceStateRight = id });
    }

    private void LEngineStateChange(Func<LWorkspaceState, LWorkspaceState> change)
    {
        lock (LEngineGate)
        {
            LWorkspaceState current = _lEngineStaff.LEngineStaffWorkspace.LWorkspaceStateRead();
            _lEngineStaff.LEngineStaffWorkspace.LWorkspaceStateSave(change(current));
        }
    }

    public Uri? LEngineLocationResolve(string? location)
    {
        lock (LEngineGate)
        {
            return _lEngineStaff.LEngineStaffTrail.LTrailClerkResolve(location);
        }
    }

    public Uri? LEngineLocationRead(string? location)
    {
        lock (LEngineGate)
        {
            return _lEngineStaff.LEngineStaffTrail.LTrailClerkRead(location);
        }
    }

    public void LEngineLocationOpen(string target)
    {
        _lEngineStaff.LEngineStaffTrail.LTrailClerkOpen(target);
    }
}
