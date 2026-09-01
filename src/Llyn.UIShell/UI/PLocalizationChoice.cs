using System.Windows.Controls;
using Llyn.Core;

namespace Llyn.UIShell;

public partial class PSettings
{
    private void PLocalizationHandle(object sender, SelectionChangedEventArgs e)
    {
        if (PLocalization.SelectedValue is not string language)
        {
            return;
        }

        PLocalizationLoader.PLocalizationLoaderApply(System.Windows.Application.Current.Resources, language);

        if (_pSettingsReady)
        {
            _lEngine.LEngineSettingsSave(new LSettings(language));
        }
    }
}
