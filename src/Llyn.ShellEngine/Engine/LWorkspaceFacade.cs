using System;
using Llyn.Core;

namespace Llyn.ShellEngine;

internal sealed class LWorkspaceFacade
{
    private readonly LEngine _lWorkspaceFacadeEngine;
    private readonly object _lWorkspaceFacadeGate;

    public LWorkspaceFacade(LEngine engine)
    {
        ArgumentNullException.ThrowIfNull(engine);
        _lWorkspaceFacadeEngine = engine;
        _lWorkspaceFacadeGate = engine.LEngineGate;
    }

    private LEngineStaff LWorkspaceFacadeStaff => _lWorkspaceFacadeEngine.LEngineStaffHeld;

    public LWorkspaceState LEngineStateRead()
    {
        lock (_lWorkspaceFacadeGate)
        {
            return LWorkspaceFacadeStaff.LEngineStaffWorkspace.LWorkspaceStateRead();
        }
    }

    public void LEngineLeftSave(long? id)
    {
        LWorkspaceStateChange(state => state with { LWorkspaceStateLeft = id });
    }

    public void LEngineRightSave(long? id)
    {
        LWorkspaceStateChange(state => state with { LWorkspaceStateRight = id });
    }

    private void LWorkspaceStateChange(Func<LWorkspaceState, LWorkspaceState> change)
    {
        lock (_lWorkspaceFacadeGate)
        {
            LWorkspaceState current = LWorkspaceFacadeStaff.LEngineStaffWorkspace.LWorkspaceStateRead();
            LWorkspaceFacadeStaff.LEngineStaffWorkspace.LWorkspaceStateSave(change(current));
        }
    }

    public Uri? LEngineLocationResolve(string? location)
    {
        lock (_lWorkspaceFacadeGate)
        {
            return LWorkspaceFacadeStaff.LEngineStaffTrail.LTrailClerkResolve(location);
        }
    }

    public Uri? LEngineLocationRead(string? location)
    {
        lock (_lWorkspaceFacadeGate)
        {
            return LWorkspaceFacadeStaff.LEngineStaffTrail.LTrailClerkRead(location);
        }
    }

    public void LEngineLocationOpen(string target)
    {
        LWorkspaceFacadeStaff.LEngineStaffTrail.LTrailClerkOpen(target);
    }
}
