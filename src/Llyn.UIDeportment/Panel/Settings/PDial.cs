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

    private void PDialShow(string child)
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

    private void PDialFolderHandle(object sender, RoutedEventArgs e)
    {
        try
        {
            PSettingsAtelier.CAtelierLocationOpen(_pSettingsState.CLedgerStatePath);
        }
        catch (Exception exception)
        {
            _pSettingsHost.PWindowFailureShow("Settings.FolderFailed", exception);
        }
    }

    private void PDialWidthHandle(object sender, RoutedEventArgs e)
    {
        PSettingsPosture.QPostureLayoutReset();
    }
}
