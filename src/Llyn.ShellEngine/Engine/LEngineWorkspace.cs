using System;
using Llyn.Core;

namespace Llyn.ShellEngine;

public sealed partial class LEngine
{
    public LWorkspaceState LEngineStateRead()
    {
        lock (_lEngineGate)
        {
            return _lEngineWorkspaceClerk.LWorkspaceStateRead();
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
        lock (_lEngineGate)
        {
            _lEngineWorkspaceClerk.LWorkspaceStateSave(change(_lEngineWorkspaceClerk.LWorkspaceStateRead()));
        }
    }

    public Uri? LEngineLocationResolve(string? location)
    {
        lock (_lEngineGate)
        {
            return _lEngineTrailClerk.LTrailClerkResolve(location);
        }
    }

    public Uri? LEngineLocationRead(string? location)
    {
        lock (_lEngineGate)
        {
            return _lEngineTrailClerk.LTrailClerkRead(location);
        }
    }

    public void LEngineLocationOpen(string target)
    {
        _lEngineTrailClerk.LTrailClerkOpen(target);
    }
}
