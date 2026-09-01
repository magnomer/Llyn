using System.Windows.Controls;
using Llyn.Core;

namespace Llyn.UIShell;

/// <summary>
/// The interface language the user picks in the settings panel: the chosen catalog is applied to the
/// live resource dictionary at once, and kept as a setting so the next run opens in it.
/// </summary>
public partial class PSettings
{
    private void PLocalizationHandle(object sender, SelectionChangedEventArgs e)
    {
        if (PLocalization.SelectedValue is not string language)
        {
            return;
        }

        PLocalizationLoader.PLocalizationLoaderApply(System.Windows.Application.Current.Resources, language);

        // Skip persistence while the constructor is applying the stored choice; only user changes save.
        if (_pSettingsReady)
        {
            _lEngine.LEngineSettingsSave(new LSettings(language));
        }
    }
}
