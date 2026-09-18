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
            return new LWorkspaceArchive(_lEngineDatabase).LWorkspaceStateRead();
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

    public void LEngineModeSave(string mode)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(mode);
        LEngineSettingsChange(settings => settings with { LSettingsMode = mode });
    }

    internal void LEngineSplitSave(bool split)
    {
        LEngineSettingsChange(settings => settings with { LSettingsSplit = split });
    }

    private void LEngineStateChange(Func<LWorkspaceState, LWorkspaceState> change)
    {
        lock (_lEngineGate)
        {
            LWorkspaceArchive workspace = new(_lEngineDatabase);
            workspace.LWorkspaceStateSave(change(workspace.LWorkspaceStateRead()));
        }
    }
}
