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
        LEngineStateChange(state => state with { LWorkspaceStateLeftEntryId = id });
    }

    public void LEngineRightSave(long? id)
    {
        LEngineStateChange(state => state with { LWorkspaceStateRightEntryId = id });
    }

    public void LEngineModeSave(string mode)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(mode);
        LEngineStateChange(state => state with { LWorkspaceStateMode = mode });
    }

    public void LEngineSplitSave(bool split)
    {
        LEngineStateChange(state => state with { LWorkspaceStateSplit = split });
    }

    public void LEngineOrderSave(LCatalogOrder order)
    {
        LEngineStateChange(state => state with { LWorkspaceStateOrder = order });
    }

    public void LEngineSequenceSave(LCatalogOrder order)
    {
        LEngineStateChange(state => state with { LWorkspaceStateSequence = order });
    }

    public void LEngineSeriesSave(LCatalogOrder order)
    {
        LEngineStateChange(state => state with { LWorkspaceStateSeries = order });
    }

    public void LEngineFunnelSave(LCatalogOrder order)
    {
        LEngineStateChange(state => state with { LWorkspaceStateFunnel = order });
    }

    public void LEngineDegreeSave(LCatalogOrder order)
    {
        LEngineStateChange(state => state with { LWorkspaceStateDegree = order });
    }

    public void LEngineTierSave(LCatalogOrder order)
    {
        LEngineStateChange(state => state with { LWorkspaceStateTier = order });
    }

    public void LEngineGradeSave(LCatalogOrder order)
    {
        LEngineStateChange(state => state with { LWorkspaceStateGrade = order });
    }

    public void LEngineRankSave(LCatalogOrder order)
    {
        LEngineStateChange(state => state with { LWorkspaceStateRank = order });
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
