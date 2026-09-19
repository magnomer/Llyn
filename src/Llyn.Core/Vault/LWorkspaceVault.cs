namespace Llyn.Core;

public interface LWorkspaceVault
{
    LWorkspaceState LWorkspaceStateRead();

    void LWorkspaceStateSave(LWorkspaceState state);

    long LWorkspaceFloorAdjust();
}
