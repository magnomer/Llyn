using Llyn.Core;

namespace Llyn.Tests;

internal sealed class TVaultFakeWorkspace : LWorkspaceVault
{
    private LWorkspaceState _tVaultFakeState = new(1);

    public LWorkspaceState LWorkspaceStateRead() => _tVaultFakeState;

    public void LWorkspaceStateSave(LWorkspaceState state)
    {
        _tVaultFakeState = state;
    }

    public long LWorkspaceFloorAdjust()
    {
        _tVaultFakeState = _tVaultFakeState with { LWorkspaceStateFloor = _tVaultFakeState.LWorkspaceStateFloor - 1 };
        return _tVaultFakeState.LWorkspaceStateFloor;
    }

    public long LWorkspaceSizeRead() => 0;
}
