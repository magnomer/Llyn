using System.Windows.Controls;

namespace Llyn.UIVeneer;

public partial class PSettings
{
    private void PLocalizationHandle(object sender, SelectionChangedEventArgs e)
    {
        if (PLocalization.SelectedValue is not string language)
        {
            return;
        }

        _lEngine.LEngineLocalizationSave(language);
    }

    private void PLocalizationApply(string language)
    {
        PLocalizationCatalog.PLocalizationCatalogApply(System.Windows.Application.Current.Resources, language);
        PLedgerTitleApply();
        PLedgerMetaApply();
    }
}
