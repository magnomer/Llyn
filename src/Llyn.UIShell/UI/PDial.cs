using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace Llyn.UIShell;

public partial class PSettings
{
    private void PDialShow(string child)
    {
        foreach (PLedgerItem item in _pLedgerList)
        {
            item.PLedgerItemChosen = string.Equals(item.PLedgerItemChild, child, StringComparison.Ordinal);
        }

        foreach (Border card in new[] { PDialWorkspace, PDialLanguage, PDialTranscription, PDialWeb, PDialLayout })
        {
            card.Visibility = string.Equals(card.Name, "PDial" + child, StringComparison.Ordinal)
                ? Visibility.Visible
                : Visibility.Collapsed;
        }
    }

    private void PDialFolderHandle(object sender, RoutedEventArgs e)
    {
        Process.Start(new ProcessStartInfo(_lEngine.LEngineWorkspaceRead()) { UseShellExecute = true });
    }

    private void PDialWidthHandle(object sender, RoutedEventArgs e)
    {
        _lEngine.LEngineLayoutReset();
        _pSettingsHost.PWindowLayout.PLayoutReset();
    }
}
