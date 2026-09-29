using System.Windows.Controls;

namespace Llyn.UIDeportment;

public partial class PSettings
{
    private void PLocalizationObserve(object sender, SelectionChangedEventArgs e)
    {
        if (PLocalization.SelectedValue is not string language)
        {
            return;
        }

        PSettingsAtelier.CAtelierLedger.CLedgerLocalizationSave(language);
    }
}
