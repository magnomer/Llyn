using System;
using Llyn.Core;
using Llyn.Infrastructure;

namespace Llyn.ShellEngine;

public sealed partial class LEngine
{
    public LWorkspaceState LEngineStateRead()
    {
        lock (_lEngineGate)
        {
            return _lEngineWorkspaces.LWorkspaceStateRead();
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
            _lEngineWorkspaces.LWorkspaceStateSave(change(_lEngineWorkspaces.LWorkspaceStateRead()));
        }
    }
}
