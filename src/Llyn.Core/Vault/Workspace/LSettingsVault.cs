namespace Llyn.Core;

public interface LSettingsVault
{
    bool LSettingsExist();

    LSettings LSettingsRead();

    void LSettingsSave(LSettings settings);
}
