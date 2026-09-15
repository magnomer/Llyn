using System.Windows.Controls;

namespace Llyn.UIShell;

public partial class PSettings
{
    private void PLocalizationHandle(object sender, SelectionChangedEventArgs e)
    {
        if (!_pSettingsReady || PLocalization.SelectedValue is not string language)
        {
            return;
        }

        PLocalizationApply(language);
        _lEngine.LEngineLocalizationSave(language);
    }

    private void PLocalizationApply(string language)
    {
        PLocalizationLoader.PLocalizationLoaderApply(System.Windows.Application.Current.Resources, language);
        PLedgerTitleApply();
        PLedgerMetaApply();
    }
}
