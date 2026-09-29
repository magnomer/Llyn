using System;
using System.Windows;
using System.Windows.Controls;

namespace Llyn.UIDeportment;

public partial class PSettings
{
    private (string PDialChild, StackPanel PDialPage)[] PDialTableRead()
    {
        return
        [
            ("Workspace", PDialWorkspace),
            ("Language", PDialLanguage),
            ("Transcription", PDialTranscription),
            ("Listing", PDialListing),
            ("Web", PDialWeb),
            ("Layout", PDialLayout)
        ];
    }

    private void PDialRefine(string child)
    {
        foreach (PLedgerItem item in _pLedgerList)
        {
            item.PLedgerItemChosen = string.Equals(item.PLedgerItemChild, child, StringComparison.Ordinal);
        }

        foreach ((string name, StackPanel page) in PDialTableRead())
        {
            page.Visibility = string.Equals(name, child, StringComparison.Ordinal)
                ? Visibility.Visible
                : Visibility.Collapsed;
        }
    }

    private void PDialFolderObserve(object sender, RoutedEventArgs e)
    {
        PSettingsAtelier.CAtelierLedger.CLedgerFolderOpen(_pSettingsHost.PWindowEnvoy);
    }

    private void PDialWidthRefine(object sender, RoutedEventArgs e)
    {
        PSettingsPosture.QPostureLayoutReset();
    }
}
