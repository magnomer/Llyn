using System;
using Llyn.Core;

namespace Llyn.Tests;

internal sealed class TPressVault : LPressVault
{
    internal LPressSetting? TPressVaultSetting { get; set; }

    internal int TPressVaultCount { get; private set; }

    internal Exception? TPressVaultFailure { get; set; }

    public LPressSetting? LPressVaultRead() => TPressVaultSetting;

    public void LPressVaultSave(LPressSetting setting)
    {
        if (TPressVaultFailure is not null)
        {
            throw TPressVaultFailure;
        }

        TPressVaultCount++;
        TPressVaultSetting = setting;
    }
}
