namespace Llyn.Core;

public interface LPressVault
{
    LPressSetting? LPressVaultRead();

    void LPressVaultSave(LPressSetting setting);
}
