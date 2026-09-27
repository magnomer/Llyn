using System.Windows.Controls;

namespace Llyn.UIDeportment;

public partial class PSettings
{
    private void PLocalizationHandle(object sender, SelectionChangedEventArgs e)
    {
        if (PLocalization.SelectedValue is not string language)
        {
            return;
        }

        PSettingsWindow.LWindowWorkspace.QWorkspaceLocalizationSave(language);
    }

    private void PLocalizationApply(string language)
    {
        QLocalizationCatalog.QLocalizationCatalogApply(
            System.Windows.Application.Current.Resources,
            PSettingsWindow.LWindowWorkspace.QWorkspaceLocalizationLoad(language));
        PLedgerTitleApply();
        PLedgerMetaApply();
    }
}
