namespace Llyn.Core;

public interface LPostureVault
{
    LPostureState? LPostureRead(string name);

    void LPostureSave(string name, LPostureState state);
}
