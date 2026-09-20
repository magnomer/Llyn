namespace Llyn.Core;

public interface LVault
{
    LVaultSession LVaultSessionStart();

    bool LVaultMigrated { get; }
}
